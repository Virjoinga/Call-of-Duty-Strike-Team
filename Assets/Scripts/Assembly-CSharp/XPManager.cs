using System.Collections.Generic;
using UnityEngine;

public class XPManager : SingletonMonoBehaviour, iSwrveUpdatable
{
	public XPAwards m_XPAwards;

	public XPMissionsAndObjectives m_XPMissionsAndObjectives;

	public List<XPLevel> m_XPLevels;

	public int m_MaxPrestigeLevels = 5;

	public int m_XPMultiplier = 1;

	public int XPLevelForVeteranModeUnlock = 10;

	private EventLog m_XPLog = new EventLog();

	private bool m_CanPrestige = true;

	public static XPManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<XPManager>();
		}
	}

	public EventLog Log()
	{
		return m_XPLog;
	}

	protected override void Awake()
	{
		Object.DontDestroyOnLoad(this);
		base.Awake();
	}

	private int GetThresholdForLevel(int level)
	{
		return m_XPLevels[level].m_Threshold;
	}

	public int MaxXP()
	{
		return GetThresholdForLevel(m_XPLevels.Count - 1);
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	public void ConvertXPToLevel(int xp, out int level, out int prestigeLevel, out int xpToNextLevel, out float percent)
	{
		int count = m_XPLevels.Count;
		int num = MaxXP();
		prestigeLevel = 0;
		if (m_CanPrestige)
		{
			while (xp >= num && prestigeLevel < m_MaxPrestigeLevels)
			{
				prestigeLevel++;
				xp -= num;
			}
		}
		if (xp >= num)
		{
			level = count - 1;
			xpToNextLevel = 0;
			percent = 1f;
			return;
		}
		for (int i = 0; i < count - 1; i++)
		{
			if (xp >= GetThresholdForLevel(i) && xp < GetThresholdForLevel(i + 1))
			{
				level = i + 1;
				xpToNextLevel = GetThresholdForLevel(i + 1) - xp;
				float num2 = GetThresholdForLevel(i + 1) - GetThresholdForLevel(i);
				float num3 = xp - GetThresholdForLevel(i);
				percent = num3 / num2;
				return;
			}
		}
		level = 0;
		xpToNextLevel = 0;
		percent = 0f;
	}

	public int AdjustXPByMultiplier(int xp)
	{
		return xp * m_XPMultiplier;
	}

	public string XPLevelName(int xpLevel)
	{
		return AutoLocalize.Get("S_XPLEVEL_" + xpLevel);
	}

	public int GetXPLevel()
	{
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		return level;
	}

	public int GetXPLevelAbsolute()
	{
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		return ConvertXPLevelToAbsolute(level, prestigeLevel);
	}

	public int ConvertXPLevelToAbsolute(int xpLevel, int xpPrestigeLevel)
	{
		return xpLevel + xpPrestigeLevel * (m_XPLevels.Count - 1);
	}

	public void UpdateFromSwrve()
	{
		m_XPMultiplier = Bedrock.GetRemoteVariableAsInt("xpNewMult", m_XPMultiplier);
	}
}
