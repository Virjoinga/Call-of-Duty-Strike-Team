using System.Collections.Generic;
using UnityEngine;

public class EventOnBecomeVisble : EventDescriptor
{
	private List<Renderer> mRenderer = new List<Renderer>();

	public float DistanceCheck = 25f;

	private float mDistanceCheckSqr;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		GameObject model = gameObj.GetComponent<Actor>().model;
		int num = model.name.IndexOf("(Clone)");
		string text = ((num >= 0) ? model.name.Remove(num) : model.name);
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name.Contains(text))
			{
				mRenderer.Add(componentsInChildren[i]);
			}
		}
		mDistanceCheckSqr = DistanceCheck * DistanceCheck;
		if (mRenderer.Count == 0)
		{
			Debug.LogWarning("Warning, no renderer attached for is on screen event: " + text);
			Object.Destroy(this);
		}
	}

	public void Update()
	{
		if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera || CinematicHelper.IsInCinematic)
		{
			return;
		}
		for (int i = 0; i < mRenderer.Count; i++)
		{
			if (mRenderer[i].isVisible)
			{
				float num = Vector3.SqrMagnitude(mRenderer[i].transform.position - CameraManager.Instance.CurrentCamera.transform.position);
				if (num < mDistanceCheckSqr)
				{
					FireEvent();
				}
			}
		}
	}
}
