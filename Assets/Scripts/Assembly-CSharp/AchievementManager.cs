public class AchievementManager : Stats<AchievementStat>
{
	private EventLog m_AchievementLog = new EventLog();

	private MissionListings.eMissionID m_CompletedMissionId;

	public AchievementMTXTracking MTXTracking = new AchievementMTXTracking();

	public EventLog Log()
	{
		return m_AchievementLog;
	}

	public override void ClearCurrentMission()
	{
		base.ClearCurrentMission();
	}

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnKill += Kill;
		EventHub.Instance.OnEndMission += EndMission;
		EventHub.Instance.OnHardCurrencyChanged += UpdateHoarder;
		EventHub.Instance.OnPurchaseArmour += UpdateTurtle;
		EventHub.Instance.OnPerkUnlocked += UpdateArmoury;
		EventHub.Instance.OnPurchaseEquipment += UpdateQuartermaster;
		EventHub.Instance.OnPerkUnlocked += UpdatePerky;
		EventHub.Instance.OnPurchaseEquipment += UpdateNeedALittleHelpHere;
		EventHub.Instance.OnShare += UpdateSociable;
		EventHub.Instance.OnSpecOpsWaveComplete += UpdateTourOfDuty;
		EventHub.Instance.OnSpecOpsWaveComplete += UpdateInternationalDeployment;
		Challenge.MedalEarned += UpdateEarnYourStripes;
		Challenge.MedalEarned += UpdateActiveService;
	}

	private void EndMission(object sender, Events.EndMission args)
	{
		if (args.Success && !ActStructure.Instance.MissionIsSpecOps(args.MissionId, args.Section))
		{
			UpdateGettingItDone();
			if (args.Mode == DifficultyMode.Veteran)
			{
				UpdateGettingItDoneVeteran();
			}
		}
	}

	private void Kill(object sender, Events.Kill args)
	{
		if (args.Attacker != null && args.Attacker.PlayerControlled && !args.Victim.PlayerControlled)
		{
			if (args.HeadShot)
			{
				UpdateMarksman();
			}
			if (args.GrenadeKill && FactionHelper.AreEnemies(args.Attacker.Faction, args.Victim.Faction))
			{
				UpdateFragtastic();
			}
			if (args.Attacker.WeaponClass == WeaponDescriptor.WeaponClass.SniperRifle && args.OneShotKill)
			{
				UpdateGhillie();
			}
			if (args.Attacker.HealthLow)
			{
				UpdateTheBrink();
			}
			if (GameController.Instance.IsPlayerBreaching)
			{
				UpdateBreach();
			}
			if (args.Victim.CharacterType == CharacterType.SentryGun)
			{
				UpdateNoHands();
			}
			if (args.Attacker.WeaponSilenced && !KillTypeHelper.IsAStealthKill(args))
			{
				UpdateSilencer();
			}
			UpdateWetWork();
		}
		if (KillTypeHelper.IsAStealthKill(args) && !args.Victim.PlayerControlled)
		{
			UpdateBackStabber();
		}
		if ((KillTypeHelper.IsAStealthKill(args) || KillTypeHelper.IsAKnifeKill(args)) && !args.Victim.PlayerControlled)
		{
			UpdateKnifeVeteran();
		}
	}

	public override void Create()
	{
		Achievement[] achievements = StatsManager.Instance.AchievementsList.Achievements;
		foreach (Achievement achievement in achievements)
		{
			CreateStat(achievement.Identifier);
		}
	}

	public override void Reset()
	{
		base.Reset();
		MTXTracking.Reset();
	}

	public override void Save()
	{
		base.Save();
		MTXTracking.Save();
	}

	public override void Load()
	{
		base.Load();
		MTXTracking.Load();
	}

	public override void SessionEnd()
	{
		UpdateFearless();
		if (StatsHelper.CurrentMissionSuccessful() && StatsManager.Instance.SquadStats().GetCurrentMission().NumTimesKilled == 0 && StatsManager.Instance.SquadStats().GetCurrentMission().NumTimesHealed == 0)
		{
			UpdateFlawless();
		}
		base.SessionEnd();
	}

	private void UpdateAchievement(string id, int amountToAdd)
	{
		UpdateAchievement(id, amountToAdd, false);
	}

	private void UpdateAchievement(string id, int amountToAdd, bool instant)
	{
	}

	private void UpdateWetWork()
	{
		for (int i = 0; i < 8; i++)
		{
			UpdateAchievement("wetwork" + (i + 1), 1);
		}
	}

	private void UpdateMarksman()
	{
		for (int i = 0; i < 7; i++)
		{
			UpdateAchievement("marksman" + (i + 1), 1);
		}
	}

	private void UpdateFragtastic()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateAchievement("fragtastic" + (i + 1), 1);
		}
	}

	private void UpdateGhillie()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateAchievement("ghillie" + (i + 1), 1);
		}
	}

	private void UpdateFlawless()
	{
		UpdateAchievement("flawless", 1);
	}

	private void UpdateFearless()
	{
		if (StatsManager.Instance.SquadStats().GetCurrentMission().NumTimesKilled == 0 && StatsManager.Instance.SquadStats().GetCurrentMission().NumKills >= 10)
		{
			UpdateAchievement("fearless", 1);
		}
	}

	private void UpdateBackStabber()
	{
		UpdateAchievement("backstabber", 1);
	}

	private void UpdateTheBrink()
	{
		UpdateAchievement("thebrink", 1);
	}

	private void UpdateBreach()
	{
		UpdateAchievement("breach1", 1);
		UpdateAchievement("breach2", 1);
		UpdateAchievement("breach3", 1);
	}

	private void UpdateNoHands()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateAchievement("looknohands" + (i + 1), 1);
		}
	}

	private void UpdateInternationalDeployment(object sender, Events.SpecOpsWaveComplete args)
	{
		if (args.WaveNum == 10)
		{
			for (int i = 0; i < 5; i++)
			{
				UpdateAchievement("internationaldeployment" + (i + 1), 1);
			}
		}
	}

	private void UpdateKnifeVeteran()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateAchievement("knifeveteran" + (i + 1), 1);
		}
	}

	private void UpdateSilencer()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateAchievement("silencer" + (i + 1), 1);
		}
	}

	private void UpdateActiveService(object sender, ValueEventArgs<ChallengeMedalType> e)
	{
		for (int num = MTXTracking.m_TimeOfChallengeMedalEarned.Length - 1; num > 0; num--)
		{
			MTXTracking.m_TimeOfChallengeMedalEarned[num] = MTXTracking.m_TimeOfChallengeMedalEarned[num - 1];
		}
		uint synchronizedTimeOrBestGuess = SynchronizedClock.Instance.SynchronizedTimeOrBestGuess;
		MTXTracking.m_TimeOfChallengeMedalEarned[0] = (int)synchronizedTimeOrBestGuess;
		uint num2 = 604800u;
		if (TimeUtils.GetSecondsSince((uint)MTXTracking.m_TimeOfChallengeMedalEarned[1], synchronizedTimeOrBestGuess) < num2)
		{
			UpdateAchievement("activeservice1", 1);
		}
		if (TimeUtils.GetSecondsSince((uint)MTXTracking.m_TimeOfChallengeMedalEarned[3], synchronizedTimeOrBestGuess) < num2)
		{
			UpdateAchievement("activeservice2", 1);
		}
		if (TimeUtils.GetSecondsSince((uint)MTXTracking.m_TimeOfChallengeMedalEarned[5], synchronizedTimeOrBestGuess) < num2)
		{
			UpdateAchievement("activeservice3", 1);
		}
	}

	private void UpdateEarnYourStripes(object sender, ValueEventArgs<ChallengeMedalType> e)
	{
		for (int i = 0; i < 6; i++)
		{
			UpdateAchievement("earnyourstripes" + (i + 1), 1);
		}
	}

	private void UpdateGettingItDone()
	{
		UpdateAchievement("gettingitdone1", 1);
		UpdateAchievement("gettingitdone2", 1);
	}

	private void UpdateGettingItDoneVeteran()
	{
		UpdateAchievement("gettingitdoneveteran1", 1);
		UpdateAchievement("gettingitdoneveteran2", 1);
	}

	private void UpdateHoarder(object sender, Events.HardCurrencyChanged args)
	{
		if (args.Amount > 0)
		{
			UpdateAchievement("hoarder1", args.Amount);
			UpdateAchievement("hoarder2", args.Amount);
		}
	}

	private void UpdateNeedALittleHelpHere(object sender, Events.PurchaseEquipment args)
	{
		if (!LoadoutMenuNavigator.AutoAddingEquipment && !LoadoutMenuNavigator.LoadOutActive)
		{
			UpdateAchievement("needalittlehelphere1", 1);
			UpdateAchievement("needalittlehelphere2", 1);
			UpdateAchievement("needalittlehelphere3", 1);
		}
	}

	private void UpdateSociable(object sender, Events.Share args)
	{
		UpdateAchievement("sociable1", 1, true);
		UpdateAchievement("sociable2", 1, true);
	}

	public void UpdateYoureOnTheTeam()
	{
		UpdateAchievement("youreontheteam", 1);
	}

	private void UpdateTourOfDuty(object sender, Events.SpecOpsWaveComplete args)
	{
		int specOpsMissionSection = MissionListings.Instance.GetSpecOpsMissionSection(MissionListings.eMissionID.MI_MISSION_AFGHANISTAN_GMG);
		int specOpsMissionSection2 = MissionListings.Instance.GetSpecOpsMissionSection(MissionListings.eMissionID.MI_MISSION_ARCTIC_GMG);
		int specOpsMissionSection3 = MissionListings.Instance.GetSpecOpsMissionSection(MissionListings.eMissionID.MI_MISSION_KOWLOON_GMG);
		MissionStat gameTotalStat = StatsManager.Instance.MissionStats().GetGameTotalStat(MissionListings.eMissionID.MI_MISSION_AFGHANISTAN_GMG, specOpsMissionSection);
		MissionStat gameTotalStat2 = StatsManager.Instance.MissionStats().GetGameTotalStat(MissionListings.eMissionID.MI_MISSION_ARCTIC_GMG, specOpsMissionSection2);
		MissionStat gameTotalStat3 = StatsManager.Instance.MissionStats().GetGameTotalStat(MissionListings.eMissionID.MI_MISSION_KOWLOON_GMG, specOpsMissionSection3);
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		if ((gameTotalStat.SpecOpsWavesCompleted > 0 || currentMissionID == MissionListings.eMissionID.MI_MISSION_AFGHANISTAN_GMG) && (gameTotalStat2.SpecOpsWavesCompleted > 0 || currentMissionID == MissionListings.eMissionID.MI_MISSION_ARCTIC_GMG) && (gameTotalStat3.SpecOpsWavesCompleted > 0 || currentMissionID == MissionListings.eMissionID.MI_MISSION_KOWLOON_GMG))
		{
			UpdateAchievement("tourofduty", 1);
		}
	}

	private void UpdateArmoury(object sender, Events.PerkUnlocked args)
	{
		if (args.UnlockedPerkType == PerkType.MachineGunner || args.UnlockedPerkType == PerkType.Rifleman || args.UnlockedPerkType == PerkType.Shotgunner || args.UnlockedPerkType == PerkType.SMGOperative || args.UnlockedPerkType == PerkType.Sniper)
		{
			UpdateAchievement("armory1", 1);
			UpdateAchievement("armory2", 1);
		}
	}

	private void UpdateTurtle(object sender, Events.PurchaseArmour args)
	{
		UpdateAchievement("turtle1", 1);
		UpdateAchievement("turtle2", 1);
	}

	private void UpdateQuartermaster(object sender, Events.PurchaseEquipment args)
	{
		if (!LoadoutMenuNavigator.AutoAddingEquipment)
		{
			UpdateAchievement("quartermaster1", 1);
			UpdateAchievement("quartermaster2", 1);
			UpdateAchievement("quartermaster3", 1);
		}
	}

	private void UpdatePerky(object sender, Events.PerkUnlocked args)
	{
		if (!LoadoutMenuNavigator.AutoAddingEquipment)
		{
			UpdateAchievement("perky1", 1);
			Perk perk = StatsManager.Instance.PerksList.Perks[(int)args.UnlockedPerkType];
			MTXTracking.m_PerkBoughtInTier[perk.Tier - 1] = true;
			if (MTXTracking.m_PerkBoughtInTier[0] && MTXTracking.m_PerkBoughtInTier[1] && MTXTracking.m_PerkBoughtInTier[2] && MTXTracking.m_PerkBoughtInTier[3])
			{
				UpdateAchievement("perky2", 1);
			}
		}
	}
}
