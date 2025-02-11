public class HackableObjectSentryGun : HackableObject
{
	public Actor SentryGunActor;

	private bool mHasFinished;

	private void Awake()
	{
		ShouldUseConsultant = true;
	}

	protected override void Consult()
	{
		if (!base.FullyHacked)
		{
			if (ObjectHasBecomeInvalid())
			{
				FailHackAttempt(true, false);
			}
		}
		else if (!mHasFinished)
		{
			mHasFinished = true;
			if (SentryGunActor != null)
			{
				SentryGunActor.awareness.faction = FactionHelper.Category.Player;
			}
		}
	}

	protected override bool ObjectHasBecomeInvalid()
	{
		return SentryGunActor == null || SentryGunActor.baseCharacter.IsDead();
	}
}
