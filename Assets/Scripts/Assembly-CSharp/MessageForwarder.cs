using UnityEngine;

public class MessageForwarder : MonoBehaviour
{
	public GameObject Target01;

	public GameObject Target02;

	public string Message;

	private bool isRunning;

	private void Start()
	{
		isRunning = false;
	}

	private void Update()
	{
		if (isRunning)
		{
			Container.SendMessage(Target01, Message, base.gameObject);
			Container.SendMessage(Target02, Message, base.gameObject);
			isRunning = false;
		}
	}

	private void Activate()
	{
		isRunning = true;
	}
}
