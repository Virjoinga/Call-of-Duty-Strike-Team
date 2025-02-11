using UnityEngine;

public class WeaponCasingSound
{
	public struct CasingSound
	{
		public bool mActive;

		public Vector3 mPos;

		public float mTimeToAction;

		public bool mPlaying;
	}

	private const int mMaxSounds = 5;

	private const float mTimeToHitGroundMin = 0.3f;

	private const float mTimeToHitGroundMax = 0.7f;

	private const float mLengthToSide = 0.5f;

	private CasingSound[] mCasingSounds = new CasingSound[5];

	private SurfaceMaterial mSurfaceMaterial;

	private Vector3 mOldPos;

	public void Process()
	{
		ProcessActive();
	}

	public void Add(Vector3 pos, Vector3 velocity)
	{
		SetupSurface();
		for (int i = 0; i < 5; i++)
		{
			if (!mCasingSounds[i].mActive)
			{
				mCasingSounds[i].mActive = true;
				mCasingSounds[i].mPlaying = false;
				mCasingSounds[i].mPos = pos + velocity.normalized * 0.5f;
				mCasingSounds[i].mTimeToAction = Time.time + Random.Range(0.3f, 0.7f);
				break;
			}
		}
	}

	private void SetupSurface()
	{
		if ((bool)GameController.Instance && GameController.Instance.mFirstPersonActor != null)
		{
			Vector3 position = GameController.Instance.mFirstPersonActor.GetPosition();
			if (position != mOldPos)
			{
				SurfaceImpact surfaceImpact = ProjectileManager.Trace(position, position + Vector3.down, ProjectileManager.DefaultLayerProjectileMask);
				mSurfaceMaterial = surfaceImpact.material;
				mOldPos = position;
			}
		}
	}

	private void ProcessActive()
	{
		for (int i = 0; i < 5; i++)
		{
			if (!mCasingSounds[i].mActive)
			{
				continue;
			}
			if (mCasingSounds[i].mPlaying)
			{
				if (Time.time > mCasingSounds[i].mTimeToAction)
				{
					mCasingSounds[i].mActive = false;
					mCasingSounds[i].mPlaying = false;
				}
			}
			else if (Time.time > mCasingSounds[i].mTimeToAction)
			{
				float num = PlayCasingSound(mCasingSounds[i].mPos);
				mCasingSounds[i].mTimeToAction = Time.deltaTime + num;
				mCasingSounds[i].mPlaying = true;
			}
		}
	}

	public void Init()
	{
		for (int i = 0; i < 5; i++)
		{
			mCasingSounds[i].mActive = false;
			mCasingSounds[i].mPlaying = false;
		}
	}

	public void DeactivateAll()
	{
		for (int i = 0; i < 5; i++)
		{
			mCasingSounds[i].mActive = false;
			mCasingSounds[i].mPlaying = false;
		}
	}

	private float PlayCasingSound(Vector3 position)
	{
		float result = 0f;
		SoundFXData soundFXData = null;
		switch (mSurfaceMaterial)
		{
		case SurfaceMaterial.Default:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Masonry:
			soundFXData = ImpactSFX.Instance.MetalCastingStone;
			break;
		case SurfaceMaterial.Metal:
			soundFXData = ImpactSFX.Instance.MetalCastingMetal;
			break;
		case SurfaceMaterial.Wood:
			soundFXData = ImpactSFX.Instance.MetalCastingWood;
			break;
		case SurfaceMaterial.Snow:
			soundFXData = ImpactSFX.Instance.MetalCastingSnow;
			break;
		case SurfaceMaterial.Ice:
			soundFXData = ImpactSFX.Instance.MetalCastingSnow;
			break;
		case SurfaceMaterial.Water:
			soundFXData = ImpactSFX.Instance.MetalCastingWater;
			break;
		case SurfaceMaterial.Carpet:
			soundFXData = ImpactSFX.Instance.MetalCastingWood;
			break;
		case SurfaceMaterial.Dirt:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Duct:
			soundFXData = ImpactSFX.Instance.MetalCastingMetal;
			break;
		case SurfaceMaterial.Glass:
			soundFXData = ImpactSFX.Instance.MetalCastingMetal;
			break;
		case SurfaceMaterial.Grass:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Leaves:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Mud:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Cement:
			soundFXData = ImpactSFX.Instance.MetalCastingStone;
			break;
		case SurfaceMaterial.Sand:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.None:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		case SurfaceMaterial.Flesh:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		default:
			soundFXData = ImpactSFX.Instance.MetalCastingDirt;
			break;
		}
		SoundManager.SoundInstance soundInstance = SoundManager.Instance.PlaySpotSfxAtPosition(soundFXData, position);
		if (soundInstance != null)
		{
			result = soundInstance.SampleLength;
		}
		return result;
	}
}
