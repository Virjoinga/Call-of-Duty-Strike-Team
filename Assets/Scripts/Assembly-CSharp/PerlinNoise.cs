using UnityEngine;

public class PerlinNoise
{
	private static float Noise(int x, int y)
	{
		int num = x + y * 57;
		num = (num << 13) ^ num;
		return 1f - (float)((num * (num * num * 15731 + 789221) + 1376312589) & 0x7FFFFFFF) / 1.0737418E+09f;
	}

	private static float SmoothedNoise(float x, float y)
	{
		float num = (Noise((int)(x - 1f), (int)(y - 1f)) + Noise((int)(x + 1f), (int)(y - 1f)) + Noise((int)(x - 1f), (int)(y + 1f)) + Noise((int)(x + 1f), (int)(y + 1f))) / 16f;
		float num2 = (Noise((int)(x - 1f), (int)y) + Noise((int)(x + 1f), (int)y) + Noise((int)x, (int)(y - 1f)) + Noise((int)x, (int)(y + 1f))) / 8f;
		float num3 = Noise((int)x, (int)y) / 4f;
		return num + num2 + num3;
	}

	private static float InterpolatedNoise(float x, float y)
	{
		int num = (int)x;
		float t = x - (float)num;
		int num2 = (int)y;
		float t2 = y - (float)num2;
		float from = SmoothedNoise(num, num2);
		float to = SmoothedNoise(num + 1, num2);
		float from2 = SmoothedNoise(num, num2 + 1);
		float to2 = SmoothedNoise(num + 1, num2 + 1);
		float from3 = Mathf.Lerp(from, to, t);
		float to3 = Mathf.Lerp(from2, to2, t);
		return Mathf.Lerp(from3, to3, t2);
	}

	public static float Get(float x, float y, float persistence, int numberOfOctaves)
	{
		float num = 0f;
		int num2 = numberOfOctaves - 1;
		for (int i = 0; i < num2; i++)
		{
			float num3 = Mathf.Pow(2f, i);
			float num4 = Mathf.Pow(persistence, i);
			num += InterpolatedNoise(x * num3, y * num3) * num4;
		}
		return num;
	}
}
