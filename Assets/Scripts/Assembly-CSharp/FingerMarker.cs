using UnityEngine;

public class FingerMarker : MonoBehaviour
{
	private enum MarkerState
	{
		Off = 0,
		Appearing = 1,
		ActiveIdle = 2,
		Disappearing = 3
	}

	private MarkerState mState;

	private float mAppearTime = 0.1f;

	private float mDisappearTime = 0.1f;

	private float mWaitingRotateSpeed = 10f;

	private float mActiveRotateSpeed = -2f;

	private ContextMenuBase mLinkedMenu;

	private void Start()
	{
		base.gameObject.ScaleTo(Vector3.zero, 0f, 0f, EaseType.easeOutBounce);
		mState = MarkerState.Off;
		GetComponent<PackedSprite>().SetColor(ColourChart.ContextMenuMarker);
	}

	public void Appear()
	{
		if (mState != MarkerState.Appearing && mState != MarkerState.ActiveIdle)
		{
			mState = MarkerState.Appearing;
			base.gameObject.ScaleTo(Vector3.one, mAppearTime, 0f, EaseType.easeOutCubic);
		}
	}

	public void ActiveIdle()
	{
		mState = MarkerState.ActiveIdle;
		mLinkedMenu = CommonHudController.Instance.ContextMenu;
	}

	public void Disappear()
	{
		if (mState == MarkerState.Disappearing)
		{
			return;
		}
		if (mState == MarkerState.Appearing)
		{
			iTween component = base.gameObject.GetComponent<iTween>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			base.gameObject.ScaleTo(Vector3.zero, 0f, 0f, EaseType.easeInCubic);
		}
		else
		{
			base.gameObject.ScaleTo(Vector3.zero, mDisappearTime, 0f, EaseType.easeInCubic);
		}
		mState = MarkerState.Disappearing;
	}

	private void Update()
	{
		switch (mState)
		{
		case MarkerState.Off:
			break;
		case MarkerState.Appearing:
		case MarkerState.Disappearing:
			base.gameObject.RotateUpdate(new Vector3(0f, 0f, base.gameObject.transform.eulerAngles.z + mWaitingRotateSpeed), 1f);
			break;
		case MarkerState.ActiveIdle:
			if (mLinkedMenu != null)
			{
				Vector3 position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(mLinkedMenu.FingerPosition);
				base.gameObject.MoveUpdate(position, 0.2f);
			}
			base.gameObject.RotateUpdate(new Vector3(0f, 0f, base.gameObject.transform.eulerAngles.z + mActiveRotateSpeed), 1f);
			break;
		}
	}
}
