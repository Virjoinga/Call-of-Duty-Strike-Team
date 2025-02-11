using System.Collections.Generic;
using UnityEngine;

public class ChallengeFriendsController : MenuScreenBlade
{
	private const float TIME_BETWEEN_ADDS = 0.1f;

	private const float ITEM_HEIGHT = 22f;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeInformationController), LogLevel.Debug);

	public ChallengeFriendItem ItemPrefab;

	public ChallengeFriendItem NonCompetingFriendItemPrefab;

	private List<UIListItemContainer> mCurrentPanels;

	private CommonBackgroundBox mBox;

	private AnimateCommonBackgroundBox mAnimation;

	private UIScrollList mFriendsScrollList;

	private float mTimeSinceLastAdd;

	private float mWidth;

	private float mHeight;

	private ChallengeData mChallengeData;

	public SpriteText LeaderboardStatusText;

	public override void Awake()
	{
		base.Awake();
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mHeight = 22f * (float)num;
		mCurrentPanels = new List<UIListItemContainer>();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mAnimation = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mFriendsScrollList = GetComponentInChildren<UIScrollList>();
		if (mBox != null && mAnimation != null && mFriendsScrollList != null)
		{
			mBox.Resize();
			mWidth = mBox.ForegroundSize.x;
		}
	}

	public void Setup(ChallengeData data)
	{
	}

	public void RepopulateLeaderboardList(ChallengeFriendData leaderboardData, ChallengeData challengeData)
	{
		mChallengeData = challengeData;
		_log.LogDebug("RepopulateLeaderboardList()");
		Clear();
		if (leaderboardData == null)
		{
			return;
		}
		_log.LogDebug("Creating rows for {0} friend leaderboard entries.", leaderboardData.Rows.Length);
		long? num = null;
		HashSet<ulong> hashSet = new HashSet<ulong>();
		ulong userId = ChallengeManager.UserId;
		ChallengeLeaderboardRow[] rows = leaderboardData.Rows;
		foreach (ChallengeLeaderboardRow challengeLeaderboardRow in rows)
		{
			_log.LogDebug("Creating row from data: {0}", challengeLeaderboardRow);
			ChallengeFriendItem challengeFriendItem = Object.Instantiate(ItemPrefab) as ChallengeFriendItem;
			if (challengeFriendItem != null)
			{
				challengeFriendItem.LayoutComponents(mWidth, mHeight);
				challengeFriendItem.Setup(challengeLeaderboardRow, false, mChallengeData.IsTime);
				mCurrentPanels.Add(challengeFriendItem.Container);
			}
			if (challengeLeaderboardRow.UserId == userId)
			{
				if (!challengeData.BestScoreSubmittedThisCycle.HasValue || challengeData.IsBetterScore(challengeLeaderboardRow.Score, challengeData.BestScoreSubmittedThisCycle.Value))
				{
					_log.LogError("Score on leaderboard ({0}) was better than local best submitted score ({1}). Copying leaderboard score to local data.", challengeLeaderboardRow.Score, challengeData.BestScoreSubmittedThisCycle);
					uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
					if (synchronizedTime.HasValue)
					{
						SecureStorage.Instance.SetBestScoreSubmittedThisCycle(challengeData, challengeLeaderboardRow.Score);
						SecureStorage.Instance.SetBestScoreSubmittedThisCycleTime(challengeData, synchronizedTime.Value);
					}
					else
					{
						_log.LogError("Could not update leaderboard value - clock is not synchronized.");
					}
				}
				num = challengeLeaderboardRow.Score;
			}
			hashSet.Add(challengeLeaderboardRow.UserId);
		}
		if (!num.HasValue && challengeData.IsJoined)
		{
			_log.LogDebug("Didn't find current player on leaderboard results. Inserting extra row at end.");
			ChallengeLeaderboardRow data = new ChallengeLeaderboardRow(userId, ChallengeManager.UserName, 0L, 1, false);
			ChallengeFriendItem challengeFriendItem2 = Object.Instantiate(ItemPrefab) as ChallengeFriendItem;
			if (challengeFriendItem2 != null)
			{
				challengeFriendItem2.LayoutComponents(mWidth, mHeight);
				challengeFriendItem2.Setup(data, true, mChallengeData.IsTime);
				mCurrentPanels.Add(challengeFriendItem2.Container);
			}
			hashSet.Add(userId);
		}
		_log.LogDebug("Getting list of friends.");
		ChallengeLeaderboardProvider leaderboardProvider = ChallengeManager.Instance.LeaderboardProvider;
		FriendData[] friends;
		if (leaderboardProvider.GetFriendsForCurrentPlayer(out friends))
		{
			_log.LogDebug("Got list of {0} friends.", friends.Length);
			List<FriendData> list = new List<FriendData>();
			FriendData[] array = friends;
			foreach (FriendData friendData in array)
			{
				if (hashSet.Contains(friendData.UserId))
				{
					_log.LogDebug("Friend {0} ({1}) WAS on leaderboard.", friendData.UserName, friendData.UserId);
				}
				else
				{
					_log.LogDebug("Friend {0} ({1}) WAS NOT on leaderboard.", friendData.UserName, friendData.UserId);
					list.Add(friendData);
				}
			}
			_log.LogDebug("{0} friends were missing from leaderboard. Generating items.", list.Count);
			foreach (FriendData item in list)
			{
				_log.LogDebug("Creating friend list item for friend {0}", item);
				ChallengeFriendItem challengeFriendItem3 = Object.Instantiate(NonCompetingFriendItemPrefab) as ChallengeFriendItem;
				if (challengeFriendItem3 != null)
				{
					challengeFriendItem3.LayoutComponents(mWidth, mHeight);
					challengeFriendItem3.Setup(item);
					mCurrentPanels.Add(challengeFriendItem3.Container);
				}
			}
			_log.LogDebug("Done processing friends list!");
			if (friends.Length == 0)
			{
				mCurrentPanels.Clear();
				ShowNoFriendsHelpPanel();
			}
			else
			{
				HideNoFriendsHelpPanel();
			}
		}
		else
		{
			_log.LogError("Unable to get list of friends for user.");
			SetLeaderboardPanelStatusText(Language.Get("S_GMG_FRIENDS_ERROR"), true, false);
		}
	}

	public void ShowNoFriendsHelpPanel()
	{
		uint synchronizedTimeOrBestGuess = SynchronizedClock.Instance.SynchronizedTimeOrBestGuess;
		uint remainingTime = mChallengeData.GetRemainingTime(synchronizedTimeOrBestGuess);
		string fuzzyTimeStringFromSeconds = TimeUtils.GetFuzzyTimeStringFromSeconds(remainingTime);
		if (mChallengeData.Status == ChallengeStatus.Open)
		{
			LeaderboardStatusText.Text = Language.Get("S_GMG_ADD_FRIENDS_HELP_OPEN") + "\n" + Language.GetFormatString("S_GMG_ADD_FRIENDS_HELP_OPEN_SUBTITLE", fuzzyTimeStringFromSeconds);
		}
		else
		{
			LeaderboardStatusText.Text = Language.GetFormatString("S_GMG_ADD_FRIENDS_HELP_CLOSED");
		}
		LeaderboardStatusText.Hide(false);
	}

	public void HideNoFriendsHelpPanel()
	{
		LeaderboardStatusText.Hide(true);
	}

	public void SetLeaderboardPanelStatusText(string text, bool showText, bool showSignInAnimation)
	{
		LeaderboardStatusText.Hide(!showText);
		LeaderboardStatusText.Text = text;
	}

	public void Clear()
	{
		foreach (UIListItemContainer mCurrentPanel in mCurrentPanels)
		{
			Object.Destroy(mCurrentPanel.gameObject);
		}
		mCurrentPanels.Clear();
		mFriendsScrollList.ClearList(true);
	}

	public override void Update()
	{
		base.Update();
		if (base.IsActive && !LeaderboardStatusText.IsHidden())
		{
			ShowNoFriendsHelpPanel();
		}
		if (!(mAnimation != null) || !(mFriendsScrollList != null) || mAnimation.IsOpening)
		{
			return;
		}
		bool isOpen = mAnimation.IsOpen;
		mTimeSinceLastAdd += TimeManager.DeltaTime;
		if (!(mTimeSinceLastAdd >= 0.1f))
		{
			return;
		}
		for (int i = 0; i < mCurrentPanels.Count; i++)
		{
			if (mCurrentPanels[i] != null)
			{
				bool flag = mFriendsScrollList.Contains(mCurrentPanels[i]);
				if (!flag && isOpen)
				{
					mFriendsScrollList.InsertItem(mCurrentPanels[i], i);
					mTimeSinceLastAdd = 0f;
					break;
				}
				if (flag && !isOpen)
				{
					mFriendsScrollList.RemoveItem(mCurrentPanels[i], false);
					break;
				}
			}
		}
	}

	private void OnAddFriendsButtonPressed()
	{
		if (Bedrock.getUserConnectionStatus().IsRegistered())
		{
			ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_ADD_FRIENDS_UI);
		}
		else
		{
			ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_LOG_ON_UI);
		}
	}
}
