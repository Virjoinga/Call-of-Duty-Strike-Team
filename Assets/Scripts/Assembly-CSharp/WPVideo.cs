using System;

public class WPVideo
{
	public static event EventHandler<WPVideoEventArgs> videoPlayed;

	public static void PlayVideo(object caller, string videoName)
	{
		WPVideoEventArgs wPVideoEventArgs = new WPVideoEventArgs();
		wPVideoEventArgs.VideoName = videoName;
		WPVideo.videoPlayed(caller, wPVideoEventArgs);
	}
}
