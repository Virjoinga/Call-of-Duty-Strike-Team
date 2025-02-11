using System;
using System.Collections;

public class CancelAllTasksInFPPCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			Type[] taskTypes = new Type[2]
			{
				typeof(TaskRoutine),
				typeof(TaskFirstPerson)
			};
			GameController.Instance.mFirstPersonActor.tasks.CancelTasksExcluding(taskTypes);
		}
		yield break;
	}
}
