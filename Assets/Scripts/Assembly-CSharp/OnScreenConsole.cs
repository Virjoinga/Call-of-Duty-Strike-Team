using UnityEngine;

public class OnScreenConsole : MonoBehaviour
{
	private const int kConsoleHeight = 100;

	private static OnScreenConsole s_Instance;

	private static string m_Log;

	private string m_Output;

	public static OnScreenConsole Instance
	{
		get
		{
			if (s_Instance == null)
			{
				GameObject gameObject = new GameObject("ConsoleOutput");
				s_Instance = gameObject.AddComponent<OnScreenConsole>();
			}
			return s_Instance;
		}
	}

	public void Enable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	public void Disable()
	{
		Application.RegisterLogCallback(null);
	}

	private void HandleLog(string logString, string stack, LogType type)
	{
		if (type == LogType.Error || type == LogType.Warning)
		{
			m_Output = logString;
			m_Log = m_Log + "\n" + m_Output;
		}
	}
}
