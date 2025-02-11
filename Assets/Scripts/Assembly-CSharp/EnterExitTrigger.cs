using System;
using UnityEngine;

public class EnterExitTrigger : MonoBehaviour
{
	[Serializable]
	public class MessagePair
	{
		public GameObject Receiver;

		public string Message;
	}

	public GameObject[] ActivateOnEnter;

	public GameObject[] DeactivateOnEnter;

	public MessagePair[] MessageOnEnter;

	public GameObject[] ActivateOnExit;

	public GameObject[] DeactivateOnExit;

	public MessagePair[] MessageOnExit;

	public ActorWrapper[] TriggerActors;

	public GameObject GateTest;

	private void OnTriggerEnter(Collider other)
	{
		if (IsUnitOfInterest(other.gameObject))
		{
			GameObject[] activateOnEnter = ActivateOnEnter;
			foreach (GameObject gameObject in activateOnEnter)
			{
				gameObject.SetActive(true);
			}
			GameObject[] deactivateOnEnter = DeactivateOnEnter;
			foreach (GameObject gameObject2 in deactivateOnEnter)
			{
				gameObject2.SetActive(false);
			}
			MessagePair[] messageOnEnter = MessageOnEnter;
			foreach (MessagePair messagePair in messageOnEnter)
			{
				messagePair.Receiver.SendMessage(messagePair.Message);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsUnitOfInterest(other.gameObject))
		{
			GameObject[] activateOnExit = ActivateOnExit;
			foreach (GameObject gameObject in activateOnExit)
			{
				gameObject.SetActive(true);
			}
			GameObject[] deactivateOnExit = DeactivateOnExit;
			foreach (GameObject gameObject2 in deactivateOnExit)
			{
				gameObject2.SetActive(false);
			}
			MessagePair[] messageOnExit = MessageOnExit;
			foreach (MessagePair messagePair in messageOnExit)
			{
				messagePair.Receiver.SendMessage(messagePair.Message);
			}
		}
	}

	private bool IsUnitOfInterest(GameObject unitToCheck)
	{
		ActorWrapper[] triggerActors = TriggerActors;
		foreach (ActorWrapper actorWrapper in triggerActors)
		{
			if (actorWrapper != null && actorWrapper.GetGameObject() == unitToCheck)
			{
				return true;
			}
		}
		return false;
	}
}
