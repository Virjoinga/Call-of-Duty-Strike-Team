using UnityEngine;

public class HiddenActive : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
