using System.Collections;
using System.Collections.Generic;

public class GameCenterPlayer
{
	public string playerId;

	public string alias;

	public bool isFriend;

	public GameCenterPlayer(Hashtable ht)
	{
		if (ht.Contains("playerId"))
		{
			playerId = ht["playerId"] as string;
		}
		if (ht.Contains("alias"))
		{
			alias = ht["alias"] as string;
		}
		if (ht.Contains("isFriend"))
		{
			isFriend = (bool)ht["isFriend"];
		}
	}

	public static List<GameCenterPlayer> fromJSON(string json)
	{
		List<GameCenterPlayer> list = new List<GameCenterPlayer>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(new GameCenterPlayer(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Player> playerId: {0}, alias: {1}, isFriend: {2}", playerId, alias, isFriend);
	}
}
