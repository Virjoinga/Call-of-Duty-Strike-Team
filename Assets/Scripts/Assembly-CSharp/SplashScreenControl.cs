using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenControl : MonoBehaviour
{
	private enum SplashState
	{
		Start = 0,
		FadeInGame = 1,
		CheckForUpdate = 2,
		CheckHWSpec = 3,
		HoldGame = 4,
		FadeOutGame = 5,
		Destroy = 6,
		HaltProgress = 7
	}

	public static bool SplashShown;

	public SpriteText TouchToStart;

	public SpriteText Copyright;

	public FullScreenImage GameSplash;

	private float ScreenFadeTime;

	private float mStateTime;

	private SplashState mState;

	public ActivateWatcher m_ActivateWatcher;

	public ActivateFriendInviteWatcher m_ActivateFriendInviteWatcher;

	private bool mSwrveRegisterDone;

	private void Start()
	{
		mSwrveRegisterDone = false;
		TouchToStart.gameObject.FadeUpdate(0f, 0f);
		TouchToStart.gameObject.SetActive(false);
		Copyright.gameObject.FadeUpdate(0f, 0f);
		Copyright.gameObject.SetActive(false);
		GameSplash.gameObject.FadeUpdate(0f, 0f);
		GameSplash.gameObject.SetActive(false);
		SplashShown = true;
		SecureStorage.Instance.RegisterSaveableItem(StatsManager.Instance);
		SecureStorage.Instance.RegisterSaveableItem(GameSettings.Instance);
		SecureStorage.Instance.RegisterSaveableItem(ActStructure.Instance);
		SecureStorage.Instance.RegisterSaveableItem(PickupManager.Instance);
		SecureStorage.Instance.RegisterSaveableItem(SwrveUserData.Instance);
		SecureStorage.Instance.RegisterSaveableItem(GlobalUnrestController.Instance);
		StartCoroutine(RegisterSwrve());
		if (!MobileNetworkManager.Instance.IsLoggedIn && MobileNetworkManager.Instance.SupportsAchievements)
		{
			MobileNetworkManager.Instance.Login();
		}
		m_ActivateWatcher = ActivateWatcher.Instance;
		m_ActivateFriendInviteWatcher = ActivateFriendInviteWatcher.Instance;
		Time.timeScale = 1f;
		Debug.Log("DEVICE MODEL: " + TBFUtils.iPhoneGen);
		Debug.Log("DETECTED HW: " + OptimisationManager.GetCurrentHardware());
		bool flag = OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableAntiAliasing);
		Debug.Log("AA ENABLED: " + flag);
		if (flag)
		{
			QualitySettings.antiAliasing = 2;
		}
	}

	private IEnumerator RegisterSwrve()
	{
		yield return new WaitForEndOfFrame();
		SwrveServerVariables.Instance.Register(XPManager.Instance);
		SwrveServerVariables.Instance.Register(WeaponManager.Instance);
		SwrveServerVariables.Instance.Register(PurchaseHandler.Instance);
		SwrveServerVariables.Instance.Register(GMGBalanceTweaks.Instance);
		SwrveServerVariables.Instance.Register(GMGData.Instance);
		SwrveServerVariables.Instance.Register(GameSettings.Instance);
		SwrveServerVariables.Instance.Register(StatsManager.Instance.PerksManagerNoAssert());
		SwrveServerVariables.Instance.Register(AmmoDropManager.Instance);
		SwrveServerVariables.Instance.Register(DailyRewards.Instance);
		SwrveServerVariables.Instance.Register(OptimisationManager.Instance);
		SwrveServerVariables.Instance.Register(GlobalUnrestController.Instance);
		mSwrveRegisterDone = true;
	}

	private void OnEnable()
	{
		Bedrock.UserResourcesChanged += HandleBedrockUserResourcesChanged;
		MessageBoxController.Instance.SeeIfDeviceMessageShouldBeDisplayed();
	}

	private void OnDisable()
	{
		Bedrock.UserResourcesChanged -= HandleBedrockUserResourcesChanged;
	}

	private void HandleBedrockUserResourcesChanged(object sender, EventArgs e)
	{
		MessageBoxController.Instance.SeeIfDeviceMessageShouldBeDisplayed();
	}

	private bool ShouldSkip()
	{
		if (MessageBoxController.Instance.IsAnyMessageActive)
		{
			return false;
		}
		if (Input.anyKeyDown)
		{
			return true;
		}
		if (Input.touchCount > 0 && Input.touches[0].tapCount > 0)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		mStateTime += Time.deltaTime;
		switch (mState)
		{
		case SplashState.Start:
			NextState();
			GameSplash.gameObject.SetActive(true);
			GameSplash.gameObject.FadeTo(1f, ScreenFadeTime, 0f);
			Copyright.gameObject.SetActive(true);
			Copyright.gameObject.FadeTo(1f, 0.5f, 0f);
			SoundManager.Instance.SetAllDefaultSFXVolumeGroups(SecureStorage.Instance.SoundFXVolume);
			SoundManager.Instance.SetDefaultVolumeGroup(SoundFXData.VolumeGroup.Music, SecureStorage.Instance.MusicVolume);
			SoundManager.Instance.SetDefaultVolumeGroup(SoundFXData.VolumeGroup.FrontEnd_Music, SecureStorage.Instance.MusicVolume);
			SoundManager.Instance.ActivateSplashScreenSFX();
			SwrveEventsProgression.LogosDisplayed();
			SwrveEventsUI.SwrveTalkTrigger_MainUI();
			break;
		case SplashState.FadeInGame:
			if (mStateTime >= ScreenFadeTime)
			{
				if (MessageBoxController.Instance.CheckAppUpdateDialogue())
				{
					NextState();
				}
				else
				{
					mState = SplashState.HaltProgress;
				}
			}
			break;
		case SplashState.CheckForUpdate:
			if (!MessageBoxController.Instance.IsAnyMessageActive && mSwrveRegisterDone)
			{
				if (TBFUtils.IsUnsupportedDevice())
				{
					MessageBoxController.Instance.DoUnsupportedDeviceDialogue();
				}
				NextState();
			}
			break;
		case SplashState.CheckHWSpec:
			if (!MessageBoxController.Instance.IsAnyMessageActive)
			{
				TouchToStart.gameObject.SetActive(true);
				TouchToStart.gameObject.FadeTo(1f, 0.5f, 0f, LoopType.pingPong);
				SwrveEventsProgression.TouchToStartDisplayed();
				NextState();
			}
			break;
		case SplashState.HoldGame:
			TouchToStart.gameObject.SetActive(true);
			if (ShouldSkip())
			{
				AnimatedScreenBackground instance = AnimatedScreenBackground.Instance;
				if (instance != null)
				{
					instance.Activate();
				}
				SwrveEventsProgression.TouchToStartTouched();
				TouchToStart.gameObject.FadeTo(0f, 0.1f, 0f);
				SecureStorage.Instance.ResetAllData();
				SecureStorage.Instance.LoadAllData();
				StartCoroutine(LoadFrontEndScenes());
				NextState();
			}
			break;
		case SplashState.FadeOutGame:
			break;
		case SplashState.Destroy:
			UnityEngine.Object.Destroy(base.gameObject);
			break;
		}
	}

	private void NextState()
	{
		mState++;
		mStateTime = 0f;
	}

	private IEnumerator LoadFrontEndScenes()
	{
		AnimatedScreenBackground bg = AnimatedScreenBackground.Instance;
		if (bg != null)
		{
			while (!bg.IsActive)
			{
				yield return null;
			}
		}
		GameSplash.gameObject.SetActive(false);
		foreach (string scene in new List<string> { "MTXMenu", "GlobeSelectCore" })
		{
			AsyncOperation ao = Application.LoadLevelAdditiveAsync(scene);
			Debug.Log(" Application.LoadLevelAdditiveAsync - " + scene);
			while (!ao.isDone)
			{
				yield return ao;
			}
		}
		NextState();
	}
}
