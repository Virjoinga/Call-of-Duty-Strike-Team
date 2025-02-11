public class SquadCasualtiesObjective : MissionObjective
{
	public int MaxPlayerCasualties = 3;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public override void Start()
	{
		base.Start();
		mMissionPassIfNotFail = true;
		GameplayController.Instance().OnPlayerCharacterDead += OnPlayerCharacterDead;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnPlayerCharacterDead -= OnPlayerCharacterDead;
		}
	}

	private void OnPlayerCharacterDead(object sender)
	{
		int num = 0;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.realCharacter.IsDead())
			{
				num++;
			}
		}
		if (num >= MaxPlayerCasualties)
		{
			Fail();
		}
	}

	public void ForceFail()
	{
		Fail();
	}
}
