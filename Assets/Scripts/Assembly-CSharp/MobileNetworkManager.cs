using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileNetworkManager : MonoBehaviour
{
	public static MobileNetworkManager m_instance;

	private IMobileNetworkImpl m_impl;

	private bool m_isLoggedIn;

	private bool m_loginInProgress;

	private bool m_waitingForLogin;

	private bool m_firstLogin;

	private float loginStartTime;

	private float lastErrorTime;

	private bool m_loginErrorShown;

	public static MobileNetworkManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsLoggedIn
	{
		get
		{
			return m_isLoggedIn;
		}
	}

	public bool IsLoginInProgress
	{
		get
		{
			return m_loginInProgress;
		}
	}

	public bool SupportsAchievements
	{
		get
		{
			bool result = false;
			if (m_impl != null)
			{
				result = m_impl.supportsAchievements();
			}
			return result;
		}
	}

	public string PlayerAlias
	{
		get
		{
			if (m_impl != null)
			{
				return m_impl.PlayerAlias();
			}
			return string.Empty;
		}
	}

	public static event Action playerAuthenticated;

	public static event Action<string> playerFailedToAuthenticate;

	public static event Action playerLoggedOut;

	public static event Action<string> loadAchievementsFailed;

	public static event Action<List<MobileNetworkAchievement>> achievementsLoaded;

	private void Awake()
	{
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		m_isLoggedIn = false;
		CreateNetworkImpl();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void CreateNetworkImpl()
	{
	}

	public void getAchievements()
	{
		if (m_impl != null)
		{
			m_impl.getAchievements();
		}
	}

	public void reportAchievement(string AchievementID, float PercentComplete)
	{
		if (m_impl != null)
		{
			m_impl.reportAchievement(AchievementID, PercentComplete);
		}
	}

	public void showAchievementsBanner()
	{
		if (m_impl != null)
		{
			m_impl.showAchievementsBanner();
		}
	}

	public void resetAchievements()
	{
		if (m_impl != null)
		{
			m_impl.resetAchievements();
		}
	}

	public void showAchievements()
	{
		if (!m_isLoggedIn)
		{
			LoginWaitAndPerformAction(showAchievements);
		}
		else
		{
			m_impl.showAchievements();
		}
	}

	public void Login()
	{
		m_firstLogin = true;
		StartLogin();
	}

	private bool StartLogin()
	{
		bool result = false;
		if (!m_loginInProgress)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable || !m_impl.IsMobileNetworkAvailable())
			{
				OnNoNetwork();
			}
			else if (!m_impl.IsPlayerAuthenticated())
			{
				if (m_firstLogin)
				{
					m_firstLogin = false;
					m_impl.AuthenticateLocalPlayer();
					m_loginInProgress = true;
					result = true;
					loginStartTime = Time.realtimeSinceStartup;
				}
				else
				{
					ShowNetDisabled();
				}
			}
		}
		return result;
	}

	private void LoginWaitAndPerformAction(Action onComplete)
	{
		if (m_waitingForLogin)
		{
			return;
		}
		bool flag = false;
		if (m_loginInProgress)
		{
			flag = true;
		}
		else
		{
			m_loginErrorShown = false;
			if (StartLogin())
			{
				flag = true;
			}
		}
		if (flag)
		{
			StartCoroutine(WaitForLogin(onComplete));
		}
		m_waitingForLogin = flag;
	}

	private IEnumerator WaitForLogin(Action onComplete)
	{
		EtceteraPlatformWrapper.ShowWaitingWithLabel(Language.Get("S_NET_CONNECTING_DROID"));
		if (UIManager.instance != null)
		{
			UIManager.instance.blockInput = true;
		}
		while (m_loginInProgress)
		{
			yield return null;
		}
		EtceteraPlatformWrapper.HideWaitingDialog();
		if (UIManager.instance != null)
		{
			UIManager.instance.blockInput = false;
		}
		if (m_isLoggedIn)
		{
			onComplete();
		}
		else if (!m_loginErrorShown && !m_firstLogin)
		{
			OnLoginFailed();
		}
		m_firstLogin = false;
		m_loginInProgress = false;
		m_waitingForLogin = false;
	}

	private void ShowSysDialog(string TitleKey, string BodyKey)
	{
		EtceteraPlatformWrapper.ShowAlert(Language.Get(TitleKey), Language.Get(BodyKey), Language.Get("S_OKAY"));
	}

	private void OnNoNetwork()
	{
		ShowSysDialog("S_NET_NONETWORK_TITLE", "S_NET_NONETWORK_BODY_DROID");
	}

	public void NoNetworkSocialSysDialog()
	{
		ShowSysDialog("S_NET_NONETWORK_TITLE", "S_NET_NONETWORK_SOCIAL_BODY");
	}

	private void OnLoginFailed()
	{
		ShowSysDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID");
	}

	private void OnLoginCancelled()
	{
		ShowNetDisabled();
	}

	private void ShowNetDisabled()
	{
		ShowSysDialog("S_NET_DISABLED_TITLE_DROID", "S_NET_DISABLED_BODY_DROID");
	}

	public void _OnPlayerAuthenticated()
	{
		TBFUtils.DebugLog(m_impl.PlayerAlias() + " logged into mobile network");
		m_isLoggedIn = true;
		m_loginInProgress = false;
		m_firstLogin = false;
		if (MobileNetworkManager.playerAuthenticated != null)
		{
			MobileNetworkManager.playerAuthenticated();
		}
	}

	public void _OnPlayerNotAuthenticated(string Error)
	{
		TBFUtils.DebugLog("Failed to log into mobile network: " + Error);
		m_isLoggedIn = false;
		m_loginInProgress = false;
		float num = Time.realtimeSinceStartup - loginStartTime;
		if (Error.Contains("cancelled"))
		{
			if (num < 0.5f)
			{
				m_loginErrorShown = true;
				OnLoginCancelled();
			}
		}
		else if (Time.realtimeSinceStartup - lastErrorTime > 1f)
		{
			lastErrorTime = Time.realtimeSinceStartup;
			m_loginErrorShown = true;
			OnLoginFailed();
		}
		if (MobileNetworkManager.playerFailedToAuthenticate != null)
		{
			MobileNetworkManager.playerFailedToAuthenticate(Error);
		}
	}

	public void _OnPlayerLoggedOut()
	{
		TBFUtils.DebugLog("Player logged out");
		m_isLoggedIn = false;
		m_loginInProgress = false;
		if (MobileNetworkManager.playerLoggedOut != null)
		{
			MobileNetworkManager.playerLoggedOut();
		}
	}

	public void _OnAchievementsLoaded(List<MobileNetworkAchievement> Achievements)
	{
		TBFUtils.DebugLog("Achievements Loaded ---> " + ((MobileNetworkManager.achievementsLoaded != null) ? "Has a callback" : "NULL"));
		if (MobileNetworkManager.achievementsLoaded != null)
		{
			Debug.Log("Calling callback");
			MobileNetworkManager.achievementsLoaded(Achievements);
		}
	}

	public void _OnAchievementLoadFailed(string error)
	{
		TBFUtils.DebugLog("Achievement Load Failed");
		if (MobileNetworkManager.loadAchievementsFailed != null)
		{
			MobileNetworkManager.loadAchievementsFailed(error);
		}
	}
}
