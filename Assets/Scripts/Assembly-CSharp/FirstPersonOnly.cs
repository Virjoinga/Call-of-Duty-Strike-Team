using System.Collections.Generic;
using UnityEngine;

public class FirstPersonOnly : MonoBehaviour
{
	private static List<GameObject> ObjectList = new List<GameObject>();

	private static bool mCurrentShowHide = true;

	public static void ShowHideFPPObjects(bool show)
	{
		if (show == mCurrentShowHide)
		{
			return;
		}
		for (int i = 0; i < ObjectList.Count; i++)
		{
			if (ObjectList[i] != null)
			{
				ObjectList[i].SetActive(show);
			}
		}
		mCurrentShowHide = show;
	}

	public static void ClearList()
	{
		if (ObjectList != null)
		{
			ObjectList.Clear();
		}
		mCurrentShowHide = true;
	}

	private void Start()
	{
		ObjectList.Add(base.gameObject);
	}
}
