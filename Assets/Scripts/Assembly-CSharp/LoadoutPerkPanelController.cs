using UnityEngine;

public class LoadoutPerkPanelController : MonoBehaviour
{
	public int RowLength;

	public int NumActiveItems;

	private CommonBackgroundBoxPlacement[] mPlacements;

	private PerkIconController[] mIcons;

	private PackedSprite[] mLocked;

	private UIButton[] mButtons;

	private CommonBackgroundBox mBox;

	private int mHighlightedItem;

	public int HighlightedItem
	{
		get
		{
			return mHighlightedItem;
		}
		set
		{
			mHighlightedItem = value;
			Refresh();
		}
	}

	private void Awake()
	{
		mHighlightedItem = -1;
		Transform transform = base.transform.Find("Content");
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		int num = transform.childCount - 1;
		mIcons = new PerkIconController[num];
		mLocked = new PackedSprite[num];
		mButtons = new UIButton[num];
		mPlacements = new CommonBackgroundBoxPlacement[num];
		int num2 = num / RowLength;
		int num3 = 0;
		float num4 = 0f;
		float num5 = 0.3f;
		float num6 = 1f / (float)RowLength;
		float num7 = (1f - num5) / (float)num2;
		for (int i = 0; i < num; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child != null) || !child.name.Contains("Panel"))
			{
				continue;
			}
			mPlacements[i] = child.GetComponent<CommonBackgroundBoxPlacement>();
			mPlacements[i].StartPositionAsPercentageOfBoxWidth = num4;
			mPlacements[i].StartPositionAsPercentageOfBoxHeight = num5;
			mPlacements[i].WidthAsPercentageOfBoxWidth = num6;
			mPlacements[i].HeightAsPercentageOfBoxHeight = num7;
			mIcons[i] = child.GetComponentInChildren<PerkIconController>();
			mButtons[i] = child.GetComponentInChildren<UIButton>();
			PackedSprite[] componentsInChildren = child.GetComponentsInChildren<PackedSprite>();
			PackedSprite[] array = componentsInChildren;
			foreach (PackedSprite packedSprite in array)
			{
				if (packedSprite.name == "Locked")
				{
					mLocked[i] = packedSprite;
				}
			}
			if (mIcons[i] != null && mLocked[i] != null && mButtons[i] != null && mPlacements[i] != null)
			{
				Color color = mButtons[i].color;
				color.a = ((i >= NumActiveItems) ? 0.4f : 1f);
				mButtons[i].SetColor(color);
				mButtons[i].scriptWithMethodToInvoke = this;
				mButtons[i].methodToInvoke = "OnButtonPress";
				CommonBackgroundBoxPlacement commonBackgroundBoxPlacement = mLocked[i].gameObject.AddComponent<CommonBackgroundBoxPlacement>();
				CommonBackgroundBoxPlacement commonBackgroundBoxPlacement2 = mButtons[i].gameObject.AddComponent<CommonBackgroundBoxPlacement>();
				commonBackgroundBoxPlacement2.StartPositionAsPercentageOfBoxWidth = num4;
				commonBackgroundBoxPlacement2.StartPositionAsPercentageOfBoxHeight = num5;
				commonBackgroundBoxPlacement2.WidthAsPercentageOfBoxWidth = num6;
				commonBackgroundBoxPlacement2.HeightAsPercentageOfBoxHeight = num7;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxWidth = num4;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight = num5;
				commonBackgroundBoxPlacement.WidthAsPercentageOfBoxWidth = num6;
				commonBackgroundBoxPlacement.HeightAsPercentageOfBoxHeight = num7;
			}
			num4 += num6;
			if (++num3 == RowLength)
			{
				num4 = 0f;
				num5 += num7;
				num3 = 0;
			}
		}
	}

	public void Refresh()
	{
		GameSettings instance = GameSettings.Instance;
		int num = mPlacements.Length;
		bool flag = false;
		bool available = true;
		for (int i = 0; i < num; i++)
		{
			flag = false;
			if (instance.IsPerkSlotOccupied(i))
			{
				if (mIcons[i] != null)
				{
					PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(instance.Perks[i].Descriptor.Identifier);
					bool pro = instance.Perks[i].Descriptor.ProXPTarget <= perkStatus.ProXP || instance.WasProPerkUnlockedEarly(instance.Perks[i].Descriptor.Identifier);
					mIcons[i].SetPerk(instance.Perks[i].Descriptor.Identifier, pro, available);
					mIcons[i].Hide(false);
				}
				flag = true;
			}
			if (!flag && mIcons[i] != null)
			{
				mIcons[i].Hide(true);
			}
			if (mLocked[i] != null)
			{
				bool flag2 = instance.PerkSlotLocked(i);
				mLocked[i].Hide(!flag2);
			}
			if (mButtons[i] != null && mButtons[i].GetComponent<Collider>() != null)
			{
				mButtons[i].GetComponent<Collider>().enabled = !mButtons[i].GetComponent<Collider>().enabled;
				mButtons[i].GetComponent<Collider>().enabled = !mButtons[i].GetComponent<Collider>().enabled;
			}
		}
		if (mBox != null)
		{
			mBox.RefreshPlacements = true;
			mBox.Resize();
		}
		if (mHighlightedItem != -1)
		{
			Transform root = base.transform.root;
			AnimatedHighlight componentInChildren = root.GetComponentInChildren<AnimatedHighlight>();
			UIButton componentInChildren2 = mPlacements[mHighlightedItem].gameObject.GetComponentInChildren<UIButton>();
			if (componentInChildren != null && componentInChildren2 != null)
			{
				componentInChildren.HighlightSprite(componentInChildren2);
				MenuSFX.Instance.SelectToggle.Play2D();
			}
		}
	}

	public GameObject SetPerkHighlighted(int slot)
	{
		mHighlightedItem = slot;
		Refresh();
		return mPlacements[mHighlightedItem].gameObject;
	}

	public void ClearHighlights()
	{
		mHighlightedItem = -1;
		Transform root = base.transform.root;
		AnimatedHighlight componentInChildren = root.GetComponentInChildren<AnimatedHighlight>();
		if (componentInChildren != null)
		{
			componentInChildren.DismissHighlight();
		}
	}

	private void OnButtonPress()
	{
		int num = -1;
		for (int i = 0; i < mButtons.Length; i++)
		{
			if (mButtons[i].controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				num = i;
			}
		}
		LoadoutMenuNavigator component = base.transform.root.GetComponent<LoadoutMenuNavigator>();
		if (component != null && num != -1)
		{
			component.GoToLoadoutPerkMenu(num);
		}
	}
}
