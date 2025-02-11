using System;
using UnityEngine;

[Serializable]
public class MysteryCacheControllerData
{
	public int MaxActiveSimultaneous = 1;

	public float TimeBetweenActivation = 30f;

	public float TimeVariation;

	public void FindMysteryCaches(MysteryCacheController cont)
	{
		GameObject gameObject = Container.GetContainerFromObject(cont.gameObject) as GameObject;
		if (gameObject != null)
		{
			cont.mMysteryCaches.AddRange(gameObject.GetComponentsInChildren<MysteryCache>());
		}
	}
}
