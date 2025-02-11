using UnityEngine;

public class MenuButton : MonoBehaviour
{
	public SpriteText Text;

	public PackedSprite Icon;

	public UIButton Button;

	public PackedSprite Cross;

	private bool mActive = true;

	public ContextMenuIcons IconType;

	public bool IsActive()
	{
		return mActive;
	}

	public void SetButtonActive(bool isActive)
	{
		if (mActive != isActive)
		{
			mActive = isActive;
			Cross.renderer.enabled = !mActive;
			float alpha = 1f;
			if (!isActive)
			{
				alpha = 0.3f;
			}
			if (Text != null)
			{
				Text.SetColor(Text.Color.Alpha(alpha));
			}
			if (Icon != null)
			{
				Icon.SetColor(Icon.Color.Alpha(alpha));
			}
		}
	}

	private void LateUpdate()
	{
	}
}
