using UnityEngine;

public class MasterBuildInfoDisplay : MonoBehaviour
{
	private SpriteText mSpriteText;

	private void Start()
	{
		mSpriteText = base.gameObject.GetComponent<SpriteText>();
		Refresh();
	}

	public void Hide(bool hide)
	{
		if (mSpriteText != null)
		{
			mSpriteText.Hide(hide);
		}
	}

	public bool IsHidden()
	{
		if (mSpriteText != null)
		{
			return mSpriteText.IsHidden();
		}
		return true;
	}

	public void Refresh()
	{
		if (mSpriteText != null)
		{
			string bundleVersion = TBFUtils.BundleVersion;
			mSpriteText.Text = bundleVersion;
		}
	}
}
