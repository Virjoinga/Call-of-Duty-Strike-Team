using UnityEngine;

public class EnemySniperOverride : ContainerOverride
{
	public SpawnerData m_SpawnerOverrideData = new SpawnerData();

	public SniperOverrideData m_SniperOverrideData = new SniperOverrideData();

	public override void SetupOverride(Container cont)
	{
		RoutineDescriptor componentInChildren = GetComponentInChildren<RoutineDescriptor>();
		if (componentInChildren != null)
		{
			SniperDescriptor component = componentInChildren.GetComponent<SniperDescriptor>();
			if (component != null && (m_SniperOverrideData.KillZones.Count == 0 || m_SniperOverrideData.KillZones[0] == null))
			{
				if (m_SniperOverrideData.KillZones.Count == 0)
				{
					m_SniperOverrideData.KillZones.Add(cont.GetInnerObject().GetComponentInChildren<KillZone>());
				}
				else
				{
					m_SniperOverrideData.KillZones[0] = cont.GetInnerObject().GetComponentInChildren<KillZone>();
				}
			}
		}
		else
		{
			Debug.Log("No routine on EnemySniperOverride " + cont.name);
		}
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_SpawnerOverrideData.ResolveGuidLinks();
		m_SniperOverrideData.ResolveGuidLinks();
		Spawner spawner = cont.FindComponentOfType(typeof(Spawner)) as Spawner;
		if (spawner != null)
		{
			spawner.m_Interface = m_SpawnerOverrideData;
		}
		if (m_SniperOverrideData.KillZones.Count >= 1 && m_SniperOverrideData.DesiredTargets.Count == 0)
		{
			m_SniperOverrideData.Targets.Clear();
			NewCoverPointManager newCoverPointManager = (NewCoverPointManager)Object.FindObjectOfType(typeof(NewCoverPointManager));
			for (int i = 0; i < newCoverPointManager.coverPoints.Length - 1; i++)
			{
				foreach (KillZone killZone in m_SniperOverrideData.KillZones)
				{
					BoxCollider component = killZone.GetComponent<BoxCollider>();
					if (component != null && component.bounds.Contains(newCoverPointManager.coverPoints[i].gamePos))
					{
						m_SniperOverrideData.Targets.Add(newCoverPointManager.coverPoints[i].gamePos);
					}
				}
			}
		}
		SniperDescriptor component2 = GetComponentInChildren<RoutineDescriptor>().GetComponent<SniperDescriptor>();
		if (component2 != null)
		{
			if (m_SniperOverrideData.KillZones.Count == 0 || m_SniperOverrideData.KillZones[0] == null)
			{
				m_SniperOverrideData.KillZones[0] = cont.GetInnerObject().GetComponentInChildren<KillZone>();
			}
			component2.SniperOverrides = m_SniperOverrideData;
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
