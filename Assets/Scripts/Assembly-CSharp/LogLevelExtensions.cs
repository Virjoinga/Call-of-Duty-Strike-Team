public static class LogLevelExtensions
{
	public static bool ShouldLogMessage(LogLevel messageLogLevel, LogLevel outputLevel)
	{
		return messageLogLevel >= outputLevel;
	}
}
