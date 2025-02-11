using System.Collections;
using UnityEngine;

public class LogCommand : Command
{
	public string m_Message = "Log Message";

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		Debug.Log("Log Command: " + m_Message);
		yield return null;
	}
}
