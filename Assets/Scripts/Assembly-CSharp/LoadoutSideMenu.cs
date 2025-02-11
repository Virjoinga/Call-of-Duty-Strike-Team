using UnityEngine;

public class LoadoutSideMenu : MonoBehaviour
{
	public SideMenuOption[] MenuOptions;

	public MonoBehaviour ScriptWithMethodToInvoke;

	public Color SpriteSelectedColour;

	public Color SpriteNormalColour;

	public Color TextSelectedColour;

	public Color TextNormalColour;

	public Vector2 ButtonOffset;

	public float ButtonSpacing;

	private AnimateCommonBackgroundBox mAnimator;

	private CommonBackgroundBox mBox;

	private int mCurrentlySelected;

	private int mTargetSelected;

	private bool mStateDirty;

	private void Awake()
	{
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		mAnimator = GetComponentInChildren<AnimateCommonBackgroundBox>();
		int num = MenuOptions.Length;
		Vector2 buttonOffset = ButtonOffset;
		for (int i = 0; i < num; i++)
		{
			UIButton uIButton = MenuOptions[i].Button;
			if (uIButton != null)
			{
				UIButton smallButton = MenuOptions[i].SmallButton;
				if (smallButton != null)
				{
					if (TBFUtils.UseAlternativeLayout())
					{
						uIButton.gameObject.SetActive(false);
						MenuOptions[i].Button = smallButton;
						uIButton = smallButton;
					}
					else
					{
						smallButton.gameObject.SetActive(false);
					}
				}
				FrontEndButton component = uIButton.GetComponent<FrontEndButton>();
				MenuOptions[i].ButtonState = component;
				MenuOptions[i].TooltipText = string.Empty;
				MenuOptions[i].Disabled = false;
				uIButton.scriptWithMethodToInvoke = ScriptWithMethodToInvoke;
				uIButton.methodToInvoke = MenuOptions[i].MethodToInvoke;
				uIButton.transform.localPosition = Vector3.zero;
				if (uIButton.spriteText != null)
				{
					uIButton.spriteText.Text = AutoLocalize.Get(MenuOptions[i].TextKey);
				}
				EZScreenPlacement component2 = uIButton.transform.parent.GetComponent<EZScreenPlacement>();
				if (component2 != null && mBox != null)
				{
					component2.screenPos.x = buttonOffset.x;
					component2.screenPos.y = buttonOffset.y;
					component2.screenPos.z = -1f;
					component2.relativeTo.horizontal = EZScreenPlacement.HORIZONTAL_ALIGN.OBJECT;
					component2.relativeTo.vertical = EZScreenPlacement.VERTICAL_ALIGN.OBJECT;
					component2.relativeObject = mBox.transform;
				}
				buttonOffset.y -= ButtonSpacing;
			}
			else
			{
				Debug.LogWarning("Unable to update item for side menu as menu option " + i + " doesn't contain a button");
			}
			mTargetSelected = 0;
			mStateDirty = false;
		}
	}

	public int FindPressed()
	{
		int result = -1;
		for (int i = 0; i < MenuOptions.Length; i++)
		{
			UIButton button = MenuOptions[i].Button;
			if (button.controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public void SetSelected(int index)
	{
		mTargetSelected = index;
		mStateDirty = true;
	}

	public void SetDisabled(bool disabled, int index, string reason)
	{
		if (index >= 0 && index < MenuOptions.Length && MenuOptions[index].Disabled != disabled)
		{
			MenuOptions[index].TooltipText = ((!disabled) ? string.Empty : reason);
			MenuOptions[index].Disabled = disabled;
			mStateDirty = true;
		}
	}

	private void UpdateStateNow(int index)
	{
		if (index < MenuOptions.Length)
		{
			int num = MenuOptions.Length;
			for (int i = 0; i < num; i++)
			{
				FrontEndButton buttonState = MenuOptions[i].ButtonState;
				bool disabled = MenuOptions[i].Disabled;
				bool flag = i == index;
				if (buttonState != null)
				{
					if (disabled)
					{
						buttonState.CurrentState = FrontEndButton.State.Disabled;
						buttonState.ToolTip = MenuOptions[i].TooltipText;
					}
					else if (flag)
					{
						buttonState.CurrentState = FrontEndButton.State.Selected;
						buttonState.ToolTip = string.Empty;
					}
					else
					{
						buttonState.CurrentState = FrontEndButton.State.Normal;
						buttonState.ToolTip = string.Empty;
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("Set selected called out of bounds on " + base.name);
		}
		mStateDirty = false;
	}

	private void Update()
	{
		if (mAnimator != null && mAnimator.IsOpen && mStateDirty)
		{
			UpdateStateNow(mTargetSelected);
		}
	}

	public void HideButton(int index, bool hide)
	{
		if (index >= 0 && index < MenuOptions.Length)
		{
			MenuOptions[index].Button.gameObject.SetActive(!hide);
		}
	}
}
