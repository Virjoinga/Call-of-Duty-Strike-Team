using System;
using System.Collections;
using UnityEngine;

public class HostageSequence : MonoBehaviour
{
	public HostageSequenceData m_Interface;

	public ActorDescriptor Hostage;

	public ActorDescriptor HostageTaker;

	public WeaponDescriptor HostageTakerWeapon;

	public SingleAnimation HostageReadyLoop;

	public SingleAnimation HostageTakerReadyLoop;

	public SingleAnimation HostageTakerPullUp;

	public SingleAnimation HostagePullUp;

	public SingleAnimation HostageHold;

	public SingleAnimation HostageTakerHold;

	public SingleAnimation HostageReleased;

	public SingleAnimation HostageExecuted;

	public SingleAnimation HostageShotByPlayer;

	public SingleAnimation HostageTakerFlinches;

	public SingleAnimation HostageTakerShoots;

	public NavGate NavGate;

	public ObjectMessage[] HostageDeadMessages;

	public ObjectMessage[] HostageTakerDeadMessages;

	public ObjectMessage[] HostageSavedMessages;

	private int mSeed;

	private GameObject mHostage;

	private GameObject mHostageTaker;

	private HealthComponent mHostageHealth;

	private HealthComponent mHostageTakerHealth;

	private Ragdoll mHostageRagdoll;

	private Ragdoll mHostageTakerRagdoll;

	private HudBlipIcon mHostageTakerHudBlip;

	private bool mPlayFinishSequence;

	private SnapTarget mSnapTarget;

	public bool Complete { get; private set; }

	public bool HostageDead { get; private set; }

	public bool HostageTakerDead { get; private set; }

	private void Awake()
	{
		ProbeUtils.Initialise();
	}

	private void Start()
	{
		mSeed = UnityEngine.Random.Range(0, int.MaxValue);
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		mHostage = CreateModel(Hostage, theme, null, mSeed);
		mHostageTaker = CreateModel(HostageTaker, theme, HostageTaker.DefaultPrimaryWeapon ?? HostageTakerWeapon, mSeed);
		HostageReadyLoop.Initialise("ReadyLoop", mHostage.GetComponent<Animation>());
		HostageTakerReadyLoop.Initialise("ReadyLoop", mHostageTaker.GetComponent<Animation>());
		HostagePullUp.Initialise("PullUp", mHostage.GetComponent<Animation>());
		HostageTakerPullUp.Initialise("PullUp", mHostageTaker.GetComponent<Animation>());
		HostageHold.Initialise("Hold", mHostage.GetComponent<Animation>());
		HostageTakerHold.Initialise("Hold", mHostageTaker.GetComponent<Animation>());
		HostageReleased.Initialise("Released", mHostage.GetComponent<Animation>());
		HostageExecuted.Initialise("Executed", mHostage.GetComponent<Animation>());
		HostageShotByPlayer.Initialise("ShotByPlayer", mHostage.GetComponent<Animation>());
		HostageTakerFlinches.Initialise("Flinch", mHostageTaker.GetComponent<Animation>());
		HostageTakerShoots.Initialise("Shoot", mHostageTaker.GetComponent<Animation>());
		mHostageTakerHudBlip = ActorGenerator.AddBlip(mHostageTaker, HostageTaker, mHostageTaker);
		ShowHudBlips(m_Interface.VisibleBeforeFinalSequence);
		Transform hostageTakerHead = mHostageTaker.transform.FindInHierarchy("Bip002 Head");
		mSnapTarget = ActorGenerator.CreateEmptySnapTarget(base.transform);
		mSnapTarget.SnapPositionOverride = () => ActorGenerator.GetHeadCentreFromHeadBone(hostageTakerHead);
		mSnapTarget.LockOnOverride = () => (!mPlayFinishSequence) ? SnapTarget.LockOnType.DontLockOn : SnapTarget.LockOnType.LockOn;
	}

	private void OnEnable()
	{
		mPlayFinishSequence = false;
		if (mHostage != null && mHostage.GetComponent<Animation>() != null)
		{
			mHostage.GetComponent<Animation>().Stop();
		}
		if (mHostageTaker != null && mHostageTaker.GetComponent<Animation>() != null)
		{
			mHostageTaker.GetComponent<Animation>().Stop();
		}
		StartCoroutine(UpdateState());
	}

	public void ReadyFinalSequence()
	{
	}

	public void TriggerFinalSequence()
	{
		mPlayFinishSequence = true;
	}

	private IEnumerator UpdateHostage()
	{
		yield return WaitWhile(() => HostageReadyLoop.State == null);
		PlayAnimation(HostageReadyLoop, true);
		yield return WaitWhile(() => !mPlayFinishSequence);
		StopAnimation(HostageReadyLoop);
		AttachHostageHitBoxes();
		PlayAnimation(HostagePullUp, false, 0f, 1f);
		HostagePullUp.NormalisedTime = 0.5f;
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostagePullUp.RemainingTime > 0f);
		StopAnimation(HostagePullUp);
		if (HostageTakerDead)
		{
			yield return PlayAnimation(HostageReleased, false);
			PlayAnimation(HostageReadyLoop, true);
			yield break;
		}
		if (HostageDead)
		{
			PlayAnimation(HostageShotByPlayer, false);
			yield break;
		}
		PlayAnimation(HostageHold, false);
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostageHold.RemainingTime > 0f);
		StopAnimation(HostageHold);
		if (HostageTakerDead)
		{
			yield return PlayAnimation(HostageReleased, false);
			PlayAnimation(HostageReadyLoop, true);
			yield break;
		}
		if (HostageDead)
		{
			PlayAnimation(HostageShotByPlayer, false);
			yield break;
		}
		PlayAnimation(HostageExecuted, false);
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostageExecuted.RemainingTime > 2f);
		if (HostageTakerDead)
		{
			yield return PlayAnimation(HostageReleased, false);
			PlayAnimation(HostageReadyLoop, true);
		}
		else if (HostageDead)
		{
			PlayAnimation(HostageShotByPlayer, false);
		}
	}

	private IEnumerator UpdateHostageTaker()
	{
		yield return WaitWhile(() => HostageTakerReadyLoop.State == null);
		PlayAnimation(HostageTakerReadyLoop, true);
		yield return WaitWhile(() => !mPlayFinishSequence);
		StopAnimation(HostageTakerReadyLoop);
		AttachHostageTakerHitBoxes();
		ShowHudBlips(true);
		PlayAnimation(HostageTakerPullUp, false, 0f, 1f);
		HostageTakerPullUp.NormalisedTime = 0.5f;
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostageTakerPullUp.RemainingTime > 0f);
		StopAnimation(HostageTakerPullUp);
		if (HostageTakerDead)
		{
			yield break;
		}
		if (HostageDead)
		{
			PlayAnimation(HostageTakerFlinches, false);
			yield return WaitWhile(() => HostageTakerFlinches.RemainingTime > 0.8f);
			yield break;
		}
		PlayAnimation(HostageTakerHold, false);
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostageTakerHold.RemainingTime > 0f);
		StopAnimation(HostageTakerHold);
		if (HostageTakerDead)
		{
			yield break;
		}
		if (HostageDead)
		{
			PlayAnimation(HostageTakerFlinches, false);
			yield return WaitWhile(() => HostageTakerFlinches.RemainingTime > 0.8f);
			yield break;
		}
		PlayAnimation(HostageTakerShoots, false);
		yield return WaitWhile(() => !HostageDead && !HostageTakerDead && HostageTakerShoots.RemainingTime > 2f);
		if (HostageTakerDead)
		{
			yield break;
		}
		if (HostageDead)
		{
			PlayAnimation(HostageTakerFlinches, false);
			yield return WaitWhile(() => HostageTakerFlinches.RemainingTime > 0.8f);
			yield break;
		}
		FireWeapon();
		HostageDead = true;
		mHostageHealth.OnHealthChange -= HostageHit;
		yield return WaitWhile(() => !HostageTakerDead && HostageTakerShoots.RemainingTime > 0.8f);
	}

	private IEnumerator UpdateState()
	{
		StartCoroutine(UpdateHostage());
		yield return StartCoroutine(UpdateHostageTaker());
		UnityEngine.Object.Destroy(mSnapTarget);
		if (!HostageTakerDead)
		{
			if (GKM.AvailableSpawnSlots() > 0)
			{
				GameObject spawned = SceneNanny.CreateActor(HostageTaker, base.transform.position, base.transform.rotation, mSeed);
				Actor actor = spawned.GetComponent<Actor>();
				if (actor != null)
				{
					actor.realCharacter.EnableNavMesh(true);
					actor.health.OnHealthEmpty += PostSpawnHealthEmpty;
					SpawnerUtils.InitialiseSpawnedActor(actor, null, BehaviourController.AlertState.Combat, null);
					SingleAnimation hostageTakerAnim = ((!HostageTakerFlinches.State.enabled) ? HostageTakerShoots : HostageTakerFlinches);
					StartCoroutine(BlendOutAndRemoveClip(actor.animDirector.AnimationPlayer, hostageTakerAnim.Clip, "HostageTakerBlend", hostageTakerAnim.State.time, 2f));
					actor.animDirector.AnimationPlayer.Sample();
					Transform fakeRootTransform = mHostageTaker.transform.FindInHierarchy("Bip002");
					Transform actorRootTransform = actor.model.transform.FindInHierarchy("Bip002");
					TransformUtils.ModifyTransformToAlignPoints(actor.transform, fakeRootTransform, actorRootTransform);
					TransformUtils.ModifyTransformToAlignPoints(actor.model.transform, fakeRootTransform, actorRootTransform);
					actor.realCharacter.EnableNavMesh(true);
				}
			}
			else
			{
				HostageTakerDead = true;
			}
			UnityEngine.Object.Destroy(mHostageTakerRagdoll);
			UnityEngine.Object.Destroy(mHostageTaker);
		}
		if (HostageDead)
		{
			NavGate.OpenNavGate();
			MessageHostageDead();
		}
		else
		{
			MessageHostageSaved();
		}
		if (HostageTakerDead)
		{
			MessageHostageTakerDead();
		}
		UnityEngine.Object.Destroy(mHostageHealth);
		UnityEngine.Object.Destroy(mHostageTakerHealth);
		UnityEngine.Object.Destroy(mHostageRagdoll);
		Complete = true;
	}

	private void FireWeapon()
	{
		Transform transform = mHostageTaker.transform.FindInHierarchy("muzzle_flash");
		EffectsController.Instance.GetBloodSpray(transform.position, transform.forward);
		EffectsController.Instance.GetBloodSpray(transform.position, -transform.forward);
		WeaponSFX.Instance.BerettaBurst.Play(base.gameObject);
	}

	private void MessageHostageDead()
	{
		ObjectMessage[] hostageDeadMessages = HostageDeadMessages;
		foreach (ObjectMessage objectMessage in hostageDeadMessages)
		{
			if (objectMessage.Object != null && objectMessage.Message != null && objectMessage.Message.Length > 0)
			{
				Container.SendMessage(objectMessage.Object, objectMessage.Message, base.gameObject);
			}
		}
		if (m_Interface.ObjectToCallOnHostageDeath == null || m_Interface.ObjectToCallOnHostageDeath.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectToCallOnHostageDeath)
		{
			string message = string.Empty;
			if (m_Interface.FunctionToCallOnHostageDeath != null && num < m_Interface.FunctionToCallOnHostageDeath.Count)
			{
				message = m_Interface.FunctionToCallOnHostageDeath[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	private void MessageHostageTakerDead()
	{
		ObjectMessage[] hostageTakerDeadMessages = HostageTakerDeadMessages;
		foreach (ObjectMessage objectMessage in hostageTakerDeadMessages)
		{
			if (objectMessage.Object != null && objectMessage.Message != null && objectMessage.Message.Length > 0)
			{
				Container.SendMessage(objectMessage.Object, objectMessage.Message, base.gameObject);
			}
		}
		if (m_Interface.ObjectToCallOnTakerDeath == null || m_Interface.ObjectToCallOnTakerDeath.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectToCallOnTakerDeath)
		{
			string message = string.Empty;
			if (m_Interface.FunctionToCallOnTakerDeath != null && num < m_Interface.FunctionToCallOnTakerDeath.Count)
			{
				message = m_Interface.FunctionToCallOnTakerDeath[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	private void MessageHostageSaved()
	{
		ObjectMessage[] hostageSavedMessages = HostageSavedMessages;
		foreach (ObjectMessage objectMessage in hostageSavedMessages)
		{
			if (objectMessage.Object != null && objectMessage.Message != null && objectMessage.Message.Length > 0)
			{
				Container.SendMessage(objectMessage.Object, objectMessage.Message, base.gameObject);
			}
		}
		if (m_Interface.ObjectToCallOnHostageSaved == null || m_Interface.ObjectToCallOnHostageSaved.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectToCallOnHostageSaved)
		{
			string message = string.Empty;
			if (m_Interface.FunctionToCallOnHostageSaved != null && num < m_Interface.FunctionToCallOnHostageSaved.Count)
			{
				message = m_Interface.FunctionToCallOnHostageSaved[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	private GameObject CreateModel(ActorDescriptor actorDescriptor, string theme, WeaponDescriptor weaponOverride, int seed)
	{
		GameObject gameObject = SceneNanny.CreateModel(actorDescriptor.Model.GetModelForTheme(theme, seed), base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform;
		if (weaponOverride != null)
		{
			WeaponUtils.CreateThirdPersonModel(gameObject, weaponOverride);
		}
		BlobShadow blobShadow = EffectsController.Instance.AddBlobShadow(gameObject, null, true);
		if (LightmapSettings.lightProbes != null)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length > 0)
			{
				float[] array = new float[27];
				Transform transform = gameObject.transform.FindInHierarchy("Bip002 Pelvis");
				//LightmapSettings.lightProbes.GetInterpolatedProbe(transform.position, componentsInChildren[0], array);
				ProbeUtils.UpdateMaterials(array, componentsInChildren);
				blobShadow.ShadowColour = ProbeUtils.CalculateShadowColour(array);
			}
		}
		return gameObject;
	}

	private void AttachHostageHitBoxes()
	{
		mHostageHealth = base.gameObject.AddComponent<HealthComponent>();
		mHostageHealth.Initialise(0f, 1f, 1f);
		mHostageHealth.OnHealthChange += HostageHit;
		mHostageRagdoll = SetupHitBox(mHostage, Hostage, mHostageHealth);
		mHostageRagdoll.ParentBonesPerm();
	}

	private void AttachHostageTakerHitBoxes()
	{
		mHostageTakerHealth = base.gameObject.AddComponent<HealthComponent>();
		mHostageTakerHealth.Initialise(0f, 1f, 1f);
		mHostageTakerHealth.OnHealthChange += HostageTakerHit;
		mHostageTakerRagdoll = SetupHitBox(mHostageTaker, HostageTaker, mHostageTakerHealth);
		mHostageTakerRagdoll.ParentBonesPerm();
	}

	private void MakeHostageInvulnerable()
	{
		mHostageHealth.OnHealthChange -= HostageHit;
		for (int i = 0; i < mHostageRagdoll.Bones.Length; i++)
		{
			HitBoxUtils.SetSurfaceMaterial(mHostageRagdoll.Bones[i].gameObject, SurfaceMaterial.None, true);
		}
	}

	private void HostageHit(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		HostageDead = true;
	}

	private void HostageTakerHit(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		if (!HostageTakerDead)
		{
			HostageTakerDead = true;
			MakeHostageInvulnerable();
			ActorAttachment componentInChildren = mHostageTaker.GetComponentInChildren<ActorAttachment>();
			if (componentInChildren != null)
			{
				componentInChildren.Drop();
			}
			mHostageTaker.GetComponent<Animation>().Stop();
			mHostageTakerRagdoll.SwitchToRagdoll();
			mSnapTarget.gameObject.SetActive(false);
			ShowHudBlips(false);
			Actor component = args.From.GetComponent<Actor>();
			if (EventHub.Instance != null && component != null)
			{
				Events.EventActor eventActor = new Events.EventActor();
				eventActor.CharacterType = CharacterType.Human;
				eventActor.Faction = FactionHelper.Category.Enemy;
				eventActor.Id = HostageTaker.name;
				eventActor.Name = HostageTaker.Name;
				eventActor.WeaponClass = WeaponDescriptor.WeaponClass.AssaultRifle;
				eventActor.WeaponId = HostageTakerWeapon.Name;
				EventHub.Instance.Report(new Events.Kill(component.EventActor(), eventActor, args.DamageType, args.HeadShot, args.OneShotKill, args.LongShotKill));
			}
		}
	}

	private void PostSpawnHealthEmpty(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		HostageTakerDead = true;
	}

	private Ragdoll SetupHitBox(GameObject model, ActorDescriptor actorDescriptor, HealthComponent health)
	{
		ActorGenerator.InitialiseHitLocation initialiseHitLocation = delegate(HitLocation hitLocation)
		{
			hitLocation.Health = health;
		};
		return ActorGenerator.CreateRagdoll(model.name, actorDescriptor.HitBoxRig, model, initialiseHitLocation);
	}

	private void ShowHudBlips(bool active)
	{
		if (mHostageTakerHudBlip != null)
		{
			mHostageTakerHudBlip.gameObject.SetActive(active);
		}
	}

	private YieldInstruction PlayAnimation(SingleAnimation anim, bool loop)
	{
		return PlayAnimation(anim, loop, 0f, 1f);
	}

	private YieldInstruction PlayAnimation(SingleAnimation anim, bool loop, float normalisedStartingTime, float speed)
	{
		AnimationState state = anim.State;
		state.enabled = true;
		state.weight = 1f;
		state.wrapMode = ((!loop) ? WrapMode.ClampForever : WrapMode.Loop);
		state.speed = speed;
		anim.NormalisedTime = normalisedStartingTime;
		return new WaitForSeconds(anim.RemainingTime);
	}

	private void StopAnimation(SingleAnimation animation)
	{
		animation.State.enabled = false;
	}

	private IEnumerator BlendOutAndRemoveClip(Animation player, AnimationClip clip, string name, float startTime, float blendSpeed)
	{
		AnimationState blendState = player.AddClipSafe(clip, name);
		blendState.enabled = true;
		blendState.layer = 17;
		blendState.weight = 1f;
		blendState.speed = 1f;
		blendState.time = startTime;
		blendState.wrapMode = WrapMode.ClampForever;
		while (blendState != null && blendState.weight > 0f)
		{
			blendState.weight -= blendSpeed * Time.deltaTime;
			yield return null;
		}
		player.RemoveClip(name);
	}

	public YieldInstruction WaitWhile(Func<bool> func)
	{
		return StartCoroutine(ConditionEnumerator(func));
	}

	public IEnumerator ConditionEnumerator(Func<bool> func)
	{
		while (func())
		{
			yield return null;
		}
	}
}
