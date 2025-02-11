using UnityEngine;

public class Summoner : MonoBehaviour
{
	private enum State
	{
		Searching = 0,
		DelayedReaction = 1
	}

	public SummonerData mInterface;

	private uint mRoundRobin;

	private Actor mBestCandidate;

	private float mBestCandidateDistance = float.MaxValue;

	private TaskDescriptor mSummonTask;

	private CoverCluster mCachedCluster;

	private CoverPointCore mClosestCoverPoint;

	private float mNextSummonAttempt;

	private TaskSummoned mRespondentTask;

	private State mState;

	private void Start()
	{
		mRoundRobin = 1u;
		mBestCandidate = null;
		mBestCandidateDistance = float.MaxValue;
		mState = State.Searching;
		mCachedCluster = null;
		if (mInterface.mCoverCluster.theObject != null)
		{
			mCachedCluster = mInterface.mCoverCluster.theObject.GetComponent<CoverCluster>();
		}
		TBFAssert.DoAssert(mCachedCluster != null, "Summoner " + base.name + " has no cover cluster assigned.");
		mClosestCoverPoint = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(base.transform.position);
		TBFAssert.DoAssert(mClosestCoverPoint != null, "Summoner " + base.name + " cannot find a nearby cover point to gauge distance.");
		mSummonTask = null;
		if (mInterface.mTaskObject.theObject != null)
		{
			mSummonTask = mInterface.mTaskObject.GetComponent<TaskDescriptor>();
		}
		TBFAssert.DoAssert(mSummonTask != null, "Summoner " + base.name + " has no task object.");
		mNextSummonAttempt = Time.time;
	}

	private void Update()
	{
		if (mRespondentTask == null && !(Time.time < mNextSummonAttempt))
		{
			BruteForceUpdate();
		}
	}

	private void BruteForceUpdate()
	{
		uint m = GKM.FactionMask(mInterface.mFaction) & GKM.CharacterTypeMask(CharacterType.Human) & GKM.AliveMask;
		mBestCandidate = null;
		mBestCandidateDistance = float.MaxValue;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(m);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			CompareSummonCandidate(a);
		}
		if (mBestCandidate != null)
		{
			if (mState == State.Searching)
			{
				if (mBestCandidate.behaviour.alertState >= mInterface.mMinimumAlertnessToSummon)
				{
					mState = State.DelayedReaction;
				}
			}
			else if (mState == State.DelayedReaction)
			{
				if (mBestCandidate.behaviour.alertState >= mInterface.mMinimumAlertnessToSummon)
				{
					Summon();
				}
				mState = State.Searching;
			}
		}
		else
		{
			mState = State.Searching;
		}
	}

	private void RoundRobinUpdate()
	{
		Actor actor = GKM.GetActor(mRoundRobin);
		if (BasicEligibility(actor))
		{
			CompareSummonCandidate(actor);
		}
		mRoundRobin <<= 1;
		if (mRoundRobin != 0)
		{
			return;
		}
		if (mBestCandidate != null)
		{
			actor = mBestCandidate;
			mBestCandidateDistance = float.MaxValue;
			mRoundRobin = mBestCandidate.ident;
			mBestCandidate = null;
			CompareSummonCandidate(actor);
			if (mBestCandidate != null)
			{
				Summon();
			}
		}
		mRoundRobin = 1u;
		mBestCandidate = null;
		mBestCandidateDistance = float.MaxValue;
	}

	private bool BasicEligibility(Actor candidate)
	{
		if (candidate == null || candidate.behaviour == null)
		{
			return false;
		}
		if (candidate.awareness.ChDefCharacterType != CharacterType.Human)
		{
			return false;
		}
		if (candidate.awareness.faction != mInterface.mFaction)
		{
			return false;
		}
		return true;
	}

	private void CompareSummonCandidate(Actor candidate)
	{
		if (!(candidate.awareness.closestCoverPoint == null) && candidate.behaviour.alertState >= mInterface.mMinimumAlertnessToQualify && mCachedCluster.Includes(candidate.awareness.closestCoverPoint))
		{
			float coverToCoverDistance = NewCoverPointManager.Instance().GetCoverToCoverDistance(mClosestCoverPoint, candidate.awareness.closestCoverPoint);
			if (!(coverToCoverDistance > mBestCandidateDistance) && (!mInterface.mRejectCandidateIfEnemiesVisible || candidate.awareness.EnemiesICanSee == 0) && (!mInterface.mRejectCandidateIfTargeted || !CheckSummonCandidate_EnemiesTargeting(candidate)) && (!mInterface.mRejectCandidateIfPlayerCloserToTarget || !CheckSummonCandidate_EnemiesCloserToTarget(candidate, coverToCoverDistance)))
			{
				mBestCandidateDistance = coverToCoverDistance;
				mBestCandidate = candidate;
			}
		}
	}

	private bool CheckSummonCandidate_EnemiesTargeting(Actor candidate)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(candidate.awareness.EnemiesIKnowAboutRecent);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.weapon != null && a.weapon.GetTarget() == candidate)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckSummonCandidate_EnemiesCloserToTarget(Actor candidate, float candidateDistance)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(candidate.awareness.EnemiesIKnowAboutRecent);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			float coverToCoverDistance = NewCoverPointManager.Instance().GetCoverToCoverDistance(a.awareness.closestCoverPoint, mClosestCoverPoint);
			if (coverToCoverDistance < candidateDistance)
			{
				return true;
			}
		}
		return false;
	}

	private void Summon()
	{
		mRespondentTask = new TaskSummoned(mBestCandidate.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, this);
		mSummonTask.CreateTask(mBestCandidate.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default);
	}

	public void Watch(TaskManager tasks, TaskManager.Priority priority)
	{
		mRespondentTask = new TaskSummoned(mBestCandidate.tasks, priority, Task.Config.Default, this);
	}

	public void SummonTaskFinished(TaskSummoned t)
	{
		if (t == mRespondentTask)
		{
			mRespondentTask = null;
			mNextSummonAttempt = Time.time + Random.Range(mInterface.mMinDelayBetweenAttempts, mInterface.mMaxDelayBetweenAttempts);
		}
	}
}
