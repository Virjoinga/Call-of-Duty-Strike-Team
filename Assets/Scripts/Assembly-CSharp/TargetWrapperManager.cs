using System.Collections.Generic;
using UnityEngine;

public class TargetWrapperManager : MonoBehaviour
{
	public static TargetWrapperManager instance;

	private List<TargetWrapper> mTargets;

	private bool mReady;

	public List<TargetWrapper> TargetWrappers
	{
		get
		{
			return mTargets;
		}
	}

	public static TargetWrapperManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mTargets = new List<TargetWrapper>();
	}

	private void Start()
	{
		mReady = false;
	}

	private void Update()
	{
		if (!mReady)
		{
			RegisterTargets();
			mReady = true;
		}
	}

	private void RegisterTargets()
	{
		mTargets.Clear();
		foreach (CoverPoint coverPoint in CoverPointManager.Instance().CoverPoints)
		{
			TargetWrapper item = new TargetWrapper(coverPoint);
			mTargets.Add(item);
		}
	}

	public TargetWrapper GetTarget(CoverPoint coverPoint)
	{
		foreach (TargetWrapper mTarget in mTargets)
		{
			if (mTarget.DoesInternalCoverPointMatch(coverPoint))
			{
				return mTarget;
			}
		}
		return null;
	}

	public void RefreshTargets()
	{
		foreach (TargetWrapper mTarget in mTargets)
		{
			mTarget.ClearSearch();
		}
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

	public TargetWrapper GetNearestUnsearchedTarget(Vector3 position)
	{
		float num = float.MaxValue;
		TargetWrapper result = null;
		foreach (TargetWrapper mTarget in mTargets)
		{
			if (!mTarget.HasBeenSearched() && !mTarget.HasBeenReserved())
			{
				float travelDistance = CoverPointManager.Instance().GetTravelDistance(position, mTarget.InternalCoverPoint(), null);
				if (travelDistance < num)
				{
					num = travelDistance;
					result = mTarget;
				}
			}
		}
		return result;
	}

	public TargetWrapper GetNearestUnsearchedTargetCheap(Vector3 position)
	{
		float num = float.MaxValue;
		TargetWrapper result = null;
		foreach (TargetWrapper mTarget in mTargets)
		{
			if (!mTarget.HasBeenSearched() && !mTarget.HasBeenReserved())
			{
				float travelDistanceCheap = CoverPointManager.Instance().GetTravelDistanceCheap(position, mTarget.InternalCoverPoint(), null);
				if (travelDistanceCheap < num)
				{
					num = travelDistanceCheap;
					result = mTarget;
				}
			}
		}
		return result;
	}

	public void AutoSearchNearbyVisibleTargetWrappers(Vector3 position)
	{
		AutoSearchNearbyVisibleTargetWrappers(position, 25f);
	}

	public void AutoSearchNearbyVisibleTargetWrappers(Vector3 position, float sqrRadius)
	{
		foreach (TargetWrapper targetWrapper in TargetWrappers)
		{
			float sqrMagnitude = (targetWrapper.GetPosition() - position).sqrMagnitude;
			if (sqrMagnitude < sqrRadius && !Physics.Linecast(position, targetWrapper.GetPosition()))
			{
				targetWrapper.Search();
			}
		}
	}

	public void AutoClearNearbyVisibleTargetWrappers(Vector3 position)
	{
		AutoClearNearbyVisibleTargetWrappers(position, 100f);
	}

	public void AutoClearNearbyVisibleTargetWrappers(Vector3 position, float sqrRadius)
	{
		foreach (TargetWrapper targetWrapper in TargetWrappers)
		{
			float sqrMagnitude = (targetWrapper.GetPosition() - position).sqrMagnitude;
			if (sqrMagnitude < sqrRadius && !Physics.Linecast(position, targetWrapper.GetPosition()))
			{
				targetWrapper.ClearSearch();
			}
		}
	}

	public List<TargetWrapper> GetAllTargetsInRadius(Vector3 position, float sqrRadius)
	{
		List<TargetWrapper> list = new List<TargetWrapper>();
		foreach (TargetWrapper targetWrapper in TargetWrappers)
		{
			float sqrMagnitude = (targetWrapper.GetPosition() - position).sqrMagnitude;
			if (sqrMagnitude < sqrRadius)
			{
				list.Add(targetWrapper);
			}
		}
		return list;
	}
}
