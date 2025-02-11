using UnityEngine;

public class JavaBridge
{
	public enum KindleDevices
	{
		UNDEFINED = 0,
		ORIGINAL = 1,
		REGULAR = 2,
		HD7 = 3,
		HD89 = 4,
		NUMDEVICES = 5
	}

	public const int Android_OS_GingerBread = 10;

	public const int Android_OS_ICS = 15;

	private static AndroidJavaObject BedrockSupport_plugin;

	private static AndroidJavaObject BedrockInterface_plugin;

	private static AndroidJavaObject BedrockWrapper_plugin;

	public static int Android_OS;

	public static KindleDevices Kindletype;

	public static bool NonKindleDevice;

	static JavaBridge()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			Debug.Log("JavaBridge: creating BedrockSupport");
			BedrockSupport_plugin = new AndroidJavaObject("com.stovepipe.cp.bedrock.BedrockSupport", @static);
			Debug.Log("JavaBridge: creating BedrockWrapper");
			string text = "555415696224";
			string text2 = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxobir/5nwd7A4orw9ps/babd4NggUZq57A0wereH7+6kemGZHYY87frF+upCiAcyMfnijlOzvDaZfMvLhjXSg87SOG4EEWI3lYfYok2easELy9o8+i0zfXJ0hkiqu4tgDrw+nsndUpG80DO8K8w3gbWYKLVJXXvcUZPNoQA+T4P2e4dPQd92cM+faKrHvm2o8Dw3asASc/KKp6uqMh56hig7xERdJdFs6eseFnYUaJdFelze4LWAOI0x+B0IHTJdInfE0cliTby9KZbIgxeuDCl/Yf8owSW7uKcwCD5Wqz2DHLX42h7E2EivmtKfu0AInne+PSMOMJ4Pcsl1pu12wwIDAQAB";
			BedrockWrapper_plugin = new AndroidJavaObject("com.vvisions.bedrock.wrapper.BedrockWrapper", @static);
			Debug.Log("JavaBridge: creating BedrockInterface");
			BedrockInterface_plugin = new AndroidJavaObject("com.vvisions.bedrock.BedrockInterface", @static, BedrockWrapper_plugin);
			Debug.Log("JavaBridge: setting BedrockInterface on wrapper.");
			BedrockWrapper_plugin.Call("setBedrockInterface", BedrockInterface_plugin, text, text2);
			Debug.Log("JavaBridge: setting BedrockInterface on activity override.");
			@static.Call("setBedrockInterface", BedrockInterface_plugin);
			Debug.Log("JavaBridge: finished setting BedrockInterface on activity override.");
		}
	}

	public static void StartJavaBridge()
	{
		Debug.Log("StartJavaBridge");
		Debug.Log("Screen width: " + Screen.width + ", Screen height: " + Screen.height);
		if (Application.platform == RuntimePlatform.Android)
		{
			Debug.Log("Starting Java Bridge");
			BedrockSupport_plugin.Call("InitialTasks");
			Android_OS = BedrockSupport_plugin.Call<int>("Get_Android_OS", new object[0]);
			Debug.Log("Android_OS: " + Android_OS);
			if (Android_OS < 15)
			{
				Kindletype = KindleDevices.ORIGINAL;
			}
			else if ((float)Screen.width < 1280f)
			{
				Kindletype = KindleDevices.REGULAR;
			}
			else if ((float)Screen.width < 1920f)
			{
				Kindletype = KindleDevices.HD7;
			}
			else
			{
				Kindletype = KindleDevices.HD89;
			}
			Debug.Log("Kindle device is: " + Kindletype);
		}
	}

	public static bool CheckWifi()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public static bool CheckMobileNoMacAddress()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return BedrockSupport_plugin.Call<bool>("CheckMobileNoMacAddress", new object[0]);
	}

	public static bool Nag4G(bool isgoldbox)
	{
		return false;
	}
}
