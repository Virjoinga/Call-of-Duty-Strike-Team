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
				base.renderer.material.color = Color.green;
			}
			else
			{
				base.renderer.material.color = Color.red;
			}
		}
	}
}
