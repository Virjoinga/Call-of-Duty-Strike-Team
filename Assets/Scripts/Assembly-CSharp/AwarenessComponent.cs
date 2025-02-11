using System;
using UnityEngine;

public class AwarenessComponent : BaseActorComponent
{
	private const byte kFreshKnowledge = 50;

	public const uint VALID_ZONE_IDENT_MASK = 2147483647u;

	public const uint OUTSIDE_ALL_ZONES_IDENT = 2147483648u;

	private const float SAFE_DISTANCE_TO_COVER = 0.5f;

	private const float SAFE_DISTANCE_TO_COVER_SQ = 0.25f;

	public const float kApproxActorTouchRadiusSqr = 0.42249995f;

	public bool renderFieldOfView;

	public static CoverPointCore[] validCoverArray = new CoverPointCore[1024];

	public static CoverPointCore[] usefulCoverArray = new CoverPointCore[1024];

	public static int validCoverCount;

	public static int usefulCoverCount;

	public CoverPointCore forcedClosestCover;

	public CoverPointCore closestCoverPoint;

	public bool isInCover;

	public bool keepCoverBooked;

	public Vector2 vagueDirection;

	public float currentDistance;

	public CoverPointCore coverBooked;

	public CoverCluster coverCluster;

	public FactionHelper.Category faction;

	private CharacterType mCharacterType;

	public bool visible;

	public bool canLook;

	public bool airborne;

	public float currentNoiseRadius;

	public float currentNoiseRadiusSqr;

	public DominantSoundType dominantSound;

	private int awarenessZoneSpawnGracePeriod;

	private ActorMask iCanSee = new ActorMask(0u, "I Can See");

	private ActorMask enemiesIKnowAbout = new ActorMask(0u, "Enemies IKA");

	private ActorMask enemiesIKnowAboutRecent = new ActorMask(0u, "Enemies IKA Recent");

	private ActorMask enemiesIKnowAboutFresh = new ActorMask(0u, "Enemies IKA Fresh");

	private ActorMask enemiesICanSee = new ActorMask(0u, "Enemies I Can See");

	private ActorMask friendsICanSee = new ActorMask(0u, "Friends I Can See");

	private ActorMask broadcastsTo = new ActorMask(0u, "Broadcasts To");

	private ActorMask eyesOnMe = new ActorMask(0u, "Eyes On Me");

	private ActorMask iCanHear = new ActorMask(0u, "I Can Hear");

	private ActorMask crouchObstructionChecked = new ActorMask(0u, "crouchObstructionChecked");

	public float broadcastPerceptionRange = 20f;

	public float broadcastPerceptionRangeSqr = 400f;

	private byte[] enemyKnowledgeDecay = new byte[32];

	public uint awarenessZonesImIn;

	public uint awarenessZonesDetectionOverride;

	public Vector3 standingEyeLevel = NewCoverPointManager.standingHeightOffset;

	public Vector3 crouchingEyeLevel = NewCoverPointManager.crouchingHeightOffset;

	public float closestUnobstructedEnemyDistSqr;

	public Actor closestUnobstructedEnemy;

	public float closestVisibleEnemyDistSqr;

	public Actor closestVisibleEnemy;

	public Transform cachedTrans;

	private bool poppedOutOfCover;

	private Vector2 forward2D;

	private float forward2DMagSqr;

	private float Fov;

	private float peripheralFov;

	private float visionRange;

	public float visionRangeSqr;

	private float mCs;

	private float mFovCosSqrSigned;

	private float mPeripheralCs;

	private float mPeripheralFovCosSqrSigned;

	private float mSideProximityThreshold;

	private float mSideProximityThresholdSqr;

	private float mRearProximityThreshold;

	private float mRearProximityThresholdSqr;

	public Vector2 myPos2D;

	private Vector3 lookDirection;

	private float mHalfFovRad;

	private float mPeripheralHalfFovRad;

	private Vector2 lookDirectionXZ;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public float debugValue;

	private static Vector2[] enemyPositionCache = new Vector2[32];

	private static float[] enemyDistanceCache = new float[32];

	private static int enemyCount;

	private bool respectFirstPersonCam;

	private Transform firstPersonTransform;

	public uint ICanSee
	{
		get
		{
			return iCanSee;
		}
		set
		{
			iCanSee.Set(value);
		}
	}

	public uint EnemiesIKnowAbout
	{
		get
		{
			return enemiesIKnowAbout;
		}
		set
		{
			enemiesIKnowAbout.Set(value);
		}
	}

	public uint EnemiesIKnowAboutRecent
	{
		get
		{
			return enemiesIKnowAboutRecent;
		}
		set
		{
			enemiesIKnowAboutRecent.Set(value);
		}
	}

	public uint EnemiesIKnowAboutFresh
	{
		get
		{
			return enemiesIKnowAboutFresh;
		}
		set
		{
			enemiesIKnowAboutFresh.Set(value);
		}
	}

	public uint EnemiesICanSee
	{
		get
		{
			return enemiesICanSee;
		}
		set
		{
			enemiesICanSee.Set(value);
		}
	}

	public uint FriendsICanSee
	{
		get
		{
			return friendsICanSee;
		}
		set
		{
			friendsICanSee.Set(value);
		}
	}

	public uint BroadcastsTo
	{
		get
		{
			return broadcastsTo;
		}
		set
		{
			broadcastsTo.Set(value);
		}
	}

	public uint EyesOnMe
	{
		get
		{
			return eyesOnMe;
		}
		set
		{
			eyesOnMe.Set(value);
		}
	}

	public uint ICanHear
	{
		get
		{
			return iCanHear;
		}
		set
		{
			iCanHear.Set(value);
		}
	}

	public uint CrouchObstructionChecked
	{
		get
		{
			return crouchObstructionChecked;
		}
		set
		{
			crouchObstructionChecked.Set(value);
		}
	}

	public CharacterType ChDefCharacterType
	{
		get
		{
			if (mCharacterType == CharacterType.Undefined)
			{
				throw new NotImplementedException();
			}
			return mCharacterType;
		}
		set
		{
			mCharacterType = value;
		}
	}

	public bool PoppedOutOfCover
	{
		get
		{
			return poppedOutOfCover & isInCover;
		}
		set
		{
			poppedOutOfCover = value;
		}
	}

	public bool IsCoverDataValid
	{
		get
		{
			return closestCoverPoint != null;
		}
	}

	public float FoV
	{
		get
		{
			return Fov;
		}
		set
		{
			Fov = value;
			CalculateHalfFovRad();
		}
	}

	public float PeripheralFoV
	{
		get
		{
			return peripheralFov;
		}
		set
		{
			peripheralFov = value;
			CalculateHalfPeripheralFovRad();
		}
	}

	public float VisionRange
	{
		get
		{
			return visionRange;
		}
		set
		{
			visionRange = value;
			visionRangeSqr = visionRange * visionRange;
		}
	}

	public float SideProximityThreshold
	{
		get
		{
			return mSideProximityThreshold;
		}
		set
		{
			mSideProximityThreshold = value;
			mSideProximityThresholdSqr = value * value;
		}
	}

	public float RearProximityThreshold
	{
		get
		{
			return mRearProximityThreshold;
		}
		set
		{
			mRearProximityThreshold = value;
			mRearProximityThresholdSqr = value * value;
		}
	}

	public Vector3 LookDirection
	{
		get
		{
			return lookDirection;
		}
		set
		{
			lookDirectionXZ = value.xz().normalized;
			if (isInCover && coverBooked != null && coverBooked.type == CoverPointCore.Type.HighWall)
			{
				float num = coverBooked.coverAngleSinR;
				Vector2 vector = coverBooked.cappedLookR;
				if (Vector2.Dot(lookDirectionXZ, coverBooked.snappedTangent.xz()) < 0f)
				{
					num = coverBooked.coverAngleSinL;
					vector = coverBooked.cappedLookL;
				}
				if (Vector3.Dot(lookDirectionXZ, coverBooked.snappedNormal.xz()) < num)
				{
					lookDirectionXZ = vector;
					lookDirection = value.normalized;
					lookDirection.x = lookDirectionXZ.x;
					lookDirection.z = lookDirectionXZ.y;
					lookDirection.Normalize();
					return;
				}
			}
			lookDirection = value.normalized;
		}
	}

	public Vector2 LookDirectionXZ
	{
		get
		{
			return lookDirectionXZ;
		}
	}

	public bool CanBeLookedAt
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public bool CanLook
	{
		get
		{
			return canLook;
		}
		set
		{
			canLook = value;
		}
	}

	public static void ClearCoverArrays()
	{
		for (int i = 0; i < 1024; i++)
		{
			validCoverArray[i] = null;
			usefulCoverArray[i] = null;
		}
	}

	public void SetFaction(FactionHelper.Category f)
	{
		if (f != faction)
		{
			faction = f;
			FlushAllAwareness();
		}
	}

	public void FlushAllAwareness()
	{
		uint num = ~myActor.ident;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay & num);
		GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].obstructed = uint.MaxValue;
		GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].friendlyMembers = 0u;
		GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].hostileMembers = 0u;
		ICanSee = 0u;
		EnemiesICanSee = 0u;
		FriendsICanSee = 0u;
		EnemiesIKnowAbout = 0u;
		EnemiesIKnowAboutFresh = 0u;
		EnemiesIKnowAboutRecent = 0u;
		BroadcastsTo = 0u;
		EyesOnMe = 0u;
		ICanHear = 0u;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].obstructed |= myActor.ident;
			GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].friendlyMembers &= num;
			GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].hostileMembers &= num;
			a.awareness.ICanSee &= num;
			a.awareness.FriendsICanSee &= num;
			a.awareness.EnemiesICanSee &= num;
			a.awareness.EnemiesIKnowAbout &= num;
			a.awareness.EnemiesIKnowAboutFresh &= num;
			a.awareness.EnemiesIKnowAboutRecent &= num;
			a.awareness.BroadcastsTo &= num;
			a.awareness.EyesOnMe &= num;
			a.awareness.ICanHear &= num;
		}
	}

	public void CalculateHalfFovRad()
	{
		mHalfFovRad = Fov * 0.5f * ((float)Math.PI / 180f);
		mCs = Mathf.Cos(mHalfFovRad);
		mFovCosSqrSigned = Mathf.Abs(mCs) * mCs;
	}

	public void CalculateHalfPeripheralFovRad()
	{
		mPeripheralHalfFovRad = peripheralFov * 0.5f * ((float)Math.PI / 180f);
		mPeripheralCs = Mathf.Cos(mPeripheralHalfFovRad);
		mPeripheralFovCosSqrSigned = Mathf.Abs(mPeripheralCs) * mPeripheralCs;
	}

	private void Awake()
	{
		cachedTrans = base.transform;
		Fov = 170f;
		CalculateHalfFovRad();
		PeripheralFoV = 195f;
		CalculateHalfPeripheralFovRad();
		RearProximityThreshold = 3f;
		SideProximityThreshold = 5f;
		isInCover = false;
		broadcastPerceptionRangeSqr = broadcastPerceptionRange * broadcastPerceptionRange;
		awarenessZonesImIn = 2147483648u;
		awarenessZoneSpawnGracePeriod = 4;
	}

	private void OnDestroy()
	{
		iCanSee.Release();
		enemiesIKnowAbout.Release();
		enemiesIKnowAboutFresh.Release();
		enemiesIKnowAboutRecent.Release();
		enemiesICanSee.Release();
		friendsICanSee.Release();
		broadcastsTo.Release();
		eyesOnMe.Release();
		iCanHear.Release();
		crouchObstructionChecked.Release();
	}

	private void Update()
	{
		DecayKnowledge();
		if (isInCover)
		{
			isInCover = keepCoverBooked;
		}
		keepCoverBooked = false;
		if (coverBooked != null && (coverBooked.gamePos - cachedTrans.position).sqrMagnitude < 0.25f)
		{
			isInCover = true;
			closestCoverPoint = coverBooked;
		}
		if (awarenessZoneSpawnGracePeriod > 0)
		{
			awarenessZoneSpawnGracePeriod--;
		}
	}

	private void DecayKnowledge()
	{
		uint num = 1u;
		byte b = (byte)WorldHelper.tenthSecondTick;
		for (int i = 0; i < 32; i++)
		{
			if (((uint)enemiesIKnowAboutFresh & num) != 0)
			{
				enemyKnowledgeDecay[i] = 50;
			}
			else if (enemyKnowledgeDecay[i] > b)
			{
				enemyKnowledgeDecay[i] -= b;
			}
			else
			{
				enemyKnowledgeDecay[i] = 0;
				enemiesIKnowAboutRecent.MaskBy(~num);
			}
			num += num;
		}
		enemiesIKnowAbout.Include(enemiesIKnowAboutFresh);
		enemiesIKnowAboutRecent.Include(enemiesIKnowAboutFresh);
	}

	public void EnableBroadcast(uint other, bool val)
	{
		if (val)
		{
			broadcastsTo.Include(other);
		}
		else
		{
			broadcastsTo.Exclude(other);
		}
	}

	public void UpdateFOV()
	{
		forward2D = LookDirection.xz();
		forward2DMagSqr = forward2D.sqrMagnitude;
		myPos2D = cachedTrans.position.xz();
	}

	public InFOVResult InFOV(Vector3 pos)
	{
		if (!canLook)
		{
			return InFOVResult.No;
		}
		Vector2 lhs = pos.xz() - myPos2D;
		float sqrMagnitude = lhs.sqrMagnitude;
		if (sqrMagnitude > visionRangeSqr)
		{
			return InFOVResult.No;
		}
		if (sqrMagnitude <= 0.42249995f)
		{
			return InFOVResult.Yes;
		}
		float num = Vector2.Dot(lhs, forward2D);
		float num2 = Mathf.Abs(num) * num;
		if (num2 > mFovCosSqrSigned * sqrMagnitude * forward2DMagSqr)
		{
			return InFOVResult.Yes;
		}
		if (ChDefCharacterType == CharacterType.Human)
		{
			if (num2 > mPeripheralFovCosSqrSigned * sqrMagnitude * forward2DMagSqr)
			{
				if (sqrMagnitude < mSideProximityThresholdSqr)
				{
					return InFOVResult.Yes;
				}
				return InFOVResult.YesIfStanding;
			}
			if (sqrMagnitude < mRearProximityThresholdSqr)
			{
				return InFOVResult.YesIfStanding;
			}
		}
		return InFOVResult.No;
	}

	public InFOVResult InFOV(AwarenessComponent other)
	{
		if (!other.visible)
		{
			return InFOVResult.No;
		}
		if (!AwarenessZonesInSync(other))
		{
			return InFOVResult.No;
		}
		return InFOV(other.myActor.GetPosition());
	}

	public void InitForSpotCheck()
	{
		UpdateFOV();
		iCanSee.Set(0u);
		enemiesICanSee.Set(0u);
		friendsICanSee.Set(0u);
		closestUnobstructedEnemy = null;
		closestVisibleEnemy = null;
		closestVisibleEnemyDistSqr = float.MaxValue;
		closestUnobstructedEnemyDistSqr = float.MaxValue;
		if (!myActor.baseCharacter.IsDead())
		{
			GlobalKnowledgeManager.Instance().aliveMask |= myActor.ident;
			if (!myActor.baseCharacter.IsMortallyWounded())
			{
				GlobalKnowledgeManager.Instance().upAndAboutMask |= myActor.ident;
			}
			if (myActor.baseCharacter.IsSelectable())
			{
				GlobalKnowledgeManager.Instance().selectableMask |= myActor.ident;
			}
		}
		eyesOnMe.Set(0u);
	}

	public CoverTable.VisibilityEstimate EstimateVisibility(AwarenessComponent ac)
	{
		if (closestCoverPoint != null && ac.closestCoverPoint != null && NewCoverPointManager.Instance() != null)
		{
			return NewCoverPointManager.Instance().baked.coverTable.EstimateVisibility(closestCoverPoint.index, isInCover, ac.closestCoverPoint.index, ac.isInCover);
		}
		return CoverTable.VisibilityEstimate.Impossible;
	}

	public CoverTable.VisibilityEstimate EstimateVisibility(AwarenessComponent ac, out CoverTable.VisibilityEstimate myVisibilityCrouched, out CoverTable.VisibilityEstimate otherVisibilityCrouched)
	{
		if (closestCoverPoint != null && ac.closestCoverPoint != null && NewCoverPointManager.Instance() != null)
		{
			int index = closestCoverPoint.index;
			int index2 = ac.closestCoverPoint.index;
			CoverTable coverTable = NewCoverPointManager.Instance().baked.coverTable;
			CoverTable.VisibilityEstimate visibilityEstimate = coverTable.EstimateVisibility(index, isInCover, index2, ac.isInCover);
			if (visibilityEstimate == CoverTable.VisibilityEstimate.Impossible)
			{
				myVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
				otherVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
				return visibilityEstimate;
			}
			bool crouchCoverUncertain = coverTable.GetCrouchCoverUncertain(closestCoverPoint.index, ac.closestCoverPoint.index);
			bool crouchCoverUncertain2 = coverTable.GetCrouchCoverUncertain(ac.closestCoverPoint.index, closestCoverPoint.index);
			if (crouchCoverUncertain)
			{
				myVisibilityCrouched = CoverTable.VisibilityEstimate.Possible;
			}
			else if (coverTable.GetShootingFromCrouchToStandingBlocked(index, index2))
			{
				myVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
			}
			else
			{
				myVisibilityCrouched = CoverTable.VisibilityEstimate.Certain;
			}
			if (crouchCoverUncertain2)
			{
				otherVisibilityCrouched = CoverTable.VisibilityEstimate.Possible;
			}
			else if (coverTable.GetShootingFromCrouchToStandingBlocked(index2, index))
			{
				otherVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
			}
			else
			{
				otherVisibilityCrouched = CoverTable.VisibilityEstimate.Certain;
			}
			return visibilityEstimate;
		}
		myVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
		otherVisibilityCrouched = CoverTable.VisibilityEstimate.Impossible;
		return CoverTable.VisibilityEstimate.Impossible;
	}

	public void EnterAwarenessZone(uint id)
	{
		awarenessZonesImIn = (awarenessZonesImIn & 0x7FFFFFFFu) | id;
	}

	public void LeaveAwarenessZone(uint id)
	{
		awarenessZonesImIn &= ~id;
		if (awarenessZonesImIn == 0)
		{
			awarenessZonesImIn = 2147483648u;
		}
	}

	public void BookCoverPoint(CoverPointCore ncp, float eta)
	{
		if (coverBooked != null && coverBooked.index != ncp.index)
		{
			coverBooked.Cancel(myActor);
			isInCover = false;
		}
		if (ncp.Book(myActor, eta))
		{
			coverBooked = ncp;
		}
	}

	public bool CoverValid()
	{
		if (!isInCover)
		{
			return false;
		}
		if (coverCluster != null && !coverCluster.Includes(coverBooked.index))
		{
			return false;
		}
		if (!myActor.tether.IsDestinationWithinTether(coverBooked.gamePos))
		{
			return false;
		}
		return true;
	}

	public bool CoverSafe()
	{
		return (coverBooked.noCoverAgainst & (uint)enemiesIKnowAboutRecent & GKM.AliveMask) == 0;
	}

	public void CancelCover()
	{
		isInCover = false;
		if (coverBooked != null)
		{
			coverBooked.Cancel(myActor);
			coverBooked = null;
		}
	}

	private void CacheEnemyPositions()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator((uint)enemiesIKnowAboutRecent & GKM.AliveMask);
		enemyCount = 0;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			enemyPositionCache[enemyCount] = a.GetPosition().xz() - myPos2D;
			enemyDistanceCache[enemyCount] = enemyPositionCache[enemyCount].sqrMagnitude;
			enemyCount++;
		}
	}

	private bool EnemyPositionsInvalidateCover(Vector2 runDirection, Vector2 relPos2D, float distToCoverSqr)
	{
		for (int i = 0; i < enemyCount; i++)
		{
			if (Vector2.Dot(enemyPositionCache[i], relPos2D) > 0f)
			{
				if (enemyDistanceCache[i] < distToCoverSqr)
				{
					return true;
				}
			}
			else if (Vector2.Dot(enemyPositionCache[i], runDirection) > enemyDistanceCache[i] * 0.3f && enemyDistanceCache[i] * 0.05f < distToCoverSqr)
			{
				return true;
			}
			float sqrMagnitude = (enemyPositionCache[i] - relPos2D).sqrMagnitude;
			if (sqrMagnitude < 64f)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCoverAvailable()
	{
		if (closestCoverPoint == null)
		{
			return false;
		}
		Vector3 vector = Vector3.zero;
		float num = float.MaxValue;
		if (myActor.tether.Active)
		{
			vector = myActor.tether.Position;
			num = myActor.tether.TetherLimitSq;
		}
		CacheEnemyPositions();
		int length = closestCoverPoint.neighbours.GetLength(0);
		uint num2 = (uint)enemiesIKnowAboutRecent & GKM.AliveMask;
		for (int i = 0; i < length; i++)
		{
			int packed = closestCoverPoint.neighbours[i];
			CoverPointCore coverPointCore = CoverNeighbour.CoverPoint(packed);
			Vector2 runDirection = CoverNeighbour.vagueDirection(packed);
			if (coverPointCore.Available(myActor) && ((coverPointCore.noCoverAgainst | coverPointCore.crouchCoverAgainst) & num2) == 0 && (num2 == 0 || (coverPointCore.stupidCoverAgainst & num2) != num2) && (coverPointCore.gamePos - vector).sqrMagnitude < num)
			{
				Vector2 relPos2D = coverPointCore.gamePos.xz() - myPos2D;
				float sqrMagnitude = relPos2D.sqrMagnitude;
				if (!EnemyPositionsInvalidateCover(runDirection, relPos2D, sqrMagnitude))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsInCover()
	{
		return isInCover;
	}

	public CoverPointCore GetValidOffensiveCoverNearestSpecifiedPosition(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether)
	{
		return GetValidCoverNearestSpecifiedPosition(specpos, searchRadius, exclusionRadius, respectTether, 1f);
	}

	public CoverPointCore GetValidDefensiveCoverNearestSpecifiedPosition(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether)
	{
		return GetValidCoverNearestSpecifiedPosition(specpos, searchRadius, exclusionRadius, respectTether, -1f);
	}

	public CoverPointCore GetOffensiveCoverIgnoringEnemies(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether)
	{
		uint u = enemiesIKnowAboutRecent;
		enemiesIKnowAboutRecent.Set(0u);
		CoverPointCore validCoverNearestSpecifiedPosition = GetValidCoverNearestSpecifiedPosition(specpos, searchRadius, exclusionRadius, respectTether, 1f);
		enemiesIKnowAboutRecent.Set(u);
		return validCoverNearestSpecifiedPosition;
	}

	public CoverPointCore GetDefensiveCoverAvoidingFirstPersonCam(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether)
	{
		if (GameController.Instance.IsFirstPerson)
		{
			respectFirstPersonCam = true;
			firstPersonTransform = CameraManager.Instance.PlayCamera.transform;
		}
		CoverPointCore validDefensiveCoverNearestSpecifiedPosition = GetValidDefensiveCoverNearestSpecifiedPosition(specpos, searchRadius, exclusionRadius, respectTether);
		respectFirstPersonCam = false;
		return validDefensiveCoverNearestSpecifiedPosition;
	}

	public CoverPointCore GetValidCoverNearestSpecifiedPosition(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether, float offenceDefence)
	{
		if (NewCoverPointManager.Instance() == null)
		{
			return null;
		}
		float num = searchRadius * searchRadius;
		float num2 = exclusionRadius * exclusionRadius;
		Vector2 vector;
		float distance;
		CoverPointCore coverPointCore = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(specpos, out vector, out distance);
		if (coverPointCore == null)
		{
			return null;
		}
		float num3 = distance * distance;
		float sqrMagnitude = (specpos - coverPointCore.gamePos).sqrMagnitude;
		if (sqrMagnitude > num3 * 1.5f)
		{
			num3 = sqrMagnitude;
		}
		float num4 = searchRadius + distance;
		num4 *= num4;
		Vector3 vector2 = Vector3.zero;
		float num5 = float.MaxValue;
		float num6 = float.MaxValue;
		if (respectTether && myActor.tether.Active)
		{
			vector2 = myActor.tether.Position;
			num5 = myActor.tether.TetherLimit;
			num6 = myActor.tether.TetherLimitSq;
		}
		float num7 = num5 + searchRadius;
		num7 *= num7;
		if ((specpos - vector2).sqrMagnitude > num7)
		{
			return null;
		}
		CacheEnemyPositions();
		int length = coverPointCore.neighbours.GetLength(0);
		CoverPointCore coverPointCore2 = null;
		float num8 = num3;
		uint num9 = (uint)enemiesIKnowAbout & GKM.AliveMask;
		bool flag = coverPointCore.doorMasks != null && coverPointCore.doorMasks.Length == length;
		for (int i = 0; i < length; i++)
		{
			int packed = coverPointCore.neighbours[i];
			CoverPointCore coverPointCore3 = CoverNeighbour.CoverPoint(packed);
			if ((coverCluster != null && !coverCluster.Includes(coverPointCore3)) || (flag && (coverPointCore.doorMasks[i] & myActor.navAgent.walkableMask) != coverPointCore.doorMasks[i]))
			{
				continue;
			}
			float num10 = CoverNeighbour.navMeshDistance(packed);
			if (i > 0)
			{
				num8 = num3 + num10 * num10 + 2f * distance * num10 * Vector3.Dot(vector, CoverNeighbour.vagueDirection(packed));
			}
			if (num8 > num4)
			{
				return coverPointCore2;
			}
			if (num8 > num || num8 < num2 || !coverPointCore3.AvailableAndDesirable(myActor) || (coverPointCore3.gamePos - vector2).sqrMagnitude >= num6 || ((coverPointCore3.noCoverAgainst | coverPointCore3.crouchCoverAgainst) & num9) != 0 || (num9 != 0 && (coverPointCore3.stupidCoverAgainst & num9) == num9) || Vector3.Dot(coverPointCore3.snappedNormal, specpos - coverPointCore.gamePos) * offenceDefence < 0f)
			{
				continue;
			}
			if (respectFirstPersonCam)
			{
				Vector3 lhs = coverPointCore3.gamePos - firstPersonTransform.position;
				float num11 = Vector3.Dot(lhs, firstPersonTransform.forward);
				float f = Vector3.Dot(lhs, firstPersonTransform.right);
				if (num11 > Mathf.Abs(f) * 1.1f)
				{
					continue;
				}
			}
			Vector2 relPos2D = coverPointCore3.gamePos.xz() - myPos2D;
			float sqrMagnitude2 = relPos2D.sqrMagnitude;
			if (!EnemyPositionsInvalidateCover(CoverNeighbour.vagueDirection(packed), relPos2D, sqrMagnitude2))
			{
				if (coverPointCore2 == null)
				{
					coverPointCore2 = coverPointCore3;
				}
				if (((coverPointCore3.lowCoverAgainst | coverPointCore3.highCoverAgainst) & (uint)enemiesIKnowAboutRecent) != 0)
				{
					return coverPointCore3;
				}
			}
		}
		return coverPointCore2;
	}

	public CoverPointCore GetCoverNearestSpecifiedPosition(Vector3 specpos, float searchRadius, float exclusionRadius, bool respectTether, float offenceDefence)
	{
		if (NewCoverPointManager.Instance() == null)
		{
			return null;
		}
		searchRadius += TutorialToggles.snapModifier;
		float num = searchRadius * searchRadius;
		float num2 = exclusionRadius * exclusionRadius;
		Vector2 vector;
		float distance;
		CoverPointCore coverPointCore = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(specpos, out vector, out distance);
		if (coverPointCore == null)
		{
			return null;
		}
		float num3 = distance * distance;
		float sqrMagnitude = (specpos - coverPointCore.gamePos).sqrMagnitude;
		if (sqrMagnitude > num3 * 1.5f)
		{
			num3 = sqrMagnitude;
		}
		float num4 = searchRadius + distance;
		num4 *= num4;
		Vector3 vector2 = Vector3.zero;
		float num5 = float.MaxValue;
		float num6 = float.MaxValue;
		if (respectTether && myActor.tether.Active)
		{
			vector2 = myActor.tether.Position;
			num5 = myActor.tether.TetherLimit;
			num6 = myActor.tether.TetherLimitSq;
		}
		float num7 = num5 + searchRadius;
		num7 *= num7;
		if ((specpos - vector2).sqrMagnitude > num7)
		{
			return null;
		}
		int length = coverPointCore.neighbours.GetLength(0);
		CoverPointCore result = null;
		float num8 = num3;
		for (int i = 0; i < length; i++)
		{
			int packed = coverPointCore.neighbours[i];
			CoverPointCore coverPointCore2 = CoverNeighbour.CoverPoint(packed);
			if (!(coverCluster != null) || coverCluster.Includes(coverPointCore2))
			{
				float num9 = CoverNeighbour.navMeshDistance(packed);
				if (i > 0)
				{
					num8 = num3 + num9 * num9 + 2f * distance * num9 * Vector3.Dot(vector, CoverNeighbour.vagueDirection(packed));
				}
				if (num8 > num4)
				{
					return result;
				}
				if (!(num8 >= num) && !(num8 < num2) && coverPointCore2.Available(myActor) && !((coverPointCore2.gamePos - vector2).sqrMagnitude >= num6) && Vector3.Dot(coverPointCore2.snappedNormal, specpos - coverPointCore.gamePos) * offenceDefence >= 0f)
				{
					return coverPointCore2;
				}
			}
		}
		return result;
	}

	public void SiftValidCoverFromList(int[] listOfCover)
	{
		validCoverCount = 0;
		usefulCoverCount = 0;
		if (NewCoverPointManager.Instance() == null)
		{
			return;
		}
		CacheEnemyPositions();
		int num = listOfCover.Length;
		uint num2 = (uint)enemiesIKnowAbout & GKM.AliveMask;
		for (int i = 0; i < num; i++)
		{
			CoverPointCore coverPointCore = NewCoverPointManager.Instance().coverPoints[listOfCover[i]];
			if ((coverCluster != null && !coverCluster.Includes(coverPointCore)) || !coverPointCore.Available(myActor) || ((coverPointCore.noCoverAgainst | coverPointCore.crouchCoverAgainst) & num2) != 0 || (num2 != 0 && (coverPointCore.stupidCoverAgainst & num2) == num2))
			{
				continue;
			}
			Vector2 relPos2D = coverPointCore.gamePos.xz() - myPos2D;
			float sqrMagnitude = relPos2D.sqrMagnitude;
			if (!EnemyPositionsInvalidateCover(Vector2.zero, relPos2D, sqrMagnitude))
			{
				if (((coverPointCore.lowCoverAgainst | coverPointCore.highCoverAgainst) & (uint)enemiesIKnowAboutRecent) != 0)
				{
					usefulCoverArray[usefulCoverCount++] = coverPointCore;
				}
				else
				{
					validCoverArray[validCoverCount++] = coverPointCore;
				}
			}
		}
	}

	public bool MustStandToShoot(Actor a, bool checkCertainty)
	{
		if (a == null || a.awareness == null || closestCoverPoint == null || a.awareness.closestCoverPoint == null)
		{
			return true;
		}
		if ((CrouchObstructionChecked & a.ident) != 0 && !Obstructed(a))
		{
			return false;
		}
		CoverTable coverTable = NewCoverPointManager.Instance().baked.coverTable;
		int index = closestCoverPoint.index;
		int index2 = a.awareness.closestCoverPoint.index;
		bool flag = ((!a.baseCharacter.IsCrouching()) ? coverTable.GetShootingFromCrouchToStandingBlocked(index, index2) : coverTable.GetShootingFromCrouchToCrouchBlocked(index, index2));
		if (!flag && checkCertainty && (!isInCover || !a.awareness.isInCover))
		{
			flag = coverTable.GetCrouchCoverUncertain(index, index2);
		}
		return flag;
	}

	public bool WouldCrouchToAvoidBeingSpotted(Actor spotter)
	{
		if (myActor.behaviour == null)
		{
			return false;
		}
		if (myActor.behaviour.aggressive)
		{
			return false;
		}
		if (!FactionHelper.WillCrouchToAvoidBeingSeen(myActor.awareness.faction))
		{
			return false;
		}
		if (myActor == GameController.Instance.mFirstPersonActor)
		{
			return false;
		}
		if (myActor.behaviour.aimedShotTarget != null || myActor.behaviour.suppressionTarget != null)
		{
			return false;
		}
		if (myActor.baseCharacter.MovementStyleRequested == BaseCharacter.MovementStyle.Run && myActor.baseCharacter.IsRouting())
		{
			return false;
		}
		if (EnemiesWhoCanSeeMe() == 0)
		{
			return true;
		}
		return false;
	}

	public Vector3 GetPosition()
	{
		return myActor.GetPosition() + new Vector3(0f, 1.5f, 0f);
	}

	public bool CanHear(AwarenessComponent ac, float dsqr)
	{
		if (myActor.ears == null)
		{
			return false;
		}
		if (!AwarenessZonesInSync(ac))
		{
			return false;
		}
		if (ac.dominantSound == DominantSoundType.Silence)
		{
			return false;
		}
		if (myActor.baseCharacter != null && myActor.baseCharacter.IsDead())
		{
			return false;
		}
		if (ac.currentNoiseRadiusSqr < dsqr)
		{
			return false;
		}
		if (myActor.ears.RangeSqr < dsqr)
		{
			return false;
		}
		if (ac.dominantSound == DominantSoundType.Footsteps)
		{
			if (ac.myActor.realCharacter.Location != myActor.realCharacter.Location)
			{
				return false;
			}
			if (ac.myActor.behaviour.SlowDownToAvoidBeingHeard(myActor))
			{
				return false;
			}
		}
		return true;
	}

	public void DiscloseLocation()
	{
		GlobalKnowledgeManager.InCrowd inCrowd = GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex];
		inCrowd.LastKnownPosition = myActor.GetPosition();
		inCrowd.lastSeenTime = WorldHelper.ThisFrameTime;
		inCrowd.wasInCover = isInCover;
		if (closestCoverPoint != null)
		{
			inCrowd.lastClosestCoverPoint = closestCoverPoint.index;
		}
	}

	public uint ObstructedMask()
	{
		return GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].obstructed;
	}

	public bool Obstructed(Actor a)
	{
		return (GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].obstructed & a.ident) != 0;
	}

	public bool BecomeAware(Actor a)
	{
		uint ident = a.ident;
		bool result = ((uint)enemiesIKnowAbout & ident) == 0;
		GlobalKnowledgeManager.InCrowd inCrowd = GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex];
		if (IsEnemy(a))
		{
			if (WorldHelper.ThisFrameTime - inCrowd.lastSeenTime > 0.5f)
			{
				inCrowd.LastKnownPosition = a.GetPosition();
				inCrowd.lastSeenTime = WorldHelper.ThisFrameTime;
				inCrowd.wasInCover = a.awareness.isInCover;
				if (a.awareness.closestCoverPoint != null)
				{
					inCrowd.lastClosestCoverPoint = a.awareness.closestCoverPoint.index;
				}
			}
			inCrowd.hostileMembers |= myActor.ident;
			enemiesIKnowAboutFresh.Include(ident);
			enemiesIKnowAboutRecent.Include(ident);
			enemiesIKnowAbout.Include(ident);
		}
		else
		{
			inCrowd.friendlyMembers |= myActor.ident;
		}
		return result;
	}

	public bool BecomeAware(Actor a, Vector3 pos)
	{
		uint ident = a.ident;
		bool result = ((uint)enemiesIKnowAbout & ident) == 0;
		if (a.awareness.faction != faction)
		{
			enemiesIKnowAboutFresh.Include(ident);
			enemiesIKnowAboutRecent.Include(ident);
			enemiesIKnowAbout.Include(ident);
			GlobalKnowledgeManager.InCrowd inCrowd = GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex];
			if (WorldHelper.ThisFrameTime - inCrowd.lastSeenTime > 0.5f)
			{
				inCrowd.LastKnownPosition = pos;
				inCrowd.lastSeenTime = WorldHelper.ThisFrameTime;
				inCrowd.wasInCover = false;
			}
		}
		else
		{
			GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].friendlyMembers |= myActor.ident;
		}
		return result;
	}

	public void Forget(Actor a)
	{
		int quickIndex = a.quickIndex;
		uint num = ~myActor.ident;
		GlobalKnowledgeManager.Instance().inCrowds[quickIndex].hostileMembers &= num;
		GlobalKnowledgeManager.Instance().inCrowds[quickIndex].futureHostileMembers &= num;
		num = ~a.ident;
		enemiesIKnowAbout.MaskBy(num);
		enemiesIKnowAboutFresh.MaskBy(num);
		enemiesIKnowAboutRecent.MaskBy(num);
		enemiesICanSee.MaskBy(num);
		iCanSee.MaskBy(num);
		friendsICanSee.MaskBy(num);
	}

	public void ForgetAllEnemies()
	{
		uint num = ~myActor.ident;
		for (int i = 0; i < 32; i++)
		{
			GlobalKnowledgeManager.Instance().inCrowds[i].hostileMembers &= num;
			GlobalKnowledgeManager.Instance().inCrowds[i].futureHostileMembers &= num;
		}
		enemiesICanSee.Set(0u);
		enemiesIKnowAbout.Set(0u);
		enemiesIKnowAboutFresh.Set(0u);
		enemiesIKnowAboutRecent.Set(0u);
		iCanSee = friendsICanSee;
	}

	public Actor GetNearestVisibleEnemy(out float distanceSquared)
	{
		distanceSquared = closestVisibleEnemyDistSqr;
		return closestVisibleEnemy;
	}

	public Actor GetNearestUnobstructedEnemy()
	{
		return closestUnobstructedEnemy;
	}

	public bool KnowWhereabouts(Actor a)
	{
		if (((uint)enemiesICanSee & a.ident) != 0)
		{
			return true;
		}
		return ((uint)enemiesIKnowAbout & a.ident) != 0 && GKM.InCrowdOf(a).upToDate;
	}

	public bool CanSeeAnyEnemies()
	{
		return (uint)enemiesICanSee != 0;
	}

	public bool CanSee(Actor actor)
	{
		return ((uint)iCanSee & actor.ident) != 0;
	}

	public bool CanSeeMe(Actor actor)
	{
		return ((uint)actor.awareness.iCanSee & myActor.ident) != 0;
	}

	public bool CanAnyEnemiesSeeMe()
	{
		return (GKM.InCrowdOf(myActor).hostileMembers & (uint)eyesOnMe) != 0;
	}

	public uint EnemiesWhoCanSeeMe()
	{
		return GKM.InCrowdOf(myActor).hostileMembers & (uint)eyesOnMe;
	}

	public bool AwarenessZonesInSync(AwarenessComponent ac)
	{
		if (awarenessZoneSpawnGracePeriod + ac.awarenessZoneSpawnGracePeriod > 0)
		{
			return false;
		}
		return awarenessZonesImIn == ac.awarenessZonesImIn;
	}

	public bool AwarenessZonesInSync(Actor act)
	{
		return awarenessZonesImIn == act.awareness.awarenessZonesImIn;
	}

	public bool AwareOf(Actor actor)
	{
		return ((uint)enemiesIKnowAbout & actor.ident) != 0;
	}

	public bool IsAnyKnownEnemyAlert()
	{
		return (GlobalKnowledgeManager.Instance().globalAlertness & (uint)enemiesIKnowAbout) != 0;
	}

	public bool IsAnyEnemyRecentlyAwareOfMe()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].hostileMembers);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (((uint)a.awareness.enemiesIKnowAboutRecent & myActor.ident) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsEnemy(Actor act)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)faction] & (uint)(1 << (int)act.awareness.faction)) != 0;
	}

	public bool IsFriend(Actor act)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)faction] & (uint)(1 << (int)act.awareness.faction)) == 0;
	}

	public bool IsEnemy(AwarenessComponent ac)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)faction] & (uint)(1 << (int)ac.faction)) != 0;
	}

	public bool IsFriend(AwarenessComponent ac)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)faction] & (uint)(1 << (int)ac.faction)) == 0;
	}

	public bool EngagedInCombat()
	{
		return ((uint)enemiesIKnowAbout & GlobalKnowledgeManager.Instance().aliveMask & GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].hostileMembers) != 0;
	}

	public uint EngagedInCombatWith()
	{
		return (uint)enemiesIKnowAbout & GlobalKnowledgeManager.Instance().aliveMask & GlobalKnowledgeManager.Instance().inCrowds[myActor.quickIndex].hostileMembers;
	}

	public Actor GetNearestKnownThreat()
	{
		uint m = (uint)enemiesIKnowAboutRecent & GKM.AliveMask;
		float num = float.MaxValue;
		Actor result = null;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(m);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.baseCharacter.IsMortallyWounded() && !a.weapon.IsUnarmed() && (a.awareness.EnemiesIKnowAboutRecent & myActor.ident) != 0 && !Obstructed(a))
			{
				float sqrMagnitude = (cachedTrans.position - GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].LastKnownPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = a;
				}
			}
		}
		return result;
	}

	public Actor GetNearestKnownEnemy(out Vector3 lastKnownPosition, bool recentOnly)
	{
		return GetNearestKnownEnemy(out lastKnownPosition, recentOnly, CharacterType.Undefined);
	}

	public Actor GetNearestKnownEnemy(out Vector3 lastKnownPosition, bool recentOnly, CharacterType exclude)
	{
		float num = float.MaxValue;
		int num2 = -1;
		uint num3 = ((!recentOnly) ? enemiesIKnowAbout : enemiesIKnowAboutRecent);
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(num3 & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if ((exclude == CharacterType.Undefined || a.awareness.ChDefCharacterType != exclude) && !a.baseCharacter.IsMortallyWounded())
			{
				float sqrMagnitude = (cachedTrans.position - GlobalKnowledgeManager.Instance().inCrowds[a.quickIndex].LastKnownPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = a.quickIndex;
				}
			}
		}
		if (num2 >= 0)
		{
			lastKnownPosition = GlobalKnowledgeManager.Instance().inCrowds[num2].LastKnownPosition + NewCoverPointManager.standingHeightOffset;
			return GlobalKnowledgeManager.Instance().GetActorFromIndex(num2);
		}
		lastKnownPosition = Vector3.zero;
		return null;
	}

	public void GLDebugVisualise(bool forceShow)
	{
		if (forceShow || renderFieldOfView)
		{
			DebugDraw(true);
		}
	}

	private void DrawRadialLine(bool glRender, Vector3 fwd, float angle, float inner, float outer)
	{
		Vector3 zero = Vector3.zero;
		zero.x = fwd.z;
		zero.z = 0f - fwd.x;
		Vector3 position = base.transform.position;
		position.y += 1.5f;
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		Vector3 vector = fwd * num + zero * num2;
		DebugDrawLine(glRender, position + vector * inner, position + vector * outer);
	}

	private void DrawArc(bool glRender, Vector3 fwd, float a1, float a2, float r, int sections)
	{
		Vector3 position = base.transform.position;
		position.y += 1.5f;
		Vector3 zero = Vector3.zero;
		Vector3 vector = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		zero2.x = fwd.z;
		zero2.z = 0f - fwd.x;
		for (int i = 0; i <= sections; i++)
		{
			float f = Mathf.Lerp(a1, a2, (float)i / (float)sections);
			zero = vector;
			float num = Mathf.Cos(f) * r;
			float num2 = Mathf.Sin(f) * r;
			vector = fwd * num + zero2 * num2;
			if (i > 0)
			{
				DebugDrawLine(glRender, position + zero, position + vector);
			}
		}
	}

	private void DebugDraw(bool glRender)
	{
		CharacterType chDefCharacterType = ChDefCharacterType;
		if (chDefCharacterType == CharacterType.Human)
		{
			DebugDraw_Human(glRender);
		}
		else
		{
			DebugDraw_Default(glRender);
		}
	}

	private void DebugDraw_Human(bool glRender)
	{
		Vector3 position = base.transform.position;
		position.y += 1.5f;
		Vector3 fwd = LookDirection;
		fwd.y = 0f;
		fwd.Normalize();
		DebugDrawSetColour(glRender, Color.yellow);
		DrawRadialLine(glRender, fwd, mHalfFovRad, mSideProximityThreshold, visionRange);
		DrawArc(glRender, fwd, mHalfFovRad, mPeripheralHalfFovRad, mSideProximityThreshold, 10);
		DebugDrawSetColour(glRender, Color.white);
		DrawRadialLine(glRender, fwd, 0f - mHalfFovRad, mSideProximityThreshold, visionRange);
		DrawArc(glRender, fwd, 0f - mHalfFovRad, 0f - mPeripheralHalfFovRad, mSideProximityThreshold, 10);
		DrawArc(glRender, fwd, 0f - mHalfFovRad, mHalfFovRad, visionRange, 20);
		DebugDrawSetColour(glRender, Color.red);
		DrawRadialLine(glRender, fwd, mPeripheralHalfFovRad, 0f, visionRange);
		DrawRadialLine(glRender, fwd, 0f - mPeripheralHalfFovRad, 0f, visionRange);
		DrawArc(glRender, fwd, mPeripheralHalfFovRad, (float)Math.PI * 2f - mPeripheralHalfFovRad, mRearProximityThreshold, 10);
		DrawArc(glRender, fwd, 0f - mHalfFovRad, 0f - mPeripheralHalfFovRad, visionRange, 10);
		DrawArc(glRender, fwd, mHalfFovRad, mPeripheralHalfFovRad, visionRange, 10);
		DebugDrawSetColour(glRender, Color.blue);
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(iCanSee);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 position2 = a.transform.position;
			DebugDrawLine(glRender, position, position2);
		}
	}

	private void DebugDraw_Default(bool glRender)
	{
		Vector3 position = base.transform.position;
		position.y += 1.5f;
		Vector3 fwd = LookDirection;
		fwd.y = 0f;
		fwd.Normalize();
		DebugDrawSetColour(glRender, Color.yellow);
		DrawRadialLine(glRender, fwd, mHalfFovRad, 0f, visionRange);
		DebugDrawSetColour(glRender, Color.white);
		DrawRadialLine(glRender, fwd, 0f - mHalfFovRad, 0f, visionRange);
		DrawArc(glRender, fwd, 0f - mHalfFovRad, mHalfFovRad, visionRange, 20);
		DebugDrawSetColour(glRender, Color.blue);
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(iCanSee);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 position2 = a.transform.position;
			DebugDrawLine(glRender, position, position2);
		}
	}

	private void DebugDrawSetColour(bool glRender, Color colour)
	{
		if (glRender)
		{
			GL.Color(colour);
		}
		else
		{
			Gizmos.color = colour;
		}
	}

	private void DebugDrawLine(bool glRender, Vector3 start, Vector3 end)
	{
		if (glRender)
		{
			GL.Vertex3(start.x, start.y, start.z);
			GL.Vertex3(end.x, end.y, end.z);
		}
		else
		{
			Gizmos.DrawLine(start, end);
		}
	}
}
