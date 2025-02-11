using System.Collections;

public class WhileTPPContextMenuNotPressedCommand : Command
{
	public InteriorItem ObjectWithCM;

	public bool TestButtonInstead;

	public ContextMenuIcons CMButtonToWaitFor;

	public override void ResolveGuidLinks()
	{
		if (ObjectWithCM != null)
		{
			ObjectWithCM.ResolveGuidLinks();
		}
	}

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (!TestButtonInstead)
		{
			InterfaceableObject cm = null;
			if (ObjectWithCM != null && ObjectWithCM.Interior != null)
			{
				InteriorOverride io = ObjectWithCM.Interior.theObject.GetComponentInChildren<InteriorOverride>();
				if ((bool)io)
				{
					cm = ((ObjectWithCM.ContextType != 0) ? io.m_ActiveWindows[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>() : io.m_ActiveDoors[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>());
				}
				else
				{
					ActorWrapper aw = ObjectWithCM.Interior.theObject.GetComponentInChildren<ActorWrapper>();
					if (aw != null)
					{
						Actor a = aw.GetActor();
						if (a != null)
						{
							cm = a.GetComponentInChildren<InterfaceableObject>();
						}
					}
					if (cm == null)
					{
						cm = ObjectWithCM.Interior.theObject.GetComponentInChildren<InterfaceableObject>();
					}
				}
			}
			while (CommonHudController.Instance.ContextMenu == null || CommonHudController.Instance.ContextMenu.mInterfaceObj != cm)
			{
				yield return null;
			}
			yield break;
		}
		while (CommonHudController.Instance.ContextMenu == null || CommonHudController.Instance.ContextMenu.GetButton(CMButtonToWaitFor) == null)
		{
			yield return null;
		}
		MenuButton button = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToWaitFor);
		while (button == null || button.Button.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			if (CommonHudController.Instance.ContextMenu == null)
			{
				button = null;
			}
			if (button == null && CommonHudController.Instance.ContextMenu != null)
			{
				button = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToWaitFor);
			}
			yield return null;
		}
	}
}
