using System.Collections;

public class ForceCrouchCommand : Command
{
	public bool ForceCrouch;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		if (ForceCrouch)
		{
			if (CommonHudController.Instance.CrouchButton.gameObject.activeInHierarchy)
			{
				CommonHudController.Instance.Crouch();
			}
			else
			{
				GameController.Instance.mFirstPersonActor.realCharacter.Crouch();
			}
		}
		else if (CommonHudController.Instance.CrouchButton.gameObject.activeInHierarchy)
		{
			CommonHudController.Instance.Stand();
		}
		else
		{
			GameController.Instance.mFirstPersonActor.realCharacter.Stand();
		}
	}
}
