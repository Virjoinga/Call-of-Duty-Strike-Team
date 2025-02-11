using System.Collections.Generic;
using UnityEngine;

public class ScriptingVisualiser : MonoBehaviour
{
	public enum CallType
	{
		All = 0,
		Messaging = 1,
		Routines = 2,
		Tasks = 3,
		Sequences = 4,
		Triggers = 5,
		Variables = 6,
		Objectives = 7,
		Spawning = 8
	}

	private class messageHolder
	{
		public string message = string.Empty;

		public CallType type;

		public int objectID;
	}

	private const int MAX_SCRIPTING_STACK_SIZE = 900;

	private bool fRoutines = true;

	private bool fTasks = true;

	private bool fMessages = true;

	private bool fSequences = true;

	private bool fTriggers = true;

	private bool fVariables = true;

	private bool fObjectives = true;

	private bool fSpawning = true;

	private int visibleCount;

	private bool UseFileLogging;

	public static ScriptingVisualiser instance;

	private bool mHidden;

	private Vector2 mScrollPosition;

	private List<messageHolder> mLogOutput = new List<messageHolder>();

	private int ForceFilterID = -1;

	public static ScriptingVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		mScrollPosition = Vector2.zero;
		mHidden = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		visibleCount = 0;
		for (int i = 0; i < mLogOutput.Count; i++)
		{
			if (CanAdd(i))
			{
				visibleCount++;
			}
		}
		string text = string.Format("Scripting Visualiser ({0}/{1})", visibleCount, mLogOutput.Count);
		Rect screenRect = new Rect(Screen.width - 800, Screen.height - 520, 750f, 500f);
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginHorizontal();
		fMessages = GUILayout.Toggle(fMessages, "Messaging");
		fRoutines = GUILayout.Toggle(fRoutines, "<color=#" + GetColourForCallType(CallType.Routines) + ">Routines</color>");
		fTasks = GUILayout.Toggle(fTasks, "<color=#" + GetColourForCallType(CallType.Tasks) + ">Tasks</color>");
		fSequences = GUILayout.Toggle(fSequences, "<color=#" + GetColourForCallType(CallType.Sequences) + ">Sequences</color>");
		fTriggers = GUILayout.Toggle(fTriggers, "<color=#" + GetColourForCallType(CallType.Triggers) + ">Triggers</color>");
		fVariables = GUILayout.Toggle(fVariables, "<color=#" + GetColourForCallType(CallType.Variables) + ">Variables</color>");
		fObjectives = GUILayout.Toggle(fObjectives, "<color=#" + GetColourForCallType(CallType.Objectives) + ">Objectives</color>");
		fSpawning = GUILayout.Toggle(fSpawning, "<color=#" + GetColourForCallType(CallType.Spawning) + ">Spawning</color>");
		UseFileLogging = GUILayout.Toggle(UseFileLogging, "LogFile");
		GUILayout.EndHorizontal();
		mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.stretchWidth = false;
		gUIStyle.padding = new RectOffset();
		gUIStyle.margin = new RectOffset();
		gUIStyle.richText = true;
		GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.window);
		gUIStyle2.stretchHeight = true;
		gUIStyle2.stretchWidth = true;
		GUIStyle gUIStyle3 = new GUIStyle(gUIStyle);
		gUIStyle3.normal.background = GUI.skin.box.normal.background;
		gUIStyle3.stretchWidth = true;
		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
		bool flag = true;
		GUILayout.BeginVertical(text, gUIStyle2);
		for (int j = 0; j < mLogOutput.Count; j++)
		{
			string message = mLogOutput[j].message;
			if (CanAdd(j) && ForceFilterID != mLogOutput[j].objectID)
			{
				GUIStyle style = ((!flag) ? gUIStyle : gUIStyle3);
				GUILayout.BeginHorizontal();
				GUILayout.Label(message, style);
				GUILayout.EndHorizontal();
				flag = false;
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private bool CanAdd(int i)
	{
		bool result = true;
		switch (mLogOutput[i].type)
		{
		case CallType.Messaging:
			result = fMessages;
			break;
		case CallType.Routines:
			result = fRoutines;
			break;
		case CallType.Tasks:
			result = fTasks;
			break;
		case CallType.Sequences:
			result = fSequences;
			break;
		case CallType.Variables:
			result = fVariables;
			break;
		case CallType.Objectives:
			result = fObjectives;
			break;
		case CallType.Spawning:
			result = fSpawning;
			break;
		}
		return result;
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
	}

	public void Toggle()
	{
		mHidden = !mHidden;
	}

	public void GLDebugVisualise()
	{
		if (!mHidden)
		{
		}
	}

	public void AddLogEntry(string logEntry)
	{
		AddLogEntry(logEntry, CallType.All);
	}

	public string GetColourForCallType(CallType type)
	{
		switch (type)
		{
		case CallType.Routines:
		case CallType.Tasks:
			return "616D7E";
		case CallType.Sequences:
			return "F778A1";
		case CallType.Triggers:
			return "3EA99F";
		case CallType.Variables:
			return "FDD017";
		case CallType.Objectives:
			return "ECD872";
		case CallType.Spawning:
			return "81BEF7";
		default:
			return "FFFFFF";
		}
	}

	public void AddLogEntry(string logEntry, CallType type)
	{
		AddLogEntry(logEntry, type, 0);
	}

	public void AddLogEntry(string logEntry, CallType type, int id)
	{
	}
}
