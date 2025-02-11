using System.Collections.Generic;
using UnityEngine;

public class RequiredScenes : ScriptableObject
{
	public static RequiredScenes Instance;

	public List<string> DebugRequiredSceneList = new List<string>();

	public List<string> RequiredSceneList = new List<string>();

	public List<string> AndroidSceneList = new List<string>();

	public static List<string> GetRequiredScenes()
	{
		if (Instance == null)
		{
			LoadAsset();
			if (!(Instance == null))
			{
				return Instance.AndroidSceneList;
			}
			Debug.Log("Can't find required scene list");
		}
		return null;
	}

	private static void LoadAsset()
	{
		Instance = Resources.Load("RequiredScenes") as RequiredScenes;
		Object.DontDestroyOnLoad(Instance);
	}
}
