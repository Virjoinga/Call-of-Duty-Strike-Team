using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleContextInteractionsCommand : Command
{
	public bool Deactivate;

	public bool ToggleAll;

	public List<GameObject> InteractableObjects = new List<GameObject>();

	public List<InteriorItem> InteriorItems = new List<InteriorItem>();

	public override void ResolveGuidLinks()
	{
		if (InteriorItems == null)
		{
			return;
		}
		foreach (InteriorItem interiorItem in InteriorItems)
		{
			interiorItem.ResolveGuidLinks();
		}
	}

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (ToggleAll)
		{
			if (Deactivate)
			{
				TutorialToggles.DisableAllInteractions = true;
			}
			else
			{
				TutorialToggles.DisableAllInteractions = false;
			}
			yield break;
		}
		if (InteractableObjects != null && InteractableObjects.Count > 0)
		{
			foreach (GameObject go in InteractableObjects)
			{
				InterfaceableObject io3 = go.GetComponentInChildren<InterfaceableObject>();
				if (io3 == null)
				{
					ActorWrapper aw = go.GetComponentInChildren<ActorWrapper>();
					if (aw != null)
					{
						Actor a = aw.GetActor();
						if (a != null)
						{
							io3 = a.gameObject.GetComponentInChildren<InterfaceableObject>();
						}
					}
				}
				if (io3 != null)
				{
					if (Deactivate)
					{
						io3.IgnoreTutorialLock = false;
						continue;
					}
					io3.IgnoreTutorialLock = true;
					io3.Activate();
				}
			}
		}
		if (InteriorItems == null || InteriorItems.Count <= 0)
		{
			yield break;
		}
		foreach (InteriorItem item in InteriorItems)
		{
			if (item.Interior == null)
			{
				continue;
			}
			InteriorOverride interior = item.Interior.theObject.GetComponentInChildren<InteriorOverride>();
			if (!interior)
			{
				continue;
			}
			if (item.ContextType == HighlightHudCommand.ContextType.Door)
			{
				if (interior.m_ActiveDoors.Count <= item.IndexToReference)
				{
					continue;
				}
				InterfaceableObject io2 = interior.m_ActiveDoors[item.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
				if (io2 != null)
				{
					if (Deactivate)
					{
						io2.IgnoreTutorialLock = false;
						continue;
					}
					io2.IgnoreTutorialLock = true;
					io2.Activate();
				}
			}
			else
			{
				if (interior.m_ActiveWindows.Count <= item.IndexToReference)
				{
					continue;
				}
				InterfaceableObject io = interior.m_ActiveWindows[item.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
				if (io != null)
				{
					if (Deactivate)
					{
						io.IgnoreTutorialLock = false;
						continue;
					}
					io.IgnoreTutorialLock = true;
					io.Activate();
				}
			}
		}
	}
}
