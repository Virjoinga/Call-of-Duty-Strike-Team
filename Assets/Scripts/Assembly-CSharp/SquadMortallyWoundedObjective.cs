using System.Collections.Generic;
using UnityEngine;

public class SquadMortallyWoundedObjective : MissionObjective
{
	public SquadMortallyWoundedData mDataInterface;

	[HideInInspector]
	public List<ActorWrapper> Actors = new List<ActorWrapper>();

	private Actor[] mUnitsToCheck;

	public override void Start()
	{
		base.Start();
		mMissionPassIfNotFail = true;
		mUnitsToCheck = new Actor[Actors.Count];
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged += Setup;
		}
		GameplayController.Instance().OnPlayerCharacterAboutToBeMortallyWounded += OnPlayerCharacterAboutToBeMortallyWounded;
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		for (int i = 0; i < mUnitsToCheck.Length; i++)
		{
			if (mUnitsToCheck[i] == null)
			{
				mUnitsToCheck[i] = gameObject.GetComponent<Actor>();
				break;
			}
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged -= Setup;
		}
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnPlayerCharacterAboutToBeMortallyWounded -= OnPlayerCharacterAboutToBeMortallyWounded;
		}
	}

	private void OnPlayerCharacterAboutToBeMortallyWounded(Actor actor)
	{
		if (base.State != 0)
		{
			return;
		}
		Actor[] array = mUnitsToCheck;
		foreach (Actor actor2 in array)
		{
			if (actor2 == actor)
			{
				actor.InitiatingGameFail = true;
				FrontEndController.Instance.TransitionTo(ScreenID.ContinueScreen);
				break;
			}
		}
	}

	public void ForceFail()
	{
		Fail();
	}
}
