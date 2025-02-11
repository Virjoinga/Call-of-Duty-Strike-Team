using System;
using System.Collections;
using UnityEngine;

public class GlobeSelect : MonoBehaviour
{
	public enum MissionDisplayMode
	{
		Main = 0,
		Test = 1
	}

	private const int NUM_SATELLITES = 200;

	private const int NUM_CLOUDS = 0;

	public Camera GlobeCamera;

	public GlobeSatellite SatellitePrefab;

	public GlobeSatellite CloudPrefab;

	public GameObject Globe;

	public GameObject GlobeBackground;

	public GameObject GlobeGlows;

	public GameObject[] GlobeOuterLines;

	public SpriteText LoadingText;

	private SearchTypeLabel mLoadingSearchTypeLabel;

	private static GlobeSelect smInstance;

	public static GlobeSelect Instance
	{
		get
		{
			return smInstance;
		}
	}

	public MissionDisplayMode DisplayMode { get; set; }

	public static event EventHandler<EventArgs> OnDisabled;

	private void Awake()
	{
		DisplayMode = MissionDisplayMode.Main;
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple GlobeSelect");
		}
		smInstance = this;
		Time.timeScale = 1f;
	}

	private void Start()
	{
		SwrveEventsProgression.FirstGlobeView();
		Globe.transform.localScale = Vector3.zero;
		GlobeBackground.ColorUpdate(Color.black, 0f);
		GlobeGlows.FadeUpdate(0f, 0f);
		GameObject[] globeOuterLines = GlobeOuterLines;
		foreach (GameObject gameObject in globeOuterLines)
		{
			gameObject.transform.localScale = Vector3.zero;
		}
		StartCoroutine(LoadStatsAndSettingsScenes());
	}

	private IEnumerator FadeInGlobe(float time)
	{
		GameObject[] globeOuterLines = GlobeOuterLines;
		foreach (GameObject go in globeOuterLines)
		{
			go.ScaleTo(Vector3.one, time * 0.8f, 0f, EaseType.easeOutCubic);
		}
		Globe.ScaleTo(Vector3.one, time, 0f, EaseType.easeOutCubic);
		yield return null;
	}

	private IEnumerator FadeInGlobeBackground(float time)
	{
		GlobeBackground.ColorTo(Color.white, time, 0f);
		yield return new WaitForSeconds(time * 0.5f);
		GlobeGlows.FadeTo(1f, time * 0.5f, 0f);
		yield return null;
	}

	private IEnumerator LoadStatsAndSettingsScenes()
	{
		AnimatedScreenBackground.Instance.Hide();
		mLoadingSearchTypeLabel = LoadingText.gameObject.GetComponent<SearchTypeLabel>();
		LoadingText.Text = Language.Get("S_LOADING_MSG_00");
		yield return new WaitForSeconds(0.5f);
		AsyncOperation settings = Application.LoadLevelAdditiveAsync("SettingsMenu");
		while (!settings.isDone)
		{
			yield return settings;
		}
		LoadingText.Text = Language.Get("S_LOADING_MSG_02");
		mLoadingSearchTypeLabel.ResetAndProcess();
		StartCoroutine(FadeInGlobe(0.5f));
		yield return new WaitForSeconds(0.5f);
		AsyncOperation stats = Application.LoadLevelAdditiveAsync("StatsMenu");
		while (!stats.isDone)
		{
			yield return stats;
		}
		StartCoroutine(FadeInGlobeBackground(0.5f));
		LoadingText.Text = Language.Get("S_LOADING_MSG_05");
		mLoadingSearchTypeLabel.ResetAndProcess();
		AsyncOperation credits = Application.LoadLevelAdditiveAsync("Credits");
		while (!credits.isDone)
		{
			yield return credits;
		}
		StartCoroutine(FadeInGlobeBackground(0.5f));
		LoadingText.Text = Language.Get("S_LOADING_MSG_06");
		mLoadingSearchTypeLabel.ResetAndProcess();
		StartCoroutine(CreateSatellites());
		AsyncOperation globeadditive = Application.LoadLevelAdditiveAsync("GlobeSelectAdditive");
		while (!globeadditive.isDone)
		{
			yield return globeadditive;
		}
		yield return new WaitForSeconds(0.3f);
		LoadingText.Text = string.Empty;
		mLoadingSearchTypeLabel.ResetAndProcess();
		Application.LoadLevelAdditiveAsync("AlwaysLoaded");
		CreditsController creditsController = UnityEngine.Object.FindObjectOfType(typeof(CreditsController)) as CreditsController;
		if (creditsController == null || !creditsController.SequenceIsRunning)
		{
			FrontEndController.Instance.TransitionTo(ScreenID.MissionSelect);
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void OnEnable()
	{
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 60;
		SoundManager.Instance.ActivateGlobeSFX();
	}

	private void OnDisable()
	{
		if (GlobeSelect.OnDisabled != null)
		{
			GlobeSelect.OnDisabled(this, new EventArgs());
		}
	}

	public static float GetPositionAlphaPos(Vector3 blipPos)
	{
		float result = 0f;
		if (Instance != null)
		{
			Vector3 normalized = Instance.GlobeCamera.transform.position.normalized;
			Vector3 normalized2 = blipPos.normalized;
			float num = Vector3.Dot(normalized, normalized2);
			if (num >= 0f)
			{
				result = 5f * num;
			}
		}
		return result;
	}

	private void Update()
	{
	}

	private IEnumerator CreateSatellites()
	{
		Transform satellites = base.transform.FindChild("Satellites");
		if (satellites == null)
		{
			GameObject satelliteChild = new GameObject("Satellites");
			satelliteChild.transform.parent = base.transform;
			satellites = satelliteChild.transform;
		}
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.GlobeSatalites) && SatellitePrefab != null)
		{
			int spawnsPerFrame = 0;
			for (int count2 = 0; count2 < 200; count2++)
			{
				GlobeSatellite satellite2 = UnityEngine.Object.Instantiate(SatellitePrefab) as GlobeSatellite;
				satellite2.transform.parent = satellites;
				satellite2.name = "Satellite " + count2;
				satellite2.Face = GlobeCamera;
				spawnsPerFrame++;
				if (spawnsPerFrame >= 5)
				{
					spawnsPerFrame = 0;
					yield return null;
				}
			}
		}
		if (CloudPrefab != null)
		{
			for (int count = 0; count < 0; count++)
			{
				GlobeSatellite satellite = UnityEngine.Object.Instantiate(CloudPrefab) as GlobeSatellite;
				satellite.transform.parent = satellites;
				satellite.name = "Cloud " + count;
				satellite.Face = GlobeCamera;
				yield return null;
			}
		}
	}
}
