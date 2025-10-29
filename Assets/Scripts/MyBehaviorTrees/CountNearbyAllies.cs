using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("MyTasks")]
[TaskDescription("Count nearby friendly units - for coordination decisions")]
public class CountNearbyAllies : Conditional
{
    [Tooltip("Search radius")]
    public SharedFloat searchRadius = 20f;
    
    [Tooltip("Minimum number of allies required")]
    public SharedInt minimumAllies = 2;
    
    [Tooltip("Store the count")]
    public SharedInt allyCount;

    public override TaskStatus OnUpdate()
    {
        // Find all army elements with same tag
        var allies = Object.FindObjectsByType<ArmyElement>(FindObjectsSortMode.None)
            .Where(e => e.gameObject.CompareTag(gameObject.tag) && e.gameObject != gameObject)
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= searchRadius.Value)
            .ToList();
        
        allyCount.Value = allies.Count;
        
        if (allyCount.Value >= minimumAllies.Value)
        {
            Debug.Log($"[AI-COORD] {gameObject.name} has {allyCount.Value} nearby allies - coordinating!");
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
}
