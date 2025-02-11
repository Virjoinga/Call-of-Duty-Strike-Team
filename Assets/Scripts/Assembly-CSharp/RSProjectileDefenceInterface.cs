using System;

[Serializable]
public class RSProjectileDefenceInterface
{
	private bool mInCoverHiding;

	private bool mInCoverShooting;

	private bool mMovingIntoCover;

	private bool mIsDodging;

	public bool InCoverHiding
	{
		get
		{
			return mInCoverHiding;
		}
	}

	public bool InCoverShooting
	{
		get
		{
			return mInCoverShooting;
		}
	}

	public bool IsMovingIntoCover
	{
		get
		{
			return mMovingIntoCover;
		}
	}

	public bool IsDodging
	{
		get
		{
			return mIsDodging;
		}
	}

	public void Initialise(bool inCoverHiding, bool inCoverShooting, bool movingIntoCover, bool isDodging)
	{
		mInCoverHiding = inCoverHiding;
		mInCoverShooting = inCoverShooting;
		mMovingIntoCover = movingIntoCover;
		mIsDodging = isDodging;
	}
}
