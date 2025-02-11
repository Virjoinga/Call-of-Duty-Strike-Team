using System.Collections.Generic;
using UnityEngine;

public class MoveMagnet_KM : MonoBehaviour
{
	public List<GameObject> MagnetLocations;

	private int i;

	public bool loop;

	private void Activate()
	{
		if (MagnetLocations != null)
		{
			if (i >= MagnetLocations.Count && loop)
			{
				i = 0;
			}
			if (i < MagnetLocations.Count)
			{
				base.transform.position = new Vector3(MagnetLocations[i].transform.position.x, MagnetLocations[i].transform.position.y, MagnetLocations[i].transform.position.z);
				i++;
			}
		}
	}
}
