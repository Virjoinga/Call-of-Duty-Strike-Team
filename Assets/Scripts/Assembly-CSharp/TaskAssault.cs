using UnityEngine;

public class TaskAssault : Task
{
	private enum State
	{
		Start = 0,
		InitialMove = 1,
		InitialReactionToEnemy = 2,
		ScoreNeighbours = 3,
		ChooseAction = 4,
		CheckTwoStageRoutes = 5,
		Execute = 6,
		MovingToNewCover = 7
	}

	private const float kInCombatDuration = 3f;

	private const float routeScoreDistanceBase = 1f;

	private int debugThinkStep;

	public AssaultParams mAssaultParams;

	private InheritableMovementParams mMoveParams;

	private State state;

	private CoverPointCore Hub;

	private CoverPointCore Goal;

	private float GoalDistance;

	private ActorMask enemiesIConsidered = new ActorMask(0u, "EnemiesIConsidered");

	public int[] neighbours;

	public int[] doorMasks;

	public float[] GoalScore;

	public float[] OffenceScore;

	public float[] RouteScore;

	public bool[] MustCrouch;

	public float[] DefenceScore;

	public float[] TotalScore;

	public float[] MidPointScore;

	private int roundRobin;

	private bool allScored;

	private float BestScore;

	public int bestNeighbour;

	private float inCombatUntil;

	private float holdCoverUntil;

	private float originalCoverScore;

	private CoverPointCore panicCover;

	private Vector3 cachedTargetPosition;

	private bool alternateChecking;

	private float headToMagnetTime;

	private OptimisationInstanceHandle optHandle = new OptimisationInstanceHandle();

	public bool DebugThinkStep
	{
		get
		{
			if (debugThinkStep > 0)
			{
				debugThinkStep--;
				return true;
			}
			return true;
		}
	}

	public TaskAssault(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams, AssaultParams parameters)
		: base(owner, priority, flags)
	{
		if (mActor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			Debug.LogError(string.Format("Unsupported Character Type for TaskAssault - {0}", mActor.name));
		}
		mMoveParams = moveParams;
		mAssaultParams = parameters;
		state = State.Start;
		optHandle.SafeRegister(OptType.AssaultTask);
	}

	public void AddThinkSteps(int val)
	{
		debugThinkStep += val;
	}

	public override void Update()
	{
		switch (state)
		{
		case State.Start:
			if (mActor.navAgent.enabled)
			{
				if (mAssaultParams.Target.theObject != null && mActor.awareness.EnemiesICanSee == 0)
				{
					cachedTargetPosition = mAssaultParams.Target.theObject.transform.position;
					mMoveParams.mDestination = GeneralArea.GetLocation(mAssaultParams.Target.theObject);
					new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType | Config.ConsultParent, mMoveParams);
					holdCoverUntil = 0f;
					state = State.InitialMove;
				}
				else
				{
					state = State.InitialReactionToEnemy;
				}
			}
			break;
		case State.InitialMove:
			state = State.Start;
			break;
		case State.InitialReactionToEnemy:
			if (!(mActor.awareness.closestCoverPoint == null))
			{
				bool flag = !SectionTypeHelper.IsAGMG() || mActor.awareness.EnemiesICanSee != 0;
				if (WorldHelper.PercentageChance(60) && flag)
				{
					mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
					mMoveParams.forceCrouch = WorldHelper.PercentageChance(70);
				}
				else
				{
					mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				}
				mMoveParams.mDestination = mActor.GetPosition();
				mMoveParams.holdCoverWhenBored = true;
				new TaskMoveToCover(base.Owner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType | Config.ConsultParent, mMoveParams);
				panicCover = mActor.awareness.coverBooked;
				holdCoverUntil = 0f;
				ResetScores();
				state = State.ScoreNeighbours;
				ResetRemagnetTime();
			}
			break;
		case State.ScoreNeighbours:
			if (!(mActor.awareness.closestCoverPoint == null))
			{
				if (DebugThinkStep)
				{
					ScoreNeighbours();
				}
				IdleCheck();
			}
			break;
		case State.ChooseAction:
			if (!(mActor.awareness.closestCoverPoint == null))
			{
				if (mActor.behaviour.Suppressed && mActor.awareness.isInCover)
				{
					ResetScores();
				}
				else if (DebugThinkStep)
				{
					ChooseAction();
				}
			}
			break;
		case State.CheckTwoStageRoutes:
			if (!(mActor.awareness.closestCoverPoint == null) && DebugThinkStep)
			{
				CheckTwoStageRoutes();
			}
			break;
		case State.Execute:
		{
			if (mActor.awareness.closestCoverPoint == null)
			{
				break;
			}
			CoverPointCore coverPointCore = CoverNeighbour.CoverPoint(neighbours[bestNeighbour]);
			if (coverPointCore.Available(mActor))
			{
				if (WorldHelper.PercentageChance(60))
				{
					mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
					if (MustCrouch[bestNeighbour] && WorldHelper.PercentageChance(60))
					{
						mMoveParams.forceCrouch = true;
					}
				}
				else
				{
					mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				}
				mMoveParams.holdCoverWhenBored = true;
				new TaskMoveToCover(base.Owner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType | Config.ConsultParent, coverPointCore, mMoveParams);
				panicCover = null;
				holdCoverUntil = 0f;
				state = State.MovingToNewCover;
				originalCoverScore = TotalScore[bestNeighbour];
			}
			else
			{
				ResetScores();
			}
			break;
		}
		case State.MovingToNewCover:
			if (!(mActor.awareness.closestCoverPoint == null))
			{
				ResetScores();
				state = State.ScoreNeighbours;
			}
			break;
		}
	}

	public override void Finish()
	{
		enemiesIConsidered.Release();
		optHandle.SafeUnregister();
	}

	public override void Destroy()
	{
		optHandle.SafeUnregister();
	}

	public override bool HasFinished()
	{
		if (mConsultant != null && mConsultant.Consult(this))
		{
			return true;
		}
		return false;
	}

	private bool FindFixedGun()
	{
		if (mActor.awareness == null || !FactionHelper.AreEnemies(FactionHelper.Category.Player, mActor.awareness.faction) || !mAssaultParams.canUseFixedGuns)
		{
			return false;
		}
		if (!mActor.tasks.IsRunningTask(typeof(TaskUseFixedGun)) && mActor.behaviour != null)
		{
			FixedGun nearAvailableFixedGun = FixedGunManager.instance.GetNearAvailableFixedGun(mActor.GetPosition());
			if (nearAvailableFixedGun != null && nearAvailableFixedGun.IsGoodForActor(mActor))
			{
				new TaskUseFixedGun(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.Default, nearAvailableFixedGun, false, false, false, false);
				return true;
			}
		}
		return false;
	}

	public override bool Consult(Task child)
	{
		bool flag = optHandle.Update(mActor);
		if (flag)
		{
			alternateChecking = !alternateChecking;
			if (FindFixedGun())
			{
				return true;
			}
		}
		if (mActor.behaviour.Suppressed || (~mActor.awareness.ObstructedMask() & mActor.awareness.EnemiesIKnowAboutRecent) != 0)
		{
			inCombatUntil = Time.time + 3f;
		}
		if (mConsultant != null && mConsultant.Consult(this))
		{
			return true;
		}
		if (mActor.awareness.closestCoverPoint == null)
		{
			return false;
		}
		if (holdCoverUntil == 0f && mActor.awareness.isInCover)
		{
			holdCoverUntil = Time.time + Random.Range(mAssaultParams.HoldCoverMinTime, mAssaultParams.HoldCoverMaxTime);
		}
		if (alternateChecking && mActor.awareness.coverBooked != null && mActor.awareness.coverBooked != panicCover)
		{
			if (flag)
			{
				float r_Def;
				float r_Off;
				float r_Goal;
				ScoreCoverPoint(mActor.awareness.coverBooked, out r_Def, out r_Off, out r_Goal);
				float num = CalculateTotalScore(r_Off, r_Def, r_Goal);
				if (num < 0f && num < originalCoverScore - 500f)
				{
					state = State.InitialReactionToEnemy;
					return true;
				}
			}
			return false;
		}
		switch (state)
		{
		case State.InitialMove:
			if (child.GetType() == typeof(TaskMoveTo) && mActor.awareness.closestCoverPoint != null && mActor.awareness.closestCoverPoint.neighbours.Length > 1)
			{
				if (isGMGAndNotDominationMode() && GameController.Instance.mFirstPersonActor != null && (GameController.Instance.mFirstPersonActor.awareness.EnemiesWhoCanSeeMe() & mActor.awareness.FriendsICanSee & GKM.AliveMask) != 0)
				{
					state = State.InitialReactionToEnemy;
					return true;
				}
				if (mActor.awareness.EnemiesICanSee != 0)
				{
					state = State.InitialReactionToEnemy;
					return true;
				}
				if (mAssaultParams.Target.theObject != null && (mAssaultParams.Target.theObject.transform.position - cachedTargetPosition).sqrMagnitude > 9f)
				{
					state = State.Start;
					return true;
				}
			}
			break;
		case State.ScoreNeighbours:
			if (flag)
			{
				ScoreNeighbours();
			}
			if (IdleCheck())
			{
				return true;
			}
			break;
		case State.ChooseAction:
			if (flag)
			{
				ChooseAction();
			}
			break;
		case State.CheckTwoStageRoutes:
			if (DebugThinkStep)
			{
				CheckTwoStageRoutes();
			}
			break;
		case State.Execute:
			if (bestNeighbour != 0)
			{
				return true;
			}
			ResetScores();
			return false;
		case State.MovingToNewCover:
			if (mActor.awareness.isInCover && mActor.awareness.coverBooked.index == CoverNeighbour.coverIndex(neighbours[bestNeighbour]))
			{
				ResetScores();
				state = State.ScoreNeighbours;
			}
			else if ((mActor.awareness.EnemiesIKnowAboutFresh & ~(uint)enemiesIConsidered & mActor.awareness.EyesOnMe) != 0)
			{
				state = State.InitialReactionToEnemy;
				return true;
			}
			return false;
		}
		return false;
	}

	public bool isGMGAndNotDominationMode()
	{
		if (SectionTypeHelper.IsAGMG() && GMGData.Instance.CurrentGameType != GMGData.GameType.Domination)
		{
			return true;
		}
		return false;
	}

	private bool IdleCheck()
	{
		if (SectionTypeHelper.IsAGMG() && mActor.awareness.EnemiesICanSee == 0)
		{
			Vector2 vagueDirection;
			float distance;
			CoverPointCore cpc = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(mAssaultParams.Target.theObject.transform.position, out vagueDirection, out distance);
			if (!NewCoverPointManager.Instance().IsCampsite(cpc) && Time.time > headToMagnetTime)
			{
				state = State.Start;
				mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				return true;
			}
		}
		else
		{
			ResetRemagnetTime();
		}
		return false;
	}

	private void ResetRemagnetTime()
	{
		headToMagnetTime = Time.time + Random.Range(10f, 20f);
	}

	private void ResetScores()
	{
		if (mActor == null)
		{
			Debug.LogWarning("Reset Scores mActor == null");
			return;
		}
		if (NewCoverPointManager.Instance() == null)
		{
			Debug.LogWarning("Reset Scores NewCoverPointManager == null");
			return;
		}
		if (mAssaultParams == null)
		{
			Debug.LogWarning("Reset Scores mAssaultParams == null");
			return;
		}
		if (mAssaultParams.Target == null)
		{
			Debug.LogWarning("Reset Scores mAssaultParams.Target == null");
			return;
		}
		if (mAssaultParams.Target.theObject == null)
		{
			Goal = null;
		}
		else
		{
			Vector2 vagueDirection;
			float distance;
			Goal = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(mAssaultParams.Target.theObject.transform.position, out vagueDirection, out distance);
		}
		roundRobin = 0;
		allScored = false;
		Hub = mActor.awareness.closestCoverPoint;
		neighbours = Hub.neighbours;
		doorMasks = Hub.doorMasks;
		if (doorMasks != null && doorMasks.Length < neighbours.Length)
		{
			doorMasks = null;
		}
		OffenceScore = new float[neighbours.Length];
		DefenceScore = new float[neighbours.Length];
		GoalScore = new float[neighbours.Length];
		TotalScore = new float[neighbours.Length];
		RouteScore = new float[neighbours.Length];
		MustCrouch = new bool[neighbours.Length];
		MidPointScore = new float[neighbours.Length];
		if (Goal != null)
		{
			GoalDistance = NewCoverPointManager.Instance().GetCoverToCoverDistance(Hub.index, Goal.index);
		}
		else
		{
			GoalDistance = 0f;
		}
		state = State.ScoreNeighbours;
		BestScore = -1000000f;
		bestNeighbour = 0;
		enemiesIConsidered.Set(0u);
	}

	private void ScoreCoverPoint(CoverPointCore neighbour, out float r_Def, out float r_Off, out float r_Goal)
	{
		int index = neighbour.index;
		int num = NewCoverPointManager.Instance().coverPoints.Length;
		uint num2 = neighbour.stupidCoverAgainst & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		if ((mActor.awareness.coverCluster != null && !mActor.awareness.coverCluster.Includes(neighbour)) || !neighbour.Available(mActor) || (num2 != 0 && num2 == (mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask)))
		{
			r_Def = -100f;
			r_Off = -100f;
			r_Goal = -100f;
			return;
		}
		if (Goal == null)
		{
			r_Goal = 0f;
		}
		else
		{
			r_Goal = GoalDistance - (float)(int)NewCoverPointManager.Instance().baked.distanceTable[index + num * Goal.index];
		}
		r_Off = 0f;
		r_Def = EvaluateDefence(neighbour);
		uint num3 = neighbour.goodForFlanking & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		uint num4 = (neighbour.lowCoverAgainst | neighbour.highCoverAgainst) & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		if (num4 == 0)
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(num4);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (mActor.awareness.KnowWhereabouts(a))
			{
				r_Off += CalculateThreatWithUtility(a, mActor, a.GetPosition(), neighbour.coverCheckPos) * (((num3 & a.ident) == 0) ? 1f : 3f);
			}
		}
	}

	private void ScoreNeighbours()
	{
		if (Hub == null || mActor.awareness.closestCoverPoint.index != Hub.index)
		{
			ResetScores();
			return;
		}
		int num = CoverNeighbour.coverIndex(neighbours[roundRobin]);
		CoverPointCore neighbour = NewCoverPointManager.Instance().coverPoints[num];
		if (doorMasks == null || (doorMasks[roundRobin] & mActor.navAgent.walkableMask) == doorMasks[roundRobin])
		{
			ScoreCoverPoint(neighbour, out DefenceScore[roundRobin], out OffenceScore[roundRobin], out GoalScore[roundRobin]);
		}
		else
		{
			DefenceScore[roundRobin] = -200f;
			OffenceScore[roundRobin] = -200f;
			GoalScore[roundRobin] = -200f;
		}
		TotalScore[roundRobin] = CalculateTotalScore(OffenceScore[roundRobin], DefenceScore[roundRobin], GoalScore[roundRobin]);
		if (++roundRobin == neighbours.Length)
		{
			roundRobin = 0;
			allScored = true;
		}
		if (allScored && (holdCoverUntil <= Time.time || !mActor.awareness.isInCover))
		{
			roundRobin = 0;
			state = State.ChooseAction;
		}
	}

	private float EvaluateDefence(CoverPointCore cpc)
	{
		if (mActor.awareness.closestCoverPoint.index != Hub.index)
		{
			ResetScores();
		}
		float num = 0f;
		uint num2 = (cpc.noCoverAgainst | cpc.crouchCoverAgainst) & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		enemiesIConsidered.Include(num2);
		Actor a;
		if (num2 != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(num2);
			while (actorIdentIterator.NextActor(out a))
			{
				num -= CalculateThreat(mActor, a, cpc.coverCheckPos, a.GetPosition()) * ((!mActor.awareness.KnowWhereabouts(a)) ? 0.1f : 1f);
			}
		}
		uint num3 = (cpc.highCoverAgainst | cpc.lowCoverAgainst) & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		enemiesIConsidered.Include(num3);
		if (num3 != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(num3);
			while (actorIdentIterator.NextActor(out a))
			{
				if ((GKM.InCrowdOf(a).LastKnownPosition - cpc.gamePos).sqrMagnitude < 64f)
				{
					num -= CalculateThreat(mActor, a, cpc.coverCheckPos, a.GetPosition()) * ((!mActor.awareness.KnowWhereabouts(a)) ? 0.1f : 1f);
				}
			}
		}
		return num;
	}

	private float EvaluateTransientDefence(CoverPointCore cpc, ref bool mustCrouch)
	{
		if (mActor.awareness.closestCoverPoint.index != Hub.index)
		{
			ResetScores();
		}
		float num = 0f;
		uint num2 = cpc.noCoverAgainst & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		enemiesIConsidered.Include(num2);
		Actor a;
		if (num2 != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(num2);
			while (actorIdentIterator.NextActor(out a))
			{
				if (mActor.awareness.KnowWhereabouts(a))
				{
					num -= CalculateThreatFactoringDistractions(mActor, a, cpc.coverCheckPos, a.GetPosition());
				}
			}
		}
		uint num3 = cpc.crouchCoverAgainst & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		enemiesIConsidered.Include(num3);
		if (num3 != 0)
		{
			mustCrouch = true;
		}
		uint num4 = (cpc.highCoverAgainst | cpc.lowCoverAgainst) & mActor.awareness.EnemiesIKnowAbout & GKM.AliveMask;
		enemiesIConsidered.Include(num4);
		if (num4 != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(num4);
			while (actorIdentIterator.NextActor(out a))
			{
				if ((GKM.InCrowdOf(a).LastKnownPosition - cpc.gamePos).sqrMagnitude < 64f)
				{
					num -= CalculateThreatFactoringDistractions(mActor, a, cpc.coverCheckPos, a.GetPosition()) * ((!mActor.awareness.KnowWhereabouts(a)) ? 0.1f : 1f);
				}
			}
		}
		return num;
	}

	private float CalculateTotalScore(float off, float def, float goa)
	{
		float num = 100f;
		float num2 = 10f;
		float num3 = 1f;
		float num4 = mAssaultParams.GoalModifier;
		if (inCombatUntil > Time.time)
		{
			num4 = mAssaultParams.GoalModifier_InCombat;
		}
		float num5 = goa * num4 * num;
		num5 += def * mAssaultParams.DefenceModifier * num2;
		return num5 + off * mAssaultParams.OffenceModifier * num3;
	}

	private void ChooseAction()
	{
		if ((mActor.awareness.EnemiesIKnowAboutFresh & ~(uint)enemiesIConsidered & mActor.awareness.EyesOnMe) != 0)
		{
			ResetScores();
			return;
		}
		float num;
		while (true)
		{
			num = TotalScore[roundRobin];
			TotalScore[roundRobin] = -1000000f;
			if (num > BestScore)
			{
				break;
			}
			roundRobin++;
			if (roundRobin >= neighbours.Length)
			{
				roundRobin = 0;
				state = State.Execute;
				return;
			}
		}
		NewCoverPointManager newCoverPointManager = NewCoverPointManager.Instance();
		CoverPointCore coverPointCore = Hub;
		int num2 = CoverNeighbour.coverIndex(neighbours[roundRobin]);
		float num3 = 0f;
		float b = 0f;
		int num4 = 20;
		bool mustCrouch = false;
		int neighbour = roundRobin;
		while (coverPointCore.index != num2 && num4 > 0)
		{
			float distance;
			coverPointCore = newCoverPointManager.GetNextCoverOnRoute(coverPointCore, ref neighbour, out distance);
			float num5 = EvaluateTransientDefence(coverPointCore, ref mustCrouch);
			num3 += distance * (Mathf.Min(num5, b) * mAssaultParams.ExposureModifier - 1f);
			b = num5;
			num4--;
		}
		MustCrouch[roundRobin] = mustCrouch;
		RouteScore[roundRobin] = num3;
		num += num3;
		TotalScore[roundRobin] = num;
		if (num > BestScore)
		{
			bestNeighbour = roundRobin;
			BestScore = num;
		}
		roundRobin++;
		if (roundRobin >= neighbours.Length)
		{
			roundRobin = 0;
			state = State.Execute;
		}
	}

	private void CheckTwoStageRoutes()
	{
	}

	private float CalculateThreat(Actor a, Actor b, Vector3 aPos, Vector3 bPos)
	{
		if (b.baseCharacter.IsMortallyWounded() || a.baseCharacter.IsMortallyWounded())
		{
			return 0f;
		}
		float sqrMagnitude = (aPos - bPos).sqrMagnitude;
		return TargetScorer.EstimateWeaponDamageAtSquareDistance(b, sqrMagnitude, b.behaviour.PlayerControlled);
	}

	private float CalculateThreatWithUtility(Actor a, Actor b, Vector3 aPos, Vector3 bPos)
	{
		if (b.baseCharacter.IsMortallyWounded() || a.baseCharacter.IsMortallyWounded())
		{
			return 0f;
		}
		float sqrMagnitude = (aPos - bPos).sqrMagnitude;
		return TargetScorer.EstimateWeaponDamageWithUtilityAtSquareDistance(b, sqrMagnitude, b.behaviour.PlayerControlled);
	}

	private float CalculateThreatFactoringDistractions(Actor a, Actor b, Vector3 aPos, Vector3 bPos)
	{
		if (b.baseCharacter.IsMortallyWounded() || a.baseCharacter.IsMortallyWounded())
		{
			return 0f;
		}
		float sqrMagnitude = (aPos - bPos).sqrMagnitude;
		float num = TargetScorer.EstimateWeaponDamageAtSquareDistance(b, sqrMagnitude, b.behaviour.PlayerControlled);
		uint u = b.awareness.EyesOnMe & GKM.FactionMask(a.awareness.faction) & ~a.ident;
		float num2 = (float)WorldHelper.BitCount(u) * 2f;
		return num / (num2 + 1f);
	}
}
