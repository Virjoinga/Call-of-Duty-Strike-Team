using System;

[Serializable]
public class RSProjectileDamageDefenceInterface
{
	private bool mIsHuman;

	private bool mIsAGR;

	public bool IsHuman
	{
		get
		{
			return mIsHuman;
		}
	}

	public bool IsAGR
	{
		get
		{
			return mIsAGR;
		}
	}

	public void Initialise(bool isHuman, bool isAGR)
	{
		mIsHuman = isHuman;
		mIsAGR = isAGR;
	}
}
