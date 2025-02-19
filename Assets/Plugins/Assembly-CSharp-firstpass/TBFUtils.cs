using System;
using System.Collections.Generic;
using UnityEngine;

public class TBFUtils
{
	private static AndroidJavaObject TBFUtilsObject;

	public static string BuildTime
	{
		get
		{
			return "--:--:--";
		}
	}

	public static string BuildDate
	{
		get
		{
			return "<not build>";
		}
	}

	public static string BuildVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return CallTBFJar<string>("getBuildVersion");
			}
			return BundleVersion;
		}
	}

	public static string BundleVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "1.0.40";
			}
			return "1.0";
		}
	}

	public static bool IsMusicPlaying
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return false;
			}
			return false;
		}
	}

	public static float VolumeLevel
	{
		get
		{
			return 1f;
		}
	}

	public static string IDFV
	{
		get
		{
			return null;
		}
	}

	public static bool IsCracked
	{
		get
		{
			if (Application.genuineCheckAvailable)
			{
				return Application.genuine;
			}
			return false;
		}
	}

	public static string iPhoneGen
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return SystemInfo.deviceModel;
			}
			return "Unknown";
		}
	}

	public static string DeviceManfacturer
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return CallTBFJar<string>("getDeviceManufacturer");
			}
			return "Unknown";
		}
	}

	public static string DeviceModel
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return CallTBFJar<string>("getDeviceModel");
			}
			return "Unknown";
		}
	}

	public static string OSVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return CallTBFJar<string>("getOSVersion");
			}
			return "Unknown";
		}
	}

	public static void UberGarbageCollect()
	{
		GC.Collect();
		GC.Collect();
		GC.Collect();
		GC.Collect();
		GC.Collect();
		GC.Collect();
	}

	public static void RegisterSceneUndo(string undoLabel)
	{
	}

	public static GameObject[] FindSceneObjectsOfType(Type type)
	{
		List<GameObject> list = new List<GameObject>();
		return list.ToArray();
	}

	private static AndroidJavaObject GetTBFUtilsObject()
	{
		if (TBFUtilsObject == null)
		{
#if UNITY_ANDROID
			TBFUtilsObject = new AndroidJavaObject("com.activision.TBFUtils");
#endif
			if (TBFUtilsObject == null)
			{
				DebugLog("Could not create java object for TBFUtils");
			}
		}
		return TBFUtilsObject;
	}

	public static void AndroidShowMessageBox(string title, string message, string buttonMessage, string gameObject, string callbackMethod)
	{
		CallTBFJar("showMessageBox", title, message, buttonMessage, gameObject, callbackMethod);
	}

	public static void CallTBFJar(string function)
	{
		AndroidJavaObject tBFUtilsObject = GetTBFUtilsObject();
		if (tBFUtilsObject != null)
		{
			tBFUtilsObject.Call(function);
			DebugLog("function: " + function);
		}
		else
		{
			DebugLog("TBFUtilsObject was null " + function);
		}
	}

	public static void CallTBFJar(string function, params object[] args)
	{
		AndroidJavaObject tBFUtilsObject = GetTBFUtilsObject();
		if (tBFUtilsObject != null)
		{
			tBFUtilsObject.Call(function, args);
			DebugLog("function: " + function);
		}
		else
		{
			DebugLog("TBFUtilsObject was null " + function);
		}
	}

	public static T CallTBFJar<T>(string function)
	{
		AndroidJavaObject tBFUtilsObject = GetTBFUtilsObject();
		if (tBFUtilsObject != null)
		{
			T result = tBFUtilsObject.Call<T>(function, new object[0]);
			DebugLog("function: " + function + " " + result.ToString());
			return result;
		}
		DebugLog("TBFUtilsObject was null " + function);
		return default(T);
	}

	public static void StackTrace(string header)
	{
	}

	public static void PressKInvite()
	{
	}

	public static void CheckKInviteForRewards()
	{
	}

	public static void LaunchURL(string URL)
	{
		Application.OpenURL(URL);
	}

	public static bool CanOpenURL(string URL)
	{
		return false;
	}

	public static void LaunchStoreForRating()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CallTBFJar("launchMarket");
		}
	}

	public static void AndroidTweet(string text, string url)
	{
		CallTBFJar("launchTwitter", text, url);
	}

	public static bool IsUnsupportedDevice()
	{
		return false;
	}

	public static bool Is256mbDevice()
	{
		int androidSystemMemory = GetAndroidSystemMemory();
		if (androidSystemMemory < 512 && androidSystemMemory >= 0)
		{
			return true;
		}
		return false;
	}

	public static bool Is512mbDevice()
	{
		Debug.LogWarning("Warning calling Is512mbDevice() - this isnt maintained anymore Should use OptimisationManager instead and check for GB device");
		return Is256mbDevice();
	}

	public static bool IsHighMemoryAndroidDevice()
	{
		int androidSystemMemory = GetAndroidSystemMemory();
		Debug.Log("IsHighMemoryAndroidDevice: " + androidSystemMemory);
		return androidSystemMemory > 1700 || androidSystemMemory < 0;
	}

	public static int GetAndroidSystemMemory()
	{
		return 2400; // CallTBFJar<int>("readTotalRam");
	}

	public static bool IsSmallScreenDevice()
	{
		if (Screen.height <= 640)
		{
			return true;
		}
		return false;
	}

	public static bool UseAlternativeLayout()
	{
		if (Screen.height < 600 || Screen.height == 1080 || Screen.height == 1104)
		{
			return true;
		}
		return false;
	}

	public static bool IsSmallRetinaDevice()
	{
		if ((Screen.height >= 1080 && Screen.height < 1536) || Screen.height == 1104)
		{
			return true;
		}
		return false;
	}

	public static bool IsRetinaHdDevice()
	{
		if (Screen.height >= 1080)
		{
			return true;
		}
		return false;
	}

	public static bool IsWideScreenDevice()
	{
		return (float)Screen.width / (float)Screen.height > 1.6f;
	}

	public static bool IsKindleFire()
	{
		if (DeviceManfacturer.Contains("Amazon") && DeviceModel.StartsWith("KF"))
		{
			return true;
		}
		return false;
	}

	public static void DebugLog(string strMsg)
	{
	}

	public static void StartAutoProfile(string profileName)
	{
	}

	public static void SetAutoProfileTag(string tagName)
	{
	}

	public static void StopAutoProfile()
	{
	}

	public static void PostResults()
	{
	}
}
