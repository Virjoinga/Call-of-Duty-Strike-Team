using UnityEngine;

public class SpawnerCoordinatorOverride : ContainerOverride
{
	public SpawnerCoordinatorData m_OverrideData = new SpawnerCoordinatorData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_OverrideData.ResolveGuidLinks();
		SpawnerCoordinator spawnerCoordinator = cont.FindComponentOfType(typeof(SpawnerCoordinator)) as SpawnerCoordinator;
		if (spawnerCoordinator != null)
		{
			spawnerCoordinator.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SpawnerCoordinator componentInChildren = gameObj.GetComponentInChildren<SpawnerCoordinator>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.MultiSpawners == null || m_OverrideData.MultiSpawners.Count == 0)
		{
			return;
		}
		foreach (GameObject multiSpawner in m_OverrideData.MultiSpawners)
		{
			LineFlag inFlag = LineFlag.Out;
			Transform inTrans = multiSpawner.transform;
			Color inColor = color;
			LineDetail item = new LineDetail(inFlag, inTrans, inColor);
			visibleConnections.lineProperties.Add(item);
		}
	}
}
