using System.Collections;
using UnityEngine;

public class SwitchTargetCommand : Command
{
	public GuidRef Shooter;

	public GameObject Target;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (Shooter == null)
		{
			yield break;
		}
		GameObject shooterObj = Shooter.theObject;
		if (shooterObj != null)
		{
			TargetSwitchComponent tsc = shooterObj.GetComponentInChildren<TargetSwitchComponent>();
			if (tsc != null)
			{
				tsc.CurrentTarget = Target;
			}
		}
	}
}
