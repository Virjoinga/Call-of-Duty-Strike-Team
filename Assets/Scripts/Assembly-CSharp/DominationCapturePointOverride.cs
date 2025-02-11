using UnityEngine;

public class DominationCapturePointOverride : ContainerOverride
{
	public DominationCapturePointData m_OverrideData = new DominationCapturePointData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DominationCapturePoint dominationCapturePoint = cont.FindComponentOfType(typeof(DominationCapturePoint)) as DominationCapturePoint;
		if (dominationCapturePoint != null)
		{
			dominationCapturePoint.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(dominationCapturePoint);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DominationCapturePoint componentInChildren = gameObj.GetComponentInChildren<DominationCapturePoint>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
	}
}
