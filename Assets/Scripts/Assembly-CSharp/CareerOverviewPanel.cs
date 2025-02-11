using UnityEngine;

public class CareerOverviewPanel : MonoBehaviour
{
	public SpriteText MedalCountText;

	public ProgressBar MedalsProgress;

	public void Start()
	{
		if (SplashScreenControl.SplashShown)
		{
			UpdateMedals();
		}
	}

	private void UpdateMedals()
	{
		MedalCountText.Text = string.Format("{0}/{1}", StatsHelper.TotalMedalsEarned(), StatsHelper.NumberOfMedalsAvailable());
	}
}
