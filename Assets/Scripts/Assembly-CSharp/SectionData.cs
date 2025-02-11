using System;

[Serializable]
public class SectionData
{
	public string SceneName;

	public string Name;

	public string Description;

	public int IntelToCollect;

	public int LeaderboardID;

	public int UnlockedAtXpLevel = -1;

	public int UnlockEarlyCost = 250;

	public bool Locked;

	public bool IsTutorial;

	public bool IsSpecOps;

	public bool IsVTOL;

	public bool AllowInMaster = true;

	public GMGData.GameType GMGGameType = GMGData.GameType.Total;

	public MissionListings.FlashpointData.Objective FlashpointObjective;

	public static int numMedalSlots = 5;

	public MedalData[] Medals = new MedalData[numMedalSlots];

	public StatisticsPanel.Type ImageType;

	public bool IsValidInCurrentBuild
	{
		get
		{
			if (GMGGameType == GMGData.GameType.TimeAttack || GMGGameType == GMGData.GameType.Domination)
			{
				return false;
			}
			return AllowInMaster;
		}
	}
}
