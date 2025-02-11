using UnityEngine;

public class HandGestureModule : BaseActorComponent
{
	public enum GestureEnum
	{
		kNone = 0,
		kAdvance = 1,
		kStop = 2,
		kPointToDestination = 4,
		kCount = 5,
		kAll = -1
	}

	private enum ActionEnum
	{
		kBeckonFront = 0,
		kBeckonRight = 1,
		kBeckonBack = 2,
		kBeckonLeft = 3,
		kStop = 4,
		kCount = 5
	}

	public enum ForbidPriority
	{
		CurrentAction = 0,
		Context = 1,
		Count = 2
	}

	private enum RequestStateEnum
	{
		kUndefined = 0,
		kWaitingToPointToDestination = 1
	}

	private enum ActingStateEnum
	{
		kUndefined = 0,
		kBeckoning = 1,
		kStopping = 2,
		kPointing = 3
	}

	private const float kMinAdvanceGestureDistanceSqr = 25f;

	private const float kPointingRedundantDistanceSqr = 9f;

	private GestureEnum currentGesture;

	public Actor preferredFriend;

	private AnimDirector.ActionHandle[] mActionHandle;

	private int gestureHandle;

	private RequestStateEnum mRequestState;

	private ActingStateEnum mActingState;

	private float requestTimer;

	private float actingTimer;

	private Vector3 mAimAtPos;

	private GestureEnum[] mGestureRequestMask = new GestureEnum[2];

	private GestureEnum[] mGestureParticipationMask = new GestureEnum[2];

	private float[] mParticipationDelay = new float[5];

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private GestureEnum GestureRequestMask
	{
		get
		{
			GestureEnum gestureEnum = GestureEnum.kAll;
			for (int i = 0; i < 2; i++)
			{
				gestureEnum &= mGestureRequestMask[i];
			}
			return gestureEnum;
		}
	}

	private GestureEnum GestureParticipationMask
	{
		get
		{
			GestureEnum gestureEnum = GestureEnum.kAll;
			for (int i = 0; i < 2; i++)
			{
				gestureEnum &= mGestureParticipationMask[i];
			}
			return gestureEnum;
		}
	}

	private void Start()
	{
		gestureHandle = 0;
		currentGesture = GestureEnum.kNone;
		mActionHandle = new AnimDirector.ActionHandle[5];
		requestTimer = 0f;
		actingTimer = 0f;
		int categoryHandle = myActor.animDirector.GetCategoryHandle("Gesture");
		mActionHandle[4] = myActor.animDirector.GetActionHandle(categoryHandle, "Stop");
		mActionHandle[0] = myActor.animDirector.GetActionHandle(categoryHandle, "BeckonFront");
		mActionHandle[1] = myActor.animDirector.GetActionHandle(categoryHandle, "BeckonRight");
		mActionHandle[2] = myActor.animDirector.GetActionHandle(categoryHandle, "BeckonBack");
		mActionHandle[3] = myActor.animDirector.GetActionHandle(categoryHandle, "BeckonLeft");
		for (int i = 0; i < 2; i++)
		{
			mGestureRequestMask[i] = GestureEnum.kAll;
			mGestureParticipationMask[i] = GestureEnum.kAll;
		}
	}

	public void EnableGestureRequests(ForbidPriority p, GestureEnum mask)
	{
		mGestureRequestMask[(int)p] |= mask;
	}

	public void DisableGestureRequests(ForbidPriority p, GestureEnum mask)
	{
		mGestureRequestMask[(int)p] &= mask;
	}

	public void SetValidGestureRequests(ForbidPriority p, GestureEnum mask)
	{
		mGestureRequestMask[(int)p] = mask;
	}

	public void EnableGestureParticipation(ForbidPriority p, GestureEnum mask)
	{
		mGestureParticipationMask[(int)p] |= mask;
	}

	public void DisableGestureParticipation(ForbidPriority p, GestureEnum mask)
	{
		mGestureParticipationMask[(int)p] &= mask;
	}

	public void SetValidGestureParticipation(ForbidPriority p, GestureEnum mask)
	{
		mGestureParticipationMask[(int)p] = mask;
	}

	public int RequestGesture(GestureEnum g)
	{
		if ((GestureRequestMask & g) != 0)
		{
			mRequestState = RequestStateEnum.kUndefined;
			currentGesture = g;
			gestureHandle++;
			return gestureHandle;
		}
		return -1;
	}

	public void CancelGesture(int gh)
	{
		if (gestureHandle == gh)
		{
			currentGesture = GestureEnum.kNone;
		}
	}

	private void Update()
	{
		if (!GameController.Instance.IsPaused)
		{
			if (!myActor.OnScreen)
			{
				currentGesture = GestureEnum.kNone;
			}
			switch (currentGesture)
			{
			case GestureEnum.kAdvance:
				RequestUpdate_Advance();
				break;
			case GestureEnum.kStop:
				RequestUpdate_Stop();
				break;
			case GestureEnum.kPointToDestination:
				RequestUpdate_PointToDestination();
				break;
			}
			ActingStateEnum actingStateEnum = mActingState;
			if (actingStateEnum == ActingStateEnum.kPointing)
			{
				ActingUpdate_Pointing();
			}
		}
	}

	private void RequestUpdate_Advance()
	{
		switch (mRequestState)
		{
		case RequestStateEnum.kUndefined:
			RequestUpdate_Advance_Main();
			break;
		case RequestStateEnum.kWaitingToPointToDestination:
			RequestUpdate_Advance_WaitingToPointToDestination();
			break;
		}
	}

	private void RequestUpdate_Advance_Main()
	{
		uint m = GKM.FriendsMask(myActor) & ~GKM.InCrowdOf(myActor).obstructed;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(m);
		float num = 1000000f;
		Actor actor = null;
		if (myActor.navAgent.path == null || myActor.navAgent.path.corners.Length < 2)
		{
			return;
		}
		Vector3 rhs = myActor.navAgent.path.corners[1] - myActor.navAgent.path.corners[0];
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			float sqrMagnitude = (a.GetPosition() - myActor.navAgent.destination).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				continue;
			}
			Vector3 vector = a.GetPosition() - GetPosition();
			if (!(Vector3.Dot(vector, rhs) < 0f))
			{
				float sqrMagnitude2 = vector.sqrMagnitude;
				if (!(sqrMagnitude2 < 25f) && WorldHelper.GetQuadrant(myActor.navAgent.velocity, vector, 1f) == WorldHelper.Quadrant.kFront && (WorldHelper.GetQuadrant(base.transform.forward, vector, 1f) == WorldHelper.Quadrant.kFront || WorldHelper.GetQuadrant(myActor.model.transform.forward, vector, 1f) == WorldHelper.Quadrant.kFront))
				{
					num = sqrMagnitude;
					actor = a;
				}
			}
		}
		if (!(actor != null))
		{
			return;
		}
		HandGestureModule gestures = actor.gestures;
		if (gestures != null && gestures.Participate(this, GestureEnum.kAdvance))
		{
			if ((actor.GetPosition() - myActor.navAgent.destination).sqrMagnitude > 9f)
			{
				preferredFriend = actor;
				mRequestState = RequestStateEnum.kWaitingToPointToDestination;
				requestTimer = 0.65f;
			}
			else
			{
				currentGesture = GestureEnum.kNone;
			}
		}
	}

	private void RequestUpdate_Advance_WaitingToPointToDestination()
	{
		if (requestTimer > 0f)
		{
			requestTimer -= Time.deltaTime;
			return;
		}
		currentGesture = GestureEnum.kPointToDestination;
		mRequestState = RequestStateEnum.kUndefined;
	}

	private void RequestUpdate_Stop()
	{
		uint m = GKM.FriendsMask(myActor) & GKM.AliveMask & ~GKM.InCrowdOf(myActor).obstructed;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(m);
		float num = 1000000f;
		Actor actor = null;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.awareness.ChDefCharacterType != CharacterType.Human)
			{
				continue;
			}
			Vector3 to = myActor.navAgent.destination - a.GetPosition();
			float sqrMagnitude = to.sqrMagnitude;
			if (!(sqrMagnitude > num) && WorldHelper.GetQuadrant(a.realCharacter.GetStanceDirection(), to, 1f) != 0)
			{
				Vector3 to2 = a.GetPosition() - GetPosition();
				if (WorldHelper.GetQuadrant(base.transform.forward, to2, 1f) == WorldHelper.Quadrant.kFront || WorldHelper.GetQuadrant(myActor.realCharacter.GetStanceDirection(), to2, 1f) == WorldHelper.Quadrant.kFront)
				{
					num = sqrMagnitude;
					actor = a;
				}
			}
		}
		if (actor != null)
		{
			HandGestureModule gestures = actor.gestures;
			if (gestures != null && gestures.Participate(this, GestureEnum.kStop))
			{
				currentGesture = GestureEnum.kNone;
			}
		}
	}

	private void RequestUpdate_PointToDestination()
	{
		uint m = GKM.FriendsMask(myActor) & ~GKM.InCrowdOf(myActor).obstructed;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(m);
		float num = 1000000f;
		Actor actor = null;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.awareness.ChDefCharacterType != CharacterType.Human)
			{
				continue;
			}
			Vector3 to = myActor.navAgent.destination - a.GetPosition();
			float num2 = to.sqrMagnitude;
			Vector3 to2 = a.GetPosition() - GetPosition();
			if ((WorldHelper.GetQuadrant(base.transform.forward, to2, 1f) == WorldHelper.Quadrant.kFront || WorldHelper.GetQuadrant(myActor.realCharacter.GetStanceDirection(), to2, 1f) == WorldHelper.Quadrant.kFront) && (WorldHelper.InFront(a.realCharacter.GetStanceDirection(), to) || WorldHelper.InFront(a.transform.forward, to)))
			{
				if (preferredFriend == a)
				{
					num2 = 0f;
				}
				if (!(num2 > num))
				{
					num = num2;
					actor = a;
				}
			}
		}
		if (actor != null)
		{
			HandGestureModule gestures = actor.gestures;
			if (gestures != null && gestures.Participate(this, GestureEnum.kPointToDestination))
			{
				currentGesture = GestureEnum.kNone;
			}
		}
	}

	private bool Participate(HandGestureModule friend, GestureEnum g)
	{
		if (myActor.weapon != null && myActor.weapon.DesiredFiringBehaviour() > SoldierFiringState.FireType.Hold)
		{
			return false;
		}
		if (Time.time < mParticipationDelay[(int)g])
		{
			return false;
		}
		if ((GestureParticipationMask & g) != 0)
		{
			switch (g)
			{
			case GestureEnum.kAdvance:
				return Participate_Advance(friend);
			case GestureEnum.kStop:
				return Participate_Stop(friend);
			case GestureEnum.kPointToDestination:
				return Participate_PointToDestination(friend);
			}
		}
		return false;
	}

	private void Delay(GestureEnum g, float min, float max)
	{
		float num = Time.time + Random.Range(min, max);
		if (num > mParticipationDelay[(int)g])
		{
			mParticipationDelay[(int)g] = num;
		}
	}

	private bool Participate_Advance(HandGestureModule friend)
	{
		myActor.Command("CancelIdle");
		myActor.animDirector.PlayAction(mActionHandle[(int)(0 + WorldHelper.GetQuadrant(myActor.realCharacter.GetStanceDirection(), friend.GetPosition() - base.transform.position, 1f))], true);
		Delay(GestureEnum.kAdvance, 3f, 5f);
		return true;
	}

	private bool Participate_Stop(HandGestureModule friend)
	{
		myActor.Command("CancelIdle");
		myActor.animDirector.PlayAction(mActionHandle[4], true);
		Delay(GestureEnum.kStop, 3f, 5f);
		Delay(GestureEnum.kAdvance, 3f, 5f);
		return true;
	}

	private bool Participate_PointToDestination(HandGestureModule friend)
	{
		myActor.Command("CancelIdle");
		myActor.Command("Point");
		mActingState = ActingStateEnum.kPointing;
		mAimAtPos = friend.myActor.navAgent.destination;
		actingTimer = 0.8f;
		Delay(GestureEnum.kPointToDestination, 3f, 5f);
		Delay(GestureEnum.kStop, 2f, 4f);
		Delay(GestureEnum.kAdvance, 3f, 5f);
		return true;
	}

	private void ActingUpdate_Pointing()
	{
		actingTimer -= Time.deltaTime;
		if (actingTimer <= 0f)
		{
			mActingState = ActingStateEnum.kUndefined;
			myActor.Command("Aim");
		}
	}

	private Vector3 GetPosition()
	{
		return myActor.GetPosition();
	}

	public bool GetAimAt(ref Vector3 pos)
	{
		if (mActingState == ActingStateEnum.kPointing)
		{
			pos = mAimAtPos;
			return true;
		}
		return false;
	}
}
