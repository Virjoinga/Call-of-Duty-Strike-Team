using UnityEngine;

public class SentryGunOverride : ContainerOverride
{
	public SpawnerData m_SpawnerOverrideData = new SpawnerData();

	public SentryGunOverrideData m_SentryOverrideData = new SentryGunOverrideData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_SpawnerOverrideData.ResolveGuidLinks();
		Spawner spawner = cont.FindComponentOfType(typeof(Spawner)) as Spawner;
		if (spawner != null)
		{
			spawner.m_Interface = m_SpawnerOverrideData;
			spawner.m_SentryGunOverrides = m_SentryOverrideData;
		}
	}

	public override void CreateAssociatedDefaultObjects(Container cont)
	{
		ContainerOverride containerOverride = cont.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		m_SpawnerOverrideData.EventsList = containerOverride.CreateNewBagObject("Script/Character/Behaviours/Character Event Listeners", "Event Listeners");
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
