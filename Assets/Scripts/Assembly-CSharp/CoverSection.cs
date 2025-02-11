using System.Collections.Generic;
using UnityEngine;

public class CoverSection : MonoBehaviour
{
	private List<CoverPoint> coverPoints = new List<CoverPoint>();

	public bool CoverEnabled;

	private bool initialised;

	private void Awake()
	{
		WaypointGameObject[] componentsInChildren = GetComponentsInChildren<WaypointGameObject>();
		WaypointGameObject[] array = componentsInChildren;
		foreach (WaypointGameObject waypoint in array)
		{
			CoverPoint item = new CoverPoint(waypoint);
			coverPoints.Add(item);
		}
		initialised = false;
	}

	private void Update()
	{
		if (!initialised && CoverPointManager.Instance() != null)
		{
			if (CoverEnabled)
			{
				CoverEnabled = false;
				EnableCover();
			}
			initialised = true;
		}
	}

	public void EnableCover()
	{
		if (CoverEnabled)
		{
			return;
		}
		CoverEnabled = true;
		foreach (CoverPoint coverPoint in coverPoints)
		{
			CoverPointManager.Instance().AddCoverPoint(coverPoint);
		}
	}

	public void DisableCover()
	{
		if (!CoverEnabled)
		{
			return;
		}
		CoverEnabled = false;
		foreach (CoverPoint coverPoint in coverPoints)
		{
			CoverPointManager.Instance().RemoveCoverPoint(coverPoint);
		}
	}
}
