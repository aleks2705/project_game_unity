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
		if (m_ArmyElement.ArmyManager == null) return TaskStatus.Running;

		var enemy = m_ArmyElement.ArmyManager.GetRandomEnemy<Drone>(gameObject, transform.position, minRadius.Value, maxRadius.Value);

		if (enemy != null) {
			target.Value = enemy.transform;
			m_ArmyElement.ArmyManager.LockTarget(gameObject, enemy);
			return TaskStatus.Success;
		} else {
			target.Value = null;
			m_ArmyElement.ArmyManager.UnlockTarget(gameObject);
			return TaskStatus.Failure;
		}
	}
}