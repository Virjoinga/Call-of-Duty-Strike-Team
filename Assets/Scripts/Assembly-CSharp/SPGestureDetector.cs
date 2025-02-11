using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SPGestureDetector
{
	public enum Type
	{
		Invalid = 0,
		FingerSwipe = 1,
		Count = 2
	}

	public static List<string> GestureNames;

	public bool detected;

	public Type type;

	public SPGestureDetector(Type t)
	{
		detected = false;
		type = t;
	}

	public static void PopulateNameList()
	{
		if (GestureNames == null)
		{
			GestureNames = new List<string>();
			GestureNames.Add("Set Gesture...");
			for (Type type = Type.FingerSwipe; type < Type.Count; type++)
			{
				GestureNames.Add(type.ToString());
			}
		}
	}

	public virtual void Activate()
	{
		Type type = this.type;
		if (type == Type.FingerSwipe)
		{
			InputManager.Instance.AddOnFingerSwipeEventHandler(OnFingerSwipe, 1);
		}
	}

	public virtual void Deactivate()
	{
		Type type = this.type;
		if (type == Type.FingerSwipe)
		{
			InputManager.Instance.RemoveOnFingerSwipeEventHandler(OnFingerSwipe);
		}
	}

	private void OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		detected = true;
	}
}
