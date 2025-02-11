using System.Collections;
using UnityEngine;

public class WhileNot3PCommand : Command
{
	public GameObject VariableToCheck;

	public int Value;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (HudStateController.Instance.State != HudStateController.HudState.TPP)
		{
			yield return null;
		}
	}
}
