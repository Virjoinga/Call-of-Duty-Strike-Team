using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuDistanceManager : MonoBehaviour
{
	public const float kMaxRadius = 1024f;

	public List<GameObject> ContextMenuParents;

	public float Radius;

	private float m_fRadiusSqr;

	private bool mForceUpdate;

	private bool m_bActivated;

	private InterfaceableObject[] mContextMenuObjects;

	private bool[] mActivated;

	private List<Actor> mTriggerActors = new List<Actor>(4);

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private bool mCoroutineRunning;

	public float RadiusSqr
	{
		get
		{
			return m_fRadiusSqr;
		}
	}

	public void ForceUpdate()
	{
		mForceUpdate = true;
	}

	private void Start()
	{
		if (ContextMenuParents == null || ContextMenuParents.Count == 0)
		{
			ContextMenuParents = new List<GameObject>();
			ContextMenuParents.Add(base.gameObject);
		}
		List<InterfaceableObject> list = new List<InterfaceableObject>();
		foreach (GameObject contextMenuParent in ContextMenuParents)
		{
			if (contextMenuParent == null)
			{
				continue;
			}
			Component[] componentsInChildren = contextMenuParent.GetComponentsInChildren(typeof(InterfaceableObject), true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				InterfaceableObject interfaceableObject = (InterfaceableObject)array[i];
				list.Add(interfaceableObject);
				if (interfaceableObject.DistanceManager == null)
				{
					interfaceableObject.DistanceManager = this;
				}
			}
		}
		mActivated = new bool[list.Count];
		mContextMenuObjects = new InterfaceableObject[list.Count];
		list.CopyTo(mContextMenuObjects);
		m_fRadiusSqr = Radius * Radius;
		StartCoroutine("CheckDistance");
	}

	private void OnEnable()
	{
		if (!mCoroutineRunning)
		{
			StartCoroutine("CheckDistance");
		}
	}

	private void OnDisable()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnFirstPersonActorChange -= ClearContextBlips;
			if (mCoroutineRunning)
			{
				StopCoroutine("CheckDistance");
				mCoroutineRunning = false;
			}
		}
	}

	private void OnDestroy()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnFirstPersonActorChange -= ClearContextBlips;
		}
	}

	public void ChangeRadius(float radius)
	{
		Radius = radius;
		m_fRadiusSqr = Radius * Radius;
		if (m_bActivated)
		{
			ClearContextBlips();
		}
	}

	private void ClearContextBlips()
	{
		for (int i = 0; i < mContextMenuObjects.Length; i++)
		{
			if (mContextMenuObjects[i] != null)
			{
				mContextMenuObjects[i].TurnOff();
			}
			mActivated[i] = false;
		}
		m_bActivated = false;
	}

	private IEnumerator CheckDistance()
	{
		mCoroutineRunning = true;
		yield return null;
		GameController.Instance.OnFirstPersonActorChange += ClearContextBlips;
		ClearContextBlips();
		yield return null;
		WaitForSeconds wfs = new WaitForSeconds(Random.Range(0.1f, 0.3f));
		while (true)
		{
			if (GKM.PlayerControlledMask == 0)
			{
				yield return null;
			}
			if (GameController.Instance.IsFirstPerson)
			{
				CheckDistanceFirstPerson();
			}
			else
			{
				CheckDistanceThirdPerson();
			}
			yield return wfs;
		}
	}

	private void CheckDistanceFirstPerson()
	{
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor == null || GameController.Instance.AimimgDownScopeThisFrame)
		{
			if (m_bActivated)
			{
				ClearContextBlips();
			}
			return;
		}
		InterfaceableObject[] array = mContextMenuObjects;
		foreach (InterfaceableObject interfaceableObject in array)
		{
			if (!(interfaceableObject != null) || !interfaceableObject.CanTurnOn())
			{
				continue;
			}
			bool flag = false;
			float num = Vector2.SqrMagnitude(mFirstPersonActor.realCharacter.FirstPersonCamera.transform.position.xz() - interfaceableObject.transform.position.xz());
			if (num < interfaceableObject.FirstPersonVisibleRadiusSqr && IsInteriorRelative(mFirstPersonActor, interfaceableObject))
			{
				if (interfaceableObject.IsInCaptureRange(mFirstPersonActor.realCharacter.FirstPersonCamera.transform.position - interfaceableObject.transform.position, num))
				{
					if (!interfaceableObject.enabled || mForceUpdate)
					{
						m_bActivated = true;
						interfaceableObject.TurnOn();
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag && interfaceableObject.enabled)
			{
				m_bActivated = false;
				interfaceableObject.TurnOff();
			}
		}
		mForceUpdate = false;
	}

	private bool IsInteriorRelative(Actor a, InterfaceableObject io)
	{
		if (io.quickType == SelectableObject.QuickType.EnemySoldier || io.quickType == SelectableObject.QuickType.PlayerSoldier)
		{
			return true;
		}
		BuildingDoor component = io.gameObject.GetComponent<BuildingDoor>();
		if ((bool)component && !component.m_Interface.IsInterior)
		{
			return true;
		}
		if (a != null)
		{
			return io.Interior == a.baseCharacter.Location;
		}
		return false;
	}

	private void CheckDistanceThirdPerson()
	{
		bool flag = false;
		mTriggerActors.Clear();
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != null && a.realCharacter != null && !a.realCharacter.IsMortallyWounded() && !a.realCharacter.IsDead() && Vector3.SqrMagnitude(a.transform.position - base.transform.position) < m_fRadiusSqr)
			{
				flag = true;
				mTriggerActors.Add(a);
			}
		}
		if (flag || Radius == 1024f)
		{
			if (!m_bActivated || mForceUpdate)
			{
				m_bActivated = false;
				int num = 0;
				while (!m_bActivated && num < mTriggerActors.Count)
				{
					for (int i = 0; i < mContextMenuObjects.Length; i++)
					{
						if (mContextMenuObjects[i] != null && mContextMenuObjects[i].CanTurnOn() && IsInteriorRelative(mTriggerActors[num], mContextMenuObjects[i]))
						{
							mContextMenuObjects[i].TurnOn();
							m_bActivated = true;
						}
					}
					num++;
				}
			}
		}
		else if (m_bActivated)
		{
			for (int j = 0; j < mContextMenuObjects.Length; j++)
			{
				if (mContextMenuObjects[j] != null && mContextMenuObjects[j].CanTurnOn())
				{
					mContextMenuObjects[j].TurnOff();
					m_bActivated = false;
				}
			}
		}
		mForceUpdate = false;
	}

	public void SetActive(bool active)
	{
		m_bActivated = active;
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(base.transform.position, Radius);
		}
	}
}
