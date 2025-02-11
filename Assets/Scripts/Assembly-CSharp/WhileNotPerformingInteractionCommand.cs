using System;
using System.Collections;

public class WhileNotPerformingInteractionCommand : Command
{
	public enum Interactions
	{
		WindowPeek = 0,
		OpenDoor = 1
	}

	public Type[] InteractionTypes = new Type[2]
	{
		typeof(TaskPeekaboo),
		typeof(TaskOpen)
	};

	public bool WaitForIsInteracting;

	public Interactions InteractionToWaitFor;

	public Type[] GetTypes()
	{
		return InteractionTypes;
	}

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		bool isPerformingTask2 = false;
		isPerformingTask2 = CheckForTask(GetTypes()[(int)InteractionToWaitFor]);
		if (WaitForIsInteracting)
		{
			while (!isPerformingTask2)
			{
				isPerformingTask2 = CheckForTask(GetTypes()[(int)InteractionToWaitFor]);
				yield return null;
			}
		}
		else
		{
			while (isPerformingTask2)
			{
				isPerformingTask2 = CheckForTask(GetTypes()[(int)InteractionToWaitFor]);
				yield return null;
			}
		}
	}

	private bool CheckForTask(Type taskType)
	{
		foreach (Actor validFirstPersonActor in GameplayController.instance.GetValidFirstPersonActors())
		{
			if (taskType == typeof(TaskPeekaboo))
			{
				TaskPeekaboo taskPeekaboo = null;
				if (validFirstPersonActor.tasks.IsRunningTask(taskType))
				{
					taskPeekaboo = (TaskPeekaboo)validFirstPersonActor.tasks.GetRunningTask(taskType);
					if (taskPeekaboo.AtTarget())
					{
						return true;
					}
				}
			}
			else
			{
				if (taskType != typeof(TaskOpen))
				{
					continue;
				}
				TaskOpen taskOpen = null;
				if (validFirstPersonActor.tasks.IsRunningTask(taskType))
				{
					taskOpen = (TaskOpen)validFirstPersonActor.tasks.GetRunningTask(taskType);
					if (taskOpen.AtTarget())
					{
						return true;
					}
				}
			}
		}
		return false;
	}
}
