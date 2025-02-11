using UnityEngine;

public class TerminalOverride : ContainerOverride
{
	public HackTerminalData m_OverrideData = new HackTerminalData();

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
		hackableOverride.mOverrideData.TimeToHack = m_OverrideData.TimeToHack;
		hackableOverride.mOverrideData.ObjectToCallOnSuccess = m_OverrideData.ObjectToCallOnSuccess;
		hackableOverride.mOverrideData.FunctionToCallOnSuccess = m_OverrideData.FunctionToCallOnSuccess;
		hackableOverride.mOverrideData.GroupObjectToCallOnSuccess = m_OverrideData.GroupObjectToCallOnSuccess;
		hackableOverride.mOverrideData.GroupFunctionToCallOnSuccess = m_OverrideData.GroupFunctionToCallOnSuccess;
		hackableOverride.mOverrideData.ObjectToCallOnFail = m_OverrideData.ObjectToCallOnFail;
		hackableOverride.mOverrideData.FunctionToCallOnFail = m_OverrideData.FunctionToCallOnFail;
		hackableOverride.mOverrideData.GroupObjectToCallOnFail = m_OverrideData.GroupObjectToCallOnFail;
		hackableOverride.mOverrideData.GroupFunctionToCallOnFail = m_OverrideData.GroupFunctionToCallOnFail;
		if (m_OverrideData.TimerData != null)
		{
			hackableOverride.mOverrideData.TimerData.IsTimerSwtich = m_OverrideData.TimerData.IsTimerSwtich;
			hackableOverride.mOverrideData.TimerData.ActiveSwitchTime = m_OverrideData.TimerData.ActiveSwitchTime;
			hackableOverride.mOverrideData.TimerData.ObjectToControl = m_OverrideData.TimerData.ObjectToControl;
			hackableOverride.mOverrideData.TimerData.FuncToCallOnHack = m_OverrideData.TimerData.FuncToCallOnHack;
			hackableOverride.mOverrideData.TimerData.FuncToCallOnTimeOut = m_OverrideData.TimerData.FuncToCallOnTimeOut;
		}
		if (hackableOverride != null)
		{
			Object.DestroyImmediate(this);
		}
	}
}
