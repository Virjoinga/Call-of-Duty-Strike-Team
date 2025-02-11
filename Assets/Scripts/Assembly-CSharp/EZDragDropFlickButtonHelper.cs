using UnityEngine;

public class EZDragDropFlickButtonHelper : EZDragDropHelper
{
	private Vector3 mWorldOrigin;

	private UIFlickButton mFlickButton;

	public Vector3 WorldOrigin
	{
		get
		{
			return mWorldOrigin;
		}
		set
		{
			mWorldOrigin = value;
		}
	}

	public EZDragDropFlickButtonHelper(UIFlickButton h)
		: base(h)
	{
		dragDropDelegate = SliderDragDropDelegate;
		dragPosUpdateDel = SliderDragUpdatePosition;
		mWorldOrigin = h.gameObject.transform.position;
		mFlickButton = h;
	}

	private float DragOffsetX()
	{
		return host.transform.position.x - mWorldOrigin.x;
	}

	public void UpdateAlpha()
	{
		float f = DragOffsetX();
		float value = 1f - Mathf.Abs(f) / (mFlickButton.Range * mFlickButton.WrapRangeMultiplier);
		value = Mathf.Clamp01(value);
		Color color = mFlickButton.Color;
		color.a = value;
		mFlickButton.Color = color;
		if ((bool)mFlickButton.spriteText)
		{
			mFlickButton.spriteText.Color = color;
		}
	}

	public void SliderDragDropDelegate(EZDragDropParams parms)
	{
		UpdateAlpha();
	}

	public void ForceAnimFromLeft()
	{
		UIFlickButton uIFlickButton = mFlickButton;
		Vector3 position = host.transform.position;
		position.x -= uIFlickButton.Range + uIFlickButton.Range * uIFlickButton.WrapRangeMultiplier;
		host.transform.position = position;
		base.IsDragging = true;
		CancelDrag();
	}

	public void ForceAnimFromRight()
	{
		UIFlickButton uIFlickButton = mFlickButton;
		Vector3 position = host.transform.position;
		position.x += uIFlickButton.Range + uIFlickButton.Range * uIFlickButton.WrapRangeMultiplier;
		host.transform.position = position;
		base.IsDragging = true;
		CancelDrag();
	}

	private void SliderDragUpdatePosition(POINTER_INFO ptr)
	{
		DefaultDragUpdatePosition(ptr);
		UIFlickButton uIFlickButton = mFlickButton;
		Vector3 position = host.transform.position;
		position.y = mWorldOrigin.y;
		host.transform.position = position;
		Vector3 vector = host.transform.position - mWorldOrigin;
		if (vector.x >= uIFlickButton.Range)
		{
			uIFlickButton.HitRightButton();
			position.x -= uIFlickButton.Range + uIFlickButton.Range * uIFlickButton.WrapRangeMultiplier;
			host.transform.position = position;
			base.IsDragging = false;
		}
		else if (vector.x <= 0f - uIFlickButton.Range)
		{
			uIFlickButton.HitLeftButton();
			position.x += uIFlickButton.Range + uIFlickButton.Range * uIFlickButton.WrapRangeMultiplier;
			host.transform.position = position;
			base.IsDragging = false;
		}
	}
}
