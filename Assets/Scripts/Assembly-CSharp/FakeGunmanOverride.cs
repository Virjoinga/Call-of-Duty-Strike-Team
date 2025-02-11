using UnityEngine;

public class FakeGunmanOverride : ContainerOverride
{
	public FakeGunmanData m_OverrideData = new FakeGunmanData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		FakeGunman fakeGunman = cont.FindComponentOfType(typeof(FakeGunman)) as FakeGunman;
		if (fakeGunman != null)
		{
			fakeGunman.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(fakeGunman);
			FakeGunfire componentInChildren = fakeGunman.GetComponentInChildren<FakeGunfire>();
			if (componentInChildren != null)
			{
				componentInChildren.GenerateImpactPoints();
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		FakeGunman componentInChildren = gameObj.GetComponentInChildren<FakeGunman>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
