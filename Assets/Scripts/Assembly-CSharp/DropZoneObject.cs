using UnityEngine;

[RequireComponent(typeof(CMDropZoneObject))]
public class DropZoneObject : MonoBehaviour
{
	public enum DropZoneObjectType
	{
		SuppliesDZObject = 0,
		SupportDZObject = 1
	}

	public bool Locked = true;

	public DropZoneObjectType ObjectType;

	private EquipmentDescriptor mEquipment;

	public EquipmentDescriptor Equipment
	{
		get
		{
			return mEquipment;
		}
	}

	public void ActivateObject(EquipmentDescriptor equipmentDrop)
	{
		if (ObjectType == DropZoneObjectType.SuppliesDZObject)
		{
			mEquipment = equipmentDrop;
			Locked = false;
			base.gameObject.SetActive(true);
		}
	}
}
