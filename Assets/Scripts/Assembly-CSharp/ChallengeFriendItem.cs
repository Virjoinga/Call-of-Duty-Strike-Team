using System;
using UnityEngine;

public class ChallengeFriendItem : MonoBehaviour
{
	public SpriteText RankNumber;

	public SpriteText FriendName;

	public SpriteText FriendScore;

	public PackedSprite EliteIcon;

	public RankIconController Rank;

	public Color StandardColour;

	public Color PlayerColour;

	public UIButton InviteButton;

	private CommonBackgroundBoxPlacement[] mPlacements;

	private UIListItemContainer mContainer;

	private float mWidth;

	private float mHeight;

	private int mAbsoluteLevelCached;

	private bool mIsMyScore;

	private ChallengeLeaderboardRow mLeaderboardRow;

	private FriendData mFriendData;

	public UIListItemContainer Container
	{
		get
		{
			return mContainer;
		}
	}

	public static event EventHandler<ValueEventArgs<ChallengeLeaderboardRow>> ListItemPressed;

	public static event EventHandler<ValueEventArgs<FriendData>> FriendListItemPressed;

	private void Awake()
	{
		mPlacements = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		mContainer = GetComponent<UIListItemContainer>();
		if (InviteButton != null)
		{
			InviteButton.scriptWithMethodToInvoke = this;
			InviteButton.methodToInvoke = "DoPressAction";
		}
		mAbsoluteLevelCached = XPManager.Instance.GetXPLevelAbsolute();
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	public void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (!MessageBox.MessageBoxInProgress)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			if (new Rect(vector.x - mWidth * 0.5f, vector.y - mHeight * 0.5f, mWidth, mHeight).Contains(fingerPos) && mLeaderboardRow != null)
			{
				DoPressAction();
			}
		}
	}

	public void DoPressAction()
	{
		if (mLeaderboardRow != null)
		{
			if (ChallengeFriendItem.ListItemPressed != null)
			{
				ChallengeFriendItem.ListItemPressed(this, new ValueEventArgs<ChallengeLeaderboardRow>(mLeaderboardRow));
			}
		}
		else if (mFriendData != null && ChallengeFriendItem.FriendListItemPressed != null)
		{
			ChallengeFriendItem.FriendListItemPressed(this, new ValueEventArgs<FriendData>(mFriendData));
		}
	}

	public void LayoutComponents(float width, float height)
	{
		mWidth = width;
		mHeight = height;
		Vector2 boxSize = new Vector2(mWidth, mHeight);
		CommonBackgroundBoxPlacement[] array = mPlacements;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}

	private void LateUpdate()
	{
		UpdateIconVisibility();
	}

	private void UpdateIconVisibility()
	{
		bool flag = true;
		if (Rank != null)
		{
			Rank.Hide(!flag);
		}
		if (EliteIcon != null)
		{
			EliteIcon.Hide(!flag);
		}
	}

	public void Setup(ChallengeLeaderboardRow data, bool myScore, bool isTime)
	{
		bool flag = data.Score > 0;
		mLeaderboardRow = data;
		mIsMyScore = myScore;
		mFriendData = null;
		if (RankNumber != null)
		{
			RankNumber.Text = ((!flag) ? string.Empty : (data.Rank + "."));
		}
		if (FriendName != null)
		{
			FriendName.Text = data.UserName;
			FriendName.SetColor((!mIsMyScore) ? StandardColour : PlayerColour);
		}
		if (FriendScore != null)
		{
			if (isTime)
			{
				FriendScore.Text = ((!flag) ? "-" : TimeUtils.GetMinutesSeconds(data.Score));
			}
			else
			{
				FriendScore.Text = ((!flag) ? "-" : data.Score.ToString());
			}
			FriendScore.SetColor((!mIsMyScore) ? StandardColour : PlayerColour);
		}
		if (Rank != null)
		{
			Rank.SetRank((!mIsMyScore) ? data.XPRank : mAbsoluteLevelCached);
		}
		if (EliteIcon != null)
		{
			bool elite = data.Elite;
			EliteIcon.SetColor((!elite) ? ColourChart.GreyedOut : Color.white);
		}
		UpdateIconVisibility();
	}

	public void Setup(FriendData data)
	{
		mFriendData = data;
		mLeaderboardRow = null;
		if (RankNumber != null)
		{
			RankNumber.Text = string.Empty;
		}
		if (FriendName != null)
		{
			FriendName.Text = data.UserName;
			FriendName.SetColor(StandardColour);
		}
		if (FriendScore != null)
		{
			FriendScore.Text = "-";
		}
		if (Rank != null)
		{
			Rank.SetNoRank();
			Rank.Hide(true);
		}
		if (EliteIcon != null)
		{
			EliteIcon.Hide(true);
		}
	}

	public void DebugSetup(int row, bool myScore)
	{
		if (RankNumber != null)
		{
			RankNumber.Text = row + ".";
		}
		if (FriendName != null)
		{
			FriendName.Text = "Friend " + (row + 1);
			FriendName.SetColor((!myScore) ? StandardColour : PlayerColour);
		}
		if (FriendScore != null)
		{
			FriendScore.Text = (20 - row).ToString();
			FriendScore.SetColor((!myScore) ? StandardColour : PlayerColour);
		}
		if (Rank != null)
		{
			int rank = UnityEngine.Random.Range(1, 50);
			Rank.SetRank(rank);
		}
		if (EliteIcon != null)
		{
			bool flag = UnityEngine.Random.Range(0, 3) == 0;
			EliteIcon.SetColor((!flag) ? ColourChart.GreyedOut : Color.white);
		}
	}
}
