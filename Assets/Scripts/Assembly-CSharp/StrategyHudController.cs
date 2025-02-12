using System;
using UnityEngine;

public class StrategyHudController : MonoBehaviour
{
	public delegate void TechPurchaseClickEventHandler(object sender, EventArgs args);

	private static StrategyHudController smInstance;

	public GameObject Target;

	public GameObject TargetFrame;

	public UIButton PauseButton;

	public UIButton Look;

	public UIButton TriggerButton;

	public UIProgressBar HeatBarLeft;

	public UIProgressBar HeatBarRight;

	public PackedSprite HeatWarningLeft;

	public PackedSprite HeatWarningRight;

	public GameObject ChainGunUi;

	public PackedSprite[] RocketSprites;

	private int mLookAmountFrameIndex;

	private Vector2[] mLookAmountFrames = new Vector2[4];

	private Vector2 mLookAmountInternalTouch;

	private Vector2 mLookAmountInternalPad;

	private Vector3[] mLookStickNotchesPad;

	private bool mTriggerPressedBuffer;

	private static float MaxDelayTime = 5f;

	private static float MinDelayTime = 2f;

	private float mCurrentDelayTime;

	private float mAccumulatedDeltaTime;

	public static StrategyHudController Instance
	{
		get
		{
			return smInstance;
		}
	}

	public bool MovingTriggerButton { get; set; }

	public Vector2 LookAmount { get; private set; }

	public Vector2 LookAmountPad { get; private set; }

	public bool TriggerPressed { get; private set; }

	public bool HoldingView { get; private set; }

	public Vector3[] LookStickNotchesPad
	{
		get
		{
			return mLookStickNotchesPad;
		}
		set
		{
			if (value != null && value.Length > 0)
			{
				mLookStickNotchesPad = value;
				return;
			}
			mLookStickNotchesPad = new Vector3[1]
			{
				new Vector3(1f, 0f, 1f)
			};
		}
	}

	public bool ShowOverwatchShootingHUD { get; set; }

	public event TechPurchaseClickEventHandler OnTechPurchaseClick;

	private void Awake()
	{
		MovingTriggerButton = false;
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple StrategyHudController");
		}
		smInstance = this;
		if (GameController.Instance != null)
		{
			GameController.Instance.LinkHudController(this);
		}
		Look.AddInputDelegate(OnLookInput);
		TriggerButton.AddInputDelegate(OnTriggerInput);
		Look.gameObject.SetActive(false);
		mLookStickNotchesPad = new Vector3[3]
		{
			new Vector3(0.8f, 0f, 0.1f),
			new Vector3(0.95f, 0.1f, 0.2f),
			new Vector3(1f, 0.5f, 1f)
		};
	}

	private void Start()
	{
		HideOverwatchShootingHud(true);
		mCurrentDelayTime = UnityEngine.Random.Range(MinDelayTime, MaxDelayTime);
		ResizeLookRegions();
		int num = RocketSprites.Length;
		float num2 = 0.05f;
		float num3 = 0.66f;
		float num4 = (0f - (num3 + 0.5f)) / 4f;
		float num5 = (0f - (num2 + 0.14f)) / 4f;
		for (int num6 = num - 1; num6 >= 0; num6--)
		{
			RocketSprites[num6].transform.position = Vector3.zero;
			RocketSprites[num6].transform.localPosition = new Vector3(num2, num3, -0.1f);
			num3 += num4;
			num2 += num5;
		}
	}

	private void ResizeLookRegions()
	{
		BoxCollider boxCollider = (Look.GetComponent<Collider>() as BoxCollider) ?? Look.gameObject.AddComponent<BoxCollider>();
		if (boxCollider != null && GUISystem.Instance != null && GUISystem.Instance.m_guiCamera != null)
		{
			Vector3 vector = GUISystem.Instance.m_guiCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector2 = GUISystem.Instance.m_guiCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
			Vector3 size = vector2 - vector;
			boxCollider.center = new Vector3(vector.x + 0.5f * size.x, vector.y + 0.5f * size.y, 0f);
			boxCollider.size = size;
			Look.transform.localScale = Vector3.one;
			Look.transform.position = new Vector3(0f, 0f, Look.transform.position.z);
		}
	}

	public void OnLookInput(ref POINTER_INFO ptr)
	{
		mLookAmountInternalTouch += ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : InputUtils.GetTrackpadInput(ptr.ToPointerInfo()));
		HoldingView = ptr.active;
	}

	private void OnTriggerInput(ref POINTER_INFO ptr)
	{
		if (ptr.active)
		{
			mTriggerPressedBuffer = true;
		}
		float num = 1f;
		mLookAmountInternalTouch += ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : (num * InputUtils.GetTrackpadInput(ptr.ToPointerInfo())));
	}

	public void SetTriggerPressed(bool pressed)
	{
		OverwatchController instance = OverwatchController.Instance;
		if (!MovingTriggerButton)
		{
			if (pressed)
			{
				TriggerPressed = true;
				TriggerButton.SetColor(ColourChart.HudButtonPress);
				instance.FireWeapon(true);
			}
			else
			{
				TriggerPressed = false;
				TriggerButton.SetColor(new Color(1f, 1f, 1f, 1f));
				instance.FireWeapon(false);
			}
		}
	}

	public void FireRocket()
	{
		OverwatchController.Instance.FireCannon();
	}

	private void OnPauseButtonPress()
	{
		if (PauseButton != null && PauseButton.gameObject.activeInHierarchy)
		{
			GameController.Instance.TogglePause();
		}
	}

	private void UpdatePadInput()
	{
		Controller.State state = Controller.GetState();
		if (!state.connected || !SwrveServerVariables.Instance.AllowGCController)
		{
			return;
		}
		if (state.pause)
		{
			OnPauseButtonPress();
		}
		if (!(GameController.Instance == null) && !GameController.Instance.IsPaused && !MessageBoxController.Instance.IsAnyMessageActive)
		{
			if (state.rightTrigger.pressed && TriggerButton != null && TriggerButton.gameObject.activeInHierarchy)
			{
				mTriggerPressedBuffer = true;
			}
			if (state.leftTrigger.pressed)
			{
				FireRocket();
			}
			Vector2 input = new Vector2(state.rightThumbstick.x, state.rightThumbstick.y);
			mLookAmountInternalPad = TimeManager.DeltaTime * InputUtils.GetThumbstickInput(input, mLookStickNotchesPad);
		}
	}

	private void UpdateAndroidPadInput()
	{
		Controller.State state = Controller.GetState();
		if (!state.connected || !SwrveServerVariables.Instance.AllowGCController)
		{
			return;
		}
		bool isMogaPro = Controller.GetIsMogaPro();
		if (state.pause)
		{
			OnPauseButtonPress();
		}
		if (!(GameController.Instance == null) && !GameController.Instance.IsPaused && !MessageBoxController.Instance.IsAnyMessageActive)
		{
			if ((state.rightTrigger.pressed || (!isMogaPro && state.rightShoulder.pressed)) && TriggerButton != null && TriggerButton.gameObject.activeInHierarchy)
			{
				mTriggerPressedBuffer = true;
			}
			if (state.leftTrigger.pressed || (!isMogaPro && state.leftShoulder.pressed))
			{
				FireRocket();
			}
			Vector2 vector = new Vector2(state.rightThumbstick.x, state.rightThumbstick.y);
			mLookAmountInternalPad = TimeManager.DeltaTime * (vector * vector.magnitude);
		}
	}

	private void Update()
	{
		if (ShowOverwatchShootingHUD)
		{
			HideOverwatchShootingHud(false);
			ShowOverwatchShootingHUD = false;
		}
		UpdateAndroidPadInput();
		SetTriggerPressed(mTriggerPressedBuffer);
		mTriggerPressedBuffer = false;
		mLookAmountFrames[mLookAmountFrameIndex++] = mLookAmountInternalTouch;
		mLookAmountFrameIndex %= mLookAmountFrames.Length;
		LookAmount = Vector2.zero;
		for (int i = 0; i < mLookAmountFrames.Length; i++)
		{
			LookAmount += mLookAmountFrames[i];
		}
		LookAmount *= 1f / (float)mLookAmountFrames.Length;
		mLookAmountInternalTouch = Vector2.zero;
		LookAmountPad = mLookAmountInternalPad;
		mLookAmountInternalPad = Vector2.zero;
		mAccumulatedDeltaTime += Time.deltaTime;
		if (mAccumulatedDeltaTime > mCurrentDelayTime)
		{
			mAccumulatedDeltaTime = 0f;
			mCurrentDelayTime = UnityEngine.Random.Range(MinDelayTime, MaxDelayTime);
			OverwatchSFX.Instance.RadioVoice.Play2D();
		}
		int num = RocketSprites.Length;
		int num2 = OverwatchController.Instance.NumRocketsRemaining;
		if (num2 > num)
		{
			num2 = num;
		}
		bool reloadingRocket = OverwatchController.Instance.ReloadingRocket;
		float rocketClipReady = OverwatchController.Instance.RocketClipReady01;
		float num3 = rocketClipReady * (float)num;
		bool flag = rocketClipReady < 1f;
		for (int j = 0; j < num; j++)
		{
			if (flag)
			{
				bool flag2 = (float)j >= num3;
				if (RocketSprites[j].IsHidden() != flag2)
				{
					OverwatchSFX.Instance.RocketReload.Play2D();
					RocketSprites[j].Hide(flag2);
				}
				if (RocketSprites[j].Color != ColourChart.HudRed)
				{
					RocketSprites[j].SetColor(ColourChart.HudRed);
				}
			}
			else
			{
				bool flag3 = j >= num2;
				if (RocketSprites[j].IsHidden() != flag3)
				{
					RocketSprites[j].Hide(flag3);
				}
				Color color = ((!reloadingRocket) ? ColourChart.HudWhite : ColourChart.HudYellow);
				if (RocketSprites[j].Color != color)
				{
					RocketSprites[j].SetColor(color);
				}
			}
		}
	}

	public void GoToTechPurchase(TechPurchaseEventArgs args)
	{
		if (this.OnTechPurchaseClick != null)
		{
			this.OnTechPurchaseClick(this, args);
		}
	}

	public void HideOverwatchShootingHud(bool hide)
	{
		Target.BroadcastMessage("Hide", hide);
		TargetFrame.BroadcastMessage("Hide", hide);
		ChainGunUi.BroadcastMessage("Hide", hide);
		TriggerButton.gameObject.BroadcastMessage("Hide", hide);
		HeatBarLeft.Hide(hide);
		HeatBarRight.Hide(hide);
		HeatWarningLeft.Hide(hide);
		HeatWarningRight.Hide(hide);
	}

	public void SetChainGunHeat(float heatFraction)
	{
		HeatBarLeft.Value = heatFraction;
		HeatBarRight.Value = heatFraction;
		float num = 0.05f;
		Color color = ((!(Time.time % (num * 2f) > num)) ? ColourChart.HudWhite : ColourChart.HudRed);
		if (heatFraction > 0.8f)
		{
			HeatWarningLeft.SetColor(color);
			HeatWarningRight.SetColor(color);
		}
		else
		{
			HeatWarningLeft.SetColor(ColourChart.HudWhite);
			HeatWarningRight.SetColor(ColourChart.HudWhite);
		}
	}
}
