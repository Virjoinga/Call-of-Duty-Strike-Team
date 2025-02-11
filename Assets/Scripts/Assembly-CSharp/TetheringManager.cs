using System.Collections.Generic;
using UnityEngine;

public class TetheringManager : MonoBehaviour
{
	public static TetheringManager instance = null;

	public static float TETHER_LIMIT = 6f;

	public static float TETHER_LIMIT_SQ = TETHER_LIMIT * TETHER_LIMIT;

	private bool mActive;

	public static TetheringManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		ResetState();
	}

	private void Update()
	{
		if (!mActive && GKM.PlayerControlledMask != 0)
		{
			CreatePlayerSquadTether();
			mActive = true;
		}
	}

	public void ResetState()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			a.tether.Active = false;
		}
		mActive = false;
	}

	public void RefreshPlayerSquadTethers(Vector3 updatedPosition, List<Actor> newTetherMembers)
	{
		for (int i = 0; i < newTetherMembers.Count; i++)
		{
			newTetherMembers[i].tether.SetPositionAndActivate(updatedPosition);
		}
		if (updatedPosition == Vector3.zero)
		{
			TBFAssert.DoAssert(false, "Player Squad Tether refreshed with no position");
		}
	}

	private void CreatePlayerSquadTether()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			a.tether.TetherToSelf();
		}
	}
}
