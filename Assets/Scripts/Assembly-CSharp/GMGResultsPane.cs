using System;
using UnityEngine;

public class GMGResultsPane : MonoBehaviour
{
	[Serializable]
	public class ResultsLine
	{
		public SpriteText Lable;

		public SpriteText Value;

		public SpriteText XP;
	}

	private const float kGMGResultsTimeout = 10f;

	public ResultsLine WaveTime;

	public ResultsLine Wave;

	public ResultsLine Kills;

	public ResultsLine Headshots;

	public ResultsLine Accuracy;

	public ResultsLine OtherXP;

	public ResultsLine WaveBonus;

	public Scale9Grid Brackets;

	public Scale9Grid Background;

	private float mTimeout;

	private void Awake()
	{
		mTimeout = -1f;
	}

	private void Start()
	{
		if (Brackets != null)
		{
			if (TBFUtils.IsRetinaHdDevice())
			{
				Brackets.size *= 2f;
			}
			Brackets.Resize();
		}
		if (Background != null)
		{
			if (TBFUtils.IsRetinaHdDevice())
			{
				Background.size *= 2f;
			}
			Background.Resize();
		}
	}

	private void Update()
	{
		if (mTimeout > 0f)
		{
			mTimeout -= Time.deltaTime;
			if (mTimeout <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	public void UpdateStats()
	{
		mTimeout = 10f;
		WaveStats instance = WaveStats.Instance;
		int num = GMGData.Instance.CurrentWave() * WaveStats.Instance.XPForEachWave;
		SetTime(instance.WaveTime, 0);
		SetWave(GMGData.Instance.CurrentWave(), num);
		SetKills(instance.Kills, instance.XPFromKills);
		SetHeadshots(instance.Headshots, instance.XPFromHeadshots);
		SetAccuracy(instance.Accuracy, instance.Accuracy);
		SetOtherXP(instance.OtherXP);
		SetWaveBonus(num + instance.XPFromKills + instance.XPFromHeadshots + instance.Accuracy + instance.OtherXP);
	}

	private string IntToXp(int xp)
	{
		if (xp == 0)
		{
			return string.Empty;
		}
		return "+" + xp + Language.Get("S_RESULT_XP");
	}

	private void SetTime(float time, int xp)
	{
		WaveTime.Value.Text = TimeUtils.GetMinutesSecondsCountdownFromSeconds((int)time);
		WaveTime.XP.Text = IntToXp(xp);
	}

	private void SetWave(int wave, int xp)
	{
		Wave.Value.Text = wave.ToString();
		Wave.XP.Text = IntToXp(xp);
	}

	private void SetKills(int kills, int xp)
	{
		Kills.Value.Text = kills.ToString();
		Kills.XP.Text = IntToXp(xp);
	}

	private void SetHeadshots(int headshots, int xp)
	{
		Headshots.Value.Text = headshots.ToString();
		Headshots.XP.Text = IntToXp(xp);
	}

	private void SetAccuracy(float accuracy, int xp)
	{
		if (accuracy < 0f)
		{
			Accuracy.Value.Text = "--";
			Accuracy.XP.Text = IntToXp(0);
		}
		else
		{
			Accuracy.Value.Text = accuracy + "%";
			Accuracy.XP.Text = IntToXp(xp);
		}
	}

	private void SetOtherXP(int xp)
	{
		OtherXP.XP.Text = IntToXp(xp);
	}

	private void SetWaveBonus(int xp)
	{
		WaveBonus.XP.Text = IntToXp(xp);
	}
}
