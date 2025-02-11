using UnityEngine;

public class ScriptVariableOverride : ContainerOverride
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
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		ScriptVariable componentInChildren = gameObj.GetComponentInChildren<ScriptVariable>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
