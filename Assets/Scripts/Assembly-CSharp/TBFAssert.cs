using UnityEngine;

public class TBFAssert : MonoBehaviour
{
	public SpriteText Title;

	public SpriteText BuildNumber;

	public SpriteText CurrentDateTime;

	public SpriteText Message;

	private static void SetStackTrace()
	{
	}

	public static void DoAssert(bool check)
	{
	}

	public static void DoAssert(bool check, string message)
	{
	}

	public static void DoAssert(bool check, string title, string message)
	{
	}

	public void Set(string title, string buildId, string message)
	{
	}
}
