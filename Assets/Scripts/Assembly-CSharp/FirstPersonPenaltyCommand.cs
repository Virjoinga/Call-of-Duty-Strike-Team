using System.Collections;

public class FirstPersonPenaltyCommand : Command
{
	public bool enablePenalty;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		FirstPersonPenaliser.EnablePenalty(enablePenalty);
		yield break;
	}
}
