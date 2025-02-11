using System.Collections;
using UnityEngine;

public class SpawnerDoorDataCommand : Command
{
	public GameObject Target;

	public bool AutoActivateAfterModify = true;

	public SpawnerDoorData data;

	public override bool Blocking()
	{
		return false;
	}

	public override void ResolveGuidLinks()
	{
		data.ResolveGuidLinks();
	}

	public override IEnumerator Execute()
	{
		EnemyFromDoorOverride efdo = Target.GetComponentInChildren<EnemyFromDoorOverride>();
		efdo.m_OverrideData = data;
		Container cont = Target.GetComponentInChildren<Container>();
		efdo.ApplyOverride(cont);
		SpawnerDoor sd = Target.GetComponentInChildren<SpawnerDoor>();
		sd.Start();
		if (AutoActivateAfterModify)
		{
			sd.Activate();
		}
		yield break;
	}
}
