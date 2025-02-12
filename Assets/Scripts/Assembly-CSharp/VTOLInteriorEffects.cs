using System;
using System.Collections.Generic;
using UnityEngine;

public class VTOLInteriorEffects : MonoBehaviour, ILightingOverride
{
	[Serializable]
	public class Light
	{
		public Transform Locator;

		public Color Colour;

		public float Intensity;

		public float Range;
	}

	private const int maxLights = 4;

	public Transform Root;

	public Color AmbientColour = Color.white;

	public float AmbientIntensity = 1f;

	public Light[] Lights = new Light[0];

	private Transform mRearDoor;

	private Transform mLeftDoor;

	private Transform mRightDoor;

	private Color mCachedAmbient;

	private Vector3[] mLightPositions = new Vector3[4];

	private Renderer[] mInteriorRenderers;

	private static MaterialPropertyBlock smPropertyBlock;

	private static int smPropertyEffectsAmbient;

	private static int smPropertyLightPosX;

	private static int smPropertyLightPosY;

	private static int smPropertyLightPosZ;

	private static int smPropertyLightAttenSq;

	private static int[] smPropertyLightColor;

	private static bool sInitialised;

	public static void Initialise()
	{
		if (!sInitialised)
		{
			smPropertyBlock = new MaterialPropertyBlock();
			smPropertyEffectsAmbient = Shader.PropertyToID("_EffectsAmbient");
			smPropertyLightPosX = Shader.PropertyToID("_LightPosX");
			smPropertyLightPosY = Shader.PropertyToID("_LightPosY");
			smPropertyLightPosZ = Shader.PropertyToID("_LightPosZ");
			smPropertyLightAttenSq = Shader.PropertyToID("_LightAttenSq");
			smPropertyLightColor = new int[4];
			smPropertyLightColor[0] = Shader.PropertyToID("_LightColour0");
			smPropertyLightColor[1] = Shader.PropertyToID("_LightColour1");
			smPropertyLightColor[2] = Shader.PropertyToID("_LightColour2");
			smPropertyLightColor[3] = Shader.PropertyToID("_LightColour3");
		}
		sInitialised = true;
	}

	public void Start()
	{
		ProbeUtils.Initialise();
		Initialise();
		FindDoors();
		FindInteriorRenderers();
	}

	public void Update()
	{
		float a = ((!(mRearDoor != null)) ? 0f : Mathf.InverseLerp(270f, 330f, mRearDoor.localEulerAngles.x));
		float b = ((!(mRearDoor != null)) ? 0f : Mathf.Clamp01(Vector3.Distance(mRearDoor.localPosition, new Vector3(4.215139f, 1.009727f, 1.140726f))));
		float num = ((!(mLeftDoor != null)) ? 0f : Mathf.InverseLerp(-0.126f, -0.0387f, mLeftDoor.localPosition.y));
		float num2 = ((!(mRightDoor != null)) ? 0f : Mathf.InverseLerp(-0.126f, -0.0387f, mRightDoor.localPosition.y));
		float num3 = Mathf.Max(a, b) + num + num2;
		Color value = num3 * AmbientIntensity * AmbientColour;
		for (int i = 0; i < mLightPositions.Length; i++)
		{
			mLightPositions[i] = Vector3.zero;
		}
		Vector4 value2 = new Vector4(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);
		smPropertyBlock.Clear();
		for (int j = 0; j < Mathf.Min(Lights.Length, 4); j++)
		{
			Light light = Lights[j];
			if (light.Locator != null)
			{
				float intensity = light.Intensity;
				smPropertyBlock.SetColor(smPropertyLightColor[j], 2f * intensity * light.Colour);
				mLightPositions[j] = light.Locator.position;
				value2[j] = 25f / (light.Range * light.Range);
			}
		}
		smPropertyBlock.SetColor(smPropertyEffectsAmbient, value);
		smPropertyBlock.SetVector(smPropertyLightPosX, new Vector4(mLightPositions[0].x, mLightPositions[1].x, mLightPositions[2].x, mLightPositions[3].x));
		smPropertyBlock.SetVector(smPropertyLightPosY, new Vector4(mLightPositions[0].y, mLightPositions[1].y, mLightPositions[2].y, mLightPositions[3].y));
		smPropertyBlock.SetVector(smPropertyLightPosZ, new Vector4(mLightPositions[0].z, mLightPositions[1].z, mLightPositions[2].z, mLightPositions[3].z));
		smPropertyBlock.SetVector(smPropertyLightAttenSq, value2);
		Renderer[] array = mInteriorRenderers;
		foreach (Renderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.SetPropertyBlock(smPropertyBlock);
			}
		}
		mCachedAmbient = value;
	}

	public void CalculateLighting(Vector3 anchorPosition, float[] coefficients)
	{
		ProbeUtils.AddSHAmbientLight(mCachedAmbient, coefficients, 0);
		Light[] lights = Lights;
		foreach (Light light in lights)
		{
			ProbeUtils.AddSHPointLight(light.Colour, light.Locator.position, light.Range, light.Intensity, coefficients, 0, anchorPosition);
		}
	}

	public bool Valid()
	{
		return this != null;
	}

	private void FindDoors()
	{
		if (Root != null)
		{
			Transform transform = Root.FindInHierarchy("NODE_VTOL_Int");
			if (transform != null)
			{
				mRearDoor = transform.FindInHierarchyStartsWith("door_lower");
				mLeftDoor = transform.FindInHierarchyStartsWith("PROP_Door001");
				mRightDoor = transform.FindInHierarchyStartsWith("PROP_Door002");
			}
		}
	}

	private void FindInteriorRenderers()
	{
		List<Renderer> list = new List<Renderer>();
		Renderer[] componentsInChildren = Root.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in componentsInChildren)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material != null && material.shader != null && material.shader.name.Contains("VTOL Interior"))
				{
					list.Add(renderer);
					break;
				}
			}
		}
		mInteriorRenderers = list.ToArray();
	}
}
