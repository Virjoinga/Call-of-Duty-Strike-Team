using UnityEngine;

public class PerceptedEnemy
{
	private RealCharacter mTarget;

	private Vector3 mLastKnownPosition;

	public RealCharacter Target
	{
		get
		{
			return mTarget;
		}
	}

	public Vector3 LastKnownPosition
	{
		get
		{
			return mLastKnownPosition;
		}
		set
		{
			mLastKnownPosition = value;
		}
	}

	public PerceptedEnemy(RealCharacter target, Vector3 lastKnownPosition)
	{
		mTarget = target;
		mLastKnownPosition = lastKnownPosition;
	}
}
