using UnityEngine;

public class LoadoutArmourPanel : MonoBehaviour
{
	private EquipmentIconController mIcon;

	private CommonBackgroundBox mBox;

	private bool mShowHighlight;

	public bool ShowHighlight
	{
		get
		{
			return mShowHighlight;
		}
		set
		{
			mShowHighlight = value;
			Refresh();
		}
	}

	private void Awake()
	{
		mShowHighlight = false;
		Transform transform = base.transform.FindChild("Content");
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child != null) || !child.name.Contains("PanelIcon"))
			{
				continue;
			}
			mIcon = child.GetComponentInChildren<EquipmentIconController>();
			SpriteText componentInChildren = child.GetComponentInChildren<SpriteText>();
			componentInChildren.Hide(true);
			PackedSprite[] componentsInChildren = child.GetComponentsInChildren<PackedSprite>();
			PackedSprite[] array = componentsInChildren;
			foreach (PackedSprite packedSprite in array)
			{
				if (packedSprite.name == "Locked")
				{
					packedSprite.Hide(true);
				}
			}
		}
	}

	public void Refresh()
	{
		GameSettings instance = GameSettings.Instance;
		if (mIcon != null)
		{
			if (instance.Armour != null)
			{
				mIcon.SetEquipment(instance.Armour.Type, true);
			}
			else
			{
				mIcon.SetEquipment(EquipmentIconController.EquipmentType.ArmourLevel1, false);
			}
		}
		if (mBox != null)
		{
			mBox.RefreshPlacements = true;
			mBox.Resize();
		}
		Transform root = base.transform.root;
		AnimatedHighlight componentInChildren = root.GetComponentInChildren<AnimatedHighlight>();
		if (!(componentInChildren != null))
		{
			return;
		}
		if (mShowHighlight)
		{
			SpriteRoot componentInChildren2 = mIcon.gameObject.GetComponentInChildren<SpriteRoot>();
			if (componentInChildren != null && componentInChildren2 != null)
			{
				componentInChildren.HighlightSprite(componentInChildren2);
				MenuSFX.Instance.SelectToggle.Play2D();
			}
		}
		else
		{
			componentInChildren.DismissHighlight();
		}
	}
}
