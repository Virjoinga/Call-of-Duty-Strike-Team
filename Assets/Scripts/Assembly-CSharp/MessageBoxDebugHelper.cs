using UnityEngine;

public class MessageBoxDebugHelper : MonoBehaviour
{
	public MessageBox MsgBoxPrefab;

	public string Title = "Hello world";

	public string BodyText = "This is my text for testing the body of a message box";

	public Vector2 MinSize = new Vector2(-1f, -1f);

	public bool AutoSize = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			MessageBox messageBox = (MessageBox)Object.Instantiate(MsgBoxPrefab);
			StartCoroutine(messageBox.Display(Title, BodyText, false));
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			CommonHudController.Instance.AddXpFeedback(500, "it works", null);
		}
	}
}
