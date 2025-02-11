using System.Globalization;
using UnityEngine;

public class GMGScorePanes : MonoBehaviour
{
	private const float X_BORDER_IN_PIXELS = 8f;

	private const float Y_BORDER_IN_PIXELS = 4f;

	public SpriteText PlayerScore;

	public SpriteText FriendName;

	public SpriteText FriendScore;

	public SpriteText Multiplier;

	private NumberFormatInfo mNfi;

	private Transform mMultiplierRoot;

	private Transform mFriendScoreRoot;

	private Transform mPlayerScoreRoot;

	private float mPixelSize;

	private void Awake()
	{
		mPixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		mMultiplierRoot = base.transform.FindChild("Multiplier");
		mFriendScoreRoot = base.transform.FindChild("FriendScore");
		mPlayerScoreRoot = base.transform.FindChild("PlayerScore");
		mNfi = GlobalizationUtils.GetNumberFormat(0);
		mNfi.NumberDecimalDigits = 0;
		UpdateVisibility();
	}

	public void Show(GMGData.GameType gameType)
	{
		ShowPlayerScoreRoot(true);
		ShowMultiplierRoot(gameType == GMGData.GameType.Domination);
		UpdateVisibility();
	}

	public void Hide()
	{
		ShowPlayerScoreRoot(false);
		ShowFriendScoreRoot(false);
		ShowMultiplierRoot(false);
	}

	public void UpdateScore(int playerScore)
	{
		if (PlayerScore != null)
		{
			PlayerScore.Text = playerScore.ToString("N", mNfi);
		}
		UpdateBackground(mPlayerScoreRoot);
	}

	public void UpdateMultiplier(int multiplier)
	{
		if (Multiplier != null)
		{
			Multiplier.Text = "x" + multiplier;
		}
		UpdateBackground(mMultiplierRoot);
	}

	public void UpdateFriend(LeaderboardResult friend)
	{
		ShowFriendScoreRoot(friend != null);
		if (friend != null && FriendName != null && FriendScore != null)
		{
			FriendName.Text = friend.Name;
			FriendScore.Text = friend.Rating.ToString("N", mNfi);
		}
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		if (FriendName != null && FriendScore != null && mFriendScoreRoot.gameObject.activeInHierarchy)
		{
			string text = FriendScore.Text;
			float width = FriendScore.GetWidth(text + "-");
			Vector3 position = FriendScore.transform.position;
			position.x -= width;
			FriendName.transform.position = position;
		}
		UpdateBackground(mPlayerScoreRoot);
		UpdateBackground(mMultiplierRoot);
		UpdateBackground(mFriendScoreRoot);
	}

	private void UpdateBackground(Transform root)
	{
		Vector2 vector = default(Vector2);
		SpriteText[] componentsInChildren = root.GetComponentsInChildren<SpriteText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SpriteText spriteText = componentsInChildren[i];
			if (spriteText != null)
			{
				string text = spriteText.Text;
				float width = spriteText.GetWidth(text + ((i + 1 >= componentsInChildren.Length) ? string.Empty : "-"));
				float y = spriteText.BaseHeight * spriteText.lineSpacing;
				vector.x += width;
				vector.y = y;
			}
		}
		Scale9Grid componentInChildren = root.GetComponentInChildren<Scale9Grid>();
		if (componentInChildren != null)
		{
			Vector3 position = root.position;
			position.x -= vector.x * 0.5f;
			componentInChildren.transform.position = position;
			componentInChildren.size.x = vector.x / mPixelSize + 16f;
			componentInChildren.size.y = vector.y / mPixelSize + 8f;
			componentInChildren.Resize();
		}
	}

	private void ShowMultiplierRoot(bool show)
	{
		if (mMultiplierRoot != null)
		{
			mMultiplierRoot.gameObject.SetActive(show);
		}
	}

	private void ShowFriendScoreRoot(bool show)
	{
		if (mFriendScoreRoot != null)
		{
			mFriendScoreRoot.gameObject.SetActive(show);
		}
	}

	private void ShowPlayerScoreRoot(bool show)
	{
		if (mPlayerScoreRoot != null)
		{
			mPlayerScoreRoot.gameObject.SetActive(show);
		}
	}
}
