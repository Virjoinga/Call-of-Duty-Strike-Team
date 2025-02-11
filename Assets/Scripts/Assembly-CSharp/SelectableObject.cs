using UnityEngine;

public class SelectableObject : MonoBehaviour
{
	public enum QuickType
	{
		Unspecified = 0,
		PlayerSoldier = 1,
		EnemySoldier = 2,
		WayPointMarker = 3,
		PreviewGhost = 4
	}

	public static int LayerId = 8;

	public static int LayerIdMask = 1 << LayerId;

	public static string TagName = "SelectableGameObject";

	public static string LayerName = "SelectableGameObject";

	public static float SelectionAssumptionDistance = 1f;

	private GameObject mAssociatedObject;

	public QuickType quickType;

	public GameObject AssociatedObject
	{
		get
		{
			if (mAssociatedObject == null)
			{
				return base.gameObject;
			}
			return mAssociatedObject;
		}
		set
		{
			mAssociatedObject = value;
		}
	}

	public bool ValidInCurrentCamera
	{
		get
		{
			return !GameController.Instance.IsFirstPerson || validInFirstPerson();
		}
	}

	public virtual Vector3 GetInterfacePosition()
	{
		return base.transform.position;
	}

	protected virtual bool validInFirstPerson()
	{
		return false;
	}

	private static SelectableObject CastAgainstSelectableLayer(Vector2 screenPos, ref bool allowHudCast)
	{
		SelectableObject selectableObject = null;
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(screenPos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, LayerIdMask))
		{
			return null;
		}
		selectableObject = hitInfo.collider.GetComponent<SelectableObject>();
		if (selectableObject == null || !selectableObject.enabled || !selectableObject.ValidInCurrentCamera)
		{
			return null;
		}
		allowHudCast = false;
		if (selectableObject.quickType == QuickType.PlayerSoldier)
		{
			Actor component = selectableObject.AssociatedObject.GetComponent<Actor>();
			if (component != null && GameplayController.Instance().IsSelected(component))
			{
				allowHudCast = true;
			}
		}
		return selectableObject;
	}

	private static void CastAgainstHudLayer(Vector2 screenPos, ref bool AllowNearerSelectedUnits, ref SelectableObject result)
	{
		Ray ray = GUISystem.Instance.m_guiCamera.ScreenPointToRay(screenPos);
		SelectableObject selectableObject = result;
		if (!Physics.Raycast(ray, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hud")))
		{
			return;
		}
		RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hud"));
		for (int i = 0; i < array.Length; i++)
		{
			HudBlipIcon component = array[i].collider.GetComponent<HudBlipIcon>();
			if (!(component != null))
			{
				continue;
			}
			SelectableObject component2 = component.Target.GetComponent<SelectableObject>();
			if (!(component2 != null) || !(component2 != selectableObject) || !component2.enabled || !component2.ValidInCurrentCamera)
			{
				continue;
			}
			result = component2;
			AllowNearerSelectedUnits = false;
			if (component2.quickType == QuickType.PlayerSoldier)
			{
				Actor myActor = (component2 as CMSoldier).MyActor;
				if (myActor.realCharacter.IsMortallyWounded())
				{
					break;
				}
			}
		}
	}

	public static SelectableObject SelectionHitTest(Vector2 screenPos)
	{
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController.IsTransitioning())
		{
			return null;
		}
		bool allowHudCast = true;
		bool AllowNearerSelectedUnits = true;
		SelectableObject result = null;
		if (!GameController.Instance.IsFirstPerson)
		{
			result = CastAgainstSelectableLayer(screenPos, ref allowHudCast);
			if (!allowHudCast)
			{
				SelectableObject result2 = null;
				bool AllowNearerSelectedUnits2 = false;
				CastAgainstHudLayer(screenPos, ref AllowNearerSelectedUnits2, ref result2);
				if (result2 != null && result2.quickType == QuickType.PlayerSoldier && (result2 as CMSoldier).MyActor.baseCharacter.IsMortallyWounded())
				{
					return result2;
				}
				return FindNearestSelectable(screenPos, false, AllowNearerSelectedUnits, result);
			}
		}
		CastAgainstHudLayer(screenPos, ref AllowNearerSelectedUnits, ref result);
		if (result != null)
		{
			if (result.quickType == QuickType.PlayerSoldier && (result as CMSoldier).MyActor.baseCharacter.IsMortallyWounded())
			{
				return result;
			}
			return FindNearestSelectable(screenPos, true, AllowNearerSelectedUnits, result);
		}
		return null;
	}

	public static SelectableObject DragHitTest(Vector2 screenPos)
	{
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController.IsTransitioning())
		{
			return null;
		}
		bool allowHudCast = true;
		SelectableObject selectableObject = null;
		if (!GameController.Instance.IsFirstPerson)
		{
			selectableObject = CastAgainstSelectableLayer(screenPos, ref allowHudCast);
			return FindNearestSelectable(screenPos, false, true, selectableObject);
		}
		return null;
	}

	public static SelectableObject FindNearestSelectable(Vector2 screenPos, bool AllowHUD, bool AllowSelectedUnits, SelectableObject firstPick)
	{
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(screenPos);
		return FindNearestSelectable(ray, AllowHUD, AllowSelectedUnits, firstPick);
	}

	public static SelectableObject FindNearestSelectable(Ray ray, bool AllowHUD, bool AllowSelectedUnits, SelectableObject firstPick)
	{
		float num = SelectionAssumptionDistance;
		if (firstPick != null)
		{
			num = DistanceToLine(ray, firstPick.GetInterfacePosition());
		}
		SelectableObject result = firstPick;
		Object[] array = GameObject.FindGameObjectsWithTag(TagName);
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			GameObject gameObject = @object as GameObject;
			SelectableObject component = gameObject.GetComponent<SelectableObject>();
			if (!(component != null) || !(component != firstPick) || !component.enabled || !component.ValidInCurrentCamera)
			{
				continue;
			}
			if (!AllowHUD)
			{
				if (component.quickType == QuickType.Unspecified)
				{
					continue;
				}
				if (component.quickType == QuickType.EnemySoldier || component.quickType == QuickType.PlayerSoldier)
				{
					BaseCharacter baseCharacter = (component as CMSoldier).MyActor.baseCharacter;
					if (baseCharacter != null && (baseCharacter.IsDead() || baseCharacter.IsMortallyWounded()))
					{
						continue;
					}
				}
			}
			if (component.quickType == QuickType.PlayerSoldier && !AllowSelectedUnits)
			{
				BaseCharacter baseCharacter = (component as CMSoldier).MyActor.baseCharacter;
				if (!baseCharacter.IsMortallyWounded() && GameplayController.Instance().IsSelected((component as CMPlayerSoldier).MyActor))
				{
					continue;
				}
			}
			float num2 = DistanceToLine(ray, component.GetInterfacePosition());
			if (num2 < num)
			{
				num = num2;
				result = component;
			}
		}
		return result;
	}

	public static SelectableObject PickSelectableObject(Vector2 screenPos)
	{
		return SelectionHitTest(screenPos);
	}

	public static SelectableObject PickDraggableObject(Vector2 screenPos)
	{
		return DragHitTest(screenPos);
	}

	public static SelectableObject PickandSelectSelectableObject(Vector2 screenPos)
	{
		SelectableObject selectableObject = PickSelectableObject(screenPos);
		if (selectableObject != null)
		{
			selectableObject.OnSelected(screenPos, false);
		}
		return selectableObject;
	}

	protected static float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	protected virtual void Start()
	{
	}

	protected virtual void Awake()
	{
		if (base.gameObject.layer == 0)
		{
			base.gameObject.layer = LayerId;
			base.gameObject.tag = TagName;
		}
	}

	public virtual int OnSelected(Vector2 selectedScreenPos, bool fromTap)
	{
		return 0;
	}
}
