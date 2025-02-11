using System.Collections;
using UnityEngine;

public class ForceCharacterOnScreen : Command
{
	public bool ForceOnScreen = true;

	public GuidRef Target;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (Target == null)
		{
			yield break;
		}
		GameObject spawnerObj = Target.theObject;
		if (!(spawnerObj != null))
		{
			yield break;
		}
		Spawner spawner = IncludeDisabled.GetComponentInChildren<Spawner>(spawnerObj);
		if (!(spawner != null))
		{
			yield break;
		}
		GameObject spawned = spawner.spawned;
		if (spawned != null)
		{
			Actor act = IncludeDisabled.GetComponentInChildren<Actor>(spawned);
			if (act != null && act.realCharacter != null)
			{
				act.realCharacter.ForceOnScreen = ForceOnScreen;
			}
		}
	}

	public override void ResolveGuidLinks()
	{
		if (Target != null)
		{
			Target.ResolveLink();
		}
	}
}
