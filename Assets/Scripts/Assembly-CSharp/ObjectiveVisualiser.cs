using System.Collections.Generic;
using UnityEngine;

public class ObjectiveVisualiser : MonoBehaviour
{
	public static ObjectiveVisualiser instance;

	private ObjectiveManager mObjectiveManager;

	private HashSet<MissionObjective> mExpanded = new HashSet<MissionObjective>();

	private bool mHidden;

	public static ObjectiveVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
		mObjectiveManager = Object.FindObjectOfType(typeof(ObjectiveManager)) as ObjectiveManager;
	}

	private void Update()
	{
	}

	private void PopulateObjectivesList()
	{
		mExpanded.Clear();
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		string text = "Objectives Visualiser";
		int num = 600;
		int num2 = 640;
		Rect screenRect = new Rect(Screen.width - num2 - 20, Screen.height - (num + 20), num2, num);
		GUILayout.BeginArea(screenRect, text);
		GUILayout.BeginVertical("Objectives", GUI.skin.window);
		if (mObjectiveManager == null && GlobalObjectiveManager.Instance != null && GlobalObjectiveManager.Instance.CurrentObjectiveManager != null)
		{
			mObjectiveManager = GlobalObjectiveManager.Instance.CurrentObjectiveManager;
		}
		if (mObjectiveManager != null && mObjectiveManager.Objectives != null)
		{
			foreach (MissionObjective objective in mObjectiveManager.Objectives)
			{
				ComboObjective comboObjective = objective as ComboObjective;
				if (comboObjective != null)
				{
					if (DrawComboObjective(comboObjective))
					{
						break;
					}
				}
				else if (DrawObjective(objective))
				{
					break;
				}
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private bool DrawComboObjective(ComboObjective co)
	{
		bool result = false;
		bool flag = false;
		GUILayout.BeginHorizontal();
		GUILayout.Label(((!co.m_Interface.IsPrimaryObjective) ? string.Empty : "*P* ") + AutoLocalize.Get(co.m_Interface.ObjectiveLabel) + "-" + co.GetType().ToString() + "-" + co.State);
		if (mExpanded.Contains(co))
		{
			flag = true;
			if (GUILayout.Button("-"))
			{
				mExpanded.Remove(co);
			}
		}
		else if (GUILayout.Button("+"))
		{
			mExpanded.Add(co);
		}
		GUILayout.EndHorizontal();
		if (flag)
		{
			foreach (MissionObjective subObjective in co.SubObjectives)
			{
				ComboObjective comboObjective = subObjective as ComboObjective;
				if (comboObjective != null)
				{
					if (DrawComboObjective(comboObjective))
					{
						result = true;
					}
					else if (DrawObjective(subObjective))
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	private bool DrawObjective(MissionObjective mo)
	{
		bool result = false;
		GUILayout.BeginHorizontal();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = Color.gray;
		if (mo != null)
		{
			if (mo.m_Interface.IsVisible)
			{
				GUILayout.Label(((!mo.m_Interface.IsPrimaryObjective) ? string.Empty : "*P* ") + AutoLocalize.Get(mo.m_Interface.ObjectiveLabel) + "-" + mo.GetType().ToString() + "-" + mo.State);
			}
			else
			{
				GUILayout.Label(((!mo.m_Interface.IsPrimaryObjective) ? string.Empty : "*P* ") + AutoLocalize.Get(mo.m_Interface.ObjectiveLabel) + "-" + mo.GetType().ToString() + "-" + mo.State, gUIStyle);
			}
			if (mo.State == MissionObjective.ObjectiveState.InProgress)
			{
				if (GUILayout.Button("Pass"))
				{
					mo.Pass();
					result = true;
				}
				if (GUILayout.Button("Fail"))
				{
					mo.Fail();
					result = true;
				}
			}
		}
		GUILayout.EndHorizontal();
		return result;
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
		PopulateObjectivesList();
	}

	public void Toggle()
	{
		mHidden = !mHidden;
		if (!mHidden)
		{
			PopulateObjectivesList();
		}
	}

	public void GLDebugVisualise()
	{
		if (!mHidden)
		{
		}
	}
}
