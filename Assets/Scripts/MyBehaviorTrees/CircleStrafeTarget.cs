using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("MyTasks")]
[TaskDescription("Circle strafe around target while maintaining distance - advanced drone tactic")]
public class CircleStrafeTarget : NavMeshMovement
{
    [Tooltip("Target to circle around")]
    public SharedTransform target;
    
    [Tooltip("Distance from target to maintain while circling")]
    public SharedFloat strafeDistance = 15f;
    
    [Tooltip("Strafe speed in degrees per second")]
    public SharedFloat strafeSpeed = 45f;
    
    [Tooltip("Clockwise or counter-clockwise")]
    public SharedBool clockwise = true;

    private float m_CurrentAngle = 0f;
    private float m_LastUpdateTime = 0f;

    public override void OnStart()
    {
        base.OnStart();
        
        if (target.Value != null)
        {
            // Calculate initial angle
            Vector3 directionToSelf = transform.position - target.Value.position;
            m_CurrentAngle = Mathf.Atan2(directionToSelf.z, directionToSelf.x) * Mathf.Rad2Deg;
        }
        
        m_LastUpdateTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null) return TaskStatus.Failure;
        
        float deltaTime = Time.time - m_LastUpdateTime;
        m_LastUpdateTime = Time.time;
        
        // Update angle based on strafe speed
        float angleChange = strafeSpeed.Value * deltaTime;
        if (!clockwise.Value) angleChange = -angleChange;
        m_CurrentAngle += angleChange;
        
        // Calculate position around target
        float angleRad = m_CurrentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(angleRad) * strafeDistance.Value,
            0f,
            Mathf.Sin(angleRad) * strafeDistance.Value
        );
        
        Vector3 targetPosition = target.Value.position + offset;
        
        // Project to terrain
        Vector3 posOnTerrain = Vector3.zero;
        Vector3 normalOnTerrain = Vector3.zero;
        if (TerrainManager.Instance.GetVerticallyAlignedPositionOnTerrain(targetPosition, ref posOnTerrain, ref normalOnTerrain))
        {
            targetPosition = posOnTerrain;
        }
        
        SetDestination(targetPosition);
        
        return TaskStatus.Running;
    }
}
