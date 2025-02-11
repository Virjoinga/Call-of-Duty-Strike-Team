using System.Collections;
using UnityEngine;

public class HUDEditScreen : FrontEndScreen
{
	public AnimateCommonBackgroundBox BackgroundMessage;

	private float mfStartZDepth;

	private bool mbHiddenStateOnEntry;

	private bool mbTriggerLock;

	private bool mbWasTPPOnEntry;

	private bool mIsLeaving;

	private Vector2 mBufferedPos;

	private bool mBufferedIsMoving;

	protected override void Awake()
	{
		ID = ScreenID.HUDEditScreen;
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	protected override void Start()
	{
		mIsLeaving = false;
		mBufferedIsMoving = false;
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		if (mIsLeaving || BackgroundMessage.IsClosed)
		{
			mBufferedIsMoving = false;
			return;
		}
		GameSettings instance = GameSettings.Instance;
		CommonHudController.Instance.SetTriggerPressed(false);
		if (mBufferedIsMoving)
		{
			if (StrategyHudController.Instance != null && StrategyHudController.Instance.gameObject.activeInHierarchy)
			{
				if (CommonHudController.Instance.InTriggerButtonZone(mBufferedPos))
				{
					StrategyHudController.Instance.TriggerButton.transform.position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(new Vector3(mBufferedPos.x, mBufferedPos.y, 0f));
					Vector3 localPosition = StrategyHudController.Instance.TriggerButton.transform.localPosition;
					instance.PlayerGameSettings().FirstPersonMovableFireButtonXPos = localPosition.x;
					instance.PlayerGameSettings().FirstPersonMovableFireButtonYPos = localPosition.y;
					Debug.Log("TriggerDelta = " + localPosition);
				}
			}
			else if (CommonHudController.Instance.InTriggerButtonZone(mBufferedPos))
			{
				CommonHudController.Instance.TriggerButton.transform.position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(new Vector3(mBufferedPos.x, mBufferedPos.y, 0f));
				Vector3 localPosition2 = CommonHudController.Instance.TriggerButton.transform.localPosition;
				instance.PlayerGameSettings().FirstPersonMovableFireButtonXPos = localPosition2.x;
				instance.PlayerGameSettings().FirstPersonMovableFireButtonYPos = localPosition2.y;
				Debug.Log("TriggerDelta = " + localPosition2);
			}
		}
		mBufferedIsMoving = false;
		if (Input.touchCount >= 1)
		{
			mBufferedPos = Input.GetTouch(0).position;
			mBufferedIsMoving = true;
		}
		else if (Input.GetMouseButton(0))
		{
			mBufferedPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			mBufferedIsMoving = true;
		}
	}

	public void DoReset()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			instance.PlayerGameSettings().FirstPersonMovableFireButtonXPos = 0f;
			instance.PlayerGameSettings().FirstPersonMovableFireButtonYPos = 0f;
		}
	}

	public void DoLeave()
	{
		StartCoroutine("DoLeaveRoutine");
	}

	private IEnumerator DoLeaveRoutine()
	{
		mIsLeaving = true;
		BackgroundMessage.AnimateClosed();
		while (!BackgroundMessage.IsClosed)
		{
			yield return null;
		}
		FrontEndController.Instance.ReturnToPrevious();
		mIsLeaving = false;
	}

	public override void OnScreen()
	{
		base.OnScreen();
		GameController.Instance.SuppressHud(false);
		if (GameController.Instance.IsFirstPerson || HudStateController.Instance.State == HudStateController.HudState.TPP)
		{
			mbWasTPPOnEntry = HudStateController.Instance.State == HudStateController.HudState.TPP;
			if (mbWasTPPOnEntry)
			{
				HudStateController.Instance.SetState(HudStateController.HudState.FPP);
			}
			CommonHudController.Instance.mMovingTrigger = true;
			mbHiddenStateOnEntry = CommonHudController.Instance.TriggerButton.gameObject.activeSelf;
			mbTriggerLock = CommonHudController.Instance.TriggerLocked;
			CommonHudController.Instance.TriggerLocked = false;
			CommonHudController.Instance.TriggerButton.gameObject.SetActive(true);
			Vector3 localPosition = CommonHudController.Instance.TriggerButton.transform.parent.localPosition;
			mfStartZDepth = localPosition.z;
			localPosition.z = -11f;
			CommonHudController.Instance.TriggerButton.transform.parent.localPosition = localPosition;
		}
		else if (StrategyHudController.Instance != null)
		{
			StrategyHudController.Instance.MovingTriggerButton = true;
			Vector3 localPosition2 = StrategyHudController.Instance.TriggerButton.transform.parent.localPosition;
			mfStartZDepth = localPosition2.z;
			localPosition2.z = -11f;
			StrategyHudController.Instance.TriggerButton.transform.parent.localPosition = localPosition2;
		}
		TitleBarController instance = TitleBarController.Instance;
		instance.Dismiss();
		BackgroundMessage.AnimateOpen();
	}

	public override void OffScreen()
	{
		base.OffScreen();
		if (GameController.Instance.IsFirstPerson || mbWasTPPOnEntry)
		{
			if (mbWasTPPOnEntry)
			{
				HudStateController.Instance.SetState(HudStateController.HudState.TPP);
			}
			CommonHudController.Instance.TriggerLocked = mbTriggerLock;
			CommonHudController.Instance.TriggerButton.gameObject.SetActive(mbHiddenStateOnEntry);
			CommonHudController.Instance.mMovingTrigger = false;
			Vector3 localPosition = CommonHudController.Instance.TriggerButton.transform.parent.localPosition;
			localPosition.z = mfStartZDepth;
			CommonHudController.Instance.TriggerButton.transform.parent.localPosition = localPosition;
		}
		else if (StrategyHudController.Instance != null)
		{
			StrategyHudController.Instance.MovingTriggerButton = false;
			Vector3 localPosition2 = StrategyHudController.Instance.TriggerButton.transform.parent.localPosition;
			localPosition2.z = mfStartZDepth;
			StrategyHudController.Instance.TriggerButton.transform.parent.localPosition = localPosition2;
		}
		GameController.Instance.SuppressHud(true);
		Debug.Log("Saving Delta");
		GameSettings.Instance.PlayerGameSettings().Save();
	}
}
