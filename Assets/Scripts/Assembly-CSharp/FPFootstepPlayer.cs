using UnityEngine;

public class FPFootstepPlayer : MonoBehaviour
{
	private const float MinDistanceToTriggerSfx = 0.001f;

	private const float MaxDistanceToTriggerSfx = 0.2f;

	private const float OneOverMaxDistanceToTriggerSfx = 5f;

	private const float MinVolume = 0.7f;

	private const float VolumeCalc = 0.3f;

	private const float mCrouchingVolume = 0.5f;

	private RealCharacter mRealCharacter;

	private float TimeBetweenSteps = 0.08f;

	private float VolumeBoost = 23f;

	private float TimeForNextStep;

	private Vector3 ActorPosition;

	private Vector2 PosOfLastStep2D;

	public void Awake()
	{
		mRealCharacter = base.gameObject.GetComponent<RealCharacter>();
	}

	public void Update()
	{
		if ((bool)mRealCharacter && mRealCharacter.IsFirstPerson)
		{
			ActorPosition = mRealCharacter.transform.position;
			float magnitude = (ActorPosition.xz() - PosOfLastStep2D).magnitude;
			PosOfLastStep2D = ActorPosition.xz();
			magnitude = Mathf.Clamp(magnitude, 0f, 0.2f);
			if (GameController.Instance.LastVelocity.magnitude != 0f && magnitude > 0.001f)
			{
				float num = magnitude * 5f;
				TimeForNextStep += Time.deltaTime + num * TimeBetweenSteps;
				if (TimeForNextStep > 1f)
				{
					TimeForNextStep -= 1f;
					PlayFootStepSfx(num, mRealCharacter.NoiseRadiusModifier);
				}
			}
			else
			{
				TimeForNextStep = 0f;
			}
		}
		else
		{
			TimeForNextStep = 0f;
		}
	}

	private SurfaceMaterial CurrentMaterial()
	{
		SurfaceImpact surfaceImpact = ProjectileManager.Trace(ActorPosition, ActorPosition + Vector3.down, ProjectileManager.ProjectileMask);
		return surfaceImpact.material;
	}

	private void PlayFootStepSfx(float volumeFrac, float perkModifier)
	{
		SurfaceMaterial surfaceMaterial = CurrentMaterial();
		SoundFXData soundFXData = null;
		switch (surfaceMaterial)
		{
		case SurfaceMaterial.Default:
			soundFXData = FootstepsSFX.Instance.FSRunWood;
			break;
		case SurfaceMaterial.Masonry:
			soundFXData = FootstepsSFX.Instance.FSRunMasonry;
			break;
		case SurfaceMaterial.Metal:
			soundFXData = FootstepsSFX.Instance.FSRunMetal;
			break;
		case SurfaceMaterial.Wood:
			soundFXData = FootstepsSFX.Instance.FSRunWood;
			break;
		case SurfaceMaterial.Snow:
			soundFXData = FootstepsSFX.Instance.FSRunSnow;
			break;
		case SurfaceMaterial.Ice:
			soundFXData = FootstepsSFX.Instance.FSRunIce;
			break;
		case SurfaceMaterial.Water:
			soundFXData = FootstepsSFX.Instance.FSRunWater;
			break;
		case SurfaceMaterial.Carpet:
			soundFXData = FootstepsSFX.Instance.FSRunCarpet;
			break;
		case SurfaceMaterial.Dirt:
			soundFXData = FootstepsSFX.Instance.FSRunDirt;
			break;
		case SurfaceMaterial.Glass:
			soundFXData = FootstepsSFX.Instance.FSRunGlass;
			break;
		case SurfaceMaterial.Grass:
			soundFXData = FootstepsSFX.Instance.FSRunGrass;
			break;
		case SurfaceMaterial.Leaves:
			soundFXData = FootstepsSFX.Instance.FSRunLeaves;
			break;
		case SurfaceMaterial.Mud:
			soundFXData = FootstepsSFX.Instance.FSRunMud;
			break;
		case SurfaceMaterial.Cement:
			soundFXData = FootstepsSFX.Instance.FSRunCement;
			break;
		case SurfaceMaterial.Sand:
			soundFXData = FootstepsSFX.Instance.FSRunSand;
			break;
		}
		if (soundFXData != null)
		{
			if (!mRealCharacter.IsCrouching())
			{
				float deltaTime = Time.deltaTime;
				volumeFrac = ((deltaTime == 0f) ? 0f : (volumeFrac / (deltaTime * VolumeBoost)));
				volumeFrac = volumeFrac * 0.3f + 0.7f;
				volumeFrac = Mathf.Clamp(volumeFrac, 0f, 1f);
			}
			else
			{
				volumeFrac = 0.5f;
			}
			float num = soundFXData.m_volume * volumeFrac;
			if (GameSettings.Instance.PerksEnabled)
			{
				num = ((!(perkModifier > 0.5f)) ? (num * (perkModifier * 0.3f)) : (num * (perkModifier * 0.5f)));
			}
			SoundManager.SoundInstance soundInstance = SoundManager.Instance.Play(soundFXData, base.gameObject);
			if (soundInstance != null)
			{
				soundInstance.Volume = num;
			}
		}
	}
}
