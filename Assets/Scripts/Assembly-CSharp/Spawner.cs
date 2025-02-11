using UnityEngine;

[RequireComponent(typeof(ActorWrapper))]
public class Spawner : SpawnerBase
{
	public ActorDescriptor Spawn;

	public SpawnerData m_Interface;

	public SecurityCameraOverrideData m_CameraOverrides;

	public SentryGunOverrideData m_SentryGunOverrides;

	public TutorialEnemyOverrideData m_TutorialEnemyOverrides;

	public bool IsTutorialEnemy;

	public Condition Gate;

	public GameObject spawned;

	[HideInInspector]
	public AITetherPoint StaticTether;

	public bool TeleportToIfTransition;

	public static bool ms_bDebugTeleport;

	public BuildingWithInterior m_Interior;

	public bool DontSelectOnSpawn;

	public static bool SpawnedEnemyThisFrame;

	private int mSeed;

	public bool RespottedAfterDeath;

	private ActorDescriptor mAdRef;

	private WeaponDescriptor mWdRef;

	private void Start()
	{
		Initialise(0f);
		if (Gate == null)
		{
			Gate = base.gameObject.AddComponent<Condition_NotSpawned>();
		}
		if (m_Interface.EventsList != null)
		{
			EventsList = m_Interface.EventsList;
		}
		mSeed = Random.Range(0, int.MaxValue);
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		base.transform.forward = forward;
	}

	private void Update()
	{
		if (Gate.Value() && (!(SectionManager.GetSectionManager() != null) || SectionManager.GetSectionManager().SectionActivated) && GameController.Instance.GameplayStarted && GKM.AvailableSpawnSlots() >= 1)
		{
			if (!Spawn.PlayerControlled && !SpawnedEnemyThisFrame)
			{
				SpawnedEnemyThisFrame = true;
				ProcessSpawn();
			}
			else
			{
				ProcessSpawn();
			}
		}
	}

	public void OnDrawGizmos()
	{
		DebugDraw(Spawn);
	}

	public void SwitchRoutine(GameObject routines)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.SwitchRoutine(routines, mMonitored, base.gameObject);
		}
	}

	public void SwitchEvents(GameObject events)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.SwitchEvents(mMonitored[0], events);
		}
	}

	public void SetUnitAggression(string aggressive)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.SetUnitAggression(mMonitored[0], aggressive);
		}
	}

	public void ToggleUnitAggression()
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.ToggleUnitAggression(mMonitored[0]);
		}
	}

	public void ToggleFireAtWill()
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.ToggleFireAtWill(mMonitored, base.gameObject);
		}
	}

	public void ToggleInvincible()
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			mMonitored[mMonitored.Count - 1].health.Invulnerable = !mMonitored[mMonitored.Count - 1].health.Invulnerable;
		}
	}

	public void TogglePlayerControl()
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.TogglePlayerControl(mMonitored, base.gameObject);
		}
	}

	public void SetTargetPriorityModifier(string val)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.SetTargetPriorityModifier(mMonitored, val);
		}
	}

	public void AddHealth(string amount)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.AddHealth(mMonitored, amount);
		}
	}

	public void AddAmmo(string amount)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.AddAmmo(mMonitored, amount);
		}
	}

	public void FillAmmoByPercent(string amount)
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			CharacterPropertyModifier.FillAmmoByPercent(mMonitored, amount);
		}
	}

	public void SetStartingGMGAmmo()
	{
		if (mMonitored != null && mMonitored.Count > 0)
		{
			float num = AmmoDropManager.Instance.StartingAmmoPerc(mMonitored[0].weapon.PrimaryWeapon.GetClass());
			CharacterPropertyModifier.FillAmmoByPercent(mMonitored, num.ToString());
		}
	}

	public void Activate()
	{
		Actor component = mSpawned.GetComponent<Actor>();
		if (!(component != null))
		{
			return;
		}
		if (component.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
		{
			component.gameObject.SetActive(true);
			return;
		}
		TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)component.tasks.GetRunningTask(typeof(TaskSecurityCamera));
		if (taskSecurityCamera != null)
		{
			taskSecurityCamera.EnableCamera();
		}
	}

	public void Deactivate()
	{
		Actor component = mSpawned.GetComponent<Actor>();
		if (!(component != null))
		{
			return;
		}
		if (component.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
		{
			component.gameObject.SetActive(false);
			return;
		}
		TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)component.tasks.GetRunningTask(typeof(TaskSecurityCamera));
		if (taskSecurityCamera != null)
		{
			taskSecurityCamera.DisableCamera();
		}
	}

	private void OnDestroy()
	{
		if (mAdRef != null)
		{
			Object.Destroy(mAdRef);
			mAdRef = null;
		}
		if (mWdRef != null)
		{
			Object.Destroy(mWdRef);
			mWdRef = null;
		}
	}

	public void ProcessSpawn()
	{
		if (GKM.AvailableSpawnSlots() < 1)
		{
			return;
		}
		Transform transform = base.transform;
		ActorDescriptor actorDescriptor = (mAdRef = Object.Instantiate(Spawn) as ActorDescriptor);
		if (Spawn.PlayerControlled && actorDescriptor.soldierIndex >= 0 && actorDescriptor.soldierIndex < 4)
		{
			SoldierSettings soldierSettings = GameSettings.Instance.Soldiers[actorDescriptor.soldierIndex];
			actorDescriptor.Name = soldierSettings.Name;
			actorDescriptor.DefaultPrimaryWeapon = soldierSettings.Weapon.Descriptor;
		}
		else if (m_Interface.NameOverride != string.Empty)
		{
			actorDescriptor.Name = m_Interface.NameOverride;
		}
		if (m_Interface.WeaponOverrides != null && m_Interface.WeaponOverrides.Count > 0)
		{
			actorDescriptor.DefaultPrimaryWeapon = m_Interface.WeaponOverrides[Random.Range(0, m_Interface.WeaponOverrides.Count)];
		}
		if (m_Interface.UnlimitedAmmo)
		{
			string text = actorDescriptor.DefaultPrimaryWeapon.name;
			actorDescriptor.DefaultPrimaryWeapon = (WeaponDescriptor)Object.Instantiate(actorDescriptor.DefaultPrimaryWeapon);
			mWdRef = actorDescriptor.DefaultPrimaryWeapon;
			actorDescriptor.DefaultPrimaryWeapon.UnlimitedAmmo = m_Interface.UnlimitedAmmo;
			actorDescriptor.DefaultPrimaryWeapon.name = text;
		}
		actorDescriptor.ExtraHealthPoints = m_Interface.ExtraHealth;
		actorDescriptor.ForceKeepAlive = m_Interface.ForceKeepAlive;
		actorDescriptor.InvulernableToExplosions = m_Interface.InvulernableToExplosions;
		mSpawned = SceneNanny.CreateActor(actorDescriptor, transform.position, transform.rotation, mSeed);
		spawned = mSpawned;
		if (m_Interior != null)
		{
			m_Interior.AddInternalObject(spawned);
		}
		spawned.AddComponent<Entity>().Type = m_Interface.EntityType;
		Actor component = spawned.GetComponent<Actor>();
		if (component != null)
		{
			component.realCharacter.EnableNavMesh(true);
			component.realCharacter.mPersistentAssaultParams.CopyFrom(m_Interface.AssaultParameters);
			component.realCharacter.RespottedAfterDeath = RespottedAfterDeath;
			if (!component.health.Invulnerable)
			{
				component.health.Invulnerable = m_Interface.SpawnInvincible;
			}
			if (!component.health.CanBeMortallyWounded)
			{
				component.health.CanBeMortallyWounded = m_Interface.CanBeMortallyWounded;
			}
			component.health.OnlyDamagedByPlayer = m_Interface.OnlyDamagedByPlayer;
			SpawnerUtils.InitialiseSpawnedActor(component, StaticTether, m_Interface.SpawnedAlertState, PreferredTarget);
			if (m_Interface.quickDestination != null)
			{
				component.awareness.coverCluster = m_Interface.quickDestination.coverCluster.GetComponent<CoverCluster>();
			}
			RegisterSpawn(component, 1);
			if (m_Interface.SpawnMortallyWounded)
			{
				component.health.DeferMortallyWounded = true;
			}
			if (component.realCharacter != null)
			{
				component.realCharacter.CanTriggerAlarms = m_Interface.CanTriggerAlarms;
				component.realCharacter.DontDropAmmo = m_Interface.DontDropAmmo;
				if (component.behaviour.PlayerControlled && !DontSelectOnSpawn)
				{
					GameplayController.Instance().AddToSelected(component);
				}
			}
		}
		AddEventsList(component);
		if (component.awareness.ChDefCharacterType == CharacterType.SecurityCamera && m_CameraOverrides != null)
		{
			TrackingRobotRealCharacter trackingRobotRealCharacter = component.realCharacter as TrackingRobotRealCharacter;
			if (trackingRobotRealCharacter != null)
			{
				trackingRobotRealCharacter.CameraOverrideData = m_CameraOverrides;
			}
		}
		if (component.awareness.ChDefCharacterType == CharacterType.SentryGun && m_SentryGunOverrides != null)
		{
			TrackingRobotRealCharacter trackingRobotRealCharacter2 = component.realCharacter as TrackingRobotRealCharacter;
			if (trackingRobotRealCharacter2 != null)
			{
				trackingRobotRealCharacter2.SentryGunOverrideData = m_SentryGunOverrides;
			}
		}
		if (m_TutorialEnemyOverrides != null && IsTutorialEnemy)
		{
			component.awareness.FoV = m_TutorialEnemyOverrides.LineOfSightFOV;
			component.awareness.PeripheralFoV = m_TutorialEnemyOverrides.LineOfSightPeripheralFOV;
			component.awareness.VisionRange = m_TutorialEnemyOverrides.LineOfSightDistance;
			if ((bool)component.ears)
			{
				component.ears.Range = m_TutorialEnemyOverrides.AuditoryAwarenessRange;
				component.ears.RangeSqr = component.ears.Range * component.ears.Range;
			}
			component.realCharacter.DamageModifier = m_TutorialEnemyOverrides.DamageMultiplier;
			float num = component.health.Health * m_TutorialEnemyOverrides.HealthModifier;
			component.health.Initialise(0f, num, num);
		}
	}
}
