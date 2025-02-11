using UnityEngine;

public class DropZoneBlip : HudBlipIcon
{
	public PackedSprite EnabledIcon;

	public PackedSprite LockedIcon;

	private bool mLocked;

	public override void Start()
	{
		base.Start();
		SwitchOn();
	}

	public override void UpdateOffScreen()
	{
		base.transform.position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(base.ScreenPos);
	}

	public override void JustGoneOffScreen()
	{
		EnabledIcon.Hide(true);
		LockedIcon.Hide(true);
	}

	public override void JustComeOnScreen()
	{
		PackedSprite packedSprite = ((!mLocked) ? EnabledIcon : LockedIcon);
		PackedSprite packedSprite2 = (mLocked ? EnabledIcon : LockedIcon);
		if (packedSprite.IsHidden())
		{
			AnimateOn(packedSprite);
			packedSprite2.Hide(true);
		}
	}

	protected override void SwitchToStrategyView()
	{
		SwitchOn();
	}

	protected override void SwitchToGameplayView()
	{
		SwitchOff();
	}

	public void SetEmpty()
	{
		LockedIcon.Hide(true);
	}

	public void SetLocked()
	{
		AnimateOn(LockedIcon);
		EnabledIcon.Hide(true);
		mLocked = true;
	}

	public void SetUnlocked()
	{
		AnimateOn(EnabledIcon);
		LockedIcon.Hide(true);
		mLocked = false;
	}

	private void AnimateOn(PackedSprite sprite)
	{
		if (sprite != null)
		{
			sprite.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			sprite.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.5f, 0f, EaseType.easeOutBounce);
			sprite.Hide(false);
		}
	}
}
