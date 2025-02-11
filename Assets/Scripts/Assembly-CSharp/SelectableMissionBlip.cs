using UnityEngine;

public class SelectableMissionBlip : HudBlipIcon
{
	private const float ANIMATED_BACKGROUND_BURST_TIME = 0.4f;

	private const float ANIMATED_BACKGROUND_BREAK_TIME = 1.5f;

	private const float ANIMATED_BACKGROUND_FADE_TIME = 1f;

	private const float ANIMATE_SCALE_TO = 2f;

	private const float HIGHLIGHT_ROTATION_SPEED = 20f;

	private const int NUM_BACKGROUNDS = 3;

	public SpriteText Label;

	public PackedSprite LabelBackground;

	public PackedSprite Icon;

	public PackedSprite IconBackground;

	public PackedSprite IconForeground;

	public SpriteText TimeLabel;

	private PackedSprite[] mIconBackgrounds;

	private float m_Alpha;

	private float m_LabelAlpha;

	private float m_TimeSinceAnim;

	private int m_CurrentBackground;

	private int m_NumBursts;

	private bool m_Active;

	private bool m_Highlight;

	private bool m_WasHighlight;

	private SoundManager.SoundInstance mBlipLoopInstance;

	public bool Active
	{
		get
		{
			return m_Active;
		}
	}

	public bool Highlight
	{
		get
		{
			return m_Highlight;
		}
		set
		{
			m_WasHighlight = m_Highlight;
			m_Highlight = value;
		}
	}

	public void Awake()
	{
		SetInactiveImmediate();
	}

	public override void Start()
	{
		base.Start();
		CameraOverride = GlobeSelect.Instance.GlobeCamera;
		IconBackground.gameObject.SetActive(false);
		m_TimeSinceAnim = 0f;
		m_NumBursts = 0;
		m_Alpha = -1f;
		m_LabelAlpha = -1f;
		m_WasHighlight = false;
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		SetAlpha(GlobeSelect.GetPositionAlphaPos(Target.transform.position));
	}

	public void SetTime(int time)
	{
		if (time > 0)
		{
			Label.Text = TimeUtils.GetMinutesSecondsCountdownFromSeconds(time);
		}
		else
		{
			Label.Text = string.Empty;
		}
	}

	public void SetLabelColour(Color col)
	{
		Label.SetColor(new Color(col.r, col.g, col.b, m_LabelAlpha * m_Alpha));
	}

	public void SetActive(bool active)
	{
		if (m_Active != active)
		{
			m_Active = active;
			Vector3 scale = ((!active) ? new Vector3(0f, 0f, 0f) : new Vector3(1f, 1f, 1f));
			if (Icon != null)
			{
				Icon.gameObject.ScaleTo(scale, 0.2f, 0f);
			}
			if (IconForeground != null)
			{
				IconForeground.gameObject.ScaleTo(scale, 0.2f, 0f);
			}
			if (LabelBackground != null)
			{
				LabelBackground.gameObject.ScaleTo(scale, 0.2f, 0f);
			}
			if (Label != null)
			{
				Label.gameObject.ScaleTo(scale, 0.2f, 0f);
			}
			if (TimeLabel != null)
			{
				TimeLabel.gameObject.ScaleTo(scale, 0.2f, 0f);
			}
		}
	}

	private void Update()
	{
		float num = ((!(Label != null) || !(Label.Text != string.Empty)) ? 0f : 1f);
		if (num > 0f && GlobeSelect.Instance != null && GlobeSelect.Instance.GlobeCamera != null)
		{
			GlobeCamera component = GlobeSelect.Instance.GlobeCamera.GetComponent<GlobeCamera>();
			if (component != null && component.IsZoomedIn())
			{
				num = 0f;
			}
		}
		if (num != m_LabelAlpha)
		{
			m_LabelAlpha = num;
			if (LabelBackground != null)
			{
				LabelBackground.SetColor(new Color(LabelBackground.Color.r, LabelBackground.Color.g, LabelBackground.Color.b, m_LabelAlpha * m_Alpha));
			}
			if (Label != null)
			{
				Label.SetColor(new Color(Label.Color.r, Label.Color.g, Label.Color.b, m_LabelAlpha * m_Alpha));
				if (m_LabelAlpha <= 0.1f)
				{
					Label.Text = string.Empty;
				}
			}
		}
		if (IconBackground != null && (m_Highlight || m_WasHighlight))
		{
			m_TimeSinceAnim += Time.deltaTime;
			if (Icon != null)
			{
				Icon.transform.Rotate(new Vector3(0f, 0f, 0f - 20f * TimeManager.DeltaTime));
			}
			if (mIconBackgrounds == null || mIconBackgrounds.Length != 3)
			{
				mIconBackgrounds = new PackedSprite[3];
				mIconBackgrounds[0] = IconBackground;
				for (int i = 1; i < 3; i++)
				{
					mIconBackgrounds[i] = Object.Instantiate(IconBackground) as PackedSprite;
					mIconBackgrounds[i].SetColor(Color.clear);
					mIconBackgrounds[i].transform.parent = base.transform;
					mIconBackgrounds[i].transform.localPosition = Vector3.zero;
				}
				m_TimeSinceAnim = 0.4f;
				m_CurrentBackground = 0;
				m_NumBursts = 0;
			}
			if (m_NumBursts < 3 && m_TimeSinceAnim >= 0.4f)
			{
				mIconBackgrounds[m_CurrentBackground].gameObject.SetActive(true);
				mIconBackgrounds[m_CurrentBackground].transform.localScale = Vector3.zero;
				mIconBackgrounds[m_CurrentBackground].SetColor(Color.white);
				mIconBackgrounds[m_CurrentBackground].gameObject.ScaleTo(Vector3.one * 2f, 1f, 0f);
				m_TimeSinceAnim = 0f;
				m_CurrentBackground = (m_CurrentBackground + 1) % 3;
				m_NumBursts++;
			}
			else if (m_TimeSinceAnim >= 1.5f)
			{
				m_NumBursts = 0;
			}
			int num2 = 0;
			Color white = Color.white;
			for (int j = 0; j < 3; j++)
			{
				if (mIconBackgrounds[j].gameObject.activeInHierarchy)
				{
					float x = mIconBackgrounds[j].transform.localScale.x;
					white.a = (1f - x / 2f) * m_Alpha;
					mIconBackgrounds[j].SetColor(white);
					mIconBackgrounds[j].gameObject.SetActive(x < 2f);
					num2++;
				}
			}
			if (m_WasHighlight && num2 == 0)
			{
				m_WasHighlight = false;
			}
		}
		DoBlipSFXLoop();
	}

	private void SetInactiveImmediate()
	{
		m_Active = false;
		Icon.gameObject.transform.localScale = Vector3.zero;
		IconForeground.gameObject.transform.localScale = Vector3.zero;
		LabelBackground.gameObject.transform.localScale = Vector3.zero;
		Label.gameObject.transform.localScale = Vector3.zero;
		TimeLabel.gameObject.transform.localScale = Vector3.zero;
		StopBlipSFXLoop();
	}

	private void SetAlpha(float alpha)
	{
		if (m_Alpha != alpha)
		{
			m_Alpha = alpha;
			if (Icon != null)
			{
				Icon.SetColor(new Color(Icon.Color.r, Icon.Color.g, Icon.Color.b, alpha));
			}
			if (IconForeground != null)
			{
				IconForeground.SetColor(new Color(IconForeground.Color.r, IconForeground.Color.g, IconForeground.Color.b, alpha));
			}
			if (LabelBackground != null)
			{
				LabelBackground.SetColor(new Color(LabelBackground.Color.r, LabelBackground.Color.g, LabelBackground.Color.b, m_LabelAlpha * alpha));
			}
			if (Label != null)
			{
				Label.SetColor(new Color(Label.Color.r, Label.Color.g, Label.Color.b, m_LabelAlpha * alpha));
			}
			if (TimeLabel != null)
			{
				TimeLabel.SetColor(new Color(TimeLabel.Color.r, TimeLabel.Color.g, TimeLabel.Color.b, alpha));
			}
		}
	}

	private void DoBlipSFXLoop()
	{
		if (m_Active && m_Highlight && IconBackground != null && base.IsSwitchedOn && base.IsOnScreen && base.Visible && Icon != null && Icon.Color.a != 0f && !ActivateWatcher.Instance.ActivateUIOpen)
		{
			if (mBlipLoopInstance == null)
			{
				mBlipLoopInstance = GlobeSFX.Instance.GlobeMarkerLoop.Play2D();
			}
		}
		else
		{
			StopBlipSFXLoop();
		}
	}

	public void StopBlipSFXLoop()
	{
		if (mBlipLoopInstance != null)
		{
			mBlipLoopInstance.Stop();
			mBlipLoopInstance = null;
		}
	}
}
