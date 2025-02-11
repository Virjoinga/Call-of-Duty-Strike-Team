using System;
using System.Collections;
using System.Collections.Generic;

public class GameCenterScore
{
	public string category;

	public string formattedValue;

	public long value;

	public DateTime date;

	public string playerId;

	public int rank;

	public bool isFriend;

	public string alias;

	public int maxRange;

	public GameCenterScore(Hashtable ht)
	{
		if (ht.Contains("category"))
		{
			category = ht["category"] as string;
		}
		if (ht.Contains("formattedValue"))
		{
			formattedValue = ht["formattedValue"] as string;
		}
		if (ht.Contains("value"))
		{
			value = long.Parse(ht["value"].ToString());
		}
		if (ht.Contains("playerId"))
		{
			playerId = ht["playerId"] as string;
		}
		if (ht.Contains("rank"))
		{
			rank = int.Parse(ht["rank"].ToString());
		}
		if (ht.Contains("isFriend"))
		{
			isFriend = (bool)ht["isFriend"];
		}
		if (ht.Contains("alias"))
		{
			alias = ht["alias"] as string;
		}
		else
		{
			alias = "Anonymous";
		}
		if (ht.Contains("maxRange"))
		{
			maxRange = int.Parse(ht["maxRange"].ToString());
		}
		if (ht.Contains("date"))
		{
			double num = double.Parse(ht["date"].ToString());
			date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num);
		}
	}

	public static List<GameCenterScore> fromJSON(string json)
	{
		List<GameCenterScore> list = new List<GameCenterScore>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(new GameCenterScore(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Score> category: {0}, formattedValue: {1}, date: {2}, rank: {3}, alias: {4}, maxRange: {5}", category, formattedValue, date, rank, alias, maxRange);
	}
}
