using UnityEngine;

public class BriefingShowImage : ContainerOverride
{
	public Texture2D ItemImage;

	public Vector2 ScreenPosition;

	public Vector2 ImageSize = new Vector2(267f, 185f);

	public float DurationInSeconds;

	public float BlockTime;

	public MissionBriefingController.Layer Layer;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MissionBriefingShowImage missionBriefingShowImage = cont.FindComponentOfType(typeof(MissionBriefingShowImage)) as MissionBriefingShowImage;
		string texturePath;
		string x2TexturePath;
		CommonHelper.FindTexturePaths(ItemImage, out texturePath, out x2TexturePath);
		if (missionBriefingShowImage != null)
		{
			missionBriefingShowImage.FocusItemImage = texturePath;
			missionBriefingShowImage.FocusItemImagex2 = x2TexturePath;
			missionBriefingShowImage.DurationInSeconds = DurationInSeconds;
			missionBriefingShowImage.BlockInSeconds = BlockTime;
			missionBriefingShowImage.ScreenPosition = ScreenPosition;
			missionBriefingShowImage.Layer = Layer;
			missionBriefingShowImage.Size = ImageSize;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
	}

	private void Awake()
	{
		CommonHelper.UnloadImageIfSurplus(ItemImage);
		ItemImage = null;
	}
}
