using UnityEngine;

public class DropBodyDescriptor : TaskDescriptor
{
	public Vector3 Target = Vector3.zero;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		TaskCarry taskCarry = (TaskCarry)owner.GetRunningTask(typeof(TaskCarry));
		if (taskCarry == null)
		{
			return null;
		}
		if (Target == Vector3.zero)
		{
			taskCarry.DropToFloor(TaskManager.Priority.IMMEDIATE);
		}
		else
		{
			taskCarry.SetDropOffLocation(Target, Target - owner.transform.position);
		}
		return null;
	}
}
