public class LeaderboardEntry
{
	public uint LeaderboardNum;

	public int Rating;

	public bool Veteran;

	public int Wave;

	public LeaderboardEntry(uint leaderboardNum, int rating, bool veteran, int wave)
	{
		LeaderboardNum = leaderboardNum;
		Rating = rating;
		Veteran = veteran;
		Wave = wave;
	}
}
