using System.Collections.Generic;
using UnityEngine;

public class Explosion
{
	private Vector3 mOrigin;

	private float mRadiusSq;

	private List<Vector3> mThreads;

	public Explosion(Vector3 origin, float radius)
	{
		mOrigin = origin;
		mRadiusSq = radius * radius;
		mThreads = new List<Vector3>();
		Vector3 vector = new Vector3(0f, 0f, 1f);
		for (int i = 0; i < 36; i++)
		{
			Vector3 item = mOrigin + vector;
			mThreads.Add(item);
			Quaternion quaternion = Quaternion.AngleAxis(10f, Vector3.up);
			vector = quaternion * vector;
		}
	}

	public void Update()
	{
		for (int i = 0; i < mThreads.Count; i++)
		{
			Vector3 normalized = (mThreads[i] - mOrigin).normalized;
			List<Vector3> list;
			List<Vector3> list2 = (list = mThreads);
			int index;
			int index2 = (index = i);
			Vector3 vector = list[index];
			list2[index2] = vector + normalized * Time.deltaTime * 10f;
		}
	}

	public bool HasExpired()
	{
		if ((mThreads[0] - mOrigin).sqrMagnitude >= mRadiusSq)
		{
			return true;
		}
		return false;
	}

	public void GLDebugVisualise()
	{
	}
}
