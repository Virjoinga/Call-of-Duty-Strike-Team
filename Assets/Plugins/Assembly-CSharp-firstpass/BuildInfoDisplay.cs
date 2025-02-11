using UnityEngine;

public class BuildInfoDisplay : MonoBehaviour
{
	public string LoadedSection = string.Empty;

	private SpriteText mSpriteText;

	private void Start()
	{
		mSpriteText = base.gameObject.GetComponent<SpriteText>();
		Refresh();
		mSpriteText.Hide(true);
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
			string text = TBFUtils.BundleVersion + " " + TBFUtils.BuildDate;
			text = "[MAS]" + text;
			if (LoadedSection != string.Empty)
			{
				text = text + " [LVL] " + LoadedSection;
			}
			mSpriteText.Text = text;
		}
	}
}
