public static class ChallengeLeaderboardWriteTypesExtensions
{
	public static Bedrock.brLeaderboardWriteType ConvertToBedrockType(this ChallengeLeaderboardWriteTypes writeType)
	{
		if (writeType != 0 && writeType == ChallengeLeaderboardWriteTypes.UseMin)
		{
			return Bedrock.brLeaderboardWriteType.BR_STAT_WRITE_MIN;
		}
		return Bedrock.brLeaderboardWriteType.BR_STAT_WRITE_MAX;
	}
}
