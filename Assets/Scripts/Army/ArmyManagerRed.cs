using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.AI;

public class ArmyManagerRed : ArmyManager
{
	[SerializeField]
	private float m_StealHealthThreshold = 30f; // health below which stealing is allowed (increased for more aggressive stealing)

	[SerializeField]
	private float m_StealCooldown = 0.8f; // seconds per locker to avoid steal oscillation (reduced for faster adaptation)

	[SerializeField]
	private float m_FocusFireThreshold = 50f; // when target below this, all nearby units should focus it

	[SerializeField]
	private float m_CoordinationRadius = 25f; // radius for unit coordination

	// track last steal time per locker
	private Dictionary<GameObject, float> m_LastStealTime = new Dictionary<GameObject, float>();
	
	// track focus fire targets
	private Dictionary<GameObject, float> m_FocusFireTargets = new Dictionary<GameObject, float>();

	public override void ArmyElementHasBeenKilled(GameObject go)
	{
		base.ArmyElementHasBeenKilled(go);
		if (m_ArmyElements.Count == 0)
		{
			GUIUtility.systemCopyBuffer = "0\t" +((int)Timer.Value).ToString()+"\t0\t0\t0";
		}
	}

	// Stronger selection for red army: prefer turrets first, then drones; try to focus fire on low-health targets
	public override GameObject LockArmyElementOnRandomNonTargetedEnemy<T>(GameObject locker, Vector3 centerPos, float minRadius, float maxRadius)
	{
		// purge stale mappings (keys or values that were destroyed)
		var stale = m_DicoWhoTargetsWhom.Where(kv => kv.Key == null || kv.Value == null).Select(kv => kv.Key).ToList();
		foreach (var k in stale) if (k != null && m_DicoWhoTargetsWhom.ContainsKey(k)) m_DicoWhoTargetsWhom.Remove(k);

		// If already locked, check if we should keep it or steal a better target
		if (locker != null && m_DicoWhoTargetsWhom.ContainsKey(locker) && m_DicoWhoTargetsWhom[locker] != null)
		{
			var currentTarget = m_DicoWhoTargetsWhom[locker].GetComponent<IArmyElement>();
			// Keep current target if it's still weak (focus fire strategy)
			if (currentTarget != null && currentTarget.Health <= m_StealHealthThreshold)
			{
				Debug.Log($"[AI-STRATEGY] {SafeName(locker)} maintaining focus on weakened target {SafeName(m_DicoWhoTargetsWhom[locker])} ({currentTarget.Health} HP)");
				return m_DicoWhoTargetsWhom[locker];
			}
		}

		// STRATEGY 1: Check for focus fire opportunities (very weak enemies that need to be finished)
		var criticalTargets = GetAllEnemiesOfType<ArmyElement>(false)
			.Where(e => e.Health <= m_FocusFireThreshold / 2f // Critical health
				&& Vector3.Distance(centerPos, e.transform.position) > minRadius
				&& Vector3.Distance(centerPos, e.transform.position) < maxRadius)
			.OrderBy(e => e.Health)
			.ToList();

		if (criticalTargets.Count > 0)
		{
			var critical = criticalTargets.First();
			if (locker != null && m_DicoWhoTargetsWhom.ContainsKey(locker))
			{
				m_DicoWhoTargetsWhom[locker] = critical.gameObject;
			}
			else if (locker != null)
			{
				m_DicoWhoTargetsWhom[locker] = critical.gameObject;
			}
			Debug.Log($"[AI-CRITICAL] {SafeName(locker)} engaging critical target {SafeName(critical.gameObject)} with {critical.Health} HP!");
			return critical.gameObject;
		}

		// STRATEGY 2: Prefer high-value targets (turrets) - they're static and dangerous
		var prioritizedTypes = new System.Type[] { typeof(Turret), typeof(Drone) };

		 // search prioritized types first
		foreach (var t in prioritizedTypes)
		{
			if (!typeof(T).IsAssignableFrom(t)) continue; // only consider if compatible
			
			var enemies = GetAllEnemiesOfType<ArmyElement>(true).Where(item => !m_DicoWhoTargetsWhom.ContainsValue(item.gameObject)
				&& Vector3.Distance(centerPos, item.transform.position) > minRadius
				&& Vector3.Distance(centerPos, item.transform.position) < maxRadius
				&& item.GetType() == t)
				.OrderBy(item => item.Health) // Prefer weakest first
				.ThenBy(item => Vector3.Distance(centerPos, item.transform.position)) // Then closest
				.ToList();
			
			if (enemies.Count > 0)
			{
				var chosen = enemies.First().gameObject;
				m_DicoWhoTargetsWhom[locker] = chosen;
				Debug.Log($"[AI-PRIORITY] {SafeName(locker)} selected priority {t.Name}: {SafeName(chosen)}");
				return chosen;
			}
		}


		// STRATEGY 3: Aggressive target stealing - if no free target, steal from weakest enemy
		float stealHealthThreshold = m_StealHealthThreshold;
		foreach (var t in prioritizedTypes)
		{
			if (!typeof(T).IsAssignableFrom(t)) continue;
			// look for already-targeted enemies of type t with low health within range
			var targeted = m_DicoWhoTargetsWhom.Values.Where(v => v != null && v.GetComponent<ArmyElement>() != null && v.GetComponent<ArmyElement>().GetType() == t)
				.Select(v => v.GetComponent<ArmyElement>())
					.Where(ae => Vector3.Distance(centerPos, ae.transform.position) > minRadius && Vector3.Distance(centerPos, ae.transform.position) < maxRadius)
					.OrderBy(ae => ae.Health).ThenBy(ae => Vector3.Distance(centerPos, ae.transform.position)).ToList();

			if (targeted.Count > 0)
			{
				var candidate = targeted.First();
				if (candidate.Health <= stealHealthThreshold)
				{
					// anti-churn: check last steal time for this locker
					if (locker != null && m_LastStealTime.TryGetValue(locker, out float last) && Time.time - last < m_StealCooldown)
					{
						// too soon to steal again
						continue;
					}
					// find the locker currently targeting this candidate
					var prevLocker = m_DicoWhoTargetsWhom.FirstOrDefault(kv => kv.Value == candidate.gameObject).Key;
					if (prevLocker != null && prevLocker != locker) // Don't steal from self
					{
						// reassign: stealer gets the target, previous locker loses mapping
						m_DicoWhoTargetsWhom.Remove(prevLocker);
						m_DicoWhoTargetsWhom[locker] = candidate.gameObject;
						// register steal times to prevent immediate re-steal
						m_LastStealTime[locker] = Time.time;
						m_LastStealTime[prevLocker] = Time.time;
						Debug.Log($"[AI-STEAL] {SafeName(locker)} stole weakened target {SafeName(candidate.gameObject)} ({candidate.Health} HP) from {SafeName(prevLocker)}");
						return candidate.gameObject;
					}
				}
			}
		}

		// STRATEGY 4: Allow multiple units to target the same weak enemy (focus fire)
		foreach (var t in prioritizedTypes)
		{
			if (!typeof(T).IsAssignableFrom(t)) continue;
			
			var weakTargeted = m_DicoWhoTargetsWhom.Values
				.Where(v => v != null && v.GetComponent<ArmyElement>() != null && v.GetComponent<ArmyElement>().GetType() == t)
				.Select(v => v.GetComponent<ArmyElement>())
				.Where(ae => ae.Health <= m_FocusFireThreshold
					&& Vector3.Distance(centerPos, ae.transform.position) > minRadius 
					&& Vector3.Distance(centerPos, ae.transform.position) < maxRadius)
				.OrderBy(ae => ae.Health)
				.ToList();

			if (weakTargeted.Count > 0)
			{
				var focusTarget = weakTargeted.First();
				// Count how many units already targeting this
				int currentFocusCount = m_DicoWhoTargetsWhom.Values.Count(v => v == focusTarget.gameObject);
				
				// Allow up to 3 units to focus fire on weak targets
				if (currentFocusCount < 3)
				{
					m_DicoWhoTargetsWhom[locker] = focusTarget.gameObject;
					Debug.Log($"[AI-FOCUS] {SafeName(locker)} joining focus fire on {SafeName(focusTarget.gameObject)} ({focusTarget.Health} HP) - {currentFocusCount + 1} units targeting");
					return focusTarget.gameObject;
				}
			}
		}

		// fallback to base implementation
		return base.LockArmyElementOnRandomNonTargetedEnemy<T>(locker, centerPos, minRadius, maxRadius);
	}
	public void GreenArmyIsDead(string deadArmyTag)
    {
        int nDrones = 0, nTurrets = 0, health = 0;
        ComputeStatistics(ref nDrones, ref nTurrets, ref health);
		GUIUtility.systemCopyBuffer = "1\t" + ((int)Timer.Value).ToString() + "\t"+nDrones.ToString()+"\t"+nTurrets.ToString()+"\t"+health.ToString();
		
		RefreshHudDisplay(); //pour une derni�re mise � jour en cas de victoire
	}

}