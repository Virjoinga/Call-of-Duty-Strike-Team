using UnityEngine;

public class AntiTankMineOverride : ContainerOverride
{
	public AntiTankMineData m_OverrideData = new AntiTankMineData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		HackableOverride hackableOverride = GetComponent<HackableOverride>();
		if (hackableOverride == null)
		{
			hackableOverride = base.gameObject.AddComponent<HackableOverride>();
		}
		hackableOverride.mOverrideData.TimeToHack = m_OverrideData.TimeToDefuse;
		hackableOverride.mOverrideData.ObjectToCallOnStart = m_OverrideData.ObjectToCallOnStart;
		hackableOverride.mOverrideData.FunctionToCallOnStart = m_OverrideData.FunctionToCallOnStart;
		hackableOverride.mOverrideData.ObjectToCallOnSuccess = m_OverrideData.ObjectToCallOnSuccess;
		hackableOverride.mOverrideData.FunctionToCallOnSuccess = m_OverrideData.FunctionToCallOnSuccess;
		if (hackableOverride != null)
		{
			Object.DestroyImmediate(this);
		}
	}
}
