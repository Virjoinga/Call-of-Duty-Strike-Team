using UnityEngine;

public class GlobeSatellite : MonoBehaviour
{
	public float MinScale = 1f;

	public float MaxScale = 2f;

	public float MinPitch = -85f;

	public float MaxPitch = 85f;

	public float MinRadius = 90f;

	public float MaxRadius = 110f;

	public int ChanceOfAppearingOnEquator_OneIn = 2;

	public int ChanceOfMovement_OneIn = 100;

	public static float MinYaw = -180f;

	public static float MaxYaw = 180f;

	public static float MinPitchMovement;

	public static float MaxPitchMovement = 1f;

	public static float MinYawMovement = 1f;

	public static float MaxYawMovement = 10f;

	private PackedSprite mSprite;

	private Camera mFace;

	private float mDistanceAlpha;

	private float mPitch;

	private float mYaw;

	private float mRadius;

	private float mPitchMovement;

	private float mYawMovement;

	private float mFadeInAlpha;

	public Camera Face
	{
		set
		{
			mFace = value;
		}
	}

	private void Start()
	{
		mSprite = GetComponent<PackedSprite>();
		float yaw = Random.Range(MinYaw, MaxYaw);
		float num = Random.Range(MinRadius, MaxRadius);
		float pitch = 0f;
		if (Random.Range(0, ChanceOfAppearingOnEquator_OneIn) > 0)
		{
			pitch = Random.Range(MinPitch, MaxPitch);
		}
		if (ChanceOfMovement_OneIn != 0 && Random.Range(0, ChanceOfMovement_OneIn) == 0)
		{
			mPitchMovement = Random.Range(MinPitchMovement, MaxPitchMovement);
			mYawMovement = Random.Range(MinYawMovement, MaxYawMovement);
		}
		mDistanceAlpha = 1f - (num - MinRadius) / (MaxRadius - MinRadius);
		if (mSprite != null)
		{
			mSprite.SetColor(new Color(1f, 1f, 1f, mDistanceAlpha * mFadeInAlpha));
		}
		float num2 = Random.Range(MinScale, MaxScale);
		base.transform.localScale = new Vector3(num2, num2, num2);
		SetPositionFromAnglesAndRadius(pitch, yaw, num);
	}

	private void LateUpdate()
	{
		if (mPitchMovement != 0f || mYawMovement != 0f)
		{
			float pitch = mPitch + mPitchMovement * Time.deltaTime;
			float yaw = mYaw + mYawMovement * Time.deltaTime;
			SetPositionFromAnglesAndRadius(pitch, yaw, mRadius);
		}
		if (mFace != null)
		{
			base.transform.LookAt(mFace.transform);
		}
		float num = 0f;
		if (mFace != null)
		{
			Vector3 normalized = mFace.transform.right.normalized;
			Vector3 normalized2 = mFace.transform.up.normalized;
			Vector3 normalized3 = base.transform.position.normalized;
			float num2 = Mathf.Abs(Vector3.Dot(normalized, normalized3));
			float num3 = Mathf.Abs(Vector3.Dot(normalized2, normalized3));
			num = Mathf.Max(num2 * 0.5f, num3 * 0.5f);
		}
		if (mSprite != null)
		{
			mSprite.SetColor(new Color(1f, 1f, 1f, mDistanceAlpha * num * mFadeInAlpha));
		}
		if (mFadeInAlpha < 1f)
		{
			mFadeInAlpha += Time.deltaTime * 0.5f;
			if (mFadeInAlpha > 1f)
			{
				mFadeInAlpha = 1f;
			}
		}
	}

	public void SetPositionFromAnglesAndRadius(float pitch, float yaw, float radius)
	{
		if (pitch > 85f)
		{
			pitch -= 170f;
		}
		if (pitch < -85f)
		{
			pitch += 170f;
		}
		if (yaw > 180f)
		{
			yaw -= 360f;
		}
		if (yaw < -180f)
		{
			yaw += 360f;
		}
		Quaternion quaternion = Quaternion.AngleAxis(0f - pitch, Vector3.right);
		Quaternion quaternion2 = Quaternion.AngleAxis(yaw, Vector3.up);
		Vector3 vector = Vector3.forward * radius;
		vector = quaternion * vector;
		vector = quaternion2 * vector;
		mPitch = pitch;
		mYaw = yaw;
		mRadius = radius;
		base.transform.position = vector;
	}
}
