using System;
using System.Collections;
using UnityEngine;

public class ChallengeOverviewItem : MonoBehaviour
{
	public SpriteText ChallengeName;

	public SpriteText ChallengeDescription;

	public SpriteText FriendsPlaying;

	public SpriteText TimeDisplay;

	public SpriteText CurrentDisplay;

	public SpriteText BestDisplay;

	public PackedSprite GoldMedal;

	public PackedSprite SilverMedal;

	public PackedSprite BronzeMedal;

	public UIButton PlayButton;

	public PackedSprite SelectedBackground;

	public FrontEndButton PlayButtonFEButton;

	private CommonBackgroundBoxPlacement[] mPlacements;

	private ChallengeInformationController mChallengeInformation;

	private ChallengeData mData;

	private MonoBehaviour mTappedCallback;

	private string mTappedMethod;

	private float mWidth;

	private float mHeight;

	private float mPixelSize;

	private uint mCurrentTime;

	private uint? mRewardAmount;

	private bool mShouldContextButtonBePressable;

	private string mTimeUntilCloseString;

	private string mFriendsPlayingString;

	private bool mDoToggleText;

	private bool mTextIsToggled;

	private bool mShowingStopButton;

	public ChallengeInformationController ChallengeInformation
	{
		set
		{
			mChallengeInformation = value;
		}
	}

	public ChallengeData ChallengeData
	{
		get
		{
			return mData;
		}
	}

	private string SubtitleTextFromCurrentChallengeState
	{
		get
		{
			if (ChallengeData.IsJoined)
			{
				if (mShowingStopButton)
				{
					return Language.Get("S_GMG_BUTTON_STOP_CONFIRM");
				}
				Challenge challengeInstance = ChallengeData.ChallengeInstance;
				return (!(challengeInstance == null)) ? challengeInstance.LifeRemainingText : null;
			}
			return null;
		}
	}

	private void Awake()
	{
		mPlacements = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		if (PlayButton != null)
		{
			PlayButtonFEButton = PlayButton.GetComponent<FrontEndButton>();
		}
	}

	public uint? Setup(ChallengeData data, MonoBehaviour tappedCallback, string tappedMethod)
	{
		mRewardAmount = null;
		mData = data;
		mCurrentTime = SynchronizedClock.Instance.SynchronizedTimeOrBestGuess;
		mTappedCallback = tappedCallback;
		mTappedMethod = tappedMethod;
		if (mData != null)
		{
			UpdateFromTimeChange(SynchronizedClock.Instance.SynchronizedTimeOrBestGuess);
			UpdateVisuals();
		}
		return mRewardAmount;
	}

	public void LayoutComponents(float width, float height, float pixelSize)
	{
		mWidth = width;
		mHeight = height;
		mPixelSize = pixelSize;
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] array = mPlacements;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
		Vector2 boxSize = new Vector2(mWidth, mHeight);
		CommonBackgroundBoxPlacement[] array = mPlacements;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
		ChallengeInstanceCollection.JoinedChanged += HandleChallengeDataJoinedChanged;
		ChallengeManager.UpdateComplete += HandleChallengeManagerUpdateComplete;
		ChallengeLeaderboardDataCache.Invalidated += HandleChallengeLeaderboardDataCacheInvalidated;
		ChallengeLeaderboardDataCache.Updated += HandleChallengeLeaderboardDataUpdated;
		ChallengeData.StatusChanged += HandleChallengeDataStatusChanged;
		ChallengeData.RewardCollected += HandleChallengeDataRewardCollected;
		Challenge.StatusTextChanged += HandleChallengeStatusTextChanged;
		mShowingStopButton = false;
		if (ChallengeData != null)
		{
			ChallengeManager.Instance.LeaderboardDataCache.UpdateCache(ChallengeData, false);
		}
		UpdateVisuals();
	}

	private void OnDisable()
	{
		ChallengeInstanceCollection.JoinedChanged -= HandleChallengeDataJoinedChanged;
		ChallengeManager.UpdateComplete -= HandleChallengeManagerUpdateComplete;
		ChallengeLeaderboardDataCache.Invalidated -= HandleChallengeLeaderboardDataCacheInvalidated;
		ChallengeLeaderboardDataCache.Updated -= HandleChallengeLeaderboardDataUpdated;
		ChallengeData.StatusChanged -= HandleChallengeDataStatusChanged;
		ChallengeData.RewardCollected -= HandleChallengeDataRewardCollected;
		Challenge.StatusTextChanged -= HandleChallengeStatusTextChanged;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
		StopAllCoroutines();
	}

	private void HandleChallengeDataJoinedChanged(object sender, ValueEventArgs<uint> e)
	{
		if (mData != null && e.Value == mData.Id)
		{
			UpdateVisuals();
		}
	}

	public void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (!(PlayButton != null) || !(GlobeSelectNavigator.Instance != null))
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		if (!new Rect(vector.x - mWidth * 0.5f, vector.y - mHeight * 0.5f, mWidth, mHeight).Contains(fingerPos))
		{
			return;
		}
		Vector3 vector2 = Camera.main.WorldToScreenPoint(PlayButton.transform.position);
		float num = PlayButton.width / mPixelSize;
		float num2 = PlayButton.height / mPixelSize;
		Rect rect = new Rect(vector2.x - num * 0.5f, vector2.y - num2 * 0.5f, num, num2);
		if (mChallengeInformation != null && !rect.Contains(fingerPos) && mChallengeInformation.IsActive)
		{
			mChallengeInformation.ShowChallenge(mData);
			UpdateVisuals();
			if (mTappedCallback != null && mTappedMethod != null && mTappedMethod != string.Empty)
			{
				mTappedCallback.Invoke(mTappedMethod, 0f);
			}
		}
	}

	private void UpdateText(string statusText, string statusText2, string placementText, string timeText)
	{
		if (ChallengeName != null && ChallengeDescription != null)
		{
			if (mData == null)
			{
				ChallengeName.Text = string.Empty;
				ChallengeDescription.Text = string.Empty;
			}
			else
			{
				ChallengeName.Text = Language.Get(mData.Name).ToUpper();
				ChallengeDescription.Text = Language.Get(mData.Description);
			}
			if (FriendsPlaying != null)
			{
				FriendsPlaying.Text = placementText.ToUpper();
			}
			if (TimeDisplay != null)
			{
				TimeDisplay.Text = timeText.ToUpper();
			}
			if (CurrentDisplay != null)
			{
				CurrentDisplay.Text = statusText.ToUpper();
			}
			if (BestDisplay != null)
			{
				BestDisplay.Text = statusText2.ToUpper();
			}
		}
	}

	private void UpdateButton(string text, string subtitleText, FrontEndButton.State buttonState)
	{
		if (PlayButton != null)
		{
			PlayButton.scriptWithMethodToInvoke = this;
			PlayButton.methodToInvoke = "HandleChallengeContextButtonPressed";
			if (PlayButton.spriteText != null)
			{
				if (text != null && subtitleText != null && subtitleText.Length > 0)
				{
					PlayButton.spriteText.Text = text.ToUpper() + " " + subtitleText.ToUpper();
				}
				else if (text != null)
				{
					PlayButton.spriteText.Text = text.ToUpper();
				}
				else
				{
					Debug.LogError("Should this be possible? Subtitle but no main text");
				}
			}
		}
		if (PlayButtonFEButton != null)
		{
			PlayButtonFEButton.CurrentState = buttonState;
		}
	}

	private void Update()
	{
		UpdateSelectionBackground();
	}

	private void UpdateMedalImage(ChallengeMedalType type)
	{
		if (GoldMedal != null)
		{
			GoldMedal.transform.localScale = ((type != ChallengeMedalType.Gold) ? new Vector3(0f, 0f, 0f) : new Vector3(1f, 1f, 1f));
		}
		if (SilverMedal != null)
		{
			SilverMedal.transform.localScale = ((type != ChallengeMedalType.Silver) ? new Vector3(0f, 0f, 0f) : new Vector3(1f, 1f, 1f));
		}
		if (BronzeMedal != null)
		{
			BronzeMedal.transform.localScale = ((type != ChallengeMedalType.Bronze) ? new Vector3(0f, 0f, 0f) : new Vector3(1f, 1f, 1f));
		}
	}

	private void UpdateSelectionBackground()
	{
		if (SelectedBackground != null)
		{
			if (mChallengeInformation != null && mChallengeInformation.ActiveChallenge == mData)
			{
				SelectedBackground.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				SelectedBackground.transform.localScale = new Vector3(0f, 0f, 0f);
			}
		}
	}

	private void UpdateCurrentAndBest()
	{
		if (mData.Status != ChallengeStatus.Open || !(CurrentDisplay != null))
		{
			return;
		}
		Challenge challengeInstance = mData.ChallengeInstance;
		if (challengeInstance == null)
		{
			long? bestScoreSubmittedThisCycle = mData.BestScoreSubmittedThisCycle;
			if (bestScoreSubmittedThisCycle.HasValue && mData.DidJoinInCurrentCycle(mCurrentTime))
			{
				string text = ((!mData.IsTime) ? bestScoreSubmittedThisCycle.Value.ToString() : TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value));
				CurrentDisplay.Text = text;
			}
		}
		else
		{
			CurrentDisplay.Text = mData.ChallengeInstance.StatusPanelStatusText;
		}
	}

	protected void UpdateVisuals()
	{
		if (ChallengeData == null)
		{
			mShouldContextButtonBePressable = false;
			mTimeUntilCloseString = string.Empty;
			UpdateButton(string.Empty, string.Empty, FrontEndButton.State.Normal);
			UpdateText(string.Empty, string.Empty, string.Empty, string.Empty);
			UpdateMedalImage(ChallengeMedalType.None);
			UpdateSelectionBackground();
			return;
		}
		mRewardAmount = null;
		mShouldContextButtonBePressable = true;
		ChallengeStatus status = ChallengeData.Status;
		ChallengeLeaderboardDataCache.CachedLeaderboardData data = ChallengeManager.Instance.LeaderboardDataCache.GetData(ChallengeData);
		ChallengeFriendData challengeFriendData = ((data != null) ? data.FriendData : null);
		uint synchronizedTimeOrBestGuess = SynchronizedClock.Instance.SynchronizedTimeOrBestGuess;
		bool flag = ChallengeData.DidJoinInCurrentCycle(synchronizedTimeOrBestGuess);
		long? num = ((!flag) ? null : ChallengeData.BestScoreSubmittedThisCycle);
		string text = string.Empty;
		string subtitleText = string.Empty;
		string text2 = string.Empty;
		FrontEndButton.State buttonState = FrontEndButton.State.Normal;
		if (challengeFriendData == null || challengeFriendData.TotalFriendsPlaying == 0L)
		{
			mFriendsPlayingString = string.Empty;
		}
		else if (ChallengeData.IsJoined || flag)
		{
			if (ChallengeData.BestScoreSubmittedThisCycle.HasValue)
			{
				mFriendsPlayingString = Language.GetFormatString("S_GMG_X_OF_Y_FRIENDS", challengeFriendData.PlayerPosition, challengeFriendData.TotalFriendsPlaying + 1);
			}
			else
			{
				string key = ((challengeFriendData.TotalFriendsPlaying != 1) ? "S_GMG_FRIEND_PLAYING_PLURAL" : "S_GMG_FRIEND_PLAYING_SINGLE");
				mFriendsPlayingString = Language.GetFormatString(key, challengeFriendData.TotalFriendsPlaying);
			}
		}
		else if (ChallengeData.Status == ChallengeStatus.Finished)
		{
			if (ChallengeData.BestScoreSubmittedThisCycle.HasValue)
			{
				mFriendsPlayingString = Language.GetFormatString("S_GMG_X_OF_Y_FRIENDS", challengeFriendData.PlayerPosition, challengeFriendData.TotalFriendsPlaying + 1);
			}
			else if (challengeFriendData.TotalFriendsPlaying == 0L)
			{
				mFriendsPlayingString = Language.GetFormatString("S_GMG_FRIEND_PLAYING_PLURAL", 0);
			}
			else
			{
				string key2 = ((challengeFriendData.TotalFriendsPlaying != 1) ? "S_GMG_FRIEND_PLAYED_PLURAL" : "S_GMG_FRIEND_PLAYED_SINGLE");
				mFriendsPlayingString = Language.GetFormatString(key2, challengeFriendData.TotalFriendsPlaying);
			}
		}
		else
		{
			string key3 = ((challengeFriendData.TotalFriendsPlaying != 1) ? "S_GMG_FRIEND_PLAYING_PLURAL" : "S_GMG_FRIEND_PLAYING_SINGLE");
			mFriendsPlayingString = Language.GetFormatString(key3, challengeFriendData.TotalFriendsPlaying);
		}
		ChallengeMedalType challengeMedalType = ChallengeMedalType.None;
		if (num.HasValue)
		{
			challengeMedalType = ChallengeData.GetMedalForScore(num.Value);
		}
		switch (status)
		{
		case ChallengeStatus.Open:
		{
			bool isJoined = ChallengeData.IsJoined;
			text = Language.Get("S_GMG_BUTTON_JOIN");
			if (isJoined)
			{
				text = Language.Get((!mShowingStopButton) ? "S_GMG_BUTTON_PLAYING" : "S_GMG_BUTTON_STOP");
				buttonState = FrontEndButton.State.Selected;
			}
			else if (ChallengeData.DidJoinInCurrentCycle(synchronizedTimeOrBestGuess))
			{
				text = Language.Get("S_GMG_BUTTON_RETRY");
			}
			subtitleText = ((!isJoined) ? string.Empty : SubtitleTextFromCurrentChallengeState);
			mShouldContextButtonBePressable = true;
			Challenge challengeInstance = ChallengeData.ChallengeInstance;
			if (challengeInstance == null)
			{
				long? bestScoreSubmittedThisCycle = ChallengeData.BestScoreSubmittedThisCycle;
				if (bestScoreSubmittedThisCycle.HasValue && ChallengeData.DidJoinInCurrentCycle(synchronizedTimeOrBestGuess))
				{
					string text3 = ((!ChallengeData.IsTime) ? bestScoreSubmittedThisCycle.Value.ToString() : TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value));
					text2 = Language.GetFormatString("S_GMG_BEST_SCORE", text3);
				}
				else
				{
					text2 = string.Empty;
				}
			}
			else
			{
				text2 = ChallengeData.ChallengeInstance.StatusPanelStatusText;
			}
			break;
		}
		case ChallengeStatus.Finished:
		{
			ulong friendsBeaten = ((challengeFriendData != null) ? challengeFriendData.FriendsRankedLower : 0);
			uint num2 = ChallengeData.GetRewardForMedal(challengeMedalType) + ChallengeData.GetRewardForFriendsBeaten(friendsBeaten);
			if (challengeFriendData != null && flag && num2 != 0 && !ChallengeData.HasPickedUpRewardSinceLastJoin)
			{
				text = Language.Get("S_GMG_STATE_COLLECT");
				subtitleText = string.Format("{0} {1}", CommonHelper.HardCurrencySymbol(), num2.ToString("n0"));
				mShouldContextButtonBePressable = true;
				mRewardAmount = num2;
				buttonState = FrontEndButton.State.Highlighted;
			}
			else
			{
				text = Language.Get((challengeFriendData != null) ? "S_GMG_STATE_CLOSED" : "S_GMG_STATE_UPDATING");
				subtitleText = null;
				buttonState = FrontEndButton.State.Disabled;
			}
			if (challengeFriendData == null)
			{
				text2 = string.Empty;
				break;
			}
			ChallengeLeaderboardRow[] rows = challengeFriendData.Rows;
			text2 = ((rows.Length <= 0) ? string.Empty : Language.GetFormatString("S_GMG_CHAMPION", rows[0].UserName));
			break;
		}
		case ChallengeStatus.Unknown:
			text = " - ";
			mShouldContextButtonBePressable = false;
			challengeMedalType = ChallengeMedalType.None;
			text2 = string.Empty;
			buttonState = FrontEndButton.State.Disabled;
			break;
		case ChallengeStatus.Invalid:
			text = Language.Get("S_GMG_STATE_CLOSED");
			mShouldContextButtonBePressable = false;
			text2 = string.Empty;
			challengeMedalType = ChallengeMedalType.None;
			buttonState = FrontEndButton.State.Disabled;
			break;
		}
		UpdateButton(text, subtitleText, buttonState);
		string[] array = text2.Split("\n".ToCharArray());
		UpdateText(array[0], (array.GetLength(0) <= 1) ? string.Empty : array[1], mFriendsPlayingString, mTimeUntilCloseString);
		UpdateMedalImage(challengeMedalType);
		UpdateSelectionBackground();
	}

	private IEnumerator UpdateToggledText()
	{
		float updateTime = 2f;
		while (true)
		{
			if (mFriendsPlayingString != "...")
			{
				if (mTextIsToggled)
				{
					TimeDisplay.Text = mTimeUntilCloseString;
				}
				else
				{
					TimeDisplay.Text = mFriendsPlayingString;
				}
				mTextIsToggled = !mTextIsToggled;
			}
			else
			{
				TimeDisplay.Text = mTimeUntilCloseString;
			}
			yield return new WaitForSeconds(updateTime);
		}
	}

	private void HandleChallengeStatusTextChanged(object sender, EventArgs e)
	{
		Challenge challenge = (Challenge)sender;
		if (mData != null && challenge.ChallengeId == mData.Id)
		{
			UpdateVisuals();
		}
	}

	private void HandleChallengeDataRewardCollected(object sender, EventArgs e)
	{
		UpdateVisuals();
	}

	private void HandleChallengeDataStatusChanged(object sender, EventArgs e)
	{
		UpdateVisuals();
	}

	private IEnumerator RevertFromStopButton()
	{
		yield return new WaitForSeconds(3f);
		if (mShowingStopButton)
		{
			mShowingStopButton = false;
			UpdateVisuals();
		}
	}

	private void HandleChallengeManagerUpdateComplete(object sender, ValueEventArgs<uint> e)
	{
		UpdateFromTimeChange(e.Value);
	}

	private void HandleChallengeContextButtonPressed()
	{
		if (!mShouldContextButtonBePressable)
		{
			return;
		}
		switch (ChallengeData.Status)
		{
		case ChallengeStatus.Open:
			if (ChallengeData.IsJoined)
			{
				if (mShowingStopButton)
				{
					Challenge challengeInstance = ChallengeData.ChallengeInstance;
					if (challengeInstance == null)
					{
						Debug.LogError("User pressed 'Stop' button while we were not joined... How?!");
						break;
					}
					ChallengeData.ChallengeInstance.AbortAttempt();
					mShowingStopButton = false;
					StopAllCoroutines();
				}
				else
				{
					mShowingStopButton = true;
					StartCoroutine(RevertFromStopButton());
					UpdateVisuals();
				}
			}
			else
			{
				BeginJoiningChallenge();
			}
			break;
		case ChallengeStatus.Finished:
			if (mRewardAmount.HasValue)
			{
				uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
				if (synchronizedTime.HasValue)
				{
					ChallengeData.CollectReward(synchronizedTime.Value, mRewardAmount.Value);
					break;
				}
				Debug.LogError("Tried to collect reward, but clock not synchronized.");
				NotificationPanel.Instance.Display(Language.Get("S_GMG_COLLECT_ERROR"));
			}
			else if (mChallengeInformation != null && !mChallengeInformation.IsActive)
			{
				mChallengeInformation.ShowChallenge(mData);
			}
			break;
		default:
			Debug.Log("Button pressed in undhandled state - " + ChallengeData.Status);
			break;
		}
	}

	private void BeginJoiningChallenge()
	{
		FinishJoiningChallenge();
	}

	private void FinishJoiningChallenge()
	{
		uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
		if (synchronizedTime.HasValue)
		{
			ChallengeData.Join(synchronizedTime.Value);
		}
		else
		{
			Debug.LogError("Clock was not synchronized when we attempted to join challenge. Shouldn't be possible.");
			NotificationPanel.Instance.Display(Language.Get("S_GMG_JOIN_ERROR") + ": " + Language.Get(ChallengeData.Name));
			UpdateButton(Language.Get("S_GMG_BUTTON_JOIN"), string.Empty, FrontEndButton.State.Normal);
		}
		UpdateVisuals();
	}

	private void UpdateFromTimeChange(uint currentTime)
	{
		if (!(ChallengeData == null))
		{
			uint remainingTime = ChallengeData.GetRemainingTime(currentTime);
			if (ChallengeData.GetStatusAtTime(currentTime) == ChallengeStatus.Open && remainingTime != 0)
			{
				mTimeUntilCloseString = Language.GetFormatString("S_GMG_CLOSES_IN", TimeUtils.GetFuzzyTimeStringFromSeconds(remainingTime));
				UpdateVisuals();
				return;
			}
			uint cycle = ChallengeData.GetCycle(currentTime);
			uint num = ChallengeData.ActiveDuration + ChallengeData.InactiveDuration + ChallengeData.InvalidDuration;
			uint num2 = ChallengeData.StartDate + cycle * num;
			uint secondsSinceUnixEpoch = num2 + ChallengeData.ActiveDuration;
			DateTime dateTime = TimeUtils.GetDateTimeFromUnixUtcTime(secondsSinceUnixEpoch).ToLocalTime();
			mTimeUntilCloseString = Language.GetFormatString("S_GMG_CLOSE_TIME", dateTime.Month, dateTime.Day);
		}
	}

	private void HandleChallengeLeaderboardDataCacheInvalidated(object sender, ChallengeEventArgs e)
	{
		if (e.Challenge.Equals(ChallengeData))
		{
			UpdateVisuals();
		}
	}

	private void HandleChallengeLeaderboardDataUpdated(object sender, ChallengeEventArgs e)
	{
		if (e.Challenge.Equals(ChallengeData))
		{
			UpdateVisuals();
		}
	}
}
