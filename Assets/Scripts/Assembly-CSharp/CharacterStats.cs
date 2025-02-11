public class CharacterStats : Stats<CharacterStat>
{
	protected override void SetEventListeners()
	{
		EventHub.Instance.OnKill += Kill;
		EventHub.Instance.OnWeaponFired += WeaponFired;
		EventHub.Instance.OnCharacterHealed += CharacterHealed;
	}

	public override void Create()
	{
		for (int i = 0; i < 4; i++)
		{
			CreateStat(StatsManager.ConvertSoldierIndexToId(i));
		}
	}

	private void Kill(object sender, Events.Kill args)
	{
		if (args.Victim.PlayerControlled)
		{
			CharacterStat currentMissionStat = GetCurrentMissionStat(args.Victim.Id);
			currentMissionStat.NumTimesKilled++;
			if (args.Victim.IsFirstPerson || args.Victim.WasFirstPersonWhenMortallyWounded)
			{
				currentMissionStat.NumTimesKilledInFP++;
			}
		}
		if (args.Attacker == null || !args.Attacker.PlayerControlled || args.Victim.PlayerControlled)
		{
			return;
		}
		CharacterStat currentMissionStat2 = GetCurrentMissionStat(args.Attacker.Id);
		currentMissionStat2.NumKills++;
		if (args.HeadShot)
		{
			currentMissionStat2.NumHeadShots++;
		}
		if (args.Attacker.IsFirstPerson)
		{
			currentMissionStat2.NumKillsInFP++;
			if (args.HeadShot)
			{
				currentMissionStat2.NumHeadShotsInFP++;
			}
		}
	}

	private void WeaponFired(object sender, Events.WeaponFired args)
	{
		if (!args.Attacker.PlayerControlled || !args.Attacker.IsFirstPerson)
		{
			return;
		}
		CharacterStat currentMissionStat = GetCurrentMissionStat(args.Attacker.Id);
		if (args.Attacker.WeaponAccuracyStatAdjustment == 1f || args.HitTarget)
		{
			currentMissionStat.NumShotsFiredInFP++;
		}
		else
		{
			currentMissionStat.AccumulatedShotCount += args.Attacker.WeaponAccuracyStatAdjustment;
			if (currentMissionStat.AccumulatedShotCount >= 1f)
			{
				currentMissionStat.AccumulatedShotCount -= 1f;
				currentMissionStat.NumShotsFiredInFP++;
			}
		}
		if (args.HitTarget)
		{
			currentMissionStat.NumShotsHitInFP++;
		}
	}

	private void CharacterHealed(object sender, Events.CharacterHealed args)
	{
		if (args.HealedCharacter != null && !string.IsNullOrEmpty(args.HealedCharacter.Id))
		{
			CharacterStat currentMissionStat = GetCurrentMissionStat(args.HealedCharacter.Id);
			currentMissionStat.NumTimesHealed++;
		}
	}
}
