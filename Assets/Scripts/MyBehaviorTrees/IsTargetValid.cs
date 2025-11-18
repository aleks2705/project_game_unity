using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Vérifie si la variable Target est valide (pas nulle et active)")]

public class IsTargetValid : Conditional
{
    [UnityEngine.Tooltip("La cible à vérifier")]
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        if (target.Value != null && target.Value.gameObject.activeSelf)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}