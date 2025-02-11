using System.Collections.Generic;
using UnityEngine;

public class MulticontainerInstance : MonoBehaviour
{
	public List<ContainerLink> ContainerLinks;

	private void Awake()
	{
	}

	public void Apply()
	{
		Reset();
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			Transform transform = skinnedMeshRenderer.bones[0];
			while (transform.parent.name.Contains("Bip"))
			{
				transform = transform.parent;
			}
			Debug.Log("The root bone is " + transform.name + " for " + skinnedMeshRenderer.name);
			skinnedMeshRenderer.transform.parent = transform.parent;
		}
		foreach (ContainerLink containerLink in ContainerLinks)
		{
			GameObject source = containerLink.Source;
			if (source != null)
			{
				Container container = source.gameObject.AddComponent<Container>();
				container.ReferenceId = containerLink.ReferenceId;
				container.KeepTransform = false;
				container.m_Type = containerLink.Type;
			}
		}
	}

	private void Reset()
	{
		Container[] componentsInChildren = base.gameObject.GetComponentsInChildren<Container>();
		foreach (Container obj in componentsInChildren)
		{
			Object.DestroyImmediate(obj);
		}
	}
}
