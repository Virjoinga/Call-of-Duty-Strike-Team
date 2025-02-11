public class ChallengeFriendData
{
	public ulong FriendsRankedHigher { get; private set; }

	public ulong FriendsRankedLower { get; private set; }

	public ulong TotalFriendsPlaying { get; private set; }

	public ChallengeLeaderboardRow[] Rows { get; private set; }

	public uint LeaderboardID { get; private set; }

	public ulong PlayerPosition
	{
		get
		{
			return FriendsRankedHigher + 1;
		}
	}

	public ChallengeFriendData(ulong friendsAbove, ulong friendsBelow, ulong totalFriends, ChallengeLeaderboardRow[] rows, uint leaderboardID)
	{
		FriendsRankedHigher = friendsAbove;
		FriendsRankedLower = friendsBelow;
		TotalFriendsPlaying = totalFriends;
		Rows = rows;
		LeaderboardID = leaderboardID;
	}

	public override string ToString()
	{
		return string.Format("[ChallengeFriendData: FriendsRankedHigher={0}, FriendsRankedLower={1}, TotalFriends={2}, RowCount={3}]", FriendsRankedHigher, FriendsRankedLower, TotalFriendsPlaying, Rows.Length);
	}
}
