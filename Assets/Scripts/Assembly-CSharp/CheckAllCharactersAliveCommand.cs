using System.Collections;
using UnityEngine;

public class CheckAllCharactersAliveCommand : Command
{
	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		ActorIdentIterator aii2 = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		bool bThrowMessage = false;
		Actor pc;
		while (aii2.NextActor(out pc))
		{
			if (pc.realCharacter.IsMortallyWounded())
			{
				bThrowMessage = true;
			}
		}
		if (!bThrowMessage)
		{
			yield break;
		}
		FrontEndController.Instance.TransitionTo(ScreenID.ContinueScreen);
		yield return new WaitForSeconds(1f);
		while (FrontEndController.Instance.ActiveScreen == ScreenID.ContinueScreen)
		{
			yield return null;
		}
		bool bRevived = true;
		aii2 = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		while (aii2.NextActor(out pc))
		{
			if (pc.realCharacter.IsDead())
			{
				bRevived = false;
			}
		}
		if (!bRevived)
		{
			yield return new WaitForSeconds(5f);
		}
	}
}
