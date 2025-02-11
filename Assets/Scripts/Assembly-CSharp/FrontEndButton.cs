using UnityEngine;

public class FrontEndButton : MonoBehaviour
{
	public enum State
	{
		Normal = 0,
		Active = 1,
		Selected = 2,
		Disabled = 3,
		Highlighted = 4
	}

	private const float HIGHLIGHT_TIME = 0.4f;

	private const float BREAK_TIME = 1.5f;

	private const float TIME_TO_STAY_ACTIVE = 0.2f;

	private const int TIME_BETWEEN_HIGHLIGHTS = 1;

	public GameObject HighlightSpriteChildren;

	public bool Highlight;

	public bool HighlightWhileMessageBoxActive = true;

	private UIButton mButton;

	private PackedSprite mSprite;

	private MonoBehaviour mScriptWithMethodToInvoke;

	private string mMethodToInvoke = string.Empty;

	private float mTimeStateChanged;

	private State mCurrentState;

	private State mReturnState;

	private int mHighlights;

	private Color mDisabledColour;

	public string Text
	{
		get
		{
			if (mButton != null)
			{
				return mButton.Text;
			}
			return string.Empty;
		}
		set
		{
			if (mButton != null)
			{
				mButton.Text = value;
			}
		}
	}

	public State CurrentState
	{
		get
		{
			return mCurrentState;
		}
		set
		{
			if (mCurrentState != value && value != State.Active)
			{
				if (mCurrentState != State.Active)
				{
					mCurrentState = value;
					RefreshButtonColour();
					mTimeStateChanged = Time.realtimeSinceStartup;
				}
				else
				{
					mReturnState = value;
				}
			}
		}
	}

	public string ToolTip { get; set; }

	private void Awake()
	{
		mButton = base.gameObject.GetComponentInChildren<UIButton>();
		if (HighlightSpriteChildren != null)
		{
			mSprite = HighlightSpriteChildren.GetComponentInChildren<PackedSprite>();
		}
		ReplaceButtonScript();
		mCurrentState = (Highlight ? State.Highlighted : State.Normal);
		RefreshButtonColour();
		mHighlights = 0;
		mDisabledColour = ColourChart.FrontEndButtonDisabled;
	}

	private void Start()
	{
		ReplaceButtonScript();
	}

	private void Update()
	{
		ReplaceButtonScript();
		if (CurrentState == State.Active)
		{
			if (Time.realtimeSinceStartup - mTimeStateChanged > 0.2f)
			{
				mCurrentState = mReturnState;
				RefreshButtonColour();
				mTimeStateChanged = Time.realtimeSinceStartup;
			}
		}
		else
		{
			if (CurrentState != State.Highlighted)
			{
				return;
			}
			float num = Time.realtimeSinceStartup - mTimeStateChanged;
			if (num >= 0.4f && mHighlights < 3)
			{
				if (mButton != null)
				{
					mButton.gameObject.renderer.material.color = ColourChart.FrontEndButtonNormal;
					if (HighlightWhileMessageBoxActive || (!HighlightWhileMessageBoxActive && !MessageBoxController.Instance.IsAnyMessageActive))
					{
						mButton.gameObject.ColorFrom_FrontEndButton(ColourChart.FrontEndButtonHighlight, 0.2f);
					}
				}
				if (mSprite != null)
				{
					mSprite.gameObject.renderer.material.color = ColourChart.FrontEndButtonNormal;
					if (HighlightWhileMessageBoxActive || (!HighlightWhileMessageBoxActive && !MessageBoxController.Instance.IsAnyMessageActive))
					{
						mSprite.gameObject.ColorFrom_FrontEndButton(ColourChart.FrontEndButtonHighlight, 0.2f);
					}
				}
				mHighlights++;
				mTimeStateChanged = Time.realtimeSinceStartup;
			}
			else if (num >= 1.5f)
			{
				mHighlights = 0;
				mTimeStateChanged = Time.realtimeSinceStartup;
			}
		}
	}

	public void ReplaceButtonScript()
	{
		if (mButton != null && mButton.scriptWithMethodToInvoke != this)
		{
			mScriptWithMethodToInvoke = mButton.scriptWithMethodToInvoke;
			mMethodToInvoke = mButton.methodToInvoke;
			mButton.scriptWithMethodToInvoke = this;
			mButton.methodToInvoke = "OnButtonPress";
		}
	}

	public void Activate()
	{
		if (mCurrentState != State.Active && mCurrentState != State.Disabled)
		{
			mReturnState = CurrentState;
			mCurrentState = State.Active;
			RefreshButtonColour();
			mTimeStateChanged = Time.realtimeSinceStartup;
		}
	}

	private void OnButtonPress()
	{
		if (mCurrentState != State.Active && mCurrentState != State.Disabled)
		{
			mReturnState = CurrentState;
			mCurrentState = State.Active;
			RefreshButtonColour();
			mTimeStateChanged = Time.realtimeSinceStartup;
			PlaySoundFx component = base.gameObject.GetComponent<PlaySoundFx>();
			if (component != null)
			{
				component.Play();
			}
			else
			{
				InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			}
			if (mScriptWithMethodToInvoke != null)
			{
				mScriptWithMethodToInvoke.Invoke(mMethodToInvoke, mButton.delay);
			}
		}
		if (ToolTip != null && ToolTip != string.Empty)
		{
			ToolTipController.Instance.DoTooltip(Language.Get(ToolTip), base.gameObject);
		}
	}

	public void OverrideDisabledColour(Color col)
	{
		mDisabledColour = col;
		RefreshButtonColour();
	}

	private void RefreshButtonColour()
	{
		if (mButton != null)
		{
			iTween component = mButton.GetComponent<iTween>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			switch (mCurrentState)
			{
			case State.Normal:
				mButton.SetColor(ColourChart.FrontEndButtonNormal);
				mButton.gameObject.renderer.material.color = Color.white;
				break;
			case State.Active:
				mButton.SetColor(ColourChart.FrontEndButtonActive);
				mButton.gameObject.renderer.material.color = Color.white;
				break;
			case State.Selected:
				mButton.SetColor(ColourChart.FrontEndButtonSelected);
				mButton.gameObject.renderer.material.color = Color.white;
				break;
			case State.Disabled:
				mButton.SetColor(mDisabledColour);
				mButton.gameObject.renderer.material.color = Color.white;
				break;
			case State.Highlighted:
				mButton.SetColor(Color.white);
				mButton.gameObject.renderer.material.color = ColourChart.FrontEndButtonNormal;
				break;
			}
		}
		if (mSprite != null)
		{
			iTween component2 = mSprite.GetComponent<iTween>();
			if (component2 != null)
			{
				Object.Destroy(component2);
			}
			switch (mCurrentState)
			{
			case State.Normal:
				mSprite.SetColor(ColourChart.FrontEndButtonNormal);
				mSprite.gameObject.renderer.material.color = Color.white;
				break;
			case State.Active:
				mSprite.SetColor(ColourChart.FrontEndButtonActive);
				mSprite.gameObject.renderer.material.color = Color.white;
				break;
			case State.Selected:
				mSprite.SetColor(ColourChart.FrontEndButtonSelected);
				mSprite.gameObject.renderer.material.color = Color.white;
				break;
			case State.Disabled:
				mSprite.SetColor(mDisabledColour);
				mSprite.gameObject.renderer.material.color = Color.white;
				break;
			case State.Highlighted:
				mSprite.SetColor(Color.white);
				mSprite.gameObject.renderer.material.color = ColourChart.FrontEndButtonNormal;
				break;
			}
		}
	}
}
