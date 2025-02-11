public class AlarmSetOffObjective : MissionObjective
{
	private bool mSetOff;

	public override void Start()
	{
		mMissionPassIfNotFail = true;
		base.Start();
		GameplayController.Instance().OnCameraAlarmSounded += OnAlarmSounded;
		GameplayController.Instance().OnPanelAlarmSounded += OnAlarmSounded;
	}

	private void OnAlarmSounded(object sender)
	{
		if (base.State == ObjectiveState.InProgress)
		{
			if (!mSetOff)
			{
				Fail();
			}
			DoOnCompleteCall();
			mSetOff = true;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnCameraAlarmSounded -= OnAlarmSounded;
			GameplayController.Instance().OnPanelAlarmSounded -= OnAlarmSounded;
		}
	}
}
