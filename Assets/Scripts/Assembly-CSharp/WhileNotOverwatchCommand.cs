using System.Collections;
using UnityEngine;

public class WhileNotOverwatchCommand : Command
{
	public GameObject VariableToCheck;

	public int Value;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (HudStateController.Instance.State != HudStateController.HudState.Overwatch)
		{
			yield return null;
		}
	}
}
