using System;

[Serializable]
public class SideMenuOption
{
	public UIButton Button;

	public UIButton SmallButton;

	public FrontEndButton ButtonState;

	public string MethodToInvoke = string.Empty;

	public string TextKey;

	public string TooltipText;

	public bool Disabled;
}
