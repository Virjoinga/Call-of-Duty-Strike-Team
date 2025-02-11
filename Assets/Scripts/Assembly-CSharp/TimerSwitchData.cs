using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimerSwitchData
{
	public GuidRef StatusLight;

	[HideInInspector]
	public SwitchLight StatusLightComp;

	public bool IsTimerSwtich;

	public float ActiveSwitchTime = 5f;

	public List<GameObject> ObjectToControl;

	public List<string> FuncToCallOnHack;

	public List<string> FuncToCallOnTimeOut;
}
