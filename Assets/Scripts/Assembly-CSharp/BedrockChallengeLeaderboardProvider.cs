using System.Collections.Generic;

public class BedrockChallengeLeaderboardProvider : ChallengeLeaderboardProvider
{
	public const Bedrock.brLobbyServerTier LeaderboardTier = Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_TITLE;

	private const int MaxLeaderboardResults = 20;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(BedrockChallengeLeaderboardProvider), LogLevel.Warning);

	public override BedrockTask StartGetFriendsAboveAndBelow(uint leaderboardId)
	{
		_log.LogDebug("StartGetFriendsAboveAndBelow for {0}", leaderboardId);
		_log.LogDebug("Getting list of friends");
		Bedrock.brFriendInfo[] array = new Bedrock.brFriendInfo[100];
		uint numFriends = 0u;
		if (!Bedrock.GetFriendsWithCurrentGame(array, ref numFriends))
		{
			_log.LogError("Failed to get cached list of friends.");
			return null;
		}
		_log.LogDebug("Generating request array from {0} friends.", numFriends);
		ulong[] array2 = new ulong[numFriends + 1];
		for (int i = 0; i < numFriends; i++)
		{
			array2[i] = array[i]._userId;
		}
		ulong userId = ChallengeManager.UserId;
		array2[numFriends] = userId;
		if (_log.OutputLevel == LogLevel.Debug)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				_log.LogDebug(" Row [{0}]={1}", j, array2[j]);
			}
		}
		short taskHandle = Bedrock.StartReadLeaderboardByUserIds(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_TITLE, leaderboardId, array2, (uint)array2.Length, false, true);
		BedrockTask bedrockTask = new BedrockTask(taskHandle);
		bedrockTask.Tag = array2.Length;
		return bedrockTask;
	}

	protected override ChallengeLeaderboardRow[] EndGetFriendsAboveAndBelow_Internal(BedrockTask task, ChallengeLeaderboardType leaderboardType, uint leaderboardId)
	{
		_log.LogDebug("EndGetFriendsAboveAndBelow_Internal for {0}", task);
		int num = (int)task.Tag;
		Bedrock.brLeaderboardRow[] array = new Bedrock.brLeaderboardRow[num];
		_log.LogDebug("Calling bedrock method with {0} rows of buffer space.", num);
		if (Bedrock.GetLeaderboardResults(task.Handle, array, (uint)array.Length, false))
		{
			_log.LogDebug("Got {0} results from bedrock OK.", array.Length);
			return ConvertValidBedrockRows(array, leaderboardType);
		}
		_log.LogWarning("Failed 'EndGetFriendsAboveAndBelow' call - " + task);
		return null;
	}

	private static ChallengeLeaderboardRow[] ConvertValidBedrockRows(Bedrock.brLeaderboardRow[] bedrockRows, ChallengeLeaderboardType leaderboardType)
	{
		List<ChallengeLeaderboardRow> list = new List<ChallengeLeaderboardRow>();
		foreach (Bedrock.brLeaderboardRow bedrockRow in bedrockRows)
		{
			ChallengeLeaderboardRow challengeLeaderboardRow = ChallengeLeaderboardRow.BuildFromBedrock(bedrockRow);
			if (challengeLeaderboardRow != null)
			{
				list.Add(challengeLeaderboardRow);
			}
		}
		if (leaderboardType == ChallengeLeaderboardType.MinOnTop)
		{
			list.Reverse();
		}
		_log.LogDebug("Finished converting {0} bedrock leaderboard rows. {1} were valid, {2} were not.", bedrockRows.Length, list.Count, bedrockRows.Length - list.Count);
		return list.ToArray();
	}

	public override BedrockTask SubmitScore(uint leaderboardId, ChallengeLeaderboardWriteTypes writeType, long score)
	{
		_log.LogDebug("Submitting score {0} to leaderboard {1}, with write type {2} ", score, leaderboardId, writeType);
		ulong userId = ChallengeManager.UserId;
		int num = StatsHelper.PlayerXP();
		int level = -1;
		int prestigeLevel = -1;
		int xpToNextLevel = 0;
		float percent = 0f;
		XPManager.Instance.ConvertXPToLevel(num, out level, out prestigeLevel, out xpToNextLevel, out percent);
		Bedrock.brLeaderboardRow brLeaderboardRow = default(Bedrock.brLeaderboardRow);
		brLeaderboardRow._userId = userId;
		brLeaderboardRow._writeType = writeType.ConvertToBedrockType();
		brLeaderboardRow._leaderboardId = leaderboardId;
		brLeaderboardRow._rating = score;
		brLeaderboardRow._integerFields = new int[5];
		brLeaderboardRow._floatFields = new float[5];
		brLeaderboardRow._integerFields[0] = (SecureStorage.Instance.EliteAccountLinked ? 1 : 0);
		brLeaderboardRow._integerFields[1] = 0;
		brLeaderboardRow._integerFields[2] = XPManager.Instance.ConvertXPLevelToAbsolute(level, prestigeLevel) + 1;
		brLeaderboardRow._integerFields[3] = num;
		brLeaderboardRow._integerFields[4] = 0;
		brLeaderboardRow._floatFields[0] = (brLeaderboardRow._floatFields[1] = (brLeaderboardRow._floatFields[2] = (brLeaderboardRow._floatFields[3] = (brLeaderboardRow._floatFields[4] = 0f))));
		short taskHandle = Bedrock.StartWriteToLeaderboardRequest(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_TITLE, new Bedrock.brLeaderboardRow[1] { brLeaderboardRow });
		return new BedrockTask(taskHandle);
	}

	public override bool GetFriendsForCurrentPlayer(out FriendData[] results)
	{
		_log.LogDebug("GetFriendsForCurrentPlayer");
		Bedrock.brFriendInfo[] array = new Bedrock.brFriendInfo[100];
		uint numFriends = 0u;
		if (!Bedrock.GetFriendsWithCurrentGame(array, ref numFriends))
		{
			_log.LogWarning("Failed to get cached list of friends.");
			results = new FriendData[0];
			return false;
		}
		_log.LogDebug("Processing {0} results.", numFriends);
		FriendData[] array2 = new FriendData[numFriends];
		for (int i = 0; i < numFriends; i++)
		{
			Bedrock.brFriendInfo brFriendInfo = array[i];
			string userName = Bedrock.DecodeText(brFriendInfo._displayName);
			FriendData friendData = (array2[i] = new FriendData(brFriendInfo._userId, userName));
			_log.LogDebug("Converting {0} into {1}", string.Concat("[", brFriendInfo._userId, ", ", brFriendInfo._displayName, "]"), friendData);
		}
		_log.LogDebug("Done processing.");
		results = array2;
		return true;
	}
}
