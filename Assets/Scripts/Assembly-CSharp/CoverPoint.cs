using System.Collections.Generic;
using UnityEngine;

public class CoverPoint
{
	public enum CoverProvided
	{
		None = 0,
		Partial = 1,
		Full = 2
	}

	public const float kOffensive = 1f;

	public const float kDefensive = -1f;

	public const float kNeutral = 0f;

	public WaypointGameObject WaypointGObj;

	public static float COVER_TOO_CLOSE_TO_ENEMY_DIST_SQ = 9f;

	private RealCharacter mOwner;

	private int ownerSteppedOut;

	private static float MINIMUM_PERSONAL_SPACE_DIST_SQ = 1f;

	private List<CoverPoint> mPersonalSpaceEncroachers;

	public RealCharacter Owner
	{
		get
		{
			return mOwner;
		}
		set
		{
			mOwner = value;
		}
	}

	public CoverPoint(WaypointGameObject waypoint)
	{
		WaypointGObj = waypoint;
		Owner = null;
		mPersonalSpaceEncroachers = new List<CoverPoint>();
	}

	public bool IsValid(Actor soldier, Vector2 soldierGproj, List<Vector2> enemyPositions, Tether tether, Vector3 focusPoint, float offencedefence)
	{
		if (WaypointGObj.debugBreak)
		{
			MonoBehaviour.print("Debugging!");
		}
		if (Owner != null && Owner != soldier)
		{
			return false;
		}
		if (tether != null)
		{
			float sqrMagnitude = (WaypointGObj.snappedToPosGproj - tether.PositionGproj).sqrMagnitude;
			if (sqrMagnitude > tether.TetherLimitSq)
			{
				return false;
			}
		}
		if ((WaypointGObj.Configuration & WaypointGameObject.Flavour.HighCoverCorner) == WaypointGameObject.Flavour.HighCoverCorner)
		{
			bool flag = true;
			bool flag2 = true;
			if (enemyPositions.Count > 0)
			{
				foreach (Vector2 enemyPosition in enemyPositions)
				{
					Vector2 lhs = enemyPosition - WaypointGObj.snappedToPosGproj;
					if (lhs.sqrMagnitude < COVER_TOO_CLOSE_TO_ENEMY_DIST_SQ)
					{
						return false;
					}
					if (flag && Vector2.Dot(lhs, WaypointGObj.rightWall) < 0f)
					{
						flag = false;
					}
					if (flag2 && Vector2.Dot(lhs, WaypointGObj.leftWall) < 0f)
					{
						flag2 = false;
					}
					if (!(flag || flag2))
					{
						return false;
					}
					Vector2 rhs = enemyPosition - soldierGproj;
					if (Vector2.Dot(lhs, rhs) < 0f)
					{
						return false;
					}
				}
				if (flag && flag2)
				{
					return false;
				}
				if (flag)
				{
					WaypointGObj.Apply2DSubPositionFacingCached(0);
				}
				else
				{
					WaypointGObj.Apply2DSubPositionFacingCached(1);
				}
			}
			else
			{
				if (offencedefence == 0f)
				{
					offencedefence = -1f;
				}
				if (Vector3.Cross(WaypointGObj.snappedToForward, focusPoint - WaypointGObj.snappedToPos).y * offencedefence > 0f)
				{
					WaypointGObj.Apply2DSubPositionFacingCached(0);
				}
				else
				{
					WaypointGObj.Apply2DSubPositionFacingCached(1);
				}
			}
		}
		else
		{
			if (enemyPositions.Count > 0)
			{
				foreach (Vector2 enemyPosition2 in enemyPositions)
				{
					Vector2 lhs2 = enemyPosition2 - WaypointGObj.snappedToPosGproj;
					if (lhs2.sqrMagnitude < COVER_TOO_CLOSE_TO_ENEMY_DIST_SQ)
					{
						return false;
					}
					if (Vector2.Dot(lhs2, WaypointGObj.snappedToForward.xz()) > 0f)
					{
						return false;
					}
					Vector2 rhs2 = enemyPosition2 - soldierGproj;
					if (Vector2.Dot(lhs2, rhs2) < 0f)
					{
						return false;
					}
				}
			}
			else if (Vector3.Dot(WaypointGObj.snappedToForward, focusPoint - WaypointGObj.snappedToPos) * offencedefence > 0f)
			{
				return false;
			}
			WaypointGObj.Apply2DSubPositionFacingCached(0);
		}
		foreach (CoverPoint mPersonalSpaceEncroacher in mPersonalSpaceEncroachers)
		{
			if (mPersonalSpaceEncroacher.Owner == null || mPersonalSpaceEncroacher.Owner == soldier)
			{
				continue;
			}
			return false;
		}
		return true;
	}

	public bool IsAvailable()
	{
		return Owner == null;
	}

	public Vector3 GetPosition()
	{
		return WaypointGObj.GetPosition();
	}

	public Vector2 GetPositionGproj()
	{
		return new Vector2(WaypointGObj.inGamePosition.x, WaypointGObj.inGamePosition.z);
	}

	public void ClearOwnershipOnly()
	{
		mOwner = null;
	}

	public void RegisterPersonalSpaceEncroachers()
	{
		TBFAssert.DoAssert(mPersonalSpaceEncroachers == null, "Already registered");
		CoverPointManager coverPointManager = CoverPointManager.Instance();
		foreach (CoverPoint coverPoint in coverPointManager.CoverPoints)
		{
			if (coverPoint != this)
			{
				float sqrMagnitude = (coverPoint.GetPosition() - GetPosition()).sqrMagnitude;
				if (sqrMagnitude < MINIMUM_PERSONAL_SPACE_DIST_SQ)
				{
					mPersonalSpaceEncroachers.Add(coverPoint);
				}
			}
		}
	}

	public CoverProvided CoverProvidedAgainst(RealCharacter rc)
	{
		if ((WaypointGObj.Configuration & WaypointGameObject.Flavour.HighCoverCorner) == WaypointGameObject.Flavour.HighCoverCorner)
		{
			Vector2 lhs = rc.GetPositionGproj() - WaypointGObj.snappedToPosGproj;
			float num = Vector2.Dot(lhs, WaypointGObj.inGameForward.xz());
			float num2 = Vector2.Dot(lhs, WaypointGObj.inGameTangent.xz());
			if (num >= 0f)
			{
				return CoverProvided.None;
			}
			if (num2 <= 0f)
			{
				return CoverProvided.Full;
			}
			float num3 = (0f - num) / num2;
			if (ownerSteppedOut == 1)
			{
				if (num3 > 0.57f)
				{
					return CoverProvided.Full;
				}
				return CoverProvided.Partial;
			}
			if (ownerSteppedOut == 2)
			{
				if (num3 < 0.57f)
				{
					return CoverProvided.None;
				}
				if (num3 < 1.73f)
				{
					return CoverProvided.Partial;
				}
				return CoverProvided.Full;
			}
			if (ownerSteppedOut == 3)
			{
				if (num3 > 1.73f)
				{
					return CoverProvided.Partial;
				}
				return CoverProvided.None;
			}
			return CoverProvided.Full;
		}
		return CoverProvided.Partial;
	}

	public void StepOut(int amount)
	{
		ownerSteppedOut = amount;
	}
}
