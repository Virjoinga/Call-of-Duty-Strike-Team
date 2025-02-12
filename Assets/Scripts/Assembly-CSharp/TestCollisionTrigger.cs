using UnityEngine;

public class TestCollisionTrigger : MonoBehaviour
{
	public AwarenessZone zone;

	private void Start()
	{
	}

	private void Update()
	{
		if (zone != null)
		{
			if (zone.Contains(base.transform.position) != 0)
			{
				base.GetComponent<Renderer>().material.color = Color.green;
			}
			else
			{
				base.GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}
}
