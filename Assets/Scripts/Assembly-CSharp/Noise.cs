using UnityEngine;

public class Noise
{
	private static Vector3[] mNoiseOffsets;

	public static Vector3 Smooth(float time)
	{
		if (mNoiseOffsets == null)
		{
			Init();
		}
		int num = Mathf.FloorToInt(time);
		int num2 = (num % mNoiseOffsets.Length + mNoiseOffsets.Length) % mNoiseOffsets.Length;
		int num3 = ((num + 1) % mNoiseOffsets.Length + mNoiseOffsets.Length) % mNoiseOffsets.Length;
		int num4 = ((num + 2) % mNoiseOffsets.Length + mNoiseOffsets.Length) % mNoiseOffsets.Length;
		int num5 = ((num + 3) % mNoiseOffsets.Length + mNoiseOffsets.Length) % mNoiseOffsets.Length;
		float num6 = time - Mathf.Floor(time);
		Vector3 vector = mNoiseOffsets[num3];
		Vector3 vector2 = mNoiseOffsets[num4];
		Vector3 vector3 = 0.5f * (mNoiseOffsets[num3] - mNoiseOffsets[num2] + (mNoiseOffsets[num4] - mNoiseOffsets[num3]));
		Vector3 vector4 = 0.5f * (mNoiseOffsets[num4] - mNoiseOffsets[num3] + (mNoiseOffsets[num5] - mNoiseOffsets[num4]));
		float num7 = num6 * num6;
		float num8 = num7 * num6;
		float num9 = 2f * num8 - 3f * num7 + 1f;
		float num10 = num8 - 2f * num7 + num6;
		float num11 = -2f * num8 + 3f * num7;
		float num12 = num8 - num7;
		return num9 * vector + num10 * vector3 + num11 * vector2 + num12 * vector4;
	}

	private static void Init()
	{
		mNoiseOffsets = new Vector3[16];
		for (int i = 0; i < mNoiseOffsets.Length; i++)
		{
			mNoiseOffsets[i] = Random.insideUnitSphere;
		}
	}
}
