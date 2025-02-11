public class SwrveEventsMetaGame
{
	public enum ShareType
	{
		Facebook = 0,
		Twitter = 1
	}

	public static void GameCentreConnected()
	{
		Bedrock.AnalyticsLogEvent("MetaGame.Social.GameCentreConnected");
	}

	public static void FacebookBroadcast(string fromButton, MissionListings.eMissionID missionId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("SocialLocation", fromButton, "Mission", missionId.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.FacebookBroadcast", parameters, false);
	}

	public static void TwitterBroadcast(string fromButton, MissionListings.eMissionID missionId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("SocialLocation", fromButton, "Mission", missionId.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.TwitterBroadcast", parameters, false);
	}

	public static void ChallengeJoined(uint challengeId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ChallengeID", challengeId.ToString(), "CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "TotalChallenges", SwrveUserData.Instance.ChallengesJoined.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Challenges.ChallengeJoined", parameters, false);
	}

	public static void ChallengeStopped(uint challengeId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ChallengeID", challengeId.ToString(), "CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal);
		Bedrock.AnalyticsLogEvent("MetaGame.Challenges.ChallengeStopped", parameters, false);
	}

	public static void ChallengeFinished(uint challengeId, bool timedOut, ChallengeMedalType award, ulong friendsPlaying, ulong friendsBeaten)
	{
		bool flag = award != ChallengeMedalType.None;
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ChallengeID", challengeId.ToString(), "CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "ChallengeTimedOut", timedOut.ToString(), "PlayerAward", award.ToString(), "FriendsPlaying", friendsPlaying.ToString(), "FriendsBeaten", friendsBeaten.ToString(), "EarnedReward", flag.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Challenges.ChallengeFinished", parameters, false);
		Bedrock.AnalyticsLogEvent("MetaGame.Challenges.ChallengeFinished.Challenge" + challengeId, parameters, false);
	}

	public static void SharedFromStats(ShareType type)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ShareType", type.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.Shared.Stats", parameters, false);
	}

	public static void SharedFromMission(ShareType type, MissionListings.eMissionID missionId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "Mission", missionId.ToString(), "ShareType", type.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.Shared.Mission", parameters, false);
	}

	public static void SharedFromResults(ShareType type, MissionListings.eMissionID missionId)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "Mission", missionId.ToString(), "ShareType", type.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.Shared.Results", parameters, false);
	}

	public static void SharedFromLevelUp(ShareType type)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ShareType", type.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Social.Shared.LevelUp", parameters, false);
	}

	public static void ClaimedReward(int consecutiveRewards)
	{
		int value = SecureStorage.Instance.GetInt("TotalRewardsClaimed") + 1;
		SecureStorage.Instance.SetInt("TotalRewardsClaimed", value);
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("ConsecutiveRewards", consecutiveRewards.ToString(), "TotalRewards", value.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.DailyReward.ClaimedReward", parameters, false);
	}

	public static void ChallengeClaimed(uint challengeId, ulong friendsBeaten, ChallengeMedalType medal)
	{
		int value = SecureStorage.Instance.GetInt("ChallengeRewardsClaimed") + 1;
		SecureStorage.Instance.SetInt("ChallengeRewardsClaimed", value);
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "ChallengeID", challengeId.ToString(), "TotalRewards", value.ToString(), "PlayerAward", medal.ToString(), "FriendsBeaten", friendsBeaten.ToString());
		Bedrock.AnalyticsLogEvent("MetaGame.Challenges.ChallengeClaimed", parameters, false);
	}
}
