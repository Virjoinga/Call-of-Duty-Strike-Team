using UnityEngine;

public class MysteryCacheControllerOverride : ContainerOverride
{
	public MysteryCacheControllerData m_OverrideData = new MysteryCacheControllerData();

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MysteryCacheController mysteryCacheController = cont.FindComponentOfType(typeof(MysteryCacheController)) as MysteryCacheController;
		if (mysteryCacheController != null)
		{
			mysteryCacheController.m_Interface = m_OverrideData;
			mysteryCacheController.m_Interface.FindMysteryCaches(mysteryCacheController);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MysteryCacheController componentInChildren = gameObj.GetComponentInChildren<MysteryCacheController>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
