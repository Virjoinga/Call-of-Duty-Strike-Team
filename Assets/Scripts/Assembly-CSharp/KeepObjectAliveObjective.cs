using System;
using UnityEngine;

public class KeepObjectAliveObjective : MissionObjective
{
	public KeepAliveObjectiveData m_aliveInterface;

	[HideInInspector]
	public ActorWrapper Actor;

	private HealthComponent Target;

	public override void Start()
	{
		base.Start();
		mMissionPassIfNotFail = true;
		if (Actor != null)
		{
			Actor.TargetActorChanged += Setup;
			base.gameObject.transform.position = Actor.gameObject.transform.position;
			UpdateBlipTarget();
		}
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (gameObject != null && Target == null)
		{
			Target = gameObject.GetComponent<HealthComponent>();
			Target.OnHealthEmpty += OnHealthEmpty;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (Actor != null)
		{
			Actor.TargetActorChanged -= Setup;
		}
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
