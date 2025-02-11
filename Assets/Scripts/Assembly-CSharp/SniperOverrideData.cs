using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SniperOverrideData
{
	public List<KillZone> KillZones = new List<KillZone>();

	public List<GuidRef> DesiredTargets = new List<GuidRef>();

	public float ZoneInOnTargetSpeed = 2f;

	public float SpeedModifierToLockEnemy = 0.05f;

	[HideInInspector]
	public List<Vector3> Targets = new List<Vector3>();

	public void ResolveGuidLinks()
	{
		if (DesiredTargets == null)
		{
			return;
		}
		foreach (GuidRef desiredTarget in DesiredTargets)
		{
			desiredTarget.ResolveLink();
		}
	}
}
