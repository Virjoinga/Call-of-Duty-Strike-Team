using UnityEngine;

public class GateOverride : ContainerOverride
{
	public bool m_Locked = true;

	public bool m_StartOpen;

	[HideInInspector]
	public GateLogic Logic;

	public GameObject EntranceGateLogicGameObject;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		TriggerVolume triggerVolume = cont.GetComponentInChildren(typeof(TriggerVolume)) as TriggerVolume;
		if (triggerVolume != null)
		{
			triggerVolume.m_Interface.Locked = m_Locked;
		}
		Debug.LogWarning("AC: GateOverride is out of date (tries to use boolean 'walkable' on navgate. If you want to use this, it'll need fixing up.");
		GateLogic gateLogic = cont.GetComponentInChildren(typeof(GateLogic)) as GateLogic;
		if (gateLogic != null)
		{
			gateLogic.StartOpen = m_StartOpen;
		}
		Logic = gateLogic;
		if (!(EntranceGateLogicGameObject != null))
		{
			return;
		}
		EntranceGateLogic component = EntranceGateLogicGameObject.GetComponent<EntranceGateLogic>();
		if (component != null)
		{
			TriggerVolume triggerVolume2 = Object.FindObjectOfType(typeof(TriggerVolume)) as TriggerVolume;
			if (triggerVolume2 != null)
			{
				component.GateObject1 = triggerVolume2.gameObject;
				component.GateObject2 = triggerVolume2.gameObject;
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		if (Logic != null)
		{
			Logic.SendMessage(methodName);
		}
	}
}
