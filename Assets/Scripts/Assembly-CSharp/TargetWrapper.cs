using UnityEngine;

public class TargetWrapper
{
	public bool IsPartOfASearchArea;

	private Vector3 mPosition;

	private CoverPoint mCoverPoint;

	private bool mSearched;

	private bool mReserved;

	public TargetWrapper(CoverPoint coverPoint)
	{
		mCoverPoint = coverPoint;
		mPosition = coverPoint.GetPosition();
		mSearched = false;
		mReserved = false;
	}

	public TargetWrapper(Vector3 position)
	{
		mPosition = position;
		mSearched = false;
		mReserved = false;
	}

	public CoverPoint InternalCoverPoint()
	{
		return mCoverPoint;
	}

	public Vector3 GetPosition()
	{
		if (mCoverPoint != null)
		{
			return mCoverPoint.GetPosition();
		}
		return mPosition;
	}

	public void Reserve()
	{
		if (!mSearched && !mReserved)
		{
			mReserved = true;
		}
	}

	public bool HasBeenReserved()
	{
		return mReserved;
	}

	public void Search()
	{
		mSearched = true;
		mReserved = false;
	}

	public bool HasBeenSearched()
	{
		return mSearched;
	}

	public void ClearSearch()
	{
		mReserved = false;
		mSearched = false;
	}

	public bool DoesInternalCoverPointMatch(CoverPoint coverPoint)
	{
		if (mCoverPoint != null)
		{
			if (mCoverPoint == coverPoint)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
