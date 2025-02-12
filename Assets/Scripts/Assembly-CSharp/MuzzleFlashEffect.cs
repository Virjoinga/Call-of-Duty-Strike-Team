using System;
using UnityEngine;

public class MuzzleFlashEffect : MonoBehaviour
{
	public Renderer[] Renderers;

	public Transform ProbeAnchor;

	public Transform FlashAnchor;

	public float FlashIntensity = 10f;

	private float mFlash;

	private float[] mCoefficientTemp = new float[27];

	private int roundRobinIndex;

	public Color ShadowColour { get; private set; }

	public void Flash()
	{
		mFlash = 1f;
	}

	private void Awake()
	{
		ProbeUtils.Initialise();
		BaseCharacter component = GetComponent<BaseCharacter>();
		roundRobinIndex = ((!(component != null)) ? UnityEngine.Random.Range(0, 1) : (component.roundRobinIndex & 1));
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
		if (Renderers.Length == 0)
		{
			return;
		}
		bool flag = force;
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			if (renderer.isVisible)
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
				//LightmapSettings.lightProbes.GetInterpolatedProbe(ProbeAnchor.position, Renderers[0], mCoefficientTemp);
			}
			else
			{
				ProbeUtils.AddSHDirectionalLight(Color.white, Vector3.up, 1f, mCoefficientTemp, 0);
			}
			if (mFlash > 0f)
			{
				ProbeUtils.AddSHPointLight(Color.white, FlashAnchor.position, 1f, mFlash * FlashIntensity, mCoefficientTemp, 0, ProbeAnchor.position);
			}
			ProbeUtils.UpdateMaterials(mCoefficientTemp, Renderers);
			float num = 0.5f * Mathf.Sqrt(1f / (float)Math.PI);
			float num2 = 0f - Mathf.Sqrt(3f / (4f * (float)Math.PI));
			float num3 = -0.25f * Mathf.Sqrt(5f / (float)Math.PI);
			float num4 = -0.25f * Mathf.Sqrt(15f / (float)Math.PI);
			float num5 = -0.2f;
			ShadowColour = new Color(num5 + Mathf.Clamp01(RenderSettings.ambientLight.r + num * mCoefficientTemp[0] + num2 * mCoefficientTemp[6] + num3 * mCoefficientTemp[12] + num4 * mCoefficientTemp[24]), num5 + Mathf.Clamp01(RenderSettings.ambientLight.g + num * mCoefficientTemp[1] + num2 * mCoefficientTemp[7] + num3 * mCoefficientTemp[13] + num4 * mCoefficientTemp[25]), num5 + Mathf.Clamp01(RenderSettings.ambientLight.b + num * mCoefficientTemp[2] + num2 * mCoefficientTemp[8] + num3 * mCoefficientTemp[14] + num4 * mCoefficientTemp[26]));
		}
	}
}
