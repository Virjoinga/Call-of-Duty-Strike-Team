using System.Collections;
using UnityEngine;

public class ActivateMessageCommand : Command
{
	public float totalDelay = 5f;

	public GameObject[] ObjectsToActivate;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		yield return StartCoroutine(MyWaitFunction(totalDelay));
		postCutsceneActivate();
	}

	private IEnumerator MyWaitFunction(float delay)
	{
		float timer = Time.time + delay;
		while (Time.time < timer)
		{
			yield return null;
		}
	}

	private void postCutsceneActivate()
	{
		GameObject[] objectsToActivate = ObjectsToActivate;
		foreach (GameObject gameObject in objectsToActivate)
		{
			gameObject.SetActive(true);
		}
	}
}
