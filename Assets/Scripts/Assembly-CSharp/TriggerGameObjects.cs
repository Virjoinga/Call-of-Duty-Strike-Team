using System.Collections.Generic;
using UnityEngine;

public class TriggerGameObjects : MonoBehaviour
{
	public List<GameObject> Turnonables;

	private bool mActive;

	private void Start()
	{
		mActive = true;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!mActive || Turnonables == null)
		{
			return;
		}
		Actor component = other.GetComponent<Actor>();
		if (!(component != null) || !component.behaviour.PlayerControlled)
		{
			return;
		}
		foreach (GameObject turnonable in Turnonables)
		{
			turnonable.SetActive(true);
		}
		mActive = false;
	}

	private void OnTriggerStay(Collider other)
	{
	}
}
