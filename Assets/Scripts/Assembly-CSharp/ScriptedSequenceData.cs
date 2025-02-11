using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptedSequenceData
{
	public List<GameObject> Commands = new List<GameObject>();

	public bool autoRun;

	public bool runAfterLoadout;

	public bool autoLoopOnComplete;

	public void CopyContainerData(ScriptedSequence sc)
	{
		sc.Commands.Clear();
		foreach (GameObject command in Commands)
		{
			ScriptedSequence componentInChildren = command.GetComponentInChildren<ScriptedSequence>();
			if (componentInChildren != null)
			{
				sc.Commands.AddRange(componentInChildren.Commands);
				continue;
			}
			Command[] componentsInChildren = command.GetComponentsInChildren<Command>();
			foreach (Command item in componentsInChildren)
			{
				sc.Commands.Add(item);
			}
		}
	}
}
