using System.Collections.Generic;
using UnityEngine;

public class StatisticsController : MenuScreenBlade
{
	public class Panel
	{
		public string title;

		public string[] stats;

		public string[] values;

		public StatisticsPanel.Type type;
	}

	private const float TIME_BETWEEN_ADDS = 0.1f;

	private const float ITEM_WIDTH_OFFSET = 16f;

	private const float ITEM_HEIGHT = 100f;

	public StatisticsPanel Prefab;

	private List<UIListItemContainer> mPanelsToAdd;

	private CommonBackgroundBox mBox;

	private AnimateCommonBackgroundBox mAnimation;

	private UIScrollList mStatsScrollList;

	private float mTimeSinceLastAdd;

	private float mItemWidthOffset;

	private float mWidth;

	private float mHeight;

	public override void Awake()
	{
		base.Awake();
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mItemWidthOffset = 16f * (float)num;
		mHeight = 100f * (float)num;
		mPanelsToAdd = new List<UIListItemContainer>();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mAnimation = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mStatsScrollList = GetComponentInChildren<UIScrollList>();
		if (mBox != null && mStatsScrollList != null)
		{
			mBox.Resize();
			mStatsScrollList.viewableArea = mBox.ForegroundSize;
			mStatsScrollList.transform.position = mBox.ForegroundCentre;
			mWidth = mBox.GetWidth() - mItemWidthOffset;
			SoundManager.Instance.SetUIScrollListScrollSFX(mStatsScrollList);
		}
	}

	public void AddPanels(Panel[] panels)
	{
		foreach (Panel panel in panels)
		{
			StatisticsPanel statisticsPanel = (StatisticsPanel)Object.Instantiate(Prefab);
			statisticsPanel.SetData(panel.title, panel.stats, panel.values, panel.type);
			statisticsPanel.LayoutComponents(mWidth, mHeight);
			UIListItemContainer component = statisticsPanel.GetComponent<UIListItemContainer>();
			mPanelsToAdd.Add(component);
		}
	}

	public void Clear()
	{
		foreach (UIListItemContainer item in mPanelsToAdd)
		{
			Object.Destroy(item.gameObject);
		}
		mPanelsToAdd.Clear();
		mStatsScrollList.ClearList(true);
	}

	public override void Update()
	{
		base.Update();
		if (mAnimation != null && mAnimation.IsOpen)
		{
			mTimeSinceLastAdd += TimeManager.DeltaTime;
			if (mTimeSinceLastAdd >= 0.1f && mPanelsToAdd.Count > 0)
			{
				mStatsScrollList.InsertItem(mPanelsToAdd[0], mStatsScrollList.Count);
				mPanelsToAdd.RemoveAt(0);
				mTimeSinceLastAdd = 0f;
			}
		}
	}
}
