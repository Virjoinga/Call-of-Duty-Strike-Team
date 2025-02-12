using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundleManager : MonoBehaviour
{
	public class NewSceneAddedEventArgs
	{
		public string SceneName { get; private set; }

		public NewSceneAddedEventArgs(string sceneName)
		{
			SceneName = sceneName;
		}
	}

	public delegate void NewSceneAddedEventHandler(object sender, NewSceneAddedEventArgs args);

	public delegate void NewSceneStartDownloadEventHandler(object sender, NewSceneAddedEventArgs args);

	private Dictionary<string, BundleWrapper> mBundles = new Dictionary<string, BundleWrapper>();

	private BundleWrapper mDlcBundleList;

	private bool mDlcBundlesRequested;

	private StringHolder mVersionData;

	private static BundleManager smInstance;

	public static BundleManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public event NewSceneAddedEventHandler OnNewSceneAdded;

	public event NewSceneStartDownloadEventHandler OnNewSceneStartDownload;

	public void NotifyNewSceneAdded(string sceneName)
	{
		if (this.OnNewSceneAdded != null)
		{
			this.OnNewSceneAdded(this, new NewSceneAddedEventArgs(sceneName));
		}
	}

	public void NotifyNewSceneStartDownload(string sceneName)
	{
		if (this.OnNewSceneStartDownload != null)
		{
			this.OnNewSceneStartDownload(this, new NewSceneAddedEventArgs(sceneName));
		}
	}

	public void Awake()
	{
		if (smInstance != null)
		{
			Debug.LogWarning("Can not have multiple BundleManagers, destroying the new one");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		smInstance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object @object = Resources.Load("versiondata");
		mVersionData = (StringHolder)@object;
		BundleWrapper.SetAssetBundleSubPath(mVersionData.content[0]);
	}

	public void ClearCache()
	{
		OnScreenLog.Instance.AddLine("Clearning asset bundle cache...");
		Caching.ClearCache();
		BundleWrapper.ResetStaticData();
		OnScreenLog.Instance.AppendToLine("done");
		BundleWrapper.SetAssetBundleSubPath(mVersionData.content[0]);
	}

	public void DeleteAllBundles()
	{
		DeleteDLCBundles();
		mDlcBundleList.Unload();
	}

	private void DeleteDLCBundles()
	{
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			mBundle.Value.Unload();
		}
		mBundles.Clear();
	}

	public void Start()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] files = directoryInfo.GetFiles();
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			OnScreenLog.Instance.AddLine("pf - " + fileInfo.Name);
		}
		OnScreenLog.Instance.AddLine("Looking for local resources...");
		StringHolder stringHolder = Resources.Load("embeddedbundlelist", typeof(StringHolder)) as StringHolder;
		if (stringHolder != null)
		{
			OnScreenLog.Instance.AppendToLine("found and loading");
			LoadBundles(stringHolder.content, BundleWrapper.BundleType.Resource);
		}
		else
		{
			OnScreenLog.Instance.AppendToLine("non found");
		}
		mDlcBundlesRequested = false;
		mDlcBundleList = new BundleWrapper("dlcbundlelist", BundleWrapper.BundleType.WebCustomCached, "dlcbundlelist");
	}

	public void Update()
	{
		if (IsReady())
		{
			return;
		}
		if (mDlcBundleList != null)
		{
			if (!mDlcBundlesRequested)
			{
				if (!mDlcBundleList.IsValid())
				{
					mDlcBundleList = null;
				}
				else if (mDlcBundleList.IsReady())
				{
					mDlcBundlesRequested = true;
					LoadBundles(mDlcBundleList.Toc, BundleWrapper.BundleType.WebCached);
				}
			}
			if (mDlcBundleList != null)
			{
				mDlcBundleList.Update();
			}
		}
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			mBundle.Value.Update();
		}
	}

	public bool IsReady()
	{
		if (mDlcBundleList != null && !mDlcBundlesRequested)
		{
			return false;
		}
		return AreAllBundlesReady();
	}

	private bool AreAllBundlesReady()
	{
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			if (!mBundle.Value.IsReady())
			{
				return false;
			}
		}
		return true;
	}

	private void LoadBundles(string[] names, BundleWrapper.BundleType type)
	{
		foreach (string text in names)
		{
			if (!mBundles.ContainsKey(text))
			{
				mBundles[text] = new BundleWrapper(text, type);
				continue;
			}
			throw new Exception("bundle with duplicate name!! " + text);
		}
	}

	public List<string> FindAssetsWithNameContaining(string str)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			string[] toc = mBundle.Value.Toc;
			foreach (string text in toc)
			{
				if (text.Contains(str))
				{
					list.Add(mBundle.Key + "/" + text);
				}
			}
		}
		return list;
	}

	public List<BundleWrapper> FindBundlesWithObjectContaining(string str)
	{
		List<BundleWrapper> list = new List<BundleWrapper>();
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			string[] toc = mBundle.Value.Toc;
			foreach (string strA in toc)
			{
				if (string.Compare(strA, str, StringComparison.OrdinalIgnoreCase) == 0)
				{
					list.Add(mBundle.Value);
					break;
				}
			}
		}
		return list;
	}

	public BundleAssetRequest LoadObjectAsync(string gameObj, Type type)
	{
		BundleAssetRequest bundleAssetRequest = null;
		string[] array = gameObj.Split('/');
		if (array.Length == 1)
		{
			Debug.LogWarning("no path specified for game object, will need to search all bundles!");
			List<BundleWrapper> list = FindBundlesWithObjectContaining(gameObj);
			if (list.Count == 0)
			{
				Debug.LogWarning("could not find " + gameObj + " to load");
				return null;
			}
			if (list.Count > 1)
			{
				throw new NotImplementedException("need to pick which one is correct");
			}
			return list[0].LoadObjectAsync(gameObj, type);
		}
		return mBundles[array[0]].LoadObjectAsync(array[1], type);
	}

	public List<string> GetCurrentlyLoadedScenes()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, BundleWrapper> mBundle in mBundles)
		{
			string bundleSceneName = mBundle.Value.GetBundleSceneName();
			if (bundleSceneName != null && bundleSceneName != string.Empty)
			{
				list.Add(bundleSceneName);
			}
		}
		return list;
	}
}
