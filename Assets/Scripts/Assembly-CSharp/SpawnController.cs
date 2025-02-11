using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	public SpawnControllerData m_Interface;

	[HideInInspector]
	public List<Spawner> Spawners;

	private bool bSpawned;

	private void Awake()
	{
		foreach (Spawner spawner in Spawners)
		{
			spawner.Gate = base.gameObject.AddComponent<Condition_Scripted>();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Activate()
	{
		DoSpawn();
	}

	public void Deactivate()
	{
	}

	public void CleanupAssociatedActors()
	{
		foreach (Spawner spawner in Spawners)
		{
			if (spawner.Spawn != null)
			{
				Actor component = spawner.spawned.GetComponent<Actor>();
				TaskManager tasks = component.tasks;
				Object.Destroy(component.model.gameObject);
				if (component.realCharacter.Ragdoll != null)
				{
					Object.Destroy(component.realCharacter.Ragdoll.gameObject);
				}
				Object.Destroy(tasks.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsValidMonitorObject(other.gameObject))
		{
			DoSpawn();
		}
	}

	private void DoSpawn()
	{
		if (!bSpawned)
		{
			StartCoroutine(HandleSpawn());
			bSpawned = true;
		}
	}

	private IEnumerator HandleSpawn()
	{
		foreach (Spawner s in Spawners)
		{
			s.ProcessSpawn();
			if (!m_Interface.SpawnImmediate)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		FinishSpawn();
	}

	private void FinishSpawn()
	{
		if (m_Interface.StopProcessOnActive)
		{
			Object.Destroy(this);
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectsToCallOnSpawn)
		{
			if (m_Interface.FunctionsToCallOnSpawn.Count > num && m_Interface.FunctionsToCallOnSpawn[num] != string.Empty && item != null)
			{
				Container.SendMessage(item, m_Interface.FunctionsToCallOnSpawn[num], base.gameObject);
			}
			else
			{
				Debug.LogWarning("ObjectToCall or FunctionToCall from " + base.name + " is null or empty");
			}
			num++;
		}
	}

	private bool IsValidMonitorObject(GameObject check)
	{
		Entity component = check.GetComponent<Entity>();
		return component != null && (component.Type == "Player 1" || component.Type == "Player 2" || component.Type == "Player 3" || component.Type == "Player 4");
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.red.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
