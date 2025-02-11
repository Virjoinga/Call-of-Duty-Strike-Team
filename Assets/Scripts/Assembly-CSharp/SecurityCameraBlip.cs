using UnityEngine;

public class SecurityCameraBlip : HudBlipIcon
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
		if (mIcon != null)
		{
			mIcon.Hide(true);
			mIcon.SetColor(ColourChart.EnemyBlip);
			mIcon.SetFrame(0, 0);
			mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		}
		SetupViewCone();
		if (ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			IsAllowedInFirstPerson = true;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	public override void LateUpdate()
	{
		if (Target == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (Target.GetComponent<Actor>().OnScreen)
		{
			base.LateUpdate();
		}
		else
		{
			base.transform.position = Target.position + WorldOffset;
		}
		UpdateViewCone();
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
		if (mIcon != null)
		{
			mIcon.SetFrame(0, 1);
			mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		}
		base.SwitchToStrategyView();
	}

	protected override void SwitchToGameplayView()
	{
		if (mIcon != null)
		{
			mIcon.SetFrame(0, 0);
			mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
		}
		base.SwitchToGameplayView();
	}
}
