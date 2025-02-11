using System.Collections;
using UnityEngine;

public class TimerObjective : MissionObjective
{
	public TimedData m_TimedInterface;

	public override void Start()
	{
		base.Start();
		if (m_TimedInterface.TimeInSeconds == 0f)
		{
			Debug.LogWarning("Time In Seconds is ZERO! for timer objective setting time to 60 seconds: " + base.name);
			m_TimedInterface.TimeInSeconds = 60f;
		}
		else
		{
			StartCoroutine(WaitForTimeToExpire());
		}
	}

	private IEnumerator WaitForTimeToExpire()
	{
		while (CommonHudController.Instance == null || base.State == ObjectiveState.Dormant)
		{
			yield return null;
		}
		if (m_TimedInterface.CountDown)
		{
			CommonHudController.Instance.MissionTimer.Set(m_TimedInterface.TimeInSeconds, 0f);
		}
		else
		{
			CommonHudController.Instance.MissionTimer.Set(0f, m_TimedInterface.TimeInSeconds);
		}
		CommonHudController.Instance.MissionTimer.StartTimer();
		while (CommonHudController.Instance.MissionTimer.CurrentState != MissionTimer.TimerState.Finished)
		{
			yield return null;
		}
		if (m_TimedInterface.PassOnTimerComplete)
		{
			Pass();
		}
		else
		{
			Fail();
		}
		CommonHudController.Instance.MissionTimer.StopTimer();
	}
}
