using UnityEngine;

public class StatsVisualiserPlayerAbility : StatsVisualiserBase
{
	public override void Start()
	{
		xPos = 300;
		yPos = 20;
		base.Start();
	}

	private void OnGUI()
	{
		if (!(StatsVisualiser.Instance() != null) || !StatsVisualiser.Instance().Hidden())
		{
			string empty = string.Empty;
			empty = empty + "FP Kills: " + StatsHelper.NumKillsByPlayer();
			empty = empty + "\nFP Deaths: " + StatsHelper.NumDeathsForPlayer();
			empty = empty + "\nFP KDR: " + StatsHelper.KDRForPlayer();
			empty = empty + "\n\nFP Shots fired: " + StatsHelper.ShotsFiredByPlayer();
			empty = empty + "\nFP Shots hit: " + StatsHelper.ShotsHitByPlayer();
			string text = empty;
			empty = text + "\nFP Accuracy: " + StatsHelper.AccuracyForPlayer() + "%";
			empty = empty + "\nFP Head Shots: " + StatsHelper.HeadShotsForPlayer();
			empty = empty + "\n\nFavourite Weapon: " + StatsHelper.MostFiredWeaponByPlayer();
			GUI.TextField(titleRect, "Player Ability");
			GUI.TextField(statsRect, empty);
		}
	}
}
