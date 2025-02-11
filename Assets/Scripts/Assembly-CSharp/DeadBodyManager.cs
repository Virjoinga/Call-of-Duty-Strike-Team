using System.Collections.Generic;
using UnityEngine;

public class DeadBodyManager : MonoBehaviour
{
	private const float kSqRadiusForBodyMovement = 25f;

	public static DeadBodyManager instance;

	private List<DeadBodyWrapper> bodiesToRemove = new List<DeadBodyWrapper>();

	private List<DeadBodyWrapper> mBodies;

	public static DeadBodyManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mBodies = new List<DeadBodyWrapper>();
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Update()
	{
		bodiesToRemove.Clear();
		for (int i = 0; i < mBodies.Count; i++)
		{
			if (mBodies[i] == null || mBodies[i].ActorWithin == null)
			{
				bodiesToRemove.Add(mBodies[i]);
				continue;
			}
			float sqrMagnitude = (mBodies[i].SpottedPosition - mBodies[i].ActorWithin.GetPosition()).sqrMagnitude;
			if (sqrMagnitude > 25f)
			{
				bodiesToRemove.Add(mBodies[i]);
			}
		}
		for (int j = 0; j < bodiesToRemove.Count; j++)
		{
			mBodies.Remove(bodiesToRemove[j]);
		}
	}

	public void AddBody(Actor deadActor, Actor spottingActor)
	{
		if (!(deadActor != null) || !(spottingActor != null))
		{
			return;
		}
		foreach (DeadBodyWrapper mBody in mBodies)
		{
			if (mBody.ActorWithin == deadActor)
			{
				mBody.AlsoSeenBy(spottingActor);
				return;
			}
		}
		mBodies.Add(new DeadBodyWrapper(deadActor, spottingActor));
	}

	private void RemoveBody(DeadBodyWrapper deadBody)
	{
		mBodies.Remove(deadBody);
	}

	public bool IsAwareOf(Actor deadActor, Actor alertedActor)
	{
		foreach (DeadBodyWrapper mBody in mBodies)
		{
			if (mBody.ActorWithin == deadActor)
			{
				return mBody.IsAlreadySeenBy(alertedActor);
			}
		}
		return false;
	}
}
