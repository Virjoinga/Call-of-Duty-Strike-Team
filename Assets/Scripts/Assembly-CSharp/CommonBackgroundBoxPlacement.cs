using UnityEngine;

public class CommonBackgroundBoxPlacement : MonoBehaviour
{
	private const float ITEM_Z_OFFSET = 2f;

	private const float ICON_Z_OFFSET = 5.5f;

	private const float TEXT_Z_OFFSET = 6.5f;

	private const float PROGRESS_Z_OFFSET = 6f;

	public Vector2 TextIndent = new Vector2(2f, 0f);

	public float StartPositionAsPercentageOfBoxWidth;

	public float StartPositionAsPercentageOfBoxHeight;

	public float WidthAsPercentageOfBoxWidth = 1f;

	public float HeightAsPercentageOfBoxHeight = 1f;

	public int Border = 4;

	public Rect BoundingRect = new Rect(0f, 0f, 0f, 0f);

	private SpriteText[] mText;

	private IconControllerBase mIcon;

	private SimpleSprite mBackground;

	private PackedSprite mImage;

	private ProgressBar mProgressBar;

	private PercentageEditOption mPercentageEditOption;

	private UIButton mButton;

	private UIScrollList mScroll;

	private Scale9Grid mGrid;

	private void Awake()
	{
		mBackground = GetComponent<SimpleSprite>();
		mImage = GetComponent<PackedSprite>();
		mButton = GetComponent<UIButton>();
		mText = GetComponentsInChildren<SpriteText>();
		mIcon = GetComponentInChildren<IconControllerBase>();
		mProgressBar = GetComponentInChildren<ProgressBar>();
		mPercentageEditOption = GetComponentInChildren<PercentageEditOption>();
		mScroll = GetComponentInChildren<UIScrollList>();
		mGrid = GetComponentInChildren<Scale9Grid>();
	}

	public void Position(Vector3 boxPosition, Vector2 boxSize)
	{
		int num = Border;
		Vector2 textIndent = TextIndent;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num *= 2;
			textIndent *= 2f;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(boxPosition);
		float num2 = boxSize.x * WidthAsPercentageOfBoxWidth;
		float num3 = boxSize.y * HeightAsPercentageOfBoxHeight;
		float num4 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Vector3 vector2 = vector;
		vector2.x += GetXOffsetFromBox(boxSize.x, num);
		vector2.y -= GetYOffsetFromBox(boxSize.y, num);
		Vector3 position = Camera.main.ScreenToWorldPoint(vector2);
		float z = base.transform.position.z;
		position.z = z;
		base.transform.position = position;
		BoundingRect = new Rect(vector2.x, vector2.y - num3, num2, num3);
		if (mBackground != null)
		{
			AlignSpriteWithinArea(mBackground, vector2, num2 - (float)num * 2f, num3 - (float)num * 2f);
			if (!mBackground.pixelPerfect && !mBackground.autoResize)
			{
				mBackground.SetSize((num2 - (float)(num * 2)) * num4, (num3 - (float)(num * 2)) * num4);
			}
		}
		if (mImage != null)
		{
			AlignSpriteWithinArea(mImage, vector2, num2 - (float)num * 2f, num3 - (float)num * 2f);
		}
		if (mButton != null)
		{
			AlignSpriteWithinArea(mButton, vector2, num2 - (float)num * 2f, num3 - (float)num * 2f);
			if (!mButton.pixelPerfect && !mButton.autoResize)
			{
				mButton.SetSize((num2 - (float)(num * 2)) * num4, (num3 - (float)(num * 2)) * num4);
			}
		}
		if (mText != null)
		{
			for (int i = 0; i < mText.Length; i++)
			{
				AlignTextWithinArea(mText[i], vector2, num2 - (float)(num * 2), num3 - (float)(num * 2), textIndent, boxPosition.z - 6.5f);
			}
		}
		if (mIcon != null)
		{
			Vector3 position2 = vector2;
			position2.x += num2 * 0.5f;
			position2.y -= num3 * 0.5f;
			Vector3 position3 = Camera.main.ScreenToWorldPoint(position2);
			position3.z = boxPosition.z - 5.5f;
			mIcon.transform.position = position3;
		}
		if (mProgressBar != null)
		{
			float pixelWidth = num2 * 0.9f;
			mProgressBar.PixelWidth = pixelWidth;
			Vector3 position4 = vector2;
			position4.x += num2 * 0.05f;
			position4.y -= num3 * 0.5f;
			Vector3 position5 = Camera.main.ScreenToWorldPoint(position4);
			position5.z = boxPosition.z - 6f;
			mProgressBar.transform.position = position5;
		}
		if (mPercentageEditOption != null)
		{
			Vector3 position6 = vector2;
			position6.x += num2 * 0.05f;
			position6.y -= num3 * 0.5f;
			Vector3 position7 = Camera.main.ScreenToWorldPoint(position6);
			position7.z = boxPosition.z - 6f;
			mPercentageEditOption.transform.position = position7;
		}
		if (mScroll != null)
		{
			Vector3 position8 = vector2;
			position8.x += num2 * 0.5f;
			position8.y -= num3 * 0.5f;
			mScroll.transform.position = Camera.main.ScreenToWorldPoint(position8);
			mScroll.viewableArea = new Vector2(num2, num3);
			mScroll.UpdateCamera();
		}
		if (mGrid != null)
		{
			Vector3 position9 = vector2;
			position9.x += num2 * 0.5f;
			position9.y -= num3 * 0.5f;
			Vector3 position10 = Camera.main.ScreenToWorldPoint(position9);
			position10.z -= 5.5f;
			mGrid.transform.position = position10;
			mGrid.size = new Vector2(num2, num3);
			mGrid.Resize();
		}
	}

	private float GetXOffsetFromBox(float boxX, float border)
	{
		return boxX * (StartPositionAsPercentageOfBoxWidth - 0.5f) + border;
	}

	private float GetYOffsetFromBox(float boxY, float border)
	{
		return boxY * (StartPositionAsPercentageOfBoxHeight - 0.5f) + border;
	}

	public void AlignSpriteWithinArea(SpriteRoot sprite, Vector3 topLeft, float width, float height)
	{
		Vector3 position = topLeft;
		switch (sprite.anchor)
		{
		case SpriteRoot.ANCHOR_METHOD.UPPER_CENTER:
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER:
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER:
			position.x += width * 0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT:
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT:
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
			position.x += width;
			break;
		}
		switch (sprite.anchor)
		{
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT:
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER:
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
			position.y -= height;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT:
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER:
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT:
			position.y -= height * 0.5f;
			break;
		}
		float z = sprite.transform.position.z;
		Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
		position2.z = z;
		sprite.transform.position = position2;
	}

	private void AlignTextWithinArea(SpriteText text, Vector3 topLeft, float width, float height, Vector2 indent, float zOffset)
	{
		Vector3 position = topLeft;
		switch (text.anchor)
		{
		case SpriteText.Anchor_Pos.Upper_Left:
		case SpriteText.Anchor_Pos.Middle_Left:
		case SpriteText.Anchor_Pos.Lower_Left:
			position.x += indent.x;
			break;
		case SpriteText.Anchor_Pos.Upper_Center:
		case SpriteText.Anchor_Pos.Middle_Center:
		case SpriteText.Anchor_Pos.Lower_Center:
			position.x += width * 0.5f + indent.x;
			break;
		case SpriteText.Anchor_Pos.Upper_Right:
		case SpriteText.Anchor_Pos.Middle_Right:
		case SpriteText.Anchor_Pos.Lower_Right:
			position.x += width - indent.x;
			break;
		}
		switch (text.anchor)
		{
		case SpriteText.Anchor_Pos.Upper_Left:
		case SpriteText.Anchor_Pos.Upper_Center:
		case SpriteText.Anchor_Pos.Upper_Right:
			position.y -= indent.y;
			break;
		case SpriteText.Anchor_Pos.Lower_Left:
		case SpriteText.Anchor_Pos.Lower_Center:
		case SpriteText.Anchor_Pos.Lower_Right:
			position.y -= height - indent.y;
			break;
		case SpriteText.Anchor_Pos.Middle_Left:
		case SpriteText.Anchor_Pos.Middle_Center:
		case SpriteText.Anchor_Pos.Middle_Right:
			position.y -= height * 0.5f - indent.y;
			break;
		}
		Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
		position2.z = zOffset;
		text.transform.position = position2;
		text.maxWidth = width;
		text.maxWidthInPixels = true;
	}
}
