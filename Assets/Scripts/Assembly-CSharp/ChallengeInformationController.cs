using System;
using System.Collections;
using UnityEngine;

public class ChallengeInformationController : MenuScreenBlade
{
	private const float FRIENDS_CONTROLLER_DELAY = 0.1f;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeInformationController), LogLevel.Debug);

	public ChallengeFriendsController FriendsController;

	public SpriteText BronzeGoalText;

	public SpriteText SilverGoalText;

	public SpriteText GoldGoalText;

	public SpriteText FriendText;

	public SpriteText BronzeRewardText;

	public SpriteText SilverRewardText;

	public SpriteText GoldRewardText;

	public SpriteText FriendRewardText;

	public GameObject ResultsHideMeNode;

	public GameObject MarkerHideMeNode;

	private ChallengeFriendData mLeaderBoardData;

	private ChallengeData mData;

	private string mMessageToSend;

	private ulong mFriendId;

	private uint mChallengeId;

	private bool mSendingMessage;

	private bool mEventsEnabled;

	private bool mDisabling;

	private Vector3 _originalFriendsButtonPosition;

	public bool HasData
	{
		get
		{
			return mData != null;
		}
	}

	public ChallengeData ActiveChallenge
	{
		get
		{
			return mData;
		}
	}

	public void SetDisabling()
	{
		mDisabling = true;
	}

	public void ShowChallenge(ChallengeData data)
	{
		if (!(data != mData))
		{
			return;
		}
		if (base.IsActive)
		{
			Deactivate(delegate
			{
				if (!mDisabling)
				{
					Activate();
				}
			});
		}
		else if (!mDisabling && ChallengeManager.ConnectionStatus.IsOnline())
		{
			Activate();
		}
		mData = data;
	}

	public override void Awake()
	{
		base.Awake();
	}

	protected override void OnActivate()
	{
		EnableEvents();
		mSendingMessage = false;
		mDisabling = false;
		if (FriendsController != null)
		{
			FriendsController.Setup(mData);
			if (ChallengeManager.ConnectionStatus.IsOnline())
			{
				FriendsController.DelayedActivate(0.1f);
			}
		}
		Bedrock.UpdateFriendsList();
		UpdateVisuals();
		if (mData != null)
		{
			Debug.Log("Invalidating challenge data cache - re-requesting.");
			ChallengeManager.Instance.LeaderboardDataCache.UpdateCache(mData, true);
		}
		FriendsController.HideNoFriendsHelpPanel();
	}

	private void EnableEvents()
	{
		if (!mEventsEnabled)
		{
			ChallengeFriendItem.ListItemPressed += HandleChallengeLeaderboardListItemPressed;
			ChallengeFriendItem.FriendListItemPressed += HandleChallengeLeaderboardFriendListItemPressed;
			ChallengeData.StatusChanged += HandleChallengeDataStatusChanged;
			ChallengeData.RewardCollected += HandleChallengeDataRewardCollected;
			ChallengeLeaderboardDataCache.Invalidated += HandleChallengeLeaderboardDataCacheInvalidated;
			ChallengeLeaderboardDataCache.Updated += HandleChallengeLeaderboardDataCacheUpdated;
			Bedrock.BedrockUIClosed += HandleBedrockBedrockUIClosed;
			Bedrock.FriendCacheUpdated += FriendsListUpdated;
			ActivateWatcher.ConnectionStatusChange += HandleActivateWatcherConnectionStatusChange;
			ChallengeInstanceCollection.JoinedChanged += HandleChallengeInstanceCollectionJoinedChanged;
			mEventsEnabled = true;
		}
	}

	private void DisableEvents()
	{
		if (mEventsEnabled)
		{
			ChallengeFriendItem.ListItemPressed -= HandleChallengeLeaderboardListItemPressed;
			ChallengeFriendItem.FriendListItemPressed -= HandleChallengeLeaderboardFriendListItemPressed;
			ChallengeData.StatusChanged -= HandleChallengeDataStatusChanged;
			ChallengeData.RewardCollected -= HandleChallengeDataRewardCollected;
			ChallengeLeaderboardDataCache.Invalidated -= HandleChallengeLeaderboardDataCacheInvalidated;
			ChallengeLeaderboardDataCache.Updated -= HandleChallengeLeaderboardDataCacheUpdated;
			Bedrock.BedrockUIClosed -= HandleBedrockBedrockUIClosed;
			Bedrock.FriendCacheUpdated -= FriendsListUpdated;
			ActivateWatcher.ConnectionStatusChange -= HandleActivateWatcherConnectionStatusChange;
			ChallengeInstanceCollection.JoinedChanged -= HandleChallengeInstanceCollectionJoinedChanged;
			mEventsEnabled = false;
		}
	}

	private void OnDisable()
	{
		DisableEvents();
	}

	protected override void OnDeactivate()
	{
		DisableEvents();
		base.OnDeactivate();
		if (FriendsController != null)
		{
			FriendsController.DelayedDeactivate(0.1f);
			FriendsController.Clear();
		}
	}

	private void HandleChallengeInstanceCollectionJoinedChanged(object sender, ValueEventArgs<uint> e)
	{
		if (mData != null && e.Value == mData.Id)
		{
			RepopulateLeaderboardList();
		}
	}

	private void HandleActivateWatcherConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
		if (!e.NewStatus.IsOnline() || !e.NewStatus.IsRegistered())
		{
			GlobeSelectNavigator.Instance.ShowMissionSelect();
		}
		else
		{
			UpdateVisuals();
		}
	}

	private void HandleChallengeLeaderboardDataCacheInvalidated(object sender, ChallengeEventArgs e)
	{
		if (!(mData == null) && e.Challenge.Id == mData.Id)
		{
			mLeaderBoardData = null;
			FriendsController.HideNoFriendsHelpPanel();
			string text = Language.Get("S_GMG_STATE_UPDATING");
			text += "...";
			SetLeaderboardPanelStatusText(text, true, true);
			UpdateVisuals();
		}
	}

	private void HandleChallengeLeaderboardDataCacheUpdated(object sender, ChallengeLeaderboardDataCache.UpdatedEventArgs e)
	{
		if (!(mData == null) && e.Challenge.Id == mData.Id)
		{
			mLeaderBoardData = e.CacheData.FriendData;
			SetLeaderboardPanelStatusText(string.Empty, false, false);
			UpdateVisuals();
		}
	}

	private void HandleChallengeLeaderboardFriendListItemPressed(object sender, ValueEventArgs<FriendData> e)
	{
		_log.Log("Friend list item pressed.");
		FriendData value = e.Value;
		string empty = string.Empty;
		long? bestScoreSubmittedThisCycle = mData.BestScoreSubmittedThisCycle;
		if (mData.Status == ChallengeStatus.Open)
		{
			empty = ((!bestScoreSubmittedThisCycle.HasValue) ? Language.GetFormatString("S_GMG_MESSAGE_OPEN_SCORE", mData.LocalizedName) : ((!mData.IsTime) ? Language.GetFormatString("S_GMG_MESSAGE_OPEN_NO_SCORE", bestScoreSubmittedThisCycle.Value, mData.LocalizedGoalNoun, mData.LocalizedName) : Language.GetFormatString("S_GMG_MESSAGE_OPEN_NO_SCORE_TIME", TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value), mData.LocalizedName)));
		}
		else if (bestScoreSubmittedThisCycle.HasValue)
		{
			string text = ((!mData.IsTime) ? bestScoreSubmittedThisCycle.Value.ToString() : TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value));
			empty = Language.GetFormatString("S_GMG_MESSAGE_CLOSED_SCORE", text, mData.LocalizedGoalNoun, mData.LocalizedName);
		}
		else
		{
			empty = Language.GetFormatString("S_GMG_MESSAGE_CLOSED_NO_SCORE", mData.LocalizedName);
		}
		ShowSendMessageToFriendDialog(value.UserId, value.UserName, empty, mData);
	}

	private void HandleChallengeLeaderboardListItemPressed(object sender, ValueEventArgs<ChallengeLeaderboardRow> e)
	{
		_log.Log("Leaderboard list item pressed - {0}", e.Value);
		ChallengeLeaderboardRow value = e.Value;
		if (value.UserId == Bedrock.getDefaultOnlineId())
		{
			return;
		}
		string empty = string.Empty;
		long? bestScoreSubmittedThisCycle = mData.BestScoreSubmittedThisCycle;
		if (mData.Status == ChallengeStatus.Open)
		{
			if (bestScoreSubmittedThisCycle.HasValue)
			{
				string text = ((!mData.IsTime) ? bestScoreSubmittedThisCycle.Value.ToString() : TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value));
				string text2 = ((!mData.IsTime) ? mData.LocalizedGoalNoun : string.Empty);
				empty = ((value.Rank >= mLeaderBoardData.PlayerPosition) ? Language.GetFormatString("S_GMG_MESSAGE_CAN_YOU_BEAT", text, text2, mData.LocalizedName) : Language.GetFormatString("S_GMG_MESSAGE_JOINED_GAINING", mData.LocalizedName, text, text2));
			}
			else
			{
				empty = Language.GetFormatString("S_GMG_MESSAGE_PREPARE", mData.LocalizedName);
			}
		}
		else if (bestScoreSubmittedThisCycle.HasValue)
		{
			string text3 = ((!mData.IsTime) ? bestScoreSubmittedThisCycle.Value.ToString() : TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value));
			empty = ((value.Rank >= mLeaderBoardData.PlayerPosition) ? Language.GetFormatString("S_GMG_MESSAGE_JOINED_BEAT", text3, mData.LocalizedGoalNoun, mData.LocalizedName, Language.Get("S_GMG_NOUN_CURRENCY")) : Language.GetFormatString("S_GMG_MESSAGE_ALMOST_BEAT", mData.LocalizedName, text3, mData.LocalizedGoalNoun));
		}
		else
		{
			empty = Language.GetFormatString("S_GMG_MESSAGE_JOINED_NICE_JOB", value.Score, mData.LocalizedGoalNoun, mData.LocalizedName);
		}
		ShowSendMessageToFriendDialog(value.UserId, value.UserName, empty, mData);
	}

	private void HandleChallengeDataStatusChanged(object sender, ChallengeEventArgs e)
	{
		UpdateVisuals();
	}

	private void HandleChallengeDataRewardCollected(object sender, EventArgs e)
	{
		UpdateVisuals();
	}

	private void SetLeaderboardPanelStatusText(string statusText, bool showText, bool showThrobber)
	{
		FriendsController.SetLeaderboardPanelStatusText(statusText, showText, showThrobber);
	}

	private void FriendsListUpdated(object sender, EventArgs args)
	{
		UpdateVisuals();
	}

	private void HandleCollectButtonPressed()
	{
		if (mLeaderBoardData == null)
		{
			_log.LogError("Cannot collect reward because we don't have the friend data yet.");
			return;
		}
		long? bestScoreSubmittedThisCycle = mData.BestScoreSubmittedThisCycle;
		if (!bestScoreSubmittedThisCycle.HasValue)
		{
			_log.LogError("Cannot collect reward because we don't have any submitted score for this challenge.");
			return;
		}
		uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
		if (!synchronizedTime.HasValue)
		{
			_log.LogError("Could not collect reward. Clocks not synchronized.");
		}
		else
		{
			mData.CollectReward(synchronizedTime.Value, bestScoreSubmittedThisCycle.Value, mLeaderBoardData.FriendsRankedLower);
		}
	}

	private void HandleAddFriendButtonPressed()
	{
		_log.Log("Add Friend Button Pressed.");
		ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_ADD_FRIENDS_UI);
	}

	private void HandleBedrockBedrockUIClosed(object sender, Bedrock.brUserInterfaceReasonForCloseEventArgs e)
	{
		_log.Log("Came back from Bedrock screen. Refreshing data. UserId = {0}, UserName = {1}, BedrockStatus={2}", ChallengeManager.UserId, Bedrock.getUsername(), ChallengeManager.ConnectionStatus);
		if (mData != null)
		{
			Debug.Log("Updating Challenge Leaderboard Cache");
			if (ChallengeManager.Instance.LeaderboardDataCache != null)
			{
				ChallengeManager.Instance.LeaderboardDataCache.UpdateCache(mData, true);
				Debug.Log("Updating Challenge Leaderboard Cache Complete...");
			}
			else
			{
				Debug.Log("Challenge Leaderboard Cache was null! " + mData.Id);
			}
		}
	}

	private void ShowSendMessageToFriendDialog(ulong friendId, string friendName, string message, ChallengeData data)
	{
		if (friendId == ChallengeManager.UserId)
		{
			_log.Log("Aborting showing message dialog to send message to user {0}, it's Ourselves!", friendId);
			return;
		}
		_log.Log("Showing message dialog to send message to {0} ({1}) with content {2}", friendName, friendId, message);
		mMessageToSend = message;
		mChallengeId = ((data.ChallengeInstance != null) ? data.ChallengeInstance.ChallengeId : 0u);
		mFriendId = friendId;
		MessageBoxController.Instance.DoChallengeFriendMessageDialogue(this, "SendFriendMessage", "CancelFriendMessage", friendName, mMessageToSend);
	}

	private void CancelFriendMessage()
	{
	}

	private void SendFriendMessage()
	{
		if (mSendingMessage)
		{
			_log.LogWarning("We got a re-entrant send message. Skipping.");
			return;
		}
		mSendingMessage = true;
		StartCoroutine(HandleSendPressedInternal());
	}

	private IEnumerator HandleSendPressedInternal()
	{
		_log.Log("Send pressed.");
		ulong userId = ChallengeManager.UserId;
		string message = string.Format("{0}: {1}", ChallengeManager.UserName, mMessageToSend);
		_log.Log("Sending message '{0}' from user {1} to user {2} about challenge {3}.", message, userId, mFriendId, mChallengeId);
		short taskHandle = Bedrock.SendChallengeNotification(new ulong[1] { mFriendId }, mChallengeId, message, 0, null, null);
		BedrockTask task = new BedrockTask(taskHandle);
		yield return StartCoroutine(task.WaitForTaskToCompleteOrTimeoutCoroutine());
		if (task.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS)
		{
			Debug.Log("Message was successfully sent.");
		}
		else
		{
			MessageBoxController.Instance.DoFailedSendFriendMessageDialogue();
			Debug.Log("Failed to send message: " + task);
		}
		mSendingMessage = false;
	}

	private void UpdateVisuals()
	{
		if (mData == null)
		{
			return;
		}
		mLeaderBoardData = null;
		ChallengeLeaderboardDataCache.CachedLeaderboardData data = ChallengeManager.Instance.LeaderboardDataCache.GetData(mData);
		if (data != null)
		{
			mLeaderBoardData = data.FriendData;
		}
		uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
		bool flag = synchronizedTime.HasValue && mData.DidJoinInCurrentCycle(synchronizedTime.Value);
		long? bestScoreSubmittedThisCycle = mData.BestScoreSubmittedThisCycle;
		char c = CommonHelper.HardCurrencySymbol();
		if (MarkerHideMeNode != null)
		{
			MarkerHideMeNode.SetActive(true);
		}
		if (mData.Status == ChallengeStatus.Open)
		{
			if (ResultsHideMeNode != null)
			{
				ResultsHideMeNode.SetActive(true);
			}
			BronzeGoalText.Text = mData.BuildGoalString(mData.BronzeGoal);
			SilverGoalText.Text = mData.BuildGoalString(mData.SilverGoal);
			GoldGoalText.Text = mData.BuildGoalString(mData.GoldGoal);
			FriendText.Text = Language.Get("S_GMG_FRIENDS_DEFEATED");
			BronzeRewardText.Text = string.Format("{0} {1}", c, mData.GetRewardForMedal(ChallengeMedalType.Bronze).ToString("n0"));
			SilverRewardText.Text = string.Format("{0} {1}", c, mData.GetRewardForMedal(ChallengeMedalType.Silver).ToString("n0"));
			GoldRewardText.Text = string.Format("{0} {1}", c, mData.GetRewardForMedal(ChallengeMedalType.Gold).ToString("n0"));
			FriendRewardText.Text = string.Format("{0} {1}", c, mData.GetRewardForFriendsBeaten(1uL).ToString("n0"));
		}
		else if (mData.Status == ChallengeStatus.Finished)
		{
			if (ResultsHideMeNode != null)
			{
				ResultsHideMeNode.SetActive(false);
			}
			if (!flag || !bestScoreSubmittedThisCycle.HasValue)
			{
				BronzeGoalText.Text = string.Empty;
				SilverGoalText.Text = Language.Get("S_GMG_NO_SCORE_POSTED");
				GoldGoalText.Text = string.Empty;
				FriendText.Text = string.Empty;
				BronzeRewardText.Text = string.Empty;
				SilverRewardText.Text = string.Empty;
				GoldRewardText.Text = string.Empty;
				FriendRewardText.Text = string.Empty;
				if (MarkerHideMeNode != null)
				{
					MarkerHideMeNode.SetActive(false);
				}
			}
			else
			{
				GoldGoalText.Text = Language.Get("S_GMG_REWARD_BREAKDOWN_TITLE");
				GoldRewardText.Text = string.Empty;
				ulong num = ((mLeaderBoardData != null) ? mLeaderBoardData.FriendsRankedLower : 0);
				ChallengeMedalType medalForScore = mData.GetMedalForScore(bestScoreSubmittedThisCycle.Value);
				uint rewardForMedal = mData.GetRewardForMedal(medalForScore);
				uint rewardForFriendsBeaten = mData.GetRewardForFriendsBeaten(num);
				uint num2 = rewardForMedal + rewardForFriendsBeaten;
				BronzeGoalText.Text = mData.BuildRewardString(bestScoreSubmittedThisCycle.Value);
				BronzeRewardText.Text = string.Format("{0} {1}", c, rewardForMedal.ToString("n0"));
				SilverGoalText.Text = Language.GetFormatString("S_GMG_CHALLENGE_FRIENDS_DEFEATED", num);
				SilverRewardText.Text = string.Format("{0} {1}", c, rewardForFriendsBeaten.ToString("n0"));
				bool flag2 = num2 == 0 || mData.HasPickedUpRewardSinceLastJoin;
				FriendText.Text = ((!flag2) ? Language.Get("S_GMG_STATE_COLLECT") : Language.Get("S_GMG_STATE_AWARDED"));
				FriendRewardText.Text = string.Format("{0} {1}", c, num2.ToString("n0"));
			}
		}
		else if (mData.Status == ChallengeStatus.Invalid)
		{
			BronzeGoalText.Text = string.Empty;
			SilverGoalText.Text = string.Empty;
			GoldGoalText.Text = string.Empty;
			FriendText.Text = string.Empty;
			BronzeRewardText.Text = string.Empty;
			SilverRewardText.Text = string.Empty;
			GoldRewardText.Text = string.Empty;
			FriendRewardText.Text = string.Empty;
		}
		RepopulateLeaderboardList();
	}

	public void AuthorizeFacebook()
	{
		if (!Bedrock.FacebookEnabled)
		{
			Bedrock.FacebookEnabled = true;
			Bedrock.FacebookRequestPublishPermissions(false);
		}
	}

	private void RepopulateLeaderboardList()
	{
		if (mLeaderBoardData != null)
		{
			Debug.Log("=====> RepopulateLeaderboardList Challenge " + mChallengeId + " mData leaderboard " + mData.LeaderboardId + " friend data leaderboard id " + mLeaderBoardData.LeaderboardID);
		}
		FriendsController.RepopulateLeaderboardList(mLeaderBoardData, mData);
	}
}
