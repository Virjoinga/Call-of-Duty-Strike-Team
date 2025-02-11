using System;
using System.Text;
using UnityEngine;

public static class TimeUtils
{
	public static readonly long UnixEpochTicks = new DateTime(1970, 1, 1).Ticks;

	public static readonly DateTime UnixEpoch = new DateTime(UnixEpochTicks, DateTimeKind.Utc);

	public static DateTime GetDateTimeFromUnixUtcTime(uint secondsSinceUnixEpoch)
	{
		long ticks = UnixEpochTicks + (long)secondsSinceUnixEpoch * 10000000L;
		return new DateTime(ticks, DateTimeKind.Utc);
	}

	public static uint GetSecondsSinceUnixEpoch()
	{
		return GetSecondsSinceUnixEpoch(DateTime.UtcNow);
	}

	public static uint GetSecondsSinceUnixEpoch(DateTime dateTime)
	{
		DateTime dateTime2 = dateTime.ToUniversalTime();
		double num = Math.Round((dateTime2 - UnixEpoch).TotalSeconds);
		if (num < 0.0)
		{
			Debug.LogError(string.Concat("Cannot convert time '", dateTime, "' that was before unix epoch!"));
			return 0u;
		}
		return Convert.ToUInt32(num);
	}

	public static string GetFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			string arg = Language.Get((timeSpan.Days != 1) ? "S_TIME_DAYS" : "S_TIME_DAY");
			return string.Format("{0} {1}", timeSpan.Days, arg);
		}
		if (timeSpan.TotalHours >= 1.0)
		{
			if (timeSpan.TotalHours == 1.0 && timeSpan.TotalMinutes == 0.0)
			{
				return string.Format("60 " + Language.Get("S_TIME_MINUTES"));
			}
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0} {1}", num, Language.Get("S_TIME_HOURS"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0} {1}", num2, Language.Get((timeSpan.TotalMinutes != 1.0) ? "S_TIME_MINUTES" : "S_TIME_MINUTE"));
		}
		if (timeSpan.TotalSeconds > 1.0)
		{
			return string.Format("{0} {1}", timeSpan.TotalSeconds, Language.Get("S_TIME_SECONDS"));
		}
		return Language.Get("S_TIME_NOW");
	}

	public static string GetShortFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return string.Format("{0}d", timeSpan.Days);
		}
		if (timeSpan.TotalHours > 1.0)
		{
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0}{1}", num, Language.Get("S_TIME_DAY_ABBREV"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0}{1}", num2, Language.Get("S_TIME_MINUTE_ABBREV"));
		}
		return string.Format("{0}{1}", timeSpan.Seconds, Language.Get("S_TIME_SECOND_ABBREV"));
	}

	public static string GetLongTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_DAY", "S_TIME_DAYS", timeSpan.Days);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_HOUR", "S_TIME_HOURS", timeSpan.Hours);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_MINUTE", "S_TIME_MINUTES", timeSpan.Minutes);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_SECOND", "S_TIME_SECONDS", timeSpan.Seconds);
		return stringBuilder.ToString();
	}

	private static void AppendLocalizedTimeText(StringBuilder buffer, string singularIdentifier, string pluralIdentifier, int wholeAmount)
	{
		if (wholeAmount > 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			object[] args = new object[2]
			{
				wholeAmount,
				Language.Get(pluralIdentifier)
			};
			buffer.AppendFormat("{0} {1}", args);
		}
		else if (wholeAmount == 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			object[] args2 = new object[2]
			{
				wholeAmount,
				Language.Get(singularIdentifier)
			};
			buffer.AppendFormat("{0} {1}", args2);
		}
	}

	public static string GetShortTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return stringBuilder.Append("> ").Append(timeSpan.Days).Append(Language.Get("S_TIME_DAY_ABBREV"))
				.ToString();
		}
		if (timeSpan.TotalHours >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Hours).Append(Language.Get("S_TIME_HOUR_ABBREV"));
		}
		if (timeSpan.TotalMinutes >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Minutes).Append(Language.Get("S_TIME_MINUTE_ABBREV"));
		}
		if (timeSpan.Seconds > 0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Seconds).Append(Language.Get("S_TIME_SECOND_ABBREV"));
		}
		return stringBuilder.ToString();
	}

	public static uint GetSecondsSince(uint startTime, uint currentTime)
	{
		return (currentTime > startTime) ? (currentTime - startTime) : 0u;
	}

	public static string GetMinutesSecondsCountdownFromSeconds(int seconds)
	{
		int num = seconds / 60;
		int num2 = seconds - num * 60;
		return string.Format("{0:00}:{1:00}", num, num2);
	}

	public static string GetMinutesSeconds(long seconds)
	{
		long num = seconds / 60;
		long num2 = seconds - num * 60;
		return string.Format("{0}:{1:00}", num, num2);
	}
}
