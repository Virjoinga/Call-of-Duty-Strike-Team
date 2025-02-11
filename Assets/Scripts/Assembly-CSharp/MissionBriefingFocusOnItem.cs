using System.Collections;
using UnityEngine;

public class MissionBriefingFocusOnItem : Command
{
	public string FocusItemImage;

	public string FocusItemImagex2;

	public Transform FocusObject;

	public Vector2 Size = new Vector2(267f, 185f);

	public float DurationInSeconds;

	public float BlockInSeconds;

	public MissionBriefingController.Layer Layer;

	public MissionBriefingHelper.FocusDirection Direction;

	private Texture mTexture;

	public override bool Blocking()
	{
		return BlockInSeconds > 0f;
	}

	public override IEnumerator Execute()
	{
		float blockTime = 0f;
		MissionBriefingController missionBriefing = MissionBriefingController.Instance;
		if (missionBriefing != null && FocusItemImage != null && FocusObject != null)
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
			if (missionBriefing.FocusOnItem(mTexture, FocusObject, Size, Layer, DurationInSeconds, Direction))
			{
				blockTime = BlockInSeconds;
			}
		}
		yield return new WaitForSeconds(blockTime);
	}
}
