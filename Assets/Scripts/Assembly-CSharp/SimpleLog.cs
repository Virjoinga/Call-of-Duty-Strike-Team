using System.Collections.Generic;
using UnityEngine;

public class SimpleLog : MonoBehaviour
{
	private const float LOG_BACKGROUND_MULTIPLIER = 1.1f;

	private const float ANIMATION_TIME = 0.3f;

	private const float BACKGROUND_X_OFFSET = 0.46f;

	private const float BACKGROUND_Y_OFFSET = 0.5f;

	public SimpleLogEntry LogEntryStyle;

	public float TimeBetweenLogEntries = 2f;

	private float mTimeLineLastAdded;

	private List<string> mBufferedLogEntries;

	private Scale9Grid mBackground;

	private Vector2 mFromSize;

	private Vector2 mToSize;

	private Vector3 mFromPosition;

	private Vector3 mToPosition;

	private float mBoxAnimation;

	private bool mDirty;

	private void Start()
	{
		mBufferedLogEntries = new List<string>();
		mBackground = GetComponentInChildren<Scale9Grid>();
		mDirty = false;
		if (mBackground != null)
		{
			mBackground.size = Vector2.zero;
			mBoxAnimation = 0.3f;
		}
	}

	private void OnDestroy()
	{
		if (mBufferedLogEntries != null)
		{
			mBufferedLogEntries.Clear();
		}
	}

	private void Update()
	{
		if (mBufferedLogEntries.Count > 0)
		{
			bool flag = true;
			if ((bool)BlackBarsController.Instance && BlackBarsController.Instance.BlackBarsEnabled)
			{
				flag = false;
			}
			if (flag && Time.realtimeSinceStartup - mTimeLineLastAdded > TimeBetweenLogEntries)
			{
				AddLogLine(mBufferedLogEntries[0]);
				mBufferedLogEntries.RemoveAt(0);
				mTimeLineLastAdded = Time.realtimeSinceStartup;
			}
		}
		if (mBackground != null)
		{
			if (mDirty)
			{
				float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
				Vector2 vector = MeasureLogSize();
				vector *= 1.1f;
				mFromSize = mBackground.size;
				mToSize = vector;
				mToSize.x = vector.x / num;
				mToSize.y = vector.y / num;
				Vector3 position = base.transform.position;
				position.x += vector.x * 0.46f;
				position.y -= vector.y * 0.5f;
				mFromPosition = mBackground.transform.position;
				mToPosition = position;
				mBoxAnimation = 0f;
				mDirty = false;
			}
			if (mBoxAnimation < 0.3f)
			{
				mBoxAnimation += TimeManager.DeltaTime;
				Vector3 position2 = Vector3.Lerp(mFromPosition, mToPosition, mBoxAnimation / 0.3f);
				Vector2 size = Vector2.Lerp(mFromSize, mToSize, mBoxAnimation / 0.3f);
				mBackground.size = size;
				mBackground.Resize();
				mBackground.transform.position = position2;
			}
		}
	}

	public void AddLine(string text)
	{
		if (mBufferedLogEntries != null)
		{
			mBufferedLogEntries.Add(text.ToUpper());
		}
	}

	private void AddLogLine(string text)
	{
		SimpleLogEntry simpleLogEntry = Object.Instantiate(LogEntryStyle) as SimpleLogEntry;
		simpleLogEntry.SetText(text);
		simpleLogEntry.SetRemovedCallback(this, "Refresh");
		simpleLogEntry.transform.position = base.transform.position;
		Transform transform = null;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.GetComponent<SimpleLogEntry>() != null)
			{
				transform = child;
				break;
			}
		}
		simpleLogEntry.transform.parent = base.transform;
		if (transform != null)
		{
			transform.transform.parent = simpleLogEntry.transform;
			transform.SendMessage("OnReparent");
		}
		mDirty = true;
	}

	private Vector2 MeasureLogSize()
	{
		Vector2 result = default(Vector2);
		SimpleLogEntry[] componentsInChildren = GetComponentsInChildren<SimpleLogEntry>();
		SimpleLogEntry[] array = componentsInChildren;
		foreach (SimpleLogEntry simpleLogEntry in array)
		{
			SpriteText component = simpleLogEntry.GetComponent<SpriteText>();
			TeleTypeLabel component2 = simpleLogEntry.GetComponent<TeleTypeLabel>();
			if (component != null)
			{
				string s = component.Text;
				if (component2 != null && component2.CompleteText != null)
				{
					s = component2.CompleteText;
				}
				float width = component.GetWidth(s);
				float num = component.BaseHeight * component.lineSpacing;
				result.y += num;
				if (result.x < width)
				{
					result.x = width;
				}
			}
		}
		return result;
	}

	private void Refresh()
	{
		mDirty = true;
	}
}
