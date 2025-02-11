using UnityEngine;

public class MeshSwitchOverride : ContainerOverride
{
	public MeshSwitchData m_Interface = new MeshSwitchData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		if (m_Interface.DoorType == BuildingDoor.eDoorType.None)
		{
			cont.gameObject.SetActive(false);
		}
		else
		{
			cont.gameObject.SetActive(true);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
	}
}
