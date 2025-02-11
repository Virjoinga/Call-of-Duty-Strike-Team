using UnityEngine;

public class AndroidKeys : MonoBehaviour
{
	public static AndroidKeys Instance;

	public string LVLKey = string.Empty;

	public string AppID = string.Empty;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
}
