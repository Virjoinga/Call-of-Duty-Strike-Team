using System.Collections;
using UnityEngine;

public class FirstPersonCameraTrackCommand : Command
{
	public Transform Target;

	public float Seconds;

	public bool ForceADS;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		Actor actor = GameController.Instance.mFirstPersonActor;
		if (actor != null)
		{
			GameController.Instance.SetFirstPersonPointOfInterest(Target);
			if (ForceADS)
			{
				actor.realCharacter.IsAimingDownSights = true;
			}
			if (!ForceADS)
			{
				actor.realCharacter.IsAimingDownSights = false;
			}
			yield return new WaitForSeconds(Seconds);
			GameController.Instance.ClearFirstPersonPointOfInterest();
		}
	}
}
