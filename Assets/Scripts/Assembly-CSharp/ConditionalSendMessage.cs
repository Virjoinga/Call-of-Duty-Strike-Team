using UnityEngine;

public class ConditionalSendMessage : MonoBehaviour
{
	public Condition Gate;

	public GameObject Target;

	public string Message;

	private void Awake()
	{
		if (Target == null)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (Gate.Value())
		{
			Container.SendMessage(Target, Message, base.gameObject);
			base.enabled = false;
		}
	}
}
