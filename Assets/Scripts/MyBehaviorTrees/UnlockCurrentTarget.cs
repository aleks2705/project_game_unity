using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Unlock current target in army manager - allows target reassignment")]
public class UnlockCurrentTarget : Action
{
    private IArmyElement m_ArmyElement;

    public override void OnAwake()
    {
        m_ArmyElement = GetComponent<ArmyElement>();
    }

    public override TaskStatus OnUpdate()
    {
        if (m_ArmyElement?.ArmyManager != null)
        {
            m_ArmyElement.ArmyManager.UnlockArmyElement(gameObject);
            Debug.Log($"[AI-UNLOCK] {gameObject.name} unlocked its target");
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
}
