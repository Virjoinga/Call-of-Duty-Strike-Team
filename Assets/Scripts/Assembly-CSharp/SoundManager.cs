using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour
{
	public class SoundInstance : IPoolable
	{
		private static SoundInstance s_nullInst;

		public PoolSoundObject m_soundObj;

		public SoundFX m_sfx;

		private bool m_isPaused;

		private GameObject m_sourceObj;

		private float m_volume;

		private float m_pitch;

		private float m_lastTimescale;

		private float m_fadeDuration;

		private float m_fadeStartVolume;

		private float m_fadeEndVolume;

		private float m_currentFadeTime;

		private bool m_stopAfterFade;

		public SoundFX Sfx
		{
			get
			{
				return m_sfx;
			}
		}

		public GameObject SourceObject
		{
			get
			{
				return m_sourceObj;
			}
		}

		public float Pitch
		{
			get
			{
				float result = 0f;
				if (m_soundObj != null)
				{
					result = m_pitch;
				}
				return result;
			}
			set
			{
				if (m_soundObj != null && value != m_pitch)
				{
					m_pitch = value;
					m_soundObj.m_audioSource.pitch = m_pitch;
				}
			}
		}

		public float Volume
		{
			get
			{
				float result = 0f;
				if (m_soundObj != null)
				{
					result = m_volume;
				}
				return result;
			}
			set
			{
				if (m_soundObj != null)
				{
					m_volume = value;
					float volumeScale = Instance.VolumeGroups[(int)m_sfx.m_sfxData.VolGroup].VolumeScale;
					float volumeMultiply = Instance.VolumeGroups[(int)m_sfx.m_sfxData.VolGroup].VolumeMultiply;
					float volume = m_volume * volumeScale * volumeMultiply;
					m_soundObj.m_audioSource.volume = volume;
				}
			}
		}

		public bool IsPaused
		{
			get
			{
				return m_isPaused;
			}
			set
			{
				if (value != m_isPaused)
				{
					if (m_isPaused)
					{
						m_soundObj.m_audioSource.Play();
					}
					else
					{
						m_soundObj.m_audioSource.Pause();
					}
					m_isPaused = value;
				}
			}
		}

		public bool IsPlaying
		{
			get
			{
				bool result = false;
				if (m_soundObj != null)
				{
					AudioSource audioSource = m_soundObj.m_audioSource;
					if (audioSource != null)
					{
						bool flag = m_sfx != null && m_sfx.m_sfxData != null && m_sfx.m_sfxData.VolGroup != SoundFXData.VolumeGroup.FrontEnd && m_sfx.m_sfxData.VolGroup != SoundFXData.VolumeGroup.FrontEnd_Music;
						result = IsPaused || (flag && m_lastTimescale < 0.001f) || audioSource.isPlaying;
					}
				}
				return result;
			}
		}

		public bool IsMuted
		{
			get
			{
				bool result = false;
				if (m_soundObj != null)
				{
					AudioSource audioSource = m_soundObj.m_audioSource;
					if (audioSource != null)
					{
						result = audioSource.mute;
					}
				}
				return result;
			}
			set
			{
				if (m_soundObj != null)
				{
					AudioSource audioSource = m_soundObj.m_audioSource;
					if (audioSource != null)
					{
						audioSource.mute = value;
					}
				}
			}
		}

		public float SampleLength
		{
			get
			{
				float result = 0f;
				if (m_soundObj != null)
				{
					AudioSource audioSource = m_soundObj.m_audioSource;
					if (audioSource != null && audioSource.clip != null)
					{
						result = audioSource.clip.length;
					}
				}
				return result;
			}
		}

		public string SampleName
		{
			get
			{
				string result = string.Empty;
				if (m_soundObj != null)
				{
					AudioSource audioSource = m_soundObj.m_audioSource;
					if (audioSource != null && audioSource.clip != null)
					{
						result = audioSource.clip.name;
					}
				}
				return result;
			}
		}

		public static SoundInstance Null
		{
			get
			{
				if (s_nullInst == null)
				{
					s_nullInst = new SoundInstance();
				}
				return s_nullInst;
			}
		}

		public SoundInstance()
		{
			m_soundObj = null;
			m_sfx = null;
			m_sourceObj = null;
		}

		public void Create()
		{
			m_soundObj = null;
			m_sfx = null;
			m_sourceObj = null;
		}

		public void New()
		{
			m_soundObj = null;
			m_sfx = null;
			m_sourceObj = null;
			m_lastTimescale = 1f;
			m_fadeDuration = 0f;
		}

		public void Delete()
		{
			m_soundObj = null;
			m_sfx = null;
			m_sourceObj = null;
			m_pitch = -1f;
			m_volume = -1f;
		}

		public void Init(PoolSoundObject soundObj, SoundFX sfx, GameObject sourceObj, AudioClip clip)
		{
			m_soundObj = soundObj;
			m_sfx = sfx;
			m_sourceObj = sourceObj;
			m_lastTimescale = 1f;
			m_fadeDuration = 0f;
			if (m_soundObj != null && m_sfx != null)
			{
				SoundFXData sfxData = m_sfx.m_sfxData;
				m_soundObj.m_audioSource.playOnAwake = false;
				m_soundObj.m_audioSource.clip = clip;
				m_soundObj.m_audioSource.enabled = true;
				m_soundObj.m_audioSource.mute = Instance.VolumeGroups[(int)sfxData.VolGroup].Mute;
				m_soundObj.m_audioSource.bypassEffects = sfxData.m_bypassEffects;
				m_soundObj.m_audioSource.loop = sfxData.m_loop;
				m_soundObj.m_audioSource.priority = sfxData.m_priority;
				float volume = Mathf.Clamp01(sfxData.m_volume + UnityEngine.Random.Range(0f - sfxData.m_volumeVariance, sfxData.m_volumeVariance));
				float pitch = Mathf.Clamp01(sfxData.m_pitch + UnityEngine.Random.Range(0f - sfxData.m_pitchVariance, sfxData.m_pitchVariance));
				Volume = volume;
				Pitch = pitch;
				m_soundObj.m_audioSource.spatialBlend = sfxData.m_panLevel;
				m_soundObj.m_audioSource.spread = sfxData.m_spread;
				m_soundObj.m_audioSource.dopplerLevel = sfxData.m_dopplerLevel;
				m_soundObj.m_audioSource.minDistance = sfxData.m_minDistance;
				m_soundObj.m_audioSource.maxDistance = sfxData.m_maxDistance;
				m_soundObj.m_audioSource.rolloffMode = sfxData.m_rollOffMode;
				m_soundObj.m_audioSource.panStereo = sfxData.m_pan2D;
				if (!sfxData.m_play3D)
				{
					m_soundObj.m_audioSource.minDistance = 10000f;
					m_soundObj.m_audioSource.maxDistance = 10010f;
				}
				if (m_soundObj.m_audioLPF != null)
				{
					m_soundObj.m_audioLPF.enabled = false;
				}
			}
		}

		public void SetPausedVariable(bool pause)
		{
			m_isPaused = pause;
		}

		public void Play()
		{
			if (m_soundObj != null)
			{
				AudioSource audioSource = m_soundObj.m_audioSource;
				if (audioSource != null && !audioSource.isPlaying)
				{
					audioSource.Play();
				}
			}
		}

		public void Stop()
		{
			if (m_soundObj != null)
			{
				AudioSource audioSource = m_soundObj.m_audioSource;
				if (audioSource != null)
				{
					audioSource.Stop();
				}
			}
		}

		public void StopAfterLoop()
		{
			if (m_soundObj != null && m_sfx.m_sfxData.m_loop)
			{
				AudioSource audioSource = m_soundObj.m_audioSource;
				if (audioSource != null)
				{
					audioSource.loop = false;
				}
			}
		}

		public void FadeVolume(float desiredVolume, float fadeTime, bool stopAfterFade)
		{
			m_fadeDuration = fadeTime;
			m_currentFadeTime = 0f;
			m_fadeStartVolume = Volume;
			m_fadeEndVolume = desiredVolume;
			m_stopAfterFade = stopAfterFade;
		}

		private void UpdateFader()
		{
			if (m_fadeDuration == 0f)
			{
				return;
			}
			bool flag = false;
			m_currentFadeTime += Time.deltaTime;
			if (m_currentFadeTime > m_fadeDuration)
			{
				m_currentFadeTime = m_fadeDuration;
				flag = true;
			}
			float num = m_currentFadeTime / m_fadeDuration;
			float volume = m_fadeStartVolume + (m_fadeEndVolume - m_fadeStartVolume) * num;
			Volume = volume;
			if (flag)
			{
				m_fadeDuration = 0f;
				if (m_stopAfterFade)
				{
					Stop();
				}
			}
		}

		private void SetTimescale(float newTimescale)
		{
			if (!(m_sourceObj != null))
			{
				return;
			}
			AudioSource audioSource = m_soundObj.m_audioSource;
			if (!(audioSource != null) || IsPaused)
			{
				return;
			}
			bool isPlaying = audioSource.isPlaying;
			audioSource.pitch = m_pitch * newTimescale;
			if (isPlaying)
			{
				if (newTimescale < 0.001f)
				{
					audioSource.Pause();
				}
			}
			else if (newTimescale > 0.001f)
			{
				audioSource.Play();
			}
		}

		public void Update()
		{
			if (m_soundObj == null)
			{
				return;
			}
			if (m_sfx != null && m_sfx.m_sfxData != null && m_sfx.m_sfxData.VolGroup != SoundFXData.VolumeGroup.FrontEnd && m_sfx.m_sfxData.VolGroup != SoundFXData.VolumeGroup.FrontEnd_Music)
			{
				float timeScale = Time.timeScale;
				if (m_lastTimescale != timeScale)
				{
					SetTimescale(timeScale);
					m_lastTimescale = timeScale;
				}
			}
			UpdateFader();
		}

		public void LateUpdate()
		{
			if (m_sourceObj != null && (m_soundObj.m_flags & PoolObjectFlags.Is2D) == 0)
			{
				m_soundObj.m_gameObj.transform.position = m_sourceObj.transform.position;
			}
		}

		public void ApplyAudioFilter(AudioFilter filter)
		{
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.LowPassSndFilter) && m_soundObj != null && m_soundObj.m_audioSource != null && m_soundObj.m_audioLPF != null && filter != null)
			{
				switch (filter.m_AudioFilterType)
				{
				case AudioFilter.AudioFilterType.Default:
					break;
				case AudioFilter.AudioFilterType.EnvironmentEcho:
					break;
				case AudioFilter.AudioFilterType.LowPass:
				{
					AudioLowPassFilter audioLPF = m_soundObj.m_audioLPF;
					audioLPF.cutoffFrequency = filter.m_CutoffFrequency;
					audioLPF.lowpassResonanceQ = filter.m_LowpassResonaceQ;
					audioLPF.enabled = true;
					break;
				}
				}
			}
		}

		public void RemoveAudioFilter(AudioFilter.AudioFilterType filterType)
		{
			if (m_soundObj == null || !IsPlaying)
			{
				return;
			}
			switch (filterType)
			{
			case AudioFilter.AudioFilterType.Default:
				break;
			case AudioFilter.AudioFilterType.EnvironmentEcho:
				break;
			case AudioFilter.AudioFilterType.LowPass:
			{
				AudioLowPassFilter audioLPF = m_soundObj.m_audioLPF;
				if (audioLPF != null)
				{
					audioLPF.enabled = false;
				}
				break;
			}
			}
		}
	}

	public struct VolumeGroup
	{
		public float VolumeScale;

		public bool Mute;

		public bool Paused;

		public float VolumeMultiply;

		public float VolumeNonScaled;
	}

	[Flags]
	public enum PoolObjectFlags : uint
	{
		None = 0u,
		Is2D = 1u,
		Allocated = 0x80u
	}

	public class PoolSoundObject
	{
		public PoolObjectFlags m_flags;

		public GameObject m_gameObj;

		public AudioSource m_audioSource;

		public AudioLowPassFilter m_audioLPF;
	}

	private const float kMaxDealyForFirstPersonSearch = 5f;

	private const float mThirdPersonDuckValue = 0.4f;

	private const float mVolumeOfMusicInPause = 0.4f;

	private bool m_HookedUpToGameController;

	private bool mBreachPlaying;

	private bool mHealthLowPlaying;

	private bool mHealthMediumPlaying;

	private BodyDropSfxPlayer mBodyDropSfxPlayer = new BodyDropSfxPlayer();

	private WeaponCasingSound mWeaponCasingSound = new WeaponCasingSound();

	private SoundInstance mThirdPersonLoopSndInst;

	private float mCurrentDuck = 1f;

	private bool mGamePlayStarted;

	private bool mOverWatchActivated;

	private bool mMissionBriefingActive;

	private float mPauseModeMusicGroupHold;

	private bool mMessageBoxActive;

	private bool mDestroyBriefingSFX;

	public AudioFilter HealthMonitorAudioFilterLow;

	public AudioFilter HealthMonitorAudioFilterMedium;

	public AudioFilter HealthMonitorAudioFilterHigh;

	public VolumeGroup[] VolumeGroups;

	private Dictionary<SoundFXData, SoundFX> m_sfxMap = new Dictionary<SoundFXData, SoundFX>();

	private float[] mAmbientFadeGroupSave = new float[9];

	private float[] mDefaultGroupVolume = new float[9];

	private bool[] mPauseGroupSave = new bool[9];

	private List<SoundInstance> m_playingInstances = new List<SoundInstance>();

	private GameObject m_speaker2D;

	private bool mAppPaused;

	private List<PoolSoundObject> m_poolObject2D;

	private List<PoolSoundObject> m_poolObject3D;

	public static SoundManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<SoundManager>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<SoundManager>() != null;
		}
	}

	private void GameAwakeOnce()
	{
		AttemptToHookToGameController();
		mWeaponCasingSound.Init();
	}

	private void GameUpdate()
	{
		if (!m_HookedUpToGameController)
		{
			AttemptToHookToGameController();
		}
		mWeaponCasingSound.Process();
		DoFirstPersonCheck();
	}

	private void OnGameDestroy()
	{
		if (m_HookedUpToGameController && (bool)GameController.Instance)
		{
			GameController.Instance.OnExitFirstPerson -= OnZoomOutTriggered;
			GameController.Instance.OnSwitchToFirstPerson -= OnZoomInTriggered;
			m_HookedUpToGameController = false;
		}
	}

	public void SetUIScrollListScrollSFX(UIScrollList scrollList)
	{
		scrollList.scriptWithMethodToInvokeForScrollSfx = this;
		scrollList.methodToInvokeForScrollSfx = "UIScrollListScrollSFX";
	}

	public void UIScrollListScrollSFX()
	{
		InterfaceSFX.Instance.UIScrollTick.Play2D();
	}

	public void ActivateBreachSFX()
	{
		if (!mBreachPlaying)
		{
			mBreachPlaying = true;
			InterfaceSFX.Instance.SlowmoStart.Play2D();
			InterfaceSFX.Instance.SlowmoLoopBreath.Play2D();
			InterfaceSFX.Instance.SlowmoLoopTin.Play2D();
		}
	}

	public void DeactivateBreachSFX(bool onlyStopLoops)
	{
		if (mBreachPlaying)
		{
			mBreachPlaying = false;
			if (!onlyStopLoops)
			{
				InterfaceSFX.Instance.SlowmoEnd.Play2D();
			}
			InterfaceSFX.Instance.SlowmoLoopBreath.Stop2D();
			InterfaceSFX.Instance.SlowmoLoopTin.Stop2D();
		}
	}

	public void ActivatePaused()
	{
		PauseVolumeGroup(SoundFXData.VolumeGroup.Sfx, true);
		PauseVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, true);
		PauseVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, true);
		PauseVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, true);
		PauseVolumeGroup(SoundFXData.VolumeGroup.VO, true);
		if (mMessageBoxActive)
		{
			mPauseModeMusicGroupHold = mAmbientFadeGroupSave[3];
			return;
		}
		mPauseModeMusicGroupHold = GetVolumeGroupNonScaled(SoundFXData.VolumeGroup.Music);
		SetVolumeGroup(SoundFXData.VolumeGroup.Music, 0.4f);
	}

	public void DeActivatePaused(bool restoreMusicVolume)
	{
		PauseVolumeGroup(SoundFXData.VolumeGroup.Sfx, false);
		PauseVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, false);
		PauseVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, false);
		PauseVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, false);
		PauseVolumeGroup(SoundFXData.VolumeGroup.VO, false);
		if (restoreMusicVolume)
		{
			SetVolumeGroup(SoundFXData.VolumeGroup.Music, mPauseModeMusicGroupHold);
		}
	}

	public void ActivateMessageBoxSFX()
	{
		if (!mMessageBoxActive)
		{
			SaveAndSetGroup(SoundFXData.VolumeGroup.Music, 0f);
			SaveAndSetGroup(SoundFXData.VolumeGroup.Sfx, 0f);
			SaveAndSetGroup(SoundFXData.VolumeGroup.VO, 0f);
			SaveAndSetGroup(SoundFXData.VolumeGroup.BattleChatterWalla, 0f);
			SaveAndSetGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0f);
			SaveAndSetGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 0f);
			MenuSFX.Instance.MenuBoxExpand.Play2D();
			mMessageBoxActive = true;
		}
	}

	public void DeactivateMessageBoxSFX()
	{
		if (mMessageBoxActive)
		{
			RestoreGroup(SoundFXData.VolumeGroup.Music);
			RestoreGroup(SoundFXData.VolumeGroup.Sfx);
			RestoreGroup(SoundFXData.VolumeGroup.VO);
			RestoreGroup(SoundFXData.VolumeGroup.BattleChatterWalla);
			RestoreGroup(SoundFXData.VolumeGroup.Indoor_Ambience);
			RestoreGroup(SoundFXData.VolumeGroup.Outdoor_Ambience);
			MenuSFX.Instance.MenuBoxClose.Play2D();
			mMessageBoxActive = false;
		}
	}

	public void ActivateOverwatchSFX()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, 0f);
		OverwatchSFX.Instance.EnterStrategy.Play2D();
		OverwatchSFX.Instance.Hum.Play2D();
		mOverWatchActivated = true;
	}

	public void DeactivateOverwatchSFX(bool onlyStopLoops)
	{
		if (mOverWatchActivated && OverwatchSFX.HasInstance)
		{
			OverwatchSFX.Instance.Hum.Stop2D();
		}
		if (onlyStopLoops)
		{
			mOverWatchActivated = false;
			return;
		}
		SetVolumeGroup(SoundFXData.VolumeGroup.Music, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, 1f);
		if (mOverWatchActivated && OverwatchSFX.HasInstance)
		{
			OverwatchSFX.Instance.ExitStrategy.Play2D();
		}
		mOverWatchActivated = false;
	}

	public void ActivateMissionBriefingSFX()
	{
		mMissionBriefingActive = true;
		MusicManager.Instance.FadeOutCurrentMusic(2f);
		EnterFrontendVolumeGroupSettings();
	}

	public void DeactivateMissionBriefingSFX()
	{
		mMissionBriefingActive = false;
		if (!LoadoutMenuNavigator.LoadOutActive)
		{
			ExitFrontendVolumeGroupSettings();
		}
		mDestroyBriefingSFX = true;
	}

	private void EnterFrontendVolumeGroupSettings()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Music, 1f);
	}

	private void ExitFrontendVolumeGroupSettings()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla, 1f);
	}

	public void ActivateLoadOutSFX()
	{
		MusicManager.Instance.PlayLoadOutMusic();
		EnterFrontendVolumeGroupSettings();
	}

	public void DeactivateLoadOutSFX()
	{
		ExitFrontendVolumeGroupSettings();
		DeActivatePaused(false);
	}

	public void ActivateGlobeSFX()
	{
		DeactivateOverwatchSFX(true);
		EnterFrontendVolumeGroupSettings();
		MusicManager.Instance.PlayTitleMusic();
		GlobeSFX.Instance.GlobeStart.Play2D();
	}

	public void DeactivateGlobeSFX()
	{
		MusicManager.Instance.SetMusicScriptOverride(false);
		MusicManager.Instance.PlayLoadOutMusic();
	}

	public void ActivateSplashScreenSFX()
	{
		AudioSource[] components = base.gameObject.GetComponents<AudioSource>();
		AudioSource[] array = components;
		foreach (AudioSource obj in array)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		SetVolumeGroup(SoundFXData.VolumeGroup.Music, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.FrontEnd_Music, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 1f);
		MusicManager.Instance.SetMusicScriptOverride(false);
		MusicManager.Instance.PlayTitleMusic();
	}

	public void ActivateLoadingScreenSFX()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Music, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 0f);
	}

	public void ActivateMissionComplete()
	{
		EnterFrontendVolumeGroupSettings();
	}

	public void SetSFXForIndoor()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 1f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 0.5f);
	}

	public void SetSFXForOutdoor()
	{
		SetVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience, 0.5f);
		SetVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience, 1f);
	}

	private void AttemptToHookToGameController()
	{
		if ((bool)GameController.Instance)
		{
			GameController.Instance.OnExitFirstPerson += OnZoomOutTriggered;
			GameController.Instance.OnSwitchToFirstPerson += OnZoomInTriggered;
			m_HookedUpToGameController = true;
		}
	}

	public void OnZoomInTriggered()
	{
		ActivateFirstPersonSettings();
		StartCoroutine(AddFiltersAfterSwitchToFirstPerson(0.5f));
	}

	public void ForceHealthFilterOntoExistingSFX(int previousHealthLevel)
	{
		AddHealthFilter(GameController.Instance.mFirstPersonActor, previousHealthLevel);
	}

	private IEnumerator AddFiltersAfterSwitchToFirstPerson(float delay)
	{
		float maxDelay = 0f;
		Actor firstPerson = GameController.Instance.mFirstPersonActor;
		while (firstPerson == null)
		{
			yield return new WaitForSeconds(delay);
			maxDelay += delay;
			if (maxDelay > 5f)
			{
				yield return null;
			}
			firstPerson = GameController.Instance.mFirstPersonActor;
		}
		AddHealthFilter(firstPerson, -1);
	}

	private void AddHealthFilter(Actor firstPersonActor, int previousHealthLevel)
	{
		int num = 0;
		if (firstPersonActor != null)
		{
			num = ColourChart.GetAnimFrameForHealth(firstPersonActor.health.Health01);
			if (num < 3)
			{
				StopHealthLowLoop(false);
			}
			if (num < 2)
			{
				StopHealthMediumLoop();
			}
			if (num > 1)
			{
				AudioFilter audioFilter = null;
				switch (num)
				{
				case 2:
					audioFilter = HealthMonitorAudioFilterHigh;
					if (previousHealthLevel == 1)
					{
						CharacterSFX.Instance.DamageHealthMedium.Play2D();
					}
					StartHealthMediumLoop();
					break;
				case 3:
					audioFilter = HealthMonitorAudioFilterMedium;
					StartHealthLowLoop();
					StartHealthMediumLoop();
					break;
				case 4:
					audioFilter = HealthMonitorAudioFilterLow;
					StartHealthLowLoop();
					StartHealthMediumLoop();
					break;
				}
				if (!(audioFilter != null))
				{
					return;
				}
				for (int i = 0; i < 9; i++)
				{
					if (i != 1 && i != 2 && i != 8 && i != 7)
					{
						ApplyAudioFilterOnVolumeGroup((SoundFXData.VolumeGroup)i, audioFilter);
					}
				}
			}
			else
			{
				StopHealthEffects();
			}
		}
		else
		{
			StopHealthEffects();
		}
	}

	private void DoFirstPersonCheck()
	{
		if (!mGamePlayStarted)
		{
			return;
		}
		if (!InteractionsManager.Instance || !InteractionsManager.Instance.IsPlayingCutscene())
		{
			bool flag = (bool)GameController.Instance && GameController.Instance.IsFirstPerson;
			if (flag && mCurrentDuck != 1f)
			{
				ActivateFirstPersonSettings();
			}
			else if (!flag && mCurrentDuck == 1f)
			{
				StopHealthEffects();
				ActivateThirdPersonSettings();
			}
		}
		else if (mCurrentDuck != 1f)
		{
			ActivateFirstPersonSettings();
		}
	}

	public void OnZoomOutTriggered()
	{
		StopHealthEffects();
		CharacterSFX.Instance.HoldBreathLoop.Stop2D();
		if ((bool)InteractionsManager.Instance && !InteractionsManager.Instance.IsPlayingCutscene())
		{
			ActivateThirdPersonSettings();
		}
	}

	private void RemoveAllHealthFilters()
	{
		for (int i = 0; i < 9; i++)
		{
			if (i != 1 && i != 8 && i != 2 && i != 7)
			{
				RemoveAudioFilterOnVolumeGroup((SoundFXData.VolumeGroup)i, AudioFilter.AudioFilterType.LowPass);
			}
		}
	}

	public void StopHealthEffects()
	{
		RemoveAllHealthFilters();
		StopHealthLowLoop(true);
		StopHealthMediumLoop();
	}

	public void CleanUpOnLevelEnd()
	{
		DeActivatePaused(false);
		StopVolumeGroup(SoundFXData.VolumeGroup.Sfx);
		StopVolumeGroup(SoundFXData.VolumeGroup.VO);
		StopVolumeGroup(SoundFXData.VolumeGroup.BattleChatterWalla);
		StopVolumeGroup(SoundFXData.VolumeGroup.Indoor_Ambience);
		StopVolumeGroup(SoundFXData.VolumeGroup.Outdoor_Ambience);
		DeactivateBreachSFX(true);
		StopHealthEffects();
		mWeaponCasingSound.DeactivateAll();
		EndThirdPersonLoop();
		mGamePlayStarted = false;
	}

	public void ActivateUIOpened()
	{
		GlobeCamera globeCamera = UnityEngine.Object.FindObjectOfType(typeof(GlobeCamera)) as GlobeCamera;
		if (globeCamera != null)
		{
			globeCamera.StopSpinSFX();
		}
		SelectableMissionMarker.StopAllMissionBlipSounds();
	}

	public void ActivateUIClosed()
	{
		PauseVolumeGroup(SoundFXData.VolumeGroup.VO, false);
		SaveAndSetGroup(SoundFXData.VolumeGroup.VO, 1f);
	}

	public void GamePlayStarted()
	{
		PauseVolumeGroup(SoundFXData.VolumeGroup.VO, false);
		SaveAndSetGroup(SoundFXData.VolumeGroup.VO, 1f);
		mGamePlayStarted = true;
		if (mDestroyBriefingSFX)
		{
			UnityEngine.Object.Destroy(BriefingSFX.Instance.gameObject);
			mDestroyBriefingSFX = false;
		}
	}

	public void StartLoadMission()
	{
		PauseVolumeGroup(SoundFXData.VolumeGroup.VO, false);
		SaveAndSetGroup(SoundFXData.VolumeGroup.VO, 1f);
	}

	public void CinematicStarted()
	{
		StopHealthEffects();
	}

	public void PlayBodyFallSfx(Vector3 position, GameObject gameObjectToPlayOn)
	{
		mBodyDropSfxPlayer.Play(position, gameObjectToPlayOn);
	}

	public void PlayWeaponCasingSfx(Vector3 position, Vector3 velocity)
	{
		mWeaponCasingSound.Add(position, velocity);
	}

	public void StartHealthLowLoop()
	{
		if (!mHealthLowPlaying)
		{
			mHealthLowPlaying = true;
			CharacterSFX.Instance.DamageHealthLow.Play2D();
		}
	}

	private void StopHealthLowLoop(bool onlyStopLoop)
	{
		if (mHealthLowPlaying)
		{
			mHealthLowPlaying = false;
			if (!onlyStopLoop)
			{
				FadeSoundFx.FadeSfxHelper(base.gameObject, CharacterSFX.Instance.DamageHealthLow, false, false, 0.75f, 0f, true);
				CharacterSFX.Instance.DamageHealthLowEnd.Play2D();
			}
			CharacterSFX.Instance.DamageHealthLow.Stop2D();
		}
	}

	public void StartHealthMediumLoop()
	{
		if (!mHealthMediumPlaying)
		{
			mHealthMediumPlaying = true;
			CharacterSFX.Instance.DamageHealthMediumLoop.Play2D();
		}
	}

	private void StopHealthMediumLoop()
	{
		if (mHealthMediumPlaying)
		{
			mHealthMediumPlaying = false;
			CharacterSFX.Instance.DamageHealthMediumLoop.Stop2D();
		}
	}

	private AudioFilter GetReplacementAudioFilter(SoundFXData.VolumeGroup volGrp)
	{
		AudioFilter result = null;
		if (GameController.Instance != null && volGrp != SoundFXData.VolumeGroup.Cutscene && volGrp != SoundFXData.VolumeGroup.FrontEnd && volGrp != SoundFXData.VolumeGroup.FrontEnd_Music && volGrp != SoundFXData.VolumeGroup.VO)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null)
			{
				switch (ColourChart.GetAnimFrameForHealth(mFirstPersonActor.health.Health01))
				{
				case 2:
					result = HealthMonitorAudioFilterHigh;
					break;
				case 3:
					result = HealthMonitorAudioFilterMedium;
					break;
				case 4:
					result = HealthMonitorAudioFilterLow;
					break;
				}
			}
		}
		return result;
	}

	private void ActivateThirdPersonSettings()
	{
		mCurrentDuck = 0.4f;
		SetFirstThirdDuck(mCurrentDuck);
		StartThirdPersonLoop();
	}

	private void ActivateFirstPersonSettings()
	{
		mCurrentDuck = 1f;
		SetFirstThirdDuck(mCurrentDuck);
		EndThirdPersonLoop();
	}

	private void SetFirstThirdDuck(float duckVar)
	{
		SetVolumeGroupMultiply(SoundFXData.VolumeGroup.Sfx, duckVar);
		SetVolumeGroupMultiply(SoundFXData.VolumeGroup.BattleChatterWalla, duckVar);
		SetVolumeGroupMultiply(SoundFXData.VolumeGroup.Indoor_Ambience, duckVar);
		SetVolumeGroupMultiply(SoundFXData.VolumeGroup.Outdoor_Ambience, duckVar);
	}

	private void StartThirdPersonLoop()
	{
		if (mThirdPersonLoopSndInst == null)
		{
			mThirdPersonLoopSndInst = InterfaceSFX.Instance.TPPDroneLoop.Play2D();
		}
	}

	private void EndThirdPersonLoop()
	{
		if (mThirdPersonLoopSndInst != null)
		{
			mThirdPersonLoopSndInst.Stop();
			mThirdPersonLoopSndInst = null;
		}
	}

	public void PlayTeleTypeSfx()
	{
		if (mMissionBriefingActive)
		{
			BriefingSFX.Instance.TextType.Play2D();
		}
		else if (GlobeSelect.Instance != null)
		{
			GlobeSFX.Instance.TextType.Play2D();
		}
		else
		{
			InterfaceSFX.Instance.TextType.Play2D();
		}
	}

	public bool HasGamePlayStarted()
	{
		return mGamePlayStarted;
	}

	protected override void AwakeOnce()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.name = "SoundManger";
		InitialiseSoundObjectPool(16, 64);
		VolumeGroups = new VolumeGroup[Enum.GetNames(typeof(SoundFXData.VolumeGroup)).Length];
		SetAllDefaultSFXVolumeGroups(1f);
		mDefaultGroupVolume[3] = 1f;
		mDefaultGroupVolume[8] = 1f;
		VolumeGroups[0].VolumeScale = 0f;
		VolumeGroups[1].VolumeScale = 0f;
		VolumeGroups[2].VolumeScale = 1f;
		VolumeGroups[3].VolumeScale = 1f;
		VolumeGroups[8].VolumeScale = 1f;
		VolumeGroups[4].VolumeScale = 0f;
		VolumeGroups[5].VolumeScale = 0f;
		VolumeGroups[6].VolumeScale = 0f;
		VolumeGroups[7].VolumeScale = 1f;
		VolumeGroups[0].VolumeNonScaled = 0f;
		VolumeGroups[1].VolumeNonScaled = 0f;
		VolumeGroups[2].VolumeNonScaled = 1f;
		VolumeGroups[3].VolumeNonScaled = 1f;
		VolumeGroups[8].VolumeNonScaled = 1f;
		VolumeGroups[4].VolumeNonScaled = 0f;
		VolumeGroups[5].VolumeNonScaled = 0f;
		VolumeGroups[6].VolumeNonScaled = 0f;
		VolumeGroups[7].VolumeNonScaled = 1f;
		SetAllVolumeGroupMultiply(1f);
		GameAwakeOnce();
	}

	protected override void OnDestroy()
	{
		OnGameDestroy();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused && !mAppPaused)
		{
			mAppPaused = true;
			for (int i = 0; i < 9; i++)
			{
				mPauseGroupSave[i] = VolumeGroups[i].Paused;
				PauseVolumeGroup((SoundFXData.VolumeGroup)i, true);
			}
		}
		else if (!paused && mAppPaused)
		{
			mAppPaused = false;
			for (int j = 0; j < 9; j++)
			{
				PauseVolumeGroupWhenRegainingFocus((SoundFXData.VolumeGroup)j, mPauseGroupSave[j]);
			}
		}
	}

	public SoundInstance Play2D(SoundFXData fxData)
	{
		if (fxData.m_audioSourceData != null && fxData.m_audioSourceData.Count > 0)
		{
			return Play(fxData, null, null);
		}
		return SoundInstance.Null;
	}

	public SoundInstance Play(SoundFXData fxData, GameObject source)
	{
		if (source == base.gameObject)
		{
			return Play2D(fxData);
		}
		return Play(fxData, source, null);
	}

	public SoundInstance PlaySpotSfxAtPosition(SoundFXData fxData, Vector3 position)
	{
		return InternalPlay(fxData, null, position, null);
	}

	public SoundInstance Play(SoundFXData fxData, GameObject source, AudioFilter audFilter)
	{
		return InternalPlay(fxData, source, Vector3.zero, audFilter);
	}

	private SoundInstance InternalPlay(SoundFXData fxData, GameObject source, Vector3 fixedPos, AudioFilter audFilter)
	{
		if (fxData == null || fxData.m_audioSourceData.Count == 0)
		{
			Debug.LogWarning("No sound data while trying to play sound on " + source.name);
			return SoundInstance.Null;
		}
		SoundFX soundEvent = GetSoundEvent(fxData);
		if (soundEvent.nPlayingInstances >= fxData.m_maxInstances)
		{
			return SoundInstance.Null;
		}
		if (fxData.m_timeBetweenDuplicatePlay > 0f)
		{
			float num = Time.time;
			if (fxData.VolGroup == SoundFXData.VolumeGroup.FrontEnd)
			{
				num = Time.realtimeSinceStartup;
			}
			if (soundEvent.LastPlayedTime + fxData.m_timeBetweenDuplicatePlay > num)
			{
				return SoundInstance.Null;
			}
			soundEvent.LastPlayedTime = num;
		}
		AudioClip audioClip = soundEvent.ChooseSfxDataToPlay();
		if (audioClip == null)
		{
			return SoundInstance.Null;
		}
		PoolObjectFlags requiredFlags = PoolObjectFlags.None;
		if (source == null && fixedPos == Vector3.zero)
		{
			requiredFlags = PoolObjectFlags.Is2D;
		}
		PoolSoundObject poolSoundObject = AllocateSoundObject(requiredFlags);
		if (poolSoundObject != null)
		{
			SoundInstance soundInstance = ObjectPool.New<SoundInstance>();
			soundInstance.Init(poolSoundObject, soundEvent, source, audioClip);
			if (source != null)
			{
				poolSoundObject.m_gameObj.transform.position = source.transform.position;
			}
			else if (fixedPos != Vector3.zero)
			{
				poolSoundObject.m_gameObj.transform.position = fixedPos;
			}
			soundInstance.Play();
			soundInstance.IsPaused = VolumeGroups[(int)fxData.VolGroup].Paused || mAppPaused;
			AudioFilter replacementAudioFilter = GetReplacementAudioFilter(fxData.VolGroup);
			if (replacementAudioFilter != null)
			{
				audFilter = replacementAudioFilter;
			}
			if (audFilter != null)
			{
				soundInstance.ApplyAudioFilter(audFilter);
			}
			soundEvent.nPlayingInstances++;
			m_playingInstances.Add(soundInstance);
			return soundInstance;
		}
		return SoundInstance.Null;
	}

	public void Stop2D(SoundFXData fxData)
	{
		if (fxData != null)
		{
			SoundInstance soundInstance = FindSoundInstance2D(fxData);
			if (soundInstance != SoundInstance.Null)
			{
				soundInstance.Stop();
			}
		}
	}

	public void Stop(SoundFXData fxData, GameObject source)
	{
		if (fxData != null)
		{
			SoundInstance soundInstance = FindSoundInstance(fxData, source);
			if (soundInstance != SoundInstance.Null)
			{
				soundInstance.Stop();
				CleanupStoppedSoundInstance(soundInstance);
			}
		}
	}

	public void StopAll(SoundFXData fxData)
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData == fxData)
			{
				soundInstance.Stop();
			}
		}
	}

	public void StopAllAndCleanup(SoundFXData fxData)
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData == fxData)
			{
				soundInstance.Stop();
				CleanupStoppedSoundInstance(soundInstance);
			}
		}
	}

	public void StopAfterLoop(SoundFXData fxData, GameObject source)
	{
		if (fxData != null)
		{
			SoundInstance soundInstance = FindSoundInstance(fxData, source);
			if (soundInstance != SoundInstance.Null)
			{
				soundInstance.StopAfterLoop();
			}
		}
	}

	private void InternalCleanupSoundInst(SoundInstance sndInst)
	{
		if (sndInst.m_soundObj != null)
		{
			ReleaseSoundObject(sndInst.m_soundObj);
			sndInst.m_soundObj = null;
			sndInst.m_sfx.nPlayingInstances--;
			if (sndInst.m_sfx.nPlayingInstances == 0)
			{
				CleanupSoundEvent(sndInst.m_sfx);
			}
			sndInst.m_sfx = null;
		}
	}

	private void CleanupFinishedSounds()
	{
		for (int num = m_playingInstances.Count - 1; num >= 0; num--)
		{
			SoundInstance soundInstance = m_playingInstances[num];
			if (!soundInstance.IsPlaying)
			{
				InternalCleanupSoundInst(soundInstance);
				m_playingInstances.RemoveAt(num);
				ObjectPool.Delete(soundInstance);
			}
		}
	}

	private void CleanupStoppedSoundInstance(SoundInstance sndInstToCleanup)
	{
		for (int num = m_playingInstances.Count - 1; num >= 0; num--)
		{
			if (sndInstToCleanup == m_playingInstances[num])
			{
				InternalCleanupSoundInst(sndInstToCleanup);
				m_playingInstances.RemoveAt(num);
				ObjectPool.Delete(sndInstToCleanup);
				break;
			}
		}
	}

	public void NukeAllSounds()
	{
		for (int num = m_playingInstances.Count - 1; num >= 0; num--)
		{
			SoundInstance soundInstance = m_playingInstances[num];
			if (soundInstance.m_soundObj != null)
			{
				ReleaseSoundObject(soundInstance.m_soundObj);
				soundInstance.m_soundObj = null;
				soundInstance.m_sfx.nPlayingInstances--;
				soundInstance.m_sfx = null;
			}
			m_playingInstances.RemoveAt(num);
			ObjectPool.Delete(soundInstance);
		}
	}

	private void Update()
	{
		CleanupFinishedSounds();
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			soundInstance.Update();
		}
		GameUpdate();
	}

	private void LateUpdate()
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			soundInstance.LateUpdate();
		}
	}

	public void Fade(GameObject go, SoundFXData FxData, float duration, float desiredVolume, bool destroyAfterFade)
	{
		SoundInstance soundInstance = FindSoundInstance(FxData, go);
		if (soundInstance != null)
		{
			soundInstance.FadeVolume(desiredVolume, duration, destroyAfterFade);
		}
	}

	private SoundFX GetSoundEvent(SoundFXData FxData)
	{
		SoundFX value;
		if (!m_sfxMap.TryGetValue(FxData, out value))
		{
			value = ObjectPool.New<SoundFX>();
			value.m_sfxData = FxData;
			m_sfxMap[FxData] = value;
		}
		return value;
	}

	private void CleanupSoundEvent(SoundFX sfx)
	{
		if (m_sfxMap.ContainsKey(sfx.m_sfxData))
		{
			m_sfxMap.Remove(sfx.m_sfxData);
			ObjectPool.Delete(sfx);
		}
	}

	public SoundInstance FindSoundInstance2D(SoundFXData fxData)
	{
		return FindSoundInstance(fxData, null);
	}

	public SoundInstance FindSoundInstance(SoundFXData fxData, GameObject source)
	{
		SoundInstance result = SoundInstance.Null;
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.SourceObject == source && soundInstance.Sfx.m_sfxData == fxData)
			{
				result = soundInstance;
				break;
			}
		}
		return result;
	}

	public void SetVolume(SoundFXData fxData, GameObject source, float volume)
	{
		if (source == base.gameObject)
		{
			source = null;
		}
		SoundInstance soundInstance = FindSoundInstance(fxData, source);
		if (soundInstance != null)
		{
			soundInstance.Volume = volume;
		}
	}

	public float GetVolume(SoundFXData fxData, GameObject source)
	{
		float result = 0f;
		if (source == base.gameObject)
		{
			source = null;
		}
		SoundInstance soundInstance = FindSoundInstance(fxData, source);
		if (soundInstance != null)
		{
			result = soundInstance.Volume;
		}
		return result;
	}

	public void ApplyAudioFilterOnVolumeGroup(SoundFXData.VolumeGroup volGrp, AudioFilter audFilter)
	{
		AudioFilter replacementAudioFilter = GetReplacementAudioFilter(volGrp);
		if (replacementAudioFilter != null)
		{
			audFilter = replacementAudioFilter;
		}
		if (!(audFilter != null))
		{
			return;
		}
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				soundInstance.ApplyAudioFilter(audFilter);
			}
		}
	}

	public void RemoveAudioFilterOnVolumeGroup(SoundFXData.VolumeGroup volGrp, AudioFilter.AudioFilterType filterType)
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				soundInstance.RemoveAudioFilter(filterType);
			}
		}
	}

	private void UpdateGroupVolumes(SoundFXData.VolumeGroup volGrp)
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				float volume = soundInstance.Volume;
				soundInstance.Volume = volume;
			}
		}
	}

	public float GetVolumeGroup(SoundFXData.VolumeGroup grp)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "volume group out of range");
		return VolumeGroups[(int)grp].VolumeScale;
	}

	public float GetVolumeGroupNonScaled(SoundFXData.VolumeGroup grp)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "volume group out of range");
		return VolumeGroups[(int)grp].VolumeNonScaled;
	}

	public void SetVolumeGroup(SoundFXData.VolumeGroup grp, float volume, bool stopGroupFader)
	{
		if (stopGroupFader)
		{
			VolumeGroupFader.RemoveAnyDuplicateFader(grp);
		}
		volume = Mathf.Clamp01(volume);
		VolumeGroups[(int)grp].VolumeScale = mDefaultGroupVolume[(int)grp] * volume;
		VolumeGroups[(int)grp].VolumeNonScaled = volume;
		UpdateGroupVolumes(grp);
	}

	public void SetVolumeGroup(SoundFXData.VolumeGroup grp, float volume)
	{
		SetVolumeGroup(grp, volume, true);
	}

	public void SetAllDefaultSFXVolumeGroups(float volume)
	{
		volume = Mathf.Clamp01(volume);
		for (int i = 0; i < VolumeGroups.Length - 1; i++)
		{
			if (i != 3 && i != 8)
			{
				VolumeGroups[i].VolumeScale = VolumeGroups[i].VolumeNonScaled * volume;
				mDefaultGroupVolume[i] = volume;
				UpdateGroupVolumes((SoundFXData.VolumeGroup)i);
			}
		}
	}

	public float GetDefaultVolumeGroup(SoundFXData.VolumeGroup grp)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "default volume group out of range");
		return mDefaultGroupVolume[(int)grp];
	}

	public void SetDefaultVolumeGroup(SoundFXData.VolumeGroup grp, float volume)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "default volume group out of range");
		VolumeGroups[(int)grp].VolumeScale = VolumeGroups[(int)grp].VolumeNonScaled * volume;
		mDefaultGroupVolume[(int)grp] = volume;
		UpdateGroupVolumes(grp);
	}

	public void PauseVolumeGroup(SoundFXData.VolumeGroup volGrp, bool paused)
	{
		VolumeGroups[(int)volGrp].Paused = paused;
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				soundInstance.IsPaused = paused;
			}
		}
	}

	public void PauseVolumeGroupWhenRegainingFocus(SoundFXData.VolumeGroup volGrp, bool paused)
	{
		VolumeGroups[(int)volGrp].Paused = paused;
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				if (!paused)
				{
					soundInstance.SetPausedVariable(paused);
				}
				else
				{
					soundInstance.IsPaused = paused;
				}
			}
		}
	}

	public void MuteVolumeGroups(bool mute)
	{
		for (int i = 0; i < 9; i++)
		{
			MuteVolumeGroup((SoundFXData.VolumeGroup)i, mute);
		}
	}

	public void MuteVolumeGroup(SoundFXData.VolumeGroup volGrp, bool mute)
	{
		VolumeGroups[(int)volGrp].Mute = mute;
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				soundInstance.IsMuted = mute;
			}
		}
	}

	private void StopVolumeGroup(SoundFXData.VolumeGroup volGrp)
	{
		for (int i = 0; i < m_playingInstances.Count; i++)
		{
			SoundInstance soundInstance = m_playingInstances[i];
			if (soundInstance.Sfx.m_sfxData.VolGroup == volGrp)
			{
				soundInstance.Stop();
			}
		}
		CleanupFinishedSounds();
	}

	private void SaveAndSetGroup(SoundFXData.VolumeGroup grp, float grpVol)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "volume group out of range");
		mAmbientFadeGroupSave[(int)grp] = GetVolumeGroupNonScaled(grp);
		SetVolumeGroup(grp, grpVol);
	}

	private void RestoreGroup(SoundFXData.VolumeGroup grp)
	{
		TBFAssert.DoAssert(grp < SoundFXData.VolumeGroup.MaxVolGroups, "volume group out of range");
		SetVolumeGroup(grp, mAmbientFadeGroupSave[(int)grp]);
	}

	public void SetVolumeGroupMultiply(SoundFXData.VolumeGroup grp, float multiplyValue)
	{
		VolumeGroups[(int)grp].VolumeMultiply = multiplyValue;
		UpdateGroupVolumes(grp);
	}

	private void SetAllVolumeGroupMultiply(float var)
	{
		for (int i = 0; i < 9; i++)
		{
			SetVolumeGroupMultiply((SoundFXData.VolumeGroup)i, var);
		}
	}

	private PoolSoundObject CreatePoolSoundObject(bool is2D)
	{
		PoolSoundObject poolSoundObject = new PoolSoundObject();
		PoolObjectFlags poolObjectFlags = PoolObjectFlags.None;
		GameObject gameObject;
		if (is2D)
		{
			gameObject = m_speaker2D;
			poolObjectFlags |= PoolObjectFlags.Is2D;
		}
		else
		{
			gameObject = new GameObject();
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.parent = base.transform;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.name = "Speaker3D";
		}
		poolSoundObject.m_flags = poolObjectFlags;
		poolSoundObject.m_gameObj = gameObject;
		poolSoundObject.m_audioSource = poolSoundObject.m_gameObj.AddComponent<AudioSource>();
		poolSoundObject.m_audioSource.enabled = false;
		poolSoundObject.m_audioSource.playOnAwake = false;
		if (!is2D)
		{
			poolSoundObject.m_audioLPF = poolSoundObject.m_gameObj.AddComponent<AudioLowPassFilter>();
			poolSoundObject.m_audioLPF.enabled = false;
		}
		return poolSoundObject;
	}

	private void DestroyPoolSoundObject(PoolSoundObject poolObj)
	{
		if (poolObj == null)
		{
			return;
		}
		if (poolObj.m_audioSource != null)
		{
			if (poolObj.m_audioSource.isPlaying)
			{
				poolObj.m_audioSource.Stop();
			}
			poolObj.m_audioSource = null;
		}
		if (poolObj.m_gameObj != null)
		{
			if ((poolObj.m_flags & PoolObjectFlags.Is2D) == 0)
			{
				UnityEngine.Object.Destroy(poolObj.m_gameObj);
			}
			poolObj.m_gameObj = null;
		}
		poolObj.m_flags = PoolObjectFlags.None;
	}

	private void InitialiseSoundObjectPool(int num2D, int num3D)
	{
		m_speaker2D = new GameObject();
		m_speaker2D.transform.position = Vector3.zero;
		m_speaker2D.transform.rotation = Quaternion.identity;
		m_speaker2D.transform.localScale = Vector3.one;
		m_speaker2D.transform.parent = base.transform;
		UnityEngine.Object.DontDestroyOnLoad(m_speaker2D);
		m_speaker2D.name = "00-Speaker2D";
		m_poolObject2D = new List<PoolSoundObject>();
		for (int i = 0; i < num2D; i++)
		{
			PoolSoundObject item = CreatePoolSoundObject(true);
			m_poolObject2D.Add(item);
		}
		m_poolObject3D = new List<PoolSoundObject>();
		for (int j = 0; j < num3D; j++)
		{
			PoolSoundObject poolSoundObject = CreatePoolSoundObject(false);
			poolSoundObject.m_gameObj.name = string.Format("{0:D2}-Speaker3D", j);
			m_poolObject3D.Add(poolSoundObject);
		}
	}

	private void DestroySoundObjectPool()
	{
		for (int i = 0; i < m_poolObject2D.Count; i++)
		{
			DestroyPoolSoundObject(m_poolObject2D[i]);
		}
		m_poolObject2D = null;
		for (int j = 0; j < m_poolObject3D.Count; j++)
		{
			DestroyPoolSoundObject(m_poolObject3D[j]);
		}
		m_poolObject3D = null;
	}

	private PoolSoundObject AllocateSoundObject(PoolObjectFlags requiredFlags)
	{
		PoolSoundObject poolSoundObject = null;
		bool flag = (requiredFlags & PoolObjectFlags.Is2D) != 0;
		List<PoolSoundObject> list = ((!flag) ? m_poolObject3D : m_poolObject2D);
		for (int i = 0; i < list.Count; i++)
		{
			if ((list[i].m_flags & PoolObjectFlags.Allocated) == 0)
			{
				poolSoundObject = list[i];
				break;
			}
		}
		if (poolSoundObject == null)
		{
			poolSoundObject = CreatePoolSoundObject(flag);
			list.Add(poolSoundObject);
		}
		poolSoundObject.m_flags |= PoolObjectFlags.Allocated;
		return poolSoundObject;
	}

	private void ReleaseSoundObject(PoolSoundObject soundObj)
	{
		if (soundObj == null)
		{
			return;
		}
		soundObj.m_flags &= ~PoolObjectFlags.Allocated;
		if (soundObj.m_audioSource != null)
		{
			if (soundObj.m_audioSource.isPlaying)
			{
				soundObj.m_audioSource.Stop();
			}
			soundObj.m_audioSource.enabled = false;
			soundObj.m_audioSource.clip = null;
		}
		if (soundObj.m_audioLPF != null)
		{
			soundObj.m_audioLPF.enabled = false;
		}
		if (soundObj.m_gameObj != null)
		{
			soundObj.m_gameObj.transform.localPosition = Vector3.zero;
			soundObj.m_gameObj.transform.localRotation = Quaternion.identity;
			soundObj.m_gameObj.transform.localScale = Vector3.one;
		}
	}
}
