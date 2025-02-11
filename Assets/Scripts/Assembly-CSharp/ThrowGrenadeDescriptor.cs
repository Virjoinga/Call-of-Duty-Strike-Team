using UnityEngine;

public class ThrowGrenadeDescriptor : TaskDescriptor
{
	public Transform Target;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskThrowGrenade>();
			return null;
		}
		Actor component = owner.GetComponent<Actor>();
		TBFAssert.DoAssert(component != null, "ThrowGrenadeDescriptor owner has no RealCharacter component");
		component.grenadeThrower.Target = Target.position;
		return new TaskThrowGrenade(owner, priority, flags | ConfigFlags.GetAsFlags(), Target.position);
	}
}
