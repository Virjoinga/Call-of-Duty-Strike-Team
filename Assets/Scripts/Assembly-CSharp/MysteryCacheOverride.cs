using UnityEngine;

public class MysteryCacheOverride : ContainerOverride
{
	public MysteryCacheData m_OverrideData = new MysteryCacheData();

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MysteryCache mysteryCache = cont.FindComponentOfType(typeof(MysteryCache)) as MysteryCache;
		if (mysteryCache != null)
		{
			mysteryCache.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MysteryCache componentInChildren = gameObj.GetComponentInChildren<MysteryCache>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
