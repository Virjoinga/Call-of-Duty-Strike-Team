using UnityEngine;

public class NewCoverPointBakedData : ScriptableObject
{
	public CoverTable coverTable;

	[SerializeField]
	public int[] coverGrid;

	public int[] concealmentField;

	public byte[] distanceTable;

	public int coverGridXDim;

	public int coverGridZDim;

	public float tileX;

	public float tileZ;

	public Vector2 corner;

	public Vector2 extent;

	public bool baked;

	public int[] inAirCover;

	public uint CoverGrid(int x, int z)
	{
		return (uint)coverGrid[x + z * coverGridXDim];
	}

	public void CoverGrid(int x, int z, uint v)
	{
		coverGrid[x + z * coverGridXDim] = (int)v;
	}

	public uint ConcealmentField(int x, int z)
	{
		return (uint)concealmentField[x + z * coverGridXDim];
	}

	public void ConcealmentField(int x, int z, uint v)
	{
		concealmentField[x + z * coverGridXDim] = (int)v;
	}

	public void FixUpAnythingMissing()
	{
		coverTable.FixUpAnythingMissing();
	}
}
