public interface IWeaponEquip
{
	float GetEquipedBlendAmount();

	bool IsPuttingAway();

	bool IsTakingOut();

	void PutAway(float speedMultiplier);

	void TakeOut(float speedMultiplier);

	bool HasNoWeapon();

	void HasNoWeapon(bool noWeapon);
}
