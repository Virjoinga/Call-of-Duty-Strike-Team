using UnityEngine;

public class EnemyRocketBlip : HudBlipIcon
{
	private enum SpriteFrame
	{
		Default = 0,
		Overwatch = 2
	}

	private PackedSprite mIcon;

	public bool DoBaseOnOffScreenControl { private get; set; }

	public override void Start()
	{
		bool flag = true;
		if (DoBaseOnOffScreenControl)
		{
			flag = base.IsSwitchedOn;
		}
		base.Start();
		mIcon = GetComponent<PackedSprite>();
		mIcon.SetColor(ColourChart.EnemyBlip);
		if (ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			IsAllowedInFirstPerson = true;
		}
		else
		{
			Object.Destroy(this);
		}
		if (DoBaseOnOffScreenControl && !flag)
		{
			SwitchOff();
		}
	}

	public void OnDestroy()
	{
		Object.Destroy(mIcon);
	}

	public override void LateUpdate()
	{
		if (Target == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		ForceBlipUpdate();
		base.LateUpdate();
	}

	public override void UpdateOffScreen()
	{
		base.transform.position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(base.ScreenPos);
	}

	public override void JustGoneOffScreen()
	{
		if (DoBaseOnOffScreenControl)
		{
			base.JustGoneOffScreen();
		}
	}

	public override void JustComeOnScreen()
	{
		if (DoBaseOnOffScreenControl)
		{
			base.JustComeOnScreen();
		}
	}

	public void SetToFriendlyBlip()
	{
		mIcon.SetColor(ColourChart.FriendlyBlip);
		m_ViewCone.CurrentColour = ColourChart.ViewConeFriendly;
	}

	public void SetToEnemyBlip()
	{
		mIcon.SetColor(ColourChart.EnemyBlip);
		m_ViewCone.CurrentColour = ColourChart.ViewConeEnemy;
	}
}
