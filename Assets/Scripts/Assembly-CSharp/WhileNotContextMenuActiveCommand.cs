using System.Collections;
using UnityEngine;

public class WhileNotContextMenuActiveCommand : Command
{
	public GameObject VariableToCheck;

	public int Value;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (!CommonHudController.Instance.ContextMenuActive)
		{
			yield return null;
		}
	}
}
