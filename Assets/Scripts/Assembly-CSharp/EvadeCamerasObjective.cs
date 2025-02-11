public class EvadeCamerasObjective : MissionObjective
{
	public override void Start()
	{
		mMissionPassIfNotFail = true;
		GameplayController.Instance().OnCameraAlarmSounded += OnCameraAlarmSounded;
		base.Start();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		GameplayController.Instance().OnCameraAlarmSounded -= OnCameraAlarmSounded;
	}

	private void Update()
	{
		if ((GKM.AlertedMask & GKM.CharacterTypeMask(CharacterType.SecurityCamera)) != 0)
		{
			Fail();
		}
	}

	private void OnCameraAlarmSounded(object sender)
	{
		Fail();
	}
}
