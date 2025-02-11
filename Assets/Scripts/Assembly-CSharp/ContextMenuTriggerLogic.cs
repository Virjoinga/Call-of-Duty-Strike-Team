using UnityEngine;

public class ContextMenuTriggerLogic : MonoBehaviour
{
	private float mContextMenuHoldTime = 0.1f;

	private InterfaceableObject mContextSelection;

	private InterfaceableObject mBufferedContextSelection;

	private Vector2 mBufferedContextSelectionPos = Vector2.zero;

	private float mBufferedContextSelectionTime;

	private float mContextSelectionDownTime = -1f;

	private bool mContextSelectionTap;

	private Vector2 mContextMenuOrigin;

	private bool mInputEnabled;

	private bool registeredFingerDown;

	public void ClearBufferedContextSelection()
	{
		mBufferedContextSelection = null;
	}

	private void Start()
	{
		mInputEnabled = false;
		AllowInput(true);
		UIManager.instance.warnOnNonUiHits = false;
	}

	public bool HasSelection()
	{
		return mContextSelection != null;
	}

	public void AllowInput(bool allow)
	{
		if (allow)
		{
			EnableInput();
		}
		else
		{
			DisableInput();
		}
	}

	private void EnableInput()
	{
		mContextSelectionDownTime = -1f;
		if (!mInputEnabled)
		{
			mInputEnabled = true;
			InputManager.Instance.AddOnTwoFingerDragMoveEventHandler(ContextTrigger_OnTwoFingerDragMove, 0);
			InputManager.Instance.AddOnFingerSwipeEventHandler(ContextTrigger_OnFingerSwipe, 0);
			InputManager.Instance.AddOnFingerDownEventHandler(ContextTrigger_OnFingerDown, 0);
			InputManager.Instance.AddOnFingerUpEventHandler(ContextTrigger_OnFingerUp, 0);
			InputManager.Instance.AddOnFingerDragMoveEventHandler(ContextTrigger_OnFingerDragMove, 0);
			InputManager.Instance.AddOnTwoFingerDragBeginEventHandler(ContextTrigger_OnTwoFingerDragBegin, 0);
			InputManager.Instance.AddOnFingerDragEndEventHandler(ContextTrigger_OnFingerDragEnd, 0);
			InputManager.Instance.AddOnTwoFingerDragEndEventHandler(ContextTrigger_OnTwoFingerDragEnd, 0);
			InputManager.Instance.AddOnFingerTapEventHandler(ContextTrigger_OnFingerTap, 11);
			InputManager.Instance.AddOnFingerDoubleTapEventHandler(ContextTrigger_OnFingerDoubleTap, 0);
		}
	}

	private void DisableInput()
	{
		if (mInputEnabled)
		{
			mInputEnabled = false;
			if ((bool)InputManager.Instance)
			{
				InputManager.Instance.RemoveOnTwoFingerDragMoveEventHandler(ContextTrigger_OnTwoFingerDragMove);
				InputManager.Instance.RemoveOnFingerSwipeEventHandler(ContextTrigger_OnFingerSwipe);
				InputManager.Instance.RemoveOnFingerDownEventHandler(ContextTrigger_OnFingerDown);
				InputManager.Instance.RemoveOnFingerUpEventHandler(ContextTrigger_OnFingerUp);
				InputManager.Instance.RemoveOnFingerDragMoveEventHandler(ContextTrigger_OnFingerDragMove);
				InputManager.Instance.RemoveOnTwoFingerDragBeginEventHandler(ContextTrigger_OnTwoFingerDragBegin);
				InputManager.Instance.RemoveOnFingerDragEndEventHandler(ContextTrigger_OnFingerDragEnd);
				InputManager.Instance.RemoveOnTwoFingerDragEndEventHandler(ContextTrigger_OnTwoFingerDragEnd);
				InputManager.Instance.RemoveOnFingerTapEventHandler(ContextTrigger_OnFingerTap);
				InputManager.Instance.RemoveOnFingerDoubleTapEventHandler(ContextTrigger_OnFingerDoubleTap);
			}
		}
	}

	public void ForceFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		ContextTrigger_OnFingerDown(fingerIndex, fingerPos);
		if (mContextSelection != null)
		{
			mContextSelection.OnSelected(mContextMenuOrigin, false);
		}
		mContextSelectionDownTime = -1f;
	}

	private bool DoSelectionLogic(int fingerIndex, Vector2 fingerPos, bool fromTap)
	{
		if (fingerIndex == 1)
		{
			mContextSelectionDownTime = -1f;
			return false;
		}
		if (fingerPos.x > 0f && fingerPos.x < (float)Screen.width && fingerPos.y > 0f && fingerPos.y < (float)Screen.height && fingerIndex == 0)
		{
			SelectableObject selectableObject = SelectableObject.PickSelectableObject(fingerPos);
			GameObject gameObject = null;
			mContextSelection = null;
			if (selectableObject != null)
			{
				gameObject = selectableObject.gameObject;
				ContextMessagePopup component = gameObject.GetComponent<ContextMessagePopup>();
				if (component != null && component.CanDisplay)
				{
					component.OnSelected(fingerPos, fromTap);
					mContextSelectionDownTime = -1f;
					return false;
				}
				mContextSelection = gameObject.GetComponent<InterfaceableObject>();
			}
			mContextMenuOrigin = fingerPos;
			mContextSelectionDownTime = Time.realtimeSinceStartup;
			return true;
		}
		return false;
	}

	private void ContextTrigger_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
	}

	private void ContextTrigger_OnFingerDoubleTap(int fingerIndex, Vector2 fingerPos)
	{
		if (!ActStructure.Instance.CurrentMissionSectionIsTutorial() || !TutorialToggles.HighlightingCM)
		{
			CommonHudController.Instance.ClearContextMenu();
			mContextSelection = null;
			mContextSelectionDownTime = -1f;
		}
	}

	private bool ContextTrigger_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		mBufferedContextSelection = null;
		if (DoSelectionLogic(fingerIndex, fingerPos, false))
		{
			if (CameraManager.Instance.ActiveCamera != 0 && (mContextSelection == null || !mContextSelection.IsAllowedInStrateryView()))
			{
				mContextSelection = null;
				mContextSelectionDownTime = -1f;
				return false;
			}
			registeredFingerDown = true;
			mContextSelectionDownTime = -1f;
			return true;
		}
		return false;
	}

	private bool ContextTrigger_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		if (mContextSelection != null && registeredFingerDown)
		{
			InterfaceableObject interfaceableObject = mContextSelection;
			DoSelectionLogic(fingerIndex, fingerPos, false);
			if (mContextSelection == interfaceableObject)
			{
				if (mContextSelection.quickType == SelectableObject.QuickType.EnemySoldier && GlobalBalanceTweaks.kCMEnemySingleTapDelay != 0f)
				{
					mBufferedContextSelection = mContextSelection;
					mBufferedContextSelectionPos = fingerPos;
					mBufferedContextSelectionTime = Time.time + GlobalBalanceTweaks.kCMEnemySingleTapDelay;
				}
				else
				{
					mContextSelection.OnSelected(fingerPos, true);
				}
			}
		}
		registeredFingerDown = false;
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
		return true;
	}

	private void ContextTrigger_OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void ContextTrigger_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void ContextTrigger_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void ContextTrigger_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void ContextTrigger_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void ContextTrigger_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}

	private void Update()
	{
		if (mBufferedContextSelection != null && mBufferedContextSelectionTime < Time.time)
		{
			mBufferedContextSelection.OnSelected(mBufferedContextSelectionPos, true);
			mBufferedContextSelection = null;
		}
		if (mContextSelectionDownTime > 0f && (Time.realtimeSinceStartup - mContextSelectionDownTime > mContextMenuHoldTime || mContextSelectionTap) && mContextSelection != null)
		{
			if (mContextSelection.OnSelected(mContextMenuOrigin, mContextSelectionTap) != 0 || !mContextSelectionTap)
			{
			}
			mContextSelectionTap = false;
			mContextSelectionDownTime = -1f;
		}
	}

	public void NastyHackToAbsorbBouncyButtonPress()
	{
		mContextSelection = null;
		mContextSelectionDownTime = -1f;
	}
}
