public static class KillTypeHelper
{
	private const string kSilentKill = "Silent";

	private const string kSilentNeckSnapKill = "SilentNeckSnap";

	private const string kKnifeKill = "Knife";

	private const string kSuicide = "Suicide Pill";

	public static bool IsAStealthKill(HealthComponent.HeathChangeEventArgs hce)
	{
		return hce.DamageType == "Silent" || hce.DamageType == "SilentNeckSnap";
	}

	public static bool IsAStealthKill(Events.Kill ev)
	{
		return ev.DamageType == "Silent" || ev.DamageType == "SilentNeckSnap";
	}

	public static bool IsAKnifeKill(Events.Kill ev)
	{
		return ev.DamageType == "Knife";
	}

	public static bool IsASuicide(HealthComponent.HeathChangeEventArgs hce)
	{
		return hce.DamageType == "Suicide Pill";
	}

	public static bool IsNotAStealthKill(HealthComponent.HeathChangeEventArgs hce)
	{
		return !IsAStealthKill(hce);
	}

	public static bool IsNotASuicide(HealthComponent.HeathChangeEventArgs hce)
	{
		return !IsASuicide(hce);
	}
}
