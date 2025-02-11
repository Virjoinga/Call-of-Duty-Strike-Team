using System.Collections.Generic;
using UnityEngine;

public class CutsceneLighting : MonoBehaviour
{
	public Transform ProbeAnchor;

	public Transform[] GroupRoots;

	public Object Override;

	public bool AllowRuntimeUpdate;

	private Renderer[] mRenderers;

	private float[] mCoefficientTemp = new float[27];

	private void Start()
	{
		Initialise();
	}

	public void LateUpdate()
	{
		if (!(ProbeAnchor != null) || ProbeAnchor.gameObject.activeInHierarchy)
		{
			if (ProbeAnchor == null)
			{
				Object.Destroy(this);
			}
			else
			{
				UpdateMaterials(false);
			}
		}
	}

	public void UpdateMaterials(bool force)
	{
		if (mRenderers != null && mRenderers.Length != 0)
		{
			for (int i = 0; i < mCoefficientTemp.Length; i++)
			{
				mCoefficientTemp[i] = 0f;
			}
			ILightingOverride lightingOverride = Override as ILightingOverride;
			if (lightingOverride != null && lightingOverride.Valid() && ProbeAnchor != null)
			{
				lightingOverride.CalculateLighting(ProbeAnchor.position, mCoefficientTemp);
			}
			else if (LightmapSettings.lightProbes != null && ProbeAnchor != null)
			{
				LightmapSettings.lightProbes.GetInterpolatedLightProbe(ProbeAnchor.position, mRenderers[0], mCoefficientTemp);
			}
			else
			{
				ProbeUtils.AddSHDirectionalLight((!(Mathf.Repeat(Time.realtimeSinceStartup * 2f, 2f) > 1f)) ? Color.cyan : Color.magenta, Vector3.up, 1f, mCoefficientTemp, 0);
			}
			ProbeUtils.UpdateMaterials(mCoefficientTemp, mRenderers);
		}
	}

	private void Initialise()
	{
		ProbeUtils.Initialise();
		UpdateRenderers(true);
	}

	public void UpdateRenderers(bool bForced)
	{
		if (!bForced && !AllowRuntimeUpdate)
		{
			return;
		}
		List<Renderer> list = new List<Renderer>();
		if (GroupRoots != null)
		{
			Transform[] groupRoots = GroupRoots;
			foreach (Transform transform in groupRoots)
			{
				if (transform != null)
				{
					list.AddRange(transform.GetComponentsInChildren<Renderer>(true));
				}
			}
		}
		mRenderers = list.ToArray();
	}
}
