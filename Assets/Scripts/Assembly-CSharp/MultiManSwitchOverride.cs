using UnityEngine;

public class MultiManSwitchOverride : ContainerOverride
{
	public MultiManSwitchData m_OverrideData = new MultiManSwitchData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MultiManSwitch multiManSwitch = cont.FindComponentOfType(typeof(MultiManSwitch)) as MultiManSwitch;
		if (!(multiManSwitch != null))
		{
			return;
		}
		multiManSwitch.m_Interface = m_OverrideData;
		foreach (GameObject requiredTerminal in multiManSwitch.m_Interface.RequiredTerminals)
		{
			Container component = requiredTerminal.GetComponent<Container>();
			HackableOverride componentInChildren = requiredTerminal.GetComponentInChildren<HackableOverride>();
			componentInChildren.mOverrideData.ObjectToCallOnSuccess = cont.gameObject;
			component.ApplyOverride();
			HackableObject componentInChildren2 = requiredTerminal.GetComponentInChildren<HackableObject>();
			componentInChildren2.AllowDestroyOnHack = false;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MultiManSwitch componentInChildren = gameObj.GetComponentInChildren<MultiManSwitch>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(0f, 1f, 0f, 1f);
		if (m_OverrideData.RequiredTerminals != null && m_OverrideData.RequiredTerminals.Count != 0)
		{
			foreach (GameObject requiredTerminal in m_OverrideData.RequiredTerminals)
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = requiredTerminal.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
		color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.ObjectsToMessageOnSuccess == null || m_OverrideData.ObjectsToMessageOnSuccess.Count == 0)
		{
			return;
		}
		foreach (GameObject item3 in m_OverrideData.ObjectsToMessageOnSuccess)
		{
			if (item3 != null)
			{
				LineFlag inFlag2 = LineFlag.Out;
				Transform inTrans2 = item3.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
	}
}
