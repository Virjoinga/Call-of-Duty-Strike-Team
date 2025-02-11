using System;
using UnityEngine;

public class MoveToDescriptor : TaskDescriptor
{
	public Transform Target;

	public WaveSpawner SpawnParent;

	public InheritableMovementParams Parameters = new InheritableMovementParams();

	public float EnemyBreakoutRange;

	public TaskDescriptor ExecuteOnCompletion;

	protected NavMeshPath mPath;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskRouteTo>();
			return null;
		}
		if (ExecuteOnCompletion != null)
		{
			ExecuteOnCompletion.CreateTask(owner, priority, flags);
		}
		Parameters.mDestination = PickTarget();
		TaskRouteTo taskRouteTo = new TaskRouteTo(owner, priority, flags | ConfigFlags.GetAsFlags(), Parameters.Clone());
		taskRouteTo.EnemyBreakoutRange = EnemyBreakoutRange;
		return taskRouteTo;
	}

	private Vector3 PickTarget()
	{
		if (Target == null)
		{
			return new Vector3(0f, 0f, 0f);
		}
		Vector3 location = GeneralArea.GetLocation(Target);
		if (SpawnParent == null)
		{
			return location;
		}
		if (SpawnParent.RandomTargets == null || SpawnParent.RandomTargets.Count == 0)
		{
			return location;
		}
		return SpawnParent.RandomTargets[UnityEngine.Random.Range(0, SpawnParent.RandomTargets.Count)].position;
	}

	public override void ResolveGuidLinks()
	{
		Parameters.ResolveGuidLinks();
	}

	public void OnDrawGizmosSelected()
	{
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
