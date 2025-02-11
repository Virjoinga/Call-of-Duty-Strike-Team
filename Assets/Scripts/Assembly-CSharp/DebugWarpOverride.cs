using UnityEngine;

public class DebugWarpOverride : ContainerOverride
{
	public DebugWarpData m_OverrideData = new DebugWarpData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DebugWarp debugWarp = cont.FindComponentOfType(typeof(DebugWarp)) as DebugWarp;
		if (debugWarp != null)
		{
			debugWarp.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(debugWarp);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DebugWarp componentInChildren = gameObj.GetComponentInChildren<DebugWarp>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(0f, 1f, 0f, 1f);
		if (m_OverrideData.ActorsToWarp != null && m_OverrideData.ActorsToWarp.Count != 0)
		{
			foreach (GameObject item3 in m_OverrideData.ActorsToWarp)
			{
				LineFlag inFlag = LineFlag.In;
				Transform inTrans = item3.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
		color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.WarpPosition != null)
		{
			LineFlag inFlag2 = LineFlag.Out;
			Transform inTrans2 = m_OverrideData.WarpPosition.transform;
			Color inColor2 = color;
			LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
			visibleConnections.lineProperties.Add(item2);
		}
	}
}
