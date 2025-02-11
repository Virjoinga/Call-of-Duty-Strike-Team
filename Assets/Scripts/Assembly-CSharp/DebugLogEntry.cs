using System;
using UnityEngine;

public class DebugLogEntry
{
	public string Log { get; set; }

	public string Stack { get; set; }

	public LogType Type { get; set; }

	public DateTime LogTime { get; set; }

	public override string ToString()
	{
		return LogTime.ToString() + " " + Type.ToString() + " : " + Log + "\n" + Stack;
	}
}
