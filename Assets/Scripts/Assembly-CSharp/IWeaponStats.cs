public interface IWeaponStats
{
	bool IsLongRangeShot(float distSquared);

	bool IsHeadShotAllowed(float distSquared);

	float AccuracyStatAdjustment();
}
