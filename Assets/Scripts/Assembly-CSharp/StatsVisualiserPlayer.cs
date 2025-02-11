using UnityEngine;

public class StatsVisualiserPlayer : StatsVisualiserBase
{
	public override void Start()
	{
		xPos = 80;
		yPos = 20;
		height = 200;
		base.Start();
	}

	private void OnGUI()
	{
		if (!(StatsVisualiser.Instance() != null) || !StatsVisualiser.Instance().Hidden())
		{
			string empty = string.Empty;
			empty = empty + "Score: " + StatsHelper.TotalScore();
			empty = empty + "\nCash: " + GameSettings.Instance.PlayerCash().SoftCash();
			empty = empty + "\n\nXP: " + StatsHelper.PlayerXP();
			int level = 0;
			int xpToNextLevel = 0;
			int prestigeLevel = 0;
			float percent = 0f;
			XPManager.Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
			string text = empty;
			empty = text + "\nLevel: " + level + "(" + prestigeLevel + ")";
			empty = empty + "\nXP to next level: " + xpToNextLevel;
			text = empty;
			empty = text + "\nXP percent: " + percent * 100f + "%";
			empty += string.Format("\n\nTime Played: {0}", StatsHelper.SecondsToString(StatsHelper.TotalTimePlayed()));
			empty = empty + "\nMissions Played: " + StatsHelper.MissionsPlayed();
			empty = empty + "\nMissions Successful: " + StatsHelper.MissionsSuccessful();
			empty = empty + "\nMedals earned: " + StatsHelper.TotalMedalsEarned();
			GUI.TextField(titleRect, "Global (Player)");
			GUI.TextField(statsRect, empty);
		}
	}
}
