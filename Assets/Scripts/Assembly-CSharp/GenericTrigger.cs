using System.Collections.Generic;
using UnityEngine;

public class GenericTrigger : MonoBehaviour
{
	public GenericTriggerData m_Interface;

	public List<ActorWrapper> Actors = new List<ActorWrapper>();

	public List<GameObject> ObjectsInTrigger = new List<GameObject>();

	private int actorCount;

	private float mLastTriggerTime = float.MinValue;

	private List<GameObject> cache = new List<GameObject>();

	private bool bTriggered;

	private int lastRandom = -1;

	private void Start()
	{
		bTriggered = false;
	}

	private void Update()
	{
		if (!m_Interface.DeathCountsAsExit || ObjectsInTrigger.Count <= 0)
		{
			return;
		}
		cache.Clear();
		int num = 0;
		while (num < ObjectsInTrigger.Count)
		{
			if (ObjectsInTrigger[num] == null)
			{
				actorCount--;
				ObjectsInTrigger.RemoveAt(num);
				continue;
			}
			Actor componentInChildren = ObjectsInTrigger[num].GetComponentInChildren<Actor>();
			if (componentInChildren.realCharacter.IsDead())
			{
				cache.Add(ObjectsInTrigger[num]);
			}
			num++;
		}
		for (num = 0; num < cache.Count; num++)
		{
			OnTriggerExit(cache[num].collider);
		}
	}

	public void Activate()
	{
		if (m_Interface.OptionalObjectParam == null)
		{
			Container.SendMessageWithParam(m_Interface.ObjectToCall, m_Interface.FunctionToCall, m_Interface.OptionalParm, base.gameObject);
		}
		else
		{
			Container.SendMessageWithParam(m_Interface.ObjectToCall, m_Interface.FunctionToCall, m_Interface.OptionalObjectParam, base.gameObject);
		}
		if (m_Interface.GroupObjectToCall == null || m_Interface.GroupObjectToCall.Count <= 0)
		{
			return;
		}
		int num = 0;
		if (m_Interface.RandomMessage)
		{
			num = -1;
			if (m_Interface.GroupObjectToCall.Count > 1)
			{
				int num2 = 0;
				while (num == lastRandom || num == -1)
				{
					num = Random.Range(0, m_Interface.GroupObjectToCall.Count);
					num2++;
					if (num2 == 10)
					{
						num = 0;
						break;
					}
				}
			}
			else
			{
				num = 0;
			}
			lastRandom = num;
			string message = string.Empty;
			if (m_Interface.GroupFunctionToCall != null && num < m_Interface.GroupFunctionToCall.Count)
			{
				message = m_Interface.GroupFunctionToCall[num];
			}
			Container.SendMessage(m_Interface.GroupObjectToCall[num], message, base.gameObject);
			return;
		}
		foreach (GameObject item in m_Interface.GroupObjectToCall)
		{
			string message = string.Empty;
			if (m_Interface.GroupFunctionToCall != null && num < m_Interface.GroupFunctionToCall.Count)
			{
				message = m_Interface.GroupFunctionToCall[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	public void Deactivate()
	{
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
	}

	public void Enable()
	{
		base.gameObject.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((!bTriggered || !m_Interface.OneShot) && IsUnitOfInterest(other.gameObject))
		{
			if (actorCount == 0)
			{
				GameplayController.Instance().FirstScriptTriggerActor = other.gameObject;
			}
			actorCount++;
			ObjectsInTrigger.Add(other.gameObject);
			if (m_Interface.ObjectToCall == null && m_Interface.GroupObjectToCall == null)
			{
				Debug.LogError("No object to call parameter set to message");
			}
			else if ((!m_Interface.AllActorsRequired || actorCount == Actors.Count) && mLastTriggerTime + m_Interface.RepeatTriggerDelay < Time.time)
			{
				Activate();
				bTriggered = true;
				mLastTriggerTime = Time.time;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsUnitOfInterest(other.gameObject))
		{
			actorCount--;
			ObjectsInTrigger.Remove(other.gameObject);
			mLastTriggerTime = Time.time;
		}
	}

	private bool IsUnitOfInterest(GameObject unitToCheck)
	{
		if (Actors.Count == 0 && unitToCheck.GetComponentInChildren<Actor>() != null)
		{
			if (!(m_Interface.OptionalEntityTag != string.Empty))
			{
				return true;
			}
			Entity component = unitToCheck.GetComponent<Entity>();
			if (component != null && component.Type == m_Interface.OptionalEntityTag)
			{
				return true;
			}
		}
		foreach (ActorWrapper actor in Actors)
		{
			if (actor != null && actor.GetGameObject() == unitToCheck)
			{
				return true;
			}
		}
		return false;
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.yellow.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
