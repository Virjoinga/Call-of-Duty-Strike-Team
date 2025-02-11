using System;
using System.Collections.Generic;
using System.Linq;

public class MockChallengeDataProvider : ChallengeDataProvider
{
	public const string SourceDataResourceName = "PlaceholderChallenges";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(MockChallengeDataProvider), LogLevel.Debug);

	private ChallengeData[] _challengeData;

	protected override uint NumberOfChallenges_Internal
	{
		get
		{
			return (uint)_challengeData.Length;
		}
	}

	private void Awake()
	{
		List<List<string>> source = CsvUtilities.LoadCsvDataFromResource("PlaceholderChallenges");
		_challengeData = (from csvLine in source.Skip(1)
			select ChallengeData.BuildFromCsvLine(csvLine)).ToArray();
	}

	protected override ChallengeData GetChallengeData_Internal(uint challengeId)
	{
		if (challengeId >= _challengeData.Length || challengeId < 0)
		{
			return null;
		}
		return _challengeData[challengeId];
	}

	public override BedrockTask BeginUpdateAllChallenges()
	{
		return new MockBedrockTask();
	}

	public override bool EndUpdateAllChallenges(BedrockTask task)
	{
		if (task.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS)
		{
			uint secondsSinceUnixEpoch = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
			ChallengeData[] challengeData = _challengeData;
			foreach (ChallengeData challengeData2 in challengeData)
			{
				ChallengeStatus statusAtTime = challengeData2.GetStatusAtTime(secondsSinceUnixEpoch);
				challengeData2.Status = statusAtTime;
			}
			return true;
		}
		return false;
	}

	public override BedrockTask BeginGetServerTime()
	{
		MockBedrockTask mockBedrockTask = new MockBedrockTask();
		mockBedrockTask.Result = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
		return mockBedrockTask;
	}

	public override bool EndGetServerTime(BedrockTask task, out uint currentTime)
	{
		MockBedrockTask mockBedrockTask = (MockBedrockTask)task;
		currentTime = (uint)mockBedrockTask.Result;
		return mockBedrockTask.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS;
	}

	public override BedrockTask BeginGetChallengeStatus(uint challengeId)
	{
		_log.LogDebug("Getting status for challenge {0}", challengeId);
		MockBedrockTask mockBedrockTask = new MockBedrockTask();
		uint secondsSinceUnixEpoch = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
		mockBedrockTask.Result = GetChallengeData(challengeId).GetStatusAtTime(secondsSinceUnixEpoch);
		_log.LogDebug("Status for challenge {0} at time {1} is {2}", challengeId, secondsSinceUnixEpoch, mockBedrockTask.Result);
		return mockBedrockTask;
	}

	public override bool EndGetChallengeStatus(BedrockTask task, out ChallengeStatus status)
	{
		MockBedrockTask mockBedrockTask = (MockBedrockTask)task;
		status = (ChallengeStatus)(int)mockBedrockTask.Result;
		return mockBedrockTask.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS;
	}
}
