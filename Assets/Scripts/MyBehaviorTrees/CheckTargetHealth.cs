using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Check if target's health is below threshold - for focus fire optimization")]
public class CheckTargetHealth : Conditional
{
    [Tooltip("The target to check")]
    public SharedTransform target;
    
    [Tooltip("Health threshold - returns success if target health is below this")]
    public SharedFloat healthThreshold = 40f;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null) return TaskStatus.Failure;
        
        IArmyElement targetElement = target.Value.GetComponent<IArmyElement>();
        if (targetElement == null) return TaskStatus.Failure;
        
        float targetHealth = targetElement.Health;
        
        if (targetHealth <= healthThreshold.Value)
        {
            Debug.Log($"[AI-FOCUS] Target {target.Value.name} is weakened ({targetHealth} HP) - maintaining focus!");
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
}
