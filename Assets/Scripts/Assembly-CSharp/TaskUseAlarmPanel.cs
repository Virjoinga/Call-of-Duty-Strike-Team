public class TaskUseAlarmPanel : Task
{
	private enum State
	{
		Start = 0,
		Animating = 1,
		Done = 2
	}

	private State mState;

	private AlarmPanel mTarget;

	public TaskUseAlarmPanel(TaskManager owner, TaskManager.Priority priority, Config flags, AlarmPanel alarm)
		: base(owner, priority, flags)
	{
		alarm.AddToInterestedPartiesList(mActor);
		mTarget = alarm;
		if ((bool)AlarmManager.Instance && AlarmManager.Instance.AlarmSounding)
		{
			mState = State.Done;
		}
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.ActivateSetPiece);
			setPieceLogic.PlaceSetPiece(mTarget.transform);
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, setPieceLogic, false, true, true, moveParams, 0f);
			mState = State.Animating;
			break;
		}
		case State.Animating:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				float sqrMagnitude = (mTarget.transform.position - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude <= 2.25f)
				{
					mTarget.Use();
				}
				mState = State.Done;
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((bool)AlarmManager.Instance && AlarmManager.Instance.AlarmSounding)
		{
			return true;
		}
		return mState == State.Done;
	}
}
