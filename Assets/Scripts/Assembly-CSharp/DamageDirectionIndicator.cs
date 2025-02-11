using UnityEngine;

public class DamageDirectionIndicator : MonoBehaviour
{
	public PackedSprite Indicator;

	public AnimationCurve FlashCurve;

	private float mDamageTime;

	public Vector3 DamagePosition { get; private set; }

	public void Update()
	{
		float num = 0f;
		if ((bool)GameController.Instance)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			FirstPersonCamera firstPersonCamera = GameController.Instance.FirstPersonCamera;
			if (mFirstPersonActor != null && firstPersonCamera != null)
			{
				Vector3 position = firstPersonCamera.transform.position;
				Vector2 vector = DamagePosition.xz() - position.xz();
				float num2 = 0f - Mathf.Sign(Vector2.Dot(firstPersonCamera.transform.right.xz(), vector));
				float num3 = Vector2.Angle(firstPersonCamera.transform.forward.xz(), vector);
				base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, num2 * num3));
				num = FlashCurve.Evaluate(Time.time - mDamageTime);
			}
		}
		Indicator.Color = Indicator.color.Alpha(num);
		Activate(num > float.Epsilon);
	}

	public void TakeDamage(Vector3 worldPosition)
	{
		mDamageTime = Time.time;
		DamagePosition = worldPosition;
		Activate(true);
	}

	private void Activate(bool active)
	{
		Indicator.gameObject.SetActive(active);
	}
}
