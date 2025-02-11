using System.Collections;
using UnityEngine;

public class WhileNotFPPContextMenuVisibleCommand : Command
{
	public GameObject ContextMenuObject;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (!(ContextMenuObject != null))
		{
			yield break;
		}
		InterfaceableObject io = ContextMenuObject.GetComponentInChildren<InterfaceableObject>();
		if (io != null)
		{
			while (!io.IsActionIconVisible() || !CommonHudController.Instance.ContextInteractionButton.gameObject.activeInHierarchy)
			{
				yield return null;
			}
		}
	}
}
