using System;
using UnityEngine;

[Serializable]
public class NavGateData
{
	public enum CharacterType
	{
		All = 0,
		Enemy = 1,
		Player = 2,
		None = 3,
		FriendlyNPC = 4,
		PlayerAndFriendly = 5
	}

	public CharacterType WalkableBy = CharacterType.Enemy;

	[HideInInspector]
	public bool m_Walkable;

	[HideInInspector]
	public string m_NavLayer = "Unassigned";

	[HideInInspector]
	public int m_NavLayerID = -1;

	[HideInInspector]
	public int NavLayerID
	{
		get
		{
			return m_NavLayerID;
		}
	}
}
