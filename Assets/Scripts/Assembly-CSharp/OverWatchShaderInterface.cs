using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Noise")]
[ExecuteInEditMode]
public class OverWatchShaderInterface : MonoBehaviour
{
	public float SquareFlickerSpeed = 0.2f;

	public float SquareIntensity = 0.2f;

	public float grainIntensityMin;

	public float grainIntensityMax = 0.5f;

	public float grainSize = 1f;

	public float Saturation = 0.15f;

	public float FadeIntensity = 2f;

	public float TransitionSpeed = 10f;

	public bool DoTransTest;

	private float m_fBaseGrainSize;

	private float m_fBaseIntensityMax;

	private float m_fBaseIntensityMin;

	public Texture grainTexture;

	public Shader OverwatchShader;

	public Shader QuickShader;

	private Material m_MaterialRGB;

	private Material m_MaterialQuick;

	private RenderTexture m_RenderTex;

	private bool m_bDoTransition;

	private float m_fU;

	private float m_fV;

	private int m_aaSetting;

	protected Material material
	{
		get
		{
			if (m_MaterialRGB == null)
			{
				m_MaterialRGB = new Material(OverwatchShader);
				m_MaterialRGB.hideFlags = HideFlags.HideAndDontSave;
				m_MaterialRGB.SetTexture("_GrainTex", grainTexture);
			}
			return m_MaterialRGB;
		}
	}

	public Material materialQuick
	{
		get
		{
			if (m_MaterialQuick == null)
			{
				m_MaterialQuick = new Material(QuickShader);
				m_MaterialQuick.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_MaterialQuick;
		}
	}

	protected void Start()
	{
		if (OverwatchShader == null)
		{
			Debug.Log("Noise shaders are not set up! Disabling noise effect.");
			base.enabled = false;
		}
	}

	protected void Awake()
	{
		m_fBaseGrainSize = grainSize;
		m_fBaseIntensityMax = grainIntensityMax;
		m_fBaseIntensityMin = grainIntensityMin;
	}

	protected void OnEnable()
	{
		if (m_RenderTex == null)
		{
			m_RenderTex = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
			m_RenderTex.name = "OverWatch";
		}
		m_aaSetting = QualitySettings.antiAliasing;
		QualitySettings.antiAliasing = 0;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			a.baseCharacter.SetOverWatchModel();
		}
		DoTransition();
		StartCoroutine(FlickerRoutine());
		StartCoroutine(SquareFlickerRoutine());
	}

	protected void OnDisable()
	{
		Object.DestroyImmediate(m_RenderTex);
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			a.baseCharacter.SetBaseModel();
		}
		QualitySettings.antiAliasing = m_aaSetting;
	}

	public void DoTransition()
	{
		m_bDoTransition = true;
		grainIntensityMin = 5f;
		grainIntensityMax = 5f;
	}

	private void SanitizeParameters()
	{
		grainIntensityMin = Mathf.Clamp(grainIntensityMin, 0f, 5f);
		grainIntensityMax = Mathf.Clamp(grainIntensityMax, 0f, 5f);
		grainSize = Mathf.Clamp(grainSize, 0.1f, 50f);
	}

	private void OnPreRender()
	{
		if (base.camera.targetTexture == null)
		{
			base.camera.targetTexture = m_RenderTex;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		SanitizeParameters();
		material.SetVector("_GrainOffsetScale", new Vector4(m_fU, m_fV, (float)Screen.width / (float)grainTexture.width, (float)Screen.height / (float)grainTexture.height));
		material.SetFloat("_Saturation", Saturation);
		material.SetFloat("_Intensity", Random.Range(grainIntensityMin, grainIntensityMax));
		material.SetFloat("_IntensitySquare", SquareIntensity);
		material.SetFloat("_Fade", FadeIntensity);
		Graphics.Blit(source, destination, material);
		if (base.camera.targetTexture == m_RenderTex)
		{
			base.camera.targetTexture = null;
			Graphics.Blit(destination, null, materialQuick);
		}
	}

	private void LateUpdate()
	{
		if (m_bDoTransition)
		{
			m_bDoTransition = false;
			if (grainIntensityMin > m_fBaseIntensityMin)
			{
				grainIntensityMin -= TransitionSpeed * TimeManager.DeltaTime * 2f;
				m_bDoTransition = true;
			}
			else
			{
				grainIntensityMin = m_fBaseIntensityMin;
			}
			if (grainIntensityMax > m_fBaseIntensityMax)
			{
				grainIntensityMax -= TransitionSpeed * TimeManager.DeltaTime;
				m_bDoTransition = true;
			}
			else
			{
				grainIntensityMax = m_fBaseIntensityMax;
			}
			if (grainSize > m_fBaseGrainSize)
			{
				grainSize -= TransitionSpeed * TimeManager.DeltaTime * 2f;
				m_bDoTransition = true;
			}
			else
			{
				grainSize = m_fBaseGrainSize;
			}
		}
		if (DoTransTest)
		{
			DoTransition();
			DoTransTest = false;
		}
	}

	private IEnumerator FlickerRoutine()
	{
		while (base.enabled)
		{
			yield return new WaitForSeconds(5f + Random.value * 2f);
			if (!m_bDoTransition && base.enabled)
			{
				grainIntensityMin = Random.value * 2f;
				grainIntensityMax = Random.value * 5f;
				m_bDoTransition = true;
				if ((bool)OverwatchController.Instance && OverwatchController.Instance.Active)
				{
					BriefingSFX.Instance.ScreenStatic.Play2D();
				}
			}
		}
		yield return null;
	}

	private IEnumerator SquareFlickerRoutine()
	{
		while (base.enabled)
		{
			yield return new WaitForSeconds(SquareFlickerSpeed);
			if (base.enabled)
			{
				m_fU = Random.value;
				m_fV = Random.value;
			}
		}
		yield return null;
	}
}
