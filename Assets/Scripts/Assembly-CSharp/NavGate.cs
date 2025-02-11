using UnityEngine;

public class NavGate : MonoBehaviour
{
	public static int kMaxGates = 19;

	public NavGateData m_Interface;

	private void Start()
	{
		m_Interface.m_NavLayer = NavMeshUtils.FindClosestPortal(base.transform.position, 1f);
		if (m_Interface.m_NavLayer == "Unassigned")
		{
			Debug.Log("NavGate has not been assigned on object " + base.gameObject.name);
			return;
		}
		m_Interface.m_NavLayerID = 1 << NavMesh.GetNavMeshLayerFromName(m_Interface.m_NavLayer);
		if (NavGateManager.Instance != null)
		{
			NavGateManager.Instance.AddNavGate(this);
		}
		SetNavGate(m_Interface.WalkableBy);
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void OnDestroy()
	{
		if (NavGateManager.Instance != null)
		{
			NavGateManager.Instance.RemoveNavGate(this);
		}
	}

	public void Activate()
	{
		OnEnter();
	}

	public void Deactivate()
	{
		OnLeave();
	}

	public void OnEnter()
	{
		SetNavGate(NavGateData.CharacterType.All);
	}

	public void OnLeave()
	{
		SetNavGate(NavGateData.CharacterType.None);
	}

	public void SetNavGate(NavGateData.CharacterType walkType)
	{
		m_Interface.WalkableBy = walkType;
		if (!Application.isPlaying)
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!(a.navAgent != null))
			{
				continue;
			}
			switch (walkType)
			{
			case NavGateData.CharacterType.All:
				a.navAgent.walkableMask |= m_Interface.m_NavLayerID;
				break;
			case NavGateData.CharacterType.Enemy:
				if (a.awareness.faction == FactionHelper.Category.Enemy || a.awareness.faction == FactionHelper.Category.SoloEnemy)
				{
					a.navAgent.walkableMask |= m_Interface.m_NavLayerID;
				}
				else
				{
					a.navAgent.walkableMask &= ~m_Interface.m_NavLayerID;
				}
				break;
			case NavGateData.CharacterType.Player:
				if (a.behaviour.PlayerControlled)
				{
					a.navAgent.walkableMask |= m_Interface.m_NavLayerID;
				}
				else
				{
					a.navAgent.walkableMask &= ~m_Interface.m_NavLayerID;
				}
				break;
			case NavGateData.CharacterType.FriendlyNPC:
				if (!a.behaviour.PlayerControlled && a.awareness.faction == FactionHelper.Category.Player)
				{
					a.navAgent.walkableMask |= m_Interface.m_NavLayerID;
				}
				else
				{
					a.navAgent.walkableMask &= ~m_Interface.m_NavLayerID;
				}
				break;
			case NavGateData.CharacterType.PlayerAndFriendly:
				if (a.awareness.faction == FactionHelper.Category.Player)
				{
					a.navAgent.walkableMask |= m_Interface.m_NavLayerID;
				}
				else
				{
					a.navAgent.walkableMask &= ~m_Interface.m_NavLayerID;
				}
				break;
			default:
				a.navAgent.walkableMask &= ~m_Interface.m_NavLayerID;
				break;
			}
		}
	}

	public void OpenNavGate()
	{
		SetNavGate(NavGateData.CharacterType.All);
	}

	public void CloseNavGate()
	{
		SetNavGate(NavGateData.CharacterType.None);
	}
}
