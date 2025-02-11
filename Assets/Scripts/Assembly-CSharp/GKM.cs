public static class GKM
{
	public static uint ActorsInPlay
	{
		get
		{
			return GlobalKnowledgeManager.Instance().identsInUse;
		}
	}

	public static uint AliveMask
	{
		get
		{
			return GlobalKnowledgeManager.Instance().aliveMask;
		}
	}

	public static uint UpAndAboutMask
	{
		get
		{
			return GlobalKnowledgeManager.Instance().upAndAboutMask;
		}
	}

	public static uint SelectableMask
	{
		get
		{
			return GlobalKnowledgeManager.Instance().selectableMask;
		}
	}

	public static uint AIMask
	{
		get
		{
			return ~GlobalKnowledgeManager.Instance().playerControlledMask & AliveMask;
		}
	}

	public static uint PlayerControlledMask
	{
		get
		{
			return GlobalKnowledgeManager.Instance().playerControlledMask;
		}
	}

	public static uint AlertedMask
	{
		get
		{
			return GlobalKnowledgeManager.Instance().globalAlertness;
		}
	}

	public static uint IdentsInUse
	{
		get
		{
			return GlobalKnowledgeManager.Instance().identsInUse;
		}
	}

	public static int AvailableSpawnSlots()
	{
		return 32 - GlobalKnowledgeManager.Instance().activeListLen;
	}

	public static void SetNewCoverPointManager(NewCoverPointManager ncp)
	{
		GlobalKnowledgeManager.Instance().SetNewCoverPointManager(ncp);
	}

	public static void HideFromFriends(Actor a)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(~GlobalKnowledgeManager.Instance().enemyMask[(int)a.awareness.faction] & AliveMask);
		GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].obstructed = uint.MaxValue;
		uint num = ~a.ident;
		Actor a2;
		while (actorIdentIterator.NextActor(out a2))
		{
			GlobalKnowledgeManager.Instance().inCrowds[a2.quickIndex].obstructed |= a.ident;
			a2.awareness.ICanSee &= num;
			a2.awareness.FriendsICanSee &= num;
		}
	}

	public static int UnitCount(uint mask)
	{
		return GlobalKnowledgeManager.Instance().UnitCount(mask);
	}

	public static GlobalKnowledgeManager.InCrowd InCrowdOf(Actor a)
	{
		return GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex];
	}

	public static uint FriendsMask(Actor a)
	{
		return GlobalKnowledgeManager.Instance().factionMask[(int)a.awareness.faction];
	}

	public static uint FriendsMask(int faction)
	{
		return GlobalKnowledgeManager.Instance().factionMask[faction];
	}

	public static uint EnemiesMask(Actor a)
	{
		return GlobalKnowledgeManager.Instance().enemyMask[(int)a.awareness.faction];
	}

	public static uint EnemiesMask(int faction)
	{
		return GlobalKnowledgeManager.Instance().enemyMask[faction];
	}

	public static uint FactionMask(FactionHelper.Category faction)
	{
		return GlobalKnowledgeManager.Instance().factionMask[(int)faction];
	}

	public static uint CharacterTypeMask(CharacterType type)
	{
		return GlobalKnowledgeManager.Instance().characterTypeMask[(int)type];
	}

	public static uint EverythingFactionCanSee(int faction)
	{
		return GlobalKnowledgeManager.Instance().globalVision[faction];
	}

	public static uint EverythingPlayerCanSee()
	{
		return GlobalKnowledgeManager.Instance().globalVision[0];
	}

	public static uint EverythingAICanSee()
	{
		return GlobalKnowledgeManager.Instance().globalVision[1];
	}

	public static Actor GetActor(uint ident)
	{
		if ((ActorsInPlay & ident) != 0)
		{
			return GlobalKnowledgeManager.Instance().GetActorFromIdent(ident);
		}
		return null;
	}

	public static void OrderedToStand(Actor a)
	{
		GlobalKnowledgeManager.Instance().crouchBecauseOf[a.quickIndex] = 0u;
		a.behaviour.crouchToAvoidBeingSeenUntil = 0f;
	}
}
