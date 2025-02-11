using UnityEngine;

public static class SceneLoader
{
	private static string mSceneName;

	private static string mSceneNameTeleport;

	private static int mTeleportID = -1;

	private static bool mDoTeleport;

	private static bool mIsTeleporting;

	public static string SceneName
	{
		get
		{
			return mSceneName;
		}
	}

	public static string SceneNameTeleport
	{
		get
		{
			return mSceneNameTeleport;
		}
	}

	public static int TeleportID
	{
		get
		{
			return mTeleportID;
		}
	}

	public static bool DoTeleport
	{
		get
		{
			return mDoTeleport;
		}
		set
		{
			mDoTeleport = value;
		}
	}

	public static bool IsTeleporting
	{
		get
		{
			return mIsTeleporting;
		}
		set
		{
			mIsTeleporting = value;
		}
	}

	public static void AsyncLoadSceneWithLoadingScreen(string sceneName)
	{
		SectionLoader.ClearSceneName();
		mSceneName = sceneName;
		mDoTeleport = false;
		LoadLoadingScreen();
	}

	public static void ClearSceneName()
	{
		mSceneName = string.Empty;
	}

	public static void AsyncLoadSceneWithLoadingScreen(string sceneName, string sceneNameTeleport, int teleportID)
	{
		mSceneName = sceneName;
		mDoTeleport = false;
		if (teleportID != 0)
		{
			mTeleportID = teleportID;
			mSceneNameTeleport = sceneNameTeleport;
			mDoTeleport = true;
		}
		LoadLoadingScreen();
	}

	private static void LoadLoadingScreen()
	{
		GameSettings.DisableLoadoutAndBriefing = false;
		SoundManager.Instance.ActivateLoadingScreenSFX();
		Application.LoadLevel("loadingScreen");
	}
}
