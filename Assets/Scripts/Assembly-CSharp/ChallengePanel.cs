using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengePanel : MonoBehaviour
{
	private enum SubPanels
	{
		Login = 0,
		LoginInGame = 1,
		CheckConnection = 2,
		LoggingIn = 3,
		Normal = 4
	}

	private const float UpdatePeriod = 1f;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengePanel), LogLevel.Debug);

	private readonly Queue<ChallengeListItem> _listItemCache = new Queue<ChallengeListItem>();

	public ChallengeListItem challengeListItemPrefab;

	public GameObject challengePanelLoginInGamePrefab;

	public GameObject challengePanelLoggingInPrefab;

	public GameObject challngePanelOfflineInGamePrefab;

	private Dictionary<uint, ChallengeListItem> _challengeListItems = new Dictionary<uint, ChallengeListItem>();

	private GameObject _challengePanelLoginInGameInstance;

	private GameObject _challengePanelLoggingInInstance;

	private GameObject _challengePanelOfflineInstance;

	private bool _showingChallengesPanel;

	private uint? _lastGameplayStartTime;

	private bool m_ActiveLastFrame;

	private float mLogInStartTime;

	public ChallengeManager m_ChallengeManager;

	private SubPanels mCurrentPanel = SubPanels.Normal;

	private ChallengesOverviewController mChallengesOverviewController;

	private void Awake()
	{
		mChallengesOverviewController = base.transform.parent.GetComponentInChildren<ChallengesOverviewController>();
	}

	private void Start()
	{
		if (base.transform.parent != null && mChallengesOverviewController != null)
		{
			mChallengesOverviewController.SetChallengesEnabled();
		}
		m_ChallengeManager = ChallengeManager.Instance;
		PrimeListItemCache(9);
		if (ChallengeManager.ConnectionStatus.IsOnline())
		{
			RefreshPanelStatus();
		}
	}

	private ChallengeListItem GetOrCreateUnusedItemFromListItemCache()
	{
		if (_listItemCache.Count > 0)
		{
			return _listItemCache.Dequeue();
		}
		_log.LogWarning("There is no item in the cache, generating new item. Performance may suffer a bit, did you prime the list with enough items?");
		return (ChallengeListItem)UnityEngine.Object.Instantiate(challengeListItemPrefab);
	}

	private void Recycle(ChallengeListItem listItem)
	{
		listItem.StopAllCoroutines();
		listItem.gameObject.SetActive(false);
		_listItemCache.Enqueue(listItem);
	}

	private void PrimeListItemCache(int numberOfItems)
	{
		ChallengeListItem[] array = new ChallengeListItem[numberOfItems];
		for (int i = 0; i < numberOfItems; i++)
		{
			array[i] = GetOrCreateUnusedItemFromListItemCache();
		}
		ChallengeListItem[] array2 = array;
		foreach (ChallengeListItem listItem in array2)
		{
			Recycle(listItem);
		}
	}

	private void OnEnable()
	{
		ChallengeData.StatusChanged += HandleChallengeDataStatusChanged;
		ChallengeManager.ChallengesInvalidated += HandleChallengeManagerChallengesInvalidated;
		ChallengeManager.ChallengesRevalidated += HandleChallengeManagerChallengesRevalidated;
		ActivateWatcher.ActivateUILaunched += OnActivateUILaunched;
		SynchronizedClock.ClockSynchronized += HandleSynchronizedClockClockSynchronized;
		Bedrock.UserConnectionStatusChanged += HandleBedrockUserConnectionStatusChanged;
		RefreshPanelStatus();
	}

	private void HandleBedrockUserConnectionStatusChanged(object sender, EventArgs e)
	{
		RefreshPanelStatus();
	}

	private void OnDisable()
	{
		ChallengeData.StatusChanged -= HandleChallengeDataStatusChanged;
		ChallengeManager.ChallengesInvalidated -= HandleChallengeManagerChallengesInvalidated;
		ChallengeManager.ChallengesRevalidated -= HandleChallengeManagerChallengesRevalidated;
		ActivateWatcher.ConnectionStatusChange -= HandleActivateWatcherConnectionStatusChange;
		ActivateWatcher.ActivateUILaunched -= OnActivateUILaunched;
		SynchronizedClock.ClockSynchronized -= HandleSynchronizedClockClockSynchronized;
		Bedrock.UserConnectionStatusChanged -= HandleBedrockUserConnectionStatusChanged;
	}

	private void OnActivateUILaunched(object sender, EventArgs args)
	{
		if (FrontEndController.Instance.ActiveScreen != 0)
		{
			FrontEndController.Instance.ReturnToGlobe();
		}
	}

	private void HandleUserLogOnFail(object sender, EventArgs args)
	{
		RefreshPanelStatus();
	}

	private void HandleActivateWatcherConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
		RefreshPanelStatus();
	}

	private void HandleChallengeManagerChallengesInvalidated(object sender, EventArgs e)
	{
		RefreshPanelStatus();
	}

	private void HandleChallengeManagerChallengesRevalidated(object sender, EventArgs e)
	{
		RefreshPanelStatus();
	}

	private void HandleGameManagerLoaded(object sender, EventArgs e)
	{
		_lastGameplayStartTime = SynchronizedClock.Instance.SynchronizedTime;
	}

	private void HandleSynchronizedClockClockSynchronized(object sender, EventArgs e)
	{
		RefreshPanelStatus();
	}

	private void HandleChallengeDataStatusChanged(object sender, ChallengeEventArgs e)
	{
		ChallengeData challengeData = (ChallengeData)sender;
		ChallengeListItem challengeListItem = GetChallengeListItem(challengeData);
		if (challengeData.Status == ChallengeStatus.Invalid)
		{
			if (challengeListItem != null)
			{
				RemoveListItem(challengeListItem);
			}
		}
		else if (challengeData.Status == ChallengeStatus.Open && challengeListItem == null && mChallengesOverviewController.IsActive)
		{
			uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
			if (synchronizedTime.HasValue)
			{
				TryBuildAndAddListItem(challengeData, synchronizedTime.Value);
			}
		}
	}

	private void RemoveListItem(ChallengeListItem challengeListItem)
	{
		_challengeListItems.Remove(challengeListItem.OverviewItem.ChallengeData.Id);
		if (mChallengesOverviewController != null)
		{
			mChallengesOverviewController.RemoveItem(challengeListItem);
		}
		Recycle(challengeListItem);
	}

	private ChallengeListItem GetChallengeListItem(ChallengeData data)
	{
		ChallengeListItem value;
		if (_challengeListItems.TryGetValue(data.Id, out value))
		{
			return value;
		}
		return null;
	}

	public void PanelSelected()
	{
		RefreshPanelStatus();
	}

	private void Update()
	{
		if (!m_ActiveLastFrame && mChallengesOverviewController.IsActive)
		{
			RefreshPanelStatus();
		}
		m_ActiveLastFrame = mChallengesOverviewController.IsActive;
		if (mCurrentPanel == SubPanels.LoggingIn)
		{
			mChallengesOverviewController.SetStatus(DynamicLoginText(), false);
			if (Time.realtimeSinceStartup - mLogInStartTime > 60f)
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_OFFLINE_PROMPT"), false);
				mCurrentPanel = SubPanels.CheckConnection;
			}
		}
	}

	private void RefreshPanelStatus()
	{
		if (!mChallengesOverviewController.IsActive)
		{
			return;
		}
		Bedrock.brUserConnectionStatus connectionStatus = ChallengeManager.ConnectionStatus;
		SubPanels subPanels = mCurrentPanel;
		mCurrentPanel = SubPanels.Normal;
		switch (connectionStatus)
		{
		case Bedrock.brUserConnectionStatus.BR_LOGGING_IN_ANONYMOUSLY:
		case Bedrock.brUserConnectionStatus.BR_LOGGING_IN_REGISTERED:
			mCurrentPanel = SubPanels.LoggingIn;
			break;
		case Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_OFFLINE:
		case Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_OFFLINE:
			mCurrentPanel = SubPanels.CheckConnection;
			break;
		}
		_showingChallengesPanel = mCurrentPanel == SubPanels.Normal;
		if (mCurrentPanel != SubPanels.Normal)
		{
			ClearChallengeList();
			if (mChallengesOverviewController != null)
			{
				mChallengesOverviewController.HideStatus();
			}
		}
		switch (mCurrentPanel)
		{
		case SubPanels.Login:
			mChallengesOverviewController.SetStatus(Language.Get("S_GMG_SIGNIN_PROMPT"), true);
			break;
		case SubPanels.LoginInGame:
			mChallengesOverviewController.SetStatus(Language.Get("S_GMG_QUIT_AND_SIGNIN_PROMPT"), false);
			break;
		case SubPanels.LoggingIn:
			if (subPanels != 0)
			{
				mLogInStartTime = Time.realtimeSinceStartup;
			}
			mChallengesOverviewController.SetStatus(DynamicLoginText(), false);
			break;
		case SubPanels.CheckConnection:
			mChallengesOverviewController.SetStatus(Language.Get("S_GMG_OFFLINE_PROMPT"), false);
			break;
		case SubPanels.Normal:
			StartCoroutine(RefreshListCoroutine());
			break;
		}
	}

	private string DynamicLoginText()
	{
		int num = (int)(Time.realtimeSinceStartup * 3f) % 4;
		string text = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			text = ((i >= num) ? (text + "[#FFFFFF00].") : (text + "."));
		}
		return Language.Get("S_GMG_LOGGING_IN_PROMPT") + text;
	}

	private void ClearChallengeList()
	{
		if (mChallengesOverviewController != null)
		{
			mChallengesOverviewController.ClearList(false);
		}
		foreach (KeyValuePair<uint, ChallengeListItem> challengeListItem in _challengeListItems)
		{
			Recycle(challengeListItem.Value);
		}
		_challengeListItems.Clear();
	}

	private ChallengeListItem TryBuildAndAddListItem(ChallengeData challengeData, uint currentTime)
	{
		ChallengeListItem orCreateUnusedItemFromListItemCache = GetOrCreateUnusedItemFromListItemCache();
		orCreateUnusedItemFromListItemCache.OverviewItem.Setup(challengeData, mChallengesOverviewController, "RefreshSelected");
		if (mChallengesOverviewController != null)
		{
			mChallengesOverviewController.InsertItem(orCreateUnusedItemFromListItemCache, currentTime);
		}
		_challengeListItems.Add(challengeData.Id, orCreateUnusedItemFromListItemCache);
		orCreateUnusedItemFromListItemCache.gameObject.SetActive(true);
		return orCreateUnusedItemFromListItemCache;
	}

	private bool Offline()
	{
		Bedrock.brUserConnectionStatus connectionStatus = ChallengeManager.ConnectionStatus;
		return connectionStatus == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_OFFLINE;
	}

	private IEnumerator RefreshListCoroutine()
	{
		_log.LogDebug("RefreshListCoroutine()");
		if (!_showingChallengesPanel)
		{
			_log.Log("RefreshListCoroutine() called, but not showing challenges panel at the moment. Aborting.");
			yield break;
		}
		if (mChallengesOverviewController != null)
		{
			if (Offline())
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_LOGGING_IN_PROMPT"), false);
			}
			else
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_LIST_REFRESHING"), false);
			}
		}
		ClearChallengeList();
		uint? currentTime = SynchronizedClock.Instance.SynchronizedTime;
		if (!currentTime.HasValue)
		{
			_log.LogWarning("Failed to update challenges - could not get synchronized time.");
			if (!ActStructure.Instance.MissionInProgress)
			{
				GlobeSelectNavigator.Instance.ShowMissionSelect();
			}
			else if (PauseMenuHudController.Instance != null)
			{
				PauseMenuHudController.Instance.GotoObjectives();
			}
			yield break;
		}
		_log.LogDebug("Populating list of challenges.");
		foreach (ChallengeData challenge in ChallengeManager.Instance.DataProvider.AllChallenges)
		{
			if (challenge == null)
			{
				continue;
			}
			challenge.UpdateStatusFromSynchronizedTime(currentTime.Value);
			if (challenge.ShouldBeVisibleInList(currentTime.Value, _lastGameplayStartTime))
			{
				ChallengeListItem challengeListItem = GetChallengeListItem(challenge);
				if (challengeListItem == null)
				{
					TryBuildAndAddListItem(challenge, currentTime.Value);
				}
			}
		}
		if (mChallengesOverviewController != null)
		{
			if (mChallengesOverviewController.CurrentItemCount > 0)
			{
				mChallengesOverviewController.HideStatus();
			}
			else if (Offline())
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_SIGNIN_PROMPT"), true);
			}
			else if (ActStructure.Instance.MissionInProgress)
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_LIST_NO_CHALLENGES_JOINED"), false);
			}
			else
			{
				mChallengesOverviewController.SetStatus(Language.Get("S_GMG_LIST_NO_CHALLENGES_FOUND"), false);
			}
		}
	}
}
