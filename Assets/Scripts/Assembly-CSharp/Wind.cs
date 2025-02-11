using UnityEngine;

public class Wind : MonoBehaviour
{
	public static Vector3 direction = Vector3.down;

	private void Awake()
	{
		direction = base.transform.forward;
	}
}
