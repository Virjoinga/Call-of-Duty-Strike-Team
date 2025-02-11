using UnityEngine;

public class UseFixedGunDescriptor : TaskDescriptor
{
	public GameObject Gun;

	public bool Warp;

	public bool Airborne;

	public bool DontDismount;

	public bool SuppressTransition;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskUseFixedGun>();
			return null;
		}
		FixedGun componentInChildren = Gun.GetComponentInChildren<FixedGun>();
		if (componentInChildren != null)
		{
			return new TaskUseFixedGun(owner, priority, flags | ConfigFlags.GetAsFlags(), componentInChildren, Warp, Airborne, DontDismount, SuppressTransition);
		}
		return null;
	}
}
