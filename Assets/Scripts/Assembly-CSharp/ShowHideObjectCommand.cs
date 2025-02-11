using System.Collections;
using UnityEngine;

public class ShowHideObjectCommand : Command
{
	public bool Show = true;

	public GameObject TheObject;

	public float Delay = 0.1f;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		yield return new WaitForSeconds(Delay);
		if ((bool)TheObject)
		{
			ToggleVisibility(TheObject.transform, Show);
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
