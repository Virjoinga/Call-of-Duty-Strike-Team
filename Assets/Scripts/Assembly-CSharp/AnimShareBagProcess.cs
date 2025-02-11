using System.Collections.Generic;
using UnityEngine;

public class AnimShareBagProcess : BaseBagProcess
{
	public GameObject m_SourceObject;

	public GameObject m_DestObject;

	public List<AnimationClip> m_SourceAnims = new List<AnimationClip>();

	public bool m_CreatNewAnims;

	public override void ApplyProcess(GameObject obj)
	{
	}

	private void RenameMatchingPath(string sourcePath, GameObject sourceObject, GameObject targetObject)
	{
		int num = 0;
		Transform[] componentsInChildren = sourceObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.gameObject.name == sourcePath)
			{
				break;
			}
			num++;
		}
		int num2 = 0;
		Transform[] componentsInChildren2 = targetObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform2 in componentsInChildren2)
		{
			if (num2 == num)
			{
				transform2.gameObject.isStatic = false;
				transform2.gameObject.name = sourcePath;
				break;
			}
			num2++;
		}
	}
}
