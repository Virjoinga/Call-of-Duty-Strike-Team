using UnityEngine;

public class AmmoCacheOverride : ContainerOverride
{
	public AmmoCacheData m_OverrideData = new AmmoCacheData();

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		AmmoCache ammoCache = cont.FindComponentOfType(typeof(AmmoCache)) as AmmoCache;
		if (ammoCache != null)
		{
			ammoCache.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		AmmoCache componentInChildren = gameObj.GetComponentInChildren<AmmoCache>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
