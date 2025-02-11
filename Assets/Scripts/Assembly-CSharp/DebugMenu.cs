using UnityEngine;

public class DebugMenu : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(this);
	}

	public static bool IsInDetectionZone(Vector2 position)
	{
		return false;
	}
}
