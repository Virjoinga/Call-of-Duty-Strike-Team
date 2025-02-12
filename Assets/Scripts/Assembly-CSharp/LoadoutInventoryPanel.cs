using UnityEngine;

public class LoadoutInventoryPanel : MonoBehaviour
{
	public int RowLength;

	public int NumActiveItems;

	private CommonBackgroundBoxPlacement[] mPlacements;

	private SpriteText[] mTexts;

	private EquipmentIconController[] mIcons;

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
		mPlacements = new CommonBackgroundBoxPlacement[num];
		mTexts = new SpriteText[num];
		mIcons = new EquipmentIconController[num];
		mLocked = new PackedSprite[num];
		mButtons = new UIButton[num];
		int num2 = num / RowLength;
		int num3 = 0;
		float num4 = 0f;
		float num5 = 0.3f;
		float num6 = 1f / (float)RowLength;
		float num7 = (1f - num5) / (float)num2;
		for (int i = 0; i < num; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child != null) || !child.name.Contains("PanelIcon"))
			{
				continue;
			}
			mButtons[i] = child.GetComponentInChildren<UIButton>();
			mPlacements[i] = child.GetComponent<CommonBackgroundBoxPlacement>();
			mPlacements[i].StartPositionAsPercentageOfBoxWidth = num4;
			mPlacements[i].StartPositionAsPercentageOfBoxHeight = num5;
			mPlacements[i].WidthAsPercentageOfBoxWidth = num6;
			mPlacements[i].HeightAsPercentageOfBoxHeight = num7;
			mTexts[i] = child.GetComponentInChildren<SpriteText>();
			mIcons[i] = child.GetComponentInChildren<EquipmentIconController>();
			PackedSprite[] componentsInChildren = child.GetComponentsInChildren<PackedSprite>();
			PackedSprite[] array = componentsInChildren;
			foreach (PackedSprite packedSprite in array)
			{
				if (packedSprite.name == "Locked")
				{
					mLocked[i] = packedSprite;
				}
			}
			if (mTexts[i] != null && mIcons[i] != null && mLocked[i] != null && mButtons[i] != null)
			{
				Color color = mButtons[i].color;
				color.a = ((i >= NumActiveItems) ? 0.4f : 1f);
				mButtons[i].SetColor(color);
				mButtons[i].scriptWithMethodToInvoke = this;
				mButtons[i].methodToInvoke = "OnButtonPress";
				mTexts[i].transform.localPosition = Vector3.zero;
				mIcons[i].transform.localPosition = Vector3.zero;
				mLocked[i].transform.localPosition = Vector3.zero;
				mButtons[i].transform.localPosition = Vector3.zero;
				CommonBackgroundBoxPlacement commonBackgroundBoxPlacement = mLocked[i].gameObject.AddComponent<CommonBackgroundBoxPlacement>();
				CommonBackgroundBoxPlacement commonBackgroundBoxPlacement2 = mTexts[i].gameObject.AddComponent<CommonBackgroundBoxPlacement>();
				CommonBackgroundBoxPlacement commonBackgroundBoxPlacement3 = mButtons[i].gameObject.AddComponent<CommonBackgroundBoxPlacement>();
				commonBackgroundBoxPlacement3.StartPositionAsPercentageOfBoxWidth = num4;
				commonBackgroundBoxPlacement3.StartPositionAsPercentageOfBoxHeight = num5;
				commonBackgroundBoxPlacement3.WidthAsPercentageOfBoxWidth = num6;
				commonBackgroundBoxPlacement3.HeightAsPercentageOfBoxHeight = num7;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxWidth = num4;
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight = num5;
				commonBackgroundBoxPlacement.WidthAsPercentageOfBoxWidth = num6;
				commonBackgroundBoxPlacement.HeightAsPercentageOfBoxHeight = num7;
				commonBackgroundBoxPlacement2.StartPositionAsPercentageOfBoxWidth = num4 + num6 * 0.05f;
				commonBackgroundBoxPlacement2.StartPositionAsPercentageOfBoxHeight = num5 + num7 * 0.1f;
				commonBackgroundBoxPlacement2.WidthAsPercentageOfBoxWidth = num6 * 0.9f;
				commonBackgroundBoxPlacement2.HeightAsPercentageOfBoxHeight = num7 * 0.9f;
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
		for (int i = 0; i < num; i++)
		{
			if (mIcons[i] != null && mTexts[i] != null)
			{
				if (instance.Equipment[i].Descriptor != null)
				{
					int numItems = instance.Equipment[i].NumItems;
					int slotSize = instance.Equipment[i].SlotSize;
					mIcons[i].SetEquipment(instance.Equipment[i].Descriptor.Type, numItems != 0);
					mTexts[i].Text = string.Format("{0}/{1}", numItems, slotSize);
					mIcons[i].Hide(false);
					mTexts[i].Hide(false);
				}
				else
				{
					mIcons[i].Hide(true);
					mTexts[i].Hide(true);
				}
			}
			bool tf = true;
			mLocked[i].Hide(tf);
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
			component.GoToEquipmentScreen(num);
		}
	}
}
