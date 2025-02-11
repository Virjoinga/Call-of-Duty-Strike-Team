using System;
using UnityEngine;

public class ProbeUtils
{
	private static MaterialPropertyBlock smPropertyBlock;

	private static int smPropertyAr;

	private static int smPropertyAg;

	private static int smPropertyAb;

	private static int smPropertyBr;

	private static int smPropertyBg;

	private static int smPropertyBb;

	private static int smPropertyC;

	private static int smProperty0;

	private static int smPropertySpecDir;

	private static bool sInitialised = false;

	private static Vector4[] smMaterialCoefficients = new Vector4[8];

	public static void Initialise()
	{
		if (!sInitialised)
		{
			smPropertyBlock = new MaterialPropertyBlock();
			smPropertyAr = Shader.PropertyToID("cAr");
			smPropertyAg = Shader.PropertyToID("cAg");
			smPropertyAb = Shader.PropertyToID("cAb");
			smPropertyBr = Shader.PropertyToID("cBr");
			smPropertyBg = Shader.PropertyToID("cBg");
			smPropertyBb = Shader.PropertyToID("cBb");
			smPropertyC = Shader.PropertyToID("cC");
			smProperty0 = Shader.PropertyToID("c0rgb");
			smPropertySpecDir = Shader.PropertyToID("_SpecDir");
		}
		sInitialised = true;
	}

	public static void UpdateMaterials(float[] coefficients, Renderer[] renderers)
	{
		float num = 1f / (float)Math.PI;
		float num2 = 0.2820948f;
		float num3 = 0.325735f;
		float num4 = 0.27313712f;
		float num5 = 0.07884789f;
		float num6 = 0.1628675f;
		for (int i = 0; i < 3; i++)
		{
			smMaterialCoefficients[i].x = (0f - num3) * coefficients[9 + i];
			smMaterialCoefficients[i].y = (0f - num3) * coefficients[3 + i];
			smMaterialCoefficients[i].z = num3 * coefficients[6 + i];
			smMaterialCoefficients[i].w = num2 * coefficients[0 + i] - num5 * coefficients[18 + i];
		}
		for (int i = 0; i < 3; i++)
		{
			smMaterialCoefficients[i + 3].x = num4 * coefficients[12 + i];
			smMaterialCoefficients[i + 3].y = (0f - num4) * coefficients[15 + i];
			smMaterialCoefficients[i + 3].z = 3f * num5 * coefficients[18 + i];
			smMaterialCoefficients[i + 3].w = (0f - num4) * coefficients[21 + i];
		}
		smMaterialCoefficients[6].x = num6 * coefficients[24];
		smMaterialCoefficients[6].y = num6 * coefficients[25];
		smMaterialCoefficients[6].z = num6 * coefficients[26];
		smMaterialCoefficients[6].w = 1f;
		smMaterialCoefficients[7] = num * new Vector4(coefficients[0], coefficients[1], coefficients[2], 0f);
		smPropertyBlock.Clear();
		smPropertyBlock.AddVector(smPropertyAr, smMaterialCoefficients[0]);
		smPropertyBlock.AddVector(smPropertyAg, smMaterialCoefficients[1]);
		smPropertyBlock.AddVector(smPropertyAb, smMaterialCoefficients[2]);
		smPropertyBlock.AddVector(smPropertyBr, smMaterialCoefficients[3]);
		smPropertyBlock.AddVector(smPropertyBg, smMaterialCoefficients[4]);
		smPropertyBlock.AddVector(smPropertyBb, smMaterialCoefficients[5]);
		smPropertyBlock.AddVector(smPropertyC, smMaterialCoefficients[6]);
		smPropertyBlock.AddVector(smProperty0, smMaterialCoefficients[7]);
		smPropertyBlock.AddVector(smPropertySpecDir, SpecularSource.specdir);
		foreach (Renderer renderer in renderers)
		{
			if (renderer != null)
			{
				renderer.SetPropertyBlock(smPropertyBlock);
			}
		}
	}

	public static void AddSHAmbientLight(Color color, float[] coefficients, int index)
	{
		float num = 3.5449078f;
		coefficients[index] += color.r * num;
		coefficients[index + 1] += color.g * num;
		coefficients[index + 2] += color.b * num;
	}

	public static void AddSHDirectionalLight(Color color, Vector3 direction, float intensity, float[] coefficients, int index)
	{
		float num = 0.2820948f;
		float num2 = 0.48860252f;
		float num3 = 1.0925485f;
		float num4 = 0.9461747f;
		float num5 = 0.54627424f;
		float num6 = 1f / 3f;
		float[] array = new float[9]
		{
			num,
			(0f - direction.y) * num2,
			direction.z * num2,
			(0f - direction.x) * num2,
			direction.x * direction.y * num3,
			(0f - direction.y) * direction.z * num3,
			(direction.z * direction.z - num6) * num4,
			(0f - direction.x) * direction.z * num3,
			(direction.x * direction.x - direction.y * direction.y) * num5
		};
		float num7 = 2.956793f;
		intensity *= 2f;
		float num8 = color.r * intensity * num7;
		float num9 = color.g * intensity * num7;
		float num10 = color.b * intensity * num7;
		for (int i = 0; i < 9; i++)
		{
			float num11 = array[i];
			coefficients[index + 3 * i] += num11 * num8;
			coefficients[index + 3 * i + 1] += num11 * num9;
			coefficients[index + 3 * i + 2] += num11 * num10;
		}
	}

	public static void AddSHPointLight(Color color, Vector3 position, float range, float intensity, float[] coefficients, int index, Vector3 probePosition)
	{
		Vector3 vector = position - probePosition;
		float num = 1f / (1f + 25f * vector.sqrMagnitude / (range * range));
		AddSHDirectionalLight(color, vector.normalized, intensity * num, coefficients, index);
	}

	public static Color CalculateShadowColour(float[] shCoefficients)
	{
		float num = -0.2f + Mathf.Clamp01(RenderSettings.ambientLight.r + 0.5641896f * shCoefficients[0] + -0.4886025f * shCoefficients[6] + -0.3153916f * shCoefficients[12] + -0.5462742f * shCoefficients[24]) + -0.2f + Mathf.Clamp01(RenderSettings.ambientLight.g + 0.5641896f * shCoefficients[1] + -0.4886025f * shCoefficients[7] + -0.3153916f * shCoefficients[13] + -0.5462742f * shCoefficients[25]) + -0.2f + Mathf.Clamp01(RenderSettings.ambientLight.b + 0.5641896f * shCoefficients[2] + -0.4886025f * shCoefficients[8] + -0.3153916f * shCoefficients[14] + -0.5462742f * shCoefficients[26]);
		num *= 0.33333f;
		return new Color(num, num, num);
	}
}
