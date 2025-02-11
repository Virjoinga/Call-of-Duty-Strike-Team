using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeScoreSubmissionManager : MonoBehaviour
{
	private class ChallengeFinishedEntry
	{
		public uint ChallengeId { get; private set; }

		public long LeaderboardValue { get; private set; }

		public uint ChallengeCycle { get; private set; }

		public uint SubmissionAttempts { get; set; }

		public ChallengeFinishedEntry(uint challengeId, uint challengeCycle, long leaderboardValue)
		{
			ChallengeId = challengeId;
			ChallengeCycle = challengeCycle;
			LeaderboardValue = leaderboardValue;
		}

		public override string ToString()
		{
			return string.Format("[ChallengeFinishedEntry: ChallengeId={0},ChallengeCycle={1},FinalProgress={2},SubmissionAttempts={3}]", ChallengeId, ChallengeCycle, LeaderboardValue, SubmissionAttempts);
		}
	}

	private const float TimeToWaitBetweenSubmissionAttempts = 1f;

	private const int MaxRetries = 60;

	private const bool ShowChallengeDebugUiElements = true;

	private const uint ChallengeSubmitGracePeriod = 15u;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeScoreSubmissionManager), LogLevel.None);

	private List<ChallengeFinishedEntry> _pendingSubmissions = new List<ChallengeFinishedEntry>();

	private bool _isSubmittingPendingChallenges;

	private void Start()
	{
		StartCoroutine(Timer());
	}

	private IEnumerator Timer()
	{
		while (true)
		{
			yield return StartCoroutine(SubmitPendingChallenges());
			yield return StartCoroutine(CoroutineUtils.WaitForWallTime(1f));
		}
	}

	public void SubmitCurrentScoreForChallenge(Challenge challenge)
	{
		_log.LogDebug("Starting submit of progress for challenge {0}.", challenge.ChallengeId);
		ChallengeData challengeData = challenge.ChallengeData;
		if (!challenge.MadeProgress)
		{
			_log.LogDebug(string.Concat("No progress was made in challenge ", challenge, " - no submission being made."));
			return;
		}
		if (!challenge.BeatBestScoreSubmittedSinceJoin)
		{
			_log.Log("Score '{0}' did not beat previous best score '{1}' for challenge '{2}'. Not submitting.", challenge.LeaderboardValue, challenge.ChallengeData.BestScoreSubmittedThisCycle, challengeData.Name);
			return;
		}
		uint cycle = challenge.ChallengeData.GetCycle(SynchronizedClock.Instance.SynchronizedTimeOrBestGuess);
		ChallengeFinishedEntry challengeFinishedEntry = new ChallengeFinishedEntry(challenge.ChallengeId, cycle, challenge.LeaderboardValue);
		ChallengeFinishedEntry challengeFinishedEntry2 = null;
		foreach (ChallengeFinishedEntry pendingSubmission in _pendingSubmissions)
		{
			if (pendingSubmission.ChallengeId == challengeFinishedEntry.ChallengeId)
			{
				challengeFinishedEntry2 = pendingSubmission;
				break;
			}
		}
		if (challengeFinishedEntry2 != null)
		{
			_log.LogWarning("Already have a pending submission for challenge {0}.", challengeData.Id);
		}
		_log.LogDebug("Queuing challenge finish entry for " + challengeFinishedEntry);
		_pendingSubmissions.Add(challengeFinishedEntry);
		SavePendingSubmissions();
	}

	private IEnumerator SubmitPendingChallenges()
	{
		_log.LogDebug("Checking for pending challenges...");
		if (_isSubmittingPendingChallenges)
		{
			_log.LogDebug("Re-entrant call. Aborting.");
			yield break;
		}
		Bedrock.brUserConnectionStatus connectionStatus = ChallengeManager.ConnectionStatus;
		if (!connectionStatus.IsOnline())
		{
			_log.LogDebug("User is not online. Skipping pending submit.");
			yield break;
		}
		_isSubmittingPendingChallenges = true;
		if (_pendingSubmissions.Count == 0)
		{
			_log.LogDebug("No pending submissions. Aborting.");
			_isSubmittingPendingChallenges = false;
			yield break;
		}
		_log.LogDebug("Getting time from server...");
		ChallengeDataProvider dataProvider = ChallengeManager.Instance.DataProvider;
		ChallengeLeaderboardProvider leaderboardProvider = ChallengeManager.Instance.LeaderboardProvider;
		BedrockTask timerTask = dataProvider.BeginGetServerTime();
		yield return StartCoroutine(timerTask.WaitForTaskToCompleteOrTimeoutCoroutine());
		uint serverTime;
		if (dataProvider.EndGetServerTime(timerTask, out serverTime))
		{
			_log.LogDebug("Server time obtained!");
			ChallengeFinishedEntry[] submissions = _pendingSubmissions.ToArray();
			_pendingSubmissions.Clear();
			BedrockTask[] submissionTasks = new BedrockTask[submissions.Length];
			for (int i = 0; i < submissions.Length; i++)
			{
				ChallengeFinishedEntry candidate = submissions[i];
				ChallengeData challengeData2 = dataProvider.GetChallengeData(candidate.ChallengeId);
				uint currentCycle = challengeData2.GetCycle(serverTime);
				if (currentCycle != candidate.ChallengeCycle)
				{
					_log.LogError("Pending score {0} was not from current cycle ({1}). Forgetting about it. Not informing user because it might be from loaded data.", candidate, currentCycle);
				}
				uint gracePeriodStartTime = serverTime - 15;
				ChallengeStatus statusAtStartOfGracePeriod = challengeData2.GetStatusAtTime(gracePeriodStartTime);
				if (challengeData2.Status == ChallengeStatus.Open || statusAtStartOfGracePeriod == ChallengeStatus.Open)
				{
					_log.Log("Debug - Submitting Score (" + candidate.LeaderboardValue + ")");
					BedrockTask task = leaderboardProvider.SubmitScore(challengeData2.LeaderboardId, challengeData2.WriteType, candidate.LeaderboardValue);
					submissionTasks[i] = task;
					candidate.SubmissionAttempts++;
				}
				else
				{
					_log.LogError(string.Format("Failed to submit score, ChallengeStatus = {0}, StatusAtGracePeriodStart = {1}, CurrentTime = {2}, GraceTime = {3}", challengeData2.Status, statusAtStartOfGracePeriod, serverTime, gracePeriodStartTime));
				}
			}
			yield return StartCoroutine(BedrockTask.WaitForAllTasksToCompleteOrTimeoutCoroutine(submissionTasks));
			for (int j = 0; j < submissions.Length; j++)
			{
				ChallengeFinishedEntry submission = submissions[j];
				BedrockTask task2 = submissionTasks[j];
				if (task2 == null)
				{
					continue;
				}
				if (task2.Status != Bedrock.brTaskStatus.BR_TASK_SUCCESS)
				{
					_log.LogError("Failed to submit score {0}: {1}", submission, task2);
					if (submission.SubmissionAttempts >= 60)
					{
						NotificationPanel.Instance.Display(Language.Get("S_GMG_CHALLENGE_SUBMIT_FAILED_MAX_RETRIES"));
						_log.LogError("Pending score {0} failed to submit after {1} tries. Forgetting about it.", submission.LeaderboardValue, submission.SubmissionAttempts);
					}
					else
					{
						_log.Log("Debug - Submit operation failed. Will try again.");
						_pendingSubmissions.Add(submission);
					}
				}
				else
				{
					ChallengeData challengeData = dataProvider.GetChallengeData(submission.ChallengeId);
					_log.Log("Submitted score: {0} for challenge {1}", submission.LeaderboardValue, challengeData);
					SecureStorage.Instance.SetBestScoreSubmittedThisCycle(challengeData, submission.LeaderboardValue);
					SecureStorage.Instance.SetBestScoreSubmittedThisCycleTime(challengeData, serverTime);
				}
				task2.Dispose();
			}
			SavePendingSubmissions();
		}
		else
		{
			_log.LogError("Failed to get time from server. Cannot submit scores for {0} challenges. ({1})", _pendingSubmissions.Count, timerTask);
		}
		_log.LogDebug("Finished checking for scores.");
		_isSubmittingPendingChallenges = false;
	}

	public void HandleNoLongerInValidActivateState()
	{
		_log.LogDebug("Stopping submission coroutine...");
		StopAllCoroutines();
		_log.LogDebug("Clearing pending queue.");
		_pendingSubmissions.Clear();
		_log.LogDebug("Restarting submission coroutine.");
		StartCoroutine(Timer());
	}

	public void LoadPendingSubmissions(uint currentTime)
	{
		_log.LogDebug("LoadPendingSubmissions({0})", currentTime);
		_pendingSubmissions.Clear();
		string challengeScoreSubmissionQueueState = SecureStorage.Instance.ChallengeScoreSubmissionQueueState;
		if (string.IsNullOrEmpty(challengeScoreSubmissionQueueState))
		{
			_log.Log("No previous pending submissions found.");
			return;
		}
		_log.LogDebug("Loading pending submission from '{0}'", challengeScoreSubmissionQueueState);
		string[] array = challengeScoreSubmissionQueueState.Split(',');
		_log.LogDebug("Split submission string into '{0}' elements.", array.Length);
		string[] array2 = array;
		foreach (string text in array2)
		{
			_log.LogDebug("Decoding entry '{0}'", text);
			string[] array3 = text.Split('.');
			if (array3.Length != 3)
			{
				_log.LogError("Unable to split '{0}' into correct number of items (split into {1} pieces). Skipping.", text, array3.Length);
				continue;
			}
			uint result;
			if (!uint.TryParse(array3[0], out result))
			{
				_log.LogError("Unable to parse '{0}' into valid challenge ID. Skipping.", array3[0]);
				continue;
			}
			uint result2;
			if (!uint.TryParse(array3[1], out result2))
			{
				_log.LogError("Unable to parse '{0}' into valid cycle number. Skipping.", array3[1]);
				continue;
			}
			long result3;
			if (!long.TryParse(array3[2], out result3))
			{
				_log.LogError("Unable to parse '{0}' into valid leaderboard value. Skipping.", array3[2]);
				continue;
			}
			ChallengeFinishedEntry challengeFinishedEntry = new ChallengeFinishedEntry(result, result2, result3);
			ChallengeData challengeData = ChallengeManager.Instance.DataProvider.GetChallengeData(result);
			uint cycle = challengeData.GetCycle(currentTime);
			if (cycle != result2)
			{
				_log.LogWarning("Challenge submission entry '{0}' was not from current cycle ({1}). Skipping.", challengeFinishedEntry, cycle);
			}
			else
			{
				_log.LogDebug("Re-Enqueued pending challenge score submission: {0}", challengeFinishedEntry);
				_pendingSubmissions.Add(challengeFinishedEntry);
			}
		}
	}

	private void SavePendingSubmissions()
	{
		_log.LogDebug("Saving pending submissions...");
		string[] array = new string[_pendingSubmissions.Count];
		for (int i = 0; i < _pendingSubmissions.Count; i++)
		{
			ChallengeFinishedEntry challengeFinishedEntry = _pendingSubmissions[i];
			array[i] = challengeFinishedEntry.ChallengeId + "." + challengeFinishedEntry.ChallengeCycle + "." + challengeFinishedEntry.LeaderboardValue;
		}
		string text = string.Join(",", array);
		_log.LogDebug("Generated save string: " + text);
		SecureStorage.Instance.ChallengeScoreSubmissionQueueState = text;
	}
}
