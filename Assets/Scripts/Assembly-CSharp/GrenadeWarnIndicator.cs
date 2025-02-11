using UnityEngine;

public class GrenadeWarnIndicator : MonoBehaviour
{
	public PackedSprite Indicator;

	public PackedSprite Icon;

	private Grenade mGrenade;

	public Vector3 GrenadePosition { get; private set; }

	public void LateUpdate()
	{
		if (mGrenade == null)
		{
			Indicator.Color = Indicator.Color.Alpha(0f);
			Icon.Color = Icon.Color.Alpha(0f);
			return;
		}
		GrenadePosition = mGrenade.transform.position;
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null)
		{
			FirstPersonCamera firstPersonCamera = mFirstPersonActor.realCharacter.FirstPersonCamera;
			Vector3 position = firstPersonCamera.transform.position;
			Vector2 vector = GrenadePosition.xz() - position.xz();
			float num = 0f - Mathf.Sign(Vector2.Dot(firstPersonCamera.transform.right.xz(), vector));
			float num2 = Vector2.Angle(firstPersonCamera.transform.forward.xz(), vector);
			Indicator.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, num * num2));
			float magnitude = (position - GrenadePosition).magnitude;
			float alpha = Mathf.InverseLerp(mGrenade.DamageRadius + 1f, mGrenade.DamageRadius, magnitude);
			Indicator.Color = Indicator.Color.Alpha(alpha);
			Icon.Color = Icon.Color.Alpha(alpha);
		}
	}

	public void GrenadeLanded(Grenade grenade)
	{
		mGrenade = grenade;
	}
}
