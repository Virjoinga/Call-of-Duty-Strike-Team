public class SoldierFiringState
{
	public enum FireType
	{
		None = 0,
		Hold = 1,
		Burst = 2,
		FullAuto = 3
	}

	public Actor desiredTarget;

	public FireType desiredFireType;

	public bool aimingAtTarget;

	public bool animsAllowReload = true;

	public bool characterPermitsFiring = true;

	public bool ShootingIsDesiredAndAllowed()
	{
		if (desiredTarget == null)
		{
			return false;
		}
		if (desiredFireType <= FireType.Hold)
		{
			return false;
		}
		return aimingAtTarget & characterPermitsFiring;
	}

	public bool ShootingIsAllowed()
	{
		if (desiredTarget == null)
		{
			return false;
		}
		return aimingAtTarget & characterPermitsFiring;
	}
}
