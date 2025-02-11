using System.Collections.Generic;
using UnityEngine;

public class ThemedModelDescriptor : ScriptableObject
{
	public List<GameObject> Default;

	public List<string> DefaultModelStrings;

	public List<ThemeModel> ThemeModels;

	private static List<ThemedModelDescriptor> TmdGameList = new List<ThemedModelDescriptor>();

	public static void ClearThemeModelDescGameList()
	{
		foreach (ThemedModelDescriptor tmdGame in TmdGameList)
		{
			for (int i = 0; i < tmdGame.Default.Count; i++)
			{
				if (tmdGame.DefaultModelStrings[i] != null)
				{
					tmdGame.Default[i] = null;
				}
			}
			foreach (ThemeModel themeModel in tmdGame.ThemeModels)
			{
				for (int j = 0; j < themeModel.Model.Count; j++)
				{
					if (themeModel.ModelStrings[j] != null)
					{
						themeModel.Model[j] = null;
					}
				}
			}
		}
		TmdGameList.Clear();
	}

	public GameObject GetModelForTheme(string theme)
	{
		ThemeModel themeModel = ThemeModels.Find((ThemeModel obj) => obj.Theme == theme);
		LoadThemeModels(themeModel);
		return PickRandom((themeModel == null || themeModel.Model.Count <= 0) ? Default : themeModel.Model);
	}

	public GameObject GetModelForTheme(string theme, int seed)
	{
		ThemeModel themeModel = ThemeModels.Find((ThemeModel obj) => obj.Theme == theme);
		LoadThemeModels(themeModel);
		return PickSpecific((themeModel == null || themeModel.Model.Count <= 0) ? Default : themeModel.Model, seed);
	}

	private GameObject PickSpecific(List<GameObject> list, int seed)
	{
		return (list.Count <= 0) ? null : list[seed % list.Count];
	}

	private GameObject PickRandom(List<GameObject> list)
	{
		return (list.Count <= 0) ? null : list[Random.Range(0, list.Count)];
	}

	public ThemeModel GetThemeModels(string theme)
	{
		return ThemeModels.Find((ThemeModel obj) => obj.Theme == theme);
	}

	private void LoadThemeModels(ThemeModel found)
	{
		if (found != null && found.ModelStrings.Count > 0)
		{
			for (int i = 0; i < found.ModelStrings.Count; i++)
			{
				if (found.Model[i] == null)
				{
					BufferObject bufferObject = BufferObjectHelper.LoadBufferObject(found.ModelStrings[i]);
					if (bufferObject != null)
					{
						found.Model[i] = bufferObject.GetGameObjects()[0];
					}
				}
			}
			if (!TmdGameList.Contains(this))
			{
				TmdGameList.Add(this);
			}
		}
		else
		{
			if (found != null || DefaultModelStrings.Count <= 0)
			{
				return;
			}
			for (int j = 0; j < DefaultModelStrings.Count; j++)
			{
				if (Default[j] == null)
				{
					BufferObject bufferObject2 = BufferObjectHelper.LoadBufferObject(DefaultModelStrings[j]);
					if (bufferObject2 != null)
					{
						Default[j] = bufferObject2.GetGameObjects()[0];
					}
				}
			}
			if (!TmdGameList.Contains(this))
			{
				TmdGameList.Add(this);
			}
		}
	}
}
