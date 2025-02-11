using System;

[Serializable]
public class SoldierSettings : ISaveLoadNamed
{
	public string Name;

	public WeaponSettings Weapon;

	public bool Present;

	public SoldierSettings()
	{
		Weapon = new WeaponSettings();
		Reset();
	}

	public void Reset()
	{
		Weapon.Reset();
	}

	public void Save(string objectName)
	{
		Weapon.Save(objectName);
	}

	public void Load(string objectName)
	{
		Weapon.Load(objectName);
	}
}
