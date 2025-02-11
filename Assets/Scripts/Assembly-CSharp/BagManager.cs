using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class BagManager
{
	public enum ThumbDir
	{
		kFront = 0,
		kBack = 1
	}

	public static string kRootPath = "Assets/Scenes/Bags";

	public static string kThumbsPath = "Assets/Editor/Thumbs";

	public static string kExternalThumbsPath = "\\\\uklbanas01\\files\\Dev\\Corona\\BagThumbs";

	public string m_rootPath;

	public string m_thumbsPath;

	public List<BagTheme> m_themes;

	public List<string> m_LockedList = new List<string>();

	public List<string> m_PrefabsNotInBags = new List<string>();

	public ThumbDir m_ThumbDirection;

	private static BagManager m_Instance;

	public static BagManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new BagManager();
			}
			return m_Instance;
		}
	}

	private BagManager()
	{
		m_rootPath = kRootPath;
		m_thumbsPath = kThumbsPath;
		m_themes = new List<BagTheme>();
		Refresh(false);
	}

	public static bool IsPrefabInABag(GameObject obj)
	{
		return false;
	}

	public bool IsInMission()
	{
		return false;
	}

	public bool IsInBag()
	{
		return false;
	}

	public void Refresh(bool CheckLocks)
	{
		BuildThemeList(CheckLocks);
	}

	public string GetBagScenefile(string ReferenceID)
	{
		string text = m_rootPath + "/" + ReferenceID;
		int length = text.LastIndexOf("/");
		text = text.Substring(0, length);
		return text + ".unity";
	}

	public string GetBagPath(int themeIndex, int typeIndex, int categoryIndex)
	{
		if (themeIndex < m_themes.Count)
		{
			BagTheme bagTheme = m_themes[themeIndex];
			if (typeIndex < bagTheme.m_types.Count)
			{
				BagType bagType = bagTheme.m_types[typeIndex];
				if (categoryIndex < bagType.m_categories.Count)
				{
					BagCategory bagCategory = bagType.m_categories[categoryIndex];
					return m_rootPath + "/" + bagCategory.m_path + "/" + bagCategory.m_name;
				}
			}
		}
		return string.Empty;
	}

	public string GetThumbPath(int themeIndex, int typeIndex, int categoryIndex)
	{
		if (themeIndex < m_themes.Count)
		{
			BagTheme bagTheme = m_themes[themeIndex];
			if (typeIndex < bagTheme.m_types.Count)
			{
				BagType bagType = bagTheme.m_types[typeIndex];
				if (categoryIndex < bagType.m_categories.Count)
				{
					BagCategory bagCategory = bagType.m_categories[categoryIndex];
					return m_thumbsPath + "/" + bagCategory.m_path + "/" + bagCategory.m_name;
				}
			}
		}
		return string.Empty;
	}

	public string FindBagReferenceIDForMesh(string MeshName)
	{
		foreach (BagTheme theme in m_themes)
		{
			foreach (BagType type in theme.m_types)
			{
				foreach (BagCategory category in type.m_categories)
				{
					foreach (string @object in category.m_objects)
					{
						string text = @object;
						int num = text.IndexOf('(');
						string text2 = text.Substring(0, num);
						if (num > -1)
						{
							int num2 = text.IndexOf(')');
							if (num2 > num)
							{
								text = text.Substring(num + 1, num2 - num - 1);
							}
						}
						if (text == MeshName)
						{
							return theme.m_name + "/" + type.m_name + "/" + category.m_name + "/" + text2;
						}
					}
				}
			}
		}
		return string.Empty;
	}

	public BagObject.BagType GetBagObjectType(string objectName)
	{
		if (objectName.Contains("(Prefab)") && !objectName.Contains("Prefab))"))
		{
			return BagObject.BagType.Prefab;
		}
		if (objectName.Contains("(Template)") && !objectName.Contains("Template))"))
		{
			return BagObject.BagType.Template;
		}
		return BagObject.BagType.Default;
	}

	public string GetPrefabPath(int themeIndex, int typeIndex, int categoryIndex, string objectName)
	{
		string text = objectName;
		int num = text.IndexOf('(');
		if (num > -1)
		{
			text = text.Substring(0, num);
		}
		if (themeIndex < m_themes.Count)
		{
			BagTheme bagTheme = m_themes[themeIndex];
			if (typeIndex < bagTheme.m_types.Count)
			{
				BagType bagType = bagTheme.m_types[typeIndex];
				return m_rootPath + "/" + bagType.m_path + "/" + bagType.m_name + "/" + text + ".prefab";
			}
		}
		return string.Empty;
	}

	public string GetReferenceID(int themeIndex, int typeIndex, int categoryIndex, string objectName)
	{
		string text = objectName;
		int num = text.IndexOf('(');
		if (num > -1)
		{
			text = text.Substring(0, num);
		}
		if (themeIndex < m_themes.Count)
		{
			BagTheme bagTheme = m_themes[themeIndex];
			if (typeIndex < bagTheme.m_types.Count)
			{
				BagType bagType = bagTheme.m_types[typeIndex];
				if (categoryIndex < bagType.m_categories.Count)
				{
					BagCategory bagCategory = bagType.m_categories[categoryIndex];
					return bagCategory.m_path + "/" + bagCategory.m_name + "/" + text;
				}
			}
		}
		return string.Empty;
	}

	public string GetThumbnailPath()
	{
		return string.Empty;
	}

	public string GetExternalThumbnailPath()
	{
		return string.Empty;
	}

	public void FillThemeNameList(List<string> themes)
	{
		themes.Clear();
		foreach (BagTheme theme in m_themes)
		{
			themes.Add(theme.m_name);
		}
	}

	public void FillTypeNameList(List<string> types, int themeIndex)
	{
		types.Clear();
		if (themeIndex >= m_themes.Count)
		{
			return;
		}
		BagTheme bagTheme = m_themes[themeIndex];
		foreach (BagType type in bagTheme.m_types)
		{
			types.Add(type.m_name);
		}
	}

	public void FillCategoryNameList(List<string> categories, int themeIndex, int typeIndex)
	{
		categories.Clear();
		if (themeIndex >= m_themes.Count)
		{
			return;
		}
		BagTheme bagTheme = m_themes[themeIndex];
		if (typeIndex >= bagTheme.m_types.Count)
		{
			return;
		}
		BagType bagType = bagTheme.m_types[typeIndex];
		foreach (BagCategory category in bagType.m_categories)
		{
			categories.Add(category.m_name);
		}
	}

	private void BuildThemeList(bool checkLocks)
	{
		if (checkLocks)
		{
			m_LockedList.Clear();
		}
		m_themes.Clear();
		List<string> list = new List<string>();
		GetDirectories(m_rootPath, list);
		foreach (string item in list)
		{
			BagTheme bagTheme = new BagTheme(item);
			BuildTypeList(bagTheme, checkLocks);
			m_themes.Add(bagTheme);
		}
	}

	private void BuildTypeList(BagTheme theme, bool checkLocks)
	{
		List<string> list = new List<string>();
		GetDirectories(m_rootPath + "/" + theme.m_name, list);
		foreach (string item in list)
		{
			BagType bagType = new BagType(item, theme.m_name);
			BuildCategoryList(bagType, checkLocks);
			theme.m_types.Add(bagType);
		}
	}

	private void BuildCategoryList(BagType bagType, bool checkLocks)
	{
		string[] files = Directory.GetFiles(m_rootPath + "/" + bagType.m_path + "/" + bagType.m_name);
		string[] array = files;
		foreach (string text in array)
		{
			string text2 = text;
			if (text2.Contains(".unity") && !text2.Contains(".meta"))
			{
				text2 = text2.Replace("\\", "/");
				text2 = text2.Substring(text2.LastIndexOf('/') + 1);
				text2 = text2.Replace(".unity", string.Empty);
				BagCategory bagCategory = new BagCategory(text2, bagType.m_path + "/" + bagType.m_name);
				bagType.m_categories.Add(bagCategory);
				if (checkLocks && !SvnIntegration.m_DontUseSVN && SvnIntegration.Instance.IsFileLockedByMe(text))
				{
					string item = text.Replace('\\', '/');
					m_LockedList.Add(item);
				}
				BuildObjectList(bagType, bagCategory);
			}
		}
	}

	private void BuildObjectList(BagType bagType, BagCategory bagCategory)
	{
		string path = m_thumbsPath + "/" + bagType.m_path + "/" + bagType.m_name + "/" + bagCategory.m_name;
		if (!Directory.Exists(path))
		{
			return;
		}
		string[] files = Directory.GetFiles(path);
		string[] array = files;
		foreach (string text in array)
		{
			string text2 = text;
			if (!text2.Contains(".meta"))
			{
				int num = text2.LastIndexOf("\\") + 1;
				if (num > 0)
				{
					text2 = text2.Substring(num, text2.Length - num);
				}
				text2 = text2.Replace(".png", string.Empty);
				bagCategory.m_objects.Add(text2);
			}
		}
	}

	private void GetDirectories(string dirPath, List<string> list)
	{
		list.Clear();
		DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
		if (directoryInfo != null)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				list.Add(directoryInfo2.Name);
			}
		}
	}
}
