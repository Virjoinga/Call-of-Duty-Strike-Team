using UnityEngine;

public class FriendlyBlip : HudBlipIcon
{
	private enum SpriteFrame
	{
		Default = 0,
		Overwatch = 1
	}

	private PackedSprite mIcon;

	public override void Start()
	{
		base.Start();
		mIcon = GetComponent<PackedSprite>();
		mIcon.Hide(true);
		mIcon.SetColor(ColourChart.FriendlyBlip);
		mIcon.SetFrame(0, 0);
		mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		if (ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			IsAllowedInFirstPerson = true;
		}
	}

	public override void LateUpdate()
	{
		if (Target == null)
		{
			Object.Destroy(base.gameObject);
		}
		else if (Target.GetComponent<Actor>().OnScreen)
		{
			base.LateUpdate();
		}
		else
		{
			base.transform.position = Target.position + WorldOffset;
		}
	}

	public override void JustGoneOffScreen()
	{
		if (mIcon != null)
		{
			mIcon.Hide(true);
		}
	}

	public override void JustComeOnScreen()
	{
		if (mIcon != null)
		{
			mIcon.Hide(false);
		}
	}

	protected override void SwitchToStrategyView()
	{
		base.SwitchToStrategyView();
	}

	protected override void SwitchToGameplayView()
	{
		mIcon.SetFrame(0, 0);
		mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
		base.SwitchToGameplayView();
	}
}
