using UnityEngine;

public class ActivateMessage : MonoBehaviour
{
	public GameObject[] ObjectsToActivate;

	private void Activate()
	{
		GameObject[] objectsToActivate = ObjectsToActivate;
		foreach (GameObject gameObject in objectsToActivate)
		{
			gameObject.SetActive(true);
		}
	}
}
