using UnityEngine;

public class DebugInterior : MonoBehaviour
{
	public GameObject Exterior;

	public GameObject Interior;

	public GameObject Volume;

	private bool IsInside;

	private void Start()
	{
		IsInside = false;
	}

	private void Update()
	{
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!IsInside && IsPlayer(other.gameObject))
		{
			IsInside = true;
			Exterior.SetActive(false);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (IsInside && IsPlayer(other.gameObject))
		{
			IsInside = false;
			Exterior.SetActive(true);
		}
	}

	private bool IsPlayer(GameObject unitToCheck)
	{
		Entity component = unitToCheck.GetComponent<Entity>();
		return component != null && (component.Type == "Player 1" || component.Type == "Player 2" || component.Type == "Player 3" || component.Type == "Player 4");
	}
}
