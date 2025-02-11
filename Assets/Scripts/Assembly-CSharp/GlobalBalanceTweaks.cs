using UnityEngine;

public static class GlobalBalanceTweaks
{
	public const float CrouchNeglectDelay = 2f;

	public const float RunForCoverNeglectDelay = 4f;

	public const float kGrenadeReactionTime = 1f;

	public const float kMinEarliestFixedGunReUseDelay = 2f;

	public const float kMaxEarliestFixedGunReUseDelay = 4f;

	public const float kAttackThreatMultiplier = 0f;

	public const float kAttackThreatHalfLife = 5f;

	public const float kTargetPoppedUpFromCoverOffensiveMod = 0.75f;

	public const float kTargetDownInCoverOffensiveMod = 0.25f;

	public const float kTargetDownInCoverDefensiveMod = 0.1f;

	public const float kIHaveCoverAgainstThemDefensiveMod = 0.5f;

	public const float kIHaveNoCoverAgainstThemDefensiveMod = 2f;

	public const float kCoverScoreTolerance = 500f;

	public const float kPlayerDistanceThreatModifier = 1f / 15f;

	public const float kNPCDistanceThreatModifier = 0.1f;

	public const float kEngagementDuration = 5f;

	public const float kEngagementDelay = 0.25f;

	public const float kDamageDuringAimedShotPreventsFiringFor = 2f;

	public const float kSuppressionTrapsSoldiersInCoverFor = 4f;

	public const float kTimeInvulnerableForAfterFullyHealing = 2f;

	public const float kTimeInvulnerableForAfterRespottingInGMG = 4f;

	public static float kCMEnemySingleTapDelay = 0.1f;

	public static float[,] PenaltyToAccuracyScale = new float[2, 2]
	{
		{ 0.05f, 1f },
		{ 0.2f, 0.4f }
	};

	public static float[,] BaseAccuracyModifier = new float[2, 2]
	{
		{ 0.15f, 0.25f },
		{ 0f, 0f }
	};

	public static int[,] TolerableInaccuracyCount = new int[2, 2]
	{
		{ 8, 4 },
		{ 4, 4 }
	};

	public static float[,] MinimumDodgeTime = new float[2, 2]
	{
		{ 4f, 1f },
		{ 4f, 3f }
	};

	public static float[,] MaximumDodgeTime = new float[2, 2]
	{
		{ 8f, 3f },
		{ 8f, 7f }
	};

	public static float[] EventCosts = new float[7] { 0.3f, 0.2f, 0.1f, -0.5f, -0.75f, -0.5f, -0.5f };

	public static float[] VetEventCosts = new float[7] { 1f, 0.5f, 0.1f, -0.5f, -0.75f, -0.5f, -0.5f };

	public static float kAimedShotDuration = 1000f;

	public static float kAimedShotDelay = 0.5f;

	public static bool kPerWeaponAimedShotInfluence = false;

	public static float[] kAimedShot_RangeInfluence = new float[9] { 0.1f, 0.2f, 0.5f, 0.02f, 0.15f, 0.1f, 0.07f, 0f, 0f };

	public static float[] kAimedShot_CloseRange = new float[9] { 4f, 8f, 8f, 0f, 4f, 4f, 4f, 0f, 0f };

	public static float[] kAimedShot_GunnerMovementInfluence = new float[9] { 1f, 0.3f, 0.1f, 2f, 1f, 1f, 1f, 0f, 0f };

	public static float[] kAimedShot_TargetMovementInfluence = new float[9] { 1f, 0.3f, 0.1f, 1f, 1f, 1f, 1f, 0f, 0f };

	public static float kAimedShot_ResetAfterNoVisFor = 3f;

	public static float kAimedShot_MinTimeScale = 0.01f;

	private static float[] EnemyDamageTakenThirdPersonModifier = new float[2] { 0.19f, 0.08f };

	private static float[] PlayerDamageTakenThirdPersonModifier = new float[2] { 2f, 4f };

	private static float playerDamageTakenFirstPersonModifier = 1f;

	public static bool TweaksEnabled = true;

	public static int DifficultySettingsModifier = 0;

	private static int DifficultyIndex
	{
		get
		{
			return (ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran) ? 1 : 0;
		}
	}

	public static float kPenaltyToAccuracyScale
	{
		get
		{
			return PenaltyToAccuracyScale[DifficultySettingsModifier, DifficultyIndex];
		}
	}

	public static float kBaseAccuracyModifier
	{
		get
		{
			return BaseAccuracyModifier[DifficultySettingsModifier, DifficultyIndex];
		}
	}

	public static float kTolerableInaccuracyCount
	{
		get
		{
			if (GameController.Instance != null && GameController.Instance.IsFirstPerson)
			{
				return 4f;
			}
			return TolerableInaccuracyCount[DifficultySettingsModifier, DifficultyIndex];
		}
	}

	public static float kMinimumDodgeTime
	{
		get
		{
			return MinimumDodgeTime[DifficultySettingsModifier, DifficultyIndex];
		}
	}

	public static float kMaximumDodgeTime
	{
		get
		{
			return MaximumDodgeTime[DifficultySettingsModifier, DifficultyIndex];
		}
	}

	public static float[] eventCosts
	{
		get
		{
			return (DifficultyIndex <= 0) ? EventCosts : VetEventCosts;
		}
	}

	private static float enemyDamageTakenThirdPersonModifier
	{
		get
		{
			return EnemyDamageTakenThirdPersonModifier[DifficultyIndex];
		}
	}

	private static float playerDamageTakenThirdPersonModifier
	{
		get
		{
			return PlayerDamageTakenThirdPersonModifier[DifficultyIndex];
		}
	}

	public static float AimedShotRate(Actor gunner, Actor target)
	{
		if (gunner == null || target == null || gunner.weapon == null || gunner.weapon.ActiveWeapon == null)
		{
			return 1f;
		}
		float num = 1f;
		int num2 = 0;
		if (kPerWeaponAimedShotInfluence)
		{
			num2 = (int)(1 + gunner.weapon.ActiveWeapon.GetClass());
		}
		if (gunner.baseCharacter.IsMoving())
		{
			num /= 1f + kAimedShot_GunnerMovementInfluence[num2];
		}
		if (target.baseCharacter.IsMoving() || (target.realCharacter != null && target.realCharacter.IsBeingMovedManually()))
		{
			num /= 1f + kAimedShot_TargetMovementInfluence[num2];
		}
		float num3 = Mathf.Max(0f, (gunner.GetPosition() - target.GetPosition()).magnitude - kAimedShot_CloseRange[num2]);
		num /= 1f + num3 * kAimedShot_RangeInfluence[num2];
		return Mathf.Max(num, kAimedShot_MinTimeScale);
	}

	public static void ApplyDamageTweaks(string damageType, GameObject victim, GameObject culprit, ref float amount)
	{
		if (!TweaksEnabled)
		{
			return;
		}
		Actor actor = null;
		if (victim != null)
		{
			actor = victim.GetComponent<Actor>();
		}
		Actor actor2 = null;
		if (culprit != null)
		{
			actor2 = culprit.GetComponent<Actor>();
		}
		if (!(damageType == "Shot") || !(actor != null) || !(actor.behaviour != null))
		{
			return;
		}
		if (actor.behaviour.PlayerControlled)
		{
			if (!(actor2 != null) || !(actor2.weapon != null) || actor2.weapon.ActiveWeapon == null || actor2.weapon.ActiveWeapon.GetClass() != WeaponDescriptor.WeaponClass.SniperRifle)
			{
				if (actor == GameController.Instance.mFirstPersonActor)
				{
					amount *= playerDamageTakenFirstPersonModifier;
				}
				else
				{
					amount *= playerDamageTakenThirdPersonModifier;
				}
				amount = GMGBalanceTweaks.Instance.GMGModifier_EnemyDamage(amount);
			}
		}
		else if (actor2 != null && actor2 != GameController.Instance.mFirstPersonActor && actor2.awareness.ChDefCharacterType == CharacterType.Human)
		{
			amount *= enemyDamageTakenThirdPersonModifier;
		}
	}

	public static float GetExtraBurstPause(BaseCharacter ch)
	{
		if (ch != null && ch.myActor.behaviour != null && ch.myActor.behaviour.PlayerControlled)
		{
			return Random.Range(1f, 2f);
		}
		if (DifficultyIndex > 0)
		{
			return Random.Range(2f, 3f);
		}
		return Random.Range(2.5f, 3.5f);
	}
}
