using System;
using UnityEngine;

public static class PackedCoverData
{
	private const float kPackDistanceScale = 10f;

	private const float kUnpackDistanceScale = 0.1f;

	private const uint kMaxPackedDistance = 255u;

	public const uint kFirstIndexMask = 1023u;

	private const uint kSecondIndexMask = 1047552u;

	public const int kSecondIndexShift = 10;

	private const uint kDistanceMask = 267386880u;

	private const int kDistanceShift = 20;

	private const uint kDirectionMask = 4026531840u;

	private const int kDirectionShift = 28;

	public static Vector2[] directionTable;

	public static void InitDirectionTable()
	{
		if (directionTable == null)
		{
			directionTable = new Vector2[16];
			for (int i = 0; i < 16; i++)
			{
				float f = (float)Math.PI / 180f * (float)i * 22.5f;
				directionTable[i].x = Mathf.Sin(f);
				directionTable[i].y = Mathf.Cos(f);
			}
		}
	}

	public static void PackDistance(ref uint data, float d)
	{
		uint num = (uint)(d * 10f);
		if (num > 255)
		{
			num = 255u;
		}
		data = (data & 0xF00FFFFFu) | (num << 20);
	}

	public static float UnpackDistance(uint data)
	{
		return (float)((data & 0xFF00000) >> 20) * 0.1f;
	}

	public static void PackFirstIndex(ref uint data, int i)
	{
		data = (data & 0xFFFFFC00u) | (uint)i;
	}

	public static int UnpackFirstIndex(uint data)
	{
		return (int)(data & 0x3FF);
	}

	public static void PackSecondIndex(ref uint data, int i)
	{
		data = (data & 0xFFF003FFu) | (uint)(i << 10);
	}

	public static int UnpackSecondIndex(uint data)
	{
		return (int)((data & 0xFFC00) >> 10);
	}

	public static void PackDirection(ref uint data, Vector2 v)
	{
		v.Normalize();
		uint num = 0u;
		uint num2 = 0u;
		if (v.y < -0.990476f)
		{
			num = 8u;
			num2 = 8u;
		}
		else if (v.y < -0.8314696f)
		{
			num = 9u;
			num2 = 7u;
		}
		else if (v.y < -0.55557f)
		{
			num = 10u;
			num2 = 6u;
		}
		else if (v.y < -0.19509f)
		{
			num = 11u;
			num2 = 5u;
		}
		else if (v.y < 0.19509f)
		{
			num = 12u;
			num2 = 4u;
		}
		else if (v.y < 0.55557f)
		{
			num = 13u;
			num2 = 3u;
		}
		else if (v.y < 0.8314696f)
		{
			num = 14u;
			num2 = 2u;
		}
		else if (v.y < 0.990476f)
		{
			num = 15u;
			num2 = 1u;
		}
		if (v.x < 0f)
		{
			data = (data & 0xFFFFFFFu) | (num << 28);
		}
		else
		{
			data = (data & 0xFFFFFFFu) | (num2 << 28);
		}
	}

	public static Vector2 UnpackDirection(uint data)
	{
		int num = (int)((data & 0xF0000000u) >> 28);
		return directionTable[num];
	}
}
