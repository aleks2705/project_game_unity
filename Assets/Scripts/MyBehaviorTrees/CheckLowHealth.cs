using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Check if unit health is below threshold - used for tactical retreat")]
public class CheckLowHealth : Conditional
{
    [Tooltip("Health threshold below which this returns success")]
    public SharedFloat healthThreshold = 30f;
    
    private IArmyElement m_ArmyElement;

    public override void OnAwake()
    {
        m_ArmyElement = GetComponent<IArmyElement>();
    }

    public override TaskStatus OnUpdate()
    {
        if (m_ArmyElement == null) return TaskStatus.Failure;
        
        float currentHealth = m_ArmyElement.Health;
        
        if (currentHealth <= healthThreshold.Value)
        {
            Debug.Log($"[AI-HEALTH] {gameObject.name} is low on health ({currentHealth}/{healthThreshold.Value}) - retreating!");
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
}
