using System;
using UnityEngine;

public class AssaultDescriptor : TaskDescriptor
{
	public WaveSpawner SpawnParent;

	public InheritableMovementParams Parameters = new InheritableMovementParams();

	public AssaultParams assaultParams = new AssaultParams();

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskRouteTo>();
			return null;
		}
		return new TaskAssault(owner, priority, flags | ConfigFlags.GetAsFlags(), Parameters.Clone(), assaultParams);
	}

	public override void ResolveGuidLinks()
	{
		Parameters.ResolveGuidLinks();
		assaultParams.ResolveGuidLinks();
	}

	public void OnDrawGizmosSelected()
	{
		if (assaultParams.Target.theObject != null)
		{
			Parameters.mDestination = assaultParams.Target.theObject.transform.position;
		}
		if (Parameters.mDestination != Vector3.zero)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, Parameters.mDestination);
			Vector3 vector = Parameters.mDestination - base.transform.position;
			Vector3 vector2 = vector / 2f;
			vector.Normalize();
			float num = Vector3.Angle(vector, base.transform.forward);
			float y = Parameters.mDestination.x - base.transform.position.x;
			float x = Parameters.mDestination.z - base.transform.position.z;
			num = Mathf.Atan2(y, x) * 180f / (float)Math.PI;
			if (num < 0f)
			{
				num += 360f;
			}
			Quaternion quaternion = Quaternion.AngleAxis(num + 30f, Vector3.up);
			Vector3 from = Parameters.mDestination - quaternion * base.transform.forward * 0.8f;
			Gizmos.DrawLine(from, Parameters.mDestination);
			Quaternion quaternion2 = Quaternion.AngleAxis(num - 30f, Vector3.up);
			Vector3 to = Parameters.mDestination - quaternion2 * base.transform.forward * 0.8f;
			Gizmos.DrawLine(Parameters.mDestination, to);
			Vector3 from2 = Parameters.mDestination - vector2 - quaternion * base.transform.forward * 0.8f;
			Gizmos.DrawLine(from2, Parameters.mDestination - vector2);
			Vector3 from3 = Parameters.mDestination - vector2 - quaternion2 * base.transform.forward * 0.8f;
			Gizmos.DrawLine(from3, Parameters.mDestination - vector2);
		}
	}
}
