using UnityEngine;

public class SpawnReference : MonoBehaviour
{
	public GameObject reference;

	public GameObject spawned;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Vector3 center = base.transform.position + new Vector3(0f, 0.5f, 0f);
		Gizmos.color = Color.red;
		Gizmos.DrawIcon(center, "PlayerSpawn");
	}

	public void ProcessSpawn()
	{
		if (reference != null)
		{
			Spawner component = reference.GetComponent<Spawner>();
			if (component != null)
			{
				Transform transform = base.transform;
				GameObject gameObject = SceneNanny.CreateExtra(component.Spawn, transform.position, transform.rotation);
				BaseCharacter component2 = gameObject.GetComponent<BaseCharacter>();
				component2.EnableNavMesh(true);
			}
		}
	}
}
