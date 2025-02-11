using UnityEngine;

public class TowerDestroyedCheck : MonoBehaviour
{
	public GameObject RadioTower;

	public GameObject[] Spawners;

	private void OnTriggerEnter(Collider col)
	{
		if (RadioTower.activeInHierarchy)
		{
			GameObject[] spawners = Spawners;
			foreach (GameObject target in spawners)
			{
				Container.SendMessage(target, "Activate");
			}
		}
	}
}
