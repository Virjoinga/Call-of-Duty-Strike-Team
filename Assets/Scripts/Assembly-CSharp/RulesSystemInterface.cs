public class RulesSystemInterface : BaseActorComponent
{
	public RSProjectileAttackInterface ProjectileAttackInterface = new RSProjectileAttackInterface();

	public RSProjectileAttackDamageInterface ProjectileAttackDamageInterface = new RSProjectileAttackDamageInterface();

	public RSProjectileDefenceInterface ProjectileDefenceInterface = new RSProjectileDefenceInterface();

	public RSProjectileDamageDefenceInterface ProjectileDamageDefenceInterface = new RSProjectileDamageDefenceInterface();

	public RSMeleeAttackDamageInterface MeleeAttackDamageInterface = new RSMeleeAttackDamageInterface();

	public RSMeleeDamageDefenceInterface MeleeDamageDefenceInterface = new RSMeleeDamageDefenceInterface();

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void RefreshProjectileAttackDamageInterface(bool isExplosion, bool isCritical, bool isFlanking, float weaponDamageMultiplier)
	{
		ProjectileAttackDamageInterface.Initialise(isExplosion, isCritical, isFlanking, weaponDamageMultiplier);
	}

	public void RefreshProjectileDefenceInterface()
	{
		bool inCoverHiding = false;
		bool inCoverShooting = false;
		bool movingIntoCover = false;
		bool isDodging = false;
		if (myActor != null)
		{
			if (myActor.realCharacter.IsUsingFixedGun)
			{
				inCoverShooting = true;
			}
			else if (myActor.realCharacter.IsFirstPerson)
			{
				if (GameController.Instance.FirstPersonCoverPoint != null)
				{
					if (myActor.weapon.IsFiring())
					{
						inCoverShooting = true;
					}
					else
					{
						inCoverHiding = true;
					}
				}
				else if (myActor.realCharacter.IsCrouching())
				{
					inCoverShooting = true;
				}
				else if (myActor.realCharacter.IsRunning())
				{
					movingIntoCover = true;
				}
			}
			else
			{
				if (myActor.awareness.isInCover)
				{
					if (myActor.weapon.IsFiring())
					{
						inCoverShooting = true;
					}
					else
					{
						inCoverHiding = true;
					}
				}
				if (myActor.awareness.coverBooked != null && myActor.realCharacter.IsMoving())
				{
					movingIntoCover = true;
				}
			}
			isDodging = myActor.realCharacter.IsRunning();
		}
		ProjectileDefenceInterface.Initialise(inCoverHiding, inCoverShooting, movingIntoCover, isDodging);
	}

	public void RefreshProjectileDamageDefenceInterface()
	{
		bool isAGR = false;
		bool isHuman = false;
		if (myActor != null)
		{
			if (myActor.awareness.ChDefCharacterType == CharacterType.AutonomousGroundRobot)
			{
				isAGR = true;
			}
			else if (myActor.awareness.ChDefCharacterType == CharacterType.Human)
			{
				isHuman = true;
			}
		}
		ProjectileDamageDefenceInterface.Initialise(isHuman, isAGR);
	}
}
