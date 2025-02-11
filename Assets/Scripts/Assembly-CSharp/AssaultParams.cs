using System;

[Serializable]
public class AssaultParams
{
	public GuidRef Target = new GuidRef();

	public float OffenceModifier = 1f;

	public float DefenceModifier = 1f;

	public float ExposureModifier = 1f;

	public float GoalModifier = 1f;

	public float GoalModifier_InCombat = 1f;

	public float HoldCoverMinTime = 5f;

	public float HoldCoverMaxTime = 10f;

	public bool canUseFixedGuns = true;

	public void ResolveGuidLinks()
	{
		Target.ResolveLink();
	}

	public void CopyFrom(AssaultParams other)
	{
		if (other != null)
		{
			OffenceModifier = other.OffenceModifier;
			DefenceModifier = other.DefenceModifier;
			ExposureModifier = other.ExposureModifier;
			GoalModifier = other.GoalModifier;
			GoalModifier_InCombat = other.GoalModifier_InCombat;
			HoldCoverMinTime = other.HoldCoverMinTime;
			HoldCoverMaxTime = other.HoldCoverMaxTime;
			Target.theObject = other.Target.theObject;
			canUseFixedGuns = other.canUseFixedGuns;
		}
	}
}
