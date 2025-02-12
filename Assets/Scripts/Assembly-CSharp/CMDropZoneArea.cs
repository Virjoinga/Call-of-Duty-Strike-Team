using UnityEngine;

public class CMDropZoneArea : InterfaceableObject
{
	private DropZoneObject SuppliesObjRef;

	private DropZoneArea DZref;

	public DropZoneBlip DropZoneBlipObj
	{
		get
		{
			return mContextBlip as DropZoneBlip;
		}
	}

	protected override void Start()
	{
		base.gameObject.layer = SelectableObject.LayerId;
		base.gameObject.tag = SelectableObject.TagName;
		if (GameplayController.instance == null)
		{
			UseBlip = false;
		}
		if (UseBlip)
		{
			mContextBlip = Object.Instantiate(GameplayController.instance.DropZoneObjBlip) as DropZoneBlip;
			mContextBlip.Target = base.transform;
			Vector3 zero = Vector3.zero;
			MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
			Vector3 position = Vector3.zero;
			if (component != null && !base.GetComponent<Renderer>().isPartOfStaticBatch)
			{
				Mesh mesh = component.mesh;
				if (mesh != null)
				{
					position = mesh.bounds.center;
				}
			}
			zero = base.transform.TransformPoint(position) - base.transform.position;
			mContextBlip.WorldOffset = zero;
		}
		DZref = base.gameObject.GetComponent<DropZoneArea>();
		if (DropZoneBlipObj != null && DZref != null)
		{
			if (DZref.m_Interface.Locked)
			{
				DropZoneBlipObj.SetLocked();
			}
			else
			{
				DropZoneBlipObj.SetUnlocked();
			}
		}
	}

	protected override void PopulateMenuItems()
	{
		if (DZref.m_Interface.Locked)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		DropZoneObject[] dropZoneObjects = DZref.DropZoneObjects;
		foreach (DropZoneObject dropZoneObject in dropZoneObjects)
		{
			if (dropZoneObject.Locked)
			{
				switch (dropZoneObject.ObjectType)
				{
				case DropZoneObject.DropZoneObjectType.SuppliesDZObject:
					SuppliesObjRef = dropZoneObject;
					flag = true;
					break;
				case DropZoneObject.DropZoneObjectType.SupportDZObject:
					flag2 = true;
					break;
				}
			}
		}
		if (flag)
		{
			AddCallableMethod("S_CMBuySupplies", ContextMenuIcons.Heal);
		}
		if (flag2)
		{
			AddCallableMethod("S_CMBuySupport", ContextMenuIcons.Repair);
		}
	}

	public void S_CMBuySupplies()
	{
		StrategyHudController instance = StrategyHudController.Instance;
		if (instance != null)
		{
			instance.GoToTechPurchase(new TechPurchaseEventArgs(DZref, SuppliesObjRef));
		}
		CleaupAfterSelection();
	}

	public void S_CMBuySupport()
	{
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			instance.DoSupportShopCommingSoonDialogue();
		}
		CleaupAfterSelection();
	}

	private void CleaupAfterSelection()
	{
		SuppliesObjRef = null;
		CommonHudController.Instance.ClearContextMenu();
		bool flag = true;
		DropZoneObject[] dropZoneObjects = DZref.DropZoneObjects;
		foreach (DropZoneObject dropZoneObject in dropZoneObjects)
		{
			if (dropZoneObject.Locked)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			(mContextBlip as DropZoneBlip).SetEmpty();
		}
	}
}
