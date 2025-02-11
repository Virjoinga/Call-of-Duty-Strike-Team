using UnityEngine;

public interface IWeaponControl
{
	bool NeedsTriggerRelease();

	Vector2 GetSemiAutoFireRates();
}
