using System;
using System.Collections;
using System.Collections.Generic;

public class GameCenterAchievement
{
	public string identifier;

	public bool isHidden;

	public bool completed;

	public DateTime lastReportedDate;

	public float percentComplete;

	public GameCenterAchievement(Hashtable ht)
	{
		if (ht.Contains("identifier"))
		{
			identifier = ht["identifier"] as string;
		}
		if (ht.Contains("hidden"))
		{
			isHidden = (bool)ht["hidden"];
		}
		if (ht.Contains("completed"))
		{
			completed = (bool)ht["completed"];
		}
		if (ht.Contains("percentComplete"))
		{
			percentComplete = float.Parse(ht["percentComplete"].ToString());
		}
		if (ht.Contains("lastReportedDate"))
		{
			double value = double.Parse(ht["lastReportedDate"].ToString());
			lastReportedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
		}
	}

	public static List<GameCenterAchievement> fromJSON(string json)
	{
		List<GameCenterAchievement> list = new List<GameCenterAchievement>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(new GameCenterAchievement(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Achievement> identifier: {0}, hidden: {1}, completed: {2}, percentComplete: {3}, lastReported: {4}", identifier, isHidden, completed, percentComplete, lastReportedDate);
	}
}
