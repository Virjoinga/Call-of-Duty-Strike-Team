using System.Collections.Generic;
using UnityEngine;

public class QuickWaveComponent : MonoBehaviour
{
	public GMGData.GameType GameType;

	public List<WaveInfo> WaveInfo = new List<WaveInfo>();

	public List<GameModeInfo> GameModes;

	public static void DisableAllGameModes_OLD()
	{
		for (int i = 0; i < 6; i++)
		{
			GameObject gameObject = Find(GMGData.GetGeneratedTag((GMGData.GameType)i));
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public static void EnableGameModeInHierachy_OLD(GMGData.GameType gt)
	{
		DisableAllGameModes_OLD();
		GameObject gameObject = Find(GMGData.GetGeneratedTag(gt));
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
		for (int i = 0; i < 6; i++)
		{
			GameObject gameObject2 = Find(GMGData.GetGeneratedTag((GMGData.GameType)i));
			if (gameObject2 != gameObject && gameObject2 != null)
			{
				Object.DestroyImmediate(gameObject2);
			}
		}
	}

	private static GameObject Find(string name)
	{
		Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (Object @object in array)
		{
			if (@object.name == name)
			{
				return @object as GameObject;
			}
		}
		return null;
	}

	private void Awake()
	{
		GMGData.GameType gameType = GameType;
		if ((bool)GMGData.Instance)
		{
			GMGData.Instance.CurrentGameType = gameType;
		}
		EnableGameModeInHierachy_OLD(gameType);
	}
}
