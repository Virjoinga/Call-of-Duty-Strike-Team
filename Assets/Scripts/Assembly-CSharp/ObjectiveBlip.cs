using UnityEngine;

public class ObjectiveBlip : HudBlipIcon
{
	public enum BlipType
	{
		None = -1,
		Important = 0,
		Prop = 1,
		Character = 2,
		Location = 3,
		Manual = 4,
		A = 5,
		B = 6,
		C = 7,
		Ammo = 8,
		Mystery = 9
	}

	public static float[] BlipYOffset = new float[5] { 1.3f, 0.3f, 1.3f, 1.7f, 0f };

	private PackedSprite Icon;

	public SpriteText Text;

	public string mObjectiveText;

	private bool mIsDirtyText;

	private int mCachedDist = -1;

	public PackedSprite Highlight;

	public float HighlightMaxScale = 8f;

	public int HighlightRepeatCount = 3;

	private float mHighlightScale;

	private int mHighlightRepeat;

	public bool InfiniteLoop;

	public bool AllowHighlight = true;

	public Color blipColour = Color.white;

	public void Awake()
	{
		mObjectiveText = Language.Get("S_OBJECTIVE");
		Icon = GetComponent<PackedSprite>();
	}

	public override void Start()
	{
		IsAllowedInFirstPerson = true;
		ScreenEdgeOffsetMin = new Vector2(20f, 20f);
		ScreenEdgeOffsetMax = new Vector2(20f, 20f);
		ColourBlip(blipColour);
		bool isSwitchedOn = base.IsSwitchedOn;
		base.Start();
		if (!isSwitchedOn)
		{
			SwitchOff();
			mHighlightScale = 0f;
			Highlight.Hide(true);
		}
		else
		{
			ShowHighlight();
		}
		base.ClampToEdgeOfScreen = true;
		mIsDirtyText = true;
		Text.Text = string.Empty;
		mObjectiveText = mObjectiveText.ToUpper();
	}

	public void SetBlipType(BlipType bt)
	{
		switch (bt)
		{
		case BlipType.A:
		case BlipType.B:
		case BlipType.C:
		case BlipType.Ammo:
		case BlipType.Mystery:
			Icon.SetFrame(0, (int)(bt - 5 + 1));
			break;
		default:
			Icon.SetFrame(0, 0);
			break;
		}
	}

	public void PingBlip()
	{
		AllowHighlight = true;
		ShowHighlight();
	}

	public void PingBlip(Color newColour)
	{
		AllowHighlight = true;
		ShowHighlight(newColour);
	}

	public void ShowHighlight()
	{
		if (AllowHighlight)
		{
			InterfaceSFX.Instance.ObjectiveShowHighlightBlip.Play2D();
			Highlight.Hide(false);
			mHighlightScale = HighlightMaxScale;
			mHighlightRepeat = HighlightRepeatCount;
		}
		else
		{
			CancelHighlight();
		}
	}

	public void ShowHighlight(Color NewColour)
	{
		Highlight.SetColor(NewColour);
		ShowHighlight();
	}

	public void CancelHighlight()
	{
		Highlight.Hide(true);
	}

	public void Update()
	{
		if (!Highlight.IsHidden())
		{
			mHighlightScale = Mathf.Lerp(mHighlightScale, 0f, Time.deltaTime * 3f);
			if (mHighlightScale < 0f)
			{
				mHighlightScale = 0f;
			}
			Highlight.transform.localScale = new Vector3(mHighlightScale, mHighlightScale, 1f);
			if (mHighlightScale <= 0.4f)
			{
				if (!InfiniteLoop)
				{
					mHighlightRepeat--;
				}
				else
				{
					mHighlightRepeat = 1;
				}
				if (mHighlightRepeat <= 0)
				{
					Highlight.Hide(true);
				}
				else
				{
					mHighlightScale = HighlightMaxScale;
				}
			}
		}
		else if (InfiniteLoop && base.IsSwitchedOn)
		{
			Highlight.Hide(false);
		}
	}

	public override void UpdateOnScreen()
	{
		base.UpdateOnScreen();
		if (GameController.Instance.IsFirstPerson)
		{
			if (CameraManager.Instance != null && CameraManager.Instance.PlayCameraController != null)
			{
				float magnitude = (Target.position - CameraManager.Instance.PlayCameraController.transform.position).magnitude;
				if (mCachedDist != (int)magnitude)
				{
					mCachedDist = (int)magnitude;
					Text.Text = mObjectiveText + "\n" + magnitude.ToString("N0") + "m";
				}
			}
			else
			{
				Text.Text = string.Empty;
			}
			mIsDirtyText = true;
		}
		else if (mIsDirtyText)
		{
			mIsDirtyText = false;
			Text.Text = mObjectiveText;
		}
	}

	public override void UpdateOffScreen()
	{
		base.transform.position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(base.ScreenPos);
	}

	public override void JustGoneOffScreen()
	{
		if (base.IsSwitchedOn)
		{
			Text.renderer.enabled = false;
		}
	}

	public override void JustComeOnScreen()
	{
		if (base.IsSwitchedOn)
		{
			Text.renderer.enabled = true;
		}
	}

	public override void JustSwitchedOff()
	{
		base.gameObject.renderer.enabled = false;
		Text.renderer.enabled = false;
		CancelHighlight();
	}

	public override void JustSwitchedOn()
	{
		base.gameObject.renderer.enabled = true;
		Text.renderer.enabled = true;
		ShowHighlight();
	}

	protected override void SwitchToStrategyView()
	{
	}

	public void ColourBlip(Color colour)
	{
		blipColour = colour;
		base.renderer.material.color = colour;
	}
}
