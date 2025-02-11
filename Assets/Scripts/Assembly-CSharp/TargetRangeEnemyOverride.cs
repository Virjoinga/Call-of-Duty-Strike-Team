using UnityEngine;

public class TargetRangeEnemyOverride : ContainerOverride
{
	public TargetRangeEnemyData m_TargetRangeEnemyOverrideData = new TargetRangeEnemyData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		PathObject_TS pathObject_TS = cont.FindComponentOfType(typeof(PathObject_TS)) as PathObject_TS;
		if (pathObject_TS != null)
		{
			pathObject_TS.m_Interface = m_TargetRangeEnemyOverrideData;
			pathObject_TS.m_Interface.CopyContainerData(pathObject_TS);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		PathObject_TS componentInChildren = gameObj.GetComponentInChildren<PathObject_TS>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
