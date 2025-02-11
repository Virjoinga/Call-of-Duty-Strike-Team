using UnityEngine;

public class SendMessage : MonoBehaviour
{
	public MonoBehaviour target;

	public GameObject targetObj;

	public string Message;

	private void Start()
	{
		if ((bool)target)
		{
			target.SendMessage(Message);
		}
		else if ((bool)targetObj)
		{
			targetObj.SendMessage(Message);
		}
	}
}
