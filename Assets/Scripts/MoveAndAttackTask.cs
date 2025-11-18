using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI; // Important !

[TaskCategory("MyTasks")]
[RequireComponent(typeof(NavMeshAgent))]
public class MoveAndAttackTask : Action
{
    // C'est la variable d'entrée : "Qui est-ce que je dois attaquer ?"
    public SharedGameObject targetEnemy;

    public float attackRange = 10f; // La distance à laquelle le drone s'arrête pour tirer

    private NavMeshAgent navAgent;
    // Remplace "WeaponScript" par le nom de TON script de tir
    private Drone drone; 

    public override void OnStart()
    {
        navAgent = GetComponent<NavMeshAgent>();
        
        // Remplace "WeaponScript" ci-dessous aussi !
        drone = GetComponent<Drone>(); 
    }

    public override TaskStatus OnUpdate()
    {
        if (targetEnemy.Value == null || !targetEnemy.Value.activeSelf)
        {
            return TaskStatus.Failure; // L'ennemi est mort ou a disparu
        }

        Vector3 targetPosition = targetEnemy.Value.transform.position;

        // 1. Bouger vers la cible
        navAgent.SetDestination(targetPosition);

        // 2. Tirer en même temps
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= attackRange)
        {
            // Fait que le drone regarde l'ennemi
            transform.LookAt(targetPosition);
            
            // Fait s'arrêter le NavMeshAgent pour éviter de "danser"
            navAgent.isStopped = true; 

            if (drone != null)
            {
                // Remplace "Fire()" par le nom de TA fonction de tir
                drone.Shoot(); 
            }
        }
        else
        {
            // Si l'ennemi est trop loin, on reprend la poursuite
            navAgent.isStopped = false;
        }

        // Cette tâche est toujours "en cours" (Running)
        // tant que l'ennemi est en vie.
        return TaskStatus.Running; 
    }
}