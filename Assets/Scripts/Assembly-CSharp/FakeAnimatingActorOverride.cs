using UnityEngine;

public class FakeAnimatingActorOverride : ContainerOverride
{
	public FakeActorData m_OverrideData = new FakeActorData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		FakeActor fakeActor = cont.FindComponentOfType(typeof(FakeActor)) as FakeActor;
		if (fakeActor != null)
		{
			fakeActor.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(fakeActor);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		FakeActor componentInChildren = gameObj.GetComponentInChildren<FakeActor>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
