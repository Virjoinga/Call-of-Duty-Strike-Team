using System;

[Serializable]
public class EquipmentSettings : ISaveLoadNamed
{
	private const string NumItemsKey = ".NumItems";

	private const string SlotSizeKey = ".SlotSizeLevel";

	private const string ExtraSizeKey = ".ExtraSize";

	public const int MAX_SLOT_SIZE_LEVELS = 4;

	public const int TOTAL_MAX_EQUIPMENT_SLOT_SIZE = 16;

	public const int EQUIPMENT_SLOT_SIZE_INCREMENT = 3;

	public EquipmentDescriptor Descriptor;

	public int NumItems;

	public int SlotSizeLevel;

	public int ExtraSlotSize;

	public int SlotSize
	{
		get
		{
			return (SlotSizeLevel + 1) * 3 + ExtraSlotSize;
		}
	}

	public void Save(string objectName)
	{
		SecureStorage.Instance.SetInt(objectName + ".SlotSizeLevel", SlotSizeLevel);
		SecureStorage.Instance.SetInt(objectName + ".ExtraSize", ExtraSlotSize);
		SecureStorage.Instance.SetInt(objectName + ".NumItems", NumItems);
	}

	public void Load(string objectName)
	{
		SlotSizeLevel = SecureStorage.Instance.GetInt(objectName + ".SlotSizeLevel", 0);
		ExtraSlotSize = SecureStorage.Instance.GetInt(objectName + ".ExtraSize", 0);
		NumItems = SecureStorage.Instance.GetInt(objectName + ".NumItems", NumItems);
	}

	public void Reset()
	{
		NumItems = 0;
		SlotSizeLevel = 0;
		ExtraSlotSize = 0;
	}
}
