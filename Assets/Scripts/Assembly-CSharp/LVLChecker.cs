using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LVLChecker : MonoBehaviour
{
	public enum LicenseCheckState
	{
		IDLE = 0,
		CHECKING = 1,
		NOT_LICENSED = 2,
		LICENSED = 3,
		ERROR = 4
	}

	public delegate void LVLCheckerEventHandler(LicenseCheckState state);

	public static LVLChecker Instance;

	[HideInInspector]
	public LicenseCheckState LicenseState;

	[HideInInspector]
	public string ResponseCode = "-1";

	[HideInInspector]
	public string PublicKey_Modulus_Base64 = string.Empty;

	[HideInInspector]
	public string PublicKey_Exponent_Base64 = string.Empty;

	public TextAsset ServiceBinder;

	public LVLCheckerEventHandler OnGotLicenseStatus;

	private RSAParameters m_PublicKey = default(RSAParameters);

	private AndroidJavaObject m_Activity;

	private AndroidJavaObject m_LVLCheckType;

	private string m_PackageName;

	private int m_Nonce;

	private AndroidJavaObject m_LVLCheck;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void StartCheckLicense()
	{
		if (LicenseState == LicenseCheckState.IDLE || LicenseState == LicenseCheckState.ERROR)
		{
			LicenseState = LicenseCheckState.CHECKING;
			if (PublicKey_Modulus_Base64 == string.Empty || PublicKey_Exponent_Base64 == string.Empty)
			{
				byte[] modulus = null;
				byte[] exponent = null;
				SimpleParseASN1(AndroidKeys.Instance.LVLKey, ref modulus, ref exponent);
				PublicKey_Modulus_Base64 = Convert.ToBase64String(modulus);
				PublicKey_Exponent_Base64 = Convert.ToBase64String(exponent);
			}
			LoadServiceBinder();
			new SHA1CryptoServiceProvider();
			m_Nonce = new System.Random().Next();
			object[] args = new object[1] { new AndroidJavaObject[1] { m_Activity } };
			//AndroidJavaObject[] array = m_LVLCheckType.Call<AndroidJavaObject[]>("getConstructors", new object[0]);
			//m_LVLCheck = array[0].Call<AndroidJavaObject>("newInstance", args);
			//m_LVLCheck.Call("create", m_Nonce, new AndroidJavaRunnable(Process));
		}
	}

	private void LoadServiceBinder()
	{
		byte[] bytes = ServiceBinder.bytes;
		//m_Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		//m_PackageName = m_Activity.Call<string>("getPackageName", new object[0]);
		//string text = Path.Combine(m_Activity.Call<AndroidJavaObject>("getCacheDir", new object[0]).Call<string>("getPath", new object[0]), m_PackageName);
		//Directory.CreateDirectory(text);
		//File.WriteAllBytes(text + "/classes.jar", bytes);
		//Directory.CreateDirectory(text + "/odex");
		//AndroidJavaObject androidJavaObject = new AndroidJavaObject("dalvik.system.DexClassLoader", text + "/classes.jar", text + "/odex", null, m_Activity.Call<AndroidJavaObject>("getClassLoader", new object[0]));
		//m_LVLCheckType = androidJavaObject.Call<AndroidJavaObject>("findClass", new object[1] { "com.unity3d.plugin.lvl.ServiceBinder" });
		//Directory.Delete(text, true);
	}

	private void Process()
	{
		if (m_LVLCheck == null)
		{
			return;
		}
		int num = m_LVLCheck.Get<int>("_arg0");
		string text = m_LVLCheck.Get<string>("_arg1");
		string text2 = m_LVLCheck.Get<string>("_arg2");
		m_LVLCheck = null;
		ResponseCode = num.ToString();
		if (num < 0 || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
		{
			SetAndDoCallback(LicenseCheckState.ERROR);
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		byte[] rgbSignature = Convert.FromBase64String(text2);
		m_PublicKey.Modulus = Convert.FromBase64String(PublicKey_Modulus_Base64);
		m_PublicKey.Exponent = Convert.FromBase64String(PublicKey_Exponent_Base64);
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.ImportParameters(m_PublicKey);
		SHA1Managed sHA1Managed = new SHA1Managed();
		if (!rSACryptoServiceProvider.VerifyHash(sHA1Managed.ComputeHash(bytes), CryptoConfig.MapNameToOID("SHA1"), rgbSignature))
		{
			ResponseCode = "-1";
			SetAndDoCallback(LicenseCheckState.ERROR);
			return;
		}
		int num2 = text.IndexOf(':');
		string text3 = ((num2 != -1) ? text.Substring(0, num2) : text);
		string[] array = text3.Split('|');
		if (array[0].CompareTo(num.ToString()) != 0)
		{
			ResponseCode = "-1";
			SetAndDoCallback(LicenseCheckState.ERROR);
			return;
		}
		ResponseCode = array[0];
		switch (ResponseCode)
		{
		case "0":
			SetAndDoCallback(LicenseCheckState.LICENSED);
			break;
		case "1":
			SetAndDoCallback(LicenseCheckState.NOT_LICENSED);
			break;
		case "2":
			SetAndDoCallback(LicenseCheckState.LICENSED);
			break;
		default:
			SetAndDoCallback(LicenseCheckState.ERROR);
			break;
		}
	}

	private void SetAndDoCallback(LicenseCheckState state)
	{
		LicenseState = state;
		if (OnGotLicenseStatus != null)
		{
			OnGotLicenseStatus(LicenseState);
		}
	}

	private static void SimpleParseASN1(string publicKey, ref byte[] modulus, ref byte[] exponent)
	{
		byte[] array = Convert.FromBase64String(publicKey);
		Type type = Type.GetType("Mono.Security.ASN1");
		ConstructorInfo constructor = type.GetConstructor(new Type[1] { typeof(byte[]) });
		PropertyInfo property = type.GetProperty("Value");
		PropertyInfo property2 = type.GetProperty("Item");
		object obj = constructor.Invoke(new object[1] { array });
		object value = property2.GetValue(obj, new object[1] { 1 });
		byte[] array2 = (byte[])property.GetValue(value, null);
		byte[] array3 = new byte[array2.Length - 1];
		Array.Copy(array2, 1, array3, 0, array2.Length - 1);
		obj = constructor.Invoke(new object[1] { array3 });
		object value2 = property2.GetValue(obj, new object[1] { 0 });
		object value3 = property2.GetValue(obj, new object[1] { 1 });
		modulus = (byte[])property.GetValue(value2, null);
		exponent = (byte[])property.GetValue(value3, null);
	}
}
