using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableCommand : Command
{
	[HideInInspector]
	public List<GameObject> m_EnableObjects = new List<GameObject>();

	public List<GuidRef> EnableObjects = new List<GuidRef>();

	[HideInInspector]
	public List<GameObject> m_DisableObjects = new List<GameObject>();

	public List<GuidRef> DisableObjects = new List<GuidRef>();

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		foreach (GuidRef enableObject in EnableObjects)
		{
			GameObject obj2 = enableObject;
			if (obj2 != null)
			{
				obj2.SetActive(true);
			}
		}
		foreach (GuidRef disableObject in DisableObjects)
		{
			GameObject obj = disableObject;
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}
		yield break;
	}

	public override void ResolveGuidLinks()
	{
		int num = 0;
		foreach (GameObject enableObject in m_EnableObjects)
		{
			if (enableObject != null)
			{
				if (num >= EnableObjects.Count)
				{
					EnableObjects.Add(new GuidRef());
				}
				EnableObjects[num].SetObjectWithGuid(enableObject);
			}
			num++;
		}
		foreach (GuidRef enableObject2 in EnableObjects)
		{
			enableObject2.ResolveLink();
		}
		num = 0;
		foreach (GameObject disableObject in m_DisableObjects)
		{
			if (disableObject != null)
			{
				if (num >= DisableObjects.Count)
				{
					DisableObjects.Add(new GuidRef());
				}
				DisableObjects[num].SetObjectWithGuid(disableObject);
			}
			num++;
		}
		foreach (GuidRef disableObject2 in DisableObjects)
		{
			disableObject2.ResolveLink();
		}
	}
}
