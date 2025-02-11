using System.Collections;

public class FirstPersonSoftLockOverrideCommand : Command
{
	public enum OverrideMode
	{
		Disable = 0,
		Reset = 1
	}

	public OverrideMode Mode = OverrideMode.Reset;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.SoftLockDisabled = Mode == OverrideMode.Disable;
		yield break;
	}
}
