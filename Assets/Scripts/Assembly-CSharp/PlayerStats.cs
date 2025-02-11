public class PlayerStats : Stats<PlayerStat>
{
	private MissionListings.eMissionID m_MissionId;

	private int m_MissionSection;

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnEndMission += MissionEnded;
		EventHub.Instance.OnMedalEarned += MedalEarned;
		EventHub.Instance.OnAchievementCompleted += AchievementCompleted;
		EventHub.Instance.OnHardCurrencyChanged += HardCurrencyChanged;
		EventHub.Instance.GMGScoreAdded += ScoreAdded;
	}

	public PlayerStat GetCurrentMissionStat()
	{
		return GetCurrentMissionStat("Global");
	}

	public PlayerStat GetGameTotalStat()
	{
		return GetGameTotalStat("Global");
	}

	public override void Create()
	{
		CreateStat("Global");
	}

	private void MissionEnded(object sender, Events.EndMission args)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat();
		if (args.Success)
		{
			AddXPForMissionComplete(currentMissionStat, "Mission Passed " + args.Mode, XPManager.Instance.m_XPMissionsAndObjectives.MissionPassedXP(args.Mode));
		}
		CharacterXP currentMissionCombinedStat = StatsManager.Instance.CharacterXPStats().GetCurrentMissionCombinedStat();
		AddXP(currentMissionStat, currentMissionCombinedStat.XPFromKills, "Kills");
		AddXP(currentMissionStat, currentMissionCombinedStat.XPFromBonuses, "Bonuses");
		m_MissionId = args.MissionId;
		m_MissionSection = args.Section;
	}

	public override void SessionEnd()
	{
	}

	public override void PostSessionEnd()
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat();
		if (ActStructure.Instance.MissionUsesXPAsScore(m_MissionId, m_MissionSection))
		{
			if (ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran)
			{
				currentMissionStat.ScoreAsVeteran = currentMissionStat.XP;
			}
			else
			{
				currentMissionStat.Score = currentMissionStat.XP;
			}
		}
		currentMissionStat.XPBeforeMultipler = currentMissionStat.XP;
		currentMissionStat.XP = XPManager.Instance.AdjustXPByMultiplier(currentMissionStat.XP);
		MissionStat currentMissionStat2 = StatsManager.Instance.MissionStats().GetCurrentMissionStat(m_MissionId, m_MissionSection);
		currentMissionStat2.ScoreAwarded = currentMissionStat.Score;
		int xP = GetGameTotalStat().XP;
		UpdateGameTotals();
		int xP2 = GetGameTotalStat().XP;
		StatsManager.Instance.PerksManagerNoAssert().CheckPerksUnlockedByXP(xP, xP2);
	}

	private void AddXPForMissionComplete(PlayerStat stat, string reason, int xpToAdd)
	{
		stat.XPFromMissionComplete += xpToAdd;
		AddXP(stat, xpToAdd, reason);
	}

	private void AddXP(PlayerStat stat, int xpToAdd, string reason)
	{
		stat.XP += xpToAdd;
	}

	private void AchievementCompleted(object sender, Events.AchievementCompleted args)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat("Global");
		int achievementXPOnCompletion = StatsManager.Instance.GetAchievementXPOnCompletion(args.Identifier);
		if (achievementXPOnCompletion > 0)
		{
			AddXP(currentMissionStat, achievementXPOnCompletion, "Achievement " + args.Identifier);
			TBFAssert.DoAssert(ActStructure.Instance.MissionInProgress);
		}
	}

	private void IntelCollected(object sender, Events.IntelCollected args)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat("Global");
		int xpToAdd = XPManager.Instance.m_XPMissionsAndObjectives.IntelXP();
		AddXP(currentMissionStat, xpToAdd, "Intel");
	}

	private void MedalEarned(object sender, Events.MedalEarned args)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat("Global");
		int xpToAdd = XPManager.Instance.m_XPMissionsAndObjectives.EarnedMedalXP(args.Mode);
		AddXP(currentMissionStat, xpToAdd, "Medal " + args.MedalId);
	}

	public void AddWaveXP(int amount, int index)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat("Global");
		AddXP(currentMissionStat, amount, "Wave " + index);
	}

	public void AddDebugXP(int amount, string challengeName)
	{
		PlayerStat gameTotalStat = GetGameTotalStat("Global");
		AddXP(gameTotalStat, amount, "Challenge " + challengeName);
	}

	public void AddRewardXP(int amount)
	{
		PlayerStat gameTotalStat = GetGameTotalStat("Global");
		AddXP(gameTotalStat, amount, "Reward");
	}

	private void HardCurrencyChanged(object sender, Events.HardCurrencyChanged args)
	{
		PlayerStat playerStat = null;
		playerStat = ((!ActStructure.Instance.MissionInProgress) ? GetGameTotalStat("Global") : GetCurrentMissionStat("Global"));
		if (args.Amount > 0)
		{
			if (!args.Freebie)
			{
				if (args.FromPurchase)
				{
					playerStat.HardCurrencyPurchased += args.Amount;
				}
				else
				{
					playerStat.HardCurrencyEarned += args.Amount;
				}
			}
		}
		else
		{
			playerStat.HardCurrencySpent -= args.Amount;
		}
	}

	public int PlayerPurchasedSoldierXP(int soldierIndex)
	{
		ActStructure instance = ActStructure.Instance;
		StatsManager instance2 = StatsManager.Instance;
		int num = instance2.SquadStats().PlayerPurchasedSoldierXP(soldierIndex);
		PlayerStat gameTotalStat = GetGameTotalStat("Global");
		AddXP(gameTotalStat, num, "buyback");
		PlayerStat currentMissionStat = GetCurrentMissionStat();
		if (instance.CurrentMissionMode == DifficultyMode.Veteran)
		{
			currentMissionStat.ScoreAsVeteran += num;
		}
		else
		{
			currentMissionStat.Score += num;
		}
		instance2.MissionStats().GetCurrentMissionStat(instance.LastMissionID, instance.CurrentSection).ScoreAwarded += num;
		instance2.MissionStats().GetGameTotalStat(instance.LastMissionID.ToString()).ScoreAwarded += num;
		return num;
	}

	private void ScoreAdded(object sender, Events.GMGScoreAdded args)
	{
		PlayerStat currentMissionStat = GetCurrentMissionStat("Global");
		currentMissionStat.Score += (int)(GlobalUnrestController.Instance.CurrentScoreMultiplier * (float)args.Score);
	}
}
