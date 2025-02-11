using UnityEngine;

public class UIWeaponFlickButton : UIFlickButton
{
	private enum FadeState
	{
		Transparent = 0,
		FadingIn = 1,
		Opaque = 2,
		FadingOut = 3
	}

	private float TimeWentActive;

	private float TimeWentInactive;

	private FadeState mFadeState;

	public override void SetControlState(CONTROL_STATE s)
	{
		if (s == CONTROL_STATE.ACTIVE && base.controlState != CONTROL_STATE.ACTIVE)
		{
			TimeWentActive = Time.time;
		}
		else
		{
			TimeWentInactive = Time.time;
		}
		base.SetControlState(s, false);
	}

	private void LateUpdate()
	{
		if (base.controlState == CONTROL_STATE.ACTIVE)
		{
			if (mFadeState != FadeState.Opaque)
			{
				float num = Time.time - TimeWentActive;
				num -= 0.1f;
				if (num > 0f || dragDropHelper.IsDragging)
				{
					mFadeState = FadeState.FadingIn;
				}
			}
		}
		else
		{
			float num2 = Time.time - TimeWentInactive;
			num2 -= 1f;
			if (num2 > 0f)
			{
				mFadeState = FadeState.FadingOut;
			}
		}
		switch (mFadeState)
		{
		case FadeState.FadingIn:
		{
			Color color = base.Color;
			if (color.a < 1f)
			{
				color.a += Time.deltaTime * 4f;
			}
			if (color.a > 1f)
			{
				color.a = 1f;
				mFadeState = FadeState.Opaque;
			}
			if (dragDropHelper.IsDragging)
			{
				color.a = 1f;
				mFadeState = FadeState.Opaque;
			}
			base.Color = color;
			break;
		}
		case FadeState.FadingOut:
		{
			Color color = base.Color;
			if (color.a > 0f)
			{
				color.a -= Time.deltaTime * 4f;
			}
			if (color.a < 0f)
			{
				color.a = 0f;
				mFadeState = FadeState.Opaque;
			}
			base.Color = color;
			break;
		}
		case FadeState.Opaque:
			break;
		}
	}

	private void OnEZTranslated()
	{
		mDragDropFlickHelper.UpdateAlpha();
		TimeWentInactive = Time.time;
	}
}
