using UnityEngine;

public class ToggleEditOption : MonoBehaviour
{
	public Color SelectedColour;

	public Color UnselectedColour;

	public Color UnModifiableColour;

	public string SelectedKey;

	public string UnselectedKey;

	public SpriteText LabelTextItem;

	public MonoBehaviour ScriptWithMethodToInvoke;

	public string MethodToInvokeOnChange;

	private UIButton mButton;

	private FrontEndButton mFeButton;

	private SpriteText mButtonText;

	private CommonBackgroundBoxPlacement mBackgroundBox;

	private string mSelectedText;

	private string mUnselectedText;

	private bool mOn;

	private bool mModifiable;

	private bool mDirty = true;

	private string mToolTip = string.Empty;

	public bool Value
	{
		get
		{
			return mOn;
		}
		set
		{
			mOn = value;
			Color color = ((!mModifiable) ? UnModifiableColour : ((!mOn) ? UnselectedColour : SelectedColour));
			mButtonText.SetColor(color);
			mDirty = true;
		}
	}

	public string ToolTip
	{
		set
		{
			mToolTip = value;
		}
	}

	private void Awake()
	{
		mButton = GetComponentInChildren<UIButton>();
		mFeButton = GetComponentInChildren<FrontEndButton>();
		mButtonText = mButton.GetComponentInChildren<SpriteText>();
		mBackgroundBox = GetComponentInChildren<CommonBackgroundBoxPlacement>();
	}

	private void Start()
	{
		AnimateButtonOnInvoke componentInChildren = GetComponentInChildren<AnimateButtonOnInvoke>();
		mSelectedText = AutoLocalize.Get(SelectedKey);
		mUnselectedText = AutoLocalize.Get(UnselectedKey);
		mModifiable = true;
		mButton.scriptWithMethodToInvoke = this;
		mButton.methodToInvoke = "ButtonClicked";
		if (componentInChildren != null)
		{
			componentInChildren.ReplaceButtonScript();
		}
		mDirty = true;
	}

	public void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	public void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	private void Update()
	{
		if (mDirty)
		{
			mButtonText.Text = ((!mOn) ? mUnselectedText : mSelectedText);
			mDirty = false;
			if (ScriptWithMethodToInvoke != null && MethodToInvokeOnChange != null)
			{
				ScriptWithMethodToInvoke.Invoke(MethodToInvokeOnChange, 0f);
			}
		}
	}

	private void ButtonClicked()
	{
		if (mModifiable)
		{
			Value = !Value;
			mDirty = true;
		}
	}

	public void SetModifiable(bool bOn)
	{
		mModifiable = bOn;
		mButton.controlIsEnabled = bOn;
		mDirty = true;
		Color color = ((!mModifiable) ? UnModifiableColour : ((!mOn) ? UnselectedColour : SelectedColour));
		mButtonText.SetColor(color);
		mFeButton.CurrentState = ((!mModifiable) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		LabelTextItem.SetColor((!mModifiable) ? new Color(0.25f, 0.25f, 0.25f, 1f) : new Color(1f, 1f, 1f, 1f));
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		CheckTip(fingerPos);
	}

	private void CheckTip(Vector2 fingerPos)
	{
		if (mBackgroundBox.BoundingRect.Contains(fingerPos) && !string.IsNullOrEmpty(mToolTip))
		{
			ToolTipController.Instance.DoTooltip(mToolTip, mBackgroundBox.gameObject);
		}
	}
}
