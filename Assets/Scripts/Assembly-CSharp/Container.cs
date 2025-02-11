using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
	public enum ContainerType
	{
		Default = 0,
		Skin = 1,
		Prefab = 2,
		Duplicate = 3,
		Dummy = 4,
		SetPiece = 5,
		NumTypes = 6
	}

	public string ReferenceId = string.Empty;

	public bool KeepTransform;

	public bool m_BramallsSpecialFlag;

	public bool m_MovableMode;

	public bool m_ViewableMode;

	private bool mHasGatheredSelectiveData;

	public float m_LightmapScale = 1f;

	public bool m_DisableNavMesh;

	public bool m_HideCMCompletely;

	public float m_TppCmVisibleRadius = -1f;

	public float m_FppCmVisibleRadius = -1f;

	public float m_FppCmTriggerRadius = -1f;

	public float m_FppCmFacingAngle;

	public float m_FppCmCaptureAngle = 360f;

	public CoverCluster m_MultiManCoverCluster;

	public bool m_Occluder = true;

	public bool m_Occludee = true;

	public bool m_HasImported;

	public ContainerLinks m_Links;

	[NonSerialized]
	public int duplicateTestIndex = -1;

	public string m_Guid = Guid.NewGuid().ToString();

	public ContainerType m_Type;

	public List<TransformOverride> m_TransOverrides = new List<TransformOverride>();

	public List<TransformOverride> m_TransSnapshot = new List<TransformOverride>();

	public bool IsEditable
	{
		get
		{
			Transform containerContents = GetContainerContents();
			return !(containerContents != null) || (containerContents.gameObject.hideFlags & HideFlags.NotEditable) == 0;
		}
		set
		{
			Transform containerContents = GetContainerContents();
			if (!(containerContents != null))
			{
				return;
			}
			if (!value)
			{
				containerContents.gameObject.hideFlags |= HideFlags.NotEditable;
				Transform[] componentsInChildren = containerContents.GetComponentsInChildren<Transform>(true);
				foreach (Transform transform in componentsInChildren)
				{
					transform.gameObject.hideFlags |= HideFlags.NotEditable;
				}
			}
			else
			{
				containerContents.hideFlags &= ~HideFlags.NotEditable;
				Transform[] componentsInChildren2 = containerContents.GetComponentsInChildren<Transform>(true);
				foreach (Transform transform2 in componentsInChildren2)
				{
					transform2.gameObject.hideFlags &= ~HideFlags.NotEditable;
				}
			}
		}
	}

	public bool IsViewable
	{
		get
		{
			Transform containerContents = GetContainerContents();
			return !(containerContents != null) || (containerContents.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0;
		}
		set
		{
			Transform containerContents = GetContainerContents();
			if (containerContents != null)
			{
				if (value)
				{
					m_ViewableMode = true;
					containerContents.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
				}
				else
				{
					m_ViewableMode = false;
					containerContents.gameObject.hideFlags |= HideFlags.HideInHierarchy;
				}
			}
		}
	}

	public static void SendMessage(GameObject target, string message)
	{
		SendMessageInternal(target, message, null, null);
	}

	public static void SendMessage(GameObject target, string message, GameObject sender)
	{
		SendMessageInternal(target, message, null, sender);
	}

	public static void SendMessageWithParam(GameObject target, string message, GameObject param)
	{
		SendMessageInternalObj(target, message, param, null);
	}

	public static void SendMessageWithParam(GameObject target, string message, string param)
	{
		SendMessageInternal(target, message, param, null);
	}

	public static void SendMessageWithParam(GameObject target, string message, GameObject param, GameObject sender)
	{
		SendMessageInternalObj(target, message, param, sender);
	}

	public static void SendMessageWithParam(GameObject target, string message, string param, GameObject sender)
	{
		SendMessageInternal(target, message, param, sender);
	}

	private static void SendMessageInternalObj(GameObject target, string message, GameObject param, GameObject sender)
	{
		if (message == string.Empty)
		{
			message = "Activate";
		}
		if (!(target != null))
		{
			return;
		}
		GameObject gameObject = SpecifiedTargettedObject(target, message);
		if (gameObject != null)
		{
			target = gameObject;
			message = message.Split(':')[0];
		}
		ContainerOverride component = target.GetComponent<ContainerOverride>();
		if (component != null)
		{
			if (param != null)
			{
				component.SendOverrideMessageWithParam(target, message, param);
			}
			else
			{
				component.SendOverrideMessage(target, message);
			}
		}
		else
		{
			target.SendMessage(message, param);
		}
	}

	private static void SendMessageInternal(GameObject target, string message, string param, GameObject sender)
	{
		if (message == string.Empty)
		{
			message = "Activate";
		}
		if (!(target != null))
		{
			return;
		}
		GameObject gameObject = SpecifiedTargettedObject(target, message);
		if (gameObject != null)
		{
			target = gameObject;
			message = message.Split(':')[0];
		}
		ContainerOverride component = target.GetComponent<ContainerOverride>();
		if (component != null)
		{
			if (param != null && param != string.Empty)
			{
				component.SendOverrideMessageWithParam(target, message, param);
			}
			else if (message.Equals("Enable"))
			{
				target.SetActive(true);
			}
			else if (message.Equals("Disable"))
			{
				target.SetActive(false);
			}
			else if (message.Equals("DisableRenderer"))
			{
				Renderer componentInChildren = target.GetComponentInChildren<Renderer>();
				if ((bool)componentInChildren)
				{
					componentInChildren.enabled = false;
				}
			}
			else if (message.Equals("EnableRenderer"))
			{
				Renderer componentInChildren2 = target.GetComponentInChildren<Renderer>();
				if ((bool)componentInChildren2)
				{
					componentInChildren2.enabled = true;
				}
			}
			else
			{
				component.SendOverrideMessage(target, message);
			}
		}
		else if (message.Equals("Enable"))
		{
			target.SetActive(true);
		}
		else if (message.Equals("Disable"))
		{
			target.SetActive(false);
		}
		else if (message.Equals("DisableRenderer"))
		{
			Renderer componentInChildren3 = target.GetComponentInChildren<Renderer>();
			if ((bool)componentInChildren3)
			{
				componentInChildren3.enabled = false;
			}
		}
		else if (message.Equals("EnableRenderer"))
		{
			Renderer componentInChildren4 = target.GetComponentInChildren<Renderer>();
			if ((bool)componentInChildren4)
			{
				componentInChildren4.enabled = true;
			}
		}
		else
		{
			target.SendMessage(message, param, SendMessageOptions.DontRequireReceiver);
		}
	}

	private static GameObject SpecifiedTargettedObject(GameObject target, string message)
	{
		string[] array = message.Split(':');
		if (array.Length != 1)
		{
			GameObject gameObject = target.transform.Find(array[1]).gameObject;
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static UnityEngine.Object GetContainerFromObject(UnityEngine.Object o)
	{
		UnityEngine.Object result = o;
		Component component = o as Component;
		GameObject gameObject = o as GameObject;
		GameObject gameObject2 = null;
		Transform transform = ((component != null) ? component.transform : ((!(gameObject != null)) ? null : gameObject.transform));
		while (transform != null)
		{
			if (transform.GetComponent<BagObject>() != null)
			{
				gameObject2 = transform.GetComponent<BagObject>().gameObject;
			}
			Container component2 = transform.GetComponent<Container>();
			if (gameObject2 != null && component2 != null && !component2.IsViewable && gameObject2 == component2.GetBagGameObject())
			{
				result = transform.gameObject;
			}
			LevelLayer component3 = transform.GetComponent<LevelLayer>();
			if (component3 != null && !component3.IsEditable)
			{
				result = transform.gameObject;
			}
			transform = transform.parent;
		}
		return result;
	}

	public static GameObject GetBagObjectFromObject(GameObject go)
	{
		GameObject gameObject = GetContainerFromObject(go) as GameObject;
		if (gameObject != null)
		{
			Container component = gameObject.GetComponent<Container>();
			if (component != null)
			{
				return component.GetBagGameObject();
			}
		}
		return null;
	}

	public static string GetGUIDFromGameObject(GameObject gameObj)
	{
		if (gameObj == null)
		{
			return string.Empty;
		}
		Container component = gameObj.GetComponent<Container>();
		if (component != null)
		{
			return component.m_Guid;
		}
		BagObject component2 = gameObj.GetComponent<BagObject>();
		if (component2 != null)
		{
			return component2.m_MissionGuid;
		}
		BagElement component3 = gameObj.GetComponent<BagElement>();
		if (component3 != null)
		{
			return component3.m_Guid;
		}
		GuidRefHook component4 = gameObj.GetComponent<GuidRefHook>();
		if (component4 != null)
		{
			return component4.m_Guid;
		}
		Transform parent = gameObj.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}
		LevelLayer component5 = parent.GetComponent<LevelLayer>();
		if (component5 != null && !component5.IsEditable)
		{
			Debug.LogError("Cannot create a GUIRef - object has no GUID and is in a locked layer!");
			return string.Empty;
		}
		return gameObj.AddComponent<GuidRefHook>().m_Guid;
	}

	public static Container GetContainerFromGuid(string guid)
	{
		if (guid != string.Empty)
		{
			UnityEngine.Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(Container));
			for (int i = 0; i < array.Length; i++)
			{
				Container container = (Container)array[i];
				if (container.m_Guid == guid)
				{
					return container;
				}
			}
		}
		return null;
	}

	public static GameObject GetGameObjectFromGuid(string guid)
	{
		if (guid != string.Empty)
		{
			UnityEngine.Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(Container));
			for (int i = 0; i < array.Length; i++)
			{
				Container container = (Container)array[i];
				if (container.m_Guid == guid)
				{
					return container.gameObject;
				}
			}
			UnityEngine.Object[] array2 = IncludeDisabled.FindSceneObjectsOfType(typeof(BagElement));
			for (int j = 0; j < array2.Length; j++)
			{
				BagElement bagElement = (BagElement)array2[j];
				if (bagElement.m_Guid == guid)
				{
					return bagElement.gameObject;
				}
			}
			UnityEngine.Object[] array3 = IncludeDisabled.FindSceneObjectsOfType(typeof(GuidRefHook));
			for (int k = 0; k < array3.Length; k++)
			{
				GuidRefHook guidRefHook = (GuidRefHook)array3[k];
				if (guidRefHook.m_Guid == guid)
				{
					return guidRefHook.gameObject;
				}
			}
		}
		return null;
	}

	public ExposedLink GetExposedLink(string exposedName)
	{
		BagObject bagObject = GetComponentInChildren(typeof(BagObject)) as BagObject;
		if (bagObject != null)
		{
			return bagObject.GetExposedLink(exposedName);
		}
		return null;
	}

	public object GetExposedObject(string exposedName)
	{
		return ContainerLinks.GetLink(base.gameObject, exposedName);
	}

	public LevelLayer GetLayer()
	{
		Transform parent = base.transform.parent;
		int num = 0;
		while (parent != null)
		{
			LevelLayer component = parent.GetComponent<LevelLayer>();
			if (component != null)
			{
				return component;
			}
			if (num > 64)
			{
				Debug.Log("Couldn't find layer for container: " + base.name);
				return null;
			}
			parent = parent.transform.parent;
			num++;
		}
		return null;
	}

	public void GenerateNewGuid()
	{
		m_Guid = Guid.NewGuid().ToString();
	}

	private bool AssignOverride(BagObject.OverrideType srcType, BagObject.OverrideType dstType, Type type)
	{
		if (srcType == dstType)
		{
			Component component = base.gameObject.GetComponent(type);
			if (component == null)
			{
				ContainerOverride containerOverride = base.gameObject.AddComponent(type) as ContainerOverride;
				if (containerOverride != null)
				{
					containerOverride.CreateAssociatedDefaultObjects(this);
				}
			}
			else if (component as CutsceneOverride != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
				int num = 0;
				Component[] components = base.gameObject.GetComponents(typeof(SetPieceOverride));
				for (int i = 0; i < components.Length; i++)
				{
					SetPieceOverride obj = (SetPieceOverride)components[i];
					if (num != 0)
					{
						UnityEngine.Object.DestroyImmediate(obj);
					}
					num++;
				}
				if (!base.gameObject.GetComponent(typeof(SetPieceOverride)))
				{
					base.gameObject.AddComponent(typeof(SetPieceOverride));
				}
			}
			return true;
		}
		return false;
	}

	public void AutoAssignOverride(BagObject bagObj)
	{
		if (bagObj != null)
		{
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Interior, typeof(InteriorOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SetPiece, typeof(SetPieceOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.HostageSequence, typeof(HostageSequenceOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BreachActor, typeof(BreachActorOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Enemy, typeof(EnemyOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TutorialEnemy, typeof(TutorialEnemyOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemyFromDoor, typeof(EnemyFromDoorOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemySecurityCamera, typeof(EnemySecurityCameraOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemyRiotShield, typeof(EnemyRiotShieldOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SpawnerCoordinator, typeof(SpawnerCoordinatorOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SpawnController, typeof(SpawnControllerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.NPC, typeof(NPCOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Player, typeof(PlayerOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.Player)
			{
				base.gameObject.GetComponent<PlayerOverride>().m_SpawnerOverrideData.SpawnedAlertState = BehaviourController.AlertState.Alerted;
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.RPGEnemy, typeof(RPGOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemySentryTurret, typeof(EnemySentryTurretOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemyFromRoof, typeof(EnemyFromRoofOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.EnemySniper)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EnemySniper, typeof(EnemySniperOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.FakeAnimatingActor, typeof(FakeAnimatingActorOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.FakeGunman)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.FakeGunman, typeof(FakeGunmanOverride));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.AlarmManger)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AlarmManger, typeof(AlarmManager));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AlarmTripped, typeof(AlarmTrippedOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Combo, typeof(ComboOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DestroyObject, typeof(DestroyObjectOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.EvadeCameras, typeof(EvadeCamerasOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.FindObject, typeof(FindObjectOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.HackTerminalObjective, typeof(HackTerminalObjectiveOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.KeepAlive, typeof(KeepAliveOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Kill, typeof(KillOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.KillWave, typeof(KillWaveOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Pickup, typeof(PickupOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SquadCasualities, typeof(SquadCasualitiesOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SquadMortallyWounded, typeof(SquadMortallyWoundedOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.UnitsInVolume, typeof(UnitsInVolumeOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.UnitsNotInVolume, typeof(UnitsNotInVolumeOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Scripted, typeof(ScriptedOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.StayAliveOnMiniGun, typeof(StayAliveOnMiniGunOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.RespotOnFail, typeof(RespotOnFailOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.BreachableDoor)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BreachableDoor, typeof(BreachableDoorOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Collectable, typeof(CollectableOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Discoverable, typeof(DiscoverableOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.InteriorCrate, typeof(InteriorCrateOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.InteriorHandler, typeof(InteriorHandlerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Terminal, typeof(TerminalOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TerminalLight, typeof(TerminalLightOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MultiManSwitch, typeof(MultiManSwitchOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Timer, typeof(TimedOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MeshSwitch, typeof(MeshSwitchOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.FixedGun, typeof(FixedGunOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AntiTankMine, typeof(AntiTankMineOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ExplodableObject, typeof(ExplodableObjectOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Dumpster, typeof(DumpsterOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ZipLine, typeof(ZipLineOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TargetPainter, typeof(TargetPainterOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.PlacedC4, typeof(PlacedC4Override));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.HackableObject, typeof(HackableOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AmmoCache, typeof(AmmoCacheOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AmmoCacheController, typeof(AmmoCacheControllerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MysteryCache, typeof(MysteryCacheOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MysteryCacheController, typeof(MysteryCacheControllerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AwarenessZone, typeof(AwarenessZoneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DropZone, typeof(DropZoneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_3m, typeof(MantleZone_3mOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_High, typeof(MantleZone_HighOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_Low, typeof(MantleZone_LowOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_WallVault, typeof(MantleZone_WallVaultOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_WindowVault, typeof(MantleZone_WindowVaultOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_Low1M, typeof(MantleZone_Low1MOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_TwoManVault, typeof(MantleZone_TwoManVaultOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MantleZone_3MUp, typeof(MantleZone_3mUpOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.POI, typeof(POIOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TetherPoint, typeof(TetherPointOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CoverCluster, typeof(CoverCluster));
			if (bagObj.m_OverrideType == BagObject.OverrideType.CoverPointManager)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CoverPointManager, typeof(NewCoverPointManager));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.GeneralArea, typeof(GeneralArea));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MovableMagnet, typeof(MovableMagnet));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Summoner, typeof(SummonerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SuggestHUDMode, typeof(SuggestedHUDModeOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MoreInfoTrigger, typeof(MoreInfoTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AudioZone, typeof(AudioZoneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BattleChatterZone, typeof(BattleChatterZoneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.OverrideLogic, typeof(OverrideLogicOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SwitchRoutine, typeof(SwitchRoutineOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.CustomRoutine)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CustomRoutine, typeof(RoutineCreator));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CustomRoutine, typeof(RoutineDescriptor));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CustomRoutine, typeof(TaskDescriptorOverride));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.CharacterEventListeners)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CharacterEventListeners, typeof(EventsCreator));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SwitchEvents, typeof(SwitchEventsOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.InflictDamage, typeof(InflictDamageOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.Conditional)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Conditional, typeof(ConditionCreator));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Conditional, typeof(ConditionalDescriptor));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Conditional, typeof(TaskDescriptorOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CharacterPropertyModifier, typeof(CharacterPropertyOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.WorldActorsCommand, typeof(WorldActorsCommandOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.LinkedAIController, typeof(LinkedAIController));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.LinkedAIController, typeof(ContainerOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.SniperFromDoor)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SniperFromDoor, typeof(SniperFromDoorOverride));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.RPGFromDoor)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.RPGFromDoor, typeof(RPGFromDoorOverride));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.RiotShieldFromDoor)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.RiotShieldFromDoor, typeof(RiotShieldFromDoorOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ForceOnDeathTrigger, typeof(ForceOnDeathTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MissionBriefing, typeof(MissionBriefingOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptedCutscene, typeof(ScriptedCutsceneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.CutScene, typeof(ScriptedCutsceneOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BriefingShowandText, typeof(BriefingShowandTextOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BriefingUpdateText, typeof(BriefingUpdateText));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BriefingShowFocus, typeof(BriefingShowFocus));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.BriefingShowImage, typeof(BriefingShowImage));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.GenericTrigger, typeof(GenericTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TriggerBox, typeof(TriggerBoxOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MusicTrigger, typeof(MusicTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.GateOverride, typeof(GateOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.DoorOverride)
			{
				EnterMoveContentsMode();
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DoorOverride, typeof(DoorOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.GateSlide, typeof(GateSlideOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.MetalGateSwing, typeof(MetalGateSwingOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SiloGate, typeof(SiloGateOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.AlarmPanelScenario)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AlarmPanelScenario, typeof(AlarmPanelScenarioOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.AlarmOverrideLogic, typeof(AlarmOverrideLogicOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.ScriptedSequence)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptedSequence, typeof(SequenceCreator));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptedSequence, typeof(ScriptedSequenceOverride));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptedSequence, typeof(ScriptedSequence));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.ConditionalSequence)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ConditionalSequence, typeof(ConditionalSequenceCreator));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ConditionalSequence, typeof(ConditionalSequence));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ConditionalSequence, typeof(ConditionalSequenceOverride));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ConditionalSequenceChooser, typeof(ConditionalSequenceChooser));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Overwatch, typeof(OverwatchOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Helicopter, typeof(HelicopterOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.TargetRangeEnemy, typeof(TargetRangeEnemyOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DialogueManager, typeof(DialogueManagerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DialogueTrigger, typeof(DialogueTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SectionTrigger, typeof(SectionTriggerOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.NavGate, typeof(NavGateOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DebugWarp, typeof(DebugWarpOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DebugActorWarp, typeof(DebugActorWarpOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ForwardMessageToGroup, typeof(SendMessageToGroupOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SendMessageOnRecievedCount, typeof(SendMessageOnRecievedCountOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.SendMessageDelayed, typeof(SendMessageDelayedOverride));
			if (bagObj.m_OverrideType == BagObject.OverrideType.ScriptDataModifier)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptDataModifier, typeof(DataSetCreator));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptDataModifier, typeof(ScriptedSequenceOverride));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptDataModifier, typeof(ScriptedSequence));
			}
			if (bagObj.m_OverrideType == BagObject.OverrideType.ScriptVariable)
			{
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptVariable, typeof(ScriptVariableOverride));
				AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.ScriptVariable, typeof(ScriptVariable));
			}
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.Destructible, typeof(DestructibleOverride));
			AssignOverride(bagObj.m_OverrideType, BagObject.OverrideType.DominationCapturePoint, typeof(DominationCapturePointOverride));
		}
	}

	private void GatherSelectiveBagData()
	{
	}

	public void ApplyCommonOverrides()
	{
		ApplyCommonOverrides(true);
	}

	public void ApplyCommonOverrides(bool applyLightMaps)
	{
		if (!mHasGatheredSelectiveData)
		{
			GatherSelectiveBagData();
			mHasGatheredSelectiveData = true;
		}
	}

	public bool HasInteractionPoint()
	{
		return IncludeDisabled.GetComponentInChildren<InterfaceableObject>(base.gameObject) != null;
	}

	public void SetupOverride()
	{
		CreateRequiredOverride();
		ContainerOverride containerOverride = base.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		if (containerOverride != null)
		{
			containerOverride.SetupOverride(this);
		}
	}

	public void ApplyOverride()
	{
		CreateRequiredOverride();
		ContainerOverride containerOverride = base.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		if (containerOverride != null)
		{
			containerOverride.ApplyOverride(this);
		}
	}

	private void CreateRequiredOverride()
	{
		ContainerOverride containerOverride = GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		if (containerOverride == null)
		{
			GameObject bagGameObject = GetBagGameObject();
			if (bagGameObject != null && (bool)bagGameObject.GetComponent(typeof(InteriorBagProcess)))
			{
				base.gameObject.AddComponent(typeof(InteriorOverride));
			}
		}
	}

	public GameObject GetBagGameObject()
	{
		if (ReferenceId == null || ReferenceId == string.Empty)
		{
			return null;
		}
		int num = ReferenceId.LastIndexOf("/") + 1;
		string arg = ReferenceId.Substring(num, ReferenceId.Length - num);
		if (base.transform.childCount > 0)
		{
			BagObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<BagObject>(true);
			BagObject[] array = componentsInChildren;
			foreach (BagObject bagObject in array)
			{
				if (bagObject != null)
				{
					string text = string.Format("{0}(Clone)", arg);
					if (text == bagObject.name)
					{
						return bagObject.gameObject;
					}
				}
			}
		}
		return null;
	}

	public GameObject GetInnerObject()
	{
		GameObject bagGameObject = GetBagGameObject();
		if (bagGameObject != null && bagGameObject.transform.childCount > 0)
		{
			return bagGameObject.transform.GetChild(0).gameObject;
		}
		return null;
	}

	public void RevertToBagObject()
	{
	}

	public GameObject FindObjectNamesEndsWith(string endsWith)
	{
		GameObject bagGameObject = GetBagGameObject();
		if (bagGameObject != null)
		{
			int childCount = bagGameObject.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = bagGameObject.transform.GetChild(i).gameObject;
				if (BagObject.CheckEndTag(gameObject.name, endsWith))
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	public GameObject FindObjectNamesEndsWith(GameObject obj, string endsWith)
	{
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = obj.transform.GetChild(i).gameObject;
			if (BagObject.CheckEndTag(gameObject.name, endsWith))
			{
				return gameObject;
			}
		}
		return null;
	}

	public Component FindComponentOfType(Type type)
	{
		GameObject bagGameObject = GetBagGameObject();
		if (bagGameObject != null)
		{
			Component[] componentsInChildren = bagGameObject.GetComponentsInChildren(type, true);
			if (componentsInChildren.Length != 0)
			{
				return componentsInChildren[0];
			}
			return null;
		}
		return null;
	}

	public Component[] FindComponentsOfType(Type type)
	{
		GameObject bagGameObject = GetBagGameObject();
		if (bagGameObject != null)
		{
			Component[] componentsInChildren = bagGameObject.GetComponentsInChildren(type, true);
			if (componentsInChildren.Length != 0)
			{
				return componentsInChildren;
			}
			return null;
		}
		return null;
	}

	public Transform GetContainerContents()
	{
		GameObject bagGameObject = GetBagGameObject();
		if (bagGameObject != null)
		{
			return bagGameObject.transform;
		}
		return null;
	}

	public void EnterMoveContentsMode()
	{
		SnapshotCurrentTranslations();
		ApplyTranslationOverrides();
		Transform transform = GetBagGameObject().transform;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform2 in componentsInChildren)
		{
			if (GetGUIDFromGameObject(transform2.gameObject) != string.Empty || transform2 == transform)
			{
				BagElement component = transform2.gameObject.GetComponent<BagElement>();
				if (component != null)
				{
					transform2.gameObject.hideFlags &= ~HideFlags.NotEditable;
					if (component.m_CanBeMoved)
					{
						transform2.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
						Component[] componentsInChildren2 = transform2.gameObject.GetComponentsInChildren<Component>(true);
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							componentsInChildren2[j].hideFlags |= HideFlags.NotEditable;
						}
						Transform component2 = transform2.gameObject.GetComponent<Transform>();
						if (component2 != null)
						{
							component2.hideFlags &= ~HideFlags.NotEditable;
						}
						continue;
					}
					bool flag = false;
					Transform[] componentsInChildren3 = transform2.gameObject.GetComponentsInChildren<Transform>(true);
					foreach (Transform transform3 in componentsInChildren3)
					{
						if (!(GetGUIDFromGameObject(transform3.gameObject) != string.Empty))
						{
							continue;
						}
						BagElement component3 = transform3.gameObject.GetComponent<BagElement>();
						if (component3 != null && component3.m_CanBeMoved)
						{
							transform2.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
							transform2.gameObject.hideFlags |= HideFlags.NotEditable;
							Component[] componentsInChildren4 = transform2.gameObject.GetComponentsInChildren<Component>(true);
							for (int l = 0; l < componentsInChildren4.Length; l++)
							{
								componentsInChildren4[l].hideFlags |= HideFlags.NotEditable;
							}
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						transform2.gameObject.hideFlags |= HideFlags.HideInHierarchy;
					}
				}
				else
				{
					transform2.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
				}
			}
			else if (transform2.gameObject.GetComponent<Container>() == null)
			{
				transform2.gameObject.hideFlags |= HideFlags.HideInHierarchy;
			}
		}
		SnapshotCurrentTranslations();
		m_MovableMode = true;
	}

	public void ExitMoveContentsMode()
	{
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			if (GetGUIDFromGameObject(transform.gameObject) != string.Empty)
			{
				transform.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.NotEditable);
			}
		}
		CaptureTranslationOverrides();
		IsEditable = false;
		IsViewable = false;
		m_MovableMode = false;
	}

	public void ApplyTranslationOverrides()
	{
		List<TransformOverride> list = new List<TransformOverride>();
		foreach (TransformOverride transOverride in m_TransOverrides)
		{
			if (transOverride != null)
			{
				GameObject gameObject = FindInnerObjectFromGuid(transOverride.m_ObjectGuid);
				if (gameObject != null)
				{
					Transform transform = gameObject.transform;
					transform.localPosition = transOverride.m_Position;
					transform.localRotation = transOverride.m_Rotation;
					transform.localScale = transOverride.m_Scale;
				}
			}
			else
			{
				list.Add(transOverride);
			}
		}
		foreach (TransformOverride item in list)
		{
			Debug.Log("Removing internal bag element transform override: '" + item.m_ObjectName + "' for Guid - " + item.m_ObjectGuid + ". From container " + base.name);
			m_TransOverrides.Remove(item);
		}
	}

	public void SnapshotCurrentTranslations()
	{
		m_TransSnapshot.Clear();
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			if (GetGUIDFromGameObject(transform.gameObject) != string.Empty)
			{
				TransformOverride transformOverride = new TransformOverride();
				transformOverride.m_Position = transform.localPosition;
				transformOverride.m_Rotation = transform.localRotation;
				transformOverride.m_Scale = transform.localScale;
				transformOverride.m_ObjectName = transform.name;
				m_TransSnapshot.Add(transformOverride);
			}
		}
	}

	public void CaptureTranslationOverrides()
	{
		if (m_TransSnapshot.Count == 0)
		{
			Debug.Log("Couldn't capture the translations on container " + base.name);
			return;
		}
		int num = 0;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			if (!(GetGUIDFromGameObject(transform.gameObject) != string.Empty) || num >= m_TransSnapshot.Count)
			{
				continue;
			}
			TransformOverride translationOverrideIfDifferent = GetTranslationOverrideIfDifferent(transform.gameObject, m_TransSnapshot[num]);
			if (translationOverrideIfDifferent != null)
			{
				bool flag = false;
				foreach (TransformOverride transOverride in m_TransOverrides)
				{
					if (transOverride.m_ObjectGuid == translationOverrideIfDifferent.m_ObjectGuid)
					{
						transOverride.m_Position = translationOverrideIfDifferent.m_Position;
						transOverride.m_Rotation = translationOverrideIfDifferent.m_Rotation;
						transOverride.m_Scale = translationOverrideIfDifferent.m_Scale;
						transOverride.m_ObjectName = translationOverrideIfDifferent.m_ObjectName;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					m_TransOverrides.Add(translationOverrideIfDifferent);
				}
			}
			num++;
		}
	}

	public void CaptureTranslationOverrides(Container cont, GameObject obj)
	{
		m_TransOverrides.Clear();
	}

	private TransformOverride GetTranslationOverrideIfDifferent(GameObject gameObj, TransformOverride trans)
	{
		Transform transform = gameObj.transform;
		bool flag = false;
		if (transform.localPosition != trans.m_Position)
		{
			flag = true;
		}
		else if (transform.localRotation != trans.m_Rotation)
		{
			flag = true;
		}
		else if (transform.localScale != trans.m_Scale)
		{
			flag = true;
		}
		if (flag)
		{
			TransformOverride transformOverride = new TransformOverride();
			transformOverride.m_Position = transform.localPosition;
			transformOverride.m_Rotation = transform.localRotation;
			transformOverride.m_Scale = transform.localScale;
			transformOverride.m_ObjectName = transform.name;
			transformOverride.m_ObjectGuid = GetGUIDFromGameObject(gameObj);
			if (transformOverride.m_ObjectGuid != string.Empty)
			{
				return transformOverride;
			}
		}
		return null;
	}

	private GameObject FindInnerObjectFromGuid(string guid)
	{
		if (guid == string.Empty)
		{
			return null;
		}
		if (this == null)
		{
			return null;
		}
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			if (guid == GetGUIDFromGameObject(transform.gameObject))
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private void Awake()
	{
	}
}
