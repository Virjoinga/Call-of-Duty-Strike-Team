using UnityEngine;

public abstract class ChallengeLeaderboardProvider : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeLeaderboardProvider), LogLevel.Warning);

	public abstract BedrockTask SubmitScore(uint leaderboardId, ChallengeLeaderboardWriteTypes writeType, long score);

	public abstract BedrockTask StartGetFriendsAboveAndBelow(uint leaderboardId);

	protected abstract ChallengeLeaderboardRow[] EndGetFriendsAboveAndBelow_Internal(BedrockTask task, ChallengeLeaderboardType leaderboardType, uint leaderboardId);

	public abstract bool GetFriendsForCurrentPlayer(out FriendData[] friends);

	public bool EndGetFriendsAboveAndBelow(BedrockTask task, out ChallengeFriendData results, ChallengeLeaderboardType leaderboardType, uint leaderboardId)
	{
		_log.LogDebug("EndGetFriendsAboveAndBelow(...)");
		ChallengeLeaderboardRow[] array = null;
		if (task != null)
		{
			array = EndGetFriendsAboveAndBelow_Internal(task, leaderboardType, leaderboardId);
		}
		if (array == null)
		{
			results = null;
			return false;
		}
		uint friendsAbove;
		uint friendsBelow;
		uint totalFriends;
		ProcessLeaderboardResults(array, leaderboardType, out friendsAbove, out friendsBelow, out totalFriends);
		_log.LogDebug("Done processing {0} rows.", array.Length);
		results = new ChallengeFriendData(friendsAbove, friendsBelow, totalFriends, array, leaderboardId);
		return true;
	}

	protected void ProcessLeaderboardResults(ChallengeLeaderboardRow[] orderedRows, ChallengeLeaderboardType leaderboardType, out uint friendsAbove, out uint friendsBelow, out uint totalFriends)
	{
		friendsAbove = 0u;
		friendsBelow = 0u;
		totalFriends = 0u;
		if (orderedRows.Length == 0)
		{
			_log.LogDebug("No results were valid. No more processing required.");
			return;
		}
		ulong userId = ChallengeManager.UserId;
		long? num = null;
		foreach (ChallengeLeaderboardRow challengeLeaderboardRow in orderedRows)
		{
			if (challengeLeaderboardRow.UserId == userId)
			{
				_log.LogDebug("Found Score {0} for current player ({1})", challengeLeaderboardRow.Score, challengeLeaderboardRow.UserId);
				num = challengeLeaderboardRow.Score;
				break;
			}
		}
		for (int j = 0; j < orderedRows.Length; j++)
		{
			ChallengeLeaderboardRow challengeLeaderboardRow2 = orderedRows[j];
			challengeLeaderboardRow2.Rank = (ulong)(j + 1);
			if (num.HasValue)
			{
				switch (ChallengeData.CompareScores(challengeLeaderboardRow2.Score, num.Value, leaderboardType))
				{
				case ChallengeData.ScoreComparisonResult.FirstIsBetter:
					_log.LogDebug("Score {0} for player {1} was BETTER than score {2} for current player.", challengeLeaderboardRow2.Score, challengeLeaderboardRow2.UserId, num.Value);
					friendsAbove++;
					break;
				case ChallengeData.ScoreComparisonResult.SecondIsBetter:
					_log.LogDebug("Score {0} for player {1} was WORSE than score {2} for current player.", challengeLeaderboardRow2.Score, challengeLeaderboardRow2.UserId, num.Value);
					friendsBelow++;
					break;
				}
			}
		}
		totalFriends = (uint)((!num.HasValue) ? orderedRows.Length : (orderedRows.Length - 1));
	}
}
