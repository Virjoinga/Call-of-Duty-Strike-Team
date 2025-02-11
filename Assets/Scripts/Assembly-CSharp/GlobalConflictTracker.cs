using System.Globalization;
using UnityEngine;

public class GlobalConflictTracker : MenuScreenBlade
{
	private struct Row
	{
		public SpriteText Position;

		public SpriteText Name;

		public SpriteText Score;
	}

	public GameObject[] Rows;

	public SpriteText TitleTimeRemaining;

	public SpriteText LeagueTitle;

	public SpriteText LeagueScore;

	public GlobalConflictMedal LeagueMedalPrefab;

	public Transform MedalPlacement;

	private LeaderboardController.Panel[] mCachedCurrentPosition;

	private GlobalConflictMedal mLeagueMedal;

	private float mMaxPositionLength;

	private Row[] mRows;

	private NumberFormatInfo mNfi;

	public override void Awake()
	{
		base.Awake();
		mNfi = GlobalizationUtils.GetNumberFormat(0);
		mNfi.NumberDecimalDigits = 0;
		if (LeagueMedalPrefab != null && MedalPlacement != null)
		{
			mLeagueMedal = Object.Instantiate(LeagueMedalPrefab) as GlobalConflictMedal;
			if (mLeagueMedal != null)
			{
				mLeagueMedal.transform.parent = MedalPlacement.transform;
				CommonBackgroundBoxPlacement component = mLeagueMedal.GetComponent<CommonBackgroundBoxPlacement>();
				if (component != null)
				{
					component.StartPositionAsPercentageOfBoxWidth = 0.1f;
					component.StartPositionAsPercentageOfBoxHeight = 0.2f;
					component.WidthAsPercentageOfBoxWidth = 0.2f;
					component.HeightAsPercentageOfBoxHeight = 0.2f;
				}
			}
		}
		mRows = new Row[Rows.Length];
		for (int i = 0; i < Rows.Length; i++)
		{
			if (!(Rows[i] != null))
			{
				continue;
			}
			mRows[i] = default(Row);
			SpriteText[] componentsInChildren = Rows[i].GetComponentsInChildren<SpriteText>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].name.Contains("Name"))
				{
					mRows[i].Name = componentsInChildren[j];
				}
				else if (componentsInChildren[j].name.Contains("Position"))
				{
					mRows[i].Position = componentsInChildren[j];
				}
				else if (componentsInChildren[j].name.Contains("Score"))
				{
					mRows[i].Score = componentsInChildren[j];
				}
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		GlobalUnrestController instance = GlobalUnrestController.Instance;
		if (mLeagueMedal != null)
		{
			mLeagueMedal.SetMedal((GlobalConflictMedal.Medal)instance.CurrentLeague);
		}
		UpdatePanelText(instance.CurrentLeague, instance.CurrentScore, instance.SecondsToNextReset);
		UpdateLeagueLeaderboard();
	}

	public override void OnScreen()
	{
		base.OnScreen();
		LayoutLeaderboard();
	}

	private void UpdatePanelText(int leagueIndex, int playerScore, int secondsRemaining)
	{
		if (TitleTimeRemaining != null)
		{
			int num = (int)((float)secondsRemaining / 60f);
			int num2 = num / 60;
			int num3 = num - num2 * 60;
			TitleTimeRemaining.Text = string.Format(Language.Get("S_FL_TRACKER_TIME"), num2.ToString("D2"), num3.ToString("D2"));
		}
		if (LeagueTitle != null)
		{
			LeagueTitle.Text = Language.Get("S_FL_LEAGUE_0" + (leagueIndex + 1));
		}
		if (LeagueScore != null)
		{
			LeagueScore.Text = playerScore.ToString("N", mNfi);
		}
	}

	private void UpdateLeagueLeaderboard()
	{
		mCachedCurrentPosition = new LeaderboardController.Panel[mRows.Length];
		mMaxPositionLength = 0f;
		for (int i = 0; i < mRows.Length; i++)
		{
			Row row = mRows[i];
			if (Rows[i] != null && row.Position != null && row.Name != null && row.Score != null)
			{
				mCachedCurrentPosition[i] = new LeaderboardController.Panel();
				mCachedCurrentPosition[i].rank = string.Format("{0}.", i + 1);
				mCachedCurrentPosition[i].title = "Some name";
				mCachedCurrentPosition[i].score = (5628454 * (mRows.Length - i)).ToString("N", mNfi);
				row.Position.Text = string.Empty;
				row.Name.Text = string.Empty;
				row.Score.Text = string.Empty;
				float rankWidth = CommonHelper.GetRankWidth(row.Position, mCachedCurrentPosition[i].rank);
				if (rankWidth > mMaxPositionLength)
				{
					mMaxPositionLength = rankWidth;
				}
			}
		}
	}

	public void LayoutLeaderboard()
	{
		for (int i = 0; i < mRows.Length; i++)
		{
			Row row = mRows[i];
			if (Rows[i] != null && row.Position != null && row.Name != null && row.Score != null)
			{
				row.Position.Text = mCachedCurrentPosition[i].rank;
				row.Name.Text = mCachedCurrentPosition[i].title;
				row.Score.Text = mCachedCurrentPosition[i].score;
				Vector3 position = row.Position.transform.position;
				position.x += mMaxPositionLength;
				row.Name.transform.position = position;
			}
		}
	}
}
