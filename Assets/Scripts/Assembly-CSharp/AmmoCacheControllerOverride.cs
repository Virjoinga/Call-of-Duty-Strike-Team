using UnityEngine;

public class AmmoCacheControllerOverride : ContainerOverride
{
	public AmmoCacheControllerData m_OverrideData = new AmmoCacheControllerData();

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		AmmoCacheController ammoCacheController = cont.FindComponentOfType(typeof(AmmoCacheController)) as AmmoCacheController;
		if (ammoCacheController != null)
		{
			ammoCacheController.m_Interface = m_OverrideData;
			ammoCacheController.m_Interface.FindAmmoCaches(ammoCacheController);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		AmmoCacheController componentInChildren = gameObj.GetComponentInChildren<AmmoCacheController>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
