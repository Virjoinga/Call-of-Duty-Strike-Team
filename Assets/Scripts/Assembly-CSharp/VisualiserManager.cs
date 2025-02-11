using System.Collections.Generic;
using UnityEngine;

public class VisualiserManager : MonoBehaviour
{
	private class VisualiserWrapper
	{
		public MonoBehaviour Visualiser;

		public bool Open;

		public bool RequiresInput;

		public VisualiserWrapper(MonoBehaviour visualiser, bool openOnStartup, bool requiresInput)
		{
			Visualiser = visualiser;
			Open = openOnStartup;
			RequiresInput = requiresInput;
		}
	}

	public static VisualiserManager instance;

	private static int SIDE_TAB_MAX = 6;

	private Dictionary<string, VisualiserWrapper> mVisualisers = new Dictionary<string, VisualiserWrapper>();

	private float TabWidth
	{
		get
		{
			return 32f;
		}
	}

	private float TabHeight
	{
		get
		{
			return (float)Screen.height / 8f;
		}
	}

	private float TabOffset
	{
		get
		{
			return 16f;
		}
	}

	private float TabSpacing
	{
		get
		{
			return 4f;
		}
	}

	public static VisualiserManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	public bool Touched(Vector2 position)
	{
		bool flag = position.x < TabWidth && position.y > (float)Screen.height - ((float)(SIDE_TAB_MAX + 1) * (TabHeight + TabSpacing) + TabOffset);
		bool flag2 = position.x > (float)Screen.width - TabWidth && position.y > (float)Screen.height - ((float)(SIDE_TAB_MAX + 1) * (TabHeight + TabSpacing) + TabOffset);
		return flag || flag2;
	}

	public bool IsAnyOpen()
	{
		foreach (KeyValuePair<string, VisualiserWrapper> mVisualiser in mVisualisers)
		{
			if (mVisualiser.Value.Open)
			{
				return true;
			}
		}
		return false;
	}

	public bool ShouldTakeInput()
	{
		foreach (KeyValuePair<string, VisualiserWrapper> mVisualiser in mVisualisers)
		{
			if (mVisualiser.Value.Open && mVisualiser.Value.RequiresInput)
			{
				return true;
			}
		}
		return false;
	}
}
