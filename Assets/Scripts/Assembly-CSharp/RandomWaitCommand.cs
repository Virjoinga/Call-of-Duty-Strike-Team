using System.Collections;
using UnityEngine;

public class RandomWaitCommand : Command
{
	public float MinSeconds;

	public float MaxSeconds;

	private float seconds;

	public override bool Blocking()
	{
		return seconds > 0f;
	}

	public override IEnumerator Execute()
	{
		seconds = Random.Range(MinSeconds, MaxSeconds);
		yield return new WaitForSeconds(seconds);
	}
}
