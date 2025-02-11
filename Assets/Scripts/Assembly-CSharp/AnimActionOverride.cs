using System;

[Serializable]
public class AnimActionOverride
{
	public AnimAction ActionOverride;

	public AnimAction DefaultAction;

	public AnimActionOverride(AnimAction Action)
	{
		DefaultAction = Action;
		ActionOverride = new AnimAction(Action.ActionName);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
