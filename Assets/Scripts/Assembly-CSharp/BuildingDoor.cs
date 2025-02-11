using UnityEngine;

[RequireComponent(typeof(CMBuildingDoor))]
public class BuildingDoor : MonoBehaviour
{
	public enum eDoorType
	{
		None = 0,
		Arcti_1 = 1,
		Arcti_2 = 2,
		Arcti_3 = 3,
		Afgan_1 = 4,
		Afgan_2 = 5,
		Afgan_3 = 6,
		Afghan_4 = 7,
		Pakis_1 = 8,
		Pakis_2 = 9,
		Pakis_3 = 10,
		Kowlo_1 = 11,
		Kowlo_2 = 12,
		Kowlo_3 = 13
	}

	public enum DoorSate
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3,
		Missing = 4,
		LockedClosed = 5,
		LockedOpen = 6
	}

	public enum DoorDirection
	{
		CW = 0,
		ACW = 1
	}

	public BreachableDoorData m_Interface;

	public NavGate m_NavGate;

	public DoorNavigator m_DoorNavigator;

	public BuildingWithInterior mAssociatedBuilding;

	public MeshFilter DoorMesh;

	public MeshFilter AdditionalDoorMesh;

	public BuildingDoor SiblingDoor;

	public bool m_Breached;

	private bool mIsSiblingDriver;

	public BreachSequence BreachSequence { get; set; }

	public bool IsSiblingDriver
	{
		get
		{
			return mIsSiblingDriver;
		}
	}

	public BuildingWithInterior AssociatedBuilding
	{
		get
		{
			return mAssociatedBuilding;
		}
	}

	public Vector3 ExplosivePlantPosition
	{
		get
		{
			if (m_Interface.ExplosionOrigin != null)
			{
				return m_Interface.ExplosionOrigin.position;
			}
			return Vector3.zero;
		}
	}

	public static string GetModelForDoor(eDoorType door)
	{
		switch (door)
		{
		case eDoorType.Arcti_1:
			return "Assets/Models/Props/animated/prop_arc_doorbreach01_cw_geom.fbx";
		case eDoorType.Arcti_2:
			return "Assets/Models/Props/animated/prop_arc_doorbreach02_cw_geom.fbx";
		case eDoorType.Arcti_3:
			return "Assets/Models/Props/animated/prop_arc_officedor01_cw_geom.fbx";
		case eDoorType.Afgan_1:
			return "Assets/Models/Props/animated/prop_afg_doorbreach01_cw_geom.fbx";
		case eDoorType.Afgan_2:
			return "Assets/Models/Props/animated/prop_afg_doorbreach02_cw_geom.fbx";
		case eDoorType.Afgan_3:
			return "Assets/Models/Props/animated/prop_afg_doorbreach03_cw_geom.fbx";
		case eDoorType.Afghan_4:
			return "Assets/Models/Props/animated/prop_afg_doorbreach04_cw_geom.fbx";
		case eDoorType.Pakis_1:
			return "Assets/Models/Props/animated/prop_pak_doorbreach01_cw_geom.fbx";
		case eDoorType.Pakis_2:
			return "Assets/Models/Props/animated/prop_pak_doorbreach02_cw_geom.fbx";
		case eDoorType.Pakis_3:
			return "Assets/Models/Props/animated/prop_pak_doorbreach03_cw_geom.fbx";
		case eDoorType.Kowlo_1:
			return "Assets/Models/Props/animated/prop_kow_doorbreach01_cw_geom.fbx";
		case eDoorType.Kowlo_2:
			return "Assets/Models/Props/animated/prop_kow_doorbreach02_cw_geom.fbx";
		case eDoorType.Kowlo_3:
			return "Assets/Models/Props/animated/prop_kow_doorbreach03_cw_geom.fbx";
		default:
			return null;
		}
	}

	public void TryAndFindAssociatedBuilding()
	{
		Transform parent = base.transform.parent;
		while (parent != null && AssociatedBuilding == null)
		{
			mAssociatedBuilding = parent.GetComponentInChildren(typeof(BuildingWithInterior)) as BuildingWithInterior;
			parent = parent.parent;
		}
	}

	private void Start()
	{
		if (AssociatedBuilding == null)
		{
			TryAndFindAssociatedBuilding();
		}
		if (m_NavGate == null)
		{
			m_NavGate = GetComponentInChildren(typeof(NavGate)) as NavGate;
			TBFAssert.DoAssert(m_NavGate != null, "Unable to find NavGate for building door!");
		}
		if (m_DoorNavigator == null)
		{
			m_DoorNavigator = GetComponentInChildren<DoorNavigator>();
		}
		if (DoorMesh == null || DoorMesh.gameObject == null)
		{
			Debug.LogError("missing door on - " + base.gameObject.name);
		}
		DoorMesh.gameObject.layer = base.gameObject.layer;
		DoorMesh.gameObject.tag = base.gameObject.tag;
		if (SiblingDoor != null && SiblingDoor.AssociatedBuilding != null)
		{
			mIsSiblingDriver = true;
			SiblingDoor.mIsSiblingDriver = false;
		}
		if (m_DoorNavigator != null)
		{
			m_DoorNavigator.DoorMesh = DoorMesh.gameObject;
			m_DoorNavigator.CacheDoorPosition();
			m_DoorNavigator.BuildingDoorRef = this;
		}
		m_NavGate.SetNavGate(m_Interface.WalkableBy);
		if (m_Interface.State == DoorSate.Open || m_Interface.State == DoorSate.LockedOpen)
		{
			SetupAsOpen();
		}
		if (m_Interface.StartLocked || m_Interface.StartContextDisabled)
		{
			Lock();
			Deactivate();
		}
		else
		{
			Unlock();
		}
		if (m_Interface.StartContextDisabled)
		{
			Deactivate();
		}
	}

	private void SetupAsOpen()
	{
		Debug.LogWarning("AC: Setting up doors as open currently doesn't animate them open.");
		SetState(DoorSate.Open);
	}

	private void BreachFinished()
	{
		m_Interface.State = DoorSate.Missing;
		if (m_NavGate != null)
		{
			m_NavGate.SetNavGate(NavGateData.CharacterType.All);
			RemoveCMDoor();
		}
		if (m_DoorNavigator != null)
		{
			m_DoorNavigator.PermanentlyOpen();
		}
	}

	public void Lock()
	{
		switch (m_Interface.State)
		{
		case DoorSate.Closed:
			SetState(DoorSate.LockedClosed);
			break;
		case DoorSate.Open:
			SetState(DoorSate.LockedOpen);
			break;
		case DoorSate.LockedClosed:
		case DoorSate.LockedOpen:
			break;
		case DoorSate.Opening:
		case DoorSate.Closing:
			Debug.LogWarning("Attempt to lock a door while it's opening/closing failed.");
			break;
		case DoorSate.Missing:
			break;
		}
	}

	public void Unlock()
	{
		switch (m_Interface.State)
		{
		case DoorSate.LockedClosed:
			SetState(DoorSate.Closed);
			break;
		case DoorSate.LockedOpen:
			SetState(DoorSate.Open);
			break;
		case DoorSate.Opening:
		case DoorSate.Closing:
			Debug.LogWarning("Attempt to unlock a door while it's opening/closing failed.");
			break;
		}
		Activate();
	}

	public void SetState(DoorSate newState)
	{
		if (IsSiblingDriver && SiblingDoor != null)
		{
			SiblingDoor.SetState(newState);
		}
		switch (newState)
		{
		case DoorSate.Closed:
		case DoorSate.Closing:
			if (m_NavGate != null)
			{
				m_NavGate.SetNavGate(m_Interface.WalkableBy);
			}
			break;
		case DoorSate.LockedClosed:
			if (m_NavGate != null)
			{
				m_NavGate.SetNavGate(NavGateData.CharacterType.None);
			}
			break;
		case DoorSate.Open:
		case DoorSate.LockedOpen:
			if (m_NavGate != null)
			{
				m_NavGate.SetNavGate(NavGateData.CharacterType.All);
			}
			break;
		case DoorSate.Missing:
			RemoveCMDoor();
			if (m_NavGate != null)
			{
				m_NavGate.SetNavGate(NavGateData.CharacterType.All);
			}
			break;
		}
		m_Interface.State = newState;
	}

	public void Breach(Actor breachActor)
	{
		if (m_Interface.State != DoorSate.Missing)
		{
			if (IsSiblingDriver && SiblingDoor != null)
			{
				SiblingDoor.Breach(null);
			}
			m_Interface.State = DoorSate.Missing;
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			float num = ((m_Interface.OpeningDirection != 0) ? m_Interface.OpenAngle : (0f - m_Interface.OpenAngle));
			eulerAngles.y = base.transform.eulerAngles.y + num;
			eulerAngles.x = 270f;
			DoorMesh.gameObject.RotateTo(eulerAngles, 0.2f, 0f, EaseType.easeInOutCubic, "BreachFinished", base.gameObject);
			if (breachActor != null)
			{
				RulesSystem.DoAreaOfEffectDamage(base.transform.position, 0.5f, 1f, base.gameObject, ExplosionManager.ExplosionType.DoorBreach, "Explosion");
				ExplosivesSFX.Instance.C4DoorBreachExplosion.Play(base.gameObject);
			}
			m_Breached = true;
		}
	}

	public void RemoveCMDoor()
	{
		Object.Destroy(this);
		CMBuildingDoor component = base.gameObject.GetComponent<CMBuildingDoor>();
		if ((bool)component)
		{
			Object.Destroy(component);
		}
	}

	public void Activate()
	{
		CMBuildingDoor component = base.gameObject.GetComponent<CMBuildingDoor>();
		ContextMenuDistanceManager componentInChildren = IncludeDisabled.GetComponentInChildren<ContextMenuDistanceManager>(base.gameObject);
		if ((bool)componentInChildren)
		{
			componentInChildren.enabled = true;
		}
		if (component != null)
		{
			component.Activate();
		}
	}

	public void Deactivate()
	{
		CMBuildingDoor component = base.gameObject.GetComponent<CMBuildingDoor>();
		if (component != null)
		{
			component.Deactivate();
		}
		ContextMenuDistanceManager componentInChildren = IncludeDisabled.GetComponentInChildren<ContextMenuDistanceManager>(base.gameObject);
		if ((bool)componentInChildren)
		{
			componentInChildren.enabled = false;
		}
	}
}
