using System.Collections;
using UnityEngine;

public class ShowHideCharactersCommand : Command
{
	public bool Show = true;

	public float Delay = 0.1f;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameplayController gameplayController = GameplayController.Instance();
		yield return new WaitForSeconds(Delay);
		if ((bool)gameplayController)
		{
			Actor[] actors = gameplayController.Selected.ToArray();
			Actor[] array = actors;
			foreach (Actor bod in array)
			{
				ToggleVisibility(bod.model.transform, Show);
				bod.CanSeeHands = Show;
			}
		}
	}

	private void ToggleVisibility(Transform obj, bool Show)
	{
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = Show;
		}
	}
}
