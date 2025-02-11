using UnityEngine;

public class CorpseModule : BasePoseModule
{
	private AnimDirector.ActionHandle mIdleAction;

	private int deadOverride;

	private float deathDoneCounter;

	private Quaternion forceRot;

	private bool mWasMeleeDeath;

	protected override void Internal_Init()
	{
		mIdleAction = base.mAnimDirector.GetActionHandle(base.mAnimDirector.GetCategoryHandle("Movement"), "Stand");
		deadOverride = base.mAnimDirector.GetOverrideHandle("Dead");
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		if (deathDoneCounter > 0f)
		{
			deathDoneCounter -= Time.deltaTime;
			if (deathDoneCounter <= 0f)
			{
				if (mWasMeleeDeath)
				{
					BodyFallSFX.Instance.BodyFallStealth.Play(myActor.gameObject);
				}
				else
				{
					SoundManager.Instance.PlayBodyFallSfx(myActor.gameObject.transform.position, myActor.gameObject);
				}
				base.mAnimDirector.EnableOverride(deadOverride, true);
				base.mAnimDirector.PlayAction(mIdleAction);
				myActor.Pose.CancelSegue();
				if (myActor.awareness.ChDefCharacterType != CharacterType.RiotShieldNPC && myActor.awareness.ChDefCharacterType != CharacterType.RPG)
				{
					base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Crouch"), false);
					base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Turning"), false, 0f);
					base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Aiming"), false, 0f);
					base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Shooting"), false, 0f);
					base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Gesture"), false, 0f);
					base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Idles"), false, 0f);
				}
			}
		}
		return PoseModuleSharedData.Modules.Corpse;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.ShotFront:
		{
			BaseCharacter component2 = GetComponent<BaseCharacter>();
			component2.transform.forward = base.Model.transform.forward;
			base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Movement"), true, 0f);
			deathDoneCounter = 0.2f;
			break;
		}
		case PoseModuleSharedData.CommandCode.Melee:
		{
			base.mAnimDirector.EnableOverride(deadOverride, true);
			base.mAnimDirector.PlayAction(mIdleAction);
			base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Movement"), true, 0.5f);
			deathDoneCounter = 0.2f;
			mWasMeleeDeath = true;
			EventOnIveBeenStealthKilled componentInChildren = base.gameObject.GetComponentInChildren<EventOnIveBeenStealthKilled>();
			if (componentInChildren != null)
			{
				componentInChildren.OnDeathsColdEmbrace();
			}
			break;
		}
		case PoseModuleSharedData.CommandCode.Ragdoll:
		{
			bool flag = OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Ragdoll);
			BaseCharacter component = GetComponent<BaseCharacter>();
			if (component != null && component.Ragdoll != null && component.Ragdoll.Kinematic && component.myActor.OnScreen && flag)
			{
				myActor.animDirector.enabled = false;
				myActor.animDirector.AnimationPlayer.enabled = false;
				component.Ragdoll.SwitchToRagdoll();
			}
			myActor.animDirector.AnimationPlayer.animatePhysics = false;
			break;
		}
		}
		return PoseModuleSharedData.Modules.Corpse;
	}

	public override void OnInactive(PoseModuleSharedData.Modules to)
	{
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Deaths"), false, 0.1f);
		base.mAnimDirector.SetCategoryWeight(base.mAnimDirector.GetCategoryHandle("Deaths"), 0f);
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		deathDoneCounter = 0f;
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Deaths"), true, 0.1f);
	}
}
