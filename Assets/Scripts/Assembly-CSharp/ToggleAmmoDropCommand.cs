using System.Collections;

public class ToggleAmmoDropCommand : Command
{
	public bool Disable = true;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		AmmoDropManager.Instance.DisableAmmoPickups = Disable;
		yield break;
	}
}
