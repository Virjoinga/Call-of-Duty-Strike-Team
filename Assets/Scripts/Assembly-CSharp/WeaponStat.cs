public class WeaponStat : SingleStat<WeaponStat>
{
	public int ShotsFired;

	public override void Reset()
	{
		ShotsFired = 0;
	}

	public override void CombineStat(WeaponStat statToAdd)
	{
		ShotsFired += statToAdd.ShotsFired;
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref ShotsFired, "ShotsFired");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref ShotsFired, "ShotsFired");
	}
}
