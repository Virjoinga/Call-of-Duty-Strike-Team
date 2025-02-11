using UnityEngine;

public class PlayAnimationDescriptor : TaskDescriptor
{
	public AnimationClip Clip;

	public AnimationClip Exit;

	public bool Looping;

	public int NumberOfLoops = 1;

	public int RandomLoopsPlusMinus;

	public float BlendTime = 0.25f;

	public float Speed = 1f;

	public Transform Locator;

	public static string CATEGORY = "SetPiece";

	public static string ACTION = "Actor1";

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (Clip == null)
		{
			return null;
		}
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskPlayAnimation>();
			return null;
		}
		TaskPlayAnimation taskPlayAnimation = new TaskPlayAnimation(owner, priority, flags | ConfigFlags.GetAsFlags(), Clip, Exit, CATEGORY, ACTION, Looping, NumberOfLoops, RandomLoopsPlusMinus, BlendTime, Speed);
		if (Locator != null)
		{
			taskPlayAnimation.Locator = Locator;
		}
		return taskPlayAnimation;
	}
}
