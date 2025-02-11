public class PlayerStat : SingleStat<PlayerStat>
{
	public int ScoreAsVeteran;

	public int Score;

	public int HardCurrencyEarned;

	public int HardCurrencyPurchased;

	public int HardCurrencySpent;

	public int XP;

	public int XPFromMissionComplete;

	public int XPBeforeMultipler;

	public override void Reset()
	{
		ScoreAsVeteran = 0;
		Score = 0;
		HardCurrencyEarned = 0;
		HardCurrencyPurchased = 0;
		HardCurrencySpent = 0;
		XP = 0;
		XPFromMissionComplete = 0;
		XPBeforeMultipler = 0;
	}

	public override void CombineStat(PlayerStat source)
	{
		ScoreAsVeteran += source.ScoreAsVeteran;
		Score += source.Score;
		HardCurrencyEarned += source.HardCurrencyEarned;
		HardCurrencyPurchased += source.HardCurrencyPurchased;
		HardCurrencySpent += source.HardCurrencySpent;
		XP += source.XP;
		XPFromMissionComplete += source.XPFromMissionComplete;
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref ScoreAsVeteran, "scoreAsVeteran");
		Save(prefix, ref Score, "score");
		Save(prefix, ref HardCurrencyEarned, "hardcurrencyearned");
		Save(prefix, ref HardCurrencyPurchased, "hardcurrencypurchased");
		Save(prefix, ref HardCurrencySpent, "hardcurrencyspent");
		Save(prefix, ref XP, "xp");
		Save(prefix, ref XPFromMissionComplete, "xpmissionComplete");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref ScoreAsVeteran, "scoreAsVeteran");
		Load(prefix, ref Score, "score");
		Load(prefix, ref HardCurrencyEarned, "hardcurrencyearned");
		Load(prefix, ref HardCurrencyPurchased, "hardcurrencypurchased");
		Load(prefix, ref HardCurrencySpent, "hardcurrencyspent");
		Load(prefix, ref XP, "xp");
		Load(prefix, ref XPFromMissionComplete, "xpmissionComplete");
	}
}
