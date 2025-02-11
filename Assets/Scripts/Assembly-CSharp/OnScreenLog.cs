using System;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
	public int MaxLines = 15;

	public SpriteText TestLabel;

	private List<string> mLines;

	private bool mDirty;

	private static OnScreenLog smInstance;

	public static OnScreenLog Instance
	{
		get
		{
			return smInstance;
		}
	}

	public void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple OnScreenLogs");
		}
		smInstance = this;
	}

	private void Start()
	{
		mDirty = true;
		mLines = new List<string>();
		if (TestLabel == null)
		{
			TestLabel = base.gameObject.GetComponent<SpriteText>();
		}
		if (TestLabel != null)
		{
			TestLabel.Text = string.Empty;
		}
	}

	private void Update()
	{
		if (TestLabel != null && mDirty)
		{
			TestLabel.Text = string.Empty;
			foreach (string mLine in mLines)
			{
				SpriteText testLabel = TestLabel;
				testLabel.Text = testLabel.Text + mLine + "\n";
			}
		}
		while (mLines.Count > MaxLines)
		{
			mDirty = true;
			mLines.RemoveAt(0);
		}
	}

	public void Clear()
	{
		mDirty = true;
		mLines.Clear();
	}

	public void AddLine(string text)
	{
		mDirty = true;
		mLines.Add(text);
	}

	public void AppendToLine(string text)
	{
		if (mLines.Count > 0)
		{
			mDirty = true;
			List<string> list;
			List<string> list2 = (list = mLines);
			int index;
			int index2 = (index = mLines.Count - 1);
			string text2 = list[index];
			list2[index2] = text2 + text;
		}
	}
}
