public interface IMobileNetworkImpl
{
	void Init();

	bool IsMobileNetworkAvailable();

	bool AreTurnBasedMatchesAvailable();

	void AuthenticateLocalPlayer();

	bool IsPlayerAuthenticated();

	string PlayerAlias();

	string PlayerIdentifier();

	bool supportsAchievements();

	void showAchievementsBanner();

	void resetAchievements();

	void reportAchievement(string achievementId, float percentComplete);

	void getAchievements();

	void showAchievements();
}
