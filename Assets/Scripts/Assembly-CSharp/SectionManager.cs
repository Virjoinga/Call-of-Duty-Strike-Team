using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
	public List<SectionItem> m_Sections = new List<SectionItem>();

	public List<SectionLogic> m_SectionLogics = new List<SectionLogic>();

	public string m_MissionPath = string.Empty;

	public string m_MissonName = string.Empty;

	public bool IsGMG;

	public List<string> m_SharedLayers = new List<string>();

	private static SectionManager m_SectionManagerInstance;

	public List<SectionIdentifier> m_SectionIdents = new List<SectionIdentifier>();

	private bool m_bSectionActivated;

	private static bool m_bSectionManagerTested;

	private AsyncOperation m_AsyncOperation;

	public int LastSection
	{
		get
		{
			return m_Sections.Count - 1;
		}
	}

	public bool SectionActivated
	{
		get
		{
			return m_bSectionActivated;
		}
	}

	public void OnDestroy()
	{
		if (GetSectionManager() == this)
		{
			m_bSectionManagerTested = false;
		}
		m_SectionManagerInstance = null;
	}

	public static SectionManager GetSectionManager()
	{
		if (Application.isPlaying)
		{
			if (!m_bSectionManagerTested)
			{
				if (m_SectionManagerInstance == null)
				{
					Object[] array = Object.FindObjectsOfType(typeof(SectionManager));
					int num = 0;
					if (num < array.Length)
					{
						SectionManager sectionManagerInstance = (SectionManager)array[num];
						m_SectionManagerInstance = sectionManagerInstance;
					}
				}
				m_bSectionManagerTested = true;
			}
			return m_SectionManagerInstance;
		}
		Object[] array2 = Object.FindObjectsOfType(typeof(SectionManager));
		int num2 = 0;
		if (num2 < array2.Length)
		{
			return (SectionManager)array2[num2];
		}
		return null;
	}

	public void PreGameBake()
	{
	}

	public void RegisterSection(SectionIdentifier sectionIdent)
	{
		m_SectionIdents.Add(sectionIdent);
		BuildInfoDisplay buildInfoDisplay = Object.FindObjectOfType(typeof(BuildInfoDisplay)) as BuildInfoDisplay;
		if (buildInfoDisplay != null)
		{
			buildInfoDisplay.LoadedSection = GetCurrentSection();
			buildInfoDisplay.Refresh();
		}
	}

	public string GetCurrentSection()
	{
		if (m_SectionIdents != null && m_SectionIdents.Count > 0)
		{
			int index = m_SectionIdents[m_SectionIdents.Count - 1].m_Index;
			string sectionFile = m_Sections[index].m_SectionFile;
			if (sectionFile.Length > 0)
			{
				int num = sectionFile.LastIndexOf("/") + 1;
				sectionFile = sectionFile.Substring(num, sectionFile.Length - num);
				sectionFile = sectionFile.Replace("_Master", string.Empty);
				sectionFile = sectionFile.Replace("_Baked", string.Empty);
				return sectionFile.Replace(".unity", string.Empty);
			}
		}
		return string.Empty;
	}

	public void RegisterSectionLogic(SectionLogic sectionLogic)
	{
		m_SectionLogics.Add(sectionLogic);
	}

	public void ImportAllSections()
	{
	}

	public void ImportSection(int index)
	{
	}

	public void UnloadSection(int index)
	{
	}

	public void UnloadExternalSections()
	{
	}

	public LevelLayer GetLoadedLayer(string name)
	{
		return null;
	}

	private void Start()
	{
		if (GetSectionManager() != this)
		{
			Object.Destroy(base.gameObject);
		}
		else if (SceneLoader.DoTeleport)
		{
			SceneLoader.DoTeleport = false;
			LoadNextSection(SceneLoader.TeleportID);
			SceneLoader.IsTeleporting = true;
			StartCoroutine(WaitForAsync(5f));
			MissionDescriptor instance = MissionDescriptor.Instance;
			if (instance != null)
			{
				instance.m_PlayIntroSequence = false;
			}
		}
		else
		{
			m_bSectionActivated = true;
			SceneLoader.IsTeleporting = false;
			NotifySectionLogic();
		}
	}

	public IEnumerator WaitForAsync(float waitForSecs)
	{
		yield return m_AsyncOperation;
		yield return new WaitForSeconds(waitForSecs);
		ActivateSection();
		SceneLoader.IsTeleporting = false;
	}

	public void LoadNextSection(int sectionID)
	{
		if (SectionLoader.SectionLoadTriggered)
		{
			return;
		}
		SectionItem sectionItem = null;
		int num = 0;
		if (sectionID == -1)
		{
			SectionIdentifier sectionIdentifier = Object.FindObjectOfType(typeof(SectionIdentifier)) as SectionIdentifier;
			if (sectionIdentifier != null)
			{
				num = sectionIdentifier.m_Index;
			}
			num++;
			if (num < m_Sections.Count)
			{
				sectionItem = m_Sections[num];
			}
		}
		else if (sectionID < m_Sections.Count)
		{
			sectionItem = m_Sections[sectionID];
		}
		if (sectionItem != null && sectionItem.m_levelManager != null)
		{
			string text = sectionItem.m_SectionFile;
			if (!Application.isEditor)
			{
				text = text.Replace("_Master", "_Baked");
			}
			SectionLoader.AsyncLoadSceneWithLoadingScreen(text);
		}
	}

	public static int GetSectionIndex()
	{
		SectionIdentifier sectionIdentifier = Object.FindObjectOfType(typeof(SectionIdentifier)) as SectionIdentifier;
		if (sectionIdentifier != null)
		{
			return sectionIdentifier.m_Index;
		}
		return -1;
	}

	public static void DestroySectionGameObjects()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.IdentsInUse);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != null)
			{
				if (!a.behaviour.PlayerControlled)
				{
					Object.Destroy(a.gameObject);
				}
				else
				{
					Debug.Log("Keeping actor through section: " + a.name);
				}
			}
		}
		TaskDead.ClearTheDeadBetweenSections();
	}

	public void MarkObjectsToKeep()
	{
	}

	public void ActivateSection()
	{
	}

	public void NotifySectionLogic()
	{
		int sectionIndex = GetSectionIndex();
		foreach (SectionLogic sectionLogic in m_SectionLogics)
		{
			sectionLogic.StartSection(sectionIndex);
		}
	}

	public void ApplyToPrefab()
	{
	}

	public void SetupNewMission(string path)
	{
		m_MissionPath = path;
	}

	public void CreateSharedLayer(string layerName)
	{
	}

	public void AddSharedLayer(string scenePath)
	{
	}

	public void AddNewSection(string sectionName, string missionName)
	{
	}

	public LevelManager CreateLevelMananger(string sectionName)
	{
		return null;
	}

	public SectionManager CreateSectionManager(string missionName)
	{
		return null;
	}

	private void SetupMissionPath()
	{
	}
}
