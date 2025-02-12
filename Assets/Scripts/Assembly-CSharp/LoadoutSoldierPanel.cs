using UnityEngine;

public class LoadoutSoldierPanel : MonoBehaviour
{
	public SpriteText TitleBar;

	public SpriteText WeaponName;

	public SpriteText SilencedText;

	public WeaponIconController WeaponImage;

	private AnimatedHighlight mHighlight;

	private LoadoutMenuNavigator MenuNavigator;

	private FrontEndButton mButton;

	private PackedSprite[] SoldierImages;

	private float mPixelSize;

	private int mSoldierIndex;

	private bool mPresent;

	public void Awake()
	{
		Transform transform = base.transform.Find("Content");
		Transform transform2 = transform.Find("SoldierBox");
		SoldierImages = transform2.GetComponentsInChildren<PackedSprite>();
		mButton = transform.GetComponentInChildren<FrontEndButton>();
		Transform root = base.transform.root;
		mHighlight = root.GetComponentInChildren<AnimatedHighlight>();
		MenuNavigator = Object.FindObjectOfType(typeof(LoadoutMenuNavigator)) as LoadoutMenuNavigator;
		if (TBFUtils.UseAlternativeLayout() && WeaponImage != null)
		{
			CommonBackgroundBoxPlacement[] componentsInChildren = transform.GetComponentsInChildren<CommonBackgroundBoxPlacement>();
			CommonBackgroundBoxPlacement[] array = componentsInChildren;
			foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
			{
				commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxWidth = 0f;
				commonBackgroundBoxPlacement.WidthAsPercentageOfBoxWidth = 1f;
			}
		}
		mPresent = false;
	}

	public void SetWeaponHighlighted()
	{
		if (mHighlight != null && WeaponImage != null && WeaponImage.Sprite != null)
		{
			mHighlight.HighlightSprite(WeaponImage.Sprite);
		}
	}

	public void ClearHighlights()
	{
		if (mHighlight != null)
		{
			mHighlight.DismissHighlight();
		}
	}

	public void Setup(int index, SoldierSettings soldier, MissionData mission, bool auto)
	{
		mPresent = soldier.Present;
		mSoldierIndex = index;
		TitleBar.Text = soldier.Name;
		WeaponName.Text = ((!(soldier.Weapon.Descriptor != null)) ? string.Empty : soldier.Weapon.Descriptor.Name);
		TitleBar.SetColor((!mPresent) ? ColourChart.GreyedOut : Color.white);
		WeaponName.SetColor((!mPresent) ? ColourChart.GreyedOut : Color.white);
		if (WeaponName.transform.parent != null)
		{
			SubtitleBackground component = WeaponName.transform.parent.GetComponent<SubtitleBackground>();
			if (component != null)
			{
				component.Resize();
			}
		}
		if (WeaponImage != null)
		{
			WeaponImage.SetWeapon(soldier.Weapon.Descriptor.Type, mPresent && !auto);
			WeaponImage.Sprite.SetColor((!mPresent) ? ColourChart.GreyedOut : Color.white);
		}
		if (SilencedText != null)
		{
			bool flag = WeaponUtils.IsWeaponSilenced(soldier.Weapon.Descriptor);
			SilencedText.Hide(!flag);
			SilencedText.SetColor((!mPresent) ? ColourChart.GreyedOut : ColourChart.HudYellow);
		}
		MissionData.eEnvironment soldierImage = MissionData.eEnvironment.Arctic;
		if (mission != null)
		{
			soldierImage = mission.Environment;
		}
		SetSoldierImage(soldierImage);
		CommonBackgroundBox componentInChildren = GetComponentInChildren<CommonBackgroundBox>();
		if (componentInChildren != null)
		{
			componentInChildren.RefreshPlacements = true;
			componentInChildren.Resize();
			SimpleSprite[] componentsInChildren = componentInChildren.GetComponentsInChildren<SimpleSprite>();
			SimpleSprite[] array = componentsInChildren;
			foreach (SimpleSprite simpleSprite in array)
			{
				simpleSprite.SetColor((!mPresent) ? Color.gray : Color.white);
			}
		}
		if (mButton != null)
		{
			mButton.OverrideDisabledColour(new Color(0.1f, 0.1f, 0.1f, 1f));
			mButton.CurrentState = ((!mPresent) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
	}

	private void SetSoldierImage(MissionData.eEnvironment environment)
	{
		bool flag = TBFUtils.UseAlternativeLayout();
		PackedSprite[] soldierImages = SoldierImages;
		foreach (PackedSprite packedSprite in soldierImages)
		{
			bool flag2 = !flag && packedSprite.name.Contains(environment.ToString()) && packedSprite.name.Contains(mSoldierIndex.ToString());
			packedSprite.gameObject.SetActive(flag2);
			packedSprite.transform.localPosition = Vector3.zero;
			if (flag2)
			{
				packedSprite.SetColor((!mPresent) ? ColourChart.GreyedOut : Color.white);
			}
		}
	}

	public void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	public void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		CommonBackgroundBox componentInChildren = GetComponentInChildren<CommonBackgroundBox>();
		if (!CommonHelper.CreateRect(componentInChildren).Contains(fingerPos) || !(MenuNavigator != null))
		{
			return;
		}
		bool flag = MissionSetup.Instance == null || !MissionSetup.Instance.LockWeaponSelection;
		if (mPresent && flag)
		{
			MenuNavigator.GoToLoadoutSoldierMenu(mSoldierIndex);
			if (mButton != null)
			{
				mButton.Activate();
			}
		}
		else
		{
			string text = Language.Get(flag ? "S_SOLDIER_NOT_PRESENT" : "S_WEAPON_SELECTION_AUTO");
			ToolTipController.Instance.DoTooltip(text, WeaponImage.gameObject);
		}
	}
}
