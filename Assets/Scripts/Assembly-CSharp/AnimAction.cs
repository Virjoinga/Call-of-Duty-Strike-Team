using System;

[Serializable]
public class AnimAction
{
	public string ActionName;

	public DirectedAnim Anim;

	public bool HasAnimation;

	public AnimAction(string Name)
	{
		ActionName = Name;
	}

	public void UpdateHasAnimation()
	{
		if (Anim != null)
		{
			HasAnimation = Anim.HasAnimation();
		}
	}
}
