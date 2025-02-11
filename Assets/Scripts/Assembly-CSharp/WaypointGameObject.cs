using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointGameObject : MonoBehaviour
{
	[Flags]
	public enum Flavour
	{
		Default = 0,
		Cover = 1,
		Breach = 2,
		HighCover = 4,
		Corner = 8,
		HighCoverCorner = 0xC
	}

	public enum CornerDirection
	{
		Left = 0,
		Right = 1,
		None = 2
	}

	private struct Cached2DSubPositionFacing
	{
		public Vector2 Position;

		public Vector2 Facing;
	}

	private const float kDistanceFromWall = 0.4f;

	private const float kDistanceFromLowCover = 0.6f;

	private const float kDistanceFromCorner = -0.5f;

	private const float kWayPointPreviewHeight = 0.3f;

	public Waypoint Data;

	public Flavour Configuration;

	private static RaycastHit[] rch;

	public Vector3 snappedToPos;

	public Vector2 snappedToPosGproj;

	public Vector3 snappedToForward;

	public Vector2 leftWall;

	public Vector2 rightWall;

	public Vector3 inGamePosition;

	public Vector3 inGameForward;

	public Vector3 inGameTangent;

	public bool debugBreak;

	public CornerDirection cornerDirection;

	private Cached2DSubPositionFacing[] m2DSubPositionFacingCache;

	public Vector3 StandFacing
	{
		get
		{
			if ((Configuration & Flavour.HighCover) != 0)
			{
				return inGameTangent;
			}
			return inGameForward * -1f;
		}
	}

	public Vector3 AnimationFacing
	{
		get
		{
			if ((Configuration & Flavour.HighCover) != 0)
			{
				return inGameForward;
			}
			return inGameForward * -1f;
		}
	}

	public Vector2 Facing
	{
		get
		{
			return new Vector2(snappedToForward.x, snappedToForward.z);
		}
		set
		{
			Vector3 vector = new Vector3(value.x, snappedToForward.y, value.y);
			snappedToForward = vector;
		}
	}

	public int subPositionCount
	{
		get
		{
			return 1 + (((Configuration & Flavour.Corner) != 0) ? 1 : 0);
		}
	}

	private void Awake()
	{
		snappedToPosGproj = new Vector2(snappedToPos.x, snappedToPos.z);
		m2DSubPositionFacingCache = new Cached2DSubPositionFacing[2];
		if ((Configuration & Flavour.HighCover) == 0)
		{
			inGamePosition = snappedToPos + snappedToForward * 0.6f;
		}
		else
		{
			inGamePosition = snappedToPos + snappedToForward * 0.4f;
		}
		inGameForward = snappedToForward;
		inGameTangent = new Vector3(snappedToForward.z, 0f, 0f - snappedToForward.x);
		if ((Configuration & Flavour.Corner) != 0)
		{
			leftWall = new Vector2(0f - (snappedToForward.x + snappedToForward.z), 0f - (snappedToForward.z - snappedToForward.x));
			rightWall = new Vector2(0f - (snappedToForward.x - snappedToForward.z), 0f - (snappedToForward.z + snappedToForward.x));
			leftWall.Normalize();
			rightWall.Normalize();
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
		}
		Setup2DSubPositionFacingCache();
	}

	private void OnDrawGizmos()
	{
	}

	public void Initialise(Vector3 worldPosition, string debugName)
	{
		base.transform.position = worldPosition;
		snappedToPos = worldPosition;
		base.name = string.Format("WP_{0}", debugName);
	}

	public Vector3 GetPosition()
	{
		if ((Configuration & Flavour.Breach) != 0)
		{
			return base.transform.position;
		}
		return inGamePosition;
	}

	public void Get2DSubPositionFacingCached(int i, out Vector2 pos, out Vector2 facing)
	{
		pos = m2DSubPositionFacingCache[i].Position;
		facing = m2DSubPositionFacingCache[i].Facing;
	}

	public void Apply2DSubPositionFacingCached(int i)
	{
		Vector2 position = m2DSubPositionFacingCache[i].Position;
		Vector2 facing = m2DSubPositionFacingCache[i].Facing;
		inGameForward.x = facing.x;
		inGameForward.z = facing.y;
		inGamePosition.x = position.x;
		inGamePosition.y = snappedToPos.y;
		inGamePosition.z = position.y;
		if (i == 0)
		{
			cornerDirection = CornerDirection.Right;
			inGameTangent = new Vector3(0f - leftWall.x, 0f, 0f - leftWall.y);
		}
		else
		{
			cornerDirection = CornerDirection.Left;
			inGameTangent = new Vector3(0f - rightWall.x, 0f, 0f - rightWall.y);
		}
	}

	private void Setup2DSubPositionFacingCache()
	{
		for (int i = 0; i < subPositionCount; i++)
		{
			Get2DSubPositionFacing(i, out m2DSubPositionFacingCache[i].Position, out m2DSubPositionFacingCache[i].Facing);
		}
	}

	private void Get2DSubPositionFacing(int i, out Vector2 pos, out Vector2 facing)
	{
		if ((Configuration & Flavour.Corner) != 0)
		{
			if (i == 0)
			{
				inGameForward = new Vector3(0f - rightWall.x, 0f, 0f - rightWall.y);
				inGameTangent = new Vector3(0f - leftWall.x, 0f, 0f - leftWall.y);
				inGamePosition = snappedToPos + inGameForward * 0.4f + inGameTangent * -0.5f;
				cornerDirection = CornerDirection.Right;
			}
			else
			{
				inGameForward = new Vector3(0f - leftWall.x, 0f, 0f - leftWall.y);
				inGameTangent = new Vector3(0f - rightWall.x, 0f, 0f - rightWall.y);
				inGamePosition = snappedToPos + inGameForward * 0.4f + inGameTangent * -0.5f;
				cornerDirection = CornerDirection.Left;
			}
		}
		else
		{
			cornerDirection = CornerDirection.None;
		}
		Vector3 vector = inGamePosition;
		Vector3 vector2 = inGameForward;
		pos = new Vector2(vector.x, vector.z);
		facing = new Vector2(vector2.x, vector2.z);
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
			return;
		}
		float num = (float)Math.PI / 8f;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		Vector3 origin = base.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, Vector3.down, out hitInfo, 4f, 1))
		{
			origin = hitInfo.point + new Vector3(0f, 0.3f, 0f);
		}
		if (rch == null)
		{
			rch = new RaycastHit[16];
		}
		for (int i = 0; i < 16; i++)
		{
			zero.x = Mathf.Cos(num2);
			zero.z = Mathf.Sin(num2);
			zero.y = 0f;
			if (!Physics.Raycast(origin, zero, out rch[i], 4f, 1))
			{
				rch[i].distance = 100f;
			}
			num2 += num;
		}
		int num3 = 0;
		int num4 = 0;
		float num5 = 50f;
		for (int i = 1; i < 16; i++)
		{
			if (rch[i].distance < rch[num3].distance)
			{
				num3 = i;
			}
		}
		for (int j = 14; j <= 18; j++)
		{
			int i = (num3 + j) % 16;
			if (rch[i].distance < num5)
			{
				float num6 = Vector3.Dot(rch[i].normal, rch[num3].normal);
				if (num6 >= -0.1f && num6 < 0.1f && (rch[i].normal.x * rch[num3].normal.z - rch[i].normal.z * rch[num3].normal.x) * (float)(j - 16) > 0f && (rch[i].point - rch[num3].point).sqrMagnitude < 4f)
				{
					num4 = i;
					num5 = rch[i].distance;
				}
			}
		}
		if (rch[num3].distance < 100f)
		{
			if (num4 != 0)
			{
				Configuration |= Flavour.Corner;
				snappedToForward = rch[num3].normal + rch[num4].normal;
				snappedToForward.Normalize();
				Vector3 vector = new Vector3(rch[num3].normal.z, 0f, 0f - rch[num3].normal.x);
				Vector3 lhs = rch[num4].point - rch[num3].point;
				float num7 = Vector3.Dot(lhs, rch[num4].normal);
				Vector3 vector2 = vector * num7 / Vector3.Dot(vector, rch[num4].normal);
				snappedToPos = rch[num3].point + new Vector3(vector2.x, 0f, vector2.z);
			}
			else
			{
				if ((Configuration & Flavour.Corner) != 0)
				{
					Configuration -= 8;
				}
				snappedToForward = rch[num3].normal;
				snappedToPos = rch[num3].point;
			}
			snappedToPos.y -= 0.3f;
		}
		if (Physics.CheckSphere(snappedToPos + new Vector3(0f, 1.6f, 0f), 0.2f, 1))
		{
			Configuration |= Flavour.HighCover;
		}
		else if ((Configuration & Flavour.HighCover) != 0)
		{
			Configuration -= 4;
		}
	}
}
