using UnityEngine;

public class WorldActorsCommandOverride : ContainerOverride
{
	public WorldActorData m_OverrideData = new WorldActorData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		WorldActorsCommand worldActorsCommand = cont.FindComponentOfType(typeof(WorldActorsCommand)) as WorldActorsCommand;
		if (worldActorsCommand != null)
		{
			worldActorsCommand.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(worldActorsCommand);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		WorldActorsCommand componentInChildren = gameObj.GetComponentInChildren<WorldActorsCommand>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
