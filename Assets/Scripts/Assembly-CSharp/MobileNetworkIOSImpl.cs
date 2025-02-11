using System.Collections.Generic;
using UnityEngine;

public class MobileNetworkIOSImpl : IMobileNetworkImpl
{
	private GameObject m_gameCenter;

	public void Init()
	{
		if (m_gameCenter == null)
		{
			m_gameCenter = new GameObject("GameCenterManager");
			m_gameCenter.AddComponent<GameCenterManager>();
			AddEventHandlers();
		}
		Debug.Log("MobileNetworkIOSImpl Initialized");
	}

	private void OnDisable()
	{
		if (m_gameCenter != null)
		{
			RemoveEventHandlers();
			Object.Destroy(m_gameCenter);
			m_gameCenter = null;
		}
	}

	public bool IsMobileNetworkAvailable()
	{
		return GameCenterBinding.isGameCenterAvailable();
	}

	public bool AreTurnBasedMatchesAvailable()
	{
		return false;
	}

	public void AuthenticateLocalPlayer()
	{
		if (!GameCenterBinding.isPlayerAuthenticated())
		{
			GameCenterBinding.authenticateLocalPlayer();
		}
	}

	public bool IsPlayerAuthenticated()
	{
		return GameCenterBinding.isPlayerAuthenticated();
	}

	public string PlayerAlias()
	{
		return GameCenterBinding.playerAlias();
	}

	public string PlayerIdentifier()
	{
		return GameCenterBinding.playerIdentifier();
	}

	public bool supportsAchievements()
	{
		return GameCenterBinding.isGameCenterAvailable();
	}

	public void showAchievementsBanner()
	{
		GameCenterBinding.showCompletionBannerForAchievements();
	}

	public void resetAchievements()
	{
		GameCenterBinding.resetAchievements();
	}

	public void reportAchievement(string achievementId, float percentComplete)
	{
		GameCenterBinding.reportAchievement(achievementId, percentComplete);
	}

	public void getAchievements()
	{
		GameCenterBinding.getAchievements();
	}

	public void showAchievements()
	{
		GameCenterBinding.showAchievements();
	}

	private void AddEventHandlers()
	{
		if (m_gameCenter != null)
		{
			GameCenterManager.loadPlayerDataFailed += OnGCLoadPlayerDataFailed;
			GameCenterManager.playerDataLoaded += OnGCPlayerDataLoaded;
			GameCenterManager.playerAuthenticated += OnGCPlayerAuthenticated;
			GameCenterManager.playerFailedToAuthenticate += OnGCPlayerFailedToAuthenticate;
			GameCenterManager.playerLoggedOut += OnGCPlayerLoggedOut;
			GameCenterManager.reportAchievementFailed += OnGCReportAchievementFailed;
			GameCenterManager.reportAchievementFinished += OnGCReportAchievementFinished;
			GameCenterManager.loadAchievementsFailed += OnGCLoadAchievementsFailed;
			GameCenterManager.achievementsLoaded += OnGCAchievementsLoaded;
			GameCenterManager.resetAchievementsFailed += OnGCResetAchievementsFailed;
			GameCenterManager.resetAchievementsFinished += OnGCResetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailed += OnGCRetrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoaded += OnGCAchievementMetadataLoaded;
		}
	}

	private void RemoveEventHandlers()
	{
		if (m_gameCenter != null)
		{
			GameCenterManager.loadPlayerDataFailed -= OnGCLoadPlayerDataFailed;
			GameCenterManager.playerDataLoaded -= OnGCPlayerDataLoaded;
			GameCenterManager.playerAuthenticated -= OnGCPlayerAuthenticated;
			GameCenterManager.playerFailedToAuthenticate -= OnGCPlayerFailedToAuthenticate;
			GameCenterManager.playerLoggedOut -= OnGCPlayerLoggedOut;
			GameCenterManager.reportAchievementFailed -= OnGCReportAchievementFailed;
			GameCenterManager.reportAchievementFinished -= OnGCReportAchievementFinished;
			GameCenterManager.loadAchievementsFailed -= OnGCLoadAchievementsFailed;
			GameCenterManager.achievementsLoaded -= OnGCAchievementsLoaded;
			GameCenterManager.resetAchievementsFailed -= OnGCResetAchievementsFailed;
			GameCenterManager.resetAchievementsFinished -= OnGCResetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailed -= OnGCRetrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoaded -= OnGCAchievementMetadataLoaded;
		}
	}

	private void OnGCPlayerAuthenticated()
	{
		MobileNetworkManager.Instance._OnPlayerAuthenticated();
		GameCenterBinding.retrieveFriends(false);
	}

	private void OnGCPlayerFailedToAuthenticate(string error)
	{
		MobileNetworkManager.Instance._OnPlayerNotAuthenticated(error);
	}

	private void OnGCPlayerLoggedOut()
	{
		Debug.Log("GameCenter playerLoggedOut");
		MobileNetworkManager.Instance._OnPlayerLoggedOut();
	}

	private void OnGCPlayerDataLoaded(List<GameCenterPlayer> players)
	{
		Debug.Log("GameCenter playerDataLoaded");
	}

	private void OnGCLoadPlayerDataFailed(string error)
	{
		Debug.Log("GameCenter loadPlayerDataFailed: " + error);
	}

	private void OnGCAchievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementMetadata)
	{
		Debug.Log("achievementMetadatLoaded");
		foreach (GameCenterAchievementMetadata achievementMetadatum in achievementMetadata)
		{
			Debug.Log(achievementMetadatum);
		}
	}

	private void OnGCRetrieveAchievementMetadataFailed(string error)
	{
		Debug.Log("retrieveAchievementMetadataFailed: " + error);
	}

	private void OnGCResetAchievementsFinished()
	{
		Debug.Log("resetAchievmenetsFinished");
	}

	private void OnGCResetAchievementsFailed(string error)
	{
		Debug.Log("resetAchievementsFailed: " + error);
	}

	private void OnGCAchievementsLoaded(List<GameCenterAchievement> achievements)
	{
		Debug.Log("achievementsLoaded");
		List<MobileNetworkAchievement> list = new List<MobileNetworkAchievement>();
		foreach (GameCenterAchievement achievement in achievements)
		{
			MobileNetworkAchievement item = default(MobileNetworkAchievement);
			item.AchievementID = achievement.identifier;
			item.IsComplete = achievement.completed;
			item.IsHidden = achievement.isHidden;
			item.PercentComplete = achievement.percentComplete;
			list.Add(item);
		}
		MobileNetworkManager.Instance._OnAchievementsLoaded(list);
	}

	private void OnGCLoadAchievementsFailed(string error)
	{
		Debug.Log("loadAchievementsFailed: " + error);
		MobileNetworkManager.Instance._OnAchievementLoadFailed(error);
	}

	private void OnGCReportAchievementFinished(string identifier)
	{
		Debug.Log("reportAchievementFinished: " + identifier);
	}

	private void OnGCReportAchievementFailed(string error)
	{
		Debug.Log("reportAchievementFailed: " + error);
	}
}
