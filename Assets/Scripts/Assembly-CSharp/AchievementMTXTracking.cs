public class AchievementMTXTracking : ISaveLoad
{
	public bool[] m_PerkBoughtInTier = new bool[4];

	public int[] m_TimeOfChallengeMedalEarned = new int[6];

	public void Reset()
	{
		SaveLoadHelper.ResetArray(m_PerkBoughtInTier);
		SaveLoadHelper.ResetArray(m_TimeOfChallengeMedalEarned);
	}

	public void Save()
	{
		SaveLoadHelper.SaveArray("MTXTracking.perk", m_PerkBoughtInTier);
		SaveLoadHelper.SaveArray("MTXTracking.challengemedal", m_TimeOfChallengeMedalEarned);
	}

	public void Load()
	{
		SaveLoadHelper.LoadArray("MTXTracking.perk", m_PerkBoughtInTier);
		SaveLoadHelper.LoadArray("MTXTracking.challengemedal", m_TimeOfChallengeMedalEarned);
	}
}
