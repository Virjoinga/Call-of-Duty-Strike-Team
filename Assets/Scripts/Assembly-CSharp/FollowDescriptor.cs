using UnityEngine;

public class FollowDescriptor : TaskDescriptor
{
	public ActorWrapper Target;

	public GameObject TargetObject;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskFollow>();
			return null;
		}
		Actor actor = null;
		if (Target != null)
		{
			actor = Target.GetActor();
		}
		else
		{
			ActorWrapper componentInChildren = TargetObject.GetComponentInChildren<ActorWrapper>();
			if (componentInChildren != null)
			{
				actor = componentInChildren.GetActor();
			}
		}
		TBFAssert.DoAssert(actor != null, "FollowDescriptor - Target BaseCharacter not specified");
		return new TaskFollow(owner, priority, actor, flags | ConfigFlags.GetAsFlags());
	}
}
