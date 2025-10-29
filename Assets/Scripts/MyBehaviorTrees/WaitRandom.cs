using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Wait for a random duration - adds unpredictability to behavior")]
public class WaitRandom : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Minimum wait time in seconds")]
    public SharedFloat minWaitTime = 0.5f;
    
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Maximum wait time in seconds")]
    public SharedFloat maxWaitTime = 2f;
    
    private float m_WaitDuration;
    private float m_ElapsedTime;

    public override void OnStart()
    {
        m_WaitDuration = Random.Range(minWaitTime.Value, maxWaitTime.Value);
        m_ElapsedTime = 0f;
    }

    public override TaskStatus OnUpdate()
    {
        m_ElapsedTime += Time.deltaTime;
        
        if (m_ElapsedTime >= m_WaitDuration)
        {
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }
}
