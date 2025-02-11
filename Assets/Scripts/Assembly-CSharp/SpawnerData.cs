using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerData
{
	public string EntityType = string.Empty;

	public string NameOverride = string.Empty;

	public bool SpawnInvincible;

	public bool UnlimitedAmmo;

	public bool OnlyDamagedByPlayer;

	public bool CanBeMortallyWounded;

	public bool SpawnMortallyWounded;

	public BehaviourController.AlertState SpawnedAlertState;

	public GameObject TetherPoint;

	public GameObject EventsList;

	public GameObject PreferredTarget;

	public QuickDestination quickDestination;

	public bool TeleportToSpawner;

	public List<WeaponDescriptor> WeaponOverrides;

	public float ExtraHealth;

	public bool ForceKeepAlive;

	public bool InvulernableToExplosions;

	public bool CanTriggerAlarms = true;

	public AssaultParams AssaultParameters = new AssaultParams();

	public bool DontDropAmmo;

	public void CopyContainerData(Spawner s)
	{
		if (TetherPoint != null)
		{
			s.StaticTether = TetherPoint.GetComponentInChildren<AITetherPoint>();
		}
		if (PreferredTarget != null)
		{
			s.PreferredTarget = PreferredTarget.GetComponentInChildren<ActorWrapper>();
			if (s.PreferredTarget == null)
			{
				Debug.Log("null");
			}
			else
			{
				Debug.Log("!null");
			}
		}
		s.TeleportToIfTransition = TeleportToSpawner;
	}

	public void ResolveGuidLinks()
	{
		if (quickDestination != null)
		{
			quickDestination.ResolveGuidLinks();
		}
	}
}
