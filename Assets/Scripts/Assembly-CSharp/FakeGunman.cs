using System.Collections;
using UnityEngine;

public class FakeGunman : MonoBehaviour
{
	public FakeGunmanData m_Interface;

	public ActorDescriptor Actor;

	public AnimationClip CoverIdleAnimation;

	public AnimationClip CoverToAimingAnimation;

	public AnimationClip FiringAnimation;

	public AnimationClip AimingToCoverAnimation;

	public AnimationClip AimingHitToCoverAnimation;

	public AnimationClip DeathAnimation;

	public string ProbeAnchorName;

	public string MuzzleLocatorName;

	public FakeGunfire Gunfire;

	public float CullDist = 2000f;

	private int mHealth;

	private bool mDead;

	private GameObject mModel;

	private Transform mMuzzleLocator;

	private AnimationState mCoverIdleState;

	private AnimationState mCoverToAimingState;

	private AnimationState mFiringState;

	private AnimationState mAimingToCoverState;

	private AnimationState mAimingHitToCoverState;

	private AnimationState mDeathState;

	private Renderer[] mRenderers;

	private void Awake()
	{
		if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.FakeSoldiers))
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ProbeUtils.Initialise();
		}
	}

	private void Start()
	{
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		mModel = SceneNanny.CreateModel(Actor.Model.GetModelForTheme(theme), base.transform.position, base.transform.rotation);
		mModel.transform.parent = base.transform;
		WeaponUtils.CreateThirdPersonModel(mModel, Actor.DefaultPrimaryWeapon);
		mMuzzleLocator = mModel.transform.FindInHierarchy(MuzzleLocatorName);
		if (CoverIdleAnimation != null)
		{
			mCoverIdleState = mModel.GetComponent<Animation>().AddClipSafe(CoverIdleAnimation, CoverIdleAnimation.name);
		}
		if (CoverToAimingAnimation != null)
		{
			mCoverToAimingState = mModel.GetComponent<Animation>().AddClipSafe(CoverToAimingAnimation, CoverToAimingAnimation.name);
		}
		if (FiringAnimation != null)
		{
			mFiringState = mModel.GetComponent<Animation>().AddClipSafe(FiringAnimation, FiringAnimation.name);
		}
		if (AimingToCoverAnimation != null)
		{
			mAimingToCoverState = mModel.GetComponent<Animation>().AddClipSafe(AimingToCoverAnimation, AimingToCoverAnimation.name);
		}
		if (AimingHitToCoverAnimation != null)
		{
			mAimingHitToCoverState = mModel.GetComponent<Animation>().AddClipSafe(AimingHitToCoverAnimation, AimingHitToCoverAnimation.name);
		}
		if (DeathAnimation != null)
		{
			mDeathState = mModel.GetComponent<Animation>().AddClipSafe(DeathAnimation, DeathAnimation.name);
		}
		if (LightmapSettings.lightProbes != null)
		{
			Renderer[] componentsInChildren = mModel.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length > 0)
			{
				float[] coefficients = new float[27];
				Transform transform = mModel.transform.FindInHierarchy(ProbeAnchorName);
				//LightmapSettings.lightProbes.GetInterpolatedProbe(transform.position, componentsInChildren[0], coefficients);
				ProbeUtils.UpdateMaterials(coefficients, componentsInChildren);
				mRenderers = componentsInChildren;
			}
		}
	}

	public void Deactivate()
	{
		StopAllCoroutines();
	}

	public void Reset()
	{
		Collider component = base.gameObject.GetComponent<Collider>();
		component.enabled = true;
		ClearAllAnims();
		OnEnable();
	}

	public void Damage()
	{
		if (!mDead)
		{
			Deactivate();
			StartCoroutine(DoImpact());
			mHealth--;
			if (mHealth == 0)
			{
				Kill();
				Container.SendMessage(m_Interface.ObjectToMessageOnDeath, m_Interface.FunctionToCallOnDeath);
			}
		}
	}

	public void Kill()
	{
		if (!mDead)
		{
			Deactivate();
			StartCoroutine(DoDeath());
			Collider component = base.gameObject.GetComponent<Collider>();
			component.enabled = false;
			mDead = true;
		}
	}

	private void ClearAllAnims()
	{
		StopAnimation(mCoverIdleState);
		StopAnimation(mCoverToAimingState);
		StopAnimation(mFiringState);
		StopAnimation(mAimingHitToCoverState);
		StopAnimation(mAimingToCoverState);
		StopAnimation(mDeathState);
	}

	private IEnumerator DoImpact()
	{
		ClearAllAnims();
		yield return PlayAnimation(mAimingHitToCoverState, false);
	}

	private IEnumerator DoDeath()
	{
		ClearAllAnims();
		yield return PlayAnimation(mDeathState, false);
	}

	private void OnEnable()
	{
		mDead = false;
		mHealth = Random.Range(3, 7);
		StartCoroutine(UpdateFiring());
	}

	private IEnumerator UpdateFiring()
	{
		while (mCoverIdleState == null)
		{
			yield return null;
		}
		while (true)
		{
			PlayAnimation(mCoverIdleState, true);
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			StopAnimation(mCoverIdleState);
			yield return PlayAnimation(mCoverToAimingState, false);
			StopAnimation(mCoverToAimingState);
			PlayAnimation(mFiringState, true);
			if (Gunfire != null && mMuzzleLocator != null && OnScreenPlusDisableRenderer())
			{
				int burstSize = Random.Range(3, 7);
				for (int i = 0; i < burstSize; i++)
				{
					WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.AN94MuzzleFlash);
					Gunfire.FireSingleShot(mMuzzleLocator.position, true);
					yield return new WaitForSeconds(0.1f);
				}
			}
			AnimationState returnToCoverState = ((!(Random.value < 0.2f)) ? mAimingToCoverState : mAimingHitToCoverState);
			StopAnimation(mFiringState);
			yield return PlayAnimation(returnToCoverState, false);
			StopAnimation(returnToCoverState);
		}
	}

	private YieldInstruction PlayAnimation(AnimationState state, bool loop)
	{
		OnScreenPlusDisableRenderer();
		state.enabled = true;
		state.weight = 1f;
		state.wrapMode = ((!loop) ? WrapMode.ClampForever : WrapMode.Loop);
		state.time = 0f;
		state.speed = Random.Range(1f, 2f);
		return new WaitForSeconds(state.length / state.speed);
	}

	private bool OnScreenPlusDisableRenderer()
	{
		if (mRenderers == null)
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - CameraManager.Instance.CurrentCamera.transform.position) > CullDist)
		{
			if (mRenderers.Length > 0)
			{
				for (int i = 0; i < mRenderers.Length; i++)
				{
					mRenderers[i].enabled = false;
				}
			}
			return false;
		}
		if (mRenderers.Length > 0)
		{
			for (int j = 0; j < mRenderers.Length; j++)
			{
				mRenderers[j].enabled = true;
			}
		}
		return true;
	}

	private void StopAnimation(AnimationState state)
	{
		state.enabled = false;
	}
}
