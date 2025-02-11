using UnityEngine;

[RequireComponent(typeof(RealCharacter))]
public class CharacterAnimationController : FSM
{
	private class State
	{
		public const int Idle = 1;

		public const int WalkRunCycle = 2;

		public const int Dead = 3;

		public const int Firing = 4;

		public const int ExitFiring = 5;

		public const int IdleCrouch = 6;
	}

	public Animation PlayerAnimation;

	public AnimationLoader AnimationLoader;

	private BaseCharacter Character;

	private bool mPartialsSetUp;

	private string idleName;

	private string idleCrouchName;

	private Vector3 mEntryLocalPosition;

	private string mRunName;

	private string mFiringEntryName;

	private string mFiringName;

	private bool mHasEntered;

	private string mExitFiringName;

	public override void Awake()
	{
		Character = base.gameObject.GetComponent<RealCharacter>();
		base.Awake();
	}

	public void AudioEvent_LeftFoot()
	{
		if (CurrentState() != 2)
		{
		}
	}

	public void AudioEvent_RightFoot()
	{
		if (CurrentState() != 2)
		{
		}
	}

	private void Start()
	{
		mPartialsSetUp = false;
		StopAllAnims();
		mStateTable.Add(1, new StateFunctions(IdleAnimation_Enter, IdleAnimation_Update, IdleAnimation_Exit, "Idle"));
		mStateTable.Add(2, new StateFunctions(WalkRunAnimation_Enter, WalkRunAnimation_Update, WalkRunAnimation_Exit, "Walkrun"));
		mStateTable.Add(3, new StateFunctions(DeadAnimation_Enter, DeadAnimation_Update, DeadAnimation_Exit, "Dead"));
		mStateTable.Add(4, new StateFunctions(FiringAnimation_Enter, FiringAnimation_Update, FiringAnimation_Exit, "Firing"));
		mStateTable.Add(5, new StateFunctions(ExitFiringAnimation_Enter, ExitFiringAnimation_Update, ExitFiringAnimation_Exit, "ExitFiring"));
		mStateTable.Add(6, new StateFunctions(IdleCrouchAnimation_Enter, IdleCrouchAnimation_Update, IdleCrouchAnimation_Exit, "IdleCrouch"));
		SwitchState(1);
	}

	private void StopAllAnims()
	{
		PlayerAnimation.Stop();
	}

	private void SetupPartialAnimations()
	{
		if (!mPartialsSetUp)
		{
			mPartialsSetUp = true;
		}
	}

	protected override string FSMName()
	{
		return "CharacterAnimationController";
	}

	public override void Update()
	{
		SetupPartialAnimations();
		base.Update();
	}

	public void IdleAnimation_Enter(int fromState)
	{
		idleName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Idle);
		PlayerAnimation.CrossFade(idleName);
		PlayerAnimation.SyncLayer(0);
	}

	public void IdleAnimation_Update()
	{
		if (Character.IsDead())
		{
			SwitchState(3);
		}
		else if (Character.myActor.weapon.IsFiring())
		{
			SwitchState(4);
		}
		else if (Character.IsMoving())
		{
			SwitchState(2);
		}
		else if (Character.GetStance() == BaseCharacter.Stance.Crouched)
		{
			SwitchState(6);
		}
	}

	public void IdleAnimation_Exit(int toState)
	{
	}

	public void IdleCrouchAnimation_Enter(int fromState)
	{
		idleCrouchName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.IdleCrouch);
		PlayerAnimation.CrossFade(idleCrouchName);
		PlayerAnimation.SyncLayer(0);
	}

	public void IdleCrouchAnimation_Update()
	{
		if (Character.IsDead())
		{
			SwitchState(3);
		}
		else if (Character.IsMoving())
		{
			SwitchState(2);
		}
		else if (Character.GetStance() == BaseCharacter.Stance.Standing)
		{
			SwitchState(1);
		}
	}

	public void IdleCrouchAnimation_Exit(int toState)
	{
	}

	private bool AllAnimationsFinished()
	{
		foreach (AnimationState item in PlayerAnimation)
		{
			if (item.enabled && item.time < item.length)
			{
				return false;
			}
		}
		return true;
	}

	public void DeadAnimation_Enter(int fromState)
	{
		mEntryLocalPosition = base.transform.localPosition;
		AnimationGroup.AnimType type = AnimationGroup.AnimType.Dead;
		float fadeLength = 0.1f;
		string animName = AnimationLoader.GetAnimName(type);
		PlayerAnimation.CrossFade(animName, fadeLength);
	}

	public void DeadAnimation_Update()
	{
	}

	public void DeadAnimation_Exit(int toState)
	{
		StopAllAnims();
		if (toState == 1)
		{
		}
		base.transform.localPosition = mEntryLocalPosition;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
	}

	public void WalkRunAnimation_Enter(int fromState)
	{
		mRunName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Run);
		AnimationState animationState = PlayerAnimation[mRunName];
		animationState.time = 0.14f;
		if (fromState == 1)
		{
			PlayerAnimation.Play(mRunName);
		}
	}

	private void UpdateLeaning_Normal(float runWeight)
	{
		PlayerAnimation[mRunName].weight = runWeight;
		PlayerAnimation[mRunName].enabled = true;
		PlayerAnimation.CrossFade(mRunName);
	}

	public void WalkRunAnimation_Update()
	{
		float runWeight = 1f;
		UpdateLeaning_Normal(runWeight);
		if (Character.IsDead())
		{
			SwitchState(3);
		}
		else if (Character.myActor.weapon.IsFiring())
		{
			SwitchState(4);
		}
		else if (!Character.IsMoving())
		{
			if (Character.GetStance() == BaseCharacter.Stance.Crouched)
			{
				SwitchState(6);
			}
			else
			{
				SwitchState(1);
			}
		}
	}

	public void WalkRunAnimation_Exit(int toState)
	{
	}

	public void FiringAnimation_Enter(int fromState)
	{
		mHasEntered = false;
		mFiringEntryName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.IdleToFiring);
		mFiringName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Firing);
		PlayerAnimation.CrossFade(mFiringEntryName);
		PlayerAnimation.SyncLayer(0);
	}

	public void FiringAnimation_Update()
	{
		if (Character.IsDead())
		{
			SwitchState(3);
		}
		else if (Character.IsMoving())
		{
			SwitchState(2);
		}
		else if (!mHasEntered && PlayerAnimation[mFiringEntryName].weight == 1f)
		{
			mHasEntered = true;
			PlayerAnimation.Play(mFiringName);
		}
		else if (mHasEntered && !Character.myActor.weapon.IsFiring())
		{
			SwitchState(5);
		}
	}

	public void FiringAnimation_Exit(int toState)
	{
	}

	public void ExitFiringAnimation_Enter(int fromState)
	{
		mExitFiringName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.FiringToIdle);
		PlayerAnimation.CrossFade(mExitFiringName);
		PlayerAnimation.SyncLayer(0);
	}

	public void ExitFiringAnimation_Update()
	{
		if (Character.IsDead())
		{
			SwitchState(3);
		}
		else if (Character.IsMoving())
		{
			SwitchState(2);
		}
		else if (!PlayerAnimation.isPlaying)
		{
			if (Character.GetStance() == BaseCharacter.Stance.Crouched)
			{
				SwitchState(6);
			}
			else
			{
				SwitchState(1);
			}
		}
	}

	public void ExitFiringAnimation_Exit(int toState)
	{
	}
}
