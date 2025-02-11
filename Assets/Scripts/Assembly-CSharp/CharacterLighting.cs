using UnityEngine;

public class CharacterLighting : MonoBehaviour
{
	public Renderer[] Renderers;

	public Transform ProbeAnchor;

	public Transform FlashAnchor;

	public float FlashIntensity = 10f;

	private float mFlash;

	private float[] mCoefficientTemp = new float[27];

	private int roundRobinIndex;

	public Color ShadowColour { get; private set; }

	public void Flash(Transform anchor, float intensity)
	{
		mFlash = 1f;
		FlashAnchor = anchor;
		FlashIntensity = intensity;
	}

	private void Awake()
	{
		ProbeUtils.Initialise();
		BaseCharacter component = GetComponent<BaseCharacter>();
		roundRobinIndex = ((!(component != null)) ? Random.Range(0, 1) : (component.roundRobinIndex & 1));
	}

	public void LateUpdate()
	{
		if ((Time.frameCount & 1) == roundRobinIndex)
		{
			UpdateMaterials(false);
		}
		UpdateFlash();
	}

	private void UpdateFlash()
	{
		mFlash = Mathf.Max(0f, mFlash - 10f * Time.deltaTime);
	}

	public void UpdateMaterials(bool force)
	{
		if (Renderers == null || Renderers.Length == 0)
		{
			return;
		}
		bool flag = force;
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			if (renderer != null && renderer.isVisible)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			for (int j = 0; j < mCoefficientTemp.Length; j++)
			{
				mCoefficientTemp[j] = 0f;
			}
			if (LightmapSettings.lightProbes != null && ProbeAnchor != null)
			{
				LightmapSettings.lightProbes.GetInterpolatedLightProbe(ProbeAnchor.position, Renderers[0], mCoefficientTemp);
			}
			else
			{
				ProbeUtils.AddSHDirectionalLight(Color.white, Vector3.up, 1f, mCoefficientTemp, 0);
			}
			if (mFlash > 0f)
			{
				ProbeUtils.AddSHPointLight(new Color(0.835294f, 0.803921f, 0.58823f), FlashAnchor.position, 1f, mFlash * FlashIntensity, mCoefficientTemp, 0, ProbeAnchor.position);
			}
			ProbeUtils.UpdateMaterials(mCoefficientTemp, Renderers);
			ShadowColour = ProbeUtils.CalculateShadowColour(mCoefficientTemp);
		}
	}
}
