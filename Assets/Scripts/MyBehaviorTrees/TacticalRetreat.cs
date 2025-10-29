using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("MyTasks")]
[TaskDescription("Retreat away from nearest enemy when low on health - survival tactic")]
public class TacticalRetreat : NavMeshMovement
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Distance to retreat")]
    public SharedFloat retreatDistance = 20f;
    
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Current target to flee from")]
    public SharedTransform target;
    
    private IArmyElement m_ArmyElement;
    private Vector3 m_RetreatPosition;
    private bool m_HasSetDestination = false;

    public override void OnAwake()
    {
        base.OnAwake();
        m_ArmyElement = GetComponent<IArmyElement>();
    }

    public override void OnStart()
    {
        base.OnStart();
        m_HasSetDestination = false;
        
        // Calculate retreat position - away from target
        if (target.Value != null)
        {
            Vector3 directionAway = (transform.position - target.Value.position).normalized;
            m_RetreatPosition = transform.position + directionAway * retreatDistance.Value;
            
            // Make sure retreat position is on terrain
            Vector3 posOnTerrain = Vector3.zero;
            Vector3 normalOnTerrain = Vector3.zero;
            if (TerrainManager.Instance.GetVerticallyAlignedPositionOnTerrain(m_RetreatPosition, ref posOnTerrain, ref normalOnTerrain))
            {
                m_RetreatPosition = posOnTerrain;
            }
            
            SetDestination(m_RetreatPosition);
            m_HasSetDestination = true;
            
            Debug.Log($"[AI-RETREAT] {gameObject.name} retreating to {m_RetreatPosition}");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (!m_HasSetDestination) return TaskStatus.Failure;
        
        if (HasArrived())
        {
            Debug.Log($"[AI-RETREAT] {gameObject.name} successfully retreated");
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }
}
