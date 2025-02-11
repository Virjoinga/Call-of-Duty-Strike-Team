using System.Collections.Generic;
using UnityEngine;

public class MissionBriefingInstructionsPanel : MenuScreenBlade
{
	public enum Type
	{
		TopLeft = 0,
		Centre = 1
	}

	private const float INSTRUCTIONS_WIDTH = 700f;

	private const float BRACKETS_LEFT_BORDER = 16f;

	private const float BRACKETS_RIGHT_BORDER = 11f;

	private const float BRACKETS_Y_BORDER = 12f;

	private const float INSTRUCTIONS_HEIGHT_NO_IMAGE = 94f;

	private const float HUD_PADDING_FOR_LEFT_AND_RIGHT_BUTTONS = 350f;

	public Scale9Grid ImageBrackets;

	public Scale9Grid InstructionsBrackets;

	public Scale9Grid InstructionsBackground;

	public SpriteText Instructions;

	public PackedSprite ColonelImage;

	public Type PositionType;

	public DeliveredBy Messenger;

	public bool ActivateOnStart = true;

	public bool DestroyOnDeactivate = true;

	private PackedSprite mImage;

	private float mImageWidth;

	private float mImageHeight;

	private float mInstructionsWidth;

	private float mInstructionsHeightNoImage;

	private float mBracketsLeftBorder;

	private float mBracketsRightBorder;

	private float mBracketsYBorder;

	public void Start()
	{
		if (Messenger == DeliveredBy.Colonel)
		{
			mImage = ColonelImage;
		}
		else
		{
			mImage = null;
		}
		LayoutComponents();
		if (ActivateOnStart)
		{
			Activate();
		}
	}

	public override void OffScreen()
	{
		base.OffScreen();
		if (DestroyOnDeactivate)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetInstructions(string instructions)
	{
		if (Instructions != null)
		{
			List<string> list = new List<string>(instructions.Split(':'));
			Instructions.Text = string.Format("{0}{1}{2}", "[#5CC6CC]", list[0], ":[#FFFFFF]");
			list.RemoveAt(0);
			Instructions.Text += string.Join(":", list.ToArray());
		}
	}

	public void ClearInstructions()
	{
		if (Instructions != null)
		{
			Instructions.Text = string.Empty;
		}
	}

	private void LayoutComponents()
	{
		float num = 700f;
		mBracketsLeftBorder = 16f;
		mBracketsRightBorder = 11f;
		mBracketsYBorder = 12f;
		mInstructionsHeightNoImage = 94f;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num *= 2f;
			mBracketsLeftBorder *= 2f;
			mBracketsRightBorder *= 2f;
			mBracketsYBorder *= 2f;
			mInstructionsHeightNoImage *= 2f;
		}
		float num2 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		if (mImage != null)
		{
			Vector2 spritePixelSize = CommonHelper.GetSpritePixelSize(mImage);
			mImageWidth = spritePixelSize.x;
			mImageHeight = spritePixelSize.y;
			if (ImageBrackets != null)
			{
				ImageBrackets.size.x = mImageWidth + mBracketsLeftBorder + mBracketsRightBorder;
				ImageBrackets.size.y = mImageHeight + mBracketsYBorder * 2f;
				ImageBrackets.Resize();
				Vector3 position = vector;
				position.x -= mBracketsLeftBorder - mBracketsRightBorder;
				Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
				ImageBrackets.transform.position = position2;
				mImage.gameObject.SetActive(true);
				ImageBrackets.gameObject.SetActive(true);
			}
		}
		else
		{
			mImageWidth = 0f;
			mImageHeight = mInstructionsHeightNoImage;
			if (ColonelImage != null)
			{
				ColonelImage.gameObject.SetActive(false);
			}
			if (ImageBrackets != null)
			{
				ImageBrackets.gameObject.SetActive(false);
			}
		}
		float num3 = (float)Screen.width - mImageWidth - (mBracketsLeftBorder + mBracketsRightBorder) * 2f;
		if (PositionType == Type.Centre)
		{
			num3 -= 350f;
		}
		mInstructionsWidth = Mathf.Min(num, num3);
		if (InstructionsBrackets != null && InstructionsBackground != null && Instructions != null)
		{
			InstructionsBackground.size.x = mInstructionsWidth;
			InstructionsBackground.size.y = mImageHeight;
			InstructionsBackground.Resize();
			InstructionsBrackets.size.x = mInstructionsWidth + mBracketsLeftBorder + mBracketsRightBorder;
			InstructionsBrackets.size.y = mImageHeight + mBracketsYBorder * 2f;
			InstructionsBrackets.Resize();
			Instructions.maxWidth = mInstructionsWidth * 0.9f * num2;
			Vector3 position3 = vector;
			position3.x += InstructionsBrackets.size.x * 0.51f + mImageWidth * 0.5f;
			Vector3 position2 = Camera.main.ScreenToWorldPoint(position3);
			InstructionsBackground.transform.position = position2;
			InstructionsBrackets.transform.position = position2;
			CommonBackgroundBoxPlacement component = Instructions.GetComponent<CommonBackgroundBoxPlacement>();
			component.Position(InstructionsBackground.transform.position, InstructionsBackground.size);
		}
		if (PositionType == Type.TopLeft)
		{
			Vector3 position4 = new Vector3((float)Screen.width * 0.08f, (float)Screen.height - mImageHeight * 0.6f, 0f);
			Vector3 position2 = Camera.main.ScreenToWorldPoint(position4);
			mOnScreenOffset = (mOffScreenTopOffset = position2);
			mOffScreenTopOffset.y += mImageHeight * 2f * num2;
			base.transform.position = mOffScreenTopOffset;
			Orientation = BladeOrientation.Top;
		}
		else if (PositionType == Type.Centre)
		{
			float num4 = mInstructionsWidth + (mBracketsLeftBorder + mBracketsRightBorder) * 2f;
			Vector3 position5 = new Vector3((float)Screen.width * 0.5f - num4 * 0.5f, (float)Screen.height - mImageHeight * 0.6f, 0f);
			Vector3 position2 = Camera.main.ScreenToWorldPoint(position5);
			mOnScreenOffset = (mOffScreenTopOffset = position2);
			mOffScreenTopOffset.y += mImageHeight * 2f * num2;
			base.transform.position = mOffScreenBottomOffset;
			Orientation = BladeOrientation.Top;
		}
	}
}
