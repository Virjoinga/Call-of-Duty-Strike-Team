using UnityEngine;

public class SpawnControllerOverride : ContainerOverride
{
	public SpawnControllerData m_OverrideData = new SpawnControllerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SpawnController spawnController = cont.FindComponentOfType(typeof(SpawnController)) as SpawnController;
		if (spawnController != null)
		{
			spawnController.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(spawnController);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SpawnController componentInChildren = gameObj.GetComponentInChildren<SpawnController>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		if (m_OverrideData.SpawnersToControl == null || m_OverrideData.SpawnersToControl.Count == 0)
		{
			return;
		}
		foreach (GameObject item2 in m_OverrideData.SpawnersToControl)
		{
			if (item2 != null)
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = item2.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
	}
}
