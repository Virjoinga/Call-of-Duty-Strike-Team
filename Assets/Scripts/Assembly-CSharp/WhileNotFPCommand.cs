using System.Collections;

public class WhileNotFPCommand : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (HudStateController.Instance.State != HudStateController.HudState.FPP)
		{
			yield return null;
		}
	}
}
