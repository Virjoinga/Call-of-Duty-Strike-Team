using UnityEngine;

public class InsertionPointBlip : HudBlipIcon
{
	private const float ROTATION_SPEED = 20f;

	public override void Start()
	{
		IsAllowedInFirstPerson = false;
		ScreenEdgeOffsetMin = new Vector2(20f, 20f);
		ScreenEdgeOffsetMax = new Vector2(20f, 20f);
		base.Start();
		JustGoneOffScreen();
		base.ClampToEdgeOfScreen = false;
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		base.transform.Rotate(new Vector3(0f, 0f, 0f - 20f * TimeManager.DeltaTime));
	}
}
