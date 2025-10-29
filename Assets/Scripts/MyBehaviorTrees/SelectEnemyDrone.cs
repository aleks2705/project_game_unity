using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Select non targeted enemy Drone")]

public class SelectEnemyDrone : Action
{
	IArmyElement m_ArmyElement;
	public SharedTransform target;
	public SharedFloat minRadius;
	public SharedFloat maxRadius;
	/* Vector3 m_Position;
	public Vector3 Position
	{ get { return m_Position; } set { m_Position = value; }
	}*/

	public override void OnAwake()
	{
		m_ArmyElement =(IArmyElement) GetComponent(typeof(IArmyElement));
	}

	public override TaskStatus OnUpdate()
	{
		if (m_ArmyElement.ArmyManager == null) return TaskStatus.Running; // la r�f�rence � l'arm�e n'a pas encore �t� inject�e

		// Use army manager locking so that armies (especially red) can coordinate target selection
		var go = m_ArmyElement.ArmyManager.LockArmyElementOnRandomNonTargetedEnemy<Drone>(gameObject, transform.position, minRadius.Value, maxRadius.Value);
		target.Value = go?.transform;
		if (go != null)
			Debug.Log($"[AI-SEL] {gameObject.name} selected drone target {go.name}");
		else
			Debug.Log($"[AI-SEL] {gameObject.name} found NO drone target");
		if (target.Value != null) return TaskStatus.Success;
		else return TaskStatus.Failure;

	}
}