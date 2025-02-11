public class EquipmentIconController : IconControllerBase
{
	public enum EquipmentType
	{
		Grenade = 0,
		Claymore = 1,
		SentryGun = 2,
		MediPack = 3,
		AmmoPack = 4,
		ArmourLevel1 = 5,
		ArmourLevel2 = 6,
		ArmourLevel3 = 7,
		ArmourLevelPro = 8,
		GrenadeClaymoreMediPack = 9,
		ClaymoreMediPack = 10,
		NumEquipmentType = 11
	}

	public void SetEquipment(EquipmentType equipment, bool available)
	{
		base.Index = (int)equipment;
		base.Available = available;
		CalculateSimpleSpriteSettings();
	}
}
