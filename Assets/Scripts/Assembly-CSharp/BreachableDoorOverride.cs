using UnityEngine;

public class BreachableDoorOverride : ContainerOverride
{
	public BreachableDoorData m_OverrideData = new BreachableDoorData();

	public BreachSequenceData m_SequenceOverrideData = new BreachSequenceData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		BuildingDoor buildingDoor = cont.FindComponentOfType(typeof(BuildingDoor)) as BuildingDoor;
		BreachSequence breachSequence = cont.FindComponentOfType(typeof(BreachSequence)) as BreachSequence;
		if (buildingDoor != null)
		{
			Transform transform = null;
			GameObject innerObject = cont.GetInnerObject();
			if (innerObject != null && m_OverrideData.ExplosionOrigin == null && buildingDoor.m_Interface.ExplosionOrigin != null)
			{
				transform = buildingDoor.m_Interface.ExplosionOrigin;
			}
			buildingDoor.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(buildingDoor);
			if (transform != null)
			{
				buildingDoor.m_Interface.ExplosionOrigin = transform;
			}
		}
		if (breachSequence != null)
		{
			breachSequence.MaximumSlowDownDuration = m_SequenceOverrideData.MaximumSlowDownDuration;
			breachSequence.RequiredActors = m_SequenceOverrideData.RequiredActors;
			if (m_SequenceOverrideData.BreachComponents != null)
			{
				breachSequence.BreachComponents = m_SequenceOverrideData.BreachComponents;
			}
			breachSequence.BreachMessages = m_SequenceOverrideData.BreachMessages;
		}
		if (m_OverrideData.StartContextDisabled)
		{
			CMBuildingDoor cMBuildingDoor = cont.FindComponentOfType(typeof(CMBuildingDoor)) as CMBuildingDoor;
			if (cMBuildingDoor != null)
			{
				cMBuildingDoor.enabled = false;
			}
		}
		else
		{
			CMBuildingDoor cMBuildingDoor2 = cont.FindComponentOfType(typeof(CMBuildingDoor)) as CMBuildingDoor;
			if (cMBuildingDoor2 != null)
			{
				cMBuildingDoor2.enabled = true;
			}
		}
	}

	public void Activate()
	{
		BuildingDoor buildingDoor = base.gameObject.GetComponentInChildren(typeof(BuildingDoor)) as BuildingDoor;
		if (buildingDoor != null)
		{
			buildingDoor.Unlock();
			CMBuildingDoor componentInChildren = IncludeDisabled.GetComponentInChildren<CMBuildingDoor>(base.gameObject);
			if (componentInChildren != null)
			{
				componentInChildren.Activate();
				componentInChildren.TurnOn();
			}
			ContextMenuDistanceManager componentInChildren2 = IncludeDisabled.GetComponentInChildren<ContextMenuDistanceManager>(base.gameObject);
			if (componentInChildren2 != null)
			{
				componentInChildren2.enabled = true;
			}
		}
	}

	public void Deactivate()
	{
		BuildingDoor buildingDoor = base.gameObject.GetComponentInChildren(typeof(BuildingDoor)) as BuildingDoor;
		if (buildingDoor != null)
		{
			buildingDoor.Lock();
		}
		CMBuildingDoor componentInChildren = IncludeDisabled.GetComponentInChildren<CMBuildingDoor>(base.gameObject);
		if (componentInChildren != null)
		{
			componentInChildren.Deactivate();
			componentInChildren.TurnOn();
		}
	}
}
