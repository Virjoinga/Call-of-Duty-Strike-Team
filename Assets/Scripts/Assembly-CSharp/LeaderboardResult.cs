public class LeaderboardResult
{
	public string Name;

	public ulong UserID;

	public long Rating;

	public ulong LeaderboardRank;

	public bool IsPlayer;

	public bool Elite;

	public int Faction;

	public int Wave;

	public int Rank;

	public int XP;

	public bool Veteran;

	public LeaderboardResult(string name, ulong userId, long rating, ulong leaderboardRank, bool isPlayer, bool elite, int faction, int wave, int rank, int xp, bool veteran)
	{
		Name = name;
		UserID = userId;
		Rating = rating;
		LeaderboardRank = leaderboardRank;
		IsPlayer = isPlayer;
		Elite = elite;
		Faction = faction;
		Wave = wave;
		Rank = rank;
		XP = xp;
		Veteran = veteran;
	}
}
