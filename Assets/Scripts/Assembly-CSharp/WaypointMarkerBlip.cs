public class WaypointMarkerBlip : HudBlipIcon
{
	private PackedSprite mIcon;

	public override void Start()
	{
		mIcon = GetComponent<PackedSprite>();
		base.Start();
		SwitchOff();
	}

	public override void JustGoneOffScreen()
	{
		mIcon.Hide(true);
	}

	public override void JustComeOnScreen()
	{
		mIcon.Hide(false);
	}
}
