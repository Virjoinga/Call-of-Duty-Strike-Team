using System.Collections;
using UnityEngine;

public class PlayAnimationCommand : Command
{
	public Animation Target;

	public string ClipName = "Take 001";

	public bool Wait;

	public float Speed = 1f;

	public bool Reverse;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		if (Reverse)
		{
			Target[ClipName].time = Target[ClipName].length;
			Target[ClipName].speed = 0f - Speed;
			Target.Play(ClipName);
		}
		else
		{
			Target[ClipName].speed = Speed;
			Target.Play(ClipName);
		}
		if (Wait)
		{
			yield return new WaitForSeconds(Target[ClipName].length / Speed);
		}
	}
}
