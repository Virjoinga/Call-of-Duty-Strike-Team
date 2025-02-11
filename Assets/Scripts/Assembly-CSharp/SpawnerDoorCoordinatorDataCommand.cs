using System.Collections;
using UnityEngine;

public class SpawnerDoorCoordinatorDataCommand : Command
{
	public GameObject Target;

	public bool AutoActivateAfterModify = true;

	public SpawnerCoordinatorData data;

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
		SpawnerCoordinator sd = Target.GetComponentInChildren<SpawnerCoordinator>();
		sd.m_Interface = data;
		sd.Start();
		if (AutoActivateAfterModify)
		{
			sd.Activate();
		}
		yield break;
	}
}
