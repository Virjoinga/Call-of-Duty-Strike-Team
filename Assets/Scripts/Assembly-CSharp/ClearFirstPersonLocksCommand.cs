using System.Collections;

public class ClearFirstPersonLocksCommand : Command
{
	public bool ExitFirstPerson;

	public bool Transitions = true;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.IsLockedToFirstPerson = false;
		GameController.Instance.IsLockedToCurrentCharacter = false;
		if (ExitFirstPerson)
		{
			GameController.Instance.ExitFirstPerson(Transitions);
		}
		yield break;
	}
}
