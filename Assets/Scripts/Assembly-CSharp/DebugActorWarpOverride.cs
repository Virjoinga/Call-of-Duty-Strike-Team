using UnityEngine;

public class DebugActorWarpOverride : ContainerOverride
{
	public DebugActorWarpData m_OverrideData = new DebugActorWarpData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DebugActorWarp debugActorWarp = cont.FindComponentOfType(typeof(DebugActorWarp)) as DebugActorWarp;
		if (debugActorWarp != null)
		{
			debugActorWarp.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(debugActorWarp);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(0f, 0f, 1f, 1f);
		if (m_OverrideData.ActorToWarp != null)
		{
			LineFlag inFlag = LineFlag.In;
			Transform inTrans = m_OverrideData.ActorToWarp.gameObject.transform;
			Color inColor = color;
			LineDetail item = new LineDetail(inFlag, inTrans, inColor);
			visibleConnections.lineProperties.Add(item);
		}
	}

	public void OnDrawGizmos()
	{
		Vector3 center = base.transform.position + new Vector3(0f, 0.5f, 0f);
		if (!m_OverrideData.Active)
		{
			Gizmos.DrawIcon(center, "cross");
		}
		else
		{
			Gizmos.DrawIcon(center, "teleport");
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DebugActorWarp componentInChildren = gameObj.GetComponentInChildren<DebugActorWarp>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
