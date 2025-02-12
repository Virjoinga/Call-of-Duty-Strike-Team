using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
	private const float BORDER = 5f;

	public SpriteText TitleText;

	public SpriteText ScoreText;

	public SpriteText RowValue;

	public RankIconController RankIcon;

	public PackedSprite EliteIcon;

	public PackedSprite VeteranIcon;

	private float mBorderInWorldSpace;

	private bool mShowVeteran;

	private bool mShowRank;

	public void LateUpdate()
	{
		UpdateIconVisibility();
	}

	public void SetData(string rank, string name, string score, int level, bool elite, bool veteran, bool player, bool showVeteranIcon)
	{
		mShowVeteran = showVeteranIcon;
		mShowRank = name.Length > 0;
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		int num2 = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num2 = 2;
		}
		mBorderInWorldSpace = 5f * (float)num2 * num;
		if (TitleText != null && ScoreText != null && RowValue != null && RankIcon != null && EliteIcon != null && VeteranIcon != null)
		{
			float width = TitleText.GetWidth("WWWWWWWWWWWWWWWWWWWWWWWWW");
			TitleText.maxWidth = width;
			TitleText.multiline = false;
			TitleText.Text = name;
			ScoreText.Text = score;
			RowValue.Text = rank;
			bool flag = name == string.Empty && score == string.Empty && rank == string.Empty;
			TitleText.SetColor((!player) ? ColourChart.LeaderboardEntry : ColourChart.LeaderboardPlayerEntry);
			ScoreText.SetColor((!player) ? ColourChart.LeaderboardEntry : ColourChart.LeaderboardPlayerEntry);
			RowValue.SetColor((!player) ? ColourChart.LeaderboardEntry : ColourChart.LeaderboardPlayerEntry);
			RankIcon.SetRank(level);
			EliteIcon.SetColor((!elite) ? ColourChart.GreyedOut : Color.white);
			VeteranIcon.SetColor((!veteran) ? ColourChart.GreyedOut : Color.white);
			RankIcon.gameObject.SetActive(!flag);
			EliteIcon.gameObject.SetActive(!flag);
			VeteranIcon.gameObject.SetActive(!flag);
			UpdateIconVisibility();
		}
	}

	private void UpdateIconVisibility()
	{
		VeteranIcon.GetComponent<Renderer>().enabled = mShowVeteran;
		RankIcon.Hide(!mShowRank);
	}

	public void LayoutComponents(float width, float height)
	{
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
		PlaceDynamic(width, height);
	}

	public void PlaceDynamic(float width, float height)
	{
		if (TitleText != null && ScoreText != null && RowValue != null && RankIcon != null && EliteIcon != null && VeteranIcon != null)
		{
			Vector3 position = RowValue.transform.position;
			float rankWidth = CommonHelper.GetRankWidth(RowValue, RowValue.Text);
			if (!VeteranIcon.Started)
			{
				VeteranIcon.Start();
			}
			position.x += rankWidth + VeteranIcon.width * 0.5f + mBorderInWorldSpace;
			VeteranIcon.transform.position = position;
			if (mShowVeteran)
			{
				position.x += VeteranIcon.width;
			}
			EliteIcon.transform.position = position;
			if (!EliteIcon.Started)
			{
				EliteIcon.Start();
			}
			position.x += EliteIcon.width;
			RankIcon.transform.position = position;
			if (!RankIcon.Sprite.Started)
			{
				RankIcon.Sprite.Start();
			}
			position.x += RankIcon.Sprite.width * 0.5f + mBorderInWorldSpace;
			TitleText.transform.position = position;
		}
	}
}
