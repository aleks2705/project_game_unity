using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArmyManagerRed : ArmyManager
{
    [SerializeField] int maxDroneAttackers = 4;
    [SerializeField] int maxTurretAttackers = 3;
    
    private Dictionary<GameObject, GameObject> m_DicoWhoTargetsWhom = new Dictionary<GameObject, GameObject>();
    private GameObject currentDroneFocusTarget = null;

    public override GameObject GetRandomEnemy<T>(GameObject attacker, Vector3 centerPos, float minRadius, float maxRadius)
    {
		if (typeof(T) == typeof(Drone))
        {
            if (GetAllEnemiesOfType<Turret>(false).Count > 0)
            {
                return null;
            }
        }
        // --- C'EST TA LOGIQUE DE STRATÉGIE ---

        // CAS 1: UN DRONE ROUGE ATTAQUE UN DRONE VERT (Plan B de l'arbre)
        if (attacker.GetComponent<Drone>() != null && typeof(T) == typeof(Drone))
        {
            // STRATÉGIE: "TOUS SUR LE PLUS FAIBLE"
            if (currentDroneFocusTarget != null && currentDroneFocusTarget.activeSelf)
            {
                return currentDroneFocusTarget;
            }
            
            var enemiesDrone = GetAllEnemiesOfType<T>(false) // false = pas aléatoire
                .Where(item => 
                    Vector3.Distance(centerPos, item.transform.position) > minRadius && 
                    Vector3.Distance(centerPos, item.transform.position) < maxRadius
                )
                .OrderBy(e => e.GetComponent<IArmyElement>().Health)
                .ToList();
            
            currentDroneFocusTarget = enemiesDrone.FirstOrDefault()?.gameObject;
            return currentDroneFocusTarget;
        }

        // CAS 2: TOUT LE RESTE (Drones sur Tourelles, Tourelles sur Drones, Tourelles sur Tourelles)
        // STRATÉGIE: "GROUPES DE 3 ou 4"
        
        int maxAttackersPerTarget;
        if (attacker.GetComponent<Drone>() != null)
            maxAttackersPerTarget = maxDroneAttackers; // Groupes de 4
        else
            maxAttackersPerTarget = maxTurretAttackers; // Groupes de 3

        Dictionary<GameObject, int> targetAttackerCount = new Dictionary<GameObject, int>();
        foreach (GameObject target in m_DicoWhoTargetsWhom.Values)
        {
            if (target != null)
            {
                if (!targetAttackerCount.ContainsKey(target))
                    targetAttackerCount[target] = 0;
                targetAttackerCount[target]++;
            }
        }

        var allEnemies = GetAllEnemiesOfType<T>(false) 
            .OrderBy(e => e.GetComponent<IArmyElement>().Health) 
            .ToList();

        foreach (var enemy in allEnemies)
        {
            int currentAttackers = 0;
            if (targetAttackerCount.ContainsKey(enemy.gameObject))
                currentAttackers = targetAttackerCount[enemy.gameObject];
            
            if (currentAttackers < maxAttackersPerTarget)
            {
                return enemy.gameObject;
            }
        }
        
        return allEnemies.FirstOrDefault()?.gameObject;
    }

    // ON "OVERRIDE" LES FONCTIONS DE LOCK
    public override void LockTarget(GameObject attacker, GameObject target)
    {
        m_DicoWhoTargetsWhom[attacker] = target;
    }

    public override void UnlockTarget(GameObject attacker)
    {
        if (m_DicoWhoTargetsWhom.ContainsKey(attacker))
            m_DicoWhoTargetsWhom.Remove(attacker);
    }

    // ON "OVERRIDE" LA FONCTION DE MORT POUR NETTOYER LE DICTIONNAIRE
    public override void ArmyElementHasBeenKilled(GameObject go)
    {
        base.ArmyElementHasBeenKilled(go); // Appelle la logique du parent (RefreshHud, etc)

        // --- NETTOYAGE DU DICTIONNAIRE (LOGIQUE ROUGE) ---
        if (m_DicoWhoTargetsWhom.ContainsKey(go))
            m_DicoWhoTargetsWhom.Remove(go);

        if (m_DicoWhoTargetsWhom.ContainsValue(go))
        {
            var attackersToUnlock = m_DicoWhoTargetsWhom.Where(pair => pair.Value == go)
                                                      .Select(pair => pair.Key)
                                                      .ToList();
            foreach(var attacker in attackersToUnlock)
                m_DicoWhoTargetsWhom.Remove(attacker);
        }
        
        if (go == currentDroneFocusTarget)
        {
            currentDroneFocusTarget = null;
        }
        if (m_ArmyElements.Count == 0)
        {
            GUIUtility.systemCopyBuffer = "0\t" +((int)Timer.Value).ToString()+"\t0\t0\t0";
        }
    }
    
    public void GreenArmyIsDead(string deadArmyTag)
    {
        int nDrones = 0, nTurrets = 0, health = 0;
        ComputeStatistics(ref nDrones, ref nTurrets, ref health);
        GUIUtility.systemCopyBuffer = "1\t" + ((int)Timer.Value).ToString() + "\t"+nDrones.ToString()+"\t"+nTurrets.ToString()+"\t"+health.ToString();
        
        RefreshHudDisplay();
    }
}