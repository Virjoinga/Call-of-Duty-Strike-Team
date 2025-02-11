using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStateListenerData
{
	public List<ActorWrapper> Monitored;

	public GameObjectBroadcaster BroadcastOnCompletion;

	public bool BroadcastImmediately;

	public List<GameObject> Turnonables;

	public List<GameObject> Turnoffables;
}
