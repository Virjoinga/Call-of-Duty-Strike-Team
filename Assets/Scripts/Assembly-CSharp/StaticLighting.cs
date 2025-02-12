using System.Collections.Generic;
using UnityEngine;

public class StaticLighting : MonoBehaviour
{
	public Transform ProbeAnchor;

	public Transform[] GroupRoots;

	private Renderer[] mRenderers;

	private float[] mCoefficientTemp = new float[27];

	private void Start()
	{
		Initialise();
	}

	public void LateUpdate()
	{
		UpdateMaterials();
		base.enabled = false;
	}

	public void UpdateMaterials()
	{
		if (mRenderers != null && mRenderers.Length != 0)
		{
			for (int i = 0; i < mCoefficientTemp.Length; i++)
			{
				mCoefficientTemp[i] = 0f;
			}
			if (LightmapSettings.lightProbes != null && ProbeAnchor != null)
			{
				//LightmapSettings.lightProbes.GetInterpolatedProbe(ProbeAnchor.position, mRenderers[0], mCoefficientTemp);
			}
			else
			{
				ProbeUtils.AddSHDirectionalLight(Color.white, Vector3.up, 1f, mCoefficientTemp, 0);
			}
			ProbeUtils.UpdateMaterials(mCoefficientTemp, mRenderers);
		}
	}

	private void Initialise()
	{
		ProbeUtils.Initialise();
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
		foreach (Renderer item in list)
		{
			item.materials = item.materials;
		}
	}
}
