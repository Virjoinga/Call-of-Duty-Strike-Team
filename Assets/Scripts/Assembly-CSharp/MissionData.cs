using System;
using System.Collections.Generic;

[Serializable]
public class MissionData
{
	public enum eType
	{
		MT_EMPTY = 0,
		MT_STORY = 1,
		MT_SURVIVAL = 2,
		MT_DEMO = 3,
		MT_EVERYPLAY_Removed = 4,
		MT_KINVITE = 5,
		MT_FLASHPOINT = 6
	}

	public enum eEnvironment
	{
		Arctic = 0,
		Desert = 1,
		Urban = 2
	}

	public enum eSectionImages
	{
		Arctic = 0,
		Afghanistan = 1,
		Kowloon = 2,
		Morocco = 3,
		Pacific = 4
	}

	public string Location;

	public string BaseName;

	public int NumStorySections;

	public List<SectionData> Sections;

	public string NameKey;

	public MissionListings.eMissionID MissionId;

	public eType Type;

	public eEnvironment Environment;

	public eSectionImages SectionSelectImage;

	public bool IncludeInStats;

	public int FlashPointIndex;

	public bool IsValid
	{
		get
		{
			return Sections.Count > 0;
		}
	}

	public MissionData(string scenename, int intel, MissionListings.eMissionID id)
	{
		SectionData item = new SectionData
		{
			SceneName = scenename,
			IntelToCollect = intel
		};
		Sections.Add(item);
		Location = scenename.Split('_')[0];
		NameKey = "S_" + scenename.ToUpper();
		MissionId = id;
		Type = eType.MT_EMPTY;
		FlashPointIndex = -1;
	}

	public MissionData(string scenename, int intel, string location, string titleKey, MissionListings.eMissionID id, eType type)
	{
		SectionData item = new SectionData
		{
			SceneName = scenename,
			IntelToCollect = intel
		};
		Sections.Add(item);
		Location = location;
		NameKey = titleKey;
		MissionId = id;
		Type = type;
		FlashPointIndex = -1;
	}

	public bool HasStoryMissions()
	{
		bool result = false;
		for (int i = 0; i < Sections.Count; i++)
		{
			if (!Sections[i].IsSpecOps)
			{
				result = true;
			}
		}
		return result;
	}

	public int NumValidSections()
	{
		int num = 0;
		for (int i = 0; i < Sections.Count; i++)
		{
			if (Sections[i].IsValidInCurrentBuild)
			{
				num++;
			}
		}
		return num;
	}

	public string GetMissionNameAsString()
	{
		return AutoLocalize.Get(NameKey);
	}

	public int IntelCount()
	{
		int num = 0;
		for (int i = 0; i < Sections.Count; i++)
		{
			num += Sections[i].IntelToCollect;
		}
		return num;
	}

	public bool IsLocked()
	{
		bool result = true;
		for (int i = 0; i < Sections.Count; i++)
		{
			if (!Sections[i].Locked)
			{
				result = false;
				break;
			}
		}
		return result;
	}
}
