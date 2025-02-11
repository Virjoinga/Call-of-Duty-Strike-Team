using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetRangeEnemyData
{
	public float Speed = 1f;

	public List<GameObject> PathPoints = new List<GameObject>();

	public PathObject_TS.PathType pathType;

	public List<GameObject> MsgOnFinished = new List<GameObject>();

	public List<string> Message = new List<string>();

	public bool lookAlongPath = true;

	public bool MovingAtStart;

	public bool TargetSnap;

	public bool TargetRangeRotation;

	public int WaitTime;

	public GameObject ActivateOnEnemyTargetShot;

	public GameObject ActivateOnFriendlyTargetShot;

	public void CopyContainerData(PathObject_TS h)
	{
		h.Speed = Speed;
		h.PathPoints.Clear();
		foreach (GameObject pathPoint in PathPoints)
		{
			if (pathPoint != null)
			{
				h.PathPoints.Add(pathPoint);
			}
		}
		h.pathType = pathType;
		h.MsgOnFinished.Clear();
		foreach (GameObject item in MsgOnFinished)
		{
			if (item != null)
			{
				h.MsgOnFinished.Add(item);
			}
		}
		h.Message.Clear();
		foreach (string item2 in Message)
		{
			if (item2 != null)
			{
				h.Message.Add(item2);
			}
		}
		h.lookAlongPath = lookAlongPath;
		h.MovingAtStart = MovingAtStart;
		h.TargetSnap = TargetSnap;
		h.TargetRangeRotation = TargetRangeRotation;
		h.WaitTime = WaitTime;
		h.ActivateOnEnemyTargetShot = ActivateOnEnemyTargetShot;
		h.ActivateOnFriendlyTargetShot = ActivateOnFriendlyTargetShot;
	}
}
