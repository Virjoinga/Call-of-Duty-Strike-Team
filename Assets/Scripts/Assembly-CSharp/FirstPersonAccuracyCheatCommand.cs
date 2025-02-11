using System.Collections;

public class FirstPersonAccuracyCheatCommand : Command
{
	public enum CheatMode
	{
		Enable = 0,
		Reset = 1
	}

	public CheatMode Mode = CheatMode.Reset;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.FirstPersonAccuracyCheat = Mode == CheatMode.Enable;
		yield break;
	}
}
