public class MissionStat : SingleStat<MissionStat>
{
	public int NumTimesPlayed;

	public int NumTimesSucceeded;

	public int ScoreAwarded;

	public float TimePlayed;

	public int SpecOpsWavesCompleted;

	public int HighestSpecOpsWaveCompleted;

	public int NumGrenadeAndClaymoreKills;

	public override void Reset()
	{
		NumTimesPlayed = 0;
		NumTimesSucceeded = 0;
		ScoreAwarded = 0;
		TimePlayed = 0f;
		SpecOpsWavesCompleted = 0;
		HighestSpecOpsWaveCompleted = 0;
		NumGrenadeAndClaymoreKills = 0;
	}

	public override void CombineStat(MissionStat source)
	{
		NumTimesPlayed += source.NumTimesPlayed;
		NumTimesSucceeded += source.NumTimesSucceeded;
		ScoreAwarded += source.ScoreAwarded;
		TimePlayed += source.TimePlayed;
		SpecOpsWavesCompleted += source.SpecOpsWavesCompleted;
		if (source.HighestSpecOpsWaveCompleted > HighestSpecOpsWaveCompleted)
		{
			HighestSpecOpsWaveCompleted = source.HighestSpecOpsWaveCompleted;
		}
		NumGrenadeAndClaymoreKills += source.NumGrenadeAndClaymoreKills;
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref NumTimesPlayed, "NumTimesPlayed");
		Save(prefix, ref NumTimesSucceeded, "NumTimesSucceeded");
		Save(prefix, ref ScoreAwarded, "ScoreAwarded");
		Save(prefix, ref TimePlayed, "TimePlayed");
		Save(prefix, ref SpecOpsWavesCompleted, "SpecOpsWavesCompleted");
		Save(prefix, ref HighestSpecOpsWaveCompleted, "HighestSpecOpsWaveCompleted");
		Save(prefix, ref NumGrenadeAndClaymoreKills, "NumGrenadeAndClaymoreKills");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref NumTimesPlayed, "NumTimesPlayed");
		Load(prefix, ref NumTimesSucceeded, "NumTimesSucceeded");
		Load(prefix, ref ScoreAwarded, "ScoreAwarded");
		Load(prefix, ref TimePlayed, "TimePlayed");
		Load(prefix, ref SpecOpsWavesCompleted, "SpecOpsWavesCompleted");
		Load(prefix, ref HighestSpecOpsWaveCompleted, "HighestSpecOpsWaveCompleted");
		Load(prefix, ref NumGrenadeAndClaymoreKills, "NumGrenadeAndClaymoreKills");
	}
}
