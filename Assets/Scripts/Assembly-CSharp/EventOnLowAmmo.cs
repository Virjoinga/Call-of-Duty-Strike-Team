using UnityEngine;

public class EventOnLowAmmo : EventDescriptor
{
	public delegate bool AmmoCheckDelegate();

	public bool ZeroAmmoTest;

	public AmmoCheckDelegate AmmoCheck;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null && component.weapon != null)
		{
			if (ZeroAmmoTest)
			{
				AmmoCheck = component.weapon.PrimaryWeapon.HasAmmo;
			}
			else
			{
				AmmoCheck = component.weapon.PrimaryWeapon.LowAmmo;
			}
		}
	}

	public void Update()
	{
		if (AmmoCheck != null && AmmoCheck() != ZeroAmmoTest)
		{
			FireEvent();
		}
	}
}
