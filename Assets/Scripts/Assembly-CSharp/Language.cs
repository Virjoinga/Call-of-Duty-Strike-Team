using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class Language
{
	public static string settingsAssetPath;

	public static LocalizationSettings settings;

	private static List<string> availableLanguages;

	private static LanguageCode currentLanguage;

	private static Dictionary<string, Hashtable> currentEntrySheets;

	static Language()
	{
		settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";
		settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));
		currentLanguage = LanguageCode.N;
		LoadAvailableLanguages();
		bool useSystemLanguagePerDefault = settings.useSystemLanguagePerDefault;
		LanguageCode code = LocalizationSettings.GetLanguageEnum(settings.defaultLangCode);
		if (useSystemLanguagePerDefault)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault", new object[0]);
			string langCode = androidJavaObject.Call<string>("getLanguage", new object[0]);
			LanguageCode languageEnum = LocalizationSettings.GetLanguageEnum(langCode);
			if (languageEnum == LanguageCode.N)
			{
				string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName != "iv")
				{
					languageEnum = LocalizationSettings.GetLanguageEnum(twoLetterISOLanguageName);
				}
			}
			if (availableLanguages.Contains(string.Concat(languageEnum, string.Empty)))
			{
				code = languageEnum;
			}
		}
		string @string = PlayerPrefs.GetString("M2H_lastLanguage", string.Empty);
		if (@string != string.Empty && availableLanguages.Contains(@string))
		{
			SwitchLanguage(@string);
		}
		else
		{
			SwitchLanguage(code);
		}
	}

	private static void LoadAvailableLanguages()
	{
		availableLanguages = new List<string>();
		Debug.Log(settings);
		if (settings.sheetTitles == null || settings.sheetTitles.Length <= 0)
		{
			Debug.Log("None available");
			return;
		}
		foreach (int value in Enum.GetValues(typeof(LanguageCode)))
		{
			if (HasLanguageFile(string.Concat((LanguageCode)value, string.Empty), settings.sheetTitles[0]))
			{
				availableLanguages.Add(string.Concat((LanguageCode)value, string.Empty));
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static string[] GetLanguages()
	{
		return availableLanguages.ToArray();
	}

	public static bool SwitchLanguage(string langCode)
	{
		return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}

	public static bool SwitchLanguage(LanguageCode code)
	{
		if (availableLanguages.Contains(string.Concat(code, string.Empty)))
		{
			DoSwitch(code);
			return true;
		}
		Debug.LogError(string.Concat("Could not switch from language ", currentLanguage, " to ", code));
		if (currentLanguage == LanguageCode.N)
		{
			if (availableLanguages.Count > 0)
			{
				DoSwitch(LocalizationSettings.GetLanguageEnum(availableLanguages[0]));
				Debug.LogError(string.Concat("Switched to ", currentLanguage, " instead"));
			}
			else
			{
				Debug.LogError(string.Concat("Please verify that you have the file: Resources/Languages/", code, string.Empty));
				Debug.Break();
			}
		}
		return false;
	}

	private static void DoSwitch(LanguageCode newLang)
	{
		PlayerPrefs.GetString("M2H_lastLanguage", string.Concat(newLang, string.Empty));
		currentLanguage = newLang;
		currentEntrySheets = new Dictionary<string, Hashtable>();
		XMLParser xMLParser = new XMLParser();
		string[] sheetTitles = settings.sheetTitles;
		foreach (string text in sheetTitles)
		{
			currentEntrySheets[text] = new Hashtable();
			Hashtable hashtable = (Hashtable)xMLParser.Parse(GetLanguageFileContents(text));
			ArrayList arrayList = (ArrayList)(((ArrayList)hashtable["entries"])[0] as Hashtable)["entry"];
			foreach (Hashtable item in arrayList)
			{
				string text2 = (string)item["@name"];
				string s = string.Concat(item["_text"], string.Empty).Trim();
				s = s.UnescapeXML();
				if (TBFUtils.UseAlternativeLayout() && text2.StartsWith("AB_"))
				{
					text2 = "S_" + text2.Substring(3);
				}
				currentEntrySheets[text][text2] = s;
			}
		}
		LocalizedAsset[] array = (LocalizedAsset[])UnityEngine.Object.FindObjectsOfType(typeof(LocalizedAsset));
		LocalizedAsset[] array2 = array;
		foreach (LocalizedAsset localizedAsset in array2)
		{
			localizedAsset.LocalizeAsset();
		}
		SendMonoMessage("ChangedLanguage", currentLanguage);
	}

	public static UnityEngine.Object GetAsset(string name)
	{
		return Resources.Load(string.Concat("Languages/Assets/", CurrentLanguage(), "/", name));
	}

	private static bool HasLanguageFile(string lang, string sheetTitle)
	{
		return (TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null;
	}

	private static string GetLanguageFileContents(string sheetTitle)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(string.Concat("Languages/", currentLanguage, "_", sheetTitle), typeof(TextAsset));
		return textAsset.text;
	}

	public static LanguageCode CurrentLanguage()
	{
		return currentLanguage;
	}

	public static string GetWithFallback(string key, string fallbackKey)
	{
		if (currentEntrySheets == null)
		{
			return string.Empty;
		}
		foreach (Hashtable value in currentEntrySheets.Values)
		{
			if (value.ContainsKey(key))
			{
				return (string)value[key];
			}
		}
		foreach (Hashtable value2 in currentEntrySheets.Values)
		{
			if (value2.ContainsKey(fallbackKey))
			{
				return (string)value2[fallbackKey];
			}
		}
		return "MISSING LANG:" + key;
	}

	public static string Get(string key)
	{
		if (currentEntrySheets == null)
		{
			return string.Empty;
		}
		foreach (Hashtable value in currentEntrySheets.Values)
		{
			if (value.ContainsKey(key))
			{
				return (string)value[key];
			}
		}
		return "MISSING LANG:" + key;
	}

	public static string GetFormatString(string key, params object[] args)
	{
		return string.Format(Get(key), args);
	}

	public static string Get(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			Debug.LogError("The sheet with title \"" + sheetTitle + "\" does not exist!");
			return string.Empty;
		}
		if (currentEntrySheets[sheetTitle].ContainsKey(key))
		{
			return (string)currentEntrySheets[sheetTitle][key];
		}
		return "MISSING LANG:" + key;
	}

	private static void SendMonoMessage(string methodString, params object[] parameters)
	{
		if (parameters != null && parameters.Length > 1)
		{
			Debug.LogError("We cannot pass more than one argument currently!");
		}
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject && gameObject.transform.parent == null)
			{
				if (parameters != null && parameters.Length == 1)
				{
					gameObject.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static LanguageCode LanguageNameToCode(SystemLanguage name)
	{
		switch (name)
		{
		case SystemLanguage.Afrikaans:
			return LanguageCode.AF;
		case SystemLanguage.Arabic:
			return LanguageCode.AR;
		case SystemLanguage.Basque:
			return LanguageCode.BA;
		case SystemLanguage.Belarusian:
			return LanguageCode.BE;
		case SystemLanguage.Bulgarian:
			return LanguageCode.BG;
		case SystemLanguage.Catalan:
			return LanguageCode.CA;
		case SystemLanguage.Chinese:
			return LanguageCode.ZH;
		case SystemLanguage.Czech:
			return LanguageCode.CS;
		case SystemLanguage.Danish:
			return LanguageCode.DA;
		case SystemLanguage.Dutch:
			return LanguageCode.NL;
		case SystemLanguage.English:
			return LanguageCode.EN;
		case SystemLanguage.Estonian:
			return LanguageCode.ET;
		case SystemLanguage.Faroese:
			return LanguageCode.FA;
		case SystemLanguage.Finnish:
			return LanguageCode.FI;
		case SystemLanguage.French:
			return LanguageCode.FR;
		case SystemLanguage.German:
			return LanguageCode.DE;
		case SystemLanguage.Greek:
			return LanguageCode.EL;
		case SystemLanguage.Hebrew:
			return LanguageCode.HE;
		case SystemLanguage.Hungarian:
			return LanguageCode.HU;
		case SystemLanguage.Icelandic:
			return LanguageCode.IS;
		case SystemLanguage.Indonesian:
			return LanguageCode.ID;
		case SystemLanguage.Italian:
			return LanguageCode.IT;
		case SystemLanguage.Japanese:
			return LanguageCode.JA;
		case SystemLanguage.Korean:
			return LanguageCode.KO;
		case SystemLanguage.Latvian:
			return LanguageCode.LA;
		case SystemLanguage.Lithuanian:
			return LanguageCode.LT;
		case SystemLanguage.Norwegian:
			return LanguageCode.NO;
		case SystemLanguage.Polish:
			return LanguageCode.PL;
		case SystemLanguage.Portuguese:
			return LanguageCode.PT;
		case SystemLanguage.Romanian:
			return LanguageCode.RO;
		case SystemLanguage.Russian:
			return LanguageCode.RU;
		case SystemLanguage.SerboCroatian:
			return LanguageCode.SH;
		case SystemLanguage.Slovak:
			return LanguageCode.SK;
		case SystemLanguage.Slovenian:
			return LanguageCode.SL;
		case SystemLanguage.Spanish:
			return LanguageCode.ES;
		case SystemLanguage.Swedish:
			return LanguageCode.SW;
		case SystemLanguage.Thai:
			return LanguageCode.TH;
		case SystemLanguage.Turkish:
			return LanguageCode.TR;
		case SystemLanguage.Ukrainian:
			return LanguageCode.UK;
		case SystemLanguage.Vietnamese:
			return LanguageCode.VI;
		default:
			switch (name)
			{
			case SystemLanguage.Hungarian:
				return LanguageCode.HU;
			case SystemLanguage.Unknown:
				return LanguageCode.N;
			default:
				return LanguageCode.N;
			}
		}
	}

	public static void UpdateLanguageFromBedrock()
	{
		Dictionary<string, string> resourceDictionary = null;
		if (!Bedrock.GetRemoteUserResources("updatedStrings", out resourceDictionary) || resourceDictionary == null || currentEntrySheets == null)
		{
			return;
		}
		string key = currentLanguage.ToString();
		List<string> list = new List<string>();
		foreach (string key2 in resourceDictionary.Keys)
		{
			if (key2.StartsWith("S_"))
			{
				list.Add(key2);
			}
		}
		foreach (string key3 in currentEntrySheets.Keys)
		{
			Hashtable hashtable = currentEntrySheets[key3];
			foreach (string item in list)
			{
				Dictionary<string, string> resourceDictionary2 = null;
				if (Bedrock.GetRemoteUserResources(item, out resourceDictionary2) && resourceDictionary2 != null && resourceDictionary2.ContainsKey(key))
				{
					hashtable[item] = resourceDictionary2[key];
				}
			}
		}
	}
}
