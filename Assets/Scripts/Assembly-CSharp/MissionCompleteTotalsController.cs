public class MissionCompleteTotalsController : MenuScreenBlade
{
	private const float SLOW_COUNT = 2f;

	public SpriteText CurrentLevel;

	public ProgressBar LevelProgress;

	public RankIconController RankIcon;

	public CountUpText XP;

	public CountUpText Cash;

	public CountUpText CurrentXP;

	private int mPlayerXPOnStart;

	private int mPlayerCashOnStart;

	private void Start()
	{
		base.Awake();
		XPManager instance = XPManager.Instance;
		GameSettings instance2 = GameSettings.Instance;
		mPlayerCashOnStart = instance2.PlayerCash().SoftCash();
		mPlayerXPOnStart = StatsHelper.PlayerXP();
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		instance.ConvertXPToLevel(mPlayerXPOnStart, out level, out prestigeLevel, out xpToNextLevel, out percent);
		if (CurrentLevel != null)
		{
			string text = AutoLocalize.Get("S_LEVEL");
			CurrentLevel.Text = text + " " + level;
		}
		if (CurrentXP != null)
		{
			CurrentXP.CountTo(mPlayerXPOnStart, 0f);
		}
		if (XP != null)
		{
			XP.CountTo(0, 0f);
		}
		if (Cash != null)
		{
			Cash.CountTo(0, 0f);
		}
		if (LevelProgress != null)
		{
			LevelProgress.SetValueNow(percent);
		}
		if (RankIcon != null)
		{
			RankIcon.SetRank(level);
		}
	}

	private void AddCash(int cash, float timeToCount)
	{
		if (Cash != null)
		{
			Cash.CountTo(Cash.Value + cash, timeToCount);
		}
	}

	private void AddExperience(int xp, float timeToCount)
	{
		int num = xp;
		if (XP != null)
		{
			num += XP.Value;
			XP.CountTo(XP.Value + xp, timeToCount);
		}
		if (CurrentXP != null)
		{
			CurrentXP.CountTo(mPlayerXPOnStart + num, timeToCount);
		}
		if (LevelProgress != null && RankIcon != null && CurrentLevel != null)
		{
			int level = 0;
			int prestigeLevel = 0;
			float percent = 0f;
			int xpToNextLevel = 0;
			XPManager.Instance.ConvertXPToLevel(mPlayerXPOnStart + num, out level, out prestigeLevel, out xpToNextLevel, out percent);
			LevelProgress.SetValue(percent);
			RankIcon.SetRank(level);
			string text = AutoLocalize.Get("S_LEVEL");
			CurrentLevel.Text = text + " " + level;
		}
	}

	public void Refresh()
	{
		XPManager instance = XPManager.Instance;
		GameSettings instance2 = GameSettings.Instance;
		int num = instance2.PlayerCash().SoftCash();
		int num2 = StatsHelper.PlayerXP();
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		instance.ConvertXPToLevel(num2, out level, out prestigeLevel, out xpToNextLevel, out percent);
		if (CurrentLevel != null)
		{
			string text = AutoLocalize.Get("S_LEVEL");
			CurrentLevel.Text = text + " " + level;
		}
		if (CurrentXP != null)
		{
			CurrentXP.CountTo(num2, 2f);
		}
		if (XP != null)
		{
			XP.CountTo(num2 - mPlayerXPOnStart, 2f);
		}
		if (Cash != null)
		{
			Cash.CountTo(num - mPlayerCashOnStart, 2f);
		}
		if (LevelProgress != null)
		{
			LevelProgress.SetValue(percent);
		}
		if (RankIcon != null)
		{
			RankIcon.SetRank(level);
		}
	}
}
