using System.Collections.Generic;
using UnityEngine;

public class PickupManager : SingletonMonoBehaviour, ISaveLoad
{
	private Dictionary<string, int> m_PickedUp = new Dictionary<string, int>();

	public static PickupManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<PickupManager>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<PickupManager>() != null;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void Start()
	{
	}

	public void Reset()
	{
		m_PickedUp.Clear();
		for (int i = 0; i < MissionListings.Instance.Missions.Length; i++)
		{
			MissionData missionData = MissionListings.Instance.Missions[i];
			if (!missionData.IncludeInStats)
			{
				continue;
			}
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				SectionData sectionData = missionData.Sections[j];
				if (!sectionData.IsSpecOps && sectionData.IsValidInCurrentBuild)
				{
					string key = CreateKey(missionData.MissionId, j);
					m_PickedUp[key] = 0;
				}
			}
		}
	}

	public string CreateKey(MissionListings.eMissionID missionId, int section)
	{
		return "Pickup." + missionId.ToString() + "." + section;
	}

	public void RegisterPickup(MissionListings.eMissionID missionId, int section, int index)
	{
		string text = CreateKey(missionId, section);
		int num = 1 << index;
		if (!m_PickedUp.ContainsKey(text))
		{
			m_PickedUp.Add(text, 0);
		}
		Dictionary<string, int> pickedUp;
		Dictionary<string, int> dictionary = (pickedUp = m_PickedUp);
		string key;
		string key2 = (key = text);
		int num2 = pickedUp[key];
		dictionary[key2] = num2 | num;
	}

	public bool BeenPickedUp(MissionListings.eMissionID missionId, int section, int index)
	{
		string key = CreateKey(missionId, section);
		int num = 1 << index;
		if (m_PickedUp.ContainsKey(key))
		{
			return (m_PickedUp[key] & num) != 0;
		}
		return false;
	}

	public int NumBeenPickedUp(MissionListings.eMissionID missionId, int section, int numIntel)
	{
		int num = 0;
		string key = CreateKey(missionId, section);
		int value = 0;
		if (m_PickedUp.TryGetValue(key, out value))
		{
			for (int i = 0; i < numIntel; i++)
			{
				int num2 = 1 << i;
				if ((value & num2) != 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void Save()
	{
		foreach (KeyValuePair<string, int> item in m_PickedUp)
		{
			SecureStorage.Instance.SetInt(item.Key, item.Value);
		}
	}

	public void Load()
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(m_PickedUp);
		foreach (KeyValuePair<string, int> item in list)
		{
			m_PickedUp[item.Key] = SecureStorage.Instance.GetInt(item.Key, item.Value);
		}
	}
}
