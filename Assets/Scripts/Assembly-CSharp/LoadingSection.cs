using UnityEngine;

public class LoadingSection : MonoBehaviour
{
	public SimpleLog LoadingLog;

	public SpriteText Title;

	public LoadingScreenTip LoadingTip;

	private bool mLoaded;

	private AsyncOperation m_AsyncOp;

	private AsyncOperation m_AsyncOpCleanUp;

	private static LoadingSection smInstance;

	public static LoadingSection Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		AnimatedScreenBackground instance = AnimatedScreenBackground.Instance;
		if (instance != null && !instance.IsActiveOrBecomingActive)
		{
			instance.Activate();
		}
		if (Title != null)
		{
			Title.Text = AutoLocalize.Get("S_LOADING");
		}
		if (smInstance != null)
		{
			Debug.LogWarning("Can not have multiple LoadingSections, destroying the new one");
			Object.Destroy(base.gameObject);
		}
		else
		{
			smInstance = this;
			mLoaded = false;
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnEnable()
	{
		LoadingTip.DoShow();
	}

	public void ClearOff()
	{
		AnimatedScreenBackground instance = AnimatedScreenBackground.Instance;
		if (instance != null)
		{
			instance.Hide();
		}
		LoadingTip.DoHide();
		smInstance = null;
		Object.Destroy(base.gameObject);
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if (sectionManager != null)
		{
			sectionManager.NotifySectionLogic();
		}
		HudBlipIcon.ClearCutsceneFlag();
		SectionLoader.SectionLoadTriggered = false;
	}

	private void Start()
	{
		m_AsyncOpCleanUp = Resources.UnloadUnusedAssets();
	}

	private void Update()
	{
		float num = 0f;
		if (!LoadingTip.AnimationControl.IsOpen)
		{
			return;
		}
		if (m_AsyncOpCleanUp == null)
		{
			num = ((m_AsyncOp != null) ? m_AsyncOp.progress : ((!Application.isEditor) ? (-1f) : (Time.time * 0.1f - (float)Mathf.FloorToInt(Time.time * 0.1f))));
		}
		else if (m_AsyncOpCleanUp.isDone)
		{
			m_AsyncOpCleanUp = null;
			TBFUtils.UberGarbageCollect();
			if (SectionLoader.SceneName != null && SectionLoader.SceneName != string.Empty)
			{
				m_AsyncOp = Application.LoadLevelAsync(SectionLoader.SceneName);
			}
		}
		if (num >= 0f)
		{
			if (!mLoaded && !(num >= 1f))
			{
			}
		}
		else
		{
			LoadingLog.AddLine("error loading : " + SectionLoader.SceneName);
		}
	}
}
