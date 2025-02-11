using System.Collections;

public class SetMissionObjectiveStateCommand : Command
{
	public MissionObjective Objective;

	public MissionObjective.ObjectiveState State;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		switch (State)
		{
		case MissionObjective.ObjectiveState.Passed:
			Objective.Pass();
			break;
		case MissionObjective.ObjectiveState.Failed:
			Objective.Fail();
			break;
		default:
			TBFAssert.DoAssert(false, "unsupported state in this command - ask a coder");
			break;
		}
		yield break;
	}
}
