using System.Collections.Generic;
using UnityEngine;

public class AwarenessZone : MonoBehaviour
{
	public AwarenessZonelet[] children;

	public uint ident;

	public ActorMask membership = new ActorMask(0u, "Awareness Zone Member Mask");

	private int[] overlapCount = new int[32];

	private int jiggleTime;

	private Vector3 properPosition;

	private Vector3 m_cachedPos = Vector3.zero;

	private Vector3 m_cachedExtent = Vector3.zero;

	private Quaternion m_cachedInverseRot = Quaternion.identity;

	private static List<AwarenessZone> mAwarenessZonePool = new List<AwarenessZone>();

	private void Awake()
	{
		mAwarenessZonePool.Add(this);
		if (children != null && children.Length > 0)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	private void Start()
	{
		membership.Set(0u);
		jiggleTime = 10;
		properPosition = base.transform.position;
		SetTransformPosition(base.transform.position);
	}

	private void SetTransformPosition(Vector3 newPos)
	{
		if (newPos != m_cachedPos)
		{
			m_cachedPos = newPos;
			m_cachedExtent = base.transform.lossyScale * 0.5f;
			m_cachedInverseRot = Quaternion.Inverse(base.transform.rotation);
			base.transform.position = m_cachedPos;
		}
	}

	private void OnDestroy()
	{
		mAwarenessZonePool.Remove(this);
		membership.Release();
	}

	private void Update()
	{
		if ((GKM.AlertedMask & GKM.EnemiesMask(0) & (uint)membership) != 0)
		{
			DisableZone();
		}
		else if (jiggleTime > 0)
		{
			jiggleTime--;
			if ((jiggleTime & 1) == 1)
			{
				SetTransformPosition(properPosition + Vector3.up * 0.05f);
			}
			else
			{
				SetTransformPosition(properPosition);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.enabled)
		{
			Actor component = other.gameObject.GetComponent<Actor>();
			if (!(component == null))
			{
				EnterZonelet(component);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null))
		{
			ExitZonelet(component);
		}
	}

	public void EnterZonelet(Actor actor)
	{
		if (base.enabled)
		{
			overlapCount[actor.quickIndex]++;
			if (((uint)membership & actor.ident) == 0)
			{
				membership.Include(actor.ident);
				actor.awareness.EnterAwarenessZone(ident);
			}
		}
	}

	public void ExitZonelet(Actor actor)
	{
		overlapCount[actor.quickIndex]--;
		if (overlapCount[actor.quickIndex] == 0)
		{
			membership.Exclude(actor.ident);
			actor.awareness.LeaveAwarenessZone(ident);
		}
	}

	public void Deactivate()
	{
		DisableZone();
	}

	public void DisableZone()
	{
		base.gameObject.SetActive(false);
		base.enabled = false;
		membership.Set(0u);
		for (int i = 0; i < overlapCount.Length; i++)
		{
			overlapCount[i] = 0;
		}
		if (children != null)
		{
			AwarenessZonelet[] array = children;
			foreach (AwarenessZonelet awarenessZonelet in array)
			{
				awarenessZonelet.gameObject.SetActive(false);
			}
		}
		if (GlobalKnowledgeManager.Instance() != null)
		{
			GlobalKnowledgeManager.Instance().EjectMembersFromZone(ident);
		}
	}

	public static uint GetAwarenessMembership(Vector3 worldPos)
	{
		int count = mAwarenessZonePool.Count;
		uint num = 0u;
		for (int i = 0; i < count; i++)
		{
			if (mAwarenessZonePool[i].gameObject.activeSelf)
			{
				num |= mAwarenessZonePool[i].Contains(worldPos);
			}
		}
		if (num == 0)
		{
			num = 2147483648u;
		}
		return num;
	}

	public static void AddToAwarenessZonesManually(uint res, Actor actor)
	{
		int count = mAwarenessZonePool.Count;
		for (int i = 0; i < count; i++)
		{
			if ((res & mAwarenessZonePool[i].ident) == mAwarenessZonePool[i].ident && mAwarenessZonePool[i].gameObject.activeSelf)
			{
				mAwarenessZonePool[i].EnterZonelet(actor);
			}
		}
	}

	public uint Contains(Vector3 pos)
	{
		if (children != null)
		{
			int num = children.Length;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (children[i].Contains(pos))
					{
						return ident;
					}
				}
				return 0u;
			}
		}
		Vector3 cachedExtent = m_cachedExtent;
		Vector3 vector = m_cachedInverseRot * (pos - m_cachedPos);
		if (Mathf.Abs(vector.x) > cachedExtent.x)
		{
			return 0u;
		}
		if (Mathf.Abs(vector.z) > cachedExtent.z)
		{
			return 0u;
		}
		if (Mathf.Abs(vector.y) > cachedExtent.y)
		{
			return 0u;
		}
		return ident;
	}

	public static bool IsUnregisteredGameObjectAwarenessInSync(GameObject first, GameObject second)
	{
		return IsUnregisteredGameObjectAwarenessInSync(first.transform.position, second.transform.position);
	}

	public static bool IsUnregisteredGameObjectAwarenessInSync(Vector3 first, Vector3 second)
	{
		uint awarenessMembership = GetAwarenessMembership(first);
		uint awarenessMembership2 = GetAwarenessMembership(second);
		return awarenessMembership == awarenessMembership2;
	}
}
