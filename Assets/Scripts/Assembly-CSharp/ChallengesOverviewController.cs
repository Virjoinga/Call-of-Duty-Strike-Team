using System.Collections.Generic;

public class ChallengesOverviewController : MenuScreenBlade
{
	private const float CHALLENGE_DETAIL_DELAY = 0.1f;

	private const float TIME_BETWEEN_ADDS = 0.1f;

	private const float ITEM_HEIGHT = 210f;

	private const float SCROLL_TIME = 0.5f;

	public ChallengeInformationController ChallengeDetail;

	public SpriteText StatusMessageText;

	public ActivateButton ActivateButtonControl;

	private List<ChallengeListItem> mCurrentPanels;

	private List<ChallengeListItem> mRemovedPanels;

	private CommonBackgroundBox mBox;

	private AnimateCommonBackgroundBox mAnimation;

	private UIScrollList mChallengeScrollList;

	private float mTimeSinceLastAdd;

	private float mWidth;

	private float mHeight;

	private float mPixelSize;

	private bool mChallengesEnabled;

	public int CurrentItemCount
	{
		get
		{
			return mCurrentPanels.Count;
		}
	}

	public void SetChallengesEnabled()
	{
		mChallengesEnabled = true;
	}

	public override void Awake()
	{
		base.Awake();
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mHeight = 210f * (float)num;
		mPixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		mCurrentPanels = new List<ChallengeListItem>();
		mRemovedPanels = new List<ChallengeListItem>();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mAnimation = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mChallengeScrollList = GetComponentInChildren<UIScrollList>();
		if (mBox != null && mChallengeScrollList != null)
		{
			mBox.Resize();
			mChallengeScrollList.viewableArea = mBox.ForegroundSize;
			mChallengeScrollList.transform.position = mBox.ForegroundCentre;
			mWidth = mBox.ForegroundSize.x;
			SoundManager.Instance.SetUIScrollListScrollSFX(mChallengeScrollList);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!(mAnimation != null) || !(mChallengeScrollList != null))
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
				bool flag = mChallengeScrollList.Contains(mCurrentPanels[i].Container);
				if (!flag && isOpen)
				{
					mChallengeScrollList.InsertItem(mCurrentPanels[i].Container, i);
					mTimeSinceLastAdd = 0f;
					break;
				}
				if (flag && !isOpen)
				{
					mChallengeScrollList.RemoveItem(mCurrentPanels[i].Container, false);
					break;
				}
			}
		}
		if (mRemovedPanels.Count > 0)
		{
			mChallengeScrollList.RemoveItem(mRemovedPanels[0].Container, false);
			mRemovedPanels.RemoveAt(0);
		}
	}

	public void HideStatus()
	{
		if (StatusMessageText != null)
		{
			StatusMessageText.Hide(true);
		}
		if (ActivateButtonControl != null)
		{
			ActivateButtonControl.gameObject.SetActive(false);
		}
	}

	public void SetStatus(string statusText, bool buttonActive)
	{
		if (StatusMessageText != null)
		{
			StatusMessageText.Hide(false);
			StatusMessageText.Text = statusText.ToUpper();
		}
		if (ActivateButtonControl != null)
		{
			ActivateButtonControl.gameObject.SetActive(buttonActive);
			ActivateButtonControl.CurrentState = ActivateButton.State.Pulse;
			UIButton component = ActivateButtonControl.GetComponent<UIButton>();
			if (component != null)
			{
				component.scriptWithMethodToInvoke = TitleBarController.Instance;
				component.methodToInvoke = "OnActivateButtonPressed";
			}
		}
	}

	protected override void OnActivate()
	{
		if (mChallengesEnabled)
		{
			base.OnActivate();
			if (ChallengeDetail != null && ChallengeDetail.HasData && ChallengeManager.ConnectionStatus.IsOnline())
			{
				ChallengeDetail.DelayedActivate(0.1f);
			}
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (ChallengeDetail != null)
		{
			ChallengeDetail.SetDisabling();
			ChallengeDetail.DelayedDeactivate(0.1f);
		}
	}

	public void InsertItem(ChallengeListItem item, uint currentTime)
	{
		ChallengeOverviewItem overviewItem = item.OverviewItem;
		int index = FindInsertPoint(item, currentTime);
		if (mCurrentPanels == null || !(overviewItem != null))
		{
			return;
		}
		if (ChallengeDetail != null)
		{
			overviewItem.ChallengeInformation = ChallengeDetail;
			if (!ChallengeDetail.HasData)
			{
				ChallengeDetail.ShowChallenge(overviewItem.ChallengeData);
			}
		}
		overviewItem.LayoutComponents(mWidth, mHeight, mPixelSize);
		mCurrentPanels.Insert(index, item);
	}

	public void RemoveItem(ChallengeListItem item)
	{
		if (mCurrentPanels != null)
		{
			mCurrentPanels.Remove(item);
			mRemovedPanels.Add(item);
		}
	}

	public void ClearList(bool destroy)
	{
		if (mCurrentPanels != null)
		{
			mCurrentPanels.Clear();
			mChallengeScrollList.ClearList(false);
		}
	}

	private void RefreshSelected()
	{
		ChallengeData challengeData = null;
		if (ChallengeDetail != null)
		{
			challengeData = ChallengeDetail.ActiveChallenge;
		}
		if (!(mChallengeScrollList != null) || mCurrentPanels == null || !(challengeData != null))
		{
			return;
		}
		for (int i = 0; i < mCurrentPanels.Count; i++)
		{
			ChallengeListItem challengeListItem = mCurrentPanels[i];
			if (challengeListItem != null && challengeData == challengeListItem.OverviewItem.ChallengeData && i < mChallengeScrollList.Count)
			{
				mChallengeScrollList.ScrollToItem(i, 0.5f);
			}
		}
	}

	private int FindInsertPoint(ChallengeListItem itemToInsert, uint currentTime)
	{
		int result = 0;
		if (mCurrentPanels != null)
		{
			result = mCurrentPanels.Count;
			ChallengeData challengeData = itemToInsert.OverviewItem.ChallengeData;
			for (int i = 0; i < mCurrentPanels.Count; i++)
			{
				ChallengeListItem challengeListItem = mCurrentPanels[i];
				if (challengeListItem != null)
				{
					ChallengeData challengeData2 = challengeListItem.OverviewItem.ChallengeData;
					int num = challengeData.CompareTo(challengeData2, currentTime);
					if (num <= 0)
					{
						result = i;
						break;
					}
				}
			}
		}
		return result;
	}
}
