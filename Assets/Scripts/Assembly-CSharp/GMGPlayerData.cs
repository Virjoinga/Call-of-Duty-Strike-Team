public class GMGPlayerData : ISaveLoad
{
	private const string ScoreKey = "GMG.Score";

	private const string LeagueKey = "GMG.League";

	private const string NextResetKey = "GMG.NextReset";

	public int Score;

	public GlobalUnrestController.League League;

	public int NextReset;

	public void Save()
	{
		SecureStorage.Instance.SetInt("GMG.Score", Score);
		SecureStorage.Instance.SetInt("GMG.League", (int)League);
		SecureStorage.Instance.SetInt("GMG.NextReset", NextReset);
	}

	public void Load()
	{
		Score = SecureStorage.Instance.GetInt("GMG.Score", Score);
		League = (GlobalUnrestController.League)SecureStorage.Instance.GetInt("GMG.League", (int)League);
		NextReset = SecureStorage.Instance.GetInt("GMG.NextReset", NextReset);
	}

	public void Reset()
	{
		Score = 0;
		League = GlobalUnrestController.League.Iron;
		NextReset = 0;
	}
}
