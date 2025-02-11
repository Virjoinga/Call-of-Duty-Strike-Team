public class CMHackableObjectSentryGun : CMHackableObject
{
	public Actor SentryGunActor;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override bool ShouldPopulateMenuItems()
	{
		return SentryGunActor == null || !SentryGunActor.baseCharacter.IsDead();
	}

	protected override bool ShouldShowKillOption()
	{
		return true;
	}
}
