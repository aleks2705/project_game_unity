using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyTasks")]
[TaskDescription("Select non targeted enemy turret")]

public class SelectEnemyTurret : Action
{
	IArmyElement m_ArmyElement;
	public SharedTransform target;
	public SharedFloat minRadius;
	public SharedFloat maxRadius;

	public override void OnAwake()
	{
		m_ArmyElement =(IArmyElement) GetComponent(typeof(IArmyElement));
	}

	public override TaskStatus OnUpdate()
	{
		if (m_ArmyElement.ArmyManager == null) return TaskStatus.Running; // reference to the ArmyManager has not been injected yet

		// Prefer using the army manager lock so multiple units don't all pick the same target
		var go = m_ArmyElement.ArmyManager.LockArmyElementOnRandomNonTargetedEnemy<Turret>(gameObject, transform.position, minRadius.Value, maxRadius.Value);
		target.Value = go?.transform;
		if (go != null)
			Debug.Log($"[AI-SEL] {gameObject.name} selected turret target {go.name}");
		else
			Debug.Log($"[AI-SEL] {gameObject.name} found NO turret target");

		if (target.Value != null) return TaskStatus.Success;
		else return TaskStatus.Failure;

	}
}