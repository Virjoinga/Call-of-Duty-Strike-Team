using UnityEngine;

public class SwitchLight : MonoBehaviour
{
	public enum LightState
	{
		Off = 0,
		On = 1,
		Timed = 2
	}

	public enum TimeState
	{
		None = 0,
		High = 1,
		Med = 2,
		Low = 3,
		Critical = 4
	}

	private TimeState timeState;

	private float TotalCountDown;

	private float CountDown;

	private HaloEffect fx;

	public HaloEffect.HaloColour onColour = HaloEffect.HaloColour.Green;

	public HaloEffect.HaloColour offColour;

	private void Awake()
	{
		fx = GetComponent<HaloEffect>();
	}

	private void Start()
	{
		timeState = TimeState.None;
		TotalCountDown = 0f;
		CountDown = 0f;
		ChangeState(LightState.On, TimeState.None);
	}

	private void Update()
	{
		if (!(fx != null) || timeState == TimeState.None)
		{
			return;
		}
		if (CountDown > 0f)
		{
			CountDown -= Time.deltaTime;
			float num = CountDown / TotalCountDown;
			if (num > 0f)
			{
				fx.SetBlinkSpeed(num);
			}
		}
		else
		{
			CountDown = 0f;
			ChangeState(LightState.On, TimeState.None);
		}
	}

	private void ChangeState(LightState ls, TimeState ts)
	{
		switch (ls)
		{
		case LightState.Off:
			fx.SetColour(offColour);
			break;
		case LightState.On:
			fx.SetColour(onColour);
			break;
		}
		if (ts != 0)
		{
			fx.SetColour(HaloEffect.HaloColour.Red);
		}
		switch (ts)
		{
		case TimeState.None:
			fx.SetBlinkPattern(HaloEffect.BlinkPattern.On);
			break;
		case TimeState.High:
			fx.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
			break;
		case TimeState.Med:
			fx.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
			break;
		case TimeState.Low:
			fx.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkMedium);
			break;
		case TimeState.Critical:
			fx.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkFast);
			break;
		}
		timeState = ts;
	}

	public void Activate()
	{
		ChangeState(LightState.On, TimeState.None);
	}

	public void Deactivate()
	{
		ChangeState(LightState.Off, TimeState.None);
	}

	public void StartTimed(float time)
	{
		ChangeState(LightState.Off, TimeState.High);
		CountDown = time;
		TotalCountDown = time;
	}
}
