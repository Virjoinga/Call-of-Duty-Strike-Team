using System.Collections;
using UnityEngine;

public class SendMessageOnRecievedCountDataCommand : Command
{
	public GameObject Target;

	public RecievedCountData data;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		SendMessageOnRecievedCount sd = Target.GetComponentInChildren<SendMessageOnRecievedCount>();
		sd.m_Interface = data;
		sd.Start();
		yield break;
	}
}
