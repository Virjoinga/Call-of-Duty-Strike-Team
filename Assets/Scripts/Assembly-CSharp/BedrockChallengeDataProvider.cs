public class BedrockChallengeDataProvider : ChallengeDataProvider
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(BedrockChallengeDataProvider), LogLevel.Warning);

	protected override uint NumberOfChallenges_Internal
	{
		get
		{
			_log.LogDebug("Getting NumberOfChallenges");
			uint numChallenges;
			if (Bedrock.GetNumChallenges(out numChallenges))
			{
				_log.LogDebug("Number of challenges - {0}", numChallenges);
				return numChallenges;
			}
			_log.LogWarning("Failed to get the number of challenges. Returning 0.");
			return numChallenges;
		}
	}

	protected override ChallengeData GetChallengeData_Internal(uint challengeId)
	{
		Bedrock.brChallengeInfo challengeInfo;
		if (!Bedrock.GetChallengeInfo(challengeId, out challengeInfo))
		{
			_log.LogError("Failed to get ChallengeInfo for challenge {0}", challengeId);
			return null;
		}
		ChallengeData challengeData = ChallengeData.BuildFromBedrockInfo(challengeInfo);
		if (challengeData == null)
		{
			_log.LogError("Failed to convert challenge data {0} from bedrock data.", challengeId);
		}
		return challengeData;
	}

	public override BedrockTask BeginGetChallengeStatus(uint challengeId)
	{
		_log.LogDebug("BeginGetChallengeStatus for challenge {0}", challengeId);
		short taskHandle = Bedrock.StartChallengeStatusRequest(challengeId);
		return new BedrockTask(taskHandle);
	}

	public override bool EndGetChallengeStatus(BedrockTask task, out ChallengeStatus status)
	{
		_log.LogDebug("EndGetChallengeStatus");
		Bedrock.brChallengeStatus status2;
		bool challengeStatusResult = Bedrock.GetChallengeStatusResult(task.Handle, out status2);
		if (challengeStatusResult)
		{
			status = status2.ConvertToChallengeStatus();
		}
		else
		{
			_log.LogWarning("Failed EndGetChallengeStatus - " + task);
			status = ChallengeStatus.Unknown;
		}
		return challengeStatusResult;
	}

	public override BedrockTask BeginGetServerTime()
	{
		_log.LogDebug("BeginGetServerTime");
		short taskHandle = Bedrock.StartServerTimeRequest(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_TITLE);
		return new BedrockTask(taskHandle);
	}

	public override bool EndGetServerTime(BedrockTask task, out uint currentTime)
	{
		_log.LogDebug("EndGetServerTime");
		return Bedrock.GetServerTimeResult(task.Handle, out currentTime);
	}

	public override BedrockTask BeginUpdateAllChallenges()
	{
		_log.LogDebug("BeginUpdateAllChallenges");
		short taskHandle = Bedrock.UpdateStatusOfAllChallenges();
		return new BedrockTask(taskHandle);
	}

	public override bool EndUpdateAllChallenges(BedrockTask task)
	{
		_log.LogDebug("EndUpdateAllChallenges");
		if (task.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS)
		{
			foreach (ChallengeData allChallenge in base.AllChallenges)
			{
				if (allChallenge == null)
				{
					_log.LogError("Cannot update status of challenge because it is NULL.");
					continue;
				}
				Bedrock.brChallengeInfo challengeInfo;
				if (Bedrock.GetChallengeInfo(allChallenge.Id, out challengeInfo))
				{
					allChallenge.Status = challengeInfo._status.ConvertToChallengeStatus();
					continue;
				}
				_log.LogError("Failed to get status for challenge {0} after EndUpdateAllChallenges: {1}", allChallenge.Id, task);
			}
		}
		else
		{
			_log.LogWarning("Failed to update all challenge info. Could not update locally cached data: {0}", task);
		}
		return task.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS;
	}
}
