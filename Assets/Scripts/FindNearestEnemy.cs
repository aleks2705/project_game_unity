using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
public class FindNearestEnemyTask : Action
{
    // C'est la variable de sortie : "Où est-ce que je stocke l'ennemi trouvé ?"
    public SharedGameObject targetEnemy; 

    public override TaskStatus OnUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("GreenUnit");
        if (enemies.Length == 0)
        {
            return TaskStatus.Failure; // Aucun ennemi trouvé
        }

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        // On a trouvé le plus proche, on le stocke dans la variable partagée
        targetEnemy.Value = closestEnemy;
        return TaskStatus.Success;
    }
}