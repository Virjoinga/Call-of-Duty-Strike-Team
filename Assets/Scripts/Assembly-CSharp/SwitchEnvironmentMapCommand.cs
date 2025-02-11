using System.Collections;
using UnityEngine;

public class SwitchEnvironmentMapCommand : Command
{
	public Texture EnvironmentMap;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (MissionSetup.Instance != null)
		{
			MissionSetup.Instance.ReflectionMapOverride = EnvironmentMap;
		}
		yield break;
	}
}
