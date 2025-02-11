using System;
using UnityEngine;

public class OccupyCoverClusterDescriptor : TaskDescriptor
{
	public InheritableMovementParams Parameters = new InheritableMovementParams();

	public TaskDescriptor ExecuteOnCompletion;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskOccupyCoverCluster>();
			return null;
		}
		if (ExecuteOnCompletion != null)
		{
			ExecuteOnCompletion.CreateTask(owner, priority, flags);
		}
		if (ConfigFlags == null)
		{
			return null;
		}
		flags |= Task.Config.ClearAllCurrentType;
		return new TaskOccupyCoverCluster(owner, priority, flags | ConfigFlags.GetAsFlags(), Parameters.Clone());
	}

	public override void ResolveGuidLinks()
	{
		Parameters.ResolveGuidLinks();
	}

	public void OnDrawGizmosSelected()
	{
		if (Parameters.coverCluster != null)
		{
			Parameters.mDestination = Parameters.coverCluster.transform.position;
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
