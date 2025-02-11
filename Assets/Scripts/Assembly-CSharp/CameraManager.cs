using System;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public enum ActiveCameraType
	{
		PlayCamera = 0,
		StrategyCamera = 1
	}

	private static CameraManager smInstance;

	public ActiveCameraType ActiveCamera;

	public Camera PlayCamera;

	public Camera StrategyCamera;

	public Camera SniperCamera;

	public float SwitchTime = 2f;

	private bool mIsSwitching;

	private int mViewModelMask;

	private float mFogDensity;

	private float mFogDensityOverride;

	private float mFogEndOverride;

	private float mFogStartOverride;

	private GameObject mDirLight;

	private OverWatchShaderInterface mOvWaShInt;

	private float mSniperPlaneSize = 0.8f;

	private bool mSeparateViewModelBands;

	public static CameraManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public BloodEffect BloodEffect
	{
		get
		{
			if (GameController.Instance != null && GameController.Instance.AimimgDownScopeThisFrame)
			{
				return SniperCamera.GetComponent<BloodEffect>();
			}
			return PlayCamera.GetComponent<BloodEffect>();
		}
	}

	public RenderTexture SniperTexture { get; private set; }

	public CameraController PlayCameraController
	{
		get
		{
			return PlayCamera.GetComponent<CameraController>();
		}
	}

	public CameraController StrategyCameraController
	{
		get
		{
			return StrategyCamera.GetComponent<CameraController>();
		}
	}

	public bool IsSwitching
	{
		get
		{
			return mIsSwitching;
		}
	}

	public float FogOverride
	{
		set
		{
			mFogDensityOverride = value;
			RenderSettings.fogDensity = mFogDensityOverride;
		}
	}

	public Camera CurrentCamera
	{
		get
		{
			switch (ActiveCamera)
			{
			case ActiveCameraType.PlayCamera:
				return PlayCamera;
			case ActiveCameraType.StrategyCamera:
				return StrategyCamera;
			default:
				TBFAssert.DoAssert(false, "no camera?");
				return null;
			}
		}
	}

	public void OnActivateSection()
	{
	}

	public void SetStratCamEnable(bool bEnable)
	{
		if (StrategyCamera != null)
		{
			StrategyCamera.enabled = bEnable;
		}
	}

	public void SetViewModelCameraDepthClear(bool clear)
	{
		mSeparateViewModelBands = clear;
	}

	public void ResetFog()
	{
		RenderSettings.fogDensity = mFogDensity;
		mFogDensityOverride = mFogDensity;
		RenderSettings.fogEndDistance = mFogEndOverride;
		RenderSettings.fogStartDistance = mFogStartOverride;
		RenderSettings.fog = true;
		SetFogPlayCameraPlane();
	}

	private void SetFogPlayCameraPlane()
	{
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.FarClipAtFogDistance))
		{
			float fogStartDistance = RenderSettings.fogStartDistance;
			float num = RenderSettings.fogEndDistance;
			float num2 = ((!(MissionSetup.Instance != null)) ? 0f : MissionSetup.Instance.MinimumVisibility);
			if (num2 > 0f)
			{
				float num3 = num - fogStartDistance;
				float num4 = 1f - num2;
				float num5 = num4 / num3;
				num += num2 / num5;
			}
			PlayCamera.farClipPlane = num;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple CameraManager");
		}
		smInstance = this;
		mFogDensity = RenderSettings.fogDensity;
		mFogDensityOverride = RenderSettings.fogDensity;
		mFogEndOverride = RenderSettings.fogEndDistance;
		mFogStartOverride = RenderSettings.fogStartDistance;
		SniperTexture = new RenderTexture(512, 512, 24);
		SniperTexture.name = "Sniper";
		SniperScope component = PlayCamera.GetComponent<SniperScope>();
		if (component != null)
		{
			GameObject gameObject = new GameObject("SniperPlane");
			gameObject.transform.ParentAndZeroLocalPositionAndRotation(SniperCamera.transform);
			gameObject.transform.localPosition = Vector3.forward;
			gameObject.layer = LayerMask.NameToLayer("SniperScope");
			Mesh mesh = new Mesh();
			mesh.name = "SniperPlane";
			mesh.vertices = new Vector3[4]
			{
				new Vector3(0f - mSniperPlaneSize, mSniperPlaneSize, 0f),
				new Vector3(mSniperPlaneSize, mSniperPlaneSize, 0f),
				new Vector3(0f - mSniperPlaneSize, 0f - mSniperPlaneSize, 0f),
				new Vector3(mSniperPlaneSize, 0f - mSniperPlaneSize, 0f)
			};
			mesh.uv = new Vector2[4]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			};
			mesh.triangles = new int[6] { 0, 1, 2, 2, 1, 3 };
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = component.ScopeMaterial;
			component.ScopeMaterial = meshRenderer.material;
		}
		SetViewModelCameraDepthClear(true);
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void Start()
	{
		CreateDirectionalShadowLight();
		mViewModelMask = 1 << LayerMask.NameToLayer("ViewModel");
		mOvWaShInt = StrategyCamera.GetComponent<OverWatchShaderInterface>();
		if (ActiveCamera == ActiveCameraType.PlayCamera)
		{
			CameraController component = StrategyCamera.GetComponent<CameraController>();
			component.StartCamera.AllowInput(false);
			StrategyCamera.gameObject.SetActive(false);
			RenderSettings.fogDensity = mFogDensityOverride;
			RenderSettings.fogStartDistance = mFogStartOverride;
			RenderSettings.fogEndDistance = mFogEndOverride;
			RenderSettings.fog = true;
			SetFogPlayCameraPlane();
			CameraController component2 = PlayCamera.GetComponent<CameraController>();
			component2.CurrentCameraBase = component2.StartCamera;
		}
		else if (ActiveCamera == ActiveCameraType.StrategyCamera)
		{
			CameraController component3 = PlayCamera.GetComponent<CameraController>();
			component3.StartCamera.AllowInput(false);
			PlayCamera.gameObject.SetActive(false);
			CameraController component4 = StrategyCamera.GetComponent<CameraController>();
			component4.CurrentCameraBase = component4.StartCamera;
			RenderSettings.fogDensity = 0f;
			RenderSettings.fogStartDistance = float.MaxValue;
			RenderSettings.fogEndDistance = float.MaxValue;
			RenderSettings.fog = false;
			if (mDirLight != null)
			{
				Light component5 = mDirLight.GetComponent<Light>();
				component5.shadows = LightShadows.None;
			}
		}
		TimeManager.instance.SlowDownTime(0f, TimeManager.instance.StopTimeModeSpeed);
		TriggerMessageEventManager.Instance().NotifyEvent(TriggerMessageEvent.Type.StrategyView_Entry);
	}

	public void EnableOverwatchShader()
	{
		if ((bool)mOvWaShInt)
		{
			mOvWaShInt.enabled = true;
		}
	}

	private void SafeSetShaderMaxLOD(string shaderName, int maxLOD)
	{
		Shader shader = Shader.Find(shaderName);
		if (shader != null)
		{
			shader.maximumLOD = maxLOD;
		}
		else
		{
			Debug.LogWarning("SHADER NOT FOUND: " + shaderName);
		}
	}

	private void CreateDirectionalShadowLight()
	{
		GameObject gameObject = GameObject.Find("Directional light");
		if (gameObject == null)
		{
			return;
		}
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.DetailedShadows))
		{
			mDirLight = new GameObject();
			mDirLight.AddComponent<Light>();
			Light component = mDirLight.GetComponent<Light>();
			component.type = LightType.Directional;
			component.shadows = LightShadows.Hard;
			component.shadowStrength = 0.3f;
			component.cullingMask = 1 << LayerMask.NameToLayer("GlobalViewable");
			component.cullingMask |= 1 << LayerMask.NameToLayer("Default");
			mDirLight.name = "Shadow Dir Light";
			mDirLight.transform.forward = Vector3.down;
			mDirLight.transform.position = PlayCamera.transform.position;
			Light component2 = gameObject.GetComponent<Light>();
			if (component2 != null)
			{
				component.color = component2.color;
				component.intensity = component2.intensity;
				component2.shadows = LightShadows.None;
			}
			SafeSetShaderMaxLOD("Corona/Probe/Base", 300);
			SafeSetShaderMaxLOD("Corona/Lightmap/[Spec] [Detail]", 300);
		}
		else
		{
			if (gameObject.GetComponent<Light>() != null)
			{
				gameObject.GetComponent<Light>().shadows = LightShadows.None;
			}
			SafeSetShaderMaxLOD("Corona/Probe/Base", 200);
			SafeSetShaderMaxLOD("Corona/Lightmap/[Spec] [Detail]", 200);
		}
	}

	public void AllowInput(bool allow)
	{
		if (ActiveCamera == ActiveCameraType.StrategyCamera)
		{
			CameraController component = StrategyCamera.GetComponent<CameraController>();
			component.CurrentCameraBase.AllowInput(allow);
		}
		else if (ActiveCamera == ActiveCameraType.PlayCamera)
		{
			CameraController component2 = PlayCamera.GetComponent<CameraController>();
			component2.CurrentCameraBase.AllowInput(allow);
		}
	}

	private void Update()
	{
		GameController instance = GameController.Instance;
		bool flag = instance.AimimgDownScopeThisFrame || instance.mFirstPersonActor == null || instance.mFirstPersonActor.realCharacter.IsUsingFixedGun || instance.mFirstPersonActor.realCharacter.Carried != null;
		if (instance.AimimgDownScopeThisFrame)
		{
			PlayCamera.targetTexture = SniperTexture;
			SniperCamera.enabled = true;
			PlayCamera.ResetAspect();
		}
		else
		{
			PlayCamera.targetTexture = null;
			SniperCamera.enabled = false;
			PlayCamera.ResetAspect();
		}
		if (flag)
		{
			PlayCamera.cullingMask &= ~mViewModelMask;
		}
		else
		{
			PlayCamera.cullingMask |= mViewModelMask;
		}
		if (mDirLight != null)
		{
			mDirLight.transform.position = PlayCamera.transform.position;
		}
		UpdateDepthBands(mSeparateViewModelBands && instance.AllowSeparateViewModelBands);
	}

	public static void UpdateDepthBands(bool seperate)
	{
		if (seperate)
		{
			Shader.SetGlobalVector("_DepthBand", new Vector4(0.01f, 0.01f, 0.99f, 0f));
		}
		else
		{
			Shader.SetGlobalVector("_DepthBand", new Vector4(1f, 0f, 1f, 0f));
		}
	}

	public PlayCameraInterface CurrentPlayCameraInterface()
	{
		return CurrentCamera.GetComponent<CameraController>().CurrentCameraBase as PlayCameraInterface;
	}

	public Vector3 WorldToScreenPoint(Vector3 worldPos)
	{
		if (SniperCamera.enabled)
		{
			Vector3 vector = PlayCamera.WorldToViewportPoint(worldPos);
			Vector3 vector2 = SniperCamera.WorldToScreenPoint(SniperCamera.transform.position + new Vector3(0f - mSniperPlaneSize, 0f - mSniperPlaneSize, 0f));
			Vector3 vector3 = SniperCamera.WorldToScreenPoint(SniperCamera.transform.position + new Vector3(mSniperPlaneSize, mSniperPlaneSize, 0f));
			return new Vector3(vector2.x + (vector3.x - vector2.x) * vector.x, vector2.y + (vector3.y - vector2.y) * vector.y, vector.z);
		}
		if (IsSwitching && ActiveCamera == ActiveCameraType.PlayCamera)
		{
			return StrategyCamera.WorldToScreenPoint(worldPos);
		}
		return CurrentCamera.WorldToScreenPoint(worldPos);
	}

	public void SwitchToCameraType(ActiveCameraType toCamera, CameraBase logicalCameraTo)
	{
		if (ActiveCamera != toCamera)
		{
			TBFAssert.DoAssert(!mIsSwitching, "transition still in progress");
			StartCoroutine(DoTransitionCo(toCamera, logicalCameraTo));
		}
	}

	private IEnumerator DoTransitionCo(ActiveCameraType toCamera, CameraBase logicalCameraTo)
	{
		if (toCamera == ActiveCameraType.StrategyCamera && mDirLight != null)
		{
			Light lightcomp2 = mDirLight.GetComponent<Light>();
			lightcomp2.shadows = LightShadows.None;
		}
		if (mIsSwitching)
		{
			yield return null;
		}
		mIsSwitching = true;
		switch (toCamera)
		{
		case ActiveCameraType.StrategyCamera:
		{
			if ((bool)mOvWaShInt)
			{
				mOvWaShInt.enabled = true;
			}
			SniperCamera.enabled = false;
			HudBlipIcon.SwitchAllToStrategyView();
			CameraController camCCTo2 = StrategyCamera.GetComponent<CameraController>();
			CameraController camCCFrom2 = CurrentCamera.GetComponent<CameraController>();
			camCCFrom2.CurrentCameraBase.AllowInput(false);
			CameraBase toTargetCam = logicalCameraTo ?? camCCTo2.StartCamera;
			camCCTo2.CutTo(camCCFrom2.CurrentCameraBase);
			CurrentCamera.gameObject.SetActive(false);
			StrategyCamera.gameObject.SetActive(true);
			camCCTo2.BlendTo(new CameraTransitionData(toTargetCam, TweenFunctions.TweenType.easeInOutCubic, SwitchTime));
			RenderSettings.fogDensity = 0f;
			RenderSettings.fogStartDistance = float.MaxValue;
			RenderSettings.fogEndDistance = float.MaxValue;
			RenderSettings.fog = false;
			while (camCCTo2.IsTransitioning())
			{
				yield return null;
			}
			camCCTo2.CurrentCameraBase.AllowInput(true);
			ActiveCamera = ActiveCameraType.StrategyCamera;
			break;
		}
		case ActiveCameraType.PlayCamera:
		{
			CameraController camCCTo = PlayCamera.GetComponent<CameraController>();
			CameraController camCCFrom = CurrentCamera.GetComponent<CameraController>();
			camCCFrom.CurrentCameraBase.AllowInput(false);
			camCCFrom.BlendTo(new CameraTransitionData(camCCTo.StartCamera, TweenFunctions.TweenType.easeInOutCubic, SwitchTime));
			while (camCCFrom.IsTransitioning())
			{
				yield return null;
			}
			RenderSettings.fogDensity = mFogDensityOverride;
			RenderSettings.fogStartDistance = mFogStartOverride;
			RenderSettings.fogEndDistance = mFogEndOverride;
			RenderSettings.fog = true;
			SetFogPlayCameraPlane();
			if ((bool)mOvWaShInt)
			{
				mOvWaShInt.DoTransition();
			}
			CurrentCamera.gameObject.SetActive(false);
			PlayCamera.gameObject.SetActive(true);
			camCCTo.CurrentCameraBase.AllowInput(true);
			ActiveCamera = ActiveCameraType.PlayCamera;
			HudBlipIcon.SwitchAllToGameplayView();
			if ((bool)mOvWaShInt)
			{
				mOvWaShInt.enabled = false;
			}
			if (mDirLight != null)
			{
				Light lightcomp = mDirLight.GetComponent<Light>();
				lightcomp.shadows = LightShadows.Hard;
			}
			break;
		}
		}
		mIsSwitching = false;
	}

	public void AddExplosionShake(Vector3 origin, float radius)
	{
		if (GameController.Instance.AllowExplosionShake)
		{
			CameraController playCameraController = PlayCameraController;
			playCameraController.CurrentCameraBase.AddShake(origin, radius, 0.5f);
		}
	}

	public void SetCamerasToLevelCameraTarget()
	{
		CameraTarget cameraTarget = UnityEngine.Object.FindObjectOfType(typeof(CameraTarget)) as CameraTarget;
		if (cameraTarget != null)
		{
			PlayCameraInterface playCameraInterface = PlayCameraController.StartCamera as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.SnapToTarget(cameraTarget.transform);
			}
			StrategyViewCamera strategyViewCamera = StrategyCameraController.StartCamera as StrategyViewCamera;
			if (strategyViewCamera != null)
			{
				strategyViewCamera.target = cameraTarget.transform;
			}
		}
	}
}
