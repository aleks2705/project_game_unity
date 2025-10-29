using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("MyTasks")]
[TaskDescription("Maintain optimal distance from target - kiting behavior for drones")]
public class KeepDistance : NavMeshMovement
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Target to maintain distance from")]
    public SharedTransform target;
    
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Optimal distance to maintain")]
    public SharedFloat optimalDistance = 15f;
    
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Distance tolerance")]
    public SharedFloat distanceTolerance = 3f;

    private float m_RepositionCooldown = 0f;
    private const float REPOSITION_INTERVAL = 0.5f; // Only reposition every 0.5s

    public override void OnStart()
    {
        base.OnStart();
        m_RepositionCooldown = 0f;
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null) return TaskStatus.Failure;
        
        float distanceToTarget = Vector3.Distance(transform.position, target.Value.position);
        float optDist = optimalDistance.Value;
        float tolerance = distanceTolerance.Value;
        
        // Check if we need to reposition
        bool tooClose = distanceToTarget < (optDist - tolerance);
        bool tooFar = distanceToTarget > (optDist + tolerance);
        
        if (tooClose || tooFar)
        {
            m_RepositionCooldown -= Time.deltaTime;
            
            if (m_RepositionCooldown <= 0f)
            {
                // Calculate new position to maintain optimal distance
                Vector3 directionFromTarget = (transform.position - target.Value.position).normalized;
                Vector3 optimalPosition = target.Value.position + directionFromTarget * optDist;
                
                // Ensure position is on terrain
                Vector3 posOnTerrain = Vector3.zero;
                Vector3 normalOnTerrain = Vector3.zero;
                if (TerrainManager.Instance.GetVerticallyAlignedPositionOnTerrain(optimalPosition, ref posOnTerrain, ref normalOnTerrain))
                {
                    optimalPosition = posOnTerrain;
                }
                
                SetDestination(optimalPosition);
                m_RepositionCooldown = REPOSITION_INTERVAL;
                
                string reason = tooClose ? "too close" : "too far";
                Debug.Log($"[AI-KITE] {gameObject.name} repositioning ({reason}): current={distanceToTarget:F1}, optimal={optDist:F1}");
            }
            
            return TaskStatus.Running;
        }
        
        // We're at optimal distance
        return TaskStatus.Success;
    }
}
