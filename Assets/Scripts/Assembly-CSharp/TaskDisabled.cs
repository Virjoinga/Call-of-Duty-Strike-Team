public class TaskDisabled : Task
{
	public TaskDisabled(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mActor.realCharacter.IsAimingDownSights = false;
		if (mActor == GameController.Instance.mFirstPersonActor)
		{
			GameController.Instance.ExitFirstPerson();
		}
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return mActor.health.Health > 0f;
	}

	public override void Finish()
	{
		mActor.Command("Stand");
	}
}
