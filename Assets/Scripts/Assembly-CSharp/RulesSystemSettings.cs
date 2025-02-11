public static class RulesSystemSettings
{
	public static float RunningAccuracyPenaltyMultiplier = 0.5f;

	public static float AvoidChanceInCoverWhenHiding = 1f;

	public static float AvoidChanceInCoverWhenShooting = 0.9f;

	public static float AvoidChanceMovingIntoCover = 0.8f;

	public static float AvoidChanceMovingIntoCoverDistanceThreshold = 10f;

	public static float AvoidChanceMovingIntoCoverDistThresholdSq = AvoidChanceMovingIntoCoverDistanceThreshold * AvoidChanceMovingIntoCoverDistanceThreshold;

	public static float AvoidChanceDodging = 0.25f;

	public static float ProjectileDamageDefault = 0.2f;

	public static float ProjectileDamageCritical = 20f;

	public static float ProjectileDamageExplosion = 1f;

	public static float ProjectileDamageFlanking = 0.4f;

	public static float AgainstAGRPenaltyModifier = 0.5f;

	public static float AgainstNoneHumanNoFlankingPenaltyModifier = 0f;
}
