using System;

[Serializable]
public class SummonerData
{
	public GuidRef mCoverCluster = new GuidRef();

	public GuidRef mTaskObject = new GuidRef();

	public FactionHelper.Category mFaction = FactionHelper.Category.Enemy;

	public BehaviourController.AlertState mMinimumAlertnessToQualify = BehaviourController.AlertState.Reacting;

	public BehaviourController.AlertState mMinimumAlertnessToSummon = BehaviourController.AlertState.Alerted;

	public float mMinDelayBetweenAttempts = 10f;

	public float mMaxDelayBetweenAttempts = 20f;

	public bool mRejectCandidateIfEnemiesVisible;

	public bool mRejectCandidateIfTargeted;

	public bool mRejectCandidateIfPlayerCloserToTarget = true;

	public void ResolveGuidLinks()
	{
		mCoverCluster.ResolveLink();
	}
}
