using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Advanced evasive maneuver for flying drones - dodges while engaging")]
public class EvasiveManeuver : Action
{
    [Tooltip("Target to evade from")]
    public SharedTransform target;
    
    [Tooltip("Evasion radius")]
    public SharedFloat evasionRadius = 5f;
    
    [Tooltip("Evasion speed")]
    public SharedFloat evasionSpeed = 10f;
    
    [Tooltip("Duration of evasive maneuver")]
    public SharedFloat maneuverDuration = 1.5f;
    
    private Rigidbody m_Rigidbody;
    private Transform m_Transform;
    private Vector3 m_EvasionDirection;
    private float m_ElapsedTime;
    private bool m_IsInitialized;

    public override void OnAwake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = transform;
    }

    public override void OnStart()
    {
        m_ElapsedTime = 0f;
        m_IsInitialized = false;
        
        if (target.Value != null)
        {
            // Calculate perpendicular evasion direction
            Vector3 toTarget = (target.Value.position - m_Transform.position).normalized;
            Vector3 perpendicular = Vector3.Cross(toTarget, Vector3.up).normalized;
            
            // Randomly choose left or right
            if (Random.value > 0.5f) perpendicular = -perpendicular;
            
            m_EvasionDirection = perpendicular;
            m_IsInitialized = true;
            
            Debug.Log($"[AI-EVADE] {gameObject.name} executing evasive maneuver");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (!m_IsInitialized) return TaskStatus.Failure;
        
        m_ElapsedTime += Time.deltaTime;
        
        if (m_ElapsedTime >= maneuverDuration.Value)
        {
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }

    public override void OnFixedUpdate()
    {
        if (!m_IsInitialized || m_Rigidbody == null) return;
        
        // Apply evasive movement
        Vector3 evasiveVelocity = m_EvasionDirection * evasionSpeed.Value;
        Vector3 newPosition = m_Transform.position + evasiveVelocity * Time.fixedDeltaTime;
        
        m_Rigidbody.MovePosition(newPosition);
    }
}
