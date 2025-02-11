using System;
using System.Collections.Generic;
using UnityEngine;

public class BagObject : MonoBehaviour
{
	public enum BagType
	{
		Default = 0,
		Prefab = 1,
		Template = 2
	}

	public enum CreationType
	{
		Default = 0,
		BaggedUp = 1
	}

	public enum SectionType
	{
		Default = 0,
		Activated = 1,
		DisabledTillSectionStarts = 2
	}

	public enum OverrideType
	{
		Undefined = 0,
		Interior = 1,
		CutScene = 2,
		SetPiece = 3,
		Enemy = 4,
		EnemyFromDoor = 5,
		EnemySecurityCamera = 6,
		SpawnerCoordinator = 7,
		AlarmTripped = 8,
		Combo = 9,
		DestroyObject = 10,
		EvadeCameras = 11,
		FindObject = 12,
		HackTerminalObjective = 13,
		KeepAlive = 14,
		Kill = 15,
		KillWave = 16,
		Pickup = 17,
		SquadCasualities = 18,
		UnitsInVolume = 19,
		UnitsNotInVolume = 20,
		BreachableDoor = 21,
		Collectable = 22,
		Discoverable = 23,
		InteriorCrate = 24,
		InteriorHandler = 25,
		Terminal = 26,
		Timer = 27,
		AwarenessZone = 28,
		DropZone = 29,
		MantleZone_3m = 30,
		MantleZone_High = 31,
		MantleZone_Low = 32,
		POI = 33,
		TetherPoint = 34,
		OverrideLogic = 35,
		SwitchRoutine = 36,
		MissionBriefing = 37,
		ScriptedCutscene = 38,
		GenericTrigger = 39,
		TriggerBox = 40,
		GateOverride = 41,
		AlarmPanelScenario = 42,
		DialogueManager = 43,
		SectionTrigger = 44,
		CoverPointManager = 45,
		NavGate = 46,
		DebugWarp = 47,
		SpawnController = 48,
		CustomRoutine = 49,
		ScriptedSequence = 50,
		BriefingShowandText = 51,
		ForwardMessageToGroup = 52,
		SendMessageOnRecievedCount = 53,
		MeshSwitch = 54,
		NPC = 55,
		DoorOverride = 56,
		CharacterEventListeners = 57,
		None = 58,
		Player = 59,
		EnemySentryTurret = 60,
		EnemyFromRoof = 61,
		CoverCluster = 62,
		Overwatch = 63,
		Destructible = 64,
		SendMessageDelayed = 65,
		DialogueTrigger = 66,
		SwitchEvents = 67,
		InflictDamage = 68,
		EnemySniper = 69,
		GateSlide = 70,
		MantleZone_WallVault = 71,
		MantleZone_WindowVault = 72,
		MetalGateSwing = 73,
		FixedGun = 74,
		Conditional = 75,
		CharacterPropertyModifier = 76,
		FakeAnimatingActor = 77,
		FakeGunman = 78,
		WorldActorsCommand = 79,
		BriefingUpdateText = 80,
		BriefingShowFocus = 81,
		BriefingShowImage = 82,
		ConditionalSequence = 83,
		ConditionalSequenceChooser = 84,
		AlarmOverrideLogic = 85,
		DebugActorWarp = 86,
		LinkedAIController = 87,
		GeneralArea = 88,
		ScriptDataModifier = 89,
		SniperFromDoor = 90,
		EnemyRiotShield = 91,
		MultiManSwitch = 92,
		ForceOnDeathTrigger = 93,
		TerminalLight = 94,
		ScriptVariable = 95,
		Locker = 96,
		Dumpster = 97,
		Summoner = 98,
		Helicopter = 99,
		AntiTankMine = 100,
		RPGEnemy = 101,
		ExplodableObject = 102,
		SuggestHUDMode = 103,
		AlarmManger = 104,
		ZipLine = 105,
		TargetPainter = 106,
		RPGFromDoor = 107,
		Scripted = 108,
		AudioZone = 109,
		BattleChatterZone = 110,
		StayAliveOnMiniGun = 111,
		PlacedC4 = 112,
		MantleZone_TwoManVault = 113,
		MantleZone_Low1M = 114,
		SiloGate = 115,
		MovableMagnet = 116,
		MantleZone_3MUp = 117,
		HackableObject = 118,
		TutorialEnemy = 119,
		RespotOnFail = 120,
		AmmoCache = 121,
		AmmoCacheController = 122,
		HostageSequence = 123,
		RiotShieldFromDoor = 124,
		BreachActor = 125,
		SquadMortallyWounded = 126,
		MusicTrigger = 127,
		MoreInfoTrigger = 128,
		TargetRangeEnemy = 129,
		DominationCapturePoint = 130,
		MysteryCache = 131,
		MysteryCacheController = 132
	}

	private const int kLayer_IgnoreRayCast = 2;

	private const int kThumbSize = 128;

	private const float kCamDistModifier = 1f;

	public List<ExposedLink> m_ExposedLinks = new List<ExposedLink>();

	public BagType m_Type;

	public SectionType m_SectionType;

	public bool m_Capture;

	public bool m_UseAutoCamera = true;

	public bool m_FixRotationIssues;

	public string m_MissionGuid = Guid.NewGuid().ToString();

	public string m_ReferenceID = string.Empty;

	public OverrideType m_OverrideType;

	public CreationType m_CreationType;

	public Texture2D m_AdditionalIcon;

	public bool m_UseExportedPosition;

	public bool m_Depreciated;

	public void GenGuid()
	{
		m_MissionGuid = Guid.NewGuid().ToString();
	}

	public static bool CheckEndTag(string name, string tag)
	{
		string text = TakeOffLODTag(name);
		if (text.ToLower().EndsWith(tag))
		{
			return true;
		}
		return false;
	}

	public static string TakeOffLODTag(string name)
	{
		if (name.Contains("_LOD"))
		{
			return name.Substring(0, name.Length - 5);
		}
		return name;
	}

	public static void AddBagElementComponents(GameObject obj)
	{
		AddBagElements(obj);
	}

	private static void AddBagElements(GameObject obj)
	{
		Transform transform = obj.transform;
		bool flag = true;
		if (obj.GetComponent<BagObject>() != null)
		{
			flag = false;
		}
		if (obj.GetComponent<Container>() != null)
		{
			flag = false;
		}
		if (flag)
		{
			GuidRefHook component = obj.GetComponent<GuidRefHook>();
			if (obj.GetComponent<BagElement>() == null)
			{
				BagElement bagElement = obj.AddComponent<BagElement>();
				if (component != null)
				{
					bagElement.m_Guid = component.m_Guid;
				}
			}
			else
			{
				BagElement[] components = obj.GetComponents<BagElement>();
				if (components.Length > 1)
				{
					List<BagElement> list = new List<BagElement>();
					for (int i = 1; i < components.Length; i++)
					{
						list.Add(components[i]);
					}
					foreach (BagElement item in list)
					{
						UnityEngine.Object.DestroyImmediate(item);
					}
				}
			}
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
		}
		if (transform.childCount <= 0)
		{
			return;
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			bool flag2 = true;
			GameObject gameObject = transform.GetChild(j).gameObject;
			if (gameObject != null)
			{
				if (gameObject.GetComponent<Container>() != null)
				{
					flag2 = false;
				}
				if (gameObject.GetComponent<BagObject>() != null)
				{
					flag2 = false;
				}
				if (flag2)
				{
					AddBagElements(gameObject);
				}
			}
		}
	}

	private void DisableAnimationPlayback(GameObject gameObj)
	{
		Animation[] componentsInChildren = gameObj.GetComponentsInChildren<Animation>(true);
		foreach (Animation animation in componentsInChildren)
		{
			if (animation != null)
			{
				animation.playAutomatically = false;
			}
		}
	}

	private float GetSurfaceIndex(string materialName)
	{
		SurfaceMaterial surfaceMaterial = SurfaceMaterial.Default;
		if (materialName.Equals("masonry_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Masonry;
		}
		else if (materialName.Equals("metal_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Metal;
		}
		else if (materialName.Equals("wood_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Wood;
		}
		else if (materialName.Equals("snow_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Snow;
		}
		else if (materialName.Equals("ice_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Ice;
		}
		else if (materialName.Equals("water_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Water;
		}
		else if (materialName.Equals("carpet_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Carpet;
		}
		else if (materialName.Equals("dirt_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Dirt;
		}
		else if (materialName.Equals("duct_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Duct;
		}
		else if (materialName.Equals("glass_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Glass;
		}
		else if (materialName.Equals("grass_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Grass;
		}
		else if (materialName.Equals("leaves_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Leaves;
		}
		else if (materialName.Equals("mud_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Mud;
		}
		else if (materialName.Equals("cement_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Cement;
		}
		else if (materialName.Equals("sand_col", StringComparison.InvariantCultureIgnoreCase))
		{
			surfaceMaterial = SurfaceMaterial.Sand;
		}
		return (float)surfaceMaterial;
	}

	public void ApplyCommonImport()
	{
		List<GameObject> list = new List<GameObject>();
		GetObjectsWithTag("_geom", list, true);
		GetObjectsWithTag("_seam", list, true);
		foreach (GameObject item in list)
		{
			item.isStatic = true;
		}
		List<GameObject> list2 = new List<GameObject>();
		GetObjectsWithTag("_ceil", list2, true);
		foreach (GameObject item2 in list2)
		{
			item2.isStatic = true;
		}
		List<GameObject> list3 = new List<GameObject>();
		GetObjectsWithTag("_tact", list3, false);
		foreach (GameObject item3 in list3)
		{
			item3.isStatic = false;
			item3.layer = StrategyViewModelHelper.LayerId;
			StrategyViewModelHelper component = item3.GetComponent<StrategyViewModelHelper>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
		}
		List<GameObject> list4 = new List<GameObject>();
		GetObjectsWithTag("_col", list4, false);
		GetObjectsWithTag("_dyncol", list4, false);
		foreach (GameObject item4 in list4)
		{
			item4.isStatic = false;
			item4.layer = 0;
			MeshFilter component2 = item4.GetComponent<MeshFilter>();
			if (component2 != null && component2.sharedMesh != null)
			{
				item4.name = component2.sharedMesh.name;
			}
			MeshRenderer component3 = item4.GetComponent<MeshRenderer>();
			if (component3 != null)
			{
				UnityEngine.Object.DestroyImmediate(component3);
			}
			MeshCollider component4 = item4.GetComponent<MeshCollider>();
			if (component4 == null)
			{
				item4.AddComponent<MeshCollider>();
			}
			StrategyViewModelHelper component5 = item4.GetComponent<StrategyViewModelHelper>();
			if (component5 != null)
			{
				UnityEngine.Object.DestroyImmediate(component5);
			}
		}
		List<GameObject> list5 = new List<GameObject>();
		GetObjectsWithTag("_vol", list5, false);
		foreach (GameObject item5 in list5)
		{
			if (item5.GetComponent<MeshCollider>() == null)
			{
				MeshCollider meshCollider = item5.AddComponent<MeshCollider>();
				meshCollider.isTrigger = true;
				meshCollider.convex = false;
			}
			if (item5.GetComponent<MeshRenderer>() != null)
			{
				UnityEngine.Object.DestroyImmediate(item5.GetComponent<MeshRenderer>());
			}
			item5.isStatic = false;
			item5.layer = 2;
		}
		DisableAnimationPlayback(base.gameObject);
		if (m_OverrideType == OverrideType.Undefined)
		{
			AutoAssignBagType();
		}
		AutoAssignImportProcesses();
		AddBagElementComponents(base.gameObject);
		RemoveAnimationNodeMeshes(base.gameObject);
	}

	private void RemoveAnimationNodeMeshes(GameObject go)
	{
		MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			Mesh sharedMesh = meshFilter.sharedMesh;
			if (sharedMesh != null && (sharedMesh.name.Contains("NODE_") || sharedMesh.name.Contains("PROP_") || sharedMesh.name.Contains("Node_")))
			{
				MeshRenderer component = meshFilter.gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	private void AutoAssignBagType()
	{
		if (base.gameObject.GetComponent<InteriorBagProcess>() != null)
		{
			m_OverrideType = OverrideType.Interior;
		}
		if (FindInImmediateChildren<BuildingWithInterior>())
		{
			m_OverrideType = OverrideType.Interior;
		}
		if (FindInImmediateChildren<SetPieceLogic>())
		{
			m_OverrideType = OverrideType.SetPiece;
		}
	}

	private bool FindInImmediateChildren<T>()
	{
		if (base.transform.childCount > 0)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				GameObject gameObject = base.transform.GetChild(i).gameObject;
				if (gameObject.GetComponent(typeof(T)) != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AssignBagProcess<T>(OverrideType overrideType) where T : BaseBagProcess
	{
		if (m_OverrideType == overrideType && (UnityEngine.Object)base.gameObject.GetComponent<T>() == (UnityEngine.Object)null)
		{
			T val = base.gameObject.AddComponent<T>();
			val.ApplyProcess(base.gameObject);
		}
	}

	private void AutoAssignImportProcesses()
	{
		AssignBagProcess<InteriorBagProcess>(OverrideType.Interior);
		AssignBagProcess<SetPieceBagProcess>(OverrideType.SetPiece);
	}

	public void GetObjectsWithTag(string tag, List<GameObject> objList, bool contains)
	{
		MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>(true);
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			OverrideNaming component = meshFilter.GetComponent<OverrideNaming>();
			if ((component != null && component.IgnoreTag(tag)) || !(meshFilter.sharedMesh != null))
			{
				continue;
			}
			if (contains)
			{
				if (meshFilter.sharedMesh.name.ToLowerInvariant().Contains(tag))
				{
					objList.Add(meshFilter.gameObject);
				}
			}
			else if (CheckEndTag(meshFilter.sharedMesh.name, tag))
			{
				objList.Add(meshFilter.gameObject);
			}
		}
	}

	public ExposedLink GetExposedLink(string exposedName)
	{
		return m_ExposedLinks.Find((ExposedLink obj) => obj.m_LinkName == exposedName);
	}

	public bool ExposeLink(GameObject go)
	{
		foreach (ExposedLink exposedLink2 in m_ExposedLinks)
		{
			if (exposedLink2.m_LinkName == go.name)
			{
				return false;
			}
		}
		ExposedLink exposedLink = new ExposedLink();
		exposedLink.m_LinkReference = new LinkReference();
		BagElement component = go.GetComponent<BagElement>();
		if (component != null)
		{
			exposedLink.m_LinkReference.m_Guid = component.m_Guid;
		}
		exposedLink.m_LinkReference.m_ObjectName = go.name;
		exposedLink.m_LinkName = go.name;
		m_ExposedLinks.Add(exposedLink);
		return true;
	}

	private Bounds CalculateBounds(GameObject go)
	{
		Bounds result = new Bounds(go.transform.position, Vector3.zero);
		Component[] componentsInChildren = go.GetComponentsInChildren(typeof(Renderer), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = (Renderer)array[i];
			result.Encapsulate(renderer.bounds);
		}
		return result;
	}

	private void FocusCameraOnGameObject(Camera c, GameObject go)
	{
		Bounds bounds = CalculateBounds(go);
		Vector3 size = bounds.size;
		float num = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
		float num2 = num / Mathf.Sin(c.fieldOfView * ((float)Math.PI / 180f) / 2f);
		float num3 = 0.65f;
		Vector3 vector = new Vector3(-0.2f, 0.25f, 0.8f);
		if (BagManager.Instance.m_ThumbDirection == BagManager.ThumbDir.kBack)
		{
			vector.z = -0.8f;
		}
		vector.Normalize();
		Vector3 position = vector * (num2 * num3) + bounds.center;
		c.transform.position = position;
		c.transform.LookAt(bounds.center);
	}

	public void MakeThumbnail()
	{
	}
}
