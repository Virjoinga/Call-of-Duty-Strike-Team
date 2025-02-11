using UnityEngine;

public class TwitterPlugin : MonoBehaviour
{
	public static bool isAvailable
	{
		get
		{
			return false;
		}
	}

	public static void ComposeTweet(string initialText)
	{
		ComposeTweet(initialText, string.Empty);
	}

	public static void ComposeTweet(string initialText, string url)
	{
		TBFUtils.AndroidTweet(initialText, url);
	}

	public static void ComposeTweetWithScreenshot(string initialText)
	{
		ComposeTweet(initialText, string.Empty);
	}

	public static void ComposeTweetWithScreenshot(string initialText, string url)
	{
		ComposeTweet(initialText, url);
	}
}
