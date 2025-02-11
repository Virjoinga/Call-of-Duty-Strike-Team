using System;
using UnityEngine;

public class EnemyBlip : HudBlipIcon
{
	private enum SpriteFrame
	{
		Default = 0,
		Targeted = 1,
		HeavyWeapon = 2,
		Suppressed = 3,
		VeteranInvisible = 4
	}

	private const float kAttackFlashDuration = 1f;

	public PackedSprite AimShotUL;

	public PackedSprite AimShotUR;

	public PackedSprite AimShotBL;

	public PackedSprite AimShotBR;

	public float AimShotOffset = 0.7f;

	private float AimShotAlpha;

	private float mCachedAimedForTime;

	private float mCurrentAimShotOffset;

	private float mFailAimShotTimer;

	private PackedSprite mIcon;

	private float mReturnToDefaultTimer;

	private bool mIsInOverwatch;

	private bool mHeavyWeightEnemyIcon;

	private static float kTimeBeforeReturnToDefault = 1.5f;

	private bool mHasPassedStart;

	public DebugTextParticleBlip DebugBlipText;

	private float mAttackedAtTime;

	private float mScaleTime;

	private float mAimedFor;

	private int DefaultFrame()
	{
		if (mIsInOverwatch || ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			return mHeavyWeightEnemyIcon ? 2 : 0;
		}
		return 4;
	}

	public override void Start()
	{
		base.Start();
		mIcon = GetComponent<PackedSprite>();
		mIcon.Hide(true);
		mIcon.SetColor(ColourChart.EnemyBlip);
		mReturnToDefaultTimer = 0f;
		mIcon.SetFrame(0, DefaultFrame());
		mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
		SetupViewCone();
		HideAimShot(true);
		if (ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			IsAllowedInFirstPerson = true;
		}
		if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera)
		{
			SwitchToStrategyView();
		}
		mHasPassedStart = true;
		DebugBlipText = null;
		if (DebugBlipText != null)
		{
			RealCharacter component = Target.GetComponent<RealCharacter>();
			if (component != null)
			{
				component.myActor.health.OnHealthChange += OnHealthChange;
			}
		}
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
	}

	public void OnDestroy()
	{
		if (DebugBlipText != null && Target != null)
		{
			RealCharacter component = Target.GetComponent<RealCharacter>();
			if (component != null && component.myActor != null && component.myActor.health != null)
			{
				component.myActor.health.OnHealthChange -= OnHealthChange;
			}
		}
	}

	public override void LateUpdate()
	{
		if (Target == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		RealCharacter component = Target.GetComponent<RealCharacter>();
		if (component == null || component.myActor.OnScreen)
		{
			base.LateUpdate();
			float num = Time.realtimeSinceStartup - mAttackedAtTime;
			if (num <= 1f && !AimShotUL.gameObject.activeSelf)
			{
				mScaleTime += Time.deltaTime;
				float num2 = 5f;
				float num3 = 0.5f;
				float num4 = 1f + Mathf.Sin(mScaleTime * ((float)Math.PI * 2f) * num2) * num3;
				base.gameObject.transform.localScale = new Vector3(num4, num4, num4);
			}
			else
			{
				base.gameObject.transform.localScale = Vector3.one;
			}
		}
		else
		{
			base.transform.position = Target.position + WorldOffset;
		}
		if (!mIsInOverwatch && mReturnToDefaultTimer > 0f)
		{
			mReturnToDefaultTimer -= Time.deltaTime;
			if (mReturnToDefaultTimer <= 0f)
			{
				mIcon.SetFrame(0, DefaultFrame());
				FailAimShot();
			}
		}
		TrackingRobotRealCharacter trackingRobotRealCharacter = component as TrackingRobotRealCharacter;
		if (trackingRobotRealCharacter != null)
		{
			HackableObject component2 = trackingRobotRealCharacter.GetComponent<HackableObject>();
			if (component2 != null && component2.FullyHacked)
			{
				m_ViewCone.CurrentColour = ColourChart.ViewConeCamera;
			}
		}
		if (component != null)
		{
			UpdateViewCone();
		}
		UpdateAimShot();
	}

	public void AttackPulse()
	{
		if (Time.realtimeSinceStartup - mAttackedAtTime > 1f)
		{
			mScaleTime = 0f;
		}
		mAttackedAtTime = Time.realtimeSinceStartup;
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

	private void UpdateAimShot()
	{
		if (mFailAimShotTimer >= 0f)
		{
			mFailAimShotTimer += Time.deltaTime;
			ColourAimShot(Color.black.Alpha(1f - mFailAimShotTimer / 0.25f));
			mCurrentAimShotOffset += Time.deltaTime * 2f;
			PositionAimShot(mCurrentAimShotOffset);
			if (mFailAimShotTimer > 0.25f)
			{
				HideAimShot(true);
				mReturnToDefaultTimer = 0.01f;
			}
		}
	}

	private void ColourAimShot(Color col)
	{
		AimShotUL.SetColor(col);
		AimShotUR.SetColor(col);
		AimShotBL.SetColor(col);
		AimShotBR.SetColor(col);
	}

	private void PositionAimShot(float offset)
	{
		mCurrentAimShotOffset = offset;
		AimShotUL.gameObject.transform.localPosition = new Vector3(0f - offset, offset, 0f);
		AimShotUR.gameObject.transform.localPosition = new Vector3(offset, offset, 0f);
		AimShotBL.gameObject.transform.localPosition = new Vector3(0f - offset, 0f - offset, 0f);
		AimShotBR.gameObject.transform.localPosition = new Vector3(offset, 0f - offset, 0f);
	}

	public override void Targetted(float aimedFor, float takeShotAfterTime)
	{
		if (mIsInOverwatch || (aimedFor < mAimedFor && aimedFor != -1f))
		{
			return;
		}
		mAimedFor = aimedFor;
		if (mIcon != null && aimedFor != -1f)
		{
			mReturnToDefaultTimer = kTimeBeforeReturnToDefault;
			mIcon.SetFrame(0, 1);
		}
		if (mFailAimShotTimer >= 0f)
		{
			HideAimShot(true);
		}
		if (aimedFor >= 0f)
		{
			HideAimShot(false);
			float num = 1f - aimedFor / takeShotAfterTime;
			float offset = AimShotOffset * num;
			PositionAimShot(offset);
			Color color = ColourChart.AimShotActive;
			if (aimedFor == mCachedAimedForTime)
			{
				color = ColourChart.AimShotIdle;
			}
			if (AimShotAlpha < 1f)
			{
				AimShotAlpha += Time.deltaTime * 4f;
				AimShotAlpha = Mathf.Clamp01(AimShotAlpha);
				color = color.Alpha(AimShotAlpha);
			}
			ColourAimShot(color);
		}
		else
		{
			FailAimShot();
		}
	}

	public override void Suppressed()
	{
		if (!mIsInOverwatch && mIcon != null)
		{
			mReturnToDefaultTimer = kTimeBeforeReturnToDefault;
			mIcon.SetFrame(0, 3);
		}
	}

	protected override void SwitchToStrategyView()
	{
		mIsInOverwatch = true;
		mIcon.SetFrame(0, DefaultFrame());
		base.SwitchToStrategyView();
	}

	protected override void SwitchToGameplayView()
	{
		mIsInOverwatch = false;
		mIcon.SetFrame(0, DefaultFrame());
		mIcon.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
		base.SwitchToGameplayView();
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

	public void SetToSentryGunBlip()
	{
		m_ViewCone.CurrentColour = ColourChart.ViewConeSentryGun;
	}

	public void UseHeavyWeightEnemyIcon(bool useHeavyIcon)
	{
		mHeavyWeightEnemyIcon = useHeavyIcon;
		if (mHasPassedStart && !AimShotUL.gameObject.activeSelf)
		{
			mIcon.SetFrame(0, DefaultFrame());
		}
	}

	private void FailAimShot()
	{
		if (AimShotUL.gameObject.activeSelf)
		{
			mFailAimShotTimer = 0f;
		}
	}

	private void HideAimShot(bool hide)
	{
		if (AimShotUL.gameObject.activeSelf == hide)
		{
			AimShotUL.gameObject.SetActive(!hide);
			AimShotUR.gameObject.SetActive(!hide);
			AimShotBL.gameObject.SetActive(!hide);
			AimShotBR.gameObject.SetActive(!hide);
			AimShotAlpha = 0f;
			mCachedAimedForTime = 0f;
			mFailAimShotTimer = -1f;
			mAimedFor = 0f;
		}
	}
}
