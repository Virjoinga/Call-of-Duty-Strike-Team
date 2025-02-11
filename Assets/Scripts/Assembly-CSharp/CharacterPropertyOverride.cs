using UnityEngine;

public class CharacterPropertyOverride : ContainerOverride
{
	public CharacterPropertyData m_OverrideData = new CharacterPropertyData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		CharacterPropertyModifier characterPropertyModifier = cont.FindComponentOfType(typeof(CharacterPropertyModifier)) as CharacterPropertyModifier;
		if (characterPropertyModifier != null)
		{
			characterPropertyModifier.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(characterPropertyModifier);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		CharacterPropertyModifier componentInChildren = gameObj.GetComponentInChildren<CharacterPropertyModifier>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, string param)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, param);
		CharacterPropertyModifier componentInChildren = gameObj.GetComponentInChildren<CharacterPropertyModifier>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject param)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, param);
		CharacterPropertyModifier componentInChildren = gameObj.GetComponentInChildren<CharacterPropertyModifier>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.ActorsToModify == null || m_OverrideData.ActorsToModify.Count == 0)
		{
			return;
		}
		foreach (GameObject item2 in m_OverrideData.ActorsToModify)
		{
			if (item2 != null)
			{
				GameObject gameObject = item2;
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = gameObject.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
	}
}
