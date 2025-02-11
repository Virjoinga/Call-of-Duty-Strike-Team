using UnityEngine;

public class Tracer : MonoBehaviour
{
	public float Speed = 100f;

	private LineRenderer mRenderer;

	private Vector3 mPosition;

	private SurfaceImpact mImpact;

	private Vector3 mDirection;

	private float mSpeed;

	private bool mCameraAudioTriggered;

	private float mBirthTime;

	public void Awake()
	{
		mRenderer = GetComponent<LineRenderer>();
	}

	public void Fire(Vector3 origin, Vector3 target, Material material, SurfaceImpact impact, bool allowAudio)
	{
		Fire(origin, target, material, impact, Speed, allowAudio);
	}

	public void Fire(Vector3 origin, Vector3 target, Material material, SurfaceImpact impact, float speed, bool allowAudio)
	{
		Vector3 vector = target - origin;
		float magnitude = vector.magnitude;
		if (!(magnitude < 2f))
		{
			mSpeed = speed;
			mPosition = origin;
			mImpact = impact;
			mDirection = vector / magnitude;
			mRenderer.material = material;
			mCameraAudioTriggered = !allowAudio;
			base.enabled = true;
			mBirthTime = Time.time;
			mRenderer.enabled = true;
			mPosition += Time.deltaTime * mSpeed * mDirection;
			UpdateLineRenderer();
		}
	}

	public void Update()
	{
		if (mBirthTime + 1f < Time.time)
		{
			EffectsController.Instance.ReplaceTracer(base.gameObject);
			mRenderer.enabled = false;
			base.enabled = false;
			return;
		}
		base.transform.position = mPosition;
		if (Vector3.Dot(mImpact.position - mPosition, mDirection) < 0f)
		{
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.ImpactDecals))
			{
				EffectsController.Instance.TriggerSurfaceImpact(mImpact);
			}
			EffectsController.Instance.ReplaceTracer(base.gameObject);
			mRenderer.enabled = false;
			base.enabled = false;
			return;
		}
		Vector3 segmentB = mPosition + Time.deltaTime * mSpeed * mDirection;
		UpdateLineRenderer();
		Camera currentCamera = CameraManager.Instance.CurrentCamera;
		if (!mCameraAudioTriggered)
		{
			float num = Maths.DistanceToLineSegment(currentCamera.transform.position, mPosition, segmentB);
			if (num < 2f)
			{
				SoundManager.Instance.PlaySpotSfxAtPosition(WeaponSFX.Instance.TracerWhizBy, mPosition);
				mCameraAudioTriggered = true;
			}
		}
		mPosition = segmentB;
	}

	private void UpdateLineRenderer()
	{
		mRenderer.useWorldSpace = true;
		Vector3 position = mPosition;
		Vector3 position2 = mPosition + mDirection;
		mRenderer.SetPosition(0, position);
		mRenderer.SetPosition(1, position2);
	}
}
