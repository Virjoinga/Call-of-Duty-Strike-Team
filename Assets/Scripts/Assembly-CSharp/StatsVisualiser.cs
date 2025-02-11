using UnityEngine;

public class StatsVisualiser : MonoBehaviour
{
	public enum OutputLog
	{
		Achievements = 0,
		Leaderboards = 1,
		XP = 2,
		Perks = 3
	}

	public static StatsVisualiser instance;

	private StatsTestBed m_TestBed;

	private OutputLog m_CurrentOutputLog;

	private int buttonXPos = 550;

	private int buttonYPos = 730;

	private int buttonWidth = 100;

	private int buttonHeight = 20;

	private Rect actionsLoad;

	private Rect actionsSave;

	private Rect actionsBeginTest;

	private Rect actionsResetAll;

	private int scrollViewXPos = 80;

	private int scrollViewYPos = 520;

	private int scrollViewWidth = 860;

	private int scrollViewHeight = 200;

	private int genericScrollViewXPos = 80;

	private int genericScrollViewYPos = 304;

	private int genericScrollViewWidth = 634;

	private int genericScrollViewHeight = 200;

	private int searchWidth = 200;

	private int searchHeight = 20;

	private Rect scrollRect;

	private Vector2 scrollPosition = Vector2.zero;

	private Rect genericScrollRect;

	private Vector2 genericScrollPosition = Vector2.zero;

	private Rect actionsAchievementsLog;

	private Rect actionsLeaderboard;

	private Rect actionsXPLog;

	private Rect actionsPerksLog;

	private int lastHeight = -1;

	private string searchFilter1 = string.Empty;

	private string searchFilter2 = string.Empty;

	private bool m_Hidden;

	public static StatsVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		m_Hidden = true;
		m_CurrentOutputLog = OutputLog.Achievements;
		if (VisualiserManager.Instance() == null)
		{
			m_Hidden = false;
		}
		m_TestBed = StatsManager.Instance.gameObject.GetComponent<StatsTestBed>();
		base.gameObject.AddComponent<StatsVisualiserPlayer>();
		base.gameObject.AddComponent<StatsVisualiserPlayerAbility>();
		base.gameObject.AddComponent<StatsVisualiserCharacter>();
		base.gameObject.AddComponent<StatsVisualiserSquad>();
		base.gameObject.AddComponent<StatsVisualiserMission>();
		base.gameObject.AddComponent<StatsVisualiserWeapon>();
		actionsLoad = new Rect(buttonXPos, buttonYPos, buttonWidth, buttonHeight);
		actionsSave = new Rect(buttonXPos + (buttonWidth + 4), buttonYPos, buttonWidth, buttonHeight);
		actionsBeginTest = new Rect(buttonXPos + (buttonWidth + 4) * 2, buttonYPos, buttonWidth, buttonHeight);
		actionsResetAll = new Rect(buttonXPos + (buttonWidth + 4) * 3, buttonYPos, buttonWidth, buttonHeight);
		scrollRect = new Rect(scrollViewXPos, scrollViewYPos, scrollViewWidth - 20, scrollViewHeight);
		genericScrollRect = new Rect(genericScrollViewXPos, genericScrollViewYPos, genericScrollViewWidth - 20, genericScrollViewHeight);
		actionsAchievementsLog = new Rect(genericScrollViewXPos, genericScrollViewYPos - buttonHeight - 4, buttonWidth, buttonHeight);
		actionsLeaderboard = new Rect(genericScrollViewXPos + (buttonWidth + 4), genericScrollViewYPos - buttonHeight - 4, buttonWidth, buttonHeight);
		actionsXPLog = new Rect(genericScrollViewXPos + (buttonWidth + 4) * 2, genericScrollViewYPos - buttonHeight - 4, buttonWidth, buttonHeight);
		actionsPerksLog = new Rect(genericScrollViewXPos + (buttonWidth + 4) * 3, genericScrollViewYPos - buttonHeight - 4, buttonWidth, buttonHeight);
	}

	private void OnGUI()
	{
		if (m_Hidden)
		{
			return;
		}
		if (m_TestBed != null)
		{
			if (GUI.Button(actionsLoad, "Load"))
			{
				SecureStorage.Instance.LoadAllData();
			}
			if (GUI.Button(actionsSave, "Save"))
			{
				SecureStorage.Instance.SaveAllData();
			}
			if (GUI.Button(actionsBeginTest, "Generate random data"))
			{
				m_TestBed.OnStartTest();
			}
			if (GUI.Button(actionsResetAll, "Reset all data"))
			{
				SecureStorage.Instance.ResetAllData();
				SecureStorage.Instance.SaveAllData();
				Events.Log().Clear();
				StatsManager.Instance.LeaderboardManagerInstance.Log().Clear();
			}
		}
		if (GUI.Button(actionsAchievementsLog, "Achievements"))
		{
			m_CurrentOutputLog = OutputLog.Achievements;
		}
		if (GUI.Button(actionsLeaderboard, "Leaderboard"))
		{
			m_CurrentOutputLog = OutputLog.Leaderboards;
		}
		if (GUI.Button(actionsXPLog, "XP"))
		{
			m_CurrentOutputLog = OutputLog.XP;
		}
		if (GUI.Button(actionsPerksLog, "Perks"))
		{
			m_CurrentOutputLog = OutputLog.Perks;
		}
		switch (m_CurrentOutputLog)
		{
		case OutputLog.Achievements:
			LogScrollBox(StatsManager.Instance.AchievementManager().Log());
			break;
		case OutputLog.Leaderboards:
			LogScrollBox(StatsManager.Instance.LeaderboardManagerInstance.Log());
			break;
		case OutputLog.XP:
			LogScrollBox(XPManager.Instance.Log());
			break;
		case OutputLog.Perks:
			LogScrollBox(StatsManager.Instance.PerksManager().Log());
			break;
		}
		EventLogScrollBox();
	}

	private void LogScrollBox(EventLog log)
	{
		GUI.TextField(genericScrollRect, string.Empty);
		GUIStyle style = GUI.skin.GetStyle("label");
		GUILayout.BeginArea(new Rect(genericScrollViewXPos, genericScrollViewYPos, 1000f, 1000f));
		genericScrollPosition = GUILayout.BeginScrollView(genericScrollPosition, GUILayout.Width(genericScrollViewWidth), GUILayout.Height(genericScrollViewHeight));
		string displayString = log.GetDisplayString(string.Empty, string.Empty);
		GUILayout.Label(displayString, style);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void EventLogScrollBox()
	{
		GUI.TextField(scrollRect, string.Empty);
		searchFilter1 = GUI.TextField(new Rect(scrollViewXPos, scrollViewYPos + scrollViewHeight + 10, searchWidth, searchHeight), searchFilter1, 25);
		searchFilter2 = GUI.TextField(new Rect(scrollViewXPos + searchWidth + 20, scrollViewYPos + scrollViewHeight + 10, searchWidth, searchHeight), searchFilter2, 25);
		GUIStyle style = GUI.skin.GetStyle("label");
		GUILayout.BeginArea(new Rect(scrollViewXPos, scrollViewYPos, 1000f, 1000f));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(scrollViewWidth), GUILayout.Height(scrollViewHeight));
		string displayString = Events.Log().GetDisplayString(searchFilter1, searchFilter2);
		GUIContent content = new GUIContent(displayString);
		int num = (int)style.CalcHeight(content, 1000f);
		if (lastHeight != num)
		{
			scrollPosition.y = num - scrollViewHeight;
			lastHeight = num;
		}
		GUILayout.Label(displayString, style);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public void Hide()
	{
		m_Hidden = true;
	}

	public void Show()
	{
		m_Hidden = false;
	}

	public bool Hidden()
	{
		return m_Hidden;
	}

	public void Toggle()
	{
		m_Hidden = !m_Hidden;
		if (m_Hidden)
		{
		}
	}

	public void GLDebugVisualise()
	{
		if (!m_Hidden)
		{
		}
	}
}
