using System.Collections;
using UnityEngine;

public class KillCommand : Command
{
	public GameObject Killer;

	public HealthComponent Target;

	public ActorWrapper TargetWrapper;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (Target != null)
		{
			Target.ModifyHealth(Killer, float.MinValue, "ActOfGod", Vector3.zero, false);
		}
		else if (TargetWrapper != null && TargetWrapper.GetActor() != null)
		{
			TargetWrapper.GetActor().baseCharacter.Kill("Script");
		}
		yield break;
	}
}
