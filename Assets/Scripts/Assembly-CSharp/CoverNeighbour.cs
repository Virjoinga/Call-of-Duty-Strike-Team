using UnityEngine;

public static class CoverNeighbour
{
	public static int coverIndex(int packed)
	{
		return PackedCoverData.UnpackFirstIndex((uint)packed);
	}

	public static void coverIndex(ref int packed, int val)
	{
		uint data = (uint)packed;
		PackedCoverData.PackFirstIndex(ref data, val);
		packed = (int)data;
	}

	public static int routeTo(int packed)
	{
		return PackedCoverData.UnpackSecondIndex((uint)packed);
	}

	public static void routeTo(ref int packed, int val)
	{
		uint data = (uint)packed;
		PackedCoverData.PackSecondIndex(ref data, val);
		packed = (int)data;
	}

	public static float navMeshDistance(int packed)
	{
		return PackedCoverData.UnpackDistance((uint)packed);
	}

	public static void navMeshDistance(ref int packed, float val)
	{
		uint data = (uint)packed;
		PackedCoverData.PackDistance(ref data, val);
		packed = (int)data;
	}

	public static Vector2 vagueDirection(int packed)
	{
		return PackedCoverData.UnpackDirection((uint)packed);
	}

	public static void vagueDirection(ref int packed, Vector2 val)
	{
		uint data = (uint)packed;
		PackedCoverData.PackDirection(ref data, val);
		packed = (int)data;
	}

	public static CoverPointCore CoverPoint(int packed)
	{
		return NewCoverPointManager.Instance().coverPoints[coverIndex(packed)];
	}
}
