using System.Collections.Generic;
using UnityEngine;

public class RulesSystemVisualiser : MonoBehaviour
{
	public static RulesSystemVisualiser instance;

	private bool mHidden;

	private Vector2 mScrollPosition;

	private List<string> mLogOutput;

	public static RulesSystemVisualiser Instance()
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
		mLogOutput = new List<string>();
		mHidden = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (!mHidden)
		{
			string text = "Rules System Visualiser";
			Rect rect = new Rect(Screen.width - 800, Screen.height - 520, 750f, 500f);
			GUI.Box(rect, text);
			Rect position = new Rect(rect);
			position.x += 20f;
			position.y += 20f;
			position.width -= 40f;
			position.height -= 40f;
			Rect rect2 = new Rect(rect);
			rect2.y = 0f;
			rect2.width -= 40f;
			rect2.height = 20 + 20 * mLogOutput.Count;
			mScrollPosition = GUI.BeginScrollView(position, mScrollPosition, rect2);
			for (int i = 0; i < mLogOutput.Count; i++)
			{
				Rect position2 = new Rect(rect2);
				position2.y = i * 20;
				position2.height = 20f;
				GUI.Label(position2, mLogOutput[i]);
			}
			GUI.EndScrollView();
		}
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
		mLogOutput.Add(logEntry);
	}
}
