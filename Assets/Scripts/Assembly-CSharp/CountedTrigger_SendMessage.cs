using System.Collections;
using UnityEngine;

public class CountedTrigger_SendMessage : MonoBehaviour
{
	public enum Mode
	{
		Trigger = 0,
		Activate = 1,
		Deactivate = 2,
		SendMessage = 3
	}

	public GameObject[] Targets;

	public int CountTarget;

	private int presentCount;

	public float delayTimer;

	private bool hitTarget;

	private bool Overridden;

	public string MessageToSend;

	public Mode action = Mode.Activate;

	private void Awake()
	{
		if (Targets[0] == null)
		{
			base.enabled = false;
		}
	}

	private void Count()
	{
		if (!Overridden)
		{
			presentCount++;
		}
		if (presentCount == CountTarget && !hitTarget)
		{
			hitTarget = true;
			StartCoroutine("DelayAndFireMessages");
		}
	}

	private void Override()
	{
		Overridden = true;
		StartCoroutine("DelayAndFireMessages");
	}

	private IEnumerator DelayAndFireMessages()
	{
		yield return new WaitForSeconds(delayTimer);
		GameObject[] targets = Targets;
		foreach (GameObject target in targets)
		{
			Object currentTarget = ((!(target != null)) ? base.gameObject : target);
			Behaviour targetBehaviour = currentTarget as Behaviour;
			GameObject targetGameObject = currentTarget as GameObject;
			if (targetBehaviour != null)
			{
				targetGameObject = targetBehaviour.gameObject;
			}
			switch (action)
			{
			case Mode.Trigger:
				targetGameObject.BroadcastMessage("DoActivateTrigger");
				break;
			case Mode.Activate:
				targetGameObject.SetActive(true);
				break;
			case Mode.Deactivate:
				targetGameObject.SetActive(false);
				break;
			case Mode.SendMessage:
				Container.SendMessage(target, MessageToSend, base.gameObject);
				break;
			}
		}
		base.enabled = false;
	}
}
