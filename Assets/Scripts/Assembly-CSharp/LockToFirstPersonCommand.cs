using System.Collections;

public class LockToFirstPersonCommand : Command
{
	public bool LockToCurrentCharacter;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.IsLockedToFirstPerson = true;
		GameController.Instance.IsLockedToCurrentCharacter = LockToCurrentCharacter;
		yield break;
	}
}
