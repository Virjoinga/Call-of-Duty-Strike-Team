using UnityEngine;

public class NavPortalTest : MonoBehaviour
{
	public GameObject Target;

	private void Start()
	{
	}

	private void Update()
	{
		UnityEngine.AI.NavMeshAgent component = GetComponent<UnityEngine.AI.NavMeshAgent>();
		component.destination = Target.transform.position;
		if (Time.timeSinceLevelLoad > 5f)
		{
			component.walkableMask |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Portal1");
		}
		if (Time.timeSinceLevelLoad > 10f)
		{
			component.walkableMask |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Portal2");
		}
		if (Time.timeSinceLevelLoad > 15f)
		{
			component.walkableMask |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Portal3");
		}
	}
}
