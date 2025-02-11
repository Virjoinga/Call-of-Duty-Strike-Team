using System.Collections.Generic;
using UnityEngine;

public class PauseMenuMedalsController : MenuScreenBlade
{
	private const float ITEM_WIDTH_OFFSET = 8f;

	private const float TITLE_HEIGHT = 24f;

	private const float ITEM_HEIGHT = 42f;

	private const float ADD_ITEM_TIME = 0.05f;

	private const int NUM_ITEMS = 8;

	private const int MEDAL_TITLE_INDEX = 0;

	private const int INTEL_TITLE_INDEX = 6;

	private const int INTEL_ITEM_INDEX = 7;

	public UIListItemContainer TitlePrefab;

	public UIListItemContainer MedalItemPrefab;

	public UIListItemContainer IntelItemPrefab;

	private List<UIListItemContainer> mItemsToAdd;

	private MissionData mMissionData;

	private int mSection;

	private UIScrollList mList;

	private CommonBackgroundBox mBox;

	private float mTimeToAdd;

	private float mAddTime = 0.15f;

	private float mItemWidthOffset;

	private float mTitleHeight;

	private float mItemHeight;

	private int mNextItemToAdd;

	private bool mBegun;

	public override void Awake()
	{
		base.Awake();
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mItemWidthOffset = 8f * (float)num;
		mTitleHeight = 24f * (float)num;
		mItemHeight = 42f * (float)num;
		MissionListings instance = MissionListings.Instance;
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		mMissionData = instance.Mission(currentMissionID);
		mSection = ActStructure.Instance.CurrentSection;
		mList = GetComponentInChildren<UIScrollList>();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mItemsToAdd = new List<UIListItemContainer>();
		float width = 0f;
		if (mList != null && mBox != null)
		{
			mBox.Resize();
			mList.viewableArea = mBox.ForegroundSize;
			mList.transform.position = mBox.ForegroundCentre;
			width = mBox.GetWidth() - mItemWidthOffset;
		}
		CreateMedalListItems(width, mTitleHeight, mItemHeight);
		mBegun = false;
	}

	public void BeginMedalSequence(float timeToComplete)
	{
		mAddTime = timeToComplete / 8f;
		mBegun = true;
		mNextItemToAdd = 0;
		if (mList != null)
		{
			mList.ClearList(false);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!mBegun)
		{
			return;
		}
		mTimeToAdd += TimeManager.DeltaTime;
		if (mList != null && mTimeToAdd >= mAddTime && mNextItemToAdd < mItemsToAdd.Count)
		{
			if (mNextItemToAdd == 0 || mNextItemToAdd == 6)
			{
				AddTitleOption(mItemsToAdd[mNextItemToAdd]);
			}
			else if (mNextItemToAdd == 7)
			{
				AddIntelItem(mItemsToAdd[mNextItemToAdd]);
			}
			else
			{
				AddMedalItem(mItemsToAdd[mNextItemToAdd], mNextItemToAdd - 1);
			}
			mTimeToAdd = 0f;
			mNextItemToAdd++;
		}
	}

	private void CreateMedalListItems(float width, float titleHeight, float itemHeight)
	{
		UIListItemContainer uIListItemContainer = (UIListItemContainer)Object.Instantiate(TitlePrefab);
		uIListItemContainer.gameObject.name = "Title Item";
		PauseTitleItem component = uIListItemContainer.GetComponent<PauseTitleItem>();
		component.LayoutComponents("S_MEDALS", width, titleHeight);
		mItemsToAdd.Add(uIListItemContainer);
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			UIListItemContainer uIListItemContainer2 = (UIListItemContainer)Object.Instantiate(MedalItemPrefab);
			uIListItemContainer2.gameObject.name = "Medal Item - " + i;
			PauseMedalItem component2 = uIListItemContainer2.GetComponent<PauseMedalItem>();
			component2.LayoutComponents(width, itemHeight);
			mItemsToAdd.Add(uIListItemContainer2);
		}
		if (!ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			uIListItemContainer = (UIListItemContainer)Object.Instantiate(TitlePrefab);
			uIListItemContainer.gameObject.name = "Title Item";
			component = uIListItemContainer.GetComponent<PauseTitleItem>();
			component.LayoutComponents("S_INTEL", width, titleHeight);
			mItemsToAdd.Add(uIListItemContainer);
			UIListItemContainer uIListItemContainer3 = (UIListItemContainer)Object.Instantiate(IntelItemPrefab);
			uIListItemContainer3.gameObject.name = "Intel Item";
			PauseIntelItem component3 = uIListItemContainer3.GetComponent<PauseIntelItem>();
			component3.LayoutComponents(width, itemHeight);
			mItemsToAdd.Add(uIListItemContainer3);
		}
	}

	private void AddTitleOption(UIListItemContainer item)
	{
		mList.InsertItem(item, mList.Count, true);
	}

	private void AddMedalItem(UIListItemContainer item, int count)
	{
		PauseMedalItem component = item.GetComponent<PauseMedalItem>();
		component.Setup(count, mMissionData, mSection);
		mList.InsertItem(item, mList.Count, true);
	}

	private void AddIntelItem(UIListItemContainer item)
	{
		PauseIntelItem component = item.GetComponent<PauseIntelItem>();
		component.Setup(mMissionData);
		mList.InsertItem(item, mList.Count, true);
	}
}
