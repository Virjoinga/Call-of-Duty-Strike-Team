using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundleWrapper
{
	public enum BundleType
	{
		Resource = 0,
		WebCached = 1,
		WebUncached = 2,
		WebCustomCached = 3,
		Invalid = 4
	}

	public class BundleDoesntMatchPersistantDataCacheEventArgs
	{
		public string BundleName { get; private set; }

		public BundleDoesntMatchPersistantDataCacheEventArgs(string bundleName)
		{
			BundleName = bundleName;
		}
	}

	public delegate void BundleDoesntMatchPersistantDataCacheEventHandler(object sender, BundleDoesntMatchPersistantDataCacheEventArgs args);

	private BundleType mType;

	private string[] mToc;

	private WWW mWeb;

	private AssetBundleRequest mWwwRequest;

	private string mTocfile;

	private string mBundleName;

	private bool mBundleIsScene;

	private bool mIsReady;

	private AssetBundle mPersistantDataBundle;

	private AssetBundleCreateRequest mBundleCreateRequest;

	private static string mBundleSubPath;

	private int mDownloadVersion;

	public string[] Toc
	{
		get
		{
			return mToc;
		}
	}

	public static string AssetbundleBaseURL
	{
		get
		{
			string text = "http://" + DebugDlcServer.Ip + ":8000/";
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
			{
				return text + "standalonewindows/" + mBundleSubPath + "/";
			}
			return text + "iphone/" + mBundleSubPath + "/";
		}
	}

	public event BundleDoesntMatchPersistantDataCacheEventHandler OnBundleDoesntMatchPersistantDataCache;

	public BundleWrapper(string name, BundleType type, string tocFile)
	{
		Init(name, type, tocFile);
	}

	public BundleWrapper(string name, BundleType type)
	{
		Init(name, type, "toc");
	}

	public void Unload()
	{
		switch (mType)
		{
		case BundleType.Resource:
			break;
		case BundleType.WebCached:
		case BundleType.WebUncached:
			mWeb.assetBundle.Unload(true);
			break;
		case BundleType.WebCustomCached:
			if (mPersistantDataBundle == null)
			{
				mWeb.assetBundle.Unload(true);
			}
			else
			{
				mPersistantDataBundle.Unload(true);
			}
			break;
		default:
			if (mType != BundleType.Invalid)
			{
				throw new NotImplementedException();
			}
			break;
		}
	}

	private void Init(string name, BundleType type, string tocFile)
	{
		string[] array = name.Split(':');
		if (array.Length > 1)
		{
			mDownloadVersion = int.Parse(array[1]);
		}
		name = array[0];
		mIsReady = false;
		OnScreenLog.Instance.AddLine("Init bundle " + AssetbundleBaseURL + name + " of type " + type);
		mBundleIsScene = name.EndsWith(".unity3d", StringComparison.OrdinalIgnoreCase);
		mBundleName = name;
		mTocfile = tocFile;
		mToc = null;
		mType = type;
		mPersistantDataBundle = null;
		mBundleCreateRequest = null;
		string empty = string.Empty;
		if (mBundleIsScene)
		{
			BundleManager.Instance.NotifyNewSceneStartDownload(GetBundleSceneName());
		}
		switch (mType)
		{
		case BundleType.Resource:
		{
			empty = name;
			StringHolder toclist = Resources.Load(name + "/" + mTocfile, typeof(StringHolder)) as StringHolder;
			mToc = GetTocListFromStringHolder(toclist);
			mIsReady = true;
			break;
		}
		case BundleType.WebCached:
			empty = ((!mBundleIsScene) ? (AssetbundleBaseURL + name + ".assetbundle") : (AssetbundleBaseURL + name));
			mWeb = WWW.LoadFromCacheOrDownload(empty + "?p=" + DateTime.Now.Ticks, mDownloadVersion);
			break;
		case BundleType.WebUncached:
		case BundleType.WebCustomCached:
			empty = ((!mBundleIsScene) ? (AssetbundleBaseURL + name + ".assetbundle") : (AssetbundleBaseURL + name));
			mWeb = new WWW(empty + "?p=" + DateTime.Now.Ticks);
			break;
		default:
			throw new NotImplementedException();
		}
		OnScreenLog.Instance.AddLine(empty + " iscached=" + Caching.IsVersionCached(empty, mDownloadVersion));
	}

	private void MakeBundleInvalid()
	{
		OnScreenLog.Instance.AddLine(mBundleName + " is invalid. " + mWeb.error);
		Debug.LogWarning(mWeb.error);
		mType = BundleType.Invalid;
	}

	private bool IsAssetTheSameAsVersionInPersistantData(byte[] bytes)
	{
		string text = Application.persistentDataPath + Path.DirectorySeparatorChar + mBundleName + ".assetbundle";
		OnScreenLog.Instance.AddLine("Comparing " + text + " against downloaded data...");
		if (File.Exists(text))
		{
			byte[] array = File.ReadAllBytes(text);
			if (array.Length != bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != bytes[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private bool ReadAssetBundleFromPersistantData()
	{
		string text = Application.persistentDataPath + Path.DirectorySeparatorChar + mBundleName + ".assetbundle";
		OnScreenLog.Instance.AddLine("Reading " + text + "...");
		if (File.Exists(text))
		{
			byte[] binary = File.ReadAllBytes(text);
			mBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(binary);
			return true;
		}
		return false;
	}

	private void WriteAssetBundleToPersistantData(byte[] bytes)
	{
		string text = Application.persistentDataPath + Path.DirectorySeparatorChar + mBundleName + ".assetbundle";
		OnScreenLog.Instance.AddLine("Writing " + text + "...");
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		File.WriteAllBytes(text, bytes);
	}

	public void Update()
	{
		if (IsReady() || !IsValid())
		{
			return;
		}
		switch (mType)
		{
		case BundleType.Resource:
			break;
		case BundleType.WebCached:
		case BundleType.WebUncached:
		case BundleType.WebCustomCached:
			if (mWeb.error != null)
			{
				if (mType == BundleType.WebCustomCached)
				{
					if (mBundleCreateRequest == null)
					{
						OnScreenLog.Instance.AddLine("Failed to load asset bundle from web, trying prsistant data...");
						if (!ReadAssetBundleFromPersistantData())
						{
							MakeBundleInvalid();
						}
					}
					else if (mBundleCreateRequest.isDone)
					{
						mPersistantDataBundle = mBundleCreateRequest.assetBundle;
						if (mPersistantDataBundle != null)
						{
							OnScreenLog.Instance.AddLine("Loaded " + mBundleName + " from persistant data");
							StringHolder toclist = mPersistantDataBundle.LoadAsset(mTocfile, typeof(StringHolder)) as StringHolder;
							mToc = GetTocListFromStringHolder(toclist);
							mIsReady = true;
						}
						else
						{
							MakeBundleInvalid();
						}
					}
				}
				else
				{
					MakeBundleInvalid();
				}
			}
			else
			{
				if (!mWeb.isDone)
				{
					break;
				}
				if (mBundleIsScene)
				{
					OnScreenLog.Instance.AddLine("Downloaded scene bundle " + mBundleName);
					mIsReady = true;
					string bundleSceneName = GetBundleSceneName();
					BundleManager.Instance.NotifyNewSceneAdded(bundleSceneName);
				}
				else if (mWwwRequest == null)
				{
					if (mType == BundleType.WebCustomCached)
					{
						if (!IsAssetTheSameAsVersionInPersistantData(mWeb.bytes) && this.OnBundleDoesntMatchPersistantDataCache != null)
						{
							this.OnBundleDoesntMatchPersistantDataCache(this, new BundleDoesntMatchPersistantDataCacheEventArgs(mBundleName));
						}
						OnScreenLog.Instance.AddLine("Writing bundle " + mBundleName + " to persistant data...");
						WriteAssetBundleToPersistantData(mWeb.bytes);
						OnScreenLog.Instance.AppendToLine("done");
					}
					mWwwRequest = mWeb.assetBundle.LoadAssetAsync(mTocfile, typeof(StringHolder));
				}
				else if (mWwwRequest.isDone)
				{
					StringHolder toclist2 = (StringHolder)mWwwRequest.asset;
					mToc = GetTocListFromStringHolder(toclist2);
					mWwwRequest = null;
					mIsReady = true;
					OnScreenLog.Instance.AddLine("Downloaded and mounted toc for " + mBundleName);
					string[] array = mToc;
					foreach (string text in array)
					{
						OnScreenLog.Instance.AddLine(mBundleName + " : " + text);
					}
				}
			}
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private string[] GetTocListFromStringHolder(StringHolder toclist)
	{
		List<string> list = new List<string>();
		string[] content = toclist.content;
		foreach (string text in content)
		{
			if (!text.StartsWith("@"))
			{
				list.Add(text);
			}
			else if (text.StartsWith("@dlcid"))
			{
				TBFAssert.DoAssert(mDownloadVersion == 0, "download version already set");
				string[] array = text.Split(' ');
				mDownloadVersion = int.Parse(array[1]);
			}
		}
		string[] array2 = new string[list.Count];
		list.CopyTo(array2);
		return array2;
	}

	public bool IsValid()
	{
		return mType != BundleType.Invalid;
	}

	public bool IsReady()
	{
		return mIsReady;
	}

	public BundleAssetRequest LoadObjectAsync(string gameObj, Type type)
	{
		if (!IsReady())
		{
			throw new Exception("bundle '" + mBundleName + "' is not ready to be loaded from");
		}
		switch (mType)
		{
		case BundleType.Resource:
			return new BundleAssetRequest(Resources.Load(mBundleName + "/" + gameObj, type));
		case BundleType.WebCached:
		case BundleType.WebUncached:
			return new BundleAssetRequest(mWeb.assetBundle.LoadAssetAsync(gameObj, type));
		case BundleType.WebCustomCached:
			if (mPersistantDataBundle == null)
			{
				return new BundleAssetRequest(mWeb.assetBundle.LoadAssetAsync(gameObj, type));
			}
			return new BundleAssetRequest(mPersistantDataBundle.LoadAssetAsync(gameObj, type));
		default:
			throw new NotImplementedException();
		}
	}

	public static void ResetStaticData()
	{
		mBundleSubPath = string.Empty;
	}

	public static void SetAssetBundleSubPath(string path)
	{
		mBundleSubPath = path;
	}

	public string GetBundleSceneName()
	{
		if (mBundleIsScene)
		{
			return mBundleName.Split('.')[0].Split('-')[1];
		}
		return null;
	}
}
