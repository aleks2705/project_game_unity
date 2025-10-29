using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Check if target is within specified range - for engagement decisions")]
public class CheckTargetInRange : Conditional
{
    [Tooltip("Target to check distance to")]
    public SharedTransform target;
    
    [Tooltip("Minimum range")]
    public SharedFloat minRange = 0f;
    
    [Tooltip("Maximum range")]
    public SharedFloat maxRange = 100f;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null) return TaskStatus.Failure;
        
        float distance = Vector3.Distance(transform.position, target.Value.position);
        
        if (distance >= minRange.Value && distance <= maxRange.Value)
        {
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
}
