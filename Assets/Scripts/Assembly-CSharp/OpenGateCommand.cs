using System.Collections;
using UnityEngine;

public class OpenGateCommand : Command
{
	public GameObject gate;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GateSlide gs = gate.GetComponentInChildren<GateSlide>();
		if (gs != null)
		{
			gs.Activate();
		}
		yield break;
	}
}
