using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FakeActorData
{
	public ActorDescriptor Actor;

	public List<AnimationClip> Animations;

	public bool NoWeapon;

	public void CopyContainerData(FakeActor f)
	{
		f.Actor = Actor;
		if (Animations != null)
		{
			f.Animations = Animations.ToArray();
		}
		f.NoWeapon = NoWeapon;
	}
}
