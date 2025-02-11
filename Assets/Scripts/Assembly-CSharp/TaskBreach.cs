public class TaskBreach : Task
{
	private enum State
	{
		Start = 0,
		MovingToBreachPoint = 1,
		Complete = 2
	}

	public const float BLAST_RADIUS = 0.5f;

	private BreachSequence mBreachSequence;

	private State mState;

	public TaskBreach(TaskManager owner, TaskManager.Priority priority, Config flags, BreachSequence breachSequence)
		: base(owner, priority, flags)
	{
		TBFAssert.DoAssert(breachSequence, string.Format("Attempting to initialise TaskBreach for {0} with no BreachSequence", owner.name));
		mBreachSequence = breachSequence;
		mState = State.Start;
		SetupState();
	}

	public override void Finish()
	{
		TearDownState();
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams();
			inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
			inheritableMovementParams.mDestination = mBreachSequence.BreachStartLocator.position;
			mState = State.MovingToBreachPoint;
			new TaskRouteTo(mOwner, base.Priority, Config.ClearAllCurrentType, inheritableMovementParams);
			break;
		}
		case State.MovingToBreachPoint:
			if (mBreachSequence.Available && (mActor.GetPosition() - mBreachSequence.BreachStartLocator.position).sqrMagnitude < 0.1f)
			{
				mBreachSequence.StartSequence(mActor);
			}
			mState = State.Complete;
			break;
		}
	}

	public override bool HasFinished()
	{
		return mState == State.Complete;
	}

	private void SetupState()
	{
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		component.enabled = false;
	}

	private void TearDownState()
	{
		if (mActor.health != null)
		{
			mActor.health.TakeDamageModifier = 1f;
		}
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		component.enabled = true;
	}
}
