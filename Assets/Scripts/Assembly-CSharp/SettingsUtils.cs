using UnityEngine;

public static class SettingsUtils
{
	public static void SetUpPercentageEditOption(MonoBehaviour target, PercentageEditOption option, string method, float initialValue, string ToolTip)
	{
		if (option != null)
		{
			option.Value = initialValue;
			option.ScriptWithMethodToInvoke = target;
			option.MethodToInvokeOnChange = method;
			option.ToolTip = ToolTip;
		}
	}

	public static void SetUpToggleEditOption(MonoBehaviour target, ToggleEditOption option, string method, bool initialValue, string ToolTip)
	{
		if (option != null)
		{
			option.Value = initialValue;
			option.ScriptWithMethodToInvoke = target;
			option.MethodToInvokeOnChange = method;
			option.ToolTip = ToolTip;
		}
	}
}
