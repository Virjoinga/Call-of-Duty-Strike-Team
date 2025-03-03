using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : SingletonMonoBehaviour, ISaveLoad
{
	public AchievementData AchievementsList;

	public PerkData PerksList;

	public AchievementSyncManager AchievementSyncManagerInstance;

	public LeaderboardManager LeaderboardManagerInstance;

	private WeaponStats m_WeaponStats;

	private CharacterStats m_CharacterStats;

	private CharacterXPStats m_CharacterXPStats;

	private MissionStats m_MissionStats;

	private SquadStats m_SquadStats;

	private PlayerStats m_PlayerStats;

	private PersonalBestStats m_PersonalBestStats;

	private AchievementManager m_AchievementManager;

	private MedalManager m_MedalManager;

	private PerksManager m_PerksManager;

	private List<StatsManagerUpdatable> m_UpdateList;

	private string SessionInProgessKey = "Stats.SessionInProgress";

	private int m_Index;

	private bool m_SessionInProgess;

	public static StatsManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<StatsManager>();
		}
	}

	protected override void Awake()
	{
		m_WeaponStats = new WeaponStats();
		m_CharacterStats = new CharacterStats();
		m_CharacterXPStats = new CharacterXPStats();
		m_MissionStats = new MissionStats();
		m_SquadStats = new SquadStats();
		m_PlayerStats = new PlayerStats();
		m_PersonalBestStats = new PersonalBestStats();
		m_AchievementManager = new AchievementManager();
		m_MedalManager = new MedalManager();
		m_PerksManager = new PerksManager();
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		base.Awake();
	}

	protected void Start()
	{
		m_UpdateList = new List<StatsManagerUpdatable>();
		m_Index = 0;
		m_SessionInProgess = false;
		AddUpdate(m_WeaponStats);
		AddUpdate(m_CharacterStats);
		AddUpdate(m_CharacterXPStats);
		AddUpdate(m_MissionStats);
		AddUpdate(m_SquadStats);
		AddUpdate(m_PlayerStats);
		AddUpdate(m_PersonalBestStats);
		AddUpdate(m_AchievementManager);
		AddUpdate(m_MedalManager);
		AddUpdate(m_PerksManager);
		PerksList.SortIntoTiers();
	}

	private void OnEnable()
	{
		ActivateWatcher.DataLoadedAfterLogin += HandleActivateWatcherDataLoadedAfterLogin;
	}

	private void OnDisable()
	{
		ActivateWatcher.DataLoadedAfterLogin -= HandleActivateWatcherDataLoadedAfterLogin;
	}

	private void AddUpdate(StatsManagerUpdatable statsToUpdate)
	{
		m_UpdateList.Add(statsToUpdate);
		statsToUpdate.Index = m_Index++;
		statsToUpdate.Init();
	}

	private void HandleActivateWatcherDataLoadedAfterLogin(object sender, EventArgs e)
	{
	}

	private void Update()
	{
		if (m_SessionInProgess)
		{
			m_Index = 0;
			for (int i = 0; i < m_UpdateList.Count; i++)
			{
				m_UpdateList[i].Update();
				m_Index++;
			}
		}
	}

	public static int ConvertSoldierIdToIndex(string id)
	{
		return id[7] - 49;
	}

	public static string ConvertSoldierIndexToId(int index)
	{
		return "Player_" + (index + 1);
	}

	public static string MissionStatId(MissionListings.eMissionID missionId, int section)
	{
		return missionId.ToString() + "." + section;
	}

	public static string MissionSwrveId(MissionListings.eMissionID missionId, int section)
	{
		return missionId.ToString() + "_" + section;
	}

	public int GetAchievementXPOnCompletion(string id)
	{
		return AchievementsList.GetAchievement(id).XpOnCompletion;
	}

	public int GetAchievementSteps(string id)
	{
		return AchievementsList.GetAchievement(id).nSteps;
	}

	public void BeginSession()
	{
		ClearCurrentMission();
		m_SessionInProgess = true;
	}

	public void EndSession()
	{
		m_SessionInProgess = false;
		SessionEnd();
		PostSessionEnd();
	}

	private void SessionEnd()
	{
		m_Index = 0;
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.SessionEnd();
			m_Index++;
		}
	}

	private void PostSessionEnd()
	{
		m_Index = 0;
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.PostSessionEnd();
			m_Index++;
		}
	}

	private void ClearCurrentMission()
	{
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.ClearCurrentMission();
		}
	}

	public void Reset()
	{
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.Reset();
		}
	}

	public void Load()
	{
		m_SessionInProgess = SecureStorage.Instance.GetBool(SessionInProgessKey);
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.Load();
		}
		SyncAchievements();
	}

	public void SyncAchievements()
	{
		AchievementSyncManagerInstance.DoSync();
	}

	public void SyncLeaderboards()
	{
		LeaderboardManagerInstance.DoSync();
	}

	public void Save()
	{
		SecureStorage.Instance.SetBool(SessionInProgessKey, m_SessionInProgess);
		foreach (StatsManagerUpdatable update in m_UpdateList)
		{
			update.Save();
		}
		SyncAchievements();
		SyncLeaderboards();
	}

	private void HandleDataLoadedAfterLogin(object sender, EventArgs e)
	{
		SyncLeaderboards();
	}

	public MissionStats MissionStats()
	{
		return m_MissionStats;
	}

	public PlayerStats PlayerStats()
	{
		return m_PlayerStats;
	}

	public PersonalBestStats PersonalBestStats()
	{
		return m_PersonalBestStats;
	}

	public CharacterStats CharacterStats()
	{
		return m_CharacterStats;
	}

	public WeaponStats WeaponStats()
	{
		return m_WeaponStats;
	}

	public CharacterXPStats CharacterXPStats()
	{
		return m_CharacterXPStats;
	}

	public AchievementManager AchievementManager()
	{
		return m_AchievementManager;
	}

	public SquadStats SquadStats()
	{
		return m_SquadStats;
	}

	public MedalManager MedalManager()
	{
		return m_MedalManager;
	}

	public PerksManager PerksManager()
	{
		return m_PerksManager;
	}

	public PerksManager PerksManagerNoAssert()
	{
		return m_PerksManager;
	}
}
