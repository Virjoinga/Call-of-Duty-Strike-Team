using System;
using System.Collections.Generic;
using UnityEngine;

public class OptimisationManager : ScriptableObject, iSwrveUpdatable
{
	public enum OptimisationType
	{
		DetailMapping = 0,
		EnvMapping = 1,
		Corona = 2,
		FluffParticles = 3,
		FakeExplosions = 4,
		ColourCorrection = 5,
		DetailedShadows = 6,
		Ragdoll = 7,
		NativeRes = 8,
		DetailedGuns = 9,
		DetailedWater = 10,
		PropSpecMaps = 11,
		Weather = 12,
		HighEnd = 13,
		Bloom = 14,
		DepthOfField = 15,
		FakeSoldiers = 16,
		ImpactDecals = 17,
		GlobeSatalites = 18,
		SmoothOffScreenBlips = 19,
		LowPassSndFilter = 20,
		LightHalos = 21,
		FluffMesh = 22,
		FarClipAtFogDistance = 23,
		FluffGUI = 24,
		BulletCasings = 25,
		GByteDevice = 26,
		LotsOfDecals = 27,
		DisableRateTheApp = 28,
		EnableAntiAliasing = 29,
		EnableKInvite = 30,
		RefractionEffects = 31,
		NumOptimationTypes = 32
	}

	public enum HardwareType
	{
		Iphone4 = 0,
		Iphone4S = 1,
		Iphone5 = 2,
		Ipad2 = 3,
		Ipad3 = 4,
		Ipad4 = 5,
		IpadMini = 6,
		Ipod5 = 7,
		Low = 8,
		Med = 9,
		High = 10,
		Iphone5S = 11,
		Iphone5C = 12,
		AndroidHigh = 13,
		AndroidMed = 14,
		AndroidLow = 15,
		WP8 = 16,
		Ipad5 = 17,
		IpadMini2 = 18,
		NumHardwareTypes = 19
	}

	public class UpdateMask
	{
		public int mOnScreenMask;

		public int mOnScreenCount;

		public int mOffScreenMask;

		public int mOffScreenCount;

		public UpdateMask()
		{
			mOnScreenMask = -1;
			mOffScreenMask = -1;
			mOnScreenCount = 0;
			mOffScreenCount = 0;
		}

		public bool Update(Actor a)
		{
			if (a == null || a.OnScreen)
			{
				mOnScreenCount = (mOnScreenCount + 1) & 0x1F;
				return (mOnScreenMask & (1 << mOnScreenCount)) != 0;
			}
			mOffScreenCount = (mOffScreenCount + 1) & 0x1F;
			return (mOffScreenMask & (1 << mOffScreenCount)) != 0;
		}

		public void PerFramePrep()
		{
			mOnScreenCount = Time.frameCount;
			mOffScreenCount = Time.frameCount;
		}
	}

	public static OptimisationManager mInstance = null;

	public HardwareOpt[] HardwareList = new HardwareOpt[19];

	public SerializableAndroidOptLevel AndroidModels = new SerializableAndroidOptLevel();

	private string PhoneModel = string.Empty;

	private static UpdateMask[] opt_Update = new UpdateMask[5];

	private static int[] offScreenMasks = new int[5] { -1, 269488144, 286331153, 286331153, 1431655765 };

	private static int[] onScreenMasks = new int[5] { -1, 1431655765, 1431655765, 1431655765, 1431655765 };

	public static bool UpdateOptimisationsEnabled = false;

	public static bool opt_ParentSimpleCollider = true;

	public static OptimisationManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				LoadAsset();
			}
			return mInstance;
		}
	}

	public void PatchList()
	{
		if (HardwareList[0].Optimisations.Length < 32)
		{
			HardwareOpt[] hardwareList = HardwareList;
			foreach (HardwareOpt hardwareOpt in hardwareList)
			{
				bool[] array = new bool[32];
				for (int j = 0; j < hardwareOpt.Optimisations.Length; j++)
				{
					array[j] = hardwareOpt.Optimisations[j];
				}
				hardwareOpt.Optimisations = array;
			}
		}
		if (HardwareList.Length < 19)
		{
			HardwareOpt[] array2 = new HardwareOpt[19];
			for (int k = 0; k < HardwareList.Length; k++)
			{
				array2[k] = HardwareList[k];
			}
			HardwareList = array2;
		}
	}

	private void OnEnable()
	{
		AndroidModels.Deserialize();
	}

	private static void LoadAsset()
	{
		mInstance = Resources.Load("OptimisationMatrix") as OptimisationManager;
		if (TBFUtils.IsHighMemoryAndroidDevice())
		{
			Instance.HardwareList[(int)GetCurrentHardware()].Optimisations[26] = true;
		}
		UnityEngine.Object.DontDestroyOnLoad(mInstance);
	}

	public static bool CanUseOptmisation(OptimisationType type)
	{
		if (Instance == null)
		{
			Debug.LogWarning("Optimisation matrix not found!");
			return true;
		}
		return Instance.HardwareList[(int)GetCurrentHardware()].Optimisations[(int)type];
	}

	public static bool IsCurrentHardware(HardwareType type)
	{
		if (Instance == null)
		{
			Debug.LogWarning("Optimisation matrix not found!");
		}
		if (type == GetCurrentHardware())
		{
			return true;
		}
		return false;
	}

	public static string GetAndroidPhoneModel()
	{
		string text = SystemInfo.deviceModel.ToLowerInvariant();
		foreach (KeyValuePair<string, PhoneDetails> item in Instance.AndroidModels.PhoneDictionary)
		{
			if (text.Contains(item.Key))
			{
				return item.Key + " (" + item.Value.Description + ")";
			}
		}
		return "unknown (" + text + ") - report please";
	}

	public static HardwareType GetCurrentHardware()
	{
		if (Instance.PhoneModel == string.Empty)
		{
			string text = SystemInfo.deviceModel.ToLowerInvariant();
			foreach (KeyValuePair<string, PhoneDetails> item in Instance.AndroidModels.PhoneDictionary)
			{
				if (text.Contains(item.Key))
				{
					Instance.PhoneModel = item.Key;
					return item.Value.OptimisationType;
				}
			}
			Instance.PhoneModel = "unknown";
			if (TBFUtils.IsHighMemoryAndroidDevice())
			{
				return HardwareType.AndroidHigh;
			}
			if (SystemInfo.systemMemorySize > 800)
			{
				return HardwareType.AndroidMed;
			}
			return HardwareType.AndroidLow;
		}
		if (Instance.PhoneModel == "unknown")
		{
			if (SystemInfo.systemMemorySize > 1700 || SystemInfo.systemMemorySize < 0)
			{
				return HardwareType.AndroidHigh;
			}
			if (SystemInfo.systemMemorySize > 800)
			{
				return HardwareType.AndroidMed;
			}
			return HardwareType.AndroidLow;
		}
		return Instance.AndroidModels.PhoneDictionary[Instance.PhoneModel].OptimisationType;
	}

	public void UpdateFromSwrve()
	{
		string itemId = "RateTheAppSwitch";
		Dictionary<string, string> resourceDictionary = null;
		if (!Bedrock.GetRemoteUserResources(itemId, out resourceDictionary) || resourceDictionary == null)
		{
			return;
		}
		foreach (int value in Enum.GetValues(typeof(HardwareType)))
		{
			if (value != 19)
			{
				HardwareList[value].Optimisations[28] = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "Device." + (HardwareType)value, HardwareList[value].Optimisations[28]);
			}
		}
	}

	public static void PerFramePrep()
	{
		for (int i = 0; i < 5; i++)
		{
			if (opt_Update[i] == null)
			{
				opt_Update[i] = new UpdateMask();
			}
			opt_Update[i].PerFramePrep();
		}
	}

	public static bool Update(OptType ot, Actor a)
	{
		return opt_Update[(int)ot].Update(a);
	}

	public static bool Update(OptType ot, GameObject a)
	{
		return opt_Update[(int)ot].Update(a.GetComponent<Actor>());
	}

	public static void SetUpdateMask(OptType ot, int on, int off)
	{
		opt_Update[(int)ot].mOnScreenMask = on;
		opt_Update[(int)ot].mOffScreenMask = off;
	}

	public static void ToggleTestOptimisations(bool on)
	{
		if (UpdateOptimisationsEnabled == on)
		{
			return;
		}
		UpdateOptimisationsEnabled = on;
		if (on)
		{
			for (int i = 0; i < 5; i++)
			{
				opt_Update[i].mOffScreenMask = offScreenMasks[i];
				opt_Update[i].mOnScreenMask = onScreenMasks[i];
			}
		}
		else
		{
			for (int i = 0; i < 5; i++)
			{
				opt_Update[i].mOffScreenMask = -1;
				opt_Update[i].mOnScreenMask = -1;
			}
		}
	}
}
