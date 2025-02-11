public class GlobalLeagueMessageBox : MessageBox
{
	public PackedSprite MedalIcon;

	public SpriteText Award;

	public SpriteText Multiplier;

	public SpriteText Summary;

	public string BodyKey { get; private set; }

	public void Setup(int award, float multiplier, int lastLeague, int newLeague)
	{
		string key = "S_FL_LEAGUE_0" + (newLeague + 1);
		string key2 = "S_FL_WEEK_END_BODY_03";
		string bodyKey;
		string key3;
		string key4;
		if (lastLeague > newLeague)
		{
			bodyKey = "S_FL_WEEK_END_TITLE_DEMOTE";
			key3 = "S_FL_WEEK_END_BODY_02_DEMOTE";
			key4 = "S_FL_WEEK_END_BODY_01_DEMOTE";
		}
		else if (lastLeague < newLeague)
		{
			bodyKey = "S_FL_WEEK_END_TITLE_PROMOTE";
			key3 = "S_FL_WEEK_END_BODY_02_PROMOTE";
			key4 = "S_FL_WEEK_END_BODY_01_PROMOTE";
		}
		else if (newLeague == 5)
		{
			bodyKey = "S_FL_WEEK_END_TITLE_SAME";
			key3 = "S_FL_WEEK_END_BODY_02_SAME";
			key4 = "S_FL_WEEK_END_BODY_01_STILL_TOP";
		}
		else
		{
			bodyKey = "S_FL_WEEK_END_TITLE_SAME";
			key3 = "S_FL_WEEK_END_BODY_02_SAME";
			key4 = "S_FL_WEEK_END_BODY_01_SAME";
		}
		if (MedalIcon != null)
		{
			MedalIcon.SetFrame(0, newLeague);
		}
		BodyKey = bodyKey;
		if (Award != null)
		{
			char c = CommonHelper.HardCurrencySymbol();
			string arg = string.Format("{0}{1}", c, award);
			Award.Text = string.Format(Language.Get(key3), arg);
		}
		if (Multiplier != null)
		{
			Multiplier.Text = string.Format(Language.Get(key2), multiplier);
		}
		if (Summary != null)
		{
			Summary.Text = string.Format(Language.Get(key4), Language.Get(key).ToUpper());
		}
	}
}
