using System.Collections.Generic;
using UnityEngine;

public class SearchArea
{
	private const float kDefaultSearchSqrRadius = 200f;

	private const int kMinPointsForAValidSearchArea = 15;

	private Color mColor;

	private float mSqrRadius;

	private Vector3 mPosition;

	private TargetWrapperManager mTWMInstance = TargetWrapperManager.Instance();

	private List<TargetWrapper> mTargets;

	private bool mDynamicSearchArea;

	private static int mQtyOfDynamicTargetsToTry = 100;

	private static int mQtyOfDynamicTargetsToGenerate = 15;

	private GameObject mNavTester;

	private NavMeshAgent mNavTesterAgent;

	public float SqrRadius
	{
		get
		{
			return mSqrRadius;
		}
	}

	public bool DynamicSearchArea
	{
		get
		{
			return mDynamicSearchArea;
		}
	}

	public SearchArea(Vector3 position, bool bDynamic)
		: this(position, 200f, bDynamic)
	{
	}

	public SearchArea(Vector3 position, float sqrRadius, bool bDynamic)
	{
		mSqrRadius = sqrRadius;
		mPosition = position;
		mTWMInstance = TargetWrapperManager.Instance();
		mTargets = new List<TargetWrapper>();
		if (bDynamic)
		{
			mDynamicSearchArea = true;
			GenerateDynamicSearchArea();
		}
		else
		{
			mDynamicSearchArea = false;
			GenerateSearchArea();
		}
		CreateNavTester();
		mColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	}

	public void SetSearchAreaRadius(float sqrRadius, bool bDynamic)
	{
		mSqrRadius = sqrRadius;
		if (bDynamic)
		{
			GenerateDynamicSearchArea();
		}
		else
		{
			GenerateSearchArea();
		}
	}

	public void SetSearchAreaPosition(Vector3 position, bool bDynamic)
	{
		mPosition = position;
		if (bDynamic)
		{
			GenerateDynamicSearchArea();
		}
		else
		{
			GenerateSearchArea();
		}
	}

	public void SetNewSearchArea(Vector3 position, bool bDynamic)
	{
		mSqrRadius = 200f;
		mPosition = position;
		if (bDynamic)
		{
			GenerateDynamicSearchArea();
		}
		else
		{
			GenerateSearchArea();
		}
	}

	public void SetNewSearchArea(Vector3 position, float sqrRadius, bool bDynamic)
	{
		mSqrRadius = sqrRadius;
		mPosition = position;
		if (bDynamic)
		{
			GenerateDynamicSearchArea();
		}
		else
		{
			GenerateSearchArea();
		}
	}

	public void SetNewSearchArea()
	{
		mTargets.Clear();
		mTargets = mTWMInstance.TargetWrappers;
		if (mTargets == null || mTargets.Count < 15)
		{
			mTargets = CreateDynamicSearchArea(mPosition, mSqrRadius);
		}
	}

	public void GenerateDynamicSearchArea()
	{
		mTargets.Clear();
		mTargets = CreateDynamicSearchArea(mPosition, mSqrRadius);
	}

	private void GenerateSearchArea()
	{
		mTargets.Clear();
		bool SuccessfulSetup = false;
		List<TargetWrapper> allTargetsInRadius = mTWMInstance.GetAllTargetsInRadius(mPosition, mSqrRadius);
		if (allTargetsInRadius.Count > 15)
		{
			SetValidTargets(allTargetsInRadius, out SuccessfulSetup);
		}
		if (!SuccessfulSetup || mTargets.Count < 15)
		{
			mTargets = CreateDynamicSearchArea(mPosition, mSqrRadius);
		}
	}

	private List<TargetWrapper> CreateDynamicSearchArea(Vector3 position, float sqrRadius)
	{
		List<TargetWrapper> list = new List<TargetWrapper>();
		mDynamicSearchArea = true;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < mQtyOfDynamicTargetsToGenerate; i++)
		{
			Vector2 vector = Random.insideUnitCircle * Mathf.Sqrt(sqrRadius);
			zero.x = vector.x;
			zero.y = 0f;
			zero.z = vector.y;
			NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(position + zero);
			if (navMeshHit.hit)
			{
				float sqrMagnitude = (navMeshHit.position - mPosition).sqrMagnitude;
				if (sqrMagnitude < sqrRadius)
				{
					list.Add(new TargetWrapper(navMeshHit.position));
				}
			}
			if (mTargets.Count >= mQtyOfDynamicTargetsToTry)
			{
				return list;
			}
		}
		return list;
	}

	private void SetValidTargets(List<TargetWrapper> availableTargets, out bool SuccessfulSetup)
	{
		if (availableTargets == null || availableTargets.Count == 0)
		{
			SuccessfulSetup = false;
			return;
		}
		SuccessfulSetup = true;
		mTargets.Clear();
		List<TargetWrapper> list = new List<TargetWrapper>();
		foreach (TargetWrapper availableTarget in availableTargets)
		{
			RaycastHit hitInfo;
			if (!Physics.Linecast(availableTarget.GetPosition(), mPosition, out hitInfo))
			{
				mTargets.Add(availableTarget);
				list.Add(availableTarget);
			}
		}
		if (mTargets.Count > 0)
		{
			foreach (TargetWrapper item in list)
			{
				availableTargets.Remove(item);
			}
			{
				foreach (TargetWrapper availableTarget2 in availableTargets)
				{
					foreach (TargetWrapper mTarget in mTargets)
					{
						if (!Physics.Linecast(availableTarget2.GetPosition(), mTarget.GetPosition()))
						{
							mTargets.Add(availableTarget2);
							break;
						}
					}
				}
				return;
			}
		}
		SuccessfulSetup = false;
	}

	public TargetWrapper GetNearestUnsearchedTargetCheap(Vector3 position)
	{
		float num = float.MaxValue;
		TargetWrapper result = null;
		if (mNavTester == null)
		{
			return null;
		}
		mNavTester.SetActive(true);
		mNavTester.transform.position = position;
		if (mNavTesterAgent == null)
		{
			mNavTesterAgent = mNavTester.AddComponent<NavMeshAgent>();
			ActorGenerator.ConfigureNavMeshAgent(mNavTesterAgent, 0f, true, 360f);
		}
		mNavTesterAgent.enabled = true;
		foreach (TargetWrapper mTarget in mTargets)
		{
			if (mTarget.HasBeenSearched() || mTarget.HasBeenReserved())
			{
				continue;
			}
			NavMeshPath navMeshPath = new NavMeshPath();
			if (!mNavTesterAgent.CalculatePath(mTarget.GetPosition(), navMeshPath) || navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				float num2 = 0f;
				if (mDynamicSearchArea)
				{
					return mTarget;
				}
				num2 = CoverPointManager.Instance().GetTravelDistanceCheap(position, mTarget.InternalCoverPoint(), null);
				if (num2 < num)
				{
					num = num2;
					result = mTarget;
				}
			}
		}
		mNavTesterAgent.enabled = false;
		mNavTester.SetActive(false);
		return result;
	}

	public void RefreshUnreservedTargets()
	{
		foreach (TargetWrapper mTarget in mTargets)
		{
			if (!mTarget.HasBeenReserved())
			{
				mTarget.ClearSearch();
			}
		}
	}

	public void RefreshAllTargets()
	{
		foreach (TargetWrapper mTarget in mTargets)
		{
			mTarget.ClearSearch();
		}
	}

	public void GLDebugVisualise()
	{
		GL.Color(mColor);
		foreach (TargetWrapper mTarget in mTargets)
		{
			GL.Vertex3(mPosition.x, mPosition.y, mPosition.z);
			GL.Vertex3(mTarget.GetPosition().x, mTarget.GetPosition().y, mTarget.GetPosition().z);
		}
	}

	private void CreateNavTester()
	{
		mNavTester = new GameObject("Cover Eval Nav Tester");
		mNavTesterAgent = null;
		mNavTester.SetActive(false);
	}
}
