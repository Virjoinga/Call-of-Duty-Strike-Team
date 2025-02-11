using UnityEngine;

public class TerminalLightOverride : ContainerOverride
{
	public enum InitialColour
	{
		Red = 0,
		Green = 1
	}

	public InitialColour initialColour = InitialColour.Green;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SwitchLight switchLight = cont.FindComponentOfType(typeof(SwitchLight)) as SwitchLight;
		if (switchLight != null)
		{
			if (initialColour == InitialColour.Green)
			{
				switchLight.onColour = HaloEffect.HaloColour.Green;
				switchLight.offColour = HaloEffect.HaloColour.Red;
			}
			else
			{
				switchLight.onColour = HaloEffect.HaloColour.Red;
				switchLight.offColour = HaloEffect.HaloColour.Green;
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SwitchLight componentInChildren = gameObj.GetComponentInChildren<SwitchLight>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
