public class WeaponIconController : IconControllerBase
{
	public enum WeaponType
	{
		KSG = 0,
		M1216 = 1,
		AN94 = 2,
		M8A1 = 3,
		Type25 = 4,
		XPR50 = 5,
		Ballista = 6,
		HAMR = 7,
		LSAT = 8,
		KS23 = 9,
		SVUAS = 10,
		QBBLSW = 11,
		Vektor = 12,
		Skorpion = 13,
		PDW57 = 14,
		NumWeaponType = 15
	}

	public void SetWeapon(WeaponType weapon, bool available)
	{
		base.Index = (int)weapon;
		base.Available = available;
		CalculateSimpleSpriteSettings();
	}
}
