public class EnemyAlertedObjective : MissionObjective
{
	private bool mSetOff;

	public override void Start()
	{
		mMissionPassIfNotFail = true;
		base.Start();
		GameplayController.Instance().OnEnemyAlarmed += OnEnemyAlarmed;
	}

	private void OnEnemyAlarmed(object sender)
	{
		if (base.State == ObjectiveState.InProgress)
		{
			if (!mSetOff)
			{
				Fail();
			}
			mSetOff = true;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnEnemyAlarmed -= OnEnemyAlarmed;
		}
	}
}
