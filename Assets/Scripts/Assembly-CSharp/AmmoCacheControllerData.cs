using System;
using UnityEngine;

[Serializable]
public class AmmoCacheControllerData
{
	public int MaxActiveSimultaneous = 1;

	public float TimeBetweenActivation = 30f;

	public float TimeVariation;

	public void FindAmmoCaches(AmmoCacheController cont)
	{
		GameObject gameObject = Container.GetContainerFromObject(cont.gameObject) as GameObject;
		if (gameObject != null)
		{
			cont.mAmmoCaches.AddRange(gameObject.GetComponentsInChildren<AmmoCache>());
		}
	}
}
