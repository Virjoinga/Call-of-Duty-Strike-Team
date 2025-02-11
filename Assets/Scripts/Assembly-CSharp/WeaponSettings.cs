using System;

[Serializable]
public class WeaponSettings : ISaveLoadNamed
{
	private const string DescriptorKey = ".Descriptor";

	public WeaponDescriptor Descriptor;

	public int NumItems;

	public WeaponSettings()
	{
		Reset();
	}

	public void Reset()
	{
		Descriptor = null;
	}

	public void Save(string objectName)
	{
		SecureStorage.Instance.SetString(objectName + ".Descriptor", (!(Descriptor != null)) ? string.Empty : Descriptor.Name);
	}

	public void Load(string objectName)
	{
		WeaponDescriptor weaponDescriptor = WeaponManager.Instance.GetWeaponDescriptor(SecureStorage.Instance.GetString(objectName + ".Descriptor"));
		Descriptor = ((!(weaponDescriptor != null)) ? Descriptor : weaponDescriptor);
	}
}
