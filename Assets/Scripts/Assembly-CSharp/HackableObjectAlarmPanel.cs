public class HackableObjectAlarmPanel : HackableObject
{
	private AlarmPanel mPanel;

	private bool mHasFinished;

	private void Awake()
	{
		ShouldUseConsultant = true;
		mPanel = GetComponent<AlarmPanel>();
	}

	public override void Init(Actor actor)
	{
		base.Init(actor);
		if (mPanel != null)
		{
			mPanel.AddInterestedPlayerUnit(actor);
		}
	}

	public override void CleanUp(Actor actor)
	{
		base.CleanUp(actor);
		if (mPanel != null)
		{
			mPanel.RemovePlayerUnitFromInterested(actor);
		}
	}

	protected override void Consult()
	{
		if (base.FullyHacked && !mHasFinished)
		{
			mHasFinished = true;
			if (mPanel != null)
			{
				mPanel.MarkAsHacked();
			}
		}
	}
}
