using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
	public List<Transform> PatrolPoints = new List<Transform>();

	public bool UseLineRenderer;

	public bool UsePathfindingBetweenWaypoints = true;

	public bool UsePathSmoothing = true;

	public Color LineColourStart = Color.blue;

	public Color LineColourEnd = Color.red;

	public float LineWidthStart = 0.2f;

	public float LineWidthEnd = 0.2f;

	public Material TrailMaterial;

	public float TrailTime = 2f;

	public float TrailSpeed = 10f;

	public float TrailWidthStart = 0.5f;

	public float TrailWidthEnd = 0.2f;

	private List<Vector3> mPositions;

	private GameObject mWanderer;

	private int mWandererDestinationIndex;

	private void Start()
	{
		if (!ArePatrolPointsValid())
		{
			return;
		}
		CreateWanderer();
		mWanderer.transform.position = PatrolPoints[0].transform.position;
		UnityEngine.AI.NavMeshAgent navMeshAgent = mWanderer.AddComponent<UnityEngine.AI.NavMeshAgent>();
		ActorGenerator.ConfigureNavMeshAgent(navMeshAgent, 0f, true, 360f);
		mPositions = new List<Vector3>();
		for (int i = 0; i < PatrolPoints.Count; i++)
		{
			int num = i + 1;
			if (num >= PatrolPoints.Count)
			{
				num = 0;
			}
			mPositions.Add(PatrolPoints[i].transform.position);
			if (UsePathfindingBetweenWaypoints)
			{
				PathfindBetweenPoints(navMeshAgent, i, num);
			}
		}
		mPositions.Add(PatrolPoints[0].transform.position);
		if (UsePathSmoothing)
		{
			SmoothRoute();
		}
		navMeshAgent.enabled = false;
		mWanderer.transform.position = mPositions[0];
		mWanderer.transform.forward = (mPositions[1] - mPositions[0]).normalized;
		mWandererDestinationIndex = 1;
		if (UseLineRenderer)
		{
			CreateLineRenderer();
		}
	}

	private void Update()
	{
		if (!ArePatrolPointsValid() || mPositions.Count < 2)
		{
			return;
		}
		mWanderer.transform.position = mWanderer.transform.position + mWanderer.transform.forward * TimeManager.DeltaTime * TrailSpeed;
		Vector3 normalized = (mPositions[mWandererDestinationIndex] - mWanderer.transform.position).normalized;
		if (Vector3.Dot(normalized, mWanderer.transform.forward) < 0f)
		{
			int index = mWandererDestinationIndex;
			mWandererDestinationIndex++;
			if (mWandererDestinationIndex >= mPositions.Count)
			{
				mWandererDestinationIndex = 0;
			}
			mWanderer.transform.position = mPositions[index];
			Vector3 vector = mPositions[mWandererDestinationIndex] - mPositions[index];
			if (vector.sqrMagnitude > 0f)
			{
				mWanderer.transform.forward = vector.normalized;
			}
		}
	}

	private void CreateLineRenderer()
	{
		LineRenderer lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(LineColourStart, LineColourEnd);
		lineRenderer.SetWidth(LineWidthStart, LineWidthEnd);
		lineRenderer.gameObject.layer = LayerMask.NameToLayer("StrategyViewModel");
		lineRenderer.SetVertexCount(mPositions.Count + 1);
		for (int i = 0; i <= mPositions.Count; i++)
		{
			int index = i;
			if (i >= mPositions.Count)
			{
				index = 0;
			}
			Vector3 position = mPositions[index];
			lineRenderer.SetPosition(i, position);
		}
	}

	private void CreateWanderer()
	{
		mWanderer = new GameObject(base.name + "_WANDERER");
		mWanderer.transform.parent = base.transform;
		TrailRenderer trailRenderer = mWanderer.AddComponent<TrailRenderer>();
		trailRenderer.startWidth = TrailWidthStart;
		trailRenderer.endWidth = TrailWidthEnd;
		trailRenderer.material = TrailMaterial;
		trailRenderer.time = TrailTime;
		trailRenderer.gameObject.layer = LayerMask.NameToLayer("StrategyViewModel");
	}

	private void PathfindBetweenPoints(UnityEngine.AI.NavMeshAgent navAgent, int startIndex, int destinationIndex)
	{
		navAgent.enabled = false;
		mWanderer.transform.position = PatrolPoints[startIndex].transform.position;
		navAgent.enabled = true;
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		if (navAgent.CalculatePath(PatrolPoints[destinationIndex].transform.position, navMeshPath))
		{
			for (int i = 1; i < navMeshPath.corners.Length - 1; i++)
			{
				Vector3 item = navMeshPath.corners[i];
				mPositions.Add(item);
			}
		}
	}

	private void SmoothRoute()
	{
		for (int num = mPositions.Count - 1; num >= 1; num--)
		{
			if (mPositions[num] == mPositions[num - 1])
			{
				mPositions.RemoveAt(num);
			}
		}
		List<Vector3> list = new List<Vector3>();
		int num2 = 10;
		for (int i = 0; i < mPositions.Count - 2; i++)
		{
			if (i == mPositions.Count - 3)
			{
				for (int j = 0; j <= num2; j++)
				{
					Vector2 p = new Vector2(mPositions[i].x, mPositions[i].z);
					Vector2 p2 = new Vector2(mPositions[i + 1].x, mPositions[i + 1].z);
					Vector2 p3 = new Vector2(mPositions[i + 2].x, mPositions[i + 2].z);
					Vector2 pointOnCurve = PathSmoothing.GetPointOnCurve(p, p2, p3, (float)j / (float)num2, true);
					list.Add(new Vector3(pointOnCurve.x, mPositions[i + 1].y, pointOnCurve.y));
				}
				continue;
			}
			for (int k = 0; k <= num2; k++)
			{
				Vector2 p4 = new Vector2(mPositions[i].x, mPositions[i].z);
				Vector2 p5 = new Vector2(mPositions[i + 1].x, mPositions[i + 1].z);
				Vector2 p6 = new Vector2(mPositions[i + 2].x, mPositions[i + 2].z);
				Vector2 p7 = new Vector2(mPositions[i + 3].x, mPositions[i + 3].z);
				Vector2 pointOnCurve2 = PathSmoothing.GetPointOnCurve(p4, p5, p6, p7, (float)k / (float)num2);
				list.Add(new Vector3(pointOnCurve2.x, mPositions[i + 1].y, pointOnCurve2.y));
			}
		}
		mPositions.Clear();
		mPositions.AddRange(list);
	}

	private bool ArePatrolPointsValid()
	{
		if (PatrolPoints == null)
		{
			return false;
		}
		foreach (Transform patrolPoint in PatrolPoints)
		{
			if (patrolPoint == null)
			{
				return false;
			}
		}
		if (PatrolPoints.Count < 2)
		{
			return false;
		}
		return true;
	}

	private void OnDrawGizmos()
	{
		List<LineDetail> list = new List<LineDetail>();
		foreach (Transform patrolPoint in PatrolPoints)
		{
			if (patrolPoint != null)
			{
				LineDetail lineDetail = new LineDetail();
				lineDetail.trans = patrolPoint;
				lineDetail.flag = LineFlag.Out;
				list.Add(lineDetail);
			}
		}
		LineHelper.DrawAsConnectedLines(list, true);
	}
}
