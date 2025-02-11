using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{
	public const string kDefaultMissionRoot = "Assets/Scenes/Missions/";

	public const string kDefaultGlobalScene = "Global/Global.unity";

	public const int kNumBackupRevisions = 10;

	public List<LayerItem> m_Layers;

	public TransitionData m_TransitionData;

	public LayerItem m_CurrentLayer;

	public static int m_RevisionNumber;

	public static int m_AutosaveNumber;

	public bool m_NeedsUpdateAll;

	public bool m_SaveCombined;

	public bool MarkToKeep { get; set; }

	public LevelManager()
	{
		if (m_Layers == null)
		{
			m_Layers = new List<LayerItem>();
		}
	}

	public static LevelLayer GetMasterLayer()
	{
		return null;
	}

	public static void ReimportOnLoad()
	{
	}

	public void ResetNaveGates()
	{
	}

	public static void ForceImportLayers()
	{
	}

	private void Awake()
	{
	}

	private void Update()
	{
	}

	public void OnSceneLoad()
	{
	}

	public void FixUpLightmaps()
	{
	}

	public void DeepUnlock(bool OnOff)
	{
	}

	public void PostCombineScenes()
	{
		OnSceneLoad_FinaliseCombinedScenes();
	}

	private void OnSceneLoad_FinaliseCombinedScenes()
	{
		SpecularSource specularSource = Object.FindObjectOfType(typeof(SpecularSource)) as SpecularSource;
		if (specularSource != null)
		{
			specularSource.Apply();
		}
	}

	public string ExtractShortSceneName(string SceneName)
	{
		string result = string.Empty;
		int num = SceneName.LastIndexOf('/') + 1;
		int num2 = SceneName.LastIndexOf('.');
		if (num2 > num)
		{
			result = SceneName.Substring(num, num2 - num);
		}
		return result;
	}

	public string ExtractMissionPath(string SceneName)
	{
		return Path.GetDirectoryName(SceneName);
	}

	public string ExtractMissionName(string SceneName)
	{
		string text = ExtractMissionPath(SceneName);
		int num = text.LastIndexOf('/') + 1;
		if (num > 0)
		{
			return text.Substring(num, text.Length - num);
		}
		return string.Empty;
	}

	private static bool IsRootObjectValid(GameObject obj)
	{
		if (obj != null)
		{
			if (obj.GetComponent(typeof(LevelLayer)) != null)
			{
				return true;
			}
			if (obj.GetComponent(typeof(LevelManager)) != null)
			{
				return true;
			}
			if (obj.GetComponent(typeof(SectionManager)) != null)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public void AutoSave()
	{
	}

	public bool CanSave(bool allowCancel)
	{
		return true;
	}

	public static void ClearRootObjects()
	{
	}

	public bool OnSave()
	{
		return false;
	}

	private void SaveLayer(LayerItem layerItem)
	{
	}

	public void SaveExternalLayers()
	{
	}

	public void LoadSceneAdditive(string SceneName)
	{
		Object[] array = Object.FindObjectsOfType(typeof(LevelManager));
		for (int i = 0; i < array.Length; i++)
		{
			LevelManager levelManager = (LevelManager)array[i];
			if (levelManager == this)
			{
				levelManager.MarkToKeep = true;
				break;
			}
		}
		Object[] array2 = Object.FindObjectsOfType(typeof(LevelManager));
		for (int j = 0; j < array2.Length; j++)
		{
			LevelManager levelManager2 = (LevelManager)array2[j];
			if (!levelManager2.MarkToKeep)
			{
				Object.DestroyImmediate(levelManager2.gameObject);
			}
		}
	}

	public void RefreshLayerReferences()
	{
		foreach (LayerItem layer in m_Layers)
		{
			layer.m_layer = null;
		}
		Object[] array = Object.FindObjectsOfType(typeof(LevelLayer));
		for (int i = 0; i < array.Length; i++)
		{
			LevelLayer levelLayer = (LevelLayer)array[i];
			LayerItem layerItem = null;
			if (levelLayer.m_DeleteOnLoad)
			{
				Object.DestroyImmediate(levelLayer.gameObject);
			}
			else
			{
				foreach (LayerItem layer2 in m_Layers)
				{
					string text = ExtractShortSceneName(layer2.m_name);
					if (text == levelLayer.name)
					{
						layer2.m_layer = levelLayer;
						layerItem = layer2;
						break;
					}
				}
			}
			if (layerItem != null)
			{
				levelLayer.m_Type = layerItem.m_type;
			}
		}
	}

	public LevelLayer GetLayer(string SceneName)
	{
		RefreshLayerReferences();
		foreach (LayerItem layer in m_Layers)
		{
			if (layer.m_name == SceneName)
			{
				return layer.m_layer;
			}
		}
		return null;
	}
}
