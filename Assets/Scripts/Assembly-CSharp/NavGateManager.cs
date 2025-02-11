using System.Collections.Generic;
using UnityEngine;

public class NavGateManager : MonoBehaviour
{
	private static NavGateManager instance;

	private List<NavGate> m_NavGates = new List<NavGate>();

	public static NavGateManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public int AddNavGate(NavGate navGate)
	{
		int count = m_NavGates.Count;
		m_NavGates.Add(navGate);
		return count;
	}

	public void RemoveNavGate(NavGate navGate)
	{
		m_NavGates.Remove(navGate);
	}

	public void SetGatesOnNavAgent(NavMeshAgent agent, bool isnpc, FactionHelper.Category faction)
	{
		foreach (NavGate navGate in m_NavGates)
		{
			switch (navGate.m_Interface.WalkableBy)
			{
			case NavGateData.CharacterType.All:
				agent.walkableMask |= navGate.m_Interface.NavLayerID;
				break;
			case NavGateData.CharacterType.Enemy:
				if (faction == FactionHelper.Category.Enemy || faction == FactionHelper.Category.SoloEnemy)
				{
					agent.walkableMask |= navGate.m_Interface.NavLayerID;
				}
				else
				{
					agent.walkableMask &= ~navGate.m_Interface.NavLayerID;
				}
				break;
			case NavGateData.CharacterType.Player:
				if (!isnpc)
				{
					agent.walkableMask |= navGate.m_Interface.NavLayerID;
				}
				else
				{
					agent.walkableMask &= ~navGate.m_Interface.NavLayerID;
				}
				break;
			case NavGateData.CharacterType.FriendlyNPC:
				if (isnpc && faction == FactionHelper.Category.Player)
				{
					agent.walkableMask |= navGate.m_Interface.NavLayerID;
				}
				else
				{
					agent.walkableMask &= ~navGate.m_Interface.NavLayerID;
				}
				break;
			case NavGateData.CharacterType.PlayerAndFriendly:
				if (faction == FactionHelper.Category.Player)
				{
					agent.walkableMask |= navGate.m_Interface.NavLayerID;
				}
				else
				{
					agent.walkableMask &= ~navGate.m_Interface.NavLayerID;
				}
				break;
			default:
				agent.walkableMask &= ~navGate.m_Interface.NavLayerID;
				break;
			}
		}
	}

	private void ResetAllNavGates()
	{
		foreach (NavGate navGate in m_NavGates)
		{
			navGate.SetNavGate(navGate.m_Interface.WalkableBy);
		}
	}

	public NavGate GetGateFromLayerMask(int mask, Vector3 pos)
	{
		if (mask != 0)
		{
			for (int i = 0; i < m_NavGates.Count; i++)
			{
				if ((m_NavGates[i].m_Interface.NavLayerID & mask) != 0)
				{
					return m_NavGates[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < m_NavGates.Count; j++)
			{
				if (m_NavGates[j] != null && Vector3.SqrMagnitude(m_NavGates[j].transform.position - pos) < 2f)
				{
					return m_NavGates[j];
				}
			}
		}
		return null;
	}

	private void Update()
	{
	}
}
