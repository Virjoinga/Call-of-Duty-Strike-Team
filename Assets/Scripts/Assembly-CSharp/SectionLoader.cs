using UnityEngine;

public static class SectionLoader
{
	public static bool SectionLoadTriggered;

	private static string m_SceneName;

	public static string SceneName
	{
		get
		{
			return m_SceneName;
		}
	}

	public static void AsyncLoadSceneWithLoadingScreen(string sceneName)
	{
		int num = sceneName.LastIndexOf('/') + 1;
		int num2 = sceneName.LastIndexOf('.');
		if (num2 > num)
		{
			sceneName = sceneName.Substring(num, num2 - num);
		}
		SceneLoader.ClearSceneName();
		m_SceneName = sceneName;
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.PlayLoadOutMusic();
		}
		if (TimeManager.instance != null)
		{
			TimeManager.instance.PauseGame();
		}
		Application.LoadLevel("loadingSection");
		SectionLoadTriggered = true;
	}

	public static void ClearSceneName()
	{
		m_SceneName = string.Empty;
	}
}
