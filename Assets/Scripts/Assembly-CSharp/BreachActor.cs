using System.Collections;
using UnityEngine;

public class BreachActor : MonoBehaviour
{
	public BreachActorData m_Interface;

	public ActorDescriptor Actor;

	public SingleAnimation Idle;

	public SingleAnimation Reaction;

	public SingleAnimation Blend;

	private int mSeed;

	private GameObject mModel;

	private bool mReact;

	private HudBlipIcon mHudBlip;

	private Ragdoll mRagdoll;

	private SnapTarget mSnapTarget;

	public bool Alive { get; private set; }

	private void Awake()
	{
		ProbeUtils.Initialise();
	}

	private void Start()
	{
		mSeed = Random.Range(0, int.MaxValue);
		Alive = true;
		base.transform.position = NavMeshUtils.SampleNavMesh(base.transform.position).position;
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		mModel = SceneNanny.CreateModel(Actor.Model.GetModelForTheme(theme, mSeed), base.transform.position, base.transform.rotation);
		mModel.transform.parent = base.transform;
		WeaponUtils.CreateThirdPersonModel(mModel, Actor.DefaultPrimaryWeapon);
		mHudBlip = ActorGenerator.AddBlip(mModel, Actor, mModel);
		ShowHudBlip(m_Interface.VisibleBeforeBreach);
		BlobShadow blobShadow = EffectsController.Instance.AddBlobShadow(mModel, null, true);
		Transform snapPosition = mModel.transform.FindInHierarchy("Bip002 Spine2");
		mSnapTarget = ActorGenerator.CreateEmptySnapTarget(snapPosition);
		mSnapTarget.SnapPositionOverride = () => snapPosition.position;
		mSnapTarget.LockOnOverride = () => (!mReact) ? SnapTarget.LockOnType.DontLockOn : SnapTarget.LockOnType.LockOn;
		Idle.Initialise("Idle", mModel.GetComponent<Animation>());
		Reaction.Initialise("Reaction", mModel.GetComponent<Animation>());
		if (LightmapSettings.lightProbes != null)
		{
			Renderer[] componentsInChildren = mModel.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length > 0)
			{
				float[] array = new float[27];
				Transform transform = mModel.transform.FindInHierarchy("Bip002 Pelvis");
				//LightmapSettings.lightProbes.GetInterpolatedProbe(transform.position, componentsInChildren[0], array);
				ProbeUtils.UpdateMaterials(array, componentsInChildren);
				blobShadow.ShadowColour = ProbeUtils.CalculateShadowColour(array);
			}
		}
	}

	public void Activate()
	{
		base.gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	public void React()
	{
		mReact = true;
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateState());
	}

	private IEnumerator UpdateState()
	{
		while (Idle.State == null)
		{
			yield return null;
		}
		PlayAnimation(Idle, true);
		while (!mReact)
		{
			yield return null;
		}
		StopAnimation(Idle);
		HealthComponent health = base.gameObject.AddComponent<HealthComponent>();
		health.Initialise(0f, 1f, 1f);
		health.OnHealthChange += HealthChange;
		ShowHudBlip(true);
		mRagdoll = SetupHitBox(mModel, Actor, health);
		mRagdoll.TrackVelocities = true;
		mRagdoll.ParentBonesPerm();
		yield return PlayAnimation(Reaction, 0.8f, false);
		Object.Destroy(mSnapTarget);
		Object.Destroy(mRagdoll);
		if (GKM.AvailableSpawnSlots() > 0)
		{
			GameObject spawned = SceneNanny.CreateActor(position: base.transform.position, rotation: base.transform.rotation, descriptor: Actor, seed: mSeed);
			Actor actor = spawned.GetComponent<Actor>();
			if (actor == null)
			{
				Debug.LogWarning("BreachActor spawned without Actor component");
				yield break;
			}
			SpawnerUtils.InitialiseSpawnedActor(actor, null, BehaviourController.AlertState.Combat, null);
			actor.health.OnHealthEmpty += PostSpawnHealthEmpty;
			Reaction.Reset();
			Blend.Initialise("Blend", actor.animDirector.AnimationPlayer);
			Transform fakeRootTransform = base.transform.FindInHierarchy("Bip002");
			Transform actorRootTransform = actor.model.transform.FindInHierarchy("Bip002");
			TransformUtils.ModifyTransformToAlignPoints(actor.transform, fakeRootTransform, actorRootTransform);
			TransformUtils.ModifyTransformToAlignPoints(actor.model.transform, fakeRootTransform, actorRootTransform);
			actor.realCharacter.EnableNavMesh(true);
			Blend.State.layer = 16;
			Blend.State.weight = 1f;
			Blend.ClampToEnd();
			Object.Destroy(mModel);
			float blendOutTime = 0.1f;
			float blendOutCoefficient = 1f / blendOutTime;
			while (blendOutTime > 0f)
			{
				Blend.State.weight = blendOutTime * blendOutCoefficient;
				blendOutTime -= Time.deltaTime;
				yield return null;
			}
			Blend.Reset();
		}
		else
		{
			Alive = false;
			Object.Destroy(mModel);
		}
	}

	private void DoDeathCallout()
	{
		if (m_Interface.ObjectToCallOnDeath == null || m_Interface.ObjectToCallOnDeath.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectToCallOnDeath)
		{
			string message = string.Empty;
			if (m_Interface.FunctionToCallOnDeath != null && num < m_Interface.FunctionToCallOnDeath.Count)
			{
				message = m_Interface.FunctionToCallOnDeath[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	private void HealthChange(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		if (mRagdoll.Kinematic)
		{
			ActorAttachment componentInChildren = mModel.GetComponentInChildren<ActorAttachment>();
			if (componentInChildren != null)
			{
				componentInChildren.Drop();
			}
			mModel.GetComponent<Animation>().Stop();
			mRagdoll.SwitchToRagdoll();
			mSnapTarget.gameObject.SetActive(false);
			ShowHudBlip(false);
			StopAllCoroutines();
			DoDeathCallout();
			Alive = false;
			if (args.From != null)
			{
				Actor component = args.From.GetComponent<Actor>();
				if (EventHub.Instance != null && component != null)
				{
					Events.EventActor eventActor = new Events.EventActor();
					eventActor.CharacterType = CharacterType.Human;
					eventActor.Faction = FactionHelper.Category.Enemy;
					eventActor.Id = Actor.name;
					eventActor.Name = Actor.Name;
					eventActor.WeaponClass = WeaponDescriptor.WeaponClass.AssaultRifle;
					eventActor.WeaponId = Actor.DefaultPrimaryWeapon.Name;
					EventHub.Instance.Report(new Events.Kill(component.EventActor(), eventActor, args.DamageType, args.HeadShot, args.OneShotKill, args.LongShotKill));
				}
			}
		}
		float num = ((!mRagdoll.Kinematic) ? 10f : 100f);
		if (args.HitLocation != null && args.HitLocation.GetComponent<Rigidbody>() != null)
		{
			args.HitLocation.GetComponent<Rigidbody>().AddForceAtPosition((0f - num) * args.Impact.direction, args.Impact.position, ForceMode.Impulse);
		}
	}

	private void PostSpawnHealthEmpty(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		DoDeathCallout();
		Alive = false;
	}

	private IEnumerator SpawnDead(HealthComponent.HeathChangeEventArgs args)
	{
		GameObject spawned = SceneNanny.CreateActor(position: base.transform.position, rotation: base.transform.rotation, descriptor: Actor, seed: mSeed);
		Actor actor = spawned.GetComponent<Actor>();
		yield return null;
		actor.health.Kill(args.DamageType, args.From);
		actor.realCharacter.Ragdoll.CopyFrom(mRagdoll);
		Object.Destroy(mRagdoll);
		Object.Destroy(mModel);
	}

	private Ragdoll SetupHitBox(GameObject model, ActorDescriptor actorDescriptor, HealthComponent health)
	{
		ActorGenerator.InitialiseHitLocation initialiseHitLocation = delegate(HitLocation hitLocation)
		{
			hitLocation.Health = health;
		};
		return ActorGenerator.CreateRagdoll(model.name, actorDescriptor.HitBoxRig, model, initialiseHitLocation);
	}

	private void ShowHudBlip(bool active)
	{
		if (mHudBlip != null)
		{
			mHudBlip.gameObject.SetActive(active);
		}
	}

	private YieldInstruction PlayAnimation(SingleAnimation animation, bool loop)
	{
		return PlayAnimation(animation, 1f, loop);
	}

	private YieldInstruction PlayAnimation(SingleAnimation animation, float speed, bool loop)
	{
		AnimationState state = animation.State;
		state.enabled = true;
		state.weight = 1f;
		state.wrapMode = ((!loop) ? WrapMode.ClampForever : WrapMode.Loop);
		state.time = 0f;
		state.speed = speed;
		return new WaitForSeconds(state.length / state.speed);
	}

	private void StopAnimation(SingleAnimation animation)
	{
		animation.State.enabled = false;
	}
}
