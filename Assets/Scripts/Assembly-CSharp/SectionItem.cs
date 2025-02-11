using System;

[Serializable]
public class SectionItem
{
	public string m_SectionFile = string.Empty;

	public LevelManager m_levelManager;

	public int m_IntelCount;

	public string GetSectionName()
	{
		return m_SectionFile;
	}
}
