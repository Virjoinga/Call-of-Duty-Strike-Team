using System.Collections;

public class SuppressHUDCommand : Command
{
	public bool SuppressHUD = true;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.SuppressHud(SuppressHUD);
		}
		yield break;
	}
}
