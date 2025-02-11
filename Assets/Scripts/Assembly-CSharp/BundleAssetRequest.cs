using UnityEngine;

public class BundleAssetRequest
{
	private AssetBundleRequest mAssetBundleReq;

	private Object mResourceAsset;

	public BundleAssetRequest(AssetBundleRequest req)
	{
		mAssetBundleReq = req;
		mResourceAsset = null;
	}

	public BundleAssetRequest(Object obj)
	{
		mAssetBundleReq = null;
		mResourceAsset = obj;
	}

	public bool IsDone()
	{
		if (mAssetBundleReq == null)
		{
			return true;
		}
		return mAssetBundleReq.isDone;
	}

	public float Progress()
	{
		if (mAssetBundleReq == null)
		{
			return 1f;
		}
		return mAssetBundleReq.progress;
	}

	public Object Asset()
	{
		if (mResourceAsset != null)
		{
			return mResourceAsset;
		}
		return mAssetBundleReq.asset;
	}
}
