using System.Collections;

public class LockFPPMovementCommand : Command
{
	public bool LockMove;

	public bool LockLook;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TutorialToggles.LockFPPMovement = LockMove;
		TutorialToggles.LockFPPLook = LockLook;
		yield break;
	}
}
