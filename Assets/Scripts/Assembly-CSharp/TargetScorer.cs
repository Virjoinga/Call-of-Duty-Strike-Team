using UnityEngine;

public static class TargetScorer
{
	private const float kKeepTargetBonusScale = 100f;

	private const float kKeepTargetBonusDuration = 2f;

	private static float[,] threatScore = new float[32, 32];

	private static float[,] lastAttack = new float[32, 32];

	private static float[] scoreModifier = new float[32];

	private static ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private static uint[] LastBestTargetIdent = new uint[32];

	private static float[] KeepTargetBonus = new float[32];

	public static float QuickRoot(float sqr)
	{
		return Mathf.Sqrt(sqr);
	}

	public static float EstimateWeaponDamageAtSquareDistance(Actor a, float sqrDist, bool isPlayer)
	{
		if (a == null || a.weapon == null || a.weapon.ActiveWeapon == null)
		{
			return 0f;
		}
		float distance = QuickRoot(sqrDist);
		IWeaponAI weaponAI = WeaponUtils.GetWeaponAI(a.weapon.ActiveWeapon);
		if (weaponAI == null)
		{
			return 0f;
		}
		return weaponAI.CalculateDamage(distance, null, isPlayer);
	}

	public static float EstimateWeaponDamageWithUtilityAtSquareDistance(Actor a, float sqrDist, bool isPlayer)
	{
		if (a == null || a.weapon == null || a.weapon.ActiveWeapon == null)
		{
			return 0f;
		}
		float distance = QuickRoot(sqrDist);
		IWeaponAI weaponAI = WeaponUtils.GetWeaponAI(a.weapon.ActiveWeapon);
		if (weaponAI == null)
		{
			return 0f;
		}
		return weaponAI.CalculateDamage(distance, null, isPlayer) * weaponAI.CalculateUtility(distance);
	}

	public static void FlushScores()
	{
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				threatScore[i, j] = 0f;
				lastAttack[i, j] = -1000f;
			}
		}
	}

	public static void Expunge(int actorIndex)
	{
		for (int i = 0; i < 32; i++)
		{
			threatScore[actorIndex, i] = 0f;
			threatScore[i, actorIndex] = 0f;
		}
	}

	public static void CalculateDistanceScores(Actor a, Actor b, float sqrDist)
	{
		float num = QuickRoot(sqrDist);
		float num2 = ((!(a.behaviour != null) || !a.behaviour.PlayerControlled) ? 0.1f : (1f / 15f));
		float num3 = ((!(b.behaviour != null) || !b.behaviour.PlayerControlled) ? 0.1f : (1f / 15f));
		if (a.weapon != null && a.weapon.ActiveWeapon != null)
		{
			IWeaponAI weaponAI = WeaponUtils.GetWeaponAI(a.weapon.ActiveWeapon);
			float num4 = ((weaponAI == null) ? 0f : (weaponAI.CalculateDamage(num * 0.8f, null, false) + weaponAI.CalculateDamage(num * 1.3f, null, false)));
			threatScore[a.quickIndex, b.quickIndex] = num4 / (1f + num * num2);
		}
		if (b.weapon != null && b.weapon.ActiveWeapon != null)
		{
			IWeaponAI weaponAI = WeaponUtils.GetWeaponAI(b.weapon.ActiveWeapon);
			float num4 = ((weaponAI == null) ? 0f : (weaponAI.CalculateDamage(num * 0.8f, null, false) + weaponAI.CalculateDamage(num * 1.3f, null, false)));
			threatScore[b.quickIndex, a.quickIndex] = num4 / (1f + num * num3);
		}
	}

	public static void RecordAttack(Actor a, Actor b)
	{
		lastAttack[a.quickIndex, b.quickIndex] = Time.time - 5f;
	}

	public static Actor GetBestTarget(Actor a)
	{
		uint num = a.awareness.EnemiesIKnowAboutRecent & GKM.AliveMask;
		if (a.behaviour.PlayerControlled)
		{
			num &= GKM.AlertedMask;
		}
		return GetBestTargetFromMask(a, num);
	}

	public static Actor GetBestTargetFromMask(Actor a, uint mask)
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(mask);
		Actor actor = null;
		float num = 0f;
		Actor a2;
		while (actorIdentIterator.NextActor(out a2))
		{
			if (a2.baseCharacter.IsMortallyWounded() || a2.awareness.ChDefCharacterType == CharacterType.SentryGun || (a2.realCharacter != null && a2.realCharacter.IsSniper) || (a2.behaviour != null && !a2.behaviour.PlayerControlled && a2.realCharacter != null && a2.realCharacter.IsUsingFixedGun))
			{
				continue;
			}
			float num2 = threatScore[a.quickIndex, a2.quickIndex];
			float num3 = threatScore[a2.quickIndex, a.quickIndex];
			float num4 = 0f;
			float num5 = scoreModifier[a2.quickIndex];
			float num6 = 0f;
			if (LastBestTargetIdent[a.quickIndex] == a2.ident)
			{
				num6 = Mathf.Max(KeepTargetBonus[a.quickIndex] - Time.time, 0f) * 100f;
				num2 += num6 * 5f;
				num5 += num6;
			}
			if (num6 == 0f && (a.awareness.EnemiesICanSee & a2.ident) == 0)
			{
				num2 *= 0.5f;
				num3 *= 0.5f;
			}
			if (!a.behaviour.PlayerControlled)
			{
				num4 = num3 * (0f / (Time.time - lastAttack[a2.quickIndex, a.quickIndex]));
			}
			if (a2.awareness.isInCover && (a2.awareness.coverBooked.noCoverAgainst & a.ident) == 0)
			{
				if ((a2.awareness.coverBooked.lowCoverAgainst & a.ident) != 0)
				{
					if (a2.awareness.PoppedOutOfCover)
					{
						num2 *= 0.75f;
					}
					else
					{
						num2 *= 0.25f;
						num3 *= 0.1f;
					}
				}
				else if ((a2.awareness.coverBooked.highCoverAgainst & a.ident) != 0)
				{
					if (a2.awareness.PoppedOutOfCover)
					{
						num2 *= 0.75f;
					}
					else
					{
						num2 *= 0.25f;
						num3 *= 0.1f;
					}
				}
			}
			if (a.awareness.isInCover)
			{
				num3 = (((a.awareness.coverBooked.noCoverAgainst & a2.ident) != 0) ? (num3 * 2f) : (num3 * 0.5f));
			}
			float num7 = num2 + num3 + num4 + num5;
			if (num7 > num)
			{
				actor = a2;
				num = num7;
			}
		}
		if (actor == null)
		{
			LastBestTargetIdent[a.quickIndex] = 0u;
		}
		else if (actor.ident != LastBestTargetIdent[a.quickIndex])
		{
			LastBestTargetIdent[a.quickIndex] = actor.ident;
			KeepTargetBonus[a.quickIndex] = Time.time + 2f;
		}
		return actor;
	}

	public static void SetActorModifierScore(Actor a, float val)
	{
		scoreModifier[a.quickIndex] = val;
	}
}
