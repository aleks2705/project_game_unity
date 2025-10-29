using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("MyTasks")]
[TaskDescription("Select weakest enemy in range - finish off wounded targets for tactical advantage")]
public class SelectWeakestEnemy : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Type of enemy to search for (Drone or Turret)")]
    public SharedString enemyType = "Drone";
    
    public SharedTransform target;
    public SharedFloat minRadius = 0f;
    public SharedFloat maxRadius = 100f;
    
    private IArmyElement m_ArmyElement;

    public override void OnAwake()
    {
        m_ArmyElement = GetComponent<IArmyElement>();
    }

    public override TaskStatus OnUpdate()
    {
        if (m_ArmyElement?.ArmyManager == null) return TaskStatus.Running;
        
        // Get all enemies of the specified type
        System.Collections.Generic.List<ArmyElement> enemies;
        
        if (enemyType.Value == "Turret")
        {
            enemies = Object.FindObjectsByType<Turret>(FindObjectsSortMode.None)
                .Where(e => !e.gameObject.CompareTag(gameObject.tag))
                .Cast<ArmyElement>()
                .ToList();
        }
        else // Default to Drone
        {
            enemies = Object.FindObjectsByType<Drone>(FindObjectsSortMode.None)
                .Where(e => !e.gameObject.CompareTag(gameObject.tag))
                .Cast<ArmyElement>()
                .ToList();
        }
        
        // Filter by range and sort by health (ascending)
        var enemiesInRange = enemies
            .Where(e => {
                float dist = Vector3.Distance(transform.position, e.transform.position);
                return dist >= minRadius.Value && dist <= maxRadius.Value;
            })
            .OrderBy(e => e.Health)
            .ThenBy(e => Vector3.Distance(transform.position, e.transform.position))
            .ToList();
        
        if (enemiesInRange.Count > 0)
        {
            var weakest = enemiesInRange.First();
            target.Value = weakest.transform;
            
            Debug.Log($"[AI-WEAK] {gameObject.name} targeting weakest {enemyType.Value}: {weakest.name} with {weakest.Health} HP");
            return TaskStatus.Success;
        }
        
        Debug.Log($"[AI-WEAK] {gameObject.name} found no {enemyType.Value} in range");
        return TaskStatus.Failure;
    }
}
