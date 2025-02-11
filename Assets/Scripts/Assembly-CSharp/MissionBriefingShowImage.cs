using System.Collections;
using UnityEngine;

public class MissionBriefingShowImage : Command
{
	public string FocusItemImage;

	public string FocusItemImagex2;

	public float DurationInSeconds;

	public float BlockInSeconds;

	public Vector2 ScreenPosition;

	public Vector2 Size = new Vector2(267f, 185f);

	public MissionBriefingController.Layer Layer;

	private Texture mTexture;

	private Vector2 ScreenPositionRelative;

	private Vector2 ScreenSizeRelative;

	public override bool Blocking()
	{
		return BlockInSeconds > 0f;
	}

	public override IEnumerator Execute()
	{
		float blockTime = 0f;
		MissionBriefingController missionBriefing = MissionBriefingController.Instance;
		if (missionBriefing != null && ScreenPosition.x >= 0f && ScreenPosition.y >= 0f && DurationInSeconds > 0f)
		{
			string filename = FocusItemImage;
			if (TBFUtils.IsRetinaHdDevice() && FocusItemImagex2 != string.Empty && !TBFUtils.UseAlternativeLayout())
			{
				filename = FocusItemImagex2;
			}
			if (filename != string.Empty)
			{
				mTexture = Resources.Load(filename, typeof(Texture2D)) as Texture;
			}
			if ((Screen.height == 768 && Screen.width == 1024) || TBFUtils.IsRetinaHdDevice())
			{
				ScreenPositionRelative = new Vector2(ScreenPosition.x, ScreenPosition.y);
				ScreenSizeRelative = new Vector2(Size.x, Size.y);
			}
			else
			{
				float halfImageWidth = Size.x * 0.5f;
				float X = (float)Screen.width / 1024f;
				float Y = (float)Screen.height / 768f;
				ScreenSizeRelative = new Vector2(Size.x, Size.y);
				ScreenPositionRelative = new Vector2((ScreenPosition.x + halfImageWidth) * X - halfImageWidth, ScreenPosition.y * Y);
			}
			if (missionBriefing.ShowImage(mTexture, ScreenPositionRelative, ScreenSizeRelative, Layer, DurationInSeconds))
			{
				blockTime = BlockInSeconds;
			}
		}
		yield return new WaitForSeconds(blockTime);
	}
}
