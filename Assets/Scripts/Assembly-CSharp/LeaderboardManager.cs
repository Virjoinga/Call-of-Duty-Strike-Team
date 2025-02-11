using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
	private EventLog m_LeaderboardLog = new EventLog();

	public static int GlobalXPLeaderboardID = 21;

	public static int GMGLeaderboardStartID = 60;

	private List<LeaderboardResult> mCurrentLeaderboardResults;

	private int mCurrentLeaderboardResultsId = -1;

	public List<LeaderboardResult> CurrentLeaderboardResults
	{
		get
		{
			return mCurrentLeaderboardResults;
		}
	}

	public int CurrentLeaderboardResultsId
	{
		get
		{
			return mCurrentLeaderboardResultsId;
		}
	}

	public EventLog Log()
	{
		return m_LeaderboardLog;
	}

	private void OnEnable()
	{
		if (EventHub.Instance != null)
		{
			EventHub.Instance.OnStartMission += OnMissionStarted;
			EventHub.Instance.OnEndMission += OnMissionEnded;
		}
	}

	private void OnDisable()
	{
		if (EventHub.Instance != null)
		{
			EventHub.Instance.OnStartMission -= OnMissionStarted;
			EventHub.Instance.OnEndMission -= OnMissionEnded;
		}
	}

	private void OnMissionStarted(object sender, Events.StartMission args)
	{
		UpdateFriendCache(args.MissionId, args.Section);
	}

	private void OnMissionEnded(object sender, Events.EndMission args)
	{
		ActStructure instance = ActStructure.Instance;
		if (instance != null && instance.MissionIsSpecOps(args.MissionId, args.Section))
		{
			UpdateFriendCache(args.MissionId, args.Section);
		}
	}

	private void UpdateFriendCache(MissionListings.eMissionID missionId, int section)
	{
		mCurrentLeaderboardResultsId = -1;
		int num = MissionListings.Instance.LeaderboardIDForMissionSection(missionId, section);
		if (num >= 0)
		{
			BedrockWorker.Instance.GetLeaderboardValuesByPivot((uint)num, true, GotLeaderboardResults);
		}
	}

	public void GotLeaderboardResults(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		mCurrentLeaderboardResults = leaderBoard;
		mCurrentLeaderboardResultsId = (int)leaderboardIndex;
	}

	public LeaderboardResult GetNearestToPlayer()
	{
		if (mCurrentLeaderboardResultsId != -1 && mCurrentLeaderboardResults != null)
		{
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < mCurrentLeaderboardResults.Count; i++)
			{
				if (mCurrentLeaderboardResults[i].IsPlayer)
				{
					num = i - 1;
					num2 = i + 1;
				}
			}
			if (num >= 0 && num < mCurrentLeaderboardResults.Count)
			{
				return mCurrentLeaderboardResults[num];
			}
			if (num2 >= 0 && num2 < mCurrentLeaderboardResults.Count)
			{
				return mCurrentLeaderboardResults[num2];
			}
		}
		return null;
	}

	public void DoSync()
	{
		Debug.Log("Syncing leaderboards...");
		if (!(BedrockWorker.Instance != null))
		{
			return;
		}
		int num = StatsHelper.PlayerXP();
		int level = -1;
		int prestigeLevel = -1;
		int xpToNextLevel = 0;
		float percent = 0f;
		XPManager.Instance.ConvertXPToLevel(num, out level, out prestigeLevel, out xpToNextLevel, out percent);
		int num2 = XPManager.Instance.ConvertXPLevelToAbsolute(level, prestigeLevel);
		List<LeaderboardEntry> list = new List<LeaderboardEntry>();
		for (uint num3 = 0u; num3 < MissionListings.Instance.Missions.Length; num3++)
		{
			MissionData missionData = MissionListings.Instance.Missions[num3];
			for (int i = 0; i < missionData.Sections.Count; i++)
			{
				if (missionData.Sections[i].LeaderboardID > 0)
				{
					bool isVeteran = false;
					int wave = StatsHelper.HighestSpecOpsWaveCompletedGameTotal(missionData.MissionId, i);
					int scoreForMission = GetScoreForMission(missionData.MissionId, i, out isVeteran);
					if (scoreForMission > 0)
					{
						list.Add(new LeaderboardEntry((uint)missionData.Sections[i].LeaderboardID, scoreForMission, isVeteran, wave));
					}
				}
			}
		}
		list.Add(new LeaderboardEntry((uint)GlobalXPLeaderboardID, num, false, 0));
		uint leaderboardNum = (uint)(GMGLeaderboardStartID + GlobalUnrestController.Instance.CurrentLeague);
		list.Add(new LeaderboardEntry(leaderboardNum, GlobalUnrestController.Instance.CurrentScore, false, 0));
		Debug.Log("Uploading leaderboards...");
		bool eliteAccountLinked = SecureStorage.Instance.EliteAccountLinked;
		BedrockWorker.Instance.WriteMissionLeaderboardValues(list, eliteAccountLinked, num2 + 1, num);
	}

	private int GetScoreForMission(MissionListings.eMissionID missionId, int s, out bool isVeteran)
	{
		int num = StatsHelper.HighestScore(missionId, s);
		int num2 = StatsHelper.HighestScoreAsVeteran(missionId, s);
		isVeteran = false;
		int result = num;
		if (num2 >= num)
		{
			result = num2;
			isVeteran = true;
		}
		return result;
	}

	public int FindScoreForLeaderboard(int leaderboardID, out bool veteran)
	{
		veteran = false;
		if (leaderboardID == GlobalXPLeaderboardID)
		{
			return StatsHelper.PlayerXP();
		}
		for (uint num = 0u; num < MissionListings.Instance.Missions.Length; num++)
		{
			MissionData missionData = MissionListings.Instance.Missions[num];
			for (int i = 0; i < missionData.Sections.Count; i++)
			{
				if (missionData.Sections[i].LeaderboardID == leaderboardID)
				{
					return GetScoreForMission(missionData.MissionId, i, out veteran);
				}
			}
		}
		return -1;
	}
}
