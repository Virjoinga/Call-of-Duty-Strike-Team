using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RespotData
{
	public List<RespotActorDescriptor> RespotInfo = new List<RespotActorDescriptor>();

	public List<GameObject> ObjectsToCall = new List<GameObject>();

	public List<string> FunctionsToCall = new List<string>();

	public List<bool> DontDeferCall = new List<bool>();

	public string FailMessage = "S_RESPOT_FAIL";

	public string RespotMessage = "S_RESPOT_TRY";
}
