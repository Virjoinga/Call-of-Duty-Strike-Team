using UnityEngine;

internal class TaskFirstPerson : Task
{
	public TaskFirstPerson(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		SetupState();
		owner.CancelTasks(typeof(TaskMoveToCover));
	}

	public override void OnResume()
	{
		SetupState();
	}

	public override void OnSleep()
	{
		TearDownState(false);
	}

	public override void Finish()
	{
		TearDownState(true);
	}

	public override void Update()
	{
		if (mActor.behaviour.PlayerControlled && mActor.GetPosition() != Vector3.zero)
		{
			mActor.tether.TetherToSelf();
		}
	}

	public override bool HasFinished()
	{
		return !mActor.realCharacter.IsFirstPerson;
	}

	private void SetupState()
	{
		mActor.fireAtWill.enabled = false;
		if (mActor.navAgent.enabled)
		{
			mActor.navAgent.ResetPath();
		}
	}

	private void TearDownState(bool removeDefenderTasks)
	{
		mActor.fireAtWill.enabled = true;
		if (removeDefenderTasks)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & ~mActor.ident);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				a.tasks.CancelTasks(typeof(TaskDefendFirstPerson));
			}
		}
	}
}
