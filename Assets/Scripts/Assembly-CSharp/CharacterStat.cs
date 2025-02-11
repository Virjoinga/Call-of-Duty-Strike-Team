public class CharacterStat : SingleStat<CharacterStat>
{
	public float AccumulatedShotCount;

	public int NumTimesKilled;

	public int NumTimesHealed;

	public int NumKills;

	public int NumHeadShots;

	public int NumTimesKilledInFP;

	public int NumKillsInFP;

	public int NumShotsFiredInFP;

	public int NumShotsHitInFP;

	public int NumHeadShotsInFP;

	public float KDR
	{
		get
		{
			if (NumTimesKilled == 0)
			{
				return NumKills;
			}
			return (float)NumKills / (float)NumTimesKilled;
		}
	}

	public float KDRinFP
	{
		get
		{
			if (NumTimesKilledInFP == 0)
			{
				return NumKillsInFP;
			}
			return (float)NumKillsInFP / (float)NumTimesKilledInFP;
		}
	}

	public float AccuracyInFP
	{
		get
		{
			if (NumShotsFiredInFP == 0)
			{
				return -1f;
			}
			return 100f * (float)NumShotsHitInFP / (float)NumShotsFiredInFP;
		}
	}

	public override void Reset()
	{
		NumTimesKilled = 0;
		NumTimesHealed = 0;
		NumKills = 0;
		NumHeadShots = 0;
		NumTimesKilledInFP = 0;
		NumKillsInFP = 0;
		NumShotsFiredInFP = 0;
		NumShotsHitInFP = 0;
		NumHeadShotsInFP = 0;
	}

	public override void CombineStat(CharacterStat source)
	{
		NumTimesKilled += source.NumTimesKilled;
		NumTimesHealed += source.NumTimesHealed;
		NumKills += source.NumKills;
		NumHeadShots += source.NumHeadShots;
		NumTimesKilledInFP += source.NumTimesKilledInFP;
		NumKillsInFP += source.NumKillsInFP;
		NumShotsFiredInFP += source.NumShotsFiredInFP;
		NumShotsHitInFP += source.NumShotsHitInFP;
		NumHeadShotsInFP += source.NumHeadShotsInFP;
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref NumTimesKilled, "NumTimesKilled");
		Save(prefix, ref NumTimesHealed, "NumTimesHealed");
		Save(prefix, ref NumKills, "NumKills");
		Save(prefix, ref NumHeadShots, "NumHeadShots");
		Save(prefix, ref NumTimesKilledInFP, "NumTimesKilledInFP");
		Save(prefix, ref NumKillsInFP, "NumKillsInFP");
		Save(prefix, ref NumShotsFiredInFP, "NumShotsFiredInFP");
		Save(prefix, ref NumShotsHitInFP, "NumShotsHitInFP");
		Save(prefix, ref NumHeadShotsInFP, "NumHeadShotsInFP");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref NumTimesKilled, "NumTimesKilled");
		Load(prefix, ref NumTimesHealed, "NumTimesHealed");
		Load(prefix, ref NumKills, "NumKills");
		Load(prefix, ref NumHeadShots, "NumHeadShots");
		Load(prefix, ref NumTimesKilledInFP, "NumTimesKilledInFP");
		Load(prefix, ref NumKillsInFP, "NumKillsInFP");
		Load(prefix, ref NumShotsFiredInFP, "NumShotsFiredInFP");
		Load(prefix, ref NumShotsHitInFP, "NumShotsHitInFP");
		Load(prefix, ref NumHeadShotsInFP, "NumHeadShotsInFP");
	}
}
