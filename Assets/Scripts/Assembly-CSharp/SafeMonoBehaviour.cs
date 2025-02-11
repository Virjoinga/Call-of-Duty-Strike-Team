using UnityEngine;

public class SafeMonoBehaviour : MonoBehaviour
{
	protected static bool IsShuttingDown { get; private set; }

	protected virtual void OnApplicationQuit()
	{
		IsShuttingDown = true;
	}
}
