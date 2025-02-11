using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kowloon2_HelicopterRevealTrigger : MonoBehaviour
{
	public List<ActorWrapper> Monitors;

	public GameObject[] Targets;

	private int Counter;

	public int targetCount;

	private bool HasHitCounter;

	private void Start()
	{
		Counter = 0;
	}

	private void OnTriggerEnter(Collider col)
	{
		Debug.Log("Helicopter flies!");
		if (IsUnitOfInterest(col.gameObject) && HasHitCounter)
		{
			GameObject[] targets = Targets;
			foreach (GameObject gameObject in targets)
			{
				gameObject.SetActive(false);
			}
		}
		Delayer();
	}

	private void Count()
	{
		Counter++;
		if (Counter == targetCount)
		{
			HasHitCounter = true;
		}
	}

	private IEnumerator Delayer()
	{
		yield return new WaitForSeconds(14f);
		GameObject[] targets = Targets;
		foreach (GameObject target in targets)
		{
			target.SetActive(false);
		}
	}

	private bool IsUnitOfInterest(GameObject unitToCheck)
	{
		foreach (ActorWrapper monitor in Monitors)
		{
			if (monitor != null && monitor.gameObject == unitToCheck)
			{
				return true;
			}
		}
		return false;
	}
}
