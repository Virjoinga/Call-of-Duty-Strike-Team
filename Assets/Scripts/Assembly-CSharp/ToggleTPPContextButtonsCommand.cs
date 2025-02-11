using System.Collections;
using System.Collections.Generic;

public class ToggleTPPContextButtonsCommand : Command
{
	public List<ContextMenuIcons> ButtonsToToggle;

	public TutorialToggles.CMButtonLockState LockState;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		using (List<ContextMenuIcons>.Enumerator enumerator = ButtonsToToggle.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case ContextMenuIcons.Melee:
					TutorialToggles.CMMeeleLockState = LockState;
					break;
				case ContextMenuIcons.Shoot:
					TutorialToggles.CMAimedShotLockState = LockState;
					break;
				case ContextMenuIcons.Supress:
					TutorialToggles.CMSupressLockState = LockState;
					break;
				case ContextMenuIcons.PickupBody:
					TutorialToggles.CMCarryBodyLockState = LockState;
					break;
				}
			}
		}
		yield break;
	}
}
