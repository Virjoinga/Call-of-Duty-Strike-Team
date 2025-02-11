using System;
using UnityEngine;

public class HudBlipIcon : MonoBehaviour
{
	public Transform Target;

	public Transform OffsetTarget;

	public Vector3 WorldOffset = new Vector3(0f, 2.5f, 0f);

	public Vector3 OffsetTargetOffset = new Vector3(0f, 0.5f, 0f);

	private bool mClampToEdgeOfScreen;

	public Vector2 ScreenEdgeOffsetMin = new Vector2(0f, 0f);

	public Vector2 ScreenEdgeOffsetMax = new Vector2(0f, 0f);

	protected float FinalPositionZOffset = 11.5f;

	private bool mIsOnScreen;

	private Vector3 mScreenPos;

	private bool mIsSwitchedOn;

	private static bool mIsInCutscene;

	private bool mVisible = true;

	private bool mCachedIsSwitchedOn;

	private bool mCachedIsClampedToScreenEdge;

	public bool IsAllowedInFirstPerson;

	public bool IsAllowedInStrateryView;

	private float? mCurrentRot;

	public Camera CameraOverride;

	public ViewConeEffect m_ViewCone;

	public float ZFarClip = 1000f;

	public float DistToFppSqrMag;

	public static CameraManager mCamManRef;

	private float mGuiCamFar;

	private Camera mGuiCamRef;

	private int mNextUpdate;

	public Vector3 OriginalWorldOffset { get; private set; }

	public bool Visible
	{
		get
		{
			return mVisible;
		}
		set
		{
			mVisible = value;
		}
	}

	public bool ClampToEdgeOfScreen
	{
		get
		{
			return mClampToEdgeOfScreen;
		}
		set
		{
			mClampToEdgeOfScreen = (mCachedIsClampedToScreenEdge = value);
			if ((bool)CameraManager.Instance && CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera)
			{
				mClampToEdgeOfScreen = false;
			}
		}
	}

	public bool IsOnScreen
	{
		get
		{
			return mIsOnScreen;
		}
	}

	public bool IsSwitchedOn
	{
		get
		{
			return mIsSwitchedOn;
		}
	}

	public static bool AreAllSetForCutscene
	{
		get
		{
			return mIsInCutscene;
		}
	}

	public Vector3 ScreenPos
	{
		get
		{
			return mScreenPos;
		}
		protected set
		{
			mScreenPos = value;
		}
	}

	public static void ClearCutsceneFlag()
	{
		mIsInCutscene = false;
	}

	public void ForceBlipUpdate()
	{
		mNextUpdate = 0;
	}

	public virtual void Start()
	{
		mIsOnScreen = false;
		OriginalWorldOffset = WorldOffset;
		if (mIsInCutscene)
		{
			mCachedIsSwitchedOn = true;
			mIsSwitchedOn = false;
			JustSwitchedOff();
		}
		else
		{
			mIsSwitchedOn = true;
		}
		if (TBFUtils.IsRetinaHdDevice())
		{
			ScreenEdgeOffsetMin *= 2f;
			ScreenEdgeOffsetMax *= 2f;
		}
		if (mCamManRef == null)
		{
			mCamManRef = CameraManager.Instance;
		}
		if (GUISystem.Instance != null)
		{
			mGuiCamFar = GUISystem.Instance.m_guiCamera.farClipPlane;
			mGuiCamRef = GUISystem.Instance.m_guiCamera;
		}
	}

	public virtual void LateUpdate()
	{
		if (Target == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			if (mNextUpdate > Time.frameCount)
			{
				return;
			}
			Vector3 vector = Target.position + WorldOffset;
			if (OffsetTarget != null && OffsetTarget.position.y < 1000f)
			{
				vector = OffsetTarget.position + OffsetTargetOffset;
			}
			if (GameController.Instance != null && GameController.Instance.IsFirstPerson && GameController.Instance.mFirstPersonActor != null)
			{
				DistToFppSqrMag = (GameController.Instance.mFirstPersonActor.GetPosition() - vector).sqrMagnitude;
			}
			if ((bool)CameraOverride)
			{
				mScreenPos = CameraOverride.WorldToScreenPoint(vector);
			}
			else if (mCamManRef != null)
			{
				mScreenPos = mCamManRef.WorldToScreenPoint(vector);
			}
			if (mScreenPos.z < 0f)
			{
				mScreenPos.y = 0f - mScreenPos.y;
				mScreenPos.x = 0f - mScreenPos.x;
			}
			bool flag = mIsOnScreen;
			mIsOnScreen = false;
			if (mScreenPos.y > ScreenEdgeOffsetMin.y && mScreenPos.y < (float)Screen.height - ScreenEdgeOffsetMax.y && mScreenPos.x > ScreenEdgeOffsetMin.x && mScreenPos.x < (float)Screen.width - ScreenEdgeOffsetMax.x)
			{
				mIsOnScreen = true;
			}
			if (mScreenPos.z > ZFarClip || mScreenPos.z < 0f)
			{
				mIsOnScreen = false;
			}
			mScreenPos.z = Mathf.Clamp(mScreenPos.z, 0f, mGuiCamFar - 1f);
			if (!mVisible || !mIsSwitchedOn || mIsInCutscene)
			{
				return;
			}
			if (GameController.Instance != null && GameController.Instance.IsFirstPerson && !IsAllowedInFirstPerson)
			{
				mIsOnScreen = false;
			}
			if (flag != mIsOnScreen)
			{
				if (mIsOnScreen)
				{
					JustComeOnScreen();
				}
				else
				{
					JustGoneOffScreen();
				}
			}
			if (mIsOnScreen)
			{
				UpdateOnScreen();
			}
			else
			{
				if (ClampToEdgeOfScreen)
				{
					float num = (float)Screen.width * 0.5f;
					float num2 = (float)Screen.height * 0.5f;
					Vector3 vector2 = new Vector3(num, num2, 0f);
					mScreenPos -= vector2;
					mScreenPos.x *= num;
					mScreenPos.y *= num;
					float num3 = num - ScreenEdgeOffsetMax.x;
					float num4 = num2 - ScreenEdgeOffsetMax.y;
					float num5 = 0f - num + ScreenEdgeOffsetMin.x;
					float num6 = 0f - num2 + ScreenEdgeOffsetMin.y;
					if (mScreenPos.x > num3)
					{
						float num7 = num3 / mScreenPos.x;
						mScreenPos.y *= num7;
						mScreenPos.x = num3;
					}
					else if (mScreenPos.x < num5)
					{
						float num8 = num5 / mScreenPos.x;
						mScreenPos.y *= num8;
						mScreenPos.x = num5;
					}
					if (mScreenPos.y > num4)
					{
						float num9 = num4 / mScreenPos.y;
						mScreenPos.x *= num9;
						mScreenPos.y = num4;
					}
					else if (mScreenPos.y < num6)
					{
						float num10 = num6 / mScreenPos.y;
						mScreenPos.x *= num10;
						mScreenPos.y = num6;
					}
					mScreenPos += vector2;
				}
				UpdateOffScreen();
			}
			if (!mIsOnScreen && !OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.SmoothOffScreenBlips))
			{
				mNextUpdate = Time.frameCount + UnityEngine.Random.Range(5, 15);
			}
		}
	}

	public virtual void UpdateOnScreen()
	{
		Vector3 vector = mGuiCamRef.ScreenToWorldPoint(ScreenPos);
		base.transform.position = new Vector3(vector.x, vector.y, FinalPositionZOffset);
	}

	public virtual void UpdateOffScreen()
	{
	}

	public virtual void JustGoneOffScreen()
	{
		base.gameObject.CheckedScaleTo(Vector3.zero, 0.2f, 0f);
	}

	public virtual void JustComeOnScreen()
	{
		base.gameObject.CheckedScaleTo(Vector3.one, 0.2f, 0f);
	}

	public virtual void JustSwitchedOff()
	{
		JustGoneOffScreen();
	}

	public virtual void JustSwitchedOn()
	{
		if (IsOnScreen)
		{
			JustComeOnScreen();
		}
	}

	public virtual void Targetted(float aimedFor, float takeShotAfterTime)
	{
	}

	public virtual void Suppressed()
	{
	}

	public void SwitchOff()
	{
		if (mIsInCutscene)
		{
			mCachedIsSwitchedOn = (mIsSwitchedOn = false);
		}
		else if (mIsSwitchedOn)
		{
			mIsSwitchedOn = false;
			JustSwitchedOff();
		}
	}

	public void SwitchOn()
	{
		if (mIsInCutscene)
		{
			mCachedIsSwitchedOn = (mIsSwitchedOn = true);
		}
		else if (!mIsSwitchedOn)
		{
			mIsSwitchedOn = true;
			JustSwitchedOn();
		}
	}

	private void CacheForCutscene()
	{
		mCachedIsSwitchedOn = IsSwitchedOn;
		if (mCachedIsSwitchedOn)
		{
			mIsSwitchedOn = false;
			JustSwitchedOff();
		}
	}

	private void RestoreFromCacheAfterCutscene()
	{
		if (mCachedIsSwitchedOn)
		{
			mIsSwitchedOn = true;
			JustSwitchedOn();
		}
	}

	protected virtual void SwitchToStrategyView()
	{
		mCachedIsClampedToScreenEdge = ClampToEdgeOfScreen;
		if (ClampToEdgeOfScreen)
		{
			mClampToEdgeOfScreen = false;
		}
	}

	protected virtual void SwitchToGameplayView()
	{
		ClampToEdgeOfScreen = mCachedIsClampedToScreenEdge;
		if (!IsOnScreen && ClampToEdgeOfScreen && IsSwitchedOn)
		{
			JustGoneOffScreen();
		}
	}

	public static void SwitchOffForCutscene()
	{
		if (!mIsInCutscene)
		{
			HudBlipIcon[] array = UnityEngine.Object.FindObjectsOfType(typeof(HudBlipIcon)) as HudBlipIcon[];
			HudBlipIcon[] array2 = array;
			foreach (HudBlipIcon hudBlipIcon in array2)
			{
				hudBlipIcon.CacheForCutscene();
			}
			mIsInCutscene = true;
		}
	}

	public static void SwitchOnAfterCutscene()
	{
		if (mIsInCutscene)
		{
			mIsInCutscene = false;
			HudBlipIcon[] array = UnityEngine.Object.FindObjectsOfType(typeof(HudBlipIcon)) as HudBlipIcon[];
			HudBlipIcon[] array2 = array;
			foreach (HudBlipIcon hudBlipIcon in array2)
			{
				hudBlipIcon.RestoreFromCacheAfterCutscene();
			}
		}
	}

	public static void SwitchAllToStrategyView()
	{
		HudBlipIcon[] array = UnityEngine.Object.FindObjectsOfType(typeof(HudBlipIcon)) as HudBlipIcon[];
		HudBlipIcon[] array2 = array;
		foreach (HudBlipIcon hudBlipIcon in array2)
		{
			hudBlipIcon.SwitchToStrategyView();
		}
	}

	public static void SwitchAllToGameplayView()
	{
		HudBlipIcon[] array = UnityEngine.Object.FindObjectsOfType(typeof(HudBlipIcon)) as HudBlipIcon[];
		HudBlipIcon[] array2 = array;
		foreach (HudBlipIcon hudBlipIcon in array2)
		{
			hudBlipIcon.SwitchToGameplayView();
		}
		TriggerMessageEventManager.Instance().NotifyEvent(TriggerMessageEvent.Type.PlayView_Entry);
	}

	protected void SetupViewCone()
	{
		float num = 90f;
		float num2 = 6f;
		num2 /= Mathf.Cos((float)Math.PI / 180f * num * 0.5f);
		Vector3 vector = Vector3.forward * num2;
		Quaternion quaternion = Quaternion.Euler(0f, (0f - num) * 0.5f, 0f);
		vector = quaternion * vector;
		m_ViewCone.Setup(vector.x, vector.z);
		m_ViewCone.gameObject.SetActive(false);
	}

	protected void UpdateViewCone()
	{
		bool flag = (bool)OverwatchController.Instance && OverwatchController.Instance.Active;
		if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera && !CameraManager.Instance.IsSwitching && !flag && mIsOnScreen)
		{
			if (!m_ViewCone.gameObject.activeInHierarchy)
			{
				m_ViewCone.gameObject.SetActive(true);
			}
			AwarenessComponent component = Target.GetComponent<AwarenessComponent>();
			Vector3 vector = GUISystem.Instance.m_guiCamera.WorldToViewportPoint(Target.transform.position);
			Vector3 vector2 = GUISystem.Instance.m_guiCamera.WorldToViewportPoint(Target.transform.position + Vector3.right);
			Vector3 vector3 = CameraManager.Instance.StrategyCamera.WorldToViewportPoint(Target.transform.position);
			Vector3 vector4 = CameraManager.Instance.StrategyCamera.WorldToViewportPoint(Target.transform.position + Vector3.right);
			float magnitude = (vector2 - vector).magnitude;
			float magnitude2 = (vector4 - vector3).magnitude;
			float num = magnitude2 / magnitude;
			Vector3 vector5 = new Vector3(component.LookDirection.x, 0f, component.LookDirection.z);
			float num2 = Vector3.Angle(Vector3.forward, vector5);
			if (Vector3.Dot(Vector3.right, vector5) >= 0f)
			{
				num2 = 0f - num2;
			}
			if (!mCurrentRot.HasValue)
			{
				mCurrentRot = num2;
			}
			else
			{
				mCurrentRot = Mathf.LerpAngle(mCurrentRot.Value, num2, 0.1f);
			}
			m_ViewCone.transform.localEulerAngles = new Vector3(0f, 0f, mCurrentRot.Value);
			m_ViewCone.transform.localScale = new Vector3(num, num, num);
		}
		else if (m_ViewCone.gameObject.activeInHierarchy)
		{
			m_ViewCone.gameObject.SetActive(false);
		}
	}
}
