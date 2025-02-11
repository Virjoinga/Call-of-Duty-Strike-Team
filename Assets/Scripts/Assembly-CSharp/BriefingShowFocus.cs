using UnityEngine;

public class BriefingShowFocus : ContainerOverride
{
	public Texture2D FocusItemImage;

	public Transform FocusObject;

	public Vector2 ImageSize = new Vector2(267f, 185f);

	public float FocusDurationInSeconds;

	public float BlockTime;

	public MissionBriefingController.Layer Layer;

	public MissionBriefingHelper.FocusDirection ImageDirectionFromFocus;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MissionBriefingFocusOnItem missionBriefingFocusOnItem = cont.FindComponentOfType(typeof(MissionBriefingFocusOnItem)) as MissionBriefingFocusOnItem;
		string texturePath;
		string x2TexturePath;
		CommonHelper.FindTexturePaths(FocusItemImage, out texturePath, out x2TexturePath);
		if (missionBriefingFocusOnItem != null)
		{
			missionBriefingFocusOnItem.FocusItemImage = texturePath;
			missionBriefingFocusOnItem.FocusItemImagex2 = x2TexturePath;
			missionBriefingFocusOnItem.FocusObject = FocusObject;
			missionBriefingFocusOnItem.DurationInSeconds = FocusDurationInSeconds;
			missionBriefingFocusOnItem.BlockInSeconds = BlockTime;
			missionBriefingFocusOnItem.Layer = Layer;
			missionBriefingFocusOnItem.Size = ImageSize;
			missionBriefingFocusOnItem.Direction = ImageDirectionFromFocus;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
	}

	private void Awake()
	{
		CommonHelper.UnloadImageIfSurplus(FocusItemImage);
		FocusItemImage = null;
	}
}
