using System.Collections.Generic;
using UnityEngine;

public static class WorldHelper
{
	public enum Quadrant
	{
		kFront = 0,
		kRight = 1,
		kBack = 2,
		kLeft = 3
	}

	private const int LAYER_SELECTABLE_GAME_OBJECT = 256;

	private const int LAYER_HUD = 512;

	private const int LAYER_DONT_DRAW = 1024;

	private const int LAYER_STRATEGY_VIEW_MODEL = 2048;

	private const int LAYER_TRIGGERS = 4096;

	private const int LAYER_NAVZONES = 8192;

	private const int LAYER_HIT_BOX = 16384;

	private const int LAYER_CAMERA = 32768;

	public const float kSin45 = 0.70710677f;

	public const float kSin45Sqr = 0.5f;

	public static float CHARACTER_TOUCH_PROXIMITY = 2f;

	public static float CHARACTER_TOUCH_PROXIMITY_SQ = CHARACTER_TOUCH_PROXIMITY * CHARACTER_TOUCH_PROXIMITY;

	private static int LAYER_GLASS;

	private static int LAYER_CONSTANT_HIT;

	private static int LAYER_SIMPLE_HIT;

	private static int LAYER_ACTOR_GAME_OBJECT;

	private static int LAYER_IGNORERAYCASTS;

	private static int LAYER_LOS;

	private static int LAYER_TFX;

	private static int LAYER_SNIPER;

	private static int PROJECTILESPHERE;

	private static int LOCKONDETECTORS;

	private static int PELVICREGION;

	private static float mDeltaScale = 1f;

	private static float thisFrameTime = 0f;

	public static int oneSecondTick;

	public static int tenthSecondTick;

	public static int hundredthSecondTick;

	private static int noMantlingMask = 0;

	private static int mantlingLayer = 0;

	public static float ThisFrameTime
	{
		get
		{
			return thisFrameTime;
		}
	}

	public static int BitCount(uint u)
	{
		u -= (u >> 1) & 0x55555555;
		u = (u & 0x33333333) + ((u >> 2) & 0x33333333);
		return (int)(((u + (u >> 4)) & 0xF0F0F0F) * 16843009 >> 24);
	}

	public static float GetTerrainHeight(Vector2 gproj, float yTestOrigin, float yTestRange)
	{
		Vector3 traceStart = new Vector3(gproj.x, yTestOrigin + yTestRange, gproj.y);
		Vector3 traceEnd = new Vector3(gproj.x, yTestOrigin - yTestRange, gproj.y);
		Vector3 collision = new Vector3(0f, 0f, 0f);
		if (IsClearTrace(traceStart, traceEnd, out collision))
		{
			Debug.Log("WARNING: GetTerrainHeight - no terrain height position found at " + gproj);
		}
		return collision.y;
	}

	public static bool IsClearTrace(Vector3 traceStart, Vector3 traceEnd)
	{
		int layerMask = ~(LAYER_ACTOR_GAME_OBJECT | LAYER_GLASS | LAYER_CONSTANT_HIT | LAYER_SIMPLE_HIT | 0x100 | 0x200 | 0x400 | 0x800 | 0x1000 | 0x2000 | 0x4000 | 0x8000 | LAYER_IGNORERAYCASTS | LAYER_LOS | LAYER_TFX | LAYER_SNIPER | PROJECTILESPHERE | LOCKONDETECTORS | PELVICREGION);
		if (Physics.Linecast(traceStart, traceEnd, layerMask))
		{
			return false;
		}
		return true;
	}

	public static bool IsClearTrace(Vector3 traceStart, Vector3 traceEnd, out Vector3 collision)
	{
		int num = LAYER_ACTOR_GAME_OBJECT | LAYER_GLASS | LAYER_CONSTANT_HIT | LAYER_SIMPLE_HIT | 0x100 | 0x200 | 0x400 | 0x800 | 0x1000 | 0x2000 | 0x4000 | 0x8000 | LAYER_IGNORERAYCASTS | LAYER_LOS | LAYER_TFX | LAYER_SNIPER | PROJECTILESPHERE | LOCKONDETECTORS | PELVICREGION;
		num = ~num;
		RaycastHit hitInfo;
		if (Physics.Linecast(traceStart, traceEnd, out hitInfo, num))
		{
			collision = hitInfo.point;
			return false;
		}
		collision = Vector3.zero;
		return true;
	}

	public static bool IsClearLockOnTrace(Vector3 traceStart, Vector3 traceEnd)
	{
		int num = LAYER_ACTOR_GAME_OBJECT | LAYER_GLASS | LAYER_CONSTANT_HIT | LAYER_SIMPLE_HIT | 0x100 | 0x200 | 0x400 | 0x800 | 0x1000 | 0x2000 | 0x4000 | 0x8000 | LAYER_IGNORERAYCASTS | LAYER_LOS | LAYER_TFX | LAYER_SNIPER | PROJECTILESPHERE | PELVICREGION;
		num = ~num;
		RaycastHit hitInfo;
		if (Physics.Linecast(traceStart, traceEnd, out hitInfo, num))
		{
			return hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("LockOnDetectors");
		}
		return false;
	}

	public static WaypointGameObject GetNearestWaypoint(List<WaypointGameObject> waypoints, Vector3 position, WaypointGameObject exclude)
	{
		WaypointGameObject result = null;
		float num = float.MaxValue;
		foreach (WaypointGameObject waypoint in waypoints)
		{
			if (!(waypoint == exclude))
			{
				float sqrMagnitude = (waypoint.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = waypoint;
				}
			}
		}
		return result;
	}

	public static WaypointGameObject GetFurthestWaypoint(List<WaypointGameObject> waypoints, Vector3 position)
	{
		WaypointGameObject result = null;
		float num = float.MinValue;
		foreach (WaypointGameObject waypoint in waypoints)
		{
			float sqrMagnitude = (waypoint.transform.position - position).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				num = sqrMagnitude;
				result = waypoint;
			}
		}
		return result;
	}

	public static bool IsPlayerControlledActor(Actor actor)
	{
		if (actor == null)
		{
			return false;
		}
		if (actor.behaviour == null || !actor.behaviour.PlayerControlled)
		{
			return false;
		}
		return true;
	}

	public static bool IsPlayerControlledActor(GameObject obj)
	{
		return IsPlayerControlledActor(obj.GetComponent<Actor>());
	}

	public static bool IsSelectableActor(Actor actor)
	{
		if (actor != null && actor.realCharacter.CanSelect())
		{
			return true;
		}
		return false;
	}

	public static bool IsNPC(GameObject go)
	{
		BehaviourController component = go.GetComponent<BehaviourController>();
		return component != null && !component.PlayerControlled;
	}

	public static Quadrant GetQuadrant(Vector3 fr, Vector3 to, float frontBackWeighting)
	{
		float num = (fr.x * to.x + fr.z * to.z) * frontBackWeighting;
		float num2 = fr.z * to.x - fr.x * to.z;
		float num3 = Mathf.Abs(num) - Mathf.Abs(num2);
		if (num3 >= 0f)
		{
			if (num >= 0f)
			{
				return Quadrant.kFront;
			}
			return Quadrant.kBack;
		}
		if (num2 > 0f)
		{
			return Quadrant.kRight;
		}
		return Quadrant.kLeft;
	}

	public static Quadrant GetQuadrant(Vector3 fr, Vector3 to, float frontWeighting, float backWeighting)
	{
		float num = fr.x * to.x + fr.z * to.z;
		num *= ((!(num > 0f)) ? backWeighting : frontWeighting);
		float num2 = fr.z * to.x - fr.x * to.z;
		float num3 = Mathf.Abs(num) - Mathf.Abs(num2);
		if (num3 >= 0f)
		{
			if (num >= 0f)
			{
				return Quadrant.kFront;
			}
			return Quadrant.kBack;
		}
		if (num2 > 0f)
		{
			return Quadrant.kRight;
		}
		return Quadrant.kLeft;
	}

	public static float GetSector(Vector3 fr, Vector3 to)
	{
		float num = fr.x * to.x + fr.z * to.z;
		float num2 = fr.z * to.x - fr.x * to.z;
		float num3 = Mathf.Abs(num);
		float num4 = Mathf.Abs(num2);
		float num5 = 0f;
		if (num3 < num4 * 3.73205f)
		{
			num5 = ((num3 > num4) ? 30f : ((!(num3 > num4 * 0.267949f)) ? 90f : 60f));
		}
		if (num > 0f)
		{
			if (num2 > 0f)
			{
				return num5;
			}
			return 0f - num5;
		}
		if (num2 > 0f)
		{
			return 180f - num5;
		}
		return num5 - 180f;
	}

	public static bool InFront(Vector3 fr, Vector3 to)
	{
		return Vector3.Dot(fr, to) > 0f;
	}

	public static Quadrant LeftOrRight(Vector3 fr, Vector3 to)
	{
		return (fr.z * to.x - fr.x * to.z > 0f) ? Quadrant.kRight : Quadrant.kLeft;
	}

	public static Vector3 UfM_Forward(Transform t)
	{
		return t.up * -1f;
	}

	public static Quaternion UfM_Rotation(Quaternion q)
	{
		return q * Quaternion.Euler(90f, 0f, 0f);
	}

	public static Quaternion UfM_Maybe_Rotation(Quaternion q)
	{
		if ((q * Vector3.up).y > 0.5f)
		{
			return q;
		}
		return q * Quaternion.Euler(90f, 0f, 0f);
	}

	public static void PerFramePrep()
	{
		mDeltaScale = Time.deltaTime * 60f;
		if (thisFrameTime == 0f)
		{
			thisFrameTime = Time.time;
		}
		float num = thisFrameTime;
		thisFrameTime = Time.time;
		oneSecondTick = (int)thisFrameTime - (int)num;
		tenthSecondTick = (int)(thisFrameTime * 10f) - (int)(num * 10f);
		hundredthSecondTick = (int)(thisFrameTime * 100f) - (int)(num * 100f);
		LAYER_CONSTANT_HIT = 1 << LayerMask.NameToLayer("ConstantHitBox");
		LAYER_GLASS = 1 << LayerMask.NameToLayer("Glass");
		LAYER_SIMPLE_HIT = 1 << LayerMask.NameToLayer("SimpleHitBox");
		LAYER_ACTOR_GAME_OBJECT = 1 << LayerMask.NameToLayer("ActorGameObject");
		LAYER_IGNORERAYCASTS = 1 << LayerMask.NameToLayer("Ignore Raycast");
		LAYER_LOS = 1 << LayerMask.NameToLayer("LineOfSightOnly");
		LAYER_TFX = 1 << LayerMask.NameToLayer("TransparentFX");
		LAYER_SNIPER = 1 << LayerMask.NameToLayer("SniperScope");
		PROJECTILESPHERE = 1 << LayerMask.NameToLayer("ProjectileCharacterCollider");
		LOCKONDETECTORS = 1 << LayerMask.NameToLayer("LockOnDetectors");
		PELVICREGION = 1 << LayerMask.NameToLayer("PelvicRegion");
		OptimisationManager.PerFramePrep();
		OptimisationManager.ToggleTestOptimisations(true);
	}

	public static void ExpBlend(ref float fr, float to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = to * frac + fr * (1f - frac);
	}

	public static float ExpBlend(float fr, float to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = to * frac + fr * (1f - frac);
		return fr;
	}

	public static void CappedExpBlend(ref float fr, float to, float frac, float cap)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		cap *= mDeltaScale;
		float num = to * frac + fr * (1f - frac);
		if (num - fr > cap)
		{
			fr += cap;
		}
		else if (fr - num > cap)
		{
			fr -= cap;
		}
		else
		{
			fr = num;
		}
	}

	public static float CappedExpBlend(float fr, float to, float frac, float cap)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		cap *= mDeltaScale;
		float num = to * frac + fr * (1f - frac);
		if (num - fr > cap)
		{
			fr += cap;
			return fr;
		}
		if (fr - num > cap)
		{
			fr -= cap;
			return fr;
		}
		return num;
	}

	public static Vector3 ExpBlend(Vector3 fr, Vector3 to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = fr * (1f - frac) + to * frac;
		return fr;
	}

	public static void ExpBlend(ref Vector3 fr, Vector3 to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = fr * (1f - frac) + to * frac;
	}

	public static void ExpBlend(ref Vector3 fr, Vector3 to, float frac, float snap)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = fr * (1f - frac) + to * frac;
		if ((fr - to).sqrMagnitude < snap * snap)
		{
			fr = to;
		}
	}

	public static bool PointingAtTarget(Transform t, Vector3 pos, float radius)
	{
		if (t == null)
		{
			return false;
		}
		Vector3 forward = t.forward;
		Vector3 rhs = pos - t.position;
		float num = Vector3.Dot(forward, rhs);
		if (num <= 0f)
		{
			return rhs.sqrMagnitude < 0.5f;
		}
		rhs -= forward * num;
		rhs.y *= 0.6f;
		return rhs.sqrMagnitude < radius * radius * (num / 3f);
	}

	public static bool PointingAtTarget3D(Transform t, Vector3 pos, float radius)
	{
		if (t == null)
		{
			return false;
		}
		Vector3 vector = pos - t.position;
		float num = Vector3.Dot(t.forward, vector);
		if (num <= 0f)
		{
			return false;
		}
		return (vector - t.forward * num).sqrMagnitude / (num / 8f) < radius * radius;
	}

	public static bool PercentageChance(int chance)
	{
		return Random.Range(1, 100) <= chance;
	}

	public static void KillNamedChildren(GameObject go, string name)
	{
		while (true)
		{
			Transform transform = go.transform.Find(name);
			if (transform != null)
			{
				if (Application.isPlaying)
				{
					transform.transform.parent = null;
					Object.Destroy(transform.gameObject);
				}
				else
				{
					Object.DestroyImmediate(transform.gameObject);
				}
				continue;
			}
			break;
		}
	}

	public static void SetCanMantle(Actor a)
	{
		if (noMantlingMask == 0)
		{
			mantlingLayer = 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Jump");
			noMantlingMask = ~mantlingLayer;
		}
		a.navAgent.walkableMask = a.navAgent.walkableMask | mantlingLayer;
	}

	public static void SetCantMantle(Actor a)
	{
		if (noMantlingMask == 0)
		{
			mantlingLayer = 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Jump");
			noMantlingMask = ~mantlingLayer;
		}
		a.navAgent.walkableMask = a.navAgent.walkableMask & noMantlingMask;
	}

	public static bool CalculatePath_AvoidingMantlesWhenCarrying(Actor a, Vector3 destination, UnityEngine.AI.NavMeshPath path)
	{
		if (!a.navAgent.enabled)
		{
			return false;
		}
		if (a.baseCharacter.Carried == null)
		{
			SetCanMantle(a);
			return a.navAgent.CalculatePath(destination, path);
		}
		SetCantMantle(a);
		if (!a.navAgent.CalculatePath(destination, path))
		{
			return false;
		}
		if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial)
		{
			SetCanMantle(a);
			return a.navAgent.CalculatePath(destination, path);
		}
		return true;
	}

	public static void AvoidObstacles(Actor a, Actor toIgnore)
	{
		CoverPointCore closestCoverPoint = a.awareness.closestCoverPoint;
		if (!(closestCoverPoint != null))
		{
			return;
		}
		Actor occupant = closestCoverPoint.Occupant;
		if (occupant == null && closestCoverPoint.neighbours != null)
		{
			for (int i = 1; i < closestCoverPoint.neighbours.Length && i < 4; i++)
			{
				occupant = CoverNeighbour.CoverPoint(closestCoverPoint.neighbours[i]).Occupant;
				if (occupant != null)
				{
					break;
				}
			}
		}
		if (!(occupant != null) || occupant.ident == a.ident || !(occupant != toIgnore))
		{
			return;
		}
		Vector3 vector = occupant.GetPosition() - a.GetPosition();
		float sqrMagnitude = vector.sqrMagnitude;
		if (!(sqrMagnitude < 0.46474996f))
		{
			return;
		}
		Vector3 desiredVelocity = a.navAgent.desiredVelocity;
		if (Vector3.Dot(desiredVelocity, vector) > 0f)
		{
			Vector3 rhs = new Vector3(0f - occupant.awareness.closestCoverPoint.snappedNormal.z, 0f, occupant.awareness.closestCoverPoint.snappedNormal.x);
			float num = 250f * Time.deltaTime;
			if (Vector3.Dot(a.navAgent.desiredVelocity, rhs) < 0f)
			{
				num *= -1f;
			}
			vector = Quaternion.Euler(0f, num, 0f) * vector;
			Vector3 offset = a.GetPosition() - (occupant.GetPosition() - vector);
			a.navAgent.Move(offset);
		}
	}
}
