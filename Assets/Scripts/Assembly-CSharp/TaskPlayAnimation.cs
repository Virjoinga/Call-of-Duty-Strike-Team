using System;
using UnityEngine;

public class TaskPlayAnimation : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Invalidated = 1,
		ExitStarted = 2,
		PartOfSequence = 4,
		ExternallyTerminated = 8
	}

	private enum State
	{
		Start = 0,
		PlayingAnimation = 1,
		AnimationFinished = 2
	}

	private Flags mFlags;

	private State mState;

	private AnimDirector.ActionHandle mActionClipHandle;

	private float mPlaybackDuration;

	private string mCategory;

	private string mActionName;

	private AnimationClip mClip;

	private AnimationClip mExit;

	private bool mLooping;

	private float mBlendTime;

	private float mSpeed;

	private RawAnimation newRawAnim;

	private Transform mLocator;

	public bool PartOfSequence
	{
		set
		{
			if (value)
			{
				mFlags |= Flags.PartOfSequence;
				if (!mLooping && !(mPlaybackDuration > 0f))
				{
					mPlaybackDuration = mClip.length;
					mLooping = true;
				}
			}
			else
			{
				mFlags &= ~Flags.PartOfSequence;
			}
		}
	}

	public Transform Locator
	{
		set
		{
			mLocator = value;
		}
	}

	public TaskPlayAnimation(TaskManager owner, TaskManager.Priority priority, Config flags, AnimationClip clip, AnimationClip exit, string category, string actionName, bool looping, int numberOfLoops, int randomLoopsPlusMinus, float blendTime, float speed)
		: base(owner, priority, flags)
	{
		mFlags = Flags.Default;
		mState = State.Start;
		TBFAssert.DoAssert(mActor.animDirector != null, string.Format("RealCharacter {0} has no AnimDirector", mActor.realCharacter.name));
		mCategory = category;
		mActionName = actionName;
		mClip = clip;
		mExit = exit;
		mLooping = looping;
		mBlendTime = blendTime;
		mSpeed = speed;
		TBFAssert.DoAssert(clip != null, string.Format("TaskPlayAnimation called with null clip for {0}", mActor.realCharacter.name));
		if (numberOfLoops > 1)
		{
			int num = numberOfLoops - randomLoopsPlusMinus + UnityEngine.Random.Range(0, randomLoopsPlusMinus * 2);
			num = Mathf.Clamp(num, 2, num);
			mPlaybackDuration = (float)num * clip.length;
			mLooping = true;
		}
		else
		{
			mPlaybackDuration = 0f;
		}
	}

	public override void Update()
	{
		if (mLocator != null)
		{
			mActor.realCharacter.TurnToFaceDirection(mLocator.forward);
			if ((mActor.GetPosition() - mLocator.transform.position).sqrMagnitude > 1f)
			{
				mFlags |= Flags.Invalidated;
				return;
			}
		}
		switch (mState)
		{
		case State.Start:
			StartAnimation(mClip);
			mState = State.PlayingAnimation;
			break;
		case State.PlayingAnimation:
			if (mPlaybackDuration > 0f)
			{
				mPlaybackDuration -= Time.deltaTime;
				if (mPlaybackDuration <= 0f)
				{
					if ((mFlags & Flags.PartOfSequence) == 0)
					{
						mActor.animDirector.StopAnim(newRawAnim);
					}
					mState = State.AnimationFinished;
				}
			}
			else if (HasAnimationFinished())
			{
				mState = State.AnimationFinished;
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		return IsSequenceComplete();
	}

	public override void Finish()
	{
		if ((mState != State.AnimationFinished || (mFlags & Flags.ExternallyTerminated) != 0) && mActor.animDirector != null)
		{
			mActor.animDirector.StopAnim(newRawAnim, 0.2f);
		}
	}

	public override void OnSleep()
	{
		if (mActor.animDirector.AnimationPlayer != null)
		{
			mActor.animDirector.AnimationPlayer.Stop(mClip.name);
		}
		mFlags |= Flags.Invalidated;
	}

	private void StartAnimation(AnimationClip clip)
	{
		newRawAnim = new RawAnimation();
		newRawAnim.AnimClip = clip;
		newRawAnim.Looping = mLooping;
		newRawAnim.Clamp = false;
		newRawAnim.BlendTime = mBlendTime;
		newRawAnim.Speed = mSpeed;
		newRawAnim.PreventAiming = true;
		newRawAnim.PreventReloading = true;
		if (mBlendTime > 0f)
		{
			newRawAnim.BlendType = RawAnimation.AnimBlendType.kLinearCrossFade;
		}
		else
		{
			newRawAnim.BlendType = RawAnimation.AnimBlendType.kSnapTo;
		}
		mActionClipHandle = mActor.animDirector.GetActionHandle(mCategory, mActionName);
		if (mActor.animDirector.AddCategoryClip(newRawAnim, mActionClipHandle.CategoryID))
		{
			mActor.animDirector.ForcePlayAnimation(newRawAnim, mActionClipHandle.CategoryID, mActionClipHandle.ActionID);
			return;
		}
		mState = State.AnimationFinished;
		mFlags = Flags.Invalidated;
	}

	private bool HasAnimationFinished()
	{
		if (mState == State.Start)
		{
			return false;
		}
		return mActor.animDirector.HasCurrentActionCompleted(mActionClipHandle.CategoryID);
	}

	private bool IsSequenceComplete()
	{
		if ((mFlags & Flags.ExitStarted) != 0 && mState == State.PlayingAnimation)
		{
			return false;
		}
		bool flag = false;
		if (mExit != null && (mFlags & Flags.ExitStarted) == 0)
		{
			flag = true;
		}
		bool flag2 = CheckConfigFlagsFinished();
		if ((base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.alertState == BehaviourController.AlertState.Reacting)
		{
			flag2 = true;
		}
		if (flag2)
		{
			mFlags |= Flags.ExternallyTerminated;
		}
		bool flag3 = mState == State.AnimationFinished || flag2;
		if (flag3 && flag)
		{
			mLooping = false;
			StartAnimation(mExit);
			mFlags |= Flags.ExitStarted;
			mState = State.PlayingAnimation;
			return false;
		}
		return flag3;
	}
}
