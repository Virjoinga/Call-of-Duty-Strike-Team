using UnityEngine;

public class AchievementStat : SingleStat<AchievementStat>
{
	public int Step;

	public bool Update(int gameTotalAmount, int amount, bool instant)
	{
		int achievementSteps = StatsManager.Instance.GetAchievementSteps(base.Id);
		int b = achievementSteps - gameTotalAmount;
		amount = Mathf.Min(amount, b);
		Step += amount;
		bool flag = amount > 0 && amount + gameTotalAmount == achievementSteps;
		if ((flag || (amount > 0 && instant)) && MobileNetworkManager.Instance != null)
		{
			MobileNetworkManager.Instance.reportAchievement(base.Id, PercentCompleteFloat());
		}
		return flag;
	}

	public int PercentComplete()
	{
		int nSteps = StatsManager.Instance.AchievementsList.GetAchievement(base.Id).nSteps;
		return 100 * Step / nSteps;
	}

	public float PercentCompleteFloat()
	{
		int nSteps = StatsManager.Instance.AchievementsList.GetAchievement(base.Id).nSteps;
		return (float)(100 * Step) / (float)nSteps;
	}

	public override void Reset()
	{
		Step = 0;
	}

	public override void CombineStat(AchievementStat source)
	{
		Step += source.Step;
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref Step, "step");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref Step, "step");
	}
}
