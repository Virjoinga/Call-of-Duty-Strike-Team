using System.Collections;
using UnityEngine;

public class WaitCommand : Command
{
	public float Seconds;

	public override bool Blocking()
	{
		return Seconds > 0f;
	}

	public override IEnumerator Execute()
	{
		yield return new WaitForSeconds(Seconds);
	}
}
