using System.Collections.Generic;

public class ChallengeLeaderboardRow
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeLeaderboardRow), LogLevel.Warning);

	public ulong UserId { get; private set; }

	public string UserName { get; private set; }

	public long Score { get; private set; }

	public ulong Rank { get; set; }

	public int XPRank { get; set; }

	public bool Elite { get; set; }

	public ChallengeLeaderboardRow(ulong userId, string userName, long score, int xpRank, bool elite)
	{
		UserId = userId;
		UserName = userName;
		Score = score;
		XPRank = xpRank;
		Elite = elite;
	}

	public ChallengeLeaderboardRow(List<string> csvRow)
	{
		int num = 1;
		_log.LogDebug("Loading leaderboard data from row: " + string.Join(", ", csvRow.ToArray()));
		Score = long.Parse(csvRow[num++]);
		UserId = ulong.Parse(csvRow[num++]);
		UserName = csvRow[num++];
		Rank = 0uL;
		XPRank = 0;
		Elite = false;
	}

	public void SetScore(long score)
	{
		Score = score;
	}

	public override string ToString()
	{
		return string.Format("[ChallengeLeaderboardRow: UserId={0}, UserName={1}, Score={2}, Rank={3} CODRank={4} Elite={5}]", UserId, UserName, Score, Rank, XPRank, Elite);
	}

	public static ChallengeLeaderboardRow BuildFromBedrock(Bedrock.brLeaderboardRow bedrockRow)
	{
		if (!bedrockRow.isValid())
		{
			return null;
		}
		if (_log.OutputLevel == LogLevel.Debug)
		{
			bedrockRow.DebugPrint();
		}
		bool elite = bedrockRow._integerFields[0] == 1;
		ChallengeLeaderboardRow challengeLeaderboardRow = new ChallengeLeaderboardRow(bedrockRow._userId, bedrockRow.getEntityName(), bedrockRow._rating, bedrockRow._integerFields[2], elite);
		challengeLeaderboardRow.Rank = bedrockRow._rank;
		return challengeLeaderboardRow;
	}
}
