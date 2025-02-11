using UnityEngine;

public class StatsVisualiserSquad : StatsVisualiserBase
{
	public override void Start()
	{
		xPos = 520;
		yPos = 20;
		height = 70;
		base.Start();
	}

	private void OnGUI()
	{
		if (!StatsVisualiser.Instance().Hidden())
		{
			string text = "Squad";
			string empty = string.Empty;
			empty = empty + "Kills: " + StatsHelper.NumKillsBySquad();
			empty = empty + "\nDeaths: " + StatsHelper.NumDeathsForSquad();
			empty += string.Format("\nKDR: {0:.00}", StatsHelper.KDRForSquad());
			GUI.TextField(titleRect, text);
			GUI.TextField(statsRect, empty);
		}
	}
}
