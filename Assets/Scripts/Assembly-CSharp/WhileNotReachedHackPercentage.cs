using System.Collections;

public class WhileNotReachedHackPercentage : Command
{
	public GuidRef TerminalRefToCheck;

	public float PercentageToWaitFor;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (TerminalRefToCheck == null || !(TerminalRefToCheck.theObject != null))
		{
			yield break;
		}
		HackableObject terminal = IncludeDisabled.GetComponentInChildren<HackableObject>(TerminalRefToCheck.theObject);
		if (terminal != null)
		{
			while (terminal.HackedPercentage < PercentageToWaitFor)
			{
				yield return null;
			}
		}
	}
}
