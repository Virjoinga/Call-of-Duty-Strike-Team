using UnityEngine;

public class RespotOnFailOverride : ContainerOverride
{
	public RespotData m_OverrideData = new RespotData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		RespotOnFail respotOnFail = cont.FindComponentOfType(typeof(RespotOnFail)) as RespotOnFail;
		if (respotOnFail != null)
		{
			respotOnFail.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		RespotOnFail componentInChildren = gameObj.GetComponentInChildren<RespotOnFail>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
	}

	public void OnDrawGizmos()
	{
		foreach (RespotActorDescriptor item in m_OverrideData.RespotInfo)
		{
			if ((bool)item.ActorToRespot)
			{
				Vector3 center = item.ActorToRespot.transform.position + new Vector3(0f, 1f, 0f);
				Gizmos.DrawIcon(center, "stars");
			}
		}
	}
}
