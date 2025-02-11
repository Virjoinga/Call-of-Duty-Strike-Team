using UnityEngine;

public class PercentageEditOption : MonoBehaviour
{
	public PackedSprite Foreground;

	public PackedSprite Background;

	public SpriteText OptionalTextValueOutput;

	public int TotalUnits = 20;

	public SpriteText LabelTextItem;

	public GameObject TreeRoot;

	public MonoBehaviour ScriptWithMethodToInvoke;

	public string MethodToInvokeOnChange;

	private Rect mScreenLocation;

	private Rect mForegroundUVs;

	private float mPercentage;

	private float mStartWidth;

	private bool mDirty;

	public CommonBackgroundBoxPlacement BackgroundBox;

	public Color mDefaultForegroundColor;

	public Color mValueColor;

	private string mToolTip;

	private bool mSliderActive;

	private Vector2 mSliderActivePos;

	private float mPercentWhenActive;

	public float Value
	{
		get
		{
			return mPercentage;
		}
		set
		{
			mPercentage = value;
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
		mStartWidth = -1f;
		mPercentage = 0.5f;
		mDirty = true;
	}

	private void Start()
	{
		if (Foreground != null)
		{
			mForegroundUVs = Foreground.GetUVs();
			mDefaultForegroundColor = Foreground.color;
			mDefaultForegroundColor.a = 1f;
		}
		mSliderActive = false;
	}

	public void SetModifiable(bool bOn)
	{
		Foreground.SetColor((!bOn) ? (mDefaultForegroundColor * new Color(0.5f, 0.5f, 0.5f, 1f)) : mDefaultForegroundColor);
		LabelTextItem.SetColor((!bOn) ? new Color(0.25f, 0.25f, 0.25f, 1f) : new Color(1f, 1f, 1f, 1f));
		if (OptionalTextValueOutput != null)
		{
			OptionalTextValueOutput.SetColor((!bOn) ? (mValueColor * new Color(0.5f, 0.5f, 0.5f, 1f)) : mValueColor);
		}
		if (bOn)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}

	public void Enable()
	{
		FingerGestures.OnFingerDown -= OnFingerDown;
		FingerGestures.OnFingerDown += OnFingerDown;
		FingerGestures.OnFingerUp -= OnFingerUp;
		FingerGestures.OnFingerUp += OnFingerUp;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
		FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDrag;
		FingerGestures.OnFingerDragMove += FingerGestures_OnFingerDrag;
	}

	public void Disable()
	{
		FingerGestures.OnFingerDown -= OnFingerDown;
		FingerGestures.OnFingerUp -= OnFingerUp;
		FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDrag;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
		ScriptWithMethodToInvoke = null;
		MethodToInvokeOnChange = null;
	}

	private void Update()
	{
		if (mDirty)
		{
			int num = (int)((float)TotalUnits * mPercentage + 0.5f);
			float percentage = (mPercentage = (float)num / (float)TotalUnits);
			if (OptionalTextValueOutput != null)
			{
				OptionalTextValueOutput.Text = num.ToString();
			}
			UpdateForeground(percentage);
			mDirty = false;
			if (ScriptWithMethodToInvoke != null && MethodToInvokeOnChange != null)
			{
				ScriptWithMethodToInvoke.Invoke(MethodToInvokeOnChange, 0f);
			}
		}
	}

	private void UpdateForeground(float percentage)
	{
		float num = percentage;
		float num2 = 1f;
		if (Foreground != null)
		{
			if (mStartWidth < 0f)
			{
				mStartWidth = Foreground.width;
			}
			num *= mStartWidth;
			num2 *= Foreground.height;
			Rect uVs = mForegroundUVs;
			uVs.width *= percentage;
			Foreground.SetSize(num, num2);
			Foreground.SetUVs(uVs);
		}
	}

	private void CheckTip(Vector2 fingerPos)
	{
		if (BackgroundBox.BoundingRect.Contains(fingerPos) && mToolTip != null)
		{
			ToolTipController.Instance.DoTooltip(mToolTip, BackgroundBox.gameObject);
		}
	}

	private void OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (IsInBounds(fingerPos))
		{
			Foreground.SetColor(Color.white);
			mSliderActivePos = fingerPos;
			UpdatePosition(fingerPos, true);
			mPercentWhenActive = mPercentage;
			mSliderActive = true;
		}
	}

	private void OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		if (mSliderActive)
		{
			mSliderActive = false;
			Foreground.SetColor(mDefaultForegroundColor);
		}
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
	}

	private void FingerGestures_OnFingerDrag(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (mSliderActive)
		{
			UpdatePosition(fingerPos, false);
		}
	}

	private bool IsInBounds(Vector2 screenPos)
	{
		if (Background != null)
		{
			Camera main = Camera.main;
			if (mScreenLocation.width == 0f)
			{
				float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
				Vector2 vector = main.WorldToScreenPoint(base.transform.position);
				float width = Background.width / num;
				float num2 = Background.height / num;
				mScreenLocation = new Rect(vector.x, vector.y - num2 * 0.5f, width, num2);
			}
			return mScreenLocation.Contains(screenPos);
		}
		return false;
	}

	private void UpdatePosition(Vector2 screenPos, bool absolute)
	{
		if (Background != null)
		{
			if (absolute)
			{
				float num = screenPos.x - mScreenLocation.xMin;
				mPercentage = num / mScreenLocation.width;
			}
			else
			{
				mPercentage = mPercentWhenActive + (screenPos.x - mSliderActivePos.x) / (float)(Screen.width / 3);
				mPercentage = Mathf.Clamp01(mPercentage);
			}
			mDirty = true;
		}
	}
}
