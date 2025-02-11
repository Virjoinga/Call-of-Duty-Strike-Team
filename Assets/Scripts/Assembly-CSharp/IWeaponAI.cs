using UnityEngine;

public interface IWeaponAI
{
	bool PointingAtTarget(Vector3 pos, float radius);

	float CalculateUtility(float distance);

	float CalculateDamage(float distance, HitLocation target, bool isPlayer);

	float GetSuppressionRadius();
}
