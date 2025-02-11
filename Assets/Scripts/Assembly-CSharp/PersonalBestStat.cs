using UnityEngine;

public class PersonalBestStat : SingleStat<PersonalBestStat>
{
	private const float NeverPlayed = -1f;

	public float BestKDR;

	public int MostHeadShots;

	public int MostKills;

	public int HighestScore;

	public int HighestScoreAsVeteran;

	public float BestTimeToComplete;

	public bool BestTimeValid()
	{
		return BestTimeToComplete != -1f;
	}

	public override void Reset()
	{
		BestKDR = -1f;
		MostHeadShots = 0;
		MostKills = 0;
		HighestScore = 0;
		HighestScoreAsVeteran = 0;
		BestTimeToComplete = -1f;
	}

	public override void CombineStat(PersonalBestStat source)
	{
		BestKDR = Mathf.Max(BestKDR, source.BestKDR);
		MostHeadShots = Mathf.Max(MostHeadShots, source.MostHeadShots);
		MostKills = Mathf.Max(MostKills, source.MostKills);
		HighestScore = Mathf.Max(HighestScore, source.HighestScore);
		HighestScoreAsVeteran = Mathf.Max(HighestScoreAsVeteran, source.HighestScoreAsVeteran);
		if (BestTimeToComplete == -1f)
		{
			BestTimeToComplete = source.BestTimeToComplete;
		}
		else if (source.BestTimeToComplete != -1f)
		{
			BestTimeToComplete = Mathf.Min(BestTimeToComplete, source.BestTimeToComplete);
		}
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref BestKDR, "BestKDR");
		Save(prefix, ref MostHeadShots, "MostHeadShots");
		Save(prefix, ref MostKills, "MostKills");
		Save(prefix, ref HighestScore, "HighestScore");
		Save(prefix, ref HighestScoreAsVeteran, "HighestScoreAsVeteran");
		Save(prefix, ref BestTimeToComplete, "BestTimeToComplete");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref BestKDR, "BestKDR");
		Load(prefix, ref MostHeadShots, "MostHeadShots");
		Load(prefix, ref MostKills, "MostKills");
		Load(prefix, ref HighestScore, "HighestScore");
		Load(prefix, ref HighestScoreAsVeteran, "HighestScoreAsVeteran");
		Load(prefix, ref BestTimeToComplete, "BestTimeToComplete");
	}
}
