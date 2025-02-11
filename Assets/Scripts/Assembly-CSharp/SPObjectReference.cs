using System;
using UnityEngine;

[Serializable]
public class SPObjectReference
{
	public enum SPObjectType
	{
		Invalid = 0,
		PlayerCharacter = 1,
		Enemy = 2,
		Node = 3,
		WorldObject = 4,
		NumObjectTypes = 5
	}

	private enum State
	{
		Free = 0,
		Busy = 1,
		Finished = 2
	}

	public string Name;

	public SPObjectType Type;

	public int Index;

	public Vector3 Position;

	public Transform ObjectTransform;

	public Transform linkTransform;

	public Animation ActorAnimation;

	public AnimationClip CurrentAnim;

	public AnimDirector AnimDirector;

	public Actor ActorRef;

	public float AnimationStartTime;

	public bool GunAway;

	public bool GunOut = true;

	private Vector3 playgroundStartPos;

	private Quaternion playgroundStartRot;

	private State mState;

	public RealCharacter RealCharacter
	{
		get
		{
			return ActorRef.realCharacter;
		}
	}

	public SPObjectReference(SPObjectType type)
	{
		Type = type;
		Position.Set(0f, 0f, 0f);
		mState = State.Free;
		AnimationStartTime = -1f;
		AnimDirector = null;
		ActorRef = null;
	}

	public void PlaygroundSnapshot()
	{
		if (ObjectTransform != null)
		{
			playgroundStartPos = ObjectTransform.position;
			playgroundStartRot = ObjectTransform.rotation;
		}
	}

	public void PlaygroundRestore()
	{
		if (ObjectTransform != null)
		{
			ObjectTransform.position = playgroundStartPos;
			ObjectTransform.rotation = playgroundStartRot;
		}
	}

	public bool HasFinished()
	{
		return mState == State.Finished;
	}

	public void SetAnimDirector(AnimDirector director)
	{
		AnimDirector = director;
	}

	public void SetType(SPObjectType type, string name)
	{
		Type = type;
		Name = name;
		Position.Set(0f, 0f, 0f);
	}

	public void SetIndex(int index, string name)
	{
		Name = name;
		Index = index;
	}

	public void SetActorAnimation(Animation animation)
	{
		ActorAnimation = animation;
	}

	public Vector3 GetPosition()
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		switch (Type)
		{
		case SPObjectType.Node:
			return Position;
		default:
			return result;
		}
	}

	public void PlayAnimation(AnimationClip clip, float speed, bool looped, float blendTime, RawAnimation.AnimBlendType easing)
	{
		if (AnimDirector != null)
		{
			if (clip != null)
			{
				RawAnimation rawAnimation = new RawAnimation();
				rawAnimation.AnimClip = clip;
				rawAnimation.Looping = looped;
				rawAnimation.Clamp = true;
				rawAnimation.BlendTime = blendTime;
				rawAnimation.Speed = speed;
				rawAnimation.PreventAiming = true;
				rawAnimation.PreventReloading = true;
				if (blendTime > 0f)
				{
					rawAnimation.BlendType = easing;
				}
				else
				{
					rawAnimation.BlendType = RawAnimation.AnimBlendType.kSnapTo;
				}
				int categoryHandle = AnimDirector.GetCategoryHandle("SetPiece");
				AnimDirector.AddCategoryClip(rawAnimation, categoryHandle);
				AnimDirector.ForcePlayAnimation(rawAnimation, AnimDirector.GetCategoryHandle("SetPiece"), Index);
				mState = State.Busy;
			}
		}
		else if (ActorAnimation != null && clip != null)
		{
			if (ActorAnimation.GetClip(clip.name) == null)
			{
				ActorAnimation.AddClip(clip, clip.name);
			}
			ActorAnimation[clip.name].speed = speed;
			ActorAnimation[clip.name].layer = 0;
			ActorAnimation[clip.name].time = 0f;
			if (looped)
			{
				ActorAnimation[clip.name].wrapMode = WrapMode.Loop;
			}
			else
			{
				ActorAnimation[clip.name].wrapMode = WrapMode.ClampForever;
			}
			if (blendTime > 0f)
			{
				ActorAnimation.CrossFade(clip.name, blendTime);
			}
			else
			{
				ActorAnimation.Play(clip.name);
			}
			mState = State.Busy;
		}
		CurrentAnim = clip;
		AnimationStartTime = Time.time;
	}

	public void AddClip(AnimationClip clip)
	{
		if (ActorAnimation != null)
		{
			ActorAnimation.AddClip(clip, clip.name);
		}
	}

	public void SkipCurrentAnim()
	{
		if (!(CurrentAnim != null))
		{
			return;
		}
		if (AnimDirector != null)
		{
			AnimDirector.EnableCategory(AnimDirector.GetCategoryHandle("SetPiece"), false, 0f);
		}
		else
		{
			if (!(ActorAnimation != null))
			{
				return;
			}
			AnimationClip clip = ActorAnimation.GetClip(CurrentAnim.name);
			if (clip != null)
			{
				AnimationState animationState = ActorAnimation[clip.name];
				if (animationState != null)
				{
					animationState.time = clip.length;
				}
			}
		}
	}

	public void SetFinished(bool OnOff, bool Continue, bool SetTetherPoint)
	{
		mState = State.Finished;
		if (ActorRef != null)
		{
			if (!Continue)
			{
				ActorRef.realCharacter.UseLaserSite(true);
			}
			if (OnOff)
			{
				ActorRef.baseCharacter.EnableNavMesh(OnOff);
			}
			if (SetTetherPoint)
			{
				ActorRef.tether.TetherToSelf();
			}
		}
	}

	public bool IsReal()
	{
		if (ActorRef != null && ActorRef.realCharacter != null)
		{
			return true;
		}
		return false;
	}
}
