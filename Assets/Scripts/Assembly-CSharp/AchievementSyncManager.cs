using System.Collections.Generic;
using UnityEngine;

public class AchievementSyncManager : MonoBehaviour
{
	private enum AchievementState
	{
		Idle = 0,
		Sync = 1
	}

	private AchievementState m_CurrentState;

	private void Start()
	{
		MobileNetworkManager.achievementsLoaded += HandleAchievementsLoaded;
		MobileNetworkManager.loadAchievementsFailed += HandleAchievementsFailed;
	}

	public bool Busy()
	{
		return m_CurrentState != AchievementState.Idle;
	}

	public void DoSync()
	{
		Debug.Log("Synching achievements...");
		if (!Busy())
		{
			Debug.Log("Mobile Network Manager getAchievements");
			m_CurrentState = AchievementState.Sync;
			MobileNetworkManager.Instance.getAchievements();
			Debug.Log("Mobile Network Manager getAchievements... complete");
		}
		Debug.Log("Called getAchievements complete");
	}

	private void HandleAchievementsLoaded(List<MobileNetworkAchievement> achievements)
	{
		Debug.Log("HandleAchievementsLoaded");
		if (m_CurrentState == AchievementState.Sync)
		{
			Achievement[] achievements2 = StatsManager.Instance.AchievementsList.Achievements;
			foreach (Achievement achievement in achievements2)
			{
				MobileNetworkAchievement? mobileNetworkAchievement = null;
				foreach (MobileNetworkAchievement achievement2 in achievements)
				{
					if (achievement2.AchievementID == achievement.Identifier)
					{
						mobileNetworkAchievement = achievement2;
						break;
					}
				}
				AchievementStat gameTotalStat = StatsManager.Instance.AchievementManager().GetGameTotalStat(achievement.Identifier);
				float num = gameTotalStat.PercentCompleteFloat();
				if (!mobileNetworkAchievement.HasValue || num > mobileNetworkAchievement.Value.PercentComplete)
				{
					MobileNetworkManager.Instance.reportAchievement(achievement.Identifier, num);
				}
			}
		}
		m_CurrentState = AchievementState.Idle;
		Debug.Log("HandleAchievementsLoaded, state set to Idle");
	}

	private void HandleAchievementsFailed(string error)
	{
		m_CurrentState = AchievementState.Idle;
	}
}
