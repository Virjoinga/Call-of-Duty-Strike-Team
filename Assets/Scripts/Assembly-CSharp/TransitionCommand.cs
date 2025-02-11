using System.Collections;
using UnityEngine;

public class TransitionCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		AnimatedScreenBackground bg = AnimatedScreenBackground.Instance;
		if (bg != null)
		{
			bg.Activate();
			while (!bg.IsActive)
			{
				yield return new WaitForEndOfFrame();
			}
			bg.Hide();
		}
		yield return null;
	}
}
