using System;

public class ChallengeEventArgs : EventArgs
{
	public ChallengeData Challenge { get; private set; }

	public ChallengeEventArgs(ChallengeData challenge)
	{
		Challenge = challenge;
	}
}
