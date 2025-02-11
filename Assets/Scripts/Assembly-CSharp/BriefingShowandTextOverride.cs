using UnityEngine;

public class BriefingShowandTextOverride : ContainerOverride
{
	public string Text;

	public float TextTime;

	public int NarrationId = -1;

	public Texture2D FocusItemImage;

	public Transform FocusObject;

	public float FocusDurationInSeconds;

	public Vector2 ShowImageOnlyScreenPosition;

	public Vector2 ImageSize = new Vector2(267f, 185f);

	public float ImageDurationInSeconds;

	public int StageId = -1;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MissionBriefingShowInstuctions missionBriefingShowInstuctions = cont.FindComponentOfType(typeof(MissionBriefingShowInstuctions)) as MissionBriefingShowInstuctions;
		MissionBriefingFocusOnItem missionBriefingFocusOnItem = cont.FindComponentOfType(typeof(MissionBriefingFocusOnItem)) as MissionBriefingFocusOnItem;
		MissionBriefingShowImage missionBriefingShowImage = cont.FindComponentOfType(typeof(MissionBriefingShowImage)) as MissionBriefingShowImage;
		string texturePath;
		string x2TexturePath;
		CommonHelper.FindTexturePaths(FocusItemImage, out texturePath, out x2TexturePath);
		if (missionBriefingShowInstuctions != null)
		{
			missionBriefingShowInstuctions.StringKey = Text;
			missionBriefingShowInstuctions.NarrationId = NarrationId;
			missionBriefingShowInstuctions.BlockInSeconds = TextTime;
			missionBriefingShowInstuctions.WaitId = StageId;
		}
		if (missionBriefingFocusOnItem != null)
		{
			missionBriefingFocusOnItem.FocusItemImage = texturePath;
			missionBriefingFocusOnItem.FocusItemImagex2 = x2TexturePath;
			missionBriefingFocusOnItem.FocusObject = FocusObject;
			missionBriefingFocusOnItem.DurationInSeconds = FocusDurationInSeconds;
			missionBriefingFocusOnItem.BlockInSeconds = FocusDurationInSeconds;
			missionBriefingFocusOnItem.Layer = MissionBriefingController.Layer.Front;
			missionBriefingFocusOnItem.Size = ImageSize;
			missionBriefingFocusOnItem.Direction = MissionBriefingHelper.FocusDirection.Left;
		}
		if (missionBriefingShowImage != null)
		{
			missionBriefingShowImage.FocusItemImage = texturePath;
			missionBriefingShowImage.FocusItemImagex2 = x2TexturePath;
			missionBriefingShowImage.DurationInSeconds = ImageDurationInSeconds;
			missionBriefingShowImage.ScreenPosition = ShowImageOnlyScreenPosition;
			missionBriefingShowImage.BlockInSeconds = 0f;
			missionBriefingShowImage.Layer = MissionBriefingController.Layer.Front;
			missionBriefingShowImage.Size = ImageSize;
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
