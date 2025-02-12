using System;
using UnityEngine;

[AddComponentMenu("PostProcessGFXInterface")]
public class PostProcessGFXInterface : MonoBehaviour
{
	public static PostProcessGFXInterface Instance;

	public float BloomStartStrength = 0.5f;

	public float Bloomification = 2f;

	public float BlurWidthScaler = 1f;

	public float CutOff = 0.95f;

	public Texture RampTex;

	public Texture TransitionTex;

	public Texture TransitionRampTex;

	private bool mDoTrans;

	private Shader CopyShader;

	private Shader CopyDOFShader;

	private Shader CopyTransShader;

	private Shader StartShader;

	private Shader BlurShader;

	private Material mMaterial;

	private Material mBloomMaterial;

	private Material mBloomBlurMaterial;

	private RenderTexture mRenderTex;

	private RenderTexture mRenderTexBlurH;

	private RenderTexture mRenderTexBlurV;

	private float mBlurWidthW;

	private float mBlurWidthH;

	private float mYonTrans = -1f;

	private Camera RefractionCam;

	protected Material copyMaterial
	{
		get
		{
			if (mMaterial == null)
			{
				mMaterial = new Material(CopyShader);
				mMaterial.hideFlags = HideFlags.HideAndDontSave;
				mMaterial.SetTexture("_RampTex", RampTex);
			}
			return mMaterial;
		}
	}

	protected Material blurMaterial
	{
		get
		{
			if (mBloomBlurMaterial == null)
			{
				mBloomBlurMaterial = new Material(BlurShader);
				mBloomBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return mBloomBlurMaterial;
		}
	}

	protected Material bloomMaterial
	{
		get
		{
			if (mBloomMaterial == null)
			{
				mBloomMaterial = new Material(StartShader);
				mBloomMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return mBloomMaterial;
		}
	}

	protected void Start()
	{
		if (MissionSetup.Instance != null)
		{
			BloomStartStrength = MissionSetup.Instance.BloomStartStrength;
			Bloomification = MissionSetup.Instance.Bloomification;
			CutOff = MissionSetup.Instance.BloomPixelColourCutOff;
			BlurWidthScaler = MissionSetup.Instance.BlurWidthScaler;
			if (MissionSetup.Instance.ColourCorrectionRampTexture != null)
			{
				RampTex = MissionSetup.Instance.ColourCorrectionRampTexture;
			}
		}
		if (TransitionTex == null)
		{
			TransitionTex = Resources.Load("Shaders/MiscTextures/fx_droneswap") as Texture;
		}
		if (TransitionRampTex == null)
		{
			TransitionRampTex = Resources.Load("Shaders/MiscTextures/fx_droneswap_a") as Texture;
		}
	}

	private void OnDestroy()
	{
		Instance = null;
		if (RefractionCam != null)
		{
			UnityEngine.Object.Destroy(RefractionCam.gameObject);
		}
	}

	protected void OnDisable()
	{
		if ((bool)mMaterial)
		{
			UnityEngine.Object.DestroyImmediate(mMaterial);
		}
		if ((bool)mBloomMaterial)
		{
			UnityEngine.Object.DestroyImmediate(mBloomMaterial);
		}
		if ((bool)mBloomBlurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(mBloomBlurMaterial);
		}
		if ((bool)mRenderTex)
		{
			UnityEngine.Object.DestroyImmediate(mRenderTex);
		}
		if ((bool)mRenderTexBlurH)
		{
			UnityEngine.Object.DestroyImmediate(mRenderTexBlurH);
		}
		if ((bool)mRenderTexBlurV)
		{
			UnityEngine.Object.DestroyImmediate(mRenderTexBlurV);
		}
		if (CommonHudController.Instance != null)
		{
			CommonHudController.Instance.OnZoomOutTriggered -= OnZoomOutTriggered;
		}
		if (RefractionCam != null)
		{
			UnityEngine.Object.Destroy(RefractionCam.gameObject);
		}
	}

	private void Awake()
	{
		if (RampTex == null)
		{
			Debug.LogWarning("No Ramp Texture present on interface");
		}
		if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Bloom))
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void OnEnable()
	{
		StartShader = Shader.Find("Corona/PostProcess/Start");
		CopyShader = Shader.Find("Corona/PostProcess/Copy_Bloom_CC");
		CopyDOFShader = Shader.Find("Corona/PostProcess/Copy_Bloom_CC_DOF");
		CopyTransShader = Shader.Find("Corona/PostProcess/Copy_Bloom_CC_Trans");
		BlurShader = Shader.Find("Corona/PostProcess/GausBlur");
		mBloomBlurMaterial = null;
		if (mRenderTex == null)
		{
			mRenderTex = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
			mRenderTex.name = "Bloom";
			mBlurWidthW = 1f / (float)mRenderTex.width;
		}
		if (mRenderTexBlurH == null)
		{
			mRenderTexBlurH = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
			mRenderTexBlurH.name = "RenderTexBlurH";
			mBlurWidthH = 1f / (float)mRenderTexBlurH.height;
		}
		if (mRenderTexBlurV == null)
		{
			mRenderTexBlurV = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
			mRenderTexBlurV.name = "RenderTexBlurV";
		}
		if (CommonHudController.Instance != null)
		{
			CommonHudController.Instance.OnZoomOutTriggered += OnZoomOutTriggered;
		}
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.RefractionEffects))
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "RefractionCamera";
			gameObject.transform.parent = base.GetComponent<Camera>().transform;
			RefractionCam = gameObject.AddComponent<Camera>();
			RefractionCam.CopyFrom(base.GetComponent<Camera>());
			RefractionCam.clearFlags = CameraClearFlags.Nothing;
			RefractionCam.backgroundColor = Color.black;
			RefractionCam.cullingMask = 1 << LayerMask.NameToLayer("Refraction");
			RefractionCam.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bloomMaterial.SetFloat("_Strength", BloomStartStrength);
		bloomMaterial.SetFloat("_CutOff", CutOff);
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.RefractionEffects))
		{
			Graphics.Blit(source, mRenderTex);
			Shader.SetGlobalTexture("_SourceRefTex", mRenderTex);
			RefractionCam.targetTexture = source;
			RefractionCam.Render();
		}
		Graphics.Blit(source, mRenderTex, bloomMaterial);
		blurMaterial.SetVector("_BlurWidth", new Vector4(mBlurWidthW * BlurWidthScaler, 0f, mBlurWidthW * BlurWidthScaler, 0f));
		Graphics.Blit(mRenderTex, mRenderTexBlurH, blurMaterial);
		blurMaterial.SetVector("_BlurWidth", new Vector4(0f, mBlurWidthH * BlurWidthScaler, 0f, mBlurWidthH * BlurWidthScaler));
		Graphics.Blit(mRenderTexBlurH, mRenderTexBlurV, blurMaterial);
		copyMaterial.SetTexture("_BloomTex", mRenderTexBlurV);
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.DepthOfField) && (bool)GameController.Instance)
		{
			if (DoDOF())
			{
				copyMaterial.shader = CopyDOFShader;
			}
			else if (mYonTrans >= 0f)
			{
				copyMaterial.shader = CopyTransShader;
				float num = Mathf.Sin((float)Math.PI * mYonTrans);
				num = num * num * num;
				copyMaterial.SetFloat("_TimeSin", num);
				copyMaterial.SetTexture("_TranTex", TransitionTex);
				copyMaterial.SetTexture("_TranRampTex", TransitionRampTex);
			}
			else
			{
				copyMaterial.shader = CopyShader;
			}
		}
		copyMaterial.SetFloat("_Bloomification", Bloomification);
		copyMaterial.SetFloat("_YonTrans", mYonTrans);
		if (mDoTrans)
		{
			mYonTrans = 0f;
			mDoTrans = false;
		}
		Graphics.Blit(source, destination, copyMaterial);
		if (mYonTrans >= 0f)
		{
			mYonTrans += Time.deltaTime;
			if ((double)mYonTrans > 0.967)
			{
				mYonTrans = -1f;
			}
		}
	}

	public void OnZoomOutTriggered(object sender, EventArgs args)
	{
		DoTrans();
	}

	public void OnZoomInTriggered(object sender, EventArgs args)
	{
		DoTrans();
	}

	public void DoTrans()
	{
		mDoTrans = true;
	}

	private bool DoDOF()
	{
		if (GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.realCharacter.IsAimingDownSights && !GameController.Instance.mFirstPersonActor.weapon.IsReloading() && GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon.GetClass() != WeaponDescriptor.WeaponClass.Special && !InteractionsManager.IsBusy())
		{
			return true;
		}
		return false;
	}
}
