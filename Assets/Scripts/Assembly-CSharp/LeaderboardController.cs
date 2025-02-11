using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MenuScreenBlade
{
	public class Panel
	{
		public string rank;

		public string title;

		public string score;

		public int level;

		public bool elite;

		public bool veteran;

		public bool player;

		public void Blank()
		{
			title = string.Empty;
			elite = false;
			veteran = false;
			level = 1;
			rank = string.Empty;
			score = string.Empty;
			player = false;
		}
	}

	private const float TIME_BETWEEN_ADDS = 0.1f;

	private const float ITEM_WIDTH_OFFSET = 8f;

	private const float ITEM_HEIGHT = 50f;

	public LeaderboardPanel Prefab;

	public LeaderboardPanel CurrentPositionA;

	public LeaderboardPanel CurrentPositionB;

	public LeaderboardPanel CurrentPositionC;

	public GameObject NotSignedInMessage;

	private List<UIListItemContainer> mPanelsToAdd;

	private CommonBackgroundBox mBox;

	private AnimateCommonBackgroundBox mAnimation;

	private UIScrollList mLeaderboardScrollList;

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
		mItemWidthOffset = 8f * (float)num;
		mHeight = 50f * (float)num;
		mPanelsToAdd = new List<UIListItemContainer>();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mAnimation = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mLeaderboardScrollList = GetComponentInChildren<UIScrollList>();
		if (mBox != null)
		{
			mBox.Resize();
			mWidth = mBox.GetWidth() - mItemWidthOffset;
		}
		if (TBFUtils.UseAlternativeLayout())
		{
			AdjustCommonBackgroundBoxPlacementComponentsForAlternateLayout();
		}
	}

	protected override void OnActivate()
	{
		if (NotSignedInMessage != null)
		{
			bool flag = Application.isEditor || Bedrock.getUserConnectionStatus().IsRegistered();
			NotSignedInMessage.SetActive(!flag);
		}
		base.OnActivate();
	}

	private void AdjustCommonBackgroundBoxPlacementComponentsForAlternateLayout()
	{
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in componentsInChildren)
		{
			if (commonBackgroundBoxPlacement.name == "ScrollMenuPlacement")
			{
				commonBackgroundBoxPlacement.HeightAsPercentageOfBoxHeight = 0.6f;
			}
			else if (commonBackgroundBoxPlacement.name == "CurrentPositionTitle")
			{
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight = 0.69f;
			}
			else if (commonBackgroundBoxPlacement.name == "Marker")
			{
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight = 0.66f;
			}
			else if (commonBackgroundBoxPlacement.name == "RankNo")
			{
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight *= 1.7f;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight -= 0.68f;
			}
			else if (commonBackgroundBoxPlacement.name == "Score")
			{
				SpriteText componentInChildren = commonBackgroundBoxPlacement.gameObject.GetComponentInChildren<SpriteText>();
				if (componentInChildren != null)
				{
					componentInChildren.multiline = false;
				}
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxWidth = 0.8f;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight *= 1.7f;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight -= 0.68f;
				commonBackgroundBoxPlacement.WidthAsPercentageOfBoxWidth *= 1.1f;
			}
		}
	}

	public void AddPanels(Panel[] panels, bool showVeteranIcon)
	{
		foreach (Panel panel in panels)
		{
			LeaderboardPanel leaderboardPanel = (LeaderboardPanel)Object.Instantiate(Prefab);
			leaderboardPanel.SetData(panel.rank, panel.title, panel.score, panel.level, panel.elite, panel.veteran, panel.player, showVeteranIcon);
			leaderboardPanel.LayoutComponents(mWidth, mHeight);
			UIListItemContainer component = leaderboardPanel.GetComponent<UIListItemContainer>();
			mPanelsToAdd.Add(component);
		}
	}

	public void UpdateCurrentPosition(Panel[] panels, bool showVeteranIcon)
	{
		if (panels.Length != 3)
		{
			Debug.LogWarning("Unexpected number of panels ( " + panels.Length + " ) passed into UpdateCurrentPosition expected number is " + 3);
		}
		for (int i = 0; i < 3 && i < panels.Length; i++)
		{
			UpdateCurrentPositionPanel(i, panels[i], showVeteranIcon);
		}
	}

	public void Clear()
	{
		foreach (UIListItemContainer item in mPanelsToAdd)
		{
			Object.Destroy(item.gameObject);
		}
		mPanelsToAdd.Clear();
		mLeaderboardScrollList.ClearList(true);
	}

	public override void Update()
	{
		base.Update();
		if (mAnimation != null && mAnimation.IsOpen)
		{
			mTimeSinceLastAdd += TimeManager.DeltaTime;
			if (mTimeSinceLastAdd >= 0.1f && mPanelsToAdd.Count > 0)
			{
				mLeaderboardScrollList.InsertItem(mPanelsToAdd[0], mLeaderboardScrollList.Count);
				mPanelsToAdd.RemoveAt(0);
				mTimeSinceLastAdd = 0f;
			}
		}
	}

	private void UpdateCurrentPositionPanel(int panelNo, Panel panel, bool showVeteranIcon)
	{
		if (panel != null)
		{
			LeaderboardPanel leaderboardPanel = null;
			if (CurrentPositionA != null && panelNo == 0)
			{
				leaderboardPanel = CurrentPositionA;
			}
			else if (CurrentPositionB != null && panelNo == 1)
			{
				leaderboardPanel = CurrentPositionB;
			}
			else if (CurrentPositionC != null && panelNo == 2)
			{
				leaderboardPanel = CurrentPositionC;
			}
			if (leaderboardPanel != null)
			{
				leaderboardPanel.SetData(panel.rank, panel.title, panel.score, panel.level, panel.elite, panel.veteran, panel.player, showVeteranIcon);
				leaderboardPanel.PlaceDynamic(mWidth, mHeight);
			}
		}
	}
}
