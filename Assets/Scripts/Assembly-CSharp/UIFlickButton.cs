using UnityEngine;

public class UIFlickButton : UIButton
{
	public float Range = 2f;

	public float WrapRangeMultiplier = 4f;

	public float TapBiasX;

	public MonoBehaviour scriptWithMethodToInvokeOnLeft;

	public string methodToInvokeOnLeft = string.Empty;

	public MonoBehaviour scriptWithMethodToInvokeOnRight;

	public string methodToInvokeOnRight = string.Empty;

	protected EZDragDropFlickButtonHelper mDragDropFlickHelper;

	private bool mStartedOver;

	protected override void Awake()
	{
		mDragDropFlickHelper = new EZDragDropFlickButtonHelper(this);
		dragDropHelper = mDragDropFlickHelper;
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		OnReposition();
		mDragDropFlickHelper.WorldOrigin = base.transform.position;
	}

	private void OnReposition()
	{
	}

	private void OnEZTranslated()
	{
		mDragDropFlickHelper.UpdateAlpha();
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		mTapHitPoint = ptr.hitInfo.point - base.transform.position;
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF)
		{
			if (mStartedOver)
			{
				base.OnInput(ref ptr);
				mStartedOver = false;
			}
		}
		else if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			mStartedOver = true;
		}
		else if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
		{
			mStartedOver = false;
		}
	}

	public void TappedButton()
	{
		if (!mDragDropFlickHelper.IsDragging)
		{
			if (mTapHitPoint.x > TapBiasX)
			{
				HitRightButton();
				mDragDropFlickHelper.ForceAnimFromLeft();
			}
			else
			{
				HitLeftButton();
				mDragDropFlickHelper.ForceAnimFromRight();
			}
		}
	}

	public void HitLeftButton()
	{
		if (scriptWithMethodToInvokeOnLeft != null)
		{
			scriptWithMethodToInvokeOnLeft.Invoke(methodToInvokeOnLeft, delay);
		}
	}

	public void HitRightButton()
	{
		if (scriptWithMethodToInvokeOnRight != null)
		{
			scriptWithMethodToInvokeOnRight.Invoke(methodToInvokeOnRight, delay);
		}
	}
}
