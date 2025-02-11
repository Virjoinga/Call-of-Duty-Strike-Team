public class FrontEndScreenData
{
	public string Title { get; private set; }

	public ScreenID ID { get; private set; }

	public bool AllowBack { get; private set; }

	public bool AllowActivate { get; private set; }

	public bool AllowMTX { get; private set; }

	public bool UseBackground { get; private set; }

	public bool UseNewsTicker { get; private set; }

	public bool CanShowBanner { get; private set; }

	public FrontEndScreenData(ScreenID id, string title, bool back, bool activate, bool mtx, bool bg, bool news, bool banner)
	{
		if (title != string.Empty)
		{
			Title = AutoLocalize.Get(title);
		}
		ID = id;
		AllowBack = back;
		AllowActivate = activate;
		AllowMTX = mtx;
		UseBackground = bg;
		UseNewsTicker = news;
		CanShowBanner = banner;
	}
}
