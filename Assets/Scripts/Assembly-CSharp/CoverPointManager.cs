using System.Collections.Generic;
using UnityEngine;

public class CoverPointManager : MonoBehaviour
{
	public static CoverPointManager instance = null;

	public List<WaypointGameObject> Waypoints;

	private static float SAFE_DISTANCE_TO_COVER = 1f;

	private static float SAFE_DISTANCE_TO_COVER_SQ = SAFE_DISTANCE_TO_COVER * SAFE_DISTANCE_TO_COVER;

	private List<CoverPoint> mCoverPoints;

	private bool mReady;

	private GameObject mNavTester;

	private UnityEngine.AI.NavMeshAgent mNavTesterAgent;

	public List<CoverPoint> CoverPoints
	{
		get
		{
			return mCoverPoints;
		}
	}

	public static CoverPointManager Instance()
	{
		return instance;
	}

	public static bool MoveToCoverNearPosition(Actor soldier, Vector3 destination, BaseCharacter.MovementStyle speed, float unitCoverSearchRadius, bool isPlayerIssuedOrder)
	{
		if (soldier.awareness.ChDefCharacterType != CharacterType.Human)
		{
			return false;
		}
		CoverPointManager coverPointManager = Instance();
		CoverPoint nearestAvailableCover = coverPointManager.GetNearestAvailableCover(soldier, destination);
		if (nearestAvailableCover != null)
		{
			float num = unitCoverSearchRadius * unitCoverSearchRadius;
			float sqrMagnitude = (nearestAvailableCover.GetPosition() - destination).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				coverPointManager.RegisterCoverPoint(nearestAvailableCover, soldier);
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		instance = this;
		Waypoints = new List<WaypointGameObject>();
		mCoverPoints = new List<CoverPoint>();
	}

	private void OnDestroy()
	{
		Waypoints = null;
		mCoverPoints = null;
		instance = null;
	}

	private void Start()
	{
		mReady = false;
		CreateNavTester();
	}

	private void Update()
	{
		if (!mReady && Waypoints.Count > 0)
		{
			RegisterCoverPoints(Waypoints);
			mReady = true;
		}
		for (int i = 0; i < mCoverPoints.Count; i++)
		{
			if (!(mCoverPoints[i].Owner == null) && (mCoverPoints[i].Owner.IsDead() || !((mCoverPoints[i].GetPositionGproj() - mCoverPoints[i].Owner.GetPositionGproj()).sqrMagnitude < SAFE_DISTANCE_TO_COVER_SQ)) && 0 == 0)
			{
				mCoverPoints[i].Owner = null;
			}
		}
	}

	public bool IsCoverAvailable(Actor soldier)
	{
		List<Vector2> enemyPositions = new List<Vector2>();
		Vector2 positionGproj = soldier.realCharacter.GetPositionGproj();
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			if (mCoverPoint.IsValid(soldier, positionGproj, enemyPositions, soldier.tether, soldier.GetPosition(), 0f))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInCover(Actor soldier)
	{
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			if (mCoverPoint.Owner != soldier)
			{
				continue;
			}
			return IsInCover(soldier, mCoverPoint);
		}
		return false;
	}

	public bool IsInCover(Actor soldier, CoverPoint coverPoint)
	{
		TBFAssert.DoAssert(coverPoint != null, "No CoverPoint specified in call. Bad call!");
		float sqrMagnitude = (coverPoint.GetPositionGproj() - soldier.realCharacter.GetPositionGproj()).sqrMagnitude;
		return sqrMagnitude < SAFE_DISTANCE_TO_COVER_SQ;
	}

	public CoverPoint RequestCoverPoint(Actor soldier)
	{
		return RequestCoverPoint(soldier, mCoverPoints);
	}

	public CoverPoint GetNearestAvailableOffensiveCover(Actor soldier, Vector3 position, Vector3 facing, float searchRadius, float innerRadiusExclusion)
	{
		return GetNearestAvailableCover(soldier, position, facing, searchRadius, innerRadiusExclusion, true, 1f);
	}

	public CoverPoint GetNearestAvailableDefensiveCover(Actor soldier, Vector3 position, Vector3 facing, float searchRadius, float innerRadiusExclusion)
	{
		return GetNearestAvailableCover(soldier, position, facing, searchRadius, innerRadiusExclusion, true, -1f);
	}

	public CoverPoint GetNearestAvailableDefensiveCover(Actor soldier, Vector3 position, Vector3 facing, float searchRadius, float innerRadiusExclusion, bool respectTether)
	{
		return GetNearestAvailableCover(soldier, position, facing, searchRadius, innerRadiusExclusion, respectTether, -1f);
	}

	public CoverPoint GetNearestAvailableCover(Actor soldier, Vector3 position, Vector3 facing, float searchRadius, float innerRadiusExclusion, bool respectTether, float offencedefence)
	{
		List<Vector2> enemyPositions = new List<Vector2>();
		Tether tether = null;
		if (respectTether)
		{
			tether = soldier.tether;
		}
		Vector2 vector = new Vector2(position.x, position.z);
		float num = searchRadius * searchRadius;
		float num2 = innerRadiusExclusion * innerRadiusExclusion;
		float num3 = float.MaxValue;
		CoverPoint result = null;
		Vector2 positionGproj = soldier.realCharacter.GetPositionGproj();
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			float sqrMagnitude = (mCoverPoint.GetPositionGproj() - vector).sqrMagnitude;
			if (!(sqrMagnitude > num) && !(sqrMagnitude < num2) && mCoverPoint.IsValid(soldier, positionGproj, enemyPositions, tether, position, offencedefence))
			{
				float travelDistance = GetTravelDistance(soldier.GetPosition(), mCoverPoint, tether);
				if (travelDistance < num3)
				{
					num3 = travelDistance;
					result = mCoverPoint;
				}
			}
		}
		return result;
	}

	public CoverPoint GetNearestAvailableOffensiveCover(Actor soldier, Vector3 origin, float searchRadius, float innerRadiusExclusion)
	{
		return GetNearestAvailableCover(soldier, origin, searchRadius, innerRadiusExclusion, true, 1f);
	}

	public CoverPoint GetNearestAvailableDefensiveCover(Actor soldier, Vector3 origin, float searchRadius, float innerRadiusExclusion)
	{
		return GetNearestAvailableCover(soldier, origin, searchRadius, innerRadiusExclusion, true, -1f);
	}

	public CoverPoint GetNearestAvailableCover(Actor soldier, Vector3 origin, float searchRadius, float innerRadiusExclusion, bool respectTether, float facingmod)
	{
		Vector2 vector = new Vector2(origin.x, origin.z);
		float num = searchRadius * searchRadius;
		float num2 = innerRadiusExclusion * innerRadiusExclusion;
		Tether tether = null;
		if (respectTether)
		{
			tether = soldier.tether;
		}
		float num3 = float.MaxValue;
		CoverPoint result = null;
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			if (!mCoverPoint.IsAvailable())
			{
				continue;
			}
			Vector2 normalized = (mCoverPoint.GetPositionGproj() - vector).normalized;
			if (Vector2.Dot(normalized, mCoverPoint.WaypointGObj.Facing) * facingmod < 0f)
			{
				continue;
			}
			float sqrMagnitude = (mCoverPoint.GetPositionGproj() - vector).sqrMagnitude;
			if (!(sqrMagnitude > num) && !(sqrMagnitude < num2))
			{
				float travelDistance = GetTravelDistance(soldier.GetPosition(), mCoverPoint, tether);
				if (travelDistance < num3)
				{
					num3 = travelDistance;
					result = mCoverPoint;
				}
			}
		}
		return result;
	}

	public CoverPoint GetNearestAvailableCover(Actor soldier, Vector3 position)
	{
		List<Vector2> enemyPositions = new List<Vector2>();
		Tether tether = soldier.tether;
		float num = float.MaxValue;
		CoverPoint result = null;
		Vector2 positionGproj = soldier.realCharacter.GetPositionGproj();
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			if (mCoverPoint.IsValid(soldier, positionGproj, enemyPositions, tether, position, 0f))
			{
				float travelDistance = GetTravelDistance(position, mCoverPoint, tether);
				if (travelDistance < num)
				{
					num = travelDistance;
					result = mCoverPoint;
				}
			}
		}
		return result;
	}

	public CoverPoint GetNearestAvailableCoverNotVisibleTo(Actor soldier, Vector3 position, Actor soldierToAvoid, float innerRadiusExclusion)
	{
		List<Vector2> enemyPositions = new List<Vector2>();
		CoverPoint result = null;
		Vector2 vector = new Vector2(position.x, position.z);
		Tether tether = soldier.tether;
		float num = innerRadiusExclusion * innerRadiusExclusion;
		float num2 = float.MaxValue;
		Vector2 positionGproj = soldier.realCharacter.GetPositionGproj();
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			if (!mCoverPoint.IsValid(soldier, positionGproj, enemyPositions, tether, position, 1f))
			{
				continue;
			}
			Vector3 lhs = mCoverPoint.GetPosition() - soldierToAvoid.GetPosition();
			lhs.Normalize();
			float num3 = Vector3.Dot(lhs, soldierToAvoid.realCharacter.FirstPersonCamera.transform.forward);
			if (num3 > 0.75f && num3 < 1.25f)
			{
				continue;
			}
			float sqrMagnitude = (mCoverPoint.GetPositionGproj() - vector).sqrMagnitude;
			if (!(sqrMagnitude < num))
			{
				float travelDistance = GetTravelDistance(position, mCoverPoint, tether);
				if (travelDistance < num2)
				{
					num2 = travelDistance;
					result = mCoverPoint;
				}
			}
		}
		return result;
	}

	public void RegisterCoverPoint(CoverPoint coverPoint, Actor owner)
	{
		if (coverPoint.Owner != null && coverPoint.Owner != owner)
		{
			TBFAssert.DoAssert(false, string.Format("{0} is trying to book cover point {1} which is already booked by {2}", owner.name, coverPoint.WaypointGObj.name, coverPoint.Owner.name));
		}
		coverPoint.Owner = owner.realCharacter;
	}

	public void AddCoverPoint(CoverPoint coverPoint)
	{
		mCoverPoints.Add(coverPoint);
	}

	public void RemoveCoverPoint(CoverPoint coverPoint)
	{
		for (int num = mCoverPoints.Count - 1; num >= 0; num--)
		{
			if (mCoverPoints[num] == coverPoint)
			{
				mCoverPoints.RemoveAt(num);
				break;
			}
		}
	}

	private void RegisterCoverPoints(List<WaypointGameObject> waypoints)
	{
		mCoverPoints.Clear();
		foreach (WaypointGameObject waypoint in waypoints)
		{
			if ((waypoint.Configuration & WaypointGameObject.Flavour.Cover) != 0)
			{
				CoverPoint item = new CoverPoint(waypoint);
				mCoverPoints.Add(item);
			}
		}
		foreach (CoverPoint mCoverPoint in mCoverPoints)
		{
			mCoverPoint.RegisterPersonalSpaceEncroachers();
		}
	}

	private CoverPoint RequestCoverPoint(Actor soldier, List<CoverPoint> coverPoints)
	{
		List<Vector2> list = new List<Vector2>();
		if (list.Count == 0)
		{
			return null;
		}
		Tether tether = soldier.tether;
		float num = float.MaxValue;
		CoverPoint coverPoint = null;
		Vector2 positionGproj = soldier.realCharacter.GetPositionGproj();
		foreach (CoverPoint coverPoint2 in coverPoints)
		{
			float sqrMagnitude = (coverPoint2.GetPositionGproj() - positionGproj).sqrMagnitude;
			if (!(sqrMagnitude > 625f) && coverPoint2.IsValid(soldier, positionGproj, list, tether, soldier.GetPosition(), -1f))
			{
				float travelDistance = GetTravelDistance(soldier.GetPosition(), coverPoint2, tether);
				if (travelDistance < num)
				{
					num = travelDistance;
					coverPoint = coverPoint2;
				}
			}
		}
		if (coverPoint != null)
		{
			coverPoint.Owner = soldier.realCharacter;
		}
		return coverPoint;
	}

	public float GetTravelDistance(Vector3 start, CoverPoint coverPoint, Tether tether)
	{
		float travelDistanceExpensive = GetTravelDistanceExpensive(start, coverPoint);
		if (tether != null)
		{
			float travelDistanceExpensive2 = GetTravelDistanceExpensive(tether.Position, coverPoint);
			if (travelDistanceExpensive2 > travelDistanceExpensive)
			{
				return float.MaxValue;
			}
		}
		return travelDistanceExpensive;
	}

	public float GetTravelDistanceCheap(Vector3 start, CoverPoint coverPoint, Tether tether)
	{
		float sqrMagnitude = (coverPoint.GetPosition() - start).sqrMagnitude;
		if (tether != null)
		{
			float sqrMagnitude2 = (coverPoint.GetPosition() - tether.Position).sqrMagnitude;
			if (sqrMagnitude2 > tether.TetherLimitSq)
			{
				return float.MaxValue;
			}
		}
		return sqrMagnitude;
	}

	private float GetTravelDistanceExpensive(Vector3 start, CoverPoint coverPoint)
	{
		float num = float.MaxValue;
		mNavTester.SetActive(true);
		mNavTester.transform.position = start;
		if (mNavTesterAgent == null)
		{
			mNavTesterAgent = mNavTester.AddComponent<UnityEngine.AI.NavMeshAgent>();
			ActorGenerator.ConfigureNavMeshAgent(mNavTesterAgent, 0f, true, 360f);
		}
		mNavTesterAgent.enabled = true;
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		if (mNavTesterAgent.CalculatePath(coverPoint.GetPosition(), navMeshPath) && navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
		{
			num = 0f;
			for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
			{
				num += (navMeshPath.corners[i + 1] - navMeshPath.corners[i]).sqrMagnitude;
			}
		}
		mNavTesterAgent.enabled = false;
		mNavTester.SetActive(false);
		return num;
	}

	private void CreateNavTester()
	{
		mNavTester = new GameObject("Cover Eval Nav Tester");
		mNavTesterAgent = null;
		mNavTester.SetActive(false);
	}
}
