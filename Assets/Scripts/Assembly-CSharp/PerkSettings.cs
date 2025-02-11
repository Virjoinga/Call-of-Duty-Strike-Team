using System;

[Serializable]
public class PerkSettings : ISaveLoadNamed
{
	private const string DescriptorKey = ".Descriptor";

	private const string NumItemsKey = ".NumItems";

	public Perk Descriptor;

	public int NumItems;

	public void Save(string objectName)
	{
		SecureStorage.Instance.SetInt(objectName + ".Descriptor", (int)((Descriptor == null) ? PerkType.None : Descriptor.Identifier));
		SecureStorage.Instance.SetInt(objectName + ".NumItems", NumItems);
	}

	public void Load(string objectName)
	{
		PerkType @int = (PerkType)SecureStorage.Instance.GetInt(objectName + ".Descriptor", -1);
		Descriptor = StatsManager.Instance.PerksManager().GetPerk(@int);
		NumItems = SecureStorage.Instance.GetInt(objectName + ".NumItems");
	}

	public void Reset()
	{
		Descriptor = null;
		NumItems = 0;
	}
}
