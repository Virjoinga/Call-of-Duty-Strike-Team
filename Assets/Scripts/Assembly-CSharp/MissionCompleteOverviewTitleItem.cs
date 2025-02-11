using UnityEngine;

public class MissionCompleteOverviewTitleItem : MonoBehaviour
{
	public SpriteText NameText;

	public SpriteText ValueText;

	public SimpleSprite[] Subtitles;

	public UIButton ExpandButton;

	public PackedSprite ArrowIcon;

	public Color ExpandedColour;

	public Color UnexpandedColour;

	private bool mExpanded;

	private void Start()
	{
		Color color = new Color(1f, 1f, 1f, 0.5f);
		SimpleSprite[] subtitles = Subtitles;
		foreach (SimpleSprite simpleSprite in subtitles)
		{
			simpleSprite.SetColor(color);
		}
		ExpandButton = base.gameObject.GetComponentInChildren<UIButton>();
		if (ArrowIcon != null && ExpandButton != null)
		{
			ArrowIcon.SetColor(UnexpandedColour);
			ExpandButton.SetColor(UnexpandedColour);
		}
		mExpanded = false;
	}

	public void LayoutComponents(float width, float height, MonoBehaviour caller, string method)
	{
		if (ExpandButton != null)
		{
			ExpandButton.scriptWithMethodToInvoke = caller;
			ExpandButton.methodToInvoke = method;
		}
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}

	public void SetUp(string text, string val)
	{
		if (NameText != null && ValueText != null)
		{
			NameText.Text = text;
			ValueText.Text = val;
		}
	}

	public void UpdateExpanded(bool expand)
	{
		if (mExpanded != expand && ArrowIcon != null && ExpandButton != null)
		{
			Color color = new Color(1f, 1f, 1f, (!expand) ? 0.5f : 1f);
			SimpleSprite[] subtitles = Subtitles;
			foreach (SimpleSprite simpleSprite in subtitles)
			{
				simpleSprite.SetColor(color);
			}
			ArrowIcon.gameObject.RotateBy(new Vector3(0f, 0f, (!expand) ? 0.25f : (-0.25f)), 0.25f, 0f);
			ArrowIcon.SetColor((!expand) ? UnexpandedColour : ExpandedColour);
			ExpandButton.SetColor((!expand) ? UnexpandedColour : ExpandedColour);
			mExpanded = expand;
		}
	}
}
