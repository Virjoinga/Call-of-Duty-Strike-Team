using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationVisualiser : MonoBehaviour
{
	public static AnimationVisualiser instance;

	private static bool Fastforward;

	private bool mHidden;

	public static AnimationVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		UnityEngine.Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		string text = "Animation Visualiser" + Environment.NewLine;
		Actor[] array = (Actor[])UnityEngine.Object.FindObjectsOfType(typeof(Actor));
		Actor[] array2 = array;
		foreach (Actor actor in array2)
		{
			text += Environment.NewLine;
			text += Environment.NewLine;
			text += actor.baseCharacter.name;
			text += Environment.NewLine;
			Animation animationPlayer = actor.animDirector.AnimationPlayer;
			foreach (AnimationState item in animationPlayer)
			{
				if (item.enabled)
				{
					text += string.Format("{0}: {1:0.0}/{2:0.0} ({3:0.00}) L:{4} {5}", item.name, item.time % item.length, item.length, item.weight, item.layer, item.blendMode);
					text += Environment.NewLine;
				}
			}
		}
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		Rect position = Rect.MinMaxRect((float)Screen.width / 1.5f, 0f, (float)Screen.width - 32f, Screen.height);
		GUI.Box(position, text, gUIStyle);
		if (DoButton(position.x + 15f, position.y + 22.5f, 130f, 25f, "Fast Forward: " + ((!Fastforward) ? "Off" : "On")))
		{
			Fastforward = !Fastforward;
			if (Fastforward)
			{
				Time.timeScale = 40f;
			}
			else
			{
				TimeManager.instance.ResumeNormalTime();
			}
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

	private bool DoButton(float x, float y, float width, float height, string label)
	{
		Rect position = new Rect(x, y, width, height);
		return GUI.Button(position, label);
	}

	private string[] StringsFromCharacters(IEnumerable<char> chars)
	{
		List<string> list = new List<string>();
		foreach (char @char in chars)
		{
			list.Add(@char.ToString());
		}
		return list.ToArray();
	}
}
