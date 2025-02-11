using UnityEngine;

public class SwrvePayload
{
	public static string CurrentHardCurrencyTotal
	{
		get
		{
			return GameSettings.Instance.PlayerCash().HardCash().ToString();
		}
	}

	public static string TotalMTX
	{
		get
		{
			return GameSettings.Instance.PlayerCash().TotalHardCashEverPurchased().ToString();
		}
	}

	public static string TotalCurrencyAwarded
	{
		get
		{
			return GameSettings.Instance.PlayerCash().TotalHardCashEverAwarded().ToString();
		}
	}

	public static string TotalCurrencySpent
	{
		get
		{
			return GameSettings.Instance.PlayerCash().TotalHardCashEverSpent().ToString();
		}
	}

	public static string EliteConnected
	{
		get
		{
			return (!SecureStorage.Instance.EliteAccountLinked) ? "False" : "True";
		}
	}

	public static string ActivateFriends
	{
		get
		{
			Bedrock.brFriendInfo[] friendsList = new Bedrock.brFriendInfo[100];
			uint numFriends = 0u;
			if (Bedrock.isBedrockActive())
			{
				Bedrock.GetFriendsWithCurrentGame(friendsList, ref numFriends);
			}
			return numFriends.ToString();
		}
	}

	public static string ActivateLoggedIn
	{
		get
		{
			return (Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_OFFLINE && Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE) ? "False" : "True";
		}
	}

	public static string PlayerLevel
	{
		get
		{
			return XPManager.Instance.GetXPLevelAbsolute().ToString();
		}
	}

	public static string PlayerXP
	{
		get
		{
			return StatsHelper.PlayerXP().ToString();
		}
	}

	public static string MissionsStarted
	{
		get
		{
			return StatsHelper.MissionsPlayed().ToString();
		}
	}

	public static string CurrentMission
	{
		get
		{
			if (ActStructure.Instance.MissionInProgress)
			{
				return StatsManager.MissionSwrveId(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection);
			}
			return StatsManager.MissionSwrveId(ActStructure.Instance.LastMissionID, ActStructure.Instance.CurrentMissionSection);
		}
	}

	public static string MissionsPassed
	{
		get
		{
			return StatsHelper.MissionsSuccessful().ToString();
		}
	}

	public static string Device
	{
		get
		{
			return TBFUtils.iPhoneGen;
		}
	}

	public static string Platform
	{
		get
		{
			return Application.platform.ToString();
		}
	}

	public static string DeviceManufacturer
	{
		get
		{
			return TBFUtils.DeviceManfacturer;
		}
	}

	public static string DeviceModel
	{
		get
		{
			return TBFUtils.DeviceModel;
		}
	}

	public static string OSVersion
	{
		get
		{
			return TBFUtils.OSVersion;
		}
	}

	public static string CurrentLanguage
	{
		get
		{
			return Language.CurrentLanguage().ToString();
		}
	}

	public static string Version
	{
		get
		{
			return StartGameSettings.Instance.VERSION;
		}
	}

	public static string WaveAmmoTotal
	{
		get
		{
			return GameController.Instance.mFirstPersonActor.weapon.PrimaryWeapon.GetWeaponAmmo().NumClips.ToString();
		}
	}

	public static string WaveAmmoBought
	{
		get
		{
			return WaveStats.Instance.AmmoBought.ToString();
		}
	}

	public static string WaveAmmoCollected
	{
		get
		{
			return WaveStats.Instance.AmmoCollected.ToString();
		}
	}

	public static string WaveAmmoUsed
	{
		get
		{
			return WaveStats.Instance.AmmoUsed.ToString();
		}
	}

	public static string WaveAmmoSplit
	{
		get
		{
			WaveStats instance = WaveStats.Instance;
			return (instance.AmmoBought + instance.AmmoCollected - instance.AmmoUsed).ToString();
		}
	}

	public static string WaveTimesHealed
	{
		get
		{
			WaveStats instance = WaveStats.Instance;
			return instance.TimesHealed.ToString();
		}
	}

	public static string GyroscopeOnOff
	{
		get
		{
			return (!GameSettings.Instance.PlayerGameSettings().FirstPersonGyroscopeEnabled) ? "Off" : "On";
		}
	}

	public static string GesturesOnOff
	{
		get
		{
			return (!GameSettings.Instance.PlayerGameSettings().FirstPersonGesturesEnabled) ? "Off" : "On";
		}
	}

	public static string AcceptedPushNotifications
	{
		get
		{
			return false.ToString();
		}
	}

	public static string Difficulty
	{
		get
		{
			return ActStructure.Instance.CurrentMissionMode.ToString();
		}
	}

	public static string CurrentMissionTimesFailed
	{
		get
		{
			return (StatsHelper.CurrentMissionTimesPlayed() - StatsHelper.CurrentMissionTimesSucceeded()).ToString();
		}
	}

	public static string TimeInSection
	{
		get
		{
			return StatsHelper.CurrentMissionTotalTime().ToString();
		}
	}

	public static string MinutesInSection
	{
		get
		{
			return Mathf.RoundToInt(StatsHelper.CurrentMissionTotalTime()).ToString();
		}
	}

	public static string AutoLoadout
	{
		get
		{
			return GameSettings.Instance.AutoLoadoutMode.ToString();
		}
	}

	public static string CurrentWeapon
	{
		get
		{
			for (int i = 0; i < 4; i++)
			{
				if (GameSettings.Instance.Soldiers[i] != null && GameSettings.Instance.Soldiers[i].Present && GameSettings.Instance.Soldiers[i].Weapon != null && GameSettings.Instance.Soldiers[i].Weapon.Descriptor != null)
				{
					return GameSettings.Instance.Soldiers[i].Weapon.Descriptor.Name;
				}
			}
			return "None";
		}
	}

	public static string Audio
	{
		get
		{
			if (TBFUtils.IsMusicPlaying)
			{
				return "Playing Custom Music";
			}
			float volumeLevel = TBFUtils.VolumeLevel;
			if (volumeLevel == 0f)
			{
				return "Mute";
			}
			return "Game Audio";
		}
	}

	public static string Location
	{
		get
		{
			string text = ((!LoadoutMenuNavigator.LoadOutActive) ? "InGame_" : "LoadOut_");
			return text + ((!ActStructure.Instance.CurrentMissionIsSpecOps()) ? "Campaign" : "SpecOps");
		}
	}

	public static string RegularMissionsCompleted
	{
		get
		{
			int regular = 0;
			int veteran = 0;
			StatsHelper.NumMissionsComplete(out regular, out veteran);
			return regular.ToString();
		}
	}

	public static string VeteranMissionsCompleted
	{
		get
		{
			int regular = 0;
			int veteran = 0;
			StatsHelper.NumMissionsComplete(out regular, out veteran);
			return veteran.ToString();
		}
	}

	public static string IsCheater
	{
		get
		{
			return (TBFUtils.IsCracked || SecureStorage.Instance.ReceiptValidationFailed).ToString();
		}
	}

	public static string ControllerType
	{
		get
		{
			string manufacturer = Controller.GetManufacturer();
			if (manufacturer == null)
			{
				return Controller.GetControllerType().ToString();
			}
			return Controller.GetControllerType().ToString() + "_" + manufacturer;
		}
	}

	public static string TotalPreviousPurchases
	{
		get
		{
			return SecureStorage.Instance.GetInt("totalPreviousPurchases").ToString();
		}
	}

	public static string LastPackPurchased
	{
		get
		{
			string key = "LastPackPurchased";
			return SecureStorage.Instance.GetString(key).ToString();
		}
	}

	public static string LastPackPurchasedDate
	{
		get
		{
			string key = "LastPackPurchasedDate";
			return SecureStorage.Instance.GetString(key).ToString();
		}
	}

	public static string LastPackPurchasedDateLocal
	{
		get
		{
			string key = "LastPackPurchasedDateLocal";
			return SecureStorage.Instance.GetString(key).ToString();
		}
	}

	public static string WaveAmmoCacheTimesUsed(WeaponDescriptor.WeaponClass weaponClass)
	{
		WaveStats instance = WaveStats.Instance;
		return instance.AmmoCachedTimesUsed(weaponClass).ToString();
	}

	public static string PerkSlot(int slot)
	{
		return GameSettings.Instance.PerkSlotContents(slot).ToString();
	}

	public static string WeaponforSoldier(int i)
	{
		if (GameSettings.Instance.Soldiers[i] != null && GameSettings.Instance.Soldiers[i].Weapon != null && GameSettings.Instance.Soldiers[i].Weapon.Descriptor != null)
		{
			return GameSettings.Instance.Soldiers[i].Weapon.Descriptor.Name;
		}
		return "None";
	}

	public static string PreviousPurchases(string Identifier)
	{
		string key = SwrveNames.General.PreviousPurchases(Identifier);
		return SecureStorage.Instance.GetInt(key).ToString();
	}
}
