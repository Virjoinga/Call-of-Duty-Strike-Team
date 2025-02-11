using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectBroadcaster
{
	public GameObject Target;

	public string Function;

	public List<GameObject> Targets = new List<GameObject>();

	public List<string> Functions = new List<string>();

	public List<GameObject> Turnonables;

	public List<GameObject> Turnoffables;

	public void SendMessages()
	{
		if (Target != null)
		{
			Container.SendMessage(Target, Function);
		}
		string empty = string.Empty;
		int num = 0;
		foreach (GameObject target in Targets)
		{
			empty = string.Empty;
			if (num < Functions.Count)
			{
				empty = Functions[num];
			}
			Container.SendMessage(target, empty);
			num++;
		}
	}

	public void ActivateTurnonables()
	{
		if (Turnonables == null || Turnonables.Count <= 0)
		{
			return;
		}
		foreach (GameObject turnonable in Turnonables)
		{
			turnonable.SetActive(true);
		}
	}

	public void DeactivateTurnoffables()
	{
		if (Turnoffables == null || Turnoffables.Count <= 0)
		{
			return;
		}
		foreach (GameObject turnoffable in Turnoffables)
		{
			turnoffable.SetActive(false);
		}
	}
}
