using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBarsController : MonoBehaviour
{
	private const float kSkipButtonOffscreenOffset = 1.5f;

	public SimpleSprite BlackBarTop;

	public SimpleSprite BlackBarBottom;

	public UIButton SkipButton;

	public SpriteText SubtitleText;

	private float mSubtitleTimer;

	private Vector3 SkipButtonPos;

	private static BlackBarsController smInstance;

	private bool mBlackBarsEnabled;

	private bool mSkipButtonEnabled = true;

	public float BlackBarTransitionTime = 1f;

	public static BlackBarsController Instance
	{
		get
		{
			return smInstance;
		}
	}

	public bool BlackBarsEnabled
	{
		get
		{
			return mBlackBarsEnabled;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple BlackBarsController");
		}
		smInstance = this;
		if (SkipButton != null)
		{
			SkipButton.scriptWithMethodToInvoke = InteractionsManager.Instance;
			SkipButton.methodToInvoke = "OnSkipPressed";
		}
	}

	private void Start()
	{
		BlackBarTop.gameObject.ScaleUpdate(new Vector3(1f, 0f, 1f), 0f);
		BlackBarBottom.gameObject.ScaleUpdate(new Vector3(1f, 0f, 1f), 0f);
		HideBars();
		SkipButtonPos = SkipButton.transform.position;
		SkipButton.gameObject.MoveUpdate(new Vector3(SkipButtonPos.x, SkipButtonPos.y - 1.5f, SkipButtonPos.z), 0f);
		StartCoroutine(PositionSubtitleTextAfterBlackBarBottomReady());
	}

	private IEnumerator PositionSubtitleTextAfterBlackBarBottomReady()
	{
		yield return WaitForRenderCameraToBeSetOnBlackBarBottom();
		PositionSubtitleTextVerticallyInCentreOfBlackBarBottom();
	}

	private IEnumerator WaitForRenderCameraToBeSetOnBlackBarBottom()
	{
		while (BlackBarBottom.renderCamera == null)
		{
			yield return null;
		}
	}

	private Vector2 SizeOfSimpleSpriteOnScreen(SimpleSprite sprite)
	{
		if (sprite == null || sprite.renderCamera == null)
		{
			return Vector2.zero;
		}
		Vector3 position = new Vector3(sprite.width, sprite.height, 0f);
		Vector2 vector = sprite.renderCamera.WorldToScreenPoint(position);
		Vector2 vector2 = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		return vector - vector2;
	}

	private void PositionSubtitleTextVerticallyInCentreOfBlackBarBottom()
	{
		EZScreenPlacement component = SubtitleText.transform.GetComponent<EZScreenPlacement>();
		if (component != null && BlackBarBottom != null && BlackBarBottom.renderCamera != null && BlackBarBottom.height != 0f)
		{
			Vector2 vector = SizeOfSimpleSpriteOnScreen(BlackBarBottom);
			if (vector.y != 0f)
			{
				Vector3 screenPos = component.screenPos;
				screenPos.y = vector.y * 0.5f;
				component.screenPos = screenPos;
				component.PositionOnScreen();
			}
		}
	}

	public void SetBlackBars(bool enable, bool skippable)
	{
		if (mBlackBarsEnabled == enable)
		{
			return;
		}
		mBlackBarsEnabled = enable;
		mSkipButtonEnabled = skippable;
		if (mBlackBarsEnabled)
		{
			BlackBarTop.Hide(false);
			BlackBarBottom.Hide(false);
			SkipButton.Hide(!mSkipButtonEnabled);
			BlackBarTop.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), BlackBarTransitionTime, 0f, EaseType.linear);
			BlackBarBottom.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), BlackBarTransitionTime, 0f, EaseType.linear);
			if (mSkipButtonEnabled)
			{
				SkipButton.gameObject.MoveTo(new Vector3(SkipButtonPos.x, SkipButtonPos.y + 1.5f, SkipButtonPos.z), BlackBarTransitionTime, 0f, EaseType.linear);
			}
		}
		else
		{
			ClearSubtitle();
			BlackBarTop.gameObject.ScaleTo(new Vector3(1f, 0f, 1f), BlackBarTransitionTime, 0f, EaseType.linear, "HideBars", base.gameObject);
			BlackBarBottom.gameObject.ScaleTo(new Vector3(1f, 0f, 1f), BlackBarTransitionTime, 0f, EaseType.linear);
			if (mSkipButtonEnabled)
			{
				SkipButton.gameObject.MoveTo(new Vector3(SkipButtonPos.x, SkipButtonPos.y - 1.5f, SkipButtonPos.z), BlackBarTransitionTime, 0f, EaseType.linear);
			}
		}
	}

	public void HideSkipAndSubtitles()
	{
		if (mSkipButtonEnabled)
		{
			SkipButton.gameObject.MoveTo(new Vector3(SkipButtonPos.x, SkipButtonPos.y - 1.5f, SkipButtonPos.z), BlackBarTransitionTime, 0f, EaseType.linear);
		}
		mSkipButtonEnabled = false;
		ClearSubtitle();
	}

	private void HideBars()
	{
		BlackBarTop.Hide(true);
		BlackBarBottom.Hide(true);
		SkipButton.Hide(true);
		ClearSubtitle();
	}

	public void DisplaySubtitle(string text, float duration)
	{
		if (BlackBarsEnabled)
		{
			SubtitleText.Hide(false);
		}
		mSubtitleTimer = duration;
		if (text == string.Empty)
		{
			SubtitleText.Text = text;
		}
		else
		{
			SubtitleText.Text = AutoLocalize.Get(text);
		}
		List<string> list = new List<string>(SubtitleText.Text.Split(':'));
		SubtitleText.Text = string.Format("{0}{1}{2}", "[#5CC6CC]", list[0], ":[#FFFFFF]");
		list.RemoveAt(0);
		SubtitleText.Text += string.Join(":", list.ToArray());
	}

	public void ClearSubtitle()
	{
		DisplaySubtitle(string.Empty, 0f);
		SubtitleText.Hide(true);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			InteractionsManager.Instance.SendMessage("OnSkipPressed");
		}
		if (mSubtitleTimer > 0f)
		{
			mSubtitleTimer -= Time.deltaTime;
			if (mSubtitleTimer <= 0f)
			{
				ClearSubtitle();
			}
		}
	}
}
