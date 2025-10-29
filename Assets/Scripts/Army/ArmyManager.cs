using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Events;

/*
Préparer un terrain où toutes les terrasses sont accessibles
*/

public abstract class ArmyManager : MonoBehaviour
{
	[SerializeField] string m_ArmyTag;
	[SerializeField] Color m_ArmyColor;
	protected List<IArmyElement> m_ArmyElements = new List<IArmyElement>();

	[SerializeField] TMP_Text m_NDronesText;
	[SerializeField] TMP_Text m_NTurretsText;
	[SerializeField] TMP_Text m_HealthText;

	[SerializeField] UnityEvent m_OnArmyIsDead;

	// mapping locker -> target to avoid multiple friendlies selecting the same target
	protected Dictionary<GameObject, GameObject> m_DicoWhoTargetsWhom = new Dictionary<GameObject, GameObject>();

	protected List<T> GetAllEnemiesOfType<T>(bool sortRandom) where T : ArmyElement
	{
	var enemies = Object.FindObjectsByType<T>(UnityEngine.FindObjectsSortMode.None).Where(element => !element.gameObject.CompareTag(m_ArmyTag)).ToList();
		if (sortRandom) enemies.Sort((a, b) => Random.value.CompareTo(.5f));
		return enemies;
	}

	public GameObject GetRandomEnemy<T>(Vector3 centerPos, float minRadius, float maxRadius) where T : ArmyElement
	{
		var enemies = GetAllEnemiesOfType<T>(true).Where(
			item=>  Vector3.Distance(centerPos,item.transform.position)>minRadius
					&& Vector3.Distance(centerPos, item.transform.position) < maxRadius);

		return enemies.FirstOrDefault()?.gameObject;
	}

	// Return a random enemy of type T that is not currently targeted by another friendly unit
	public virtual GameObject GetRandomNonTargetedEnemy<T>(Vector3 centerPos, float minRadius, float maxRadius) where T : ArmyElement
	{
		var enemies = GetAllEnemiesOfType<T>(true).Where(
			item=>  Vector3.Distance(centerPos,item.transform.position)>minRadius
					&& Vector3.Distance(centerPos, item.transform.position) < maxRadius);

		var candidate = enemies.Where(item => !m_DicoWhoTargetsWhom.ContainsValue(item.gameObject)).FirstOrDefault()?.gameObject;
		if (candidate != null) Debug.Log($"[AI-LOCK] GetRandomNonTargetedEnemy found {SafeName(candidate)} for requester around {centerPos}");
		else Debug.Log($"[AI-LOCK] GetRandomNonTargetedEnemy found NO candidate around {centerPos}");
		return candidate;
	}

	// Lock a friendly locker on a non-targeted enemy (or return an already locked one). The default implementation records the mapping.
	public virtual GameObject LockArmyElementOnRandomNonTargetedEnemy<T>(GameObject locker, Vector3 centerPos, float minRadius, float maxRadius) where T : ArmyElement
	{
		// if already locked, return same target
		if (locker != null && m_DicoWhoTargetsWhom.ContainsKey(locker) && m_DicoWhoTargetsWhom[locker] != null)
		{
			Debug.Log($"[AI-LOCK] Locker {SafeName(locker)} already locked on {SafeName(m_DicoWhoTargetsWhom[locker])}");
			return m_DicoWhoTargetsWhom[locker];
		}

		GameObject rndGO = GetRandomNonTargetedEnemy<T>(centerPos, minRadius, maxRadius);
		if (rndGO)
		{
			if (locker != null)
			{
				m_DicoWhoTargetsWhom[locker] = rndGO;
				Debug.Log($"[AI-LOCK] Locker {SafeName(locker)} locked on {SafeName(rndGO)}");
			}
			else
			{
				Debug.Log($"[AI-LOCK] Attempted to lock null locker on {SafeName(rndGO)}");
			}
		}
		else
		{
			if (locker != null)
				Debug.Log($"[AI-LOCK] Locker {SafeName(locker)} found no target to lock");
		}
		return rndGO;
	}

	// Unlock when no longer targeting (unit died or changed state)
	public virtual void UnlockArmyElement(GameObject locker)
	{
		// Clean up any entries that reference destroyed objects first
		var toRemove = m_DicoWhoTargetsWhom.Where(kv => kv.Key == null || kv.Value == null).Select(kv => kv.Key).ToList();
		foreach (var k in toRemove)
		{
			Debug.Log($"[AI-LOCK] Cleaning stale mapping: locker={SafeName(k)}");
			m_DicoWhoTargetsWhom.Remove(k);
		}

		if (locker == null)
		{
			Debug.Log("[AI-LOCK] UnlockArmyElement called with null locker");
			return;
		}

		GameObject target;
		if (m_DicoWhoTargetsWhom.TryGetValue(locker, out target))
		{
			Debug.Log($"[AI-LOCK] Unlocking for {SafeName(locker)} (was targeting {SafeName(target)})");
			m_DicoWhoTargetsWhom.Remove(locker);
		}
		else
		{
			Debug.Log($"[AI-LOCK] Unlock requested for {SafeName(locker)} but no mapping found");
		}
	}

	protected void ComputeStatistics(ref int nDrones,ref int nTurrets,ref int cumulatedHealth)
	{
		nDrones = m_ArmyElements.Count(item => item is Drone);
		nTurrets = m_ArmyElements.Count(item => item is Turret);
		cumulatedHealth = (int)m_ArmyElements.Sum(item => item.Health);
	}

	// Start is called before the first frame update
	public virtual IEnumerator Start()
	{
		yield return null; // on attend une frame que tous les objets aient été instanciés ...

		GameObject[] allArmiesElements = GameObject.FindGameObjectsWithTag(m_ArmyTag);
		foreach (var item in allArmiesElements)
		{
			IArmyElement armyElement = item.GetComponent<IArmyElement>();
			armyElement.ArmyManager = this;
			m_ArmyElements.Add(armyElement);
		}

		RefreshHudDisplay();

		yield break;
	}

	protected void RefreshHudDisplay()
	{
		int nDrones=0, nTurrets=0, health=0;
		ComputeStatistics(ref nDrones, ref nTurrets, ref health);

		if (m_NDronesText) m_NDronesText.text = nDrones.ToString();
		if (m_NTurretsText) m_NTurretsText.text = nTurrets.ToString();
		if (m_HealthText) m_HealthText.text = health.ToString();
	}

	public virtual void ArmyElementHasBeenKilled(GameObject go)
	{
		if (go != null)
			m_ArmyElements.Remove(go.GetComponent<IArmyElement>());
		RefreshHudDisplay();

		if (m_ArmyElements.Count == 0 & m_OnArmyIsDead!=null) m_OnArmyIsDead.Invoke();
	}

	// Small helper that returns a safe name for a UnityEngine.Object that may have been destroyed
	protected string SafeName(UnityEngine.Object obj)
	{
		if (obj == null) return "<null>";
		// avoid accessing properties that may throw on destroyed objects
		try
		{
			return obj.name;
		}
		catch
		{
			return "<destroyed>";
		}
	}

}

