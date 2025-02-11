using System.Collections;
using UnityEngine;

public class TapRestrictionCommand : Command
{
	public GameObject target;

	public float radius = 1f;

	public float snapModifier;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TutorialToggles.TapRestrictionObject = target;
		TutorialToggles.tapRestrictionRadius = radius;
		TutorialToggles.snapModifier = snapModifier;
		yield break;
	}
}
