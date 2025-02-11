using System.Globalization;

public class PlayerStatusController : MenuScreenBlade
{
	public SpriteText RankText;

	public SpriteText CurrentLevelText;

	public SpriteText NextLevelText;

	public SpriteText HardCashText;

	public SpriteText SoftCashText;

	public ProgressBar LevelProgress;

	public override void Awake()
	{
		base.Awake();
		SpriteText[] componentsInChildren = GetComponentsInChildren<SpriteText>();
		SpriteText[] array = componentsInChildren;
		foreach (SpriteText spriteText in array)
		{
			if (spriteText.name == "Rank_text")
			{
				RankText = spriteText;
			}
			else if (spriteText.name == "CurrentLevel_text")
			{
				CurrentLevelText = spriteText;
			}
			else if (spriteText.name == "NextLevel_text")
			{
				NextLevelText = spriteText;
			}
			else if (spriteText.name == "HardCash_text")
			{
				HardCashText = spriteText;
			}
			else if (spriteText.name == "SoftCash_text")
			{
				SoftCashText = spriteText;
			}
		}
		LevelProgress = GetComponentInChildren<ProgressBar>();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		int num = 0;
		int num2 = GameSettings.Instance.PlayerCash().SoftCash();
		int num3 = level;
		float value = percent;
		if (LevelProgress != null)
		{
			LevelProgress.SetValue(value);
		}
		if (RankText != null)
		{
			RankText.Text = XPManager.Instance.XPLevelName(level);
		}
		if (CurrentLevelText != null)
		{
			CurrentLevelText.Text = string.Format("{0}", num3);
		}
		if (NextLevelText != null)
		{
			NextLevelText.Text = string.Format("{0}", num3 + 1);
		}
		if (HardCashText != null)
		{
			HardCashText.Text = num.ToString(numberFormat);
		}
		if (SoftCashText != null)
		{
			SoftCashText.Text = num2.ToString(numberFormat);
		}
	}
}
