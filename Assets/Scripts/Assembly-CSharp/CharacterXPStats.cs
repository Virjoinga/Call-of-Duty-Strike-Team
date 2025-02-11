using System.Collections.Generic;

public class CharacterXPStats : Stats<CharacterXP>
{
	private EnemyHitTracking m_EnemyTracking = new EnemyHitTracking();

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnKill += Kill;
		EventHub.Instance.OnWeaponFired += WeaponFired;
		EventHub.Instance.OnEndMission += MissionEnded;
		EventHub.Instance.OnStartMission += MissionStarted;
	}

	public override void Create()
	{
		for (int i = 0; i < 4; i++)
		{
			CreateStat(StatsManager.ConvertSoldierIndexToId(i));
		}
	}

	public int PlayerPurchasedSoldierXP(int soldierIndex)
	{
		CharacterXP currentMissionStat = GetCurrentMissionStat(StatsManager.ConvertSoldierIndexToId(soldierIndex));
		CharacterXP gameTotalStat = GetGameTotalStat(StatsManager.ConvertSoldierIndexToId(soldierIndex));
		currentMissionStat.HasBoughtBack = true;
		gameTotalStat.CombineStat(currentMissionStat);
		return currentMissionStat.TotalXP;
	}

	private void AddXPFromKill(CharacterXP xpStat, XPType xpType, string playerId, Events.EventActor victim)
	{
		float num = xpType.GetXP();
		num *= GetPerkXPModifier(xpStat);
		if (GMGData.Instance != null && GMGData.Instance.XPBonusRewardActive)
		{
			num *= GMGData.Instance.XPBonusMultiplier;
			CommonHudController.Instance.AddXpFeedback((int)num, Language.Get("S_MYSTERY_REWARD_13"), null);
		}
		xpStat.XPFromKills += (int)num;
		EventHub.Instance.Report(new Events.XPEarned((int)num, xpType.Identifier, playerId, victim));
	}

	private int AddXPFromBonusNoMessage(CharacterXP xpStat, XPType xpType)
	{
		float num = xpType.GetXP();
		num *= GetPerkXPModifier(xpStat);
		xpStat.XPFromBonuses += (int)num;
		xpStat.NumBonuses++;
		EventHub.Instance.Report(new Events.XPEarned((int)num, xpType.Identifier, string.Empty, null));
		return (int)num;
	}

	private void AddXPFromBonus(CharacterXP xpStat, XPType xpType)
	{
		float num = AddXPFromBonusNoMessage(xpStat, xpType);
		if (GMGData.Instance != null && GMGData.Instance.XPBonusRewardActive)
		{
			num *= GMGData.Instance.XPBonusMultiplier;
		}
		if (!IsThisTimeAttack())
		{
			if (GMGData.Instance != null && GMGData.Instance.XPBonusRewardActive)
			{
				CommonHudController.Instance.AddXpFeedback((int)num, Language.Get("S_MYSTERY_REWARD_13") + AutoLocalize.Get(xpType.Message), null);
			}
			else
			{
				CommonHudController.Instance.AddXpFeedback((int)num, AutoLocalize.Get(xpType.Message), null);
			}
		}
	}

	private bool IsThisTimeAttack()
	{
		if (SectionTypeHelper.IsAGMG() && GMGData.Instance.CurrentGameType == GMGData.GameType.TimeAttack)
		{
			return true;
		}
		return false;
	}

	private void MissionStarted(object sender, Events.StartMission args)
	{
		for (int i = 0; i < 4; i++)
		{
			CharacterXP currentMissionStat = GetCurrentMissionStat(StatsManager.ConvertSoldierIndexToId(i));
			currentMissionStat.HasBoughtBack = false;
		}
	}

	private void MissionEnded(object sender, Events.EndMission args)
	{
		if (args.Quit)
		{
			XPManager.Instance.Log().Add("Wiping character XP for mission as the player quit");
			for (int i = 0; i < 4; i++)
			{
				CharacterXP currentMissionStat = GetCurrentMissionStat(StatsManager.ConvertSoldierIndexToId(i));
				currentMissionStat.Reset();
			}
		}
	}

	private void WeaponFired(object sender, Events.WeaponFired args)
	{
		m_EnemyTracking.Update(args);
	}

	private void Kill(object sender, Events.Kill args)
	{
		if (args.Attacker != null && args.Attacker.PlayerControlled && !args.Victim.PlayerControlled)
		{
			CharacterXP currentMissionStat = GetCurrentMissionStat(args.Attacker.Id);
			if (args.Attacker.IsFirstPerson)
			{
				currentMissionStat.StoreKill(false);
				CheckEnemyHitTracking(currentMissionStat, args);
				CheckBonusHeadshot(currentMissionStat, args);
				CheckBonusExecution(currentMissionStat, args);
				CheckBonusLongshot(currentMissionStat, args);
				CheckBonusOneShotOneKill(currentMissionStat, args);
			}
			else
			{
				CheckStealthKill(currentMissionStat, args);
				CheckForAimedShot(currentMissionStat, args);
			}
			CheckKillOtherGun(currentMissionStat, args);
			CheckKillSentryGun(currentMissionStat, args);
			CheckKillMachineGunOrSniper(currentMissionStat, args);
			CheckKillMountedGun(currentMissionStat, args);
			CheckKillInWindow(currentMissionStat, args);
			CheckKillSmartBomb(currentMissionStat, args);
		}
		m_EnemyTracking.Update(args);
	}

	public override void SessionEnd()
	{
		base.SessionEnd();
		m_EnemyTracking.Reset();
		for (int i = 0; i < 4; i++)
		{
			CharacterXP currentMissionStat = GetCurrentMissionStat(StatsManager.ConvertSoldierIndexToId(i));
			currentMissionStat.ClearKillData();
		}
	}

	public override void Update()
	{
		foreach (KeyValuePair<string, CharacterXP> currentMissionStat in m_CurrentMissionStats)
		{
			CharacterXP value = currentMissionStat.Value;
			int num = value.CheckForMultiKills(false);
			if (num > 1)
			{
				if (num == 2)
				{
					AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_DoubleKill);
				}
				else if (num == 3)
				{
					AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_TripleKill);
				}
				else if (num >= 4)
				{
					AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_MultiKill);
				}
			}
			int num2 = value.CheckForMultiKills(true);
			if (num2 == 1)
			{
				AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_Backstabber);
			}
			else if (num2 == 2)
			{
				AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_Efficiency);
			}
			else if (num2 == 3)
			{
				AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_TrippleStealthKill);
			}
			else if (num2 >= 4)
			{
				AddXPFromBonus(value, XPManager.Instance.m_XPAwards.Bonus_FuryKill);
			}
		}
	}

	private float GetPerkXPModifier(CharacterXP xpStat)
	{
		if (GameSettings.Instance.PerksEnabled)
		{
			return StatsManager.Instance.PerksManagerNoAssert().GetModifierForPerk(PerkType.Hardline);
		}
		return 1f;
	}

	private void CheckKillOtherGun(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.AssaultRifle || args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.Shotgun || args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.SubMachineGun || args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.Pistol)
		{
			AddXPFromKill(xpStat, XPManager.Instance.m_XPAwards.Kill_OtherGun, args.Attacker.Id, args.Victim);
		}
	}

	private void CheckKillSentryGun(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim.CharacterType == CharacterType.SentryGun)
		{
			AddXPFromKill(xpStat, XPManager.Instance.m_XPAwards.Kill_DestroySentry, string.Empty, null);
		}
	}

	private void CheckKillMachineGunOrSniper(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.LightMachineGun || args.Victim.WeaponClass == WeaponDescriptor.WeaponClass.SniperRifle)
		{
			AddXPFromKill(xpStat, XPManager.Instance.m_XPAwards.Kill_MachineGunOrSniper, args.Attacker.Id, args.Victim);
		}
	}

	private void CheckKillMountedGun(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim.IsUsingFixedGun)
		{
			AddXPFromKill(xpStat, XPManager.Instance.m_XPAwards.Kill_MountedGun, args.Attacker.Id, args.Victim);
		}
	}

	private void CheckBonusHeadshot(CharacterXP xpStat, Events.Kill args)
	{
		if (args.HeadShot)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Headshot);
		}
	}

	private void CheckBonusExecution(CharacterXP xpStat, Events.Kill args)
	{
		if (args.HeadShot && args.Victim.IsInCover)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Execution);
		}
	}

	private void CheckBonusLongshot(CharacterXP xpStat, Events.Kill args)
	{
		if (args.LongRange)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Longshot);
		}
	}

	private void CheckBonusOneShotOneKill(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Attacker != null && args.Attacker.WeaponClass == WeaponDescriptor.WeaponClass.SniperRifle && args.OneShotKill && !args.GrenadeKill && !args.ClaymoreKill && !args.SilentKill && !HealthComponent.DamageIsFromExplosion(args.DamageType))
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_OneShotOneKill);
		}
	}

	private void CheckKillInWindow(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim.IsWindowLookout)
		{
			AddXPFromKill(xpStat, XPManager.Instance.m_XPAwards.Kill_Window, args.Attacker.Id, args.Victim);
		}
	}

	private void CheckKillSmartBomb(CharacterXP xpStat, Events.Kill args)
	{
		if (args.DamageType == "SmartBomb")
		{
			AddXPFromBonusNoMessage(xpStat, XPManager.Instance.m_XPAwards.Bonus_SmartBomb);
		}
	}

	private void CheckEnemyHitTracking(CharacterXP xpStat, Events.Kill args)
	{
		if (args.Victim == null || args.Attacker == null)
		{
			return;
		}
		EnemyHitTracking.EnemyData enemyData = m_EnemyTracking.GetEnemyData(args.Victim.Name);
		if (enemyData == null)
		{
			return;
		}
		foreach (string item in enemyData.PlayerCharactersHit)
		{
			if (item != args.Attacker.Id)
			{
				AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Payback);
			}
		}
		if (enemyData.HasIncapacitatedFriendlyUnit && args.Attacker.IsFirstPerson)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Avenger);
		}
		bool flag = false;
		foreach (string item2 in enemyData.DamagedBy)
		{
			if (item2 != args.Attacker.Id)
			{
				CharacterXP currentMissionStat = GetCurrentMissionStat(item2);
				AddXPFromBonusNoMessage(currentMissionStat, XPManager.Instance.m_XPAwards.Kill_AssistInKill);
				flag = true;
			}
		}
		if (flag && args.Attacker.IsFirstPerson)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_Backup);
		}
	}

	private void CheckForAimedShot(CharacterXP xpStat, Events.Kill args)
	{
		if (args.HeadShot)
		{
			AddXPFromBonus(xpStat, XPManager.Instance.m_XPAwards.Bonus_DeadEye);
		}
	}

	private void CheckStealthKill(CharacterXP xpStat, Events.Kill args)
	{
		if (args.SilentKill)
		{
			xpStat.StoreKill(true);
		}
	}
}
