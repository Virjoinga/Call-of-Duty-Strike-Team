using System.Collections.Generic;
using UnityEngine;

public class DebugWorldTracker : MonoBehaviour
{
	private static bool mAllEnabled = true;

	private static List<DebugWorldTracker> mTrackers = new List<DebugWorldTracker>();

	private int HistoryLength = 180;

	public Color TrackColour = Color.red;

	private Vector3 MarkerSize = new Vector3(0.1f, 0.1f, 0.1f);

	private Vector3[] mPosHistory;

	private int mHistoryIdx;

	private bool mHistoryFull;

	public static void RepositionWithWorldRecentre(Vector3 offset)
	{
		foreach (DebugWorldTracker mTracker in mTrackers)
		{
			mTracker.RepositionWithWorldRecentreLocal(offset);
		}
	}

	public void RepositionWithWorldRecentreLocal(Vector3 offset)
	{
		for (int i = 0; i < HistoryLength; i++)
		{
			mPosHistory[i] -= offset;
		}
	}

	private void Start()
	{
		mTrackers.Add(this);
		mPosHistory = new Vector3[HistoryLength];
		mHistoryIdx = 0;
		mHistoryFull = false;
		if (!Application.isEditor)
		{
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		if (mAllEnabled)
		{
			mPosHistory[mHistoryIdx] = base.gameObject.transform.position;
			mHistoryIdx++;
			if (mHistoryIdx >= HistoryLength)
			{
				mHistoryFull = true;
				mHistoryIdx = 0;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!mAllEnabled)
		{
			return;
		}
		Gizmos.color = TrackColour;
		Vector3 from = Vector3.zero;
		bool flag = false;
		if (mHistoryFull)
		{
			Vector3[] array = mPosHistory;
			foreach (Vector3 vector in array)
			{
				DrawMarker(vector);
				if (flag)
				{
					DrawLink(from, vector);
				}
				flag = true;
				from = vector;
			}
		}
		else
		{
			for (int j = 0; j < mHistoryIdx; j++)
			{
				Vector3 vector2 = mPosHistory[j];
				DrawMarker(vector2);
				if (flag)
				{
					DrawLink(from, vector2);
				}
				flag = true;
				from = vector2;
			}
		}
		Gizmos.color = Color.white;
	}

	private void DrawMarker(Vector3 pos)
	{
		Gizmos.DrawCube(pos, MarkerSize);
	}

	private void DrawLink(Vector3 from, Vector3 to)
	{
		Gizmos.DrawLine(from, to);
	}
}
