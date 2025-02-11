using UnityEngine;

public class ScriptedSequenceOverride : ContainerOverride
{
	[HideInInspector]
	public Command[] commands;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		commands = cont.GetComponents<Command>();
		if (commands.Length > 0)
		{
			Command[] array = commands;
			foreach (Command command in array)
			{
				command.ResolveGuidLinks();
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		ScriptedSequence componentInChildren = gameObj.GetComponentInChildren<ScriptedSequence>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
