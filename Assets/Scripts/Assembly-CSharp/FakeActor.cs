using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeActor : MonoBehaviour
{
	public FakeActorData m_Interface;

	public ActorDescriptor Actor;

	public AnimationClip[] Animations;

	public bool NoWeapon;

	private GameObject mModel;

	private AnimationState[] mStates;

	private HudBlipIcon mHudBlip;

	private void Awake()
	{
		ProbeUtils.Initialise();
	}

	private void Start()
	{
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		mModel = SceneNanny.CreateModel(Actor.Model.GetModelForTheme(theme), base.transform.position, base.transform.rotation);
		mModel.transform.parent = base.transform;
		if (!NoWeapon)
		{
			WeaponUtils.CreateThirdPersonModel(mModel, Actor.DefaultPrimaryWeapon);
		}
		ActorGenerator.AddBlip(mModel, Actor, mModel);
		List<AnimationState> list = new List<AnimationState>();
		AnimationClip[] animations = Animations;
		foreach (AnimationClip clip in animations)
		{
			AnimationState item = mModel.animation.AddClipSafe(clip, string.Format("Anim{0}", list.Count));
			list.Add(item);
		}
		mStates = list.ToArray();
		if (LightmapSettings.lightProbes != null)
		{
			Renderer[] componentsInChildren = mModel.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length > 0)
			{
				float[] coefficients = new float[27];
				Transform transform = mModel.transform.FindInHierarchy("Bip002 Pelvis");
				LightmapSettings.lightProbes.GetInterpolatedLightProbe(transform.position, componentsInChildren[0], coefficients);
				ProbeUtils.UpdateMaterials(coefficients, componentsInChildren);
			}
		}
	}

	private void Activate()
	{
		base.gameObject.SetActive(true);
	}

	private void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateState());
	}

	private IEnumerator UpdateState()
	{
		while (mStates == null)
		{
			yield return null;
		}
		int animIndex = Random.Range(0, mStates.Length);
		while (true)
		{
			AnimationState state = mStates[animIndex % mStates.Length];
			yield return PlayAnimation(state, false);
			StopAnimation(state);
			animIndex++;
		}
	}

	private YieldInstruction PlayAnimation(AnimationState state, bool loop)
	{
		state.enabled = true;
		state.weight = 1f;
		state.wrapMode = ((!loop) ? WrapMode.ClampForever : WrapMode.Loop);
		state.time = 0f;
		state.speed = Random.Range(1f, 2f);
		return new WaitForSeconds(state.length / state.speed);
	}

	private void StopAnimation(AnimationState state)
	{
		state.enabled = false;
	}
}
