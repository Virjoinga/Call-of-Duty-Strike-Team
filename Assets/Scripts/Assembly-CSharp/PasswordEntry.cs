using System.Collections;
using UnityEngine;

public class PasswordEntry : MonoBehaviour
{
	private enum PasswordEntryState
	{
		DeviceCheck = 0,
		InvalidDevice = 1,
		WaitingPassword = 2,
		StartPasswordCheck = 3,
		CheckingPassword = 4,
		IncorrectPassword = 5,
		LaunchLevel = 6,
		Complete = 7
	}

	private string m_username = string.Empty;

	private string m_password = string.Empty;

	private PasswordEntryState m_currentState;

	private bool m_internalServerPinged = true;

	private void Awake()
	{
	}

	private void Start()
	{
		m_currentState = PasswordEntryState.DeviceCheck;
		m_internalServerPinged = false;
		StartCoroutine(PingInternalServer());
	}

	private void Update()
	{
		switch (m_currentState)
		{
		case PasswordEntryState.DeviceCheck:
			SwitchState((!IsDeviceValid()) ? PasswordEntryState.InvalidDevice : PasswordEntryState.WaitingPassword);
			break;
		case PasswordEntryState.StartPasswordCheck:
			if (m_username.Length > 0 && m_password.Length > 0)
			{
				StartCoroutine(CheckPassword(m_username, m_password));
			}
			else if (m_internalServerPinged)
			{
				StartCoroutine(FakePasswordCheck());
			}
			else
			{
				SwitchState(PasswordEntryState.WaitingPassword);
			}
			break;
		case PasswordEntryState.LaunchLevel:
			DoLaunch();
			break;
		}
	}

	private void SwitchState(PasswordEntryState nextState)
	{
		if (m_currentState != nextState)
		{
			m_currentState = nextState;
		}
	}

	private bool IsDeviceValid()
	{
		return true;
	}

	private void DoLaunch()
	{
		SwitchState(PasswordEntryState.Complete);
		Application.LoadLevel("AndroidBootCheck");
	}

	private void DisplayBuildInfo(bool ipad3)
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = Color.white;
		gUIStyle.fontSize = ((!ipad3) ? 20 : 40);
		GUILayout.Label(SystemInfo.deviceName + " " + SystemInfo.deviceType, gUIStyle);
		GUILayout.Label("Spec: " + OptimisationManager.GetCurrentHardware().ToString() + "  Phone:" + OptimisationManager.GetAndroidPhoneModel(), gUIStyle);
		GUILayout.Label(SystemInfo.operatingSystem + " " + SystemInfo.systemMemorySize, gUIStyle);
		GUILayout.Label(Screen.width + "x" + Screen.height, gUIStyle);
		GUILayout.Label("Internal Build: " + m_internalServerPinged, gUIStyle);
		Exclamation.ExclamationsEnabled = GUILayout.Toggle(Exclamation.ExclamationsEnabled, "Exclamations?");
		FrameRateAlert.DoFrameRateAlert = GUILayout.Toggle(FrameRateAlert.DoFrameRateAlert, "Frame Rate Alert?");
		GUILayout.Space((!ipad3) ? 50 : 100);
	}

	private void DisplayInvalidDevice()
	{
		bool flag = TBFUtils.IsRetinaHdDevice();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = Color.red;
		gUIStyle.fontSize = ((!flag) ? 50 : 100);
		DisplayBuildInfo(flag);
		gUIStyle.fontSize = 15;
		gUIStyle.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label("Due to memory requirements, this week's build will only run on iPhone 5, iPad's 3 and 4", gUIStyle);
	}

	private PasswordEntryState DisplayWaitForPassword()
	{
		PasswordEntryState result = PasswordEntryState.WaitingPassword;
		bool flag = TBFUtils.IsRetinaHdDevice();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.fontSize = ((!flag) ? 20 : 40);
		gUIStyle.normal.textColor = Color.white;
		DisplayBuildInfo(flag);
		GUILayout.Space((!flag) ? 50 : 100);
		GUILayout.Label("Username:", gUIStyle);
		m_username = GUILayout.TextField(m_username, GUILayout.MaxHeight((!flag) ? 50 : 100));
		GUILayout.Label("Password:", gUIStyle);
		m_password = GUILayout.PasswordField(m_password, "*"[0], GUILayout.MaxHeight((!flag) ? 50 : 100));
		GUILayout.Space((!flag) ? 50 : 100);
		if (GUILayout.Button("GO", GUILayout.Width(200f), GUILayout.Height(100f)))
		{
			result = PasswordEntryState.StartPasswordCheck;
		}
		return result;
	}

	private void DisplayCheckingPassword()
	{
		bool flag = TBFUtils.IsRetinaHdDevice();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.fontSize = ((!flag) ? 20 : 40);
		gUIStyle.normal.textColor = Color.white;
		DisplayBuildInfo(flag);
		GUILayout.Space((!flag) ? 50 : 100);
		GUILayout.Label("Checking...", gUIStyle);
	}

	private PasswordEntryState DisplayIncorrectPassword()
	{
		PasswordEntryState result = PasswordEntryState.IncorrectPassword;
		bool flag = TBFUtils.IsRetinaHdDevice();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.fontSize = ((!flag) ? 20 : 40);
		gUIStyle.normal.textColor = Color.white;
		DisplayBuildInfo(flag);
		GUILayout.Space((!flag) ? 50 : 100);
		GUILayout.Label("Password Incorrect", gUIStyle);
		GUILayout.Space((!flag) ? 50 : 100);
		if (GUILayout.Button("TRY AGAIN", GUILayout.Width(200f), GUILayout.Height(100f)))
		{
			result = PasswordEntryState.WaitingPassword;
		}
		return result;
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width));
		PasswordEntryState nextState = m_currentState;
		switch (m_currentState)
		{
		case PasswordEntryState.InvalidDevice:
			DisplayInvalidDevice();
			break;
		case PasswordEntryState.WaitingPassword:
			nextState = DisplayWaitForPassword();
			break;
		case PasswordEntryState.CheckingPassword:
			DisplayCheckingPassword();
			break;
		case PasswordEntryState.IncorrectPassword:
			nextState = DisplayIncorrectPassword();
			break;
		}
		GUILayout.EndVertical();
		SwitchState(nextState);
	}

	private IEnumerator PingInternalServer()
	{
		yield return new WaitForSeconds(1f);
		string internalServer = "http://10.234.30.65/";
		WWW www = new WWW(internalServer);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			m_internalServerPinged = false;
		}
		else
		{
			m_internalServerPinged = true;
		}
	}

	private IEnumerator CheckPassword(string username, string password)
	{
		SwitchState(PasswordEntryState.CheckingPassword);
		string url = "http://tbfappverify.appspot.com/";
		bool credentialsValid = false;
		WWWForm formData = new WWWForm();
		formData.AddField("application", "corona");
		formData.AddField("username", username.ToLower());
		formData.AddField("password", password);
		WWW www = new WWW(url, formData);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			credentialsValid = false;
		}
		else
		{
			Hashtable results = www.text.hashtableFromJson();
			if (results != null && results.ContainsKey("status"))
			{
				int status = 0;
				if (int.TryParse(results["status"].ToString(), out status))
				{
					credentialsValid = status == 1;
				}
			}
		}
		if (credentialsValid)
		{
			SwitchState(PasswordEntryState.LaunchLevel);
		}
		else
		{
			SwitchState(PasswordEntryState.IncorrectPassword);
		}
	}

	private IEnumerator FakePasswordCheck()
	{
		SwitchState(PasswordEntryState.CheckingPassword);
		yield return new WaitForSeconds(0.2f);
		SwitchState(PasswordEntryState.LaunchLevel);
	}
}
