using UnityEngine;

public class HelicopterOverride : ContainerOverride
{
	public HelicopterData m_HelicopterOverrideData = new HelicopterData();

	public HelicopterRoutineData m_RoutineOverrideData = new HelicopterRoutineData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		HelicopterRoutine helicopterRoutine = cont.FindComponentOfType(typeof(HelicopterRoutine)) as HelicopterRoutine;
		if (helicopterRoutine != null)
		{
			helicopterRoutine.m_Interface = m_RoutineOverrideData;
			helicopterRoutine.m_Interface.CopyContainerData(helicopterRoutine);
		}
		Helicopter helicopter = cont.FindComponentOfType(typeof(Helicopter)) as Helicopter;
		if (helicopter != null)
		{
			helicopter.m_Interface = m_HelicopterOverrideData;
			helicopter.m_Interface.CopyContainerData(helicopter);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		if (methodName == "FireRocketAtCurrentTarget")
		{
			base.SendOverrideMessage(gameObj, methodName);
			Helicopter componentInChildren = gameObj.GetComponentInChildren<Helicopter>();
			if (componentInChildren != null)
			{
				componentInChildren.SendMessage(methodName);
			}
		}
		else
		{
			base.SendOverrideMessage(gameObj, methodName);
			HelicopterRoutine componentInChildren2 = gameObj.GetComponentInChildren<HelicopterRoutine>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SendMessage(methodName);
			}
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject param)
	{
		if (methodName == "DoPathToTarget" || methodName == "ChangeDestructionNotification")
		{
			base.SendOverrideMessageWithParam(gameObj, methodName, param);
			HelicopterRoutine componentInChildren = gameObj.GetComponentInChildren<HelicopterRoutine>();
			if (componentInChildren != null)
			{
				componentInChildren.SendMessage(methodName, param);
			}
		}
		else
		{
			base.SendOverrideMessageWithParam(gameObj, methodName, param);
			Helicopter componentInChildren2 = gameObj.GetComponentInChildren<Helicopter>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SendMessage(methodName, param);
			}
		}
	}
}
