using UnityEngine;

[AddComponentMenu("FingerGestures/Gesture Recognizers/Tap (Multi)")]
public class MultiTapGestureRecognizer : AveragedGestureRecognizer
{
	public int RequiredTaps;

	public bool RaiseEventOnEachTap;

	public float MaxDelayBetweenTaps = 0.25f;

	public float MaxDuration;

	public float MoveTolerance = 5f;

	private int taps;

	private bool down;

	private float lastDownTime;

	private float lastTapTime;

	private float lastUpTime;

	public int Taps
	{
		get
		{
			return taps;
		}
	}

	public event EventDelegate<MultiTapGestureRecognizer> OnTap;

	private bool HasTimedOut()
	{
		if (MaxDelayBetweenTaps > 0f && Time.time - lastTapTime > MaxDelayBetweenTaps)
		{
			return true;
		}
		if (MaxDuration > 0f && base.ElapsedTime > MaxDuration)
		{
			return true;
		}
		return false;
	}

	protected override void Reset()
	{
		taps = 0;
		down = false;
		base.Reset();
	}

	protected override void OnBegin(FingerGestures.IFingerList touches)
	{
		base.Position = touches.GetAveragePosition();
		base.StartPosition = base.Position;
		lastTapTime = Time.time;
		lastDownTime = Time.time;
	}

	protected override GestureState OnActive(FingerGestures.IFingerList touches)
	{
		down = false;
		if (touches.Count == RequiredFingerCount)
		{
			down = true;
			lastDownTime = Time.time;
		}
		else if (touches.Count == 0)
		{
			down = false;
			lastUpTime = Time.time;
		}
		else if (touches.Count < RequiredFingerCount)
		{
			if (Time.time - lastDownTime > 0.25f)
			{
				base.FailReason = "too many fingers at start";
				return GestureState.Failed;
			}
		}
		else if (!Young(touches))
		{
			base.FailReason = "fingers added";
			return GestureState.Failed;
		}
		if (HasTimedOut())
		{
			if (RequiredTaps == 0 && Taps > 0)
			{
				if (!RaiseEventOnEachTap)
				{
					RaiseOnTap();
				}
				return GestureState.Recognized;
			}
			base.FailReason = "time out";
			return GestureState.Failed;
		}
		if (down)
		{
			float num = Vector3.SqrMagnitude(touches.GetAveragePosition() - base.StartPosition);
			if (num >= MoveTolerance * MoveTolerance)
			{
				base.FailReason = "moved too far";
				return GestureState.Failed;
			}
		}
		if (lastUpTime > lastDownTime && lastDownTime >= lastTapTime)
		{
			taps++;
			lastTapTime = Time.time;
			if (RequiredTaps > 0 && taps >= RequiredTaps)
			{
				RaiseOnTap();
				return GestureState.Recognized;
			}
			if (RaiseEventOnEachTap)
			{
				RaiseOnTap();
			}
		}
		return GestureState.InProgress;
	}

	protected void RaiseOnTap()
	{
		if (this.OnTap != null)
		{
			this.OnTap(this);
		}
	}
}
