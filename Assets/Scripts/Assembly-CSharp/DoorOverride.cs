using UnityEngine;

public class DoorOverride : ContainerOverride
{
	public BuildingDoor.eDoorType DoorType;

	public NavGateData.CharacterType WalkableBy = NavGateData.CharacterType.Enemy;

	public bool Breachable;

	public bool CanOpenAndClose = true;

	public bool StartLocked;

	public bool IsInterior;

	public bool StartContextDisabled;

	public NavGateData.CharacterType WalkableByWhenClosed = NavGateData.CharacterType.Enemy;

	public BreachSequenceData BreachSequence = new BreachSequenceData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MeshSwitchOverride meshSwitchOverride = cont.FindComponentOfType(typeof(MeshSwitchOverride)) as MeshSwitchOverride;
		if (meshSwitchOverride != null)
		{
			meshSwitchOverride.m_Interface.DoorType = DoorType;
			Container container = meshSwitchOverride.gameObject.GetComponent(typeof(Container)) as Container;
			container.ApplyOverride();
		}
		BreachableDoorOverride breachableDoorOverride = cont.FindComponentOfType(typeof(BreachableDoorOverride)) as BreachableDoorOverride;
		if (breachableDoorOverride != null)
		{
			breachableDoorOverride.m_OverrideData.Breachable = Breachable;
			breachableDoorOverride.m_OverrideData.WalkableBy = WalkableBy;
			breachableDoorOverride.m_OverrideData.CanOpenAndClose = CanOpenAndClose;
			breachableDoorOverride.m_OverrideData.IsInterior = IsInterior;
			breachableDoorOverride.m_OverrideData.StartContextDisabled = StartContextDisabled;
			breachableDoorOverride.m_OverrideData.StartLocked = StartLocked;
			breachableDoorOverride.m_OverrideData.WalkableBy = WalkableByWhenClosed;
			breachableDoorOverride.m_SequenceOverrideData = BreachSequence;
			Container container2 = breachableDoorOverride.gameObject.GetComponent(typeof(Container)) as Container;
			container2.ApplyOverride();
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		BreachableDoorOverride componentInChildren = gameObj.GetComponentInChildren<BreachableDoorOverride>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
