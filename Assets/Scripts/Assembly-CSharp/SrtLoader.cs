using System;
using System.Collections.Generic;
using UnityEngine;

public class SrtLoader
{
	public class SrtKey
	{
		public int mId;

		public float mStartTime;

		public float mEndTime;

		public string mText;
	}

	private List<SrtKey> mKeys = new List<SrtKey>();

	public SrtLoader()
	{
	}

	public SrtLoader(string file)
	{
		Load(file);
	}

	public string GetTextForTime(float time)
	{
		foreach (SrtKey mKey in mKeys)
		{
			if (time > mKey.mStartTime && time < mKey.mEndTime)
			{
				return mKey.mText;
			}
		}
		return string.Empty;
	}

	public void Load(string file)
	{
		TextAsset textAsset = Resources.Load(file) as TextAsset;
		if (textAsset == null)
		{
			Debug.LogWarning("failed to load srt file " + file);
			return;
		}
		string[] array = textAsset.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		int num = 0;
		while (num < array.Length)
		{
			int result = 0;
			string s = array[num].Trim();
			if (int.TryParse(s, out result))
			{
				SrtKey srtKey = new SrtKey();
				srtKey.mId = result;
				string[] array2 = array[num + 1].Split(new string[1] { " --> " }, StringSplitOptions.None);
				array2[0] = array2[0].Trim();
				int hours = int.Parse(array2[0].Split(':')[0]);
				int minutes = int.Parse(array2[0].Split(':')[1]);
				int seconds = int.Parse(array2[0].Split(':')[2].Split(',')[0]);
				int milliseconds = int.Parse(array2[0].Split(':')[2].Split(',')[1]);
				srtKey.mStartTime = (float)new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalSeconds;
				array2[1] = array2[1].Trim();
				hours = int.Parse(array2[1].Split(':')[0]);
				minutes = int.Parse(array2[1].Split(':')[1]);
				seconds = int.Parse(array2[1].Split(':')[2].Split(',')[0]);
				milliseconds = int.Parse(array2[1].Split(':')[2].Split(',')[1]);
				srtKey.mEndTime = (float)new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalSeconds;
				string key = array[num + 2].Trim().ToUpper();
				srtKey.mText = Language.Get(key);
				mKeys.Add(srtKey);
				num += 3;
			}
			else
			{
				num++;
			}
		}
		Resources.UnloadAsset(textAsset);
	}
}
