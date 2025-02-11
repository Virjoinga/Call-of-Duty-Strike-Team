using System.Collections;
using UnityEngine;

public class WhileNotCommand : Command
{
	public GameObject VariableToCheck;

	public int Value;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		ScriptVariable v = VariableToCheck.GetComponentInChildren<ScriptVariable>();
		if (v == null)
		{
			Debug.LogError("Script Variable Not set");
			yield break;
		}
		while (v.Value < Value)
		{
			yield return null;
		}
	}
}
