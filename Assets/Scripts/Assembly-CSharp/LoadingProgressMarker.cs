using UnityEngine;

public class LoadingProgressMarker : MonoBehaviour
{
	public SimpleLog LoadingLog;

	public SpriteText Title;

	public LoadingScreenTip LoadingTip;

	private float mBufferedProgress;

	private float ExpectedLoadingTime = 100f;

	private float mLoadingDuration;

	private AsyncOperation ao;

	private AsyncOperation aoCleanUp;

	private static LoadingProgressMarker smInstance;

	public static LoadingProgressMarker Instance
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
			Debug.LogWarning("Can not have multiple LoadingProgressMarker, destroying the new one");
			Object.Destroy(base.gameObject);
		}
		else
		{
			smInstance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public void ClearOff()
	{
		AnimatedScreenBackground instance = AnimatedScreenBackground.Instance;
		if (instance != null)
		{
			instance.Hide();
		}
		if (LoadingTip != null)
		{
			LoadingTip.DoHide();
		}
		else
		{
			Debug.Log("Missing the loading tip!");
		}
		smInstance = null;
		Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		mBufferedProgress = 0f;
		mLoadingDuration = 0f;
		aoCleanUp = Resources.UnloadUnusedAssets();
	}

	private void OnEnable()
	{
		LoadingTip.DoShow();
	}

	private void Update()
	{
		float num = 0f;
		if (LoadingTip != null)
		{
			if (!LoadingTip.AnimationControl.IsOpen)
			{
				return;
			}
		}
		else
		{
			Debug.Log("Missing the loading tip!");
		}
		if (aoCleanUp == null)
		{
			num = ((ao == null) ? (-1f) : ao.progress);
		}
		else if (aoCleanUp.isDone)
		{
			TBFUtils.UberGarbageCollect();
			if (SceneLoader.SceneName != null && SceneLoader.SceneName != string.Empty)
			{
				ao = Application.LoadLevelAsync(SceneLoader.SceneName);
			}
			aoCleanUp = null;
		}
		mLoadingDuration += Time.deltaTime;
		if (num >= 0f)
		{
			float num2 = Mathf.Clamp(mLoadingDuration / ExpectedLoadingTime, 0f, 1f);
			float num3 = num;
			if (num3 > num2)
			{
				mLoadingDuration += (num3 + num2) / 2f * ExpectedLoadingTime * 0.4f;
			}
			float num4 = 4.6415887f;
			float num5 = num2 * num4;
			num5 = num5 * num5 * num5;
			int num6 = Mathf.Clamp(Mathf.FloorToInt(num5), 0, 100);
			float num7 = (num2 * (float)(100 - num6) + num3 * (float)num6) / 100f;
			float num8 = (num7 - mBufferedProgress) * Time.deltaTime;
			if (num8 > 0f)
			{
				mBufferedProgress += num8;
			}
			num = Mathf.Clamp01(mBufferedProgress);
		}
		else
		{
			LoadingLog.AddLine("error loading : " + SceneLoader.SceneName);
		}
	}
}
