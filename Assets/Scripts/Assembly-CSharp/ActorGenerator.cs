using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorGenerator
{
	public delegate void InitialiseHitLocation(HitLocation hitLocation);

	private static ActorGenerator m_Instance;

	private int mUId;

	public static ActorGenerator Instance()
	{
		if (m_Instance == null)
		{
			m_Instance = new ActorGenerator();
		}
		return m_Instance;
	}

	public static void NullInstance()
	{
		m_Instance = null;
	}

	public static void ConfigureNavMeshAgent(NavMeshAgent navAgent, float navBaseOffset, bool isEnemy, float angularSpeed)
	{
		navAgent.baseOffset = navBaseOffset;
		navAgent.angularSpeed = angularSpeed;
		navAgent.autoTraverseOffMeshLink = false;
		navAgent.autoRepath = false;
		navAgent.radius = 0.3f;
		if (OverwatchController.Instance.Active)
		{
			navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
		}
		else
		{
			navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		}
		int num = 0;
		num |= 1 << NavMesh.GetNavMeshLayerFromName("Default");
		num |= 1 << NavMesh.GetNavMeshLayerFromName("Jump");
		num |= (isEnemy ? (1 << NavMesh.GetNavMeshLayerFromName("EnemyOnly")) : 0);
		navAgent.walkableMask = num;
	}

	private GameObject Init_Common(GameObject actorBase, ActorDescriptor descriptor, Vector3 position, Quaternion rotation, int seed)
	{
		Actor actor = actorBase.AddComponent<Actor>();
		GlobalKnowledgeManager.Instance().RegisterActor(actor);
		actor.SetPosition(position);
		actorBase.transform.rotation = rotation;
		if (descriptor == null)
		{
			Debug.Log("ActorGenerator doesn't have a descriptor");
		}
		actorBase.name = descriptor.Name + "_uid" + mUId++;
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		if (OverwatchController.Instance.Active)
		{
			theme = "Overwatch";
		}
		GameObject gameObject = SceneNanny.CreateModel(descriptor.Model.GetModelForTheme(theme, seed));
		if (descriptor.UseParentTransform)
		{
			gameObject.transform.parent = actorBase.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		else
		{
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		if (descriptor.Attachments != null)
		{
			ThemedModelDescriptor[] attachments = descriptor.Attachments;
			foreach (ThemedModelDescriptor themedModelDescriptor in attachments)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(themedModelDescriptor.GetModelForTheme(theme)) as GameObject;
				while (gameObject2.transform.childCount > 0)
				{
					Transform child = gameObject2.transform.GetChild(0);
					child.parent = gameObject.transform;
					child.transform.localPosition = Vector3.zero;
					child.transform.localRotation = Quaternion.identity;
				}
				UnityEngine.Object.Destroy(gameObject2);
			}
		}
		actor.tasks = actorBase.AddComponent<TaskManager>();
		actor.Connect(actorBase.AddComponent<AwarenessComponent>());
		actor.awareness.LookDirection = gameObject.transform.forward;
		actor.awareness.standingEyeLevel = descriptor.LineOfSightOffset;
		Animation animation = gameObject.GetComponent<Animation>() ?? gameObject.AddComponent<Animation>();
		AnimDirector animDirector = (actor.animDirector = actorBase.AddComponent<AnimDirector>());
		animDirector.AnimationPlayer = animation;
		animDirector.DefaultAnimLibrary = descriptor.AnimationLibrary;
		animDirector.GameModel = gameObject;
		gameObject.SetActive(false);
		animDirector.Finalise();
		animation.localBounds.SetMinMax(new Vector3(-0.5f, -1f, -0.5f), new Vector3(0.5f, 1f, 0.5f));
		if (descriptor.ChctrType != CharacterType.SecurityCamera && descriptor.ChctrType != CharacterType.SentryGun)
		{
			actorBase.SetActive(false);
			NavMeshAgent navMeshAgent = (actor.navAgent = actorBase.AddComponent<NavMeshAgent>());
			float angularSpeed = 360f;
			if (descriptor.ChctrType == CharacterType.RiotShieldNPC)
			{
				angularSpeed = 180f;
			}
			ConfigureNavMeshAgent(navMeshAgent, descriptor.NavBaseOffset, descriptor.Faction != FactionHelper.Category.Player, angularSpeed);
			navMeshAgent.enabled = false;
			actorBase.SetActive(true);
		}
		AddHalo(actorBase, gameObject, "Halo000");
		return gameObject;
	}

	private void AddHalo(GameObject actorBase, GameObject model, string tag)
	{
		Transform transform = model.transform.FindInHierarchy(tag);
		if (transform != null)
		{
			HaloEffect haloEffect = EffectsController.Instance.AddHalo(actorBase, transform);
			haloEffect.Pattern = HaloEffect.BlinkPattern.BlinkSlow;
			haloEffect.Colour = HaloEffect.HaloColour.Red;
		}
	}

	private void SetUpLineOfSight(Actor actor, ActorDescriptor descriptor)
	{
		actor.awareness.FoV = descriptor.LineOfSightFOV;
		actor.awareness.VisionRange = descriptor.LineOfSightLookDistance;
		actor.awareness.visible = true;
		actor.awareness.canLook = descriptor.LineOfSightEnabled;
		if (descriptor.LineOfSightPeripheralFOV != 0f)
		{
			actor.awareness.PeripheralFoV = descriptor.LineOfSightPeripheralFOV;
		}
		if (descriptor.LineOfSightSideProximityRange != 0f)
		{
			actor.awareness.SideProximityThreshold = descriptor.LineOfSightSideProximityRange;
		}
		if (descriptor.LineOfSightRearProximityRange != 0f)
		{
			actor.awareness.RearProximityThreshold = descriptor.LineOfSightRearProximityRange;
		}
	}

	public static HudBlipIcon AddBlip(GameObject actorBase, ActorDescriptor descriptor, GameObject model)
	{
		HudBlipIcon hudBlipIcon = SceneNanny.Instantiate(descriptor.HudMarker) as HudBlipIcon;
		hudBlipIcon.Target = actorBase.transform;
		GameObject gameObject = ContainerLinks.FindChildByName(model, "Bip002 Head");
		if (gameObject != null)
		{
			hudBlipIcon.OffsetTarget = gameObject.transform;
		}
		else
		{
			hudBlipIcon.OffsetTarget = null;
		}
		hudBlipIcon.Visible = descriptor.BlipVisible;
		return hudBlipIcon;
	}

	private void Init_RealCharacter(GameObject actorBase, ActorDescriptor descriptor, GameObject model)
	{
		Actor component = actorBase.GetComponent<Actor>();
		RealCharacter realCharacter = actorBase.AddComponent<RealCharacter>();
		component.Connect(realCharacter);
		component.awareness.ChDefCharacterType = descriptor.ChctrType;
		realCharacter.CMRules = descriptor.CMRules;
		if (descriptor.PlayerControlled)
		{
			TBFAssert.DoAssert(descriptor.soldierIndex > -1 && descriptor.soldierIndex < 5, "Invalid ID specified for player");
			realCharacter.Id = "Player_" + (descriptor.soldierIndex + 1);
		}
		else
		{
			int num = descriptor.name.IndexOf("(Clone)");
			realCharacter.Id = ((num >= 0) ? descriptor.name.Remove(num) : descriptor.name);
		}
		HudBlipIcon hudBlipIcon = AddBlip(actorBase, descriptor, model);
		HealthComponent healthComponent = actorBase.AddComponent<HealthComponent>();
		healthComponent.Owner = component;
		component.health = healthComponent;
		float num2 = 1f;
		int num3 = 0;
		num3 = ActStructure.Instance.CurrentSection;
		if (num3 >= 0 && MissionSetup.Instance != null)
		{
			if (ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran && MissionSetup.Instance.VeteranDifficultyModifiers.Length > num3)
			{
				if (descriptor.Faction == FactionHelper.Category.Player)
				{
					num2 = GameSettings.Instance.VeteranPlayerHealthModifier * MissionSetup.Instance.VeteranDifficultyModifiers[num3].PlayerHealthMultiplier;
				}
				else if (descriptor.Faction == FactionHelper.Category.Enemy || descriptor.Faction == FactionHelper.Category.SoloEnemy)
				{
					num2 = GameSettings.Instance.VeteranEnemyHealthModifier * MissionSetup.Instance.VeteranDifficultyModifiers[num3].EnemyHealthMultiplier;
				}
			}
			else if (MissionSetup.Instance.RegularDifficultyModifiers.Length > num3)
			{
				if (descriptor.Faction == FactionHelper.Category.Player)
				{
					num2 = MissionSetup.Instance.RegularDifficultyModifiers[num3].PlayerHealthMultiplier;
				}
				else if (descriptor.Faction == FactionHelper.Category.Enemy || descriptor.Faction == FactionHelper.Category.SoloEnemy)
				{
					num2 = MissionSetup.Instance.RegularDifficultyModifiers[num3].EnemyHealthMultiplier;
				}
			}
		}
		num2 = ((num2 != 0f) ? num2 : 1f);
		float num4 = HealthComponent.DEFAULT_HEALTH * descriptor.HealthMultiplier * num2 + descriptor.ExtraHealthPoints;
		if (!descriptor.PlayerControlled)
		{
			num4 = GMGBalanceTweaks.Instance.GMGModifier_EnemyHealth(num4);
		}
		healthComponent.Initialise(0f, num4, num4);
		if (descriptor.Invulnerable)
		{
			healthComponent.Invulnerable = true;
		}
		healthComponent.Rechargeable = descriptor.Rechargeable;
		if (descriptor.ForceKeepAlive)
		{
			healthComponent.HealthMinClamped = num4 * ColourChart.HEALTH_MIN_PULSE_01;
		}
		else if (descriptor.PlayerControlled)
		{
			healthComponent.HealthMinClamped = healthComponent.MortallyWoundedThreshold - 1f;
		}
		if (descriptor.InvulernableToExplosions)
		{
			healthComponent.InvulnerableToExplosions = true;
		}
		if (descriptor.ChctrType == CharacterType.Human && descriptor.PlayerControlled)
		{
			healthComponent.CanBeMortallyWounded = true;
		}
		SetUpLineOfSight(component, descriptor);
		bool flag = (bool)ActStructure.Instance && ActStructure.Instance.CurrentMissionIsSpecOps() && descriptor.ChctrType == CharacterType.RiotShieldNPC;
		if (descriptor.AuditoryAwarenessEnabled || flag)
		{
			AuditoryAwarenessComponent auditoryAwarenessComponent = actorBase.AddComponent<AuditoryAwarenessComponent>().Connect(component) as AuditoryAwarenessComponent;
			auditoryAwarenessComponent.Range = descriptor.AuditoryAwarenessRange;
		}
		RulesSystemInterface bac = actorBase.AddComponent<RulesSystemInterface>();
		component.Connect(bac);
		BehaviourController behaviourController = actorBase.AddComponent<BehaviourController>();
		component.Connect(behaviourController);
		behaviourController.PlayerControlled = descriptor.PlayerControlled;
		behaviourController.SelectedMarkerObj = hudBlipIcon as SoldierMarker;
		behaviourController.BroadcastPerceptionRange = descriptor.BroadcastPerceptionRange;
		behaviourController.IdealMaxBurstFireTimeMin = descriptor.IdealMaxBurstFireTimeMin;
		behaviourController.IdealMaxBurstFireTimeMax = descriptor.IdealMaxBurstFireTimeMax;
		behaviourController.TimeToPopUp = descriptor.TimeToPopUp;
		AIGunHandler bac2 = actorBase.AddComponent<AIGunHandler>();
		component.Connect(bac2);
		if (CharacterTypeHelper.RequiresFireAtWillComponent(descriptor.ChctrType))
		{
			FireAtWillComponent bac3 = actorBase.AddComponent<FireAtWillComponent>();
			component.Connect(bac3);
		}
		if (CharacterTypeHelper.RequiresGrenadeThrowerComponent(descriptor.ChctrType))
		{
			GrenadeThrowerComponent bac4 = actorBase.AddComponent<GrenadeThrowerComponent>();
			component.Connect(bac4);
		}
		if (CharacterTypeHelper.RequiresSentryHackingComponent(descriptor.ChctrType) && descriptor.PlayerControlled)
		{
			SentryHackingComponent bac5 = actorBase.AddComponent<SentryHackingComponent>();
			component.Connect(bac5);
		}
		if (!OverwatchController.Instance.Active)
		{
			SpeechComponent bac6 = actorBase.AddComponent<SpeechComponent>();
			component.Connect(bac6);
		}
		CharacterController characterController = actorBase.AddComponent<CharacterController>();
		characterController.center = new Vector3(0f, 1f, 0f);
		characterController.radius = 0.5f;
		characterController.height = 2f;
		TBFAssert.DoAssert(4 == ActorSelectUtils.NormalColliderSettings.Length, "NormalEnemyColliderSettings MUST have as many entries as there are factions.");
		ActorSelectUtils.CreateActorSelectCollider(component, ActorSelectUtils.NormalColliderSettings[(int)descriptor.Faction]);
		realCharacter.HudMarker = hudBlipIcon;
		component.awareness.faction = descriptor.Faction;
		component.awareness.ChDefCharacterType = descriptor.ChctrType;
		if (component.awareness.ChDefCharacterType == CharacterType.RPG)
		{
			EnemyBlip enemyBlip = realCharacter.HudMarker as EnemyBlip;
			enemyBlip.UseHeavyWeightEnemyIcon(true);
		}
		realCharacter.Range = descriptor.Range;
		realCharacter.FiringRange = descriptor.FiringRange;
		realCharacter.SimpleHitBounds = SceneNanny.NewGameObject("SimpleHitBounds").AddComponent<HitLocation>();
		realCharacter.SimpleHitBounds.gameObject.layer = LayerMask.NameToLayer("SimpleHitBox");
		realCharacter.SimpleHitBounds.Owner = component.gameObject;
		realCharacter.SimpleHitBounds.Actor = component;
		realCharacter.SimpleHitBounds.Location = "Simple";
		realCharacter.SimpleHitBounds.Bone = null;
		realCharacter.SimpleHitBounds.DamageMultiplier = 1f;
		if (OptimisationManager.opt_ParentSimpleCollider)
		{
			realCharacter.SimpleHitBounds.transform.parent = component.transform;
			realCharacter.SimpleHitBounds.transform.localPosition = Vector3.zero;
		}
		Rigidbody rigidbody = realCharacter.SimpleHitBounds.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		rigidbody.mass = 1f;
		CapsuleCollider capsuleCollider = realCharacter.SimpleHitBounds.gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.center = 0.9f * Vector3.up;
		capsuleCollider.radius = 0.25f;
		capsuleCollider.height = 1.8f;
		capsuleCollider.direction = 1;
		capsuleCollider.enabled = true;
		DefaultSurfaceMarkup.CreateComponent(realCharacter.SimpleHitBounds.gameObject, SurfaceMaterial.None, null);
		if (behaviourController.PlayerControlled)
		{
			FirstThirdPersonWidget firstThirdPersonWidget = actorBase.AddComponent<FirstThirdPersonWidget>();
			component.firstThirdPersonWidget = firstThirdPersonWidget;
			FPFootstepPlayer footstepPlayer = actorBase.AddComponent<FPFootstepPlayer>();
			component.FootstepPlayer = footstepPlayer;
		}
	}

	private void Init_TrackingRobotRealCharacter(GameObject actorBase, ActorDescriptor descriptor, GameObject model)
	{
		Actor component = actorBase.GetComponent<Actor>();
		TrackingRobotRealCharacter trackingRobotRealCharacter = actorBase.AddComponent<TrackingRobotRealCharacter>();
		component.Connect(trackingRobotRealCharacter);
		component.awareness.ChDefCharacterType = descriptor.ChctrType;
		trackingRobotRealCharacter.CMRules = descriptor.CMRules;
		HudBlipIcon hudBlipIcon = SceneNanny.Instantiate(descriptor.HudMarker) as HudBlipIcon;
		hudBlipIcon.Target = actorBase.transform;
		hudBlipIcon.Visible = descriptor.BlipVisible;
		HealthComponent healthComponent = (component.health = actorBase.AddComponent<HealthComponent>());
		float num = HealthComponent.DEFAULT_HEALTH * descriptor.HealthMultiplier;
		if (trackingRobotRealCharacter.SentryGunOverrideData != null)
		{
			num = trackingRobotRealCharacter.SentryGunOverrideData.Health * descriptor.HealthMultiplier;
		}
		healthComponent.Initialise(0f, num, num);
		if (descriptor.Invulnerable)
		{
			healthComponent.Invulnerable = true;
		}
		healthComponent.Rechargeable = descriptor.Rechargeable;
		SetUpLineOfSight(component, descriptor);
		if (descriptor.AuditoryAwarenessEnabled)
		{
			AuditoryAwarenessComponent auditoryAwarenessComponent = actorBase.AddComponent<AuditoryAwarenessComponent>().Connect(component) as AuditoryAwarenessComponent;
			auditoryAwarenessComponent.Range = descriptor.AuditoryAwarenessRange;
		}
		RulesSystemInterface bac = actorBase.AddComponent<RulesSystemInterface>();
		component.Connect(bac);
		BehaviourController behaviourController = actorBase.AddComponent<BehaviourController>();
		component.Connect(behaviourController);
		behaviourController.PlayerControlled = descriptor.PlayerControlled;
		behaviourController.SelectedMarkerObj = hudBlipIcon as SoldierMarker;
		behaviourController.BroadcastPerceptionRange = descriptor.BroadcastPerceptionRange;
		behaviourController.IdealMaxBurstFireTimeMin = descriptor.IdealMaxBurstFireTimeMin;
		behaviourController.IdealMaxBurstFireTimeMax = descriptor.IdealMaxBurstFireTimeMax;
		AIGunHandler bac2 = actorBase.AddComponent<AIGunHandler>();
		component.Connect(bac2);
		if (descriptor.ChctrType != CharacterType.SecurityCamera && descriptor.ChctrType != CharacterType.SentryGun)
		{
			NavMeshAgent navAgent = component.navAgent;
			CharacterController characterController = actorBase.AddComponent<CharacterController>();
			characterController.center = new Vector3(0f, navAgent.height / 2f, 0f);
			characterController.radius = 0.6f;
			characterController.height = navAgent.height;
		}
		trackingRobotRealCharacter.HudMarker = hudBlipIcon;
		component.awareness.faction = descriptor.Faction;
		component.awareness.ChDefCharacterType = descriptor.ChctrType;
		trackingRobotRealCharacter.Range = descriptor.Range;
		trackingRobotRealCharacter.FiringRange = descriptor.FiringRange;
		foreach (ScriptableObject additionalDetail in descriptor.AdditionalDetails)
		{
			if (additionalDetail.GetType() == typeof(SentryGunDescriptor_Additional))
			{
				SentryGunDescriptor_Additional sentryGunDescriptor_Additional = additionalDetail as SentryGunDescriptor_Additional;
				if (sentryGunDescriptor_Additional.Shield != null)
				{
					trackingRobotRealCharacter.Shield = sentryGunDescriptor_Additional.Shield;
				}
				HackableObjectSentryGun hackableObjectSentryGun = actorBase.AddComponent<HackableObjectSentryGun>();
				if (hackableObjectSentryGun != null)
				{
					hackableObjectSentryGun.SentryGunActor = component;
					hackableObjectSentryGun.SetPieceEnter = sentryGunDescriptor_Additional.SetPieceEnter;
					hackableObjectSentryGun.SetPieceExit = sentryGunDescriptor_Additional.SetPieceExit;
					hackableObjectSentryGun.HackLoopAnimation = sentryGunDescriptor_Additional.HackLoopAnimation;
					hackableObjectSentryGun.DuckIntoAnimation = sentryGunDescriptor_Additional.DuckIntoAnimation;
					hackableObjectSentryGun.DuckIdleAnimation = sentryGunDescriptor_Additional.DuckIdleAnimation;
					hackableObjectSentryGun.DuckOutOfAnimation = sentryGunDescriptor_Additional.DuckExitAnimation;
					hackableObjectSentryGun.ProgressBlipRef = sentryGunDescriptor_Additional.ProgressBlipRef;
				}
			}
		}
	}

	private void Init_FakeCharacter(GameObject actorBase, ActorDescriptor descriptor, GameObject model)
	{
		Actor component = actorBase.GetComponent<Actor>();
		component.Connect(actorBase.AddComponent<FakeCharacter>());
		component.model = model;
	}

	private void Init_CharacterCommon(GameObject actorBase, ActorDescriptor descriptor, GameObject model)
	{
		Actor component = actorBase.GetComponent<Actor>();
		BaseCharacter component2 = actorBase.GetComponent<BaseCharacter>();
		string text = descriptor.Name;
		if (text.StartsWith("S_", StringComparison.InvariantCultureIgnoreCase))
		{
			text = Language.Get(text);
		}
		component2.UnitName = text;
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		if (descriptor.Nationality != null)
		{
			component2.VocalAccent = descriptor.Nationality.GetVocalForTheme(theme);
		}
		else
		{
			component2.VocalAccent = BaseCharacter.Nationality.Friendly;
		}
		component2.Settings.StealthSpeed = descriptor.StealthSpeed;
		component2.Settings.RunSpeed = descriptor.RunSpeed;
		component2.Settings.WalkSpeed = descriptor.WalkSpeed;
		component2.Settings.SaunterSpeed = descriptor.SaunterSpeed;
		component.model = model;
		component.SoldierIndex = descriptor.soldierIndex;
		if (descriptor.ModelOverWatchMaterial != null)
		{
			component2.ModelOverWatchMaterial = descriptor.ModelOverWatchMaterial;
		}
		Component[] componentsInChildren = component.model.gameObject.GetComponentsInChildren(typeof(Renderer), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = (Renderer)array[i];
			renderer.gameObject.layer = LayerMask.NameToLayer("GlobalViewable");
		}
		component2.moveAimDescription = descriptor.MoveAimDescription;
		component2.mNavigationSetPiece = SceneNanny.Instantiate(descriptor.NavigationSetPiece) as NavigationSetPieceLogic;
		Transform transform = model.transform.FindInHierarchy("Bone027");
		if (transform != null && descriptor.HasLaserSight)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(EffectsController.Instance.Effects.LaserSight) as GameObject;
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.forward = -transform.transform.right;
			component2.LaserSight = gameObject;
		}
		PlayerWeapon playerWeapon = actorBase.AddComponent<PlayerWeapon>();
		component.Connect(playerWeapon);
		IWeapon weapon = null;
		IWeapon weapon2 = null;
		if (descriptor.ChctrType == CharacterType.SentryGun)
		{
			weapon = new Weapon_Minigun(new SentryGunWeaponModel(component), descriptor.SentryGunWeapon);
			weapon2 = new Weapon_Null();
		}
		else
		{
			float adsModifier = 1f;
			float ammoModifier = 1f;
			float reloadModifier = 1f;
			if (component.behaviour.PlayerControlled)
			{
				adsModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.QuickDraw);
				ammoModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Bandolier);
				reloadModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.SleightOfHand);
			}
			object obj;
			if (descriptor.DefaultPrimaryWeapon != null)
			{
				IWeapon weapon3 = descriptor.DefaultPrimaryWeapon.Create(component.model, adsModifier, ammoModifier, reloadModifier);
				obj = weapon3;
			}
			else
			{
				obj = new Weapon_Null();
			}
			weapon = (IWeapon)obj;
			object obj2;
			if (descriptor.DefaultSecondaryWeapon != null)
			{
				IWeapon weapon3 = descriptor.DefaultSecondaryWeapon.Create(component.model, adsModifier, ammoModifier, 1f);
				obj2 = weapon3;
			}
			else
			{
				obj2 = new Weapon_Null();
			}
			weapon2 = (IWeapon)obj2;
		}
		playerWeapon.Initialise(weapon, weapon2);
		CharacterLighting characterLighting = null;
		if (!OverwatchController.Instance.Active)
		{
			characterLighting = SetupCharacterLighting(component);
		}
		AddLockOnDetectors(component2, descriptor);
		if (descriptor.ChctrType != CharacterType.SecurityCamera && descriptor.HitBoxRig != null)
		{
			bool constantHitBox = descriptor.ChctrType == CharacterType.SentryGun;
			AddRagdoll(component, descriptor.HitBoxRig, constantHitBox);
			Transform transform2 = component.model.transform.Find("Bip002/Bip002 Pelvis");
			if (transform2 != null)
			{
				Rigidbody rigidbody = transform2.gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
				SphereCollider sphereCollider = transform2.gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = 1f;
				sphereCollider.center = Vector3.zero;
				transform2.gameObject.layer = LayerMask.NameToLayer("PelvicRegion");
				transform2.gameObject.AddComponent<ActorLink>().linkedActor = component;
			}
			HitLocationTranslator hitLocationTranslator = null;
			switch (component.awareness.ChDefCharacterType)
			{
			case CharacterType.Human:
				hitLocationTranslator = component2.SimpleHitBounds.gameObject.AddComponent<Human_HLT>();
				break;
			case CharacterType.RiotShieldNPC:
				hitLocationTranslator = component2.SimpleHitBounds.gameObject.AddComponent<RiotShield_HLT>();
				break;
			}
			if (hitLocationTranslator != null)
			{
				Collider component3 = component2.SimpleHitBounds.GetComponent<Collider>();
				hitLocationTranslator.Initialise(component2.SimpleHitBounds, component2.Ragdoll.Bones, component.model.transform.Find("Bip002/Bip002 Pelvis/Bip002 Spine"), component.model.transform, component3);
			}
		}
		if (descriptor.PlayerControlled)
		{
			if (PlayerSquadManager.Instance.ArmourProtection != 0f)
			{
				HitLocation[] bones = component.realCharacter.Ragdoll.Bones;
				HitLocation[] array2 = bones;
				foreach (HitLocation hitLocation in array2)
				{
					hitLocation.DamageMultiplier *= PlayerSquadManager.Instance.ArmourProtection;
				}
				component2.SimpleHitBounds.DamageMultiplier *= PlayerSquadManager.Instance.ArmourProtection;
			}
			component2.CreateStandardCamera(actorBase.transform, actorBase, model);
		}
		if (!OverwatchController.Instance.Active)
		{
			BlobShadow blobShadow = EffectsController.Instance.AddBlobShadow(model, actorBase, true);
			blobShadow.name = descriptor.Name + "_BlobShadow";
			if (characterLighting != null)
			{
				blobShadow.ShadowColour = characterLighting.ShadowColour;
			}
			component2.Shadow = blobShadow;
		}
	}

	public GameObject Generate(ActorDescriptor descriptor, Vector3 position, Quaternion rotation)
	{
		int seed = UnityEngine.Random.Range(0, int.MaxValue);
		return Generate(descriptor, position, rotation, seed);
	}

	public GameObject Generate(ActorDescriptor descriptor, Vector3 position, Quaternion rotation, int seed)
	{
		GameObject gameObject = SceneNanny.NewGameObject();
		GameObject model = Init_Common(gameObject, descriptor, position, rotation, seed);
		CharacterType chctrType = descriptor.ChctrType;
		if (chctrType == CharacterType.SentryGun || chctrType == CharacterType.SecurityCamera)
		{
			Init_TrackingRobotRealCharacter(gameObject, descriptor, model);
		}
		else
		{
			Init_RealCharacter(gameObject, descriptor, model);
		}
		Init_CharacterCommon(gameObject, descriptor, model);
		Actor component = gameObject.GetComponent<Actor>();
		PoseModuleSharedData poseModuleSharedData = gameObject.AddComponent<PoseModuleSharedData>().Connect(component) as PoseModuleSharedData;
		poseModuleSharedData.Initialise(model, descriptor.MoveAimDescription);
		GameObject gameObject2 = gameObject;
		Transform transform = gameObject.transform.FindChild("Picker");
		if (transform != null)
		{
			gameObject2 = transform.gameObject;
			gameObject.layer = LayerMask.NameToLayer("ActorGameObject");
			gameObject.tag = "Untagged";
			transform.name += gameObject.name;
		}
		if (descriptor.Faction == FactionHelper.Category.Player)
		{
			SelectableObject selectableObject = gameObject2.AddComponent<CMPlayerSoldier>();
			selectableObject.AssociatedObject = gameObject;
			ContextMenuDistanceManager contextMenuDistanceManager = gameObject2.AddComponent<ContextMenuDistanceManager>();
			if (contextMenuDistanceManager != null)
			{
				contextMenuDistanceManager.ChangeRadius((!component.behaviour.PlayerControlled) ? 1.8f : 1024f);
			}
			if (descriptor.PlayerControlled)
			{
				selectableObject.quickType = SelectableObject.QuickType.PlayerSoldier;
				if ((bool)CommonHudController.Instance)
				{
					CommonHudController.Instance.TPPUnitSelecter.AddUnit(component, descriptor.soldierIndex);
				}
			}
			else
			{
				selectableObject.quickType = SelectableObject.QuickType.Unspecified;
			}
		}
		else if (descriptor.ChctrType == CharacterType.SentryGun)
		{
			ContextMenuDistanceManager contextMenuDistanceManager2 = gameObject2.AddComponent<ContextMenuDistanceManager>();
			if (contextMenuDistanceManager2 != null)
			{
				contextMenuDistanceManager2.ChangeRadius(20f);
			}
			CMHackableObjectSentryGun cMHackableObjectSentryGun = gameObject2.AddComponent<CMHackableObjectSentryGun>();
			if (cMHackableObjectSentryGun != null)
			{
				cMHackableObjectSentryGun.BlipWorldOffset.y = 1f;
				cMHackableObjectSentryGun.SentryGunActor = component;
			}
		}
		else if (descriptor.ChctrType != CharacterType.SecurityCamera)
		{
			SelectableObject selectableObject2 = gameObject2.AddComponent<CMEnemySoldier>();
			selectableObject2.AssociatedObject = gameObject;
		}
		return gameObject;
	}

	public GameObject GenerateFake(ActorDescriptor descriptor, Vector3 position, Quaternion rotation)
	{
		int seed = UnityEngine.Random.Range(0, int.MaxValue);
		GameObject gameObject = SceneNanny.NewGameObject();
		GameObject model = Init_Common(gameObject, descriptor, position, rotation, seed);
		Init_FakeCharacter(gameObject, descriptor, model);
		Init_CharacterCommon(gameObject, descriptor, model);
		return gameObject;
	}

	public GameObject GenerateCorpse(ActorDescriptor descriptor, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = Generate(descriptor, position, rotation);
		Actor component = gameObject.GetComponent<Actor>();
		component.baseCharacter.Kill(null);
		component.awareness.canLook = false;
		component.ears.enabled = false;
		new TaskDead(component.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType, null);
		return gameObject;
	}

	public static CharacterLighting SetupCharacterLighting(Actor actor)
	{
		GameObject gameObject = actor.gameObject;
		GameObject model = actor.model;
		CharacterLighting characterLighting = gameObject.GetComponent<CharacterLighting>() ?? gameObject.AddComponent<CharacterLighting>();
		characterLighting.ProbeAnchor = characterLighting.ProbeAnchor ?? model.transform.Find("Bip002/Bip002 Pelvis");
		if (characterLighting.ProbeAnchor == null)
		{
			characterLighting.ProbeAnchor = new GameObject("ProbeAnchor").transform;
			characterLighting.ProbeAnchor.parent = model.transform;
			characterLighting.ProbeAnchor.localPosition = Vector3.up;
			characterLighting.ProbeAnchor.localRotation = Quaternion.identity;
		}
		List<Renderer> list = new List<Renderer>();
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.material.shader.name.Contains("Probe"))
			{
				list.Add(renderer);
			}
		}
		characterLighting.Renderers = list.ToArray();
		characterLighting.UpdateMaterials(true);
		return characterLighting;
	}

	public static Vector3 GetHeadCentreFromHeadBone(Transform headLocator)
	{
		return (!(headLocator != null)) ? Vector3.zero : (headLocator.position - 0.1f * headLocator.right);
	}

	public static SnapTarget CreateEmptySnapTarget(Transform transform)
	{
		GameObject gameObject = new GameObject("SnapTarget");
		GameObject gameObject2 = new GameObject("LockOnDetector");
		gameObject.transform.ParentAndZeroLocalPositionAndRotation(transform);
		SnapTarget snapTarget = gameObject.AddComponent<SnapTarget>();
		snapTarget.LockOnDetector = gameObject2;
		gameObject2.SetActive(false);
		gameObject2.transform.ParentAndZeroLocalPositionAndRotation(gameObject.transform);
		gameObject2.layer = LayerMask.NameToLayer("LockOnDetectors");
		Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		return snapTarget;
	}

	public static SnapTarget CreateStandardSnapTarget(Transform transform)
	{
		SnapTarget snapTarget = CreateEmptySnapTarget(transform);
		SphereCollider sphereCollider = snapTarget.LockOnDetector.AddComponent<SphereCollider>();
		sphereCollider.radius = 3f;
		sphereCollider.center = new Vector3(0f, 1.5f, 0f);
		return snapTarget;
	}

	private static void AddLockOnDetectors(BaseCharacter character, ActorDescriptor descriptor)
	{
		if (FactionHelper.AreEnemies(FactionHelper.Category.Player, descriptor.Faction) && (descriptor.ChctrType == CharacterType.Human || descriptor.ChctrType == CharacterType.RiotShieldNPC || descriptor.ChctrType == CharacterType.RPG))
		{
			SnapTarget snapTarget = CreateStandardSnapTarget(character.transform);
			snapTarget.SnapPositionOverride = () => character.GetSnapPosition();
			character.SnapTarget = snapTarget;
		}
	}

	public static void AddRagdoll(Actor actor, HitBoxDescriptor hitBoxDescriptor, bool constantHitBox)
	{
		InitialiseHitLocation initialiseHitLocation = delegate(HitLocation hitLocation)
		{
			hitLocation.Owner = actor.gameObject;
			hitLocation.Actor = actor;
			hitLocation.Health = actor.health;
			if (constantHitBox)
			{
				hitLocation.gameObject.layer = LayerMask.NameToLayer("ConstantHitBox");
			}
		};
		actor.baseCharacter.Ragdoll = CreateRagdoll(actor.name, hitBoxDescriptor, actor.model, initialiseHitLocation);
		if (actor.awareness.ChDefCharacterType == CharacterType.SentryGun)
		{
			actor.baseCharacter.Ragdoll.ParentBonesPerm();
		}
	}

	public static Ragdoll CreateRagdoll(string name, HitBoxDescriptor hitBoxDescriptor, GameObject model, InitialiseHitLocation initialiseHitLocation)
	{
		if (hitBoxDescriptor == null)
		{
			return null;
		}
		Ragdoll ragdoll = SceneNanny.NewGameObject("Ragdoll_" + name).AddComponent<Ragdoll>();
		ragdoll.gameObject.layer = LayerMask.NameToLayer("ProjectileCharacterCollider");
		ragdoll.HitBoxDesc = hitBoxDescriptor;
		List<HitLocation> list = new List<HitLocation>();
		foreach (HitBoxDescriptor.HitBox hitBox in hitBoxDescriptor.HitBoxes)
		{
			if (!model.name.Contains("overwatch") || !(hitBox.Location != "Head") || !(hitBox.Location != "Torso") || !(hitBox.Location != "Pelvis"))
			{
				HitLocation hitLocation = HitBoxUtils.CreateHitLocation(model, hitBox);
				initialiseHitLocation(hitLocation);
				hitLocation.transform.parent = ragdoll.transform;
				list.Add(hitLocation);
			}
		}
		ragdoll.Bones = list.ToArray();
		ragdoll.AddRagdollParts();
		return ragdoll;
	}

	public static SoftJointLimit CreateSoftJointStructure(HitBoxDescriptor.SoftJointLimit descriptor)
	{
		SoftJointLimit result = default(SoftJointLimit);
		result.bounciness = descriptor.Bounciness;
		result.damper = descriptor.Damper;
		result.limit = descriptor.Limit;
		result.spring = descriptor.Spring;
		return result;
	}

	private static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecursively(item.gameObject, layer);
		}
	}
}
