using System;
using UnityEngine;

public class StayAliveOnMiniGunObjective : MissionObjective
{
	public KeepAliveObjectiveData m_aliveInterface;

	[HideInInspector]
	public Actor Actor;

	private HealthComponent Target;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public override void Start()
	{
		base.Start();
		mMissionPassIfNotFail = true;
	}

	public void Update()
	{
		if (!(Actor == null))
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.baseCharacter.IsUsingFixedGun)
			{
				Actor = a;
				Setup();
				break;
			}
		}
	}

	private void Setup()
	{
		if (Target == null)
		{
			Target = Actor.gameObject.GetComponent<HealthComponent>();
			Target.OnHealthEmpty += OnHealthEmpty;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (Target != null)
		{
			Target.OnHealthEmpty -= OnHealthEmpty;
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		Fail();
	}

	public override void SetupFromSetPieceOverride(GameObject[] Objects)
	{
		if (Target == null)
		{
			Target = Objects[0].GetComponent<HealthComponent>();
			Target.OnHealthEmpty += OnHealthEmpty;
		}
		if (base.State == ObjectiveState.Dormant)
		{
			EnableObjective();
		}
	}
}
