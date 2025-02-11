using System.Collections;

public class LockTPPMovementCommand : Command
{
	public enum MovementSpeed
	{
		DontLock = 0,
		Walk = 1,
		Run = 2
	}

	public bool LockRegularMovement;

	public bool LockGhostMovement;

	public MovementSpeed LockToSpeed;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TutorialToggles.enableGhostDrag = !LockGhostMovement;
		TutorialToggles.enableTapToMove = !LockRegularMovement;
		TutorialToggles.LockToWalkOnly = LockToSpeed == MovementSpeed.Walk;
		TutorialToggles.LockToRunOnly = LockToSpeed == MovementSpeed.Run;
		yield break;
	}
}
