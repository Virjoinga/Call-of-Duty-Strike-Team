using System.Collections.Generic;
using UnityEngine;

public class GlobalKnowledgeManager : MonoBehaviour
{
	public class InCrowd
	{
		private Vector3 lastKnownPosition;

		public int lastClosestCoverPoint;

		public int previousClosestCoverPoint = -1;

		public bool wasInCover;

		public float lastSeenTime;

		public float updateLocationUntil;

		public uint friendlyMembers;

		public uint hostileMembers;

		public uint futureHostileMembers;

		public uint obstructed;

		public bool upToDate;

		public uint Members
		{
			get
			{
				return friendlyMembers | hostileMembers;
			}
		}

		public Vector3 LastKnownPosition
		{
			get
			{
				return lastKnownPosition;
			}
			set
			{
				upToDate = true;
				lastKnownPosition = value;
			}
		}

		public InCrowd()
		{
			lastKnownPosition = Vector3.zero;
			lastSeenTime = 0f;
			friendlyMembers = 0u;
			hostileMembers = 0u;
			obstructed = uint.MaxValue;
			futureHostileMembers = 0u;
			lastClosestCoverPoint = -1;
			previousClosestCoverPoint = -1;
		}

		public void SetObstructed(uint ident, bool val)
		{
			if (val)
			{
				obstructed |= ident;
			}
			else
			{
				obstructed &= ~ident;
			}
		}

		public void InitForSpotCheck()
		{
			futureHostileMembers = 0u;
			friendlyMembers = 0u;
		}
	}

	public const int kMaxFactions = 8;

	public const int kMaxTrackers = 32;

	public const float kKeepUpdatingOutOfSightTime = 2f;

	private const float kOutOfDateRangeSqr = 9f;

	private const int kValidationCost = 1;

	private const int kObstructionCostFast = 1;

	private const int kObstructionCostSlow = 200;

	public float[,] LastLOSTime = new float[32, 32];

	public List<int> activeList = new List<int>();

	public int activeListLen;

	private static GlobalKnowledgeManager instance;

	public uint[] crouchBecauseOf = new uint[32];

	public InCrowd[] inCrowds;

	private uint[] knowledgeMatrix;

	public uint globalAlertness;

	public uint[] globalVision = new uint[8];

	public uint[] factionMask = new uint[8];

	public uint[] enemyMask = new uint[8];

	public uint persistentMask;

	public uint aliveMask;

	public uint upAndAboutMask;

	public uint selectableMask;

	public uint playerControlledMask;

	public uint[] characterTypeMask = new uint[9];

	public AwarenessZone[] awarenessZones;

	public bool forceValidation;

	public uint[] FactionBroadcastsTo = new uint[8];

	public uint[] FactionHostileTo = new uint[8];

	public int traces;

	private int refreshClosest_RR;

	private int inCrowds_RR;

	private int obstructionValidationFrameCost;

	private int InCrowdObstructionBookmark;

	private int kObstructionValidationMaxCost = 200;

	private Actor[] actorArray = new Actor[32];

	public uint identsInUse;

	public static int logCount;

	public static GlobalKnowledgeManager Instance()
	{
		return instance;
	}

	public void ConfigureFaction(FactionHelper.Category fc, string config)
	{
		for (int i = 0; i < config.Length; i++)
		{
			uint num = (uint)(1 << i);
			switch (config[i])
			{
			case 'B':
				FactionBroadcastsTo[(int)fc] |= num;
				break;
			case 'H':
				FactionHostileTo[(int)fc] |= num;
				break;
			}
		}
	}

	private void Awake()
	{
		inCrowds = new InCrowd[32];
		identsInUse = 0u;
		for (int i = 0; i < 32; i++)
		{
			actorArray[i] = null;
			inCrowds[i] = new InCrowd();
		}
		instance = this;
		knowledgeMatrix = new uint[32];
		awarenessZones = Object.FindObjectsOfType(typeof(AwarenessZone)) as AwarenessZone[];
		for (int i = 0; i < awarenessZones.GetLength(0); i++)
		{
			awarenessZones[i].ident = (uint)(1 << i);
		}
		ConfigureFaction(FactionHelper.Category.Player, "BHHN");
		ConfigureFaction(FactionHelper.Category.Enemy, "HBNN");
		ConfigureFaction(FactionHelper.Category.SoloEnemy, "HNNN");
		ConfigureFaction(FactionHelper.Category.Neutral, "NNNN");
		TargetScorer.FlushScores();
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void SetNewCoverPointManager(NewCoverPointManager ncp)
	{
		ncp.enabled = true;
		NewCoverPointManager.ReplaceInstance(ncp);
		for (int i = 0; i < 32; i++)
		{
			inCrowds[i].lastClosestCoverPoint = -1;
			inCrowds[i].previousClosestCoverPoint = -1;
			inCrowds[i].wasInCover = false;
			if (actorArray[i] != null)
			{
				actorArray[i].Command("CoverInvalid");
				AwarenessComponent awareness = actorArray[i].awareness;
				awareness.closestCoverPoint = null;
				awareness.coverBooked = null;
				awareness.isInCover = false;
				awareness.awarenessZonesImIn = 2147483648u;
			}
		}
		awarenessZones = Object.FindObjectsOfType(typeof(AwarenessZone)) as AwarenessZone[];
		for (int i = 0; i < awarenessZones.GetLength(0); i++)
		{
			awarenessZones[i].ident = (uint)(1 << i);
		}
	}

	private void Update()
	{
		NewCoverPointManager newCoverPointManager = NewCoverPointManager.Instance();
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if (!(newCoverPointManager == null) && !(newCoverPointManager.baked == null) && newCoverPointManager.baked.baked && !GameController.Instance.IsPaused && (!(sectionManager != null) || sectionManager.SectionActivated))
		{
			traces = 0;
			RefreshClosestCoverPoints();
			UpdateObstructionAndValidateCoverPoints();
			CheckForSpottingAndHearing();
		}
	}

	private void RefreshClosestCoverPoints()
	{
		int num = refreshClosest_RR & 1;
		for (int i = num; i < activeListLen; i += 2)
		{
			int num2 = activeList[i];
			AwarenessComponent awareness = actorArray[num2].awareness;
			if (awareness.closestCoverPoint != null)
			{
				awareness.closestCoverPoint.blocked = 0u;
			}
		}
		NewCoverPointManager newCoverPointManager = NewCoverPointManager.Instance();
		for (int i = num; i < activeListLen; i += 2)
		{
			int num2 = activeList[i];
			AwarenessComponent awareness = actorArray[num2].awareness;
			if (awareness.forcedClosestCover != null)
			{
				awareness.closestCoverPoint = awareness.forcedClosestCover;
			}
			else if (awareness.isInCover)
			{
				awareness.closestCoverPoint = awareness.coverBooked;
				if (awareness.closestCoverPoint != null && actorArray[num2].baseCharacter != null && !actorArray[num2].baseCharacter.IsDead())
				{
					awareness.closestCoverPoint.blocked = awareness.myActor.ident;
				}
			}
			else if (awareness.airborne)
			{
				awareness.closestCoverPoint = newCoverPointManager.FindClosestInAirCoverPoint(awareness.cachedTrans.position);
			}
			else
			{
				awareness.closestCoverPoint = newCoverPointManager.FindClosestCoverPoint_Fast(awareness.cachedTrans.position, out awareness.vagueDirection, out awareness.currentDistance);
				if (awareness.closestCoverPoint != null && !actorArray[num2].baseCharacter.IsDead() && !actorArray[num2].baseCharacter.IsMortallyWounded() && awareness.closestCoverPoint.blocked == 0 && (awareness.myActor.GetPosition() - awareness.closestCoverPoint.gamePos).sqrMagnitude < 4f)
				{
					awareness.closestCoverPoint.blocked = awareness.myActor.ident;
				}
			}
		}
		refreshClosest_RR++;
	}

	private void UpdateObstructionAndValidateCoverPoints()
	{
		if (NewCoverPointManager.Instance().baked.coverTable.uncertaintyMapInvalid)
		{
			kObstructionValidationMaxCost = 1000;
		}
		else if (SectionTypeHelper.IsAGMG())
		{
			kObstructionValidationMaxCost = 700;
		}
		else
		{
			kObstructionValidationMaxCost = 200;
		}
		obstructionValidationFrameCost = 0;
		int count = activeList.Count;
		if (count == 0)
		{
			return;
		}
		inCrowds_RR %= count;
		for (int i = 0; i < count; i++)
		{
			int index = activeList[inCrowds_RR];
			if (UpdateInCrowdObstruction(index))
			{
				ValidateCoverPoints(index);
				InCrowdObstructionBookmark = 0;
				inCrowds_RR = (inCrowds_RR + 1) % count;
			}
			if (obstructionValidationFrameCost >= kObstructionValidationMaxCost)
			{
				break;
			}
		}
	}

	private void ValidateCoverPoints(int index)
	{
		NewCoverPointManager newCoverPointManager = NewCoverPointManager.Instance();
		InCrowd inCrowd = inCrowds[index];
		if (forceValidation)
		{
			Actor actor = actorArray[index];
			newCoverPointManager.ValidateCoverPoints((uint)(1 << index), actor.awareness.closestCoverPoint.index, actor.awareness.isInCover, actor.GetPosition());
		}
		else if (inCrowd.lastClosestCoverPoint >= 0)
		{
			if (inCrowd.previousClosestCoverPoint == inCrowd.lastClosestCoverPoint)
			{
				newCoverPointManager.ValidateCoverPoints_LocationUnchanged((uint)(1 << index), inCrowd.lastClosestCoverPoint, inCrowd.wasInCover, inCrowd.LastKnownPosition);
			}
			else
			{
				newCoverPointManager.ValidateCoverPoints((uint)(1 << index), inCrowd.lastClosestCoverPoint, inCrowd.wasInCover, inCrowd.LastKnownPosition);
				inCrowd.previousClosestCoverPoint = inCrowd.lastClosestCoverPoint;
			}
			obstructionValidationFrameCost++;
		}
		if (actorArray[index].awareness.closestCoverPoint != null)
		{
			newCoverPointManager.UpdateInterest((uint)(1 << index), actorArray[index].awareness.closestCoverPoint.index);
		}
	}

	private bool MustCheckAccurately(Actor a, Actor oa, out bool possibleresult)
	{
		possibleresult = false;
		if (a.realCharacter.IsDead())
		{
			if (!oa.realCharacter.IsDead())
			{
				if (oa.behaviour.PlayerControlled || oa.behaviour.InActiveAlertState() || ((uint)oa.behaviour.bodiesIveSeen & a.ident) != 0)
				{
					return false;
				}
				return true;
			}
			possibleresult = true;
			return false;
		}
		if (oa.realCharacter.IsDead())
		{
			if (a.behaviour.PlayerControlled || a.behaviour.InActiveAlertState() || ((uint)a.behaviour.bodiesIveSeen & oa.ident) != 0)
			{
				return false;
			}
			return true;
		}
		return (FactionHostileTo[(int)a.awareness.faction] & (uint)(1 << (int)oa.awareness.faction)) != 0;
	}

	private void ForcePendingCrouch(Actor a, Actor b)
	{
		crouchBecauseOf[a.quickIndex] |= b.ident;
	}

	private void ApplyPendingCrouch(Actor a)
	{
		if (!a.baseCharacter.IsCrouching())
		{
			a.Command("Crouch");
		}
		if (a.behaviour != null)
		{
			a.behaviour.crouchToAvoidBeingSeenUntil = Time.time + 0.5f;
		}
	}

	private bool CheckForSuccessfulCrouch_OneOnly(Vector3 start, Vector3 end, Actor a, Actor b, bool mustCastToBeSure)
	{
		bool flag = false;
		bool flag2 = a.baseCharacter.LowProfile();
		bool flag3 = a.awareness.WouldCrouchToAvoidBeingSpotted(b);
		if (flag2 || flag3)
		{
			start += a.awareness.crouchingEyeLevel;
			a.awareness.CrouchObstructionChecked |= b.ident;
			flag = Obstructed(start, end);
			if (flag && flag3)
			{
				ForcePendingCrouch(a, b);
			}
			else if (mustCastToBeSure)
			{
				return Obstructed(a.awareness, b.awareness);
			}
		}
		else if (mustCastToBeSure)
		{
			return Obstructed(a.awareness, b.awareness);
		}
		return flag;
	}

	private bool CheckForSuccessfulCrouch_Either(Vector3 start, Vector3 end, Actor a, Actor b, bool mustCastToBeSure)
	{
		bool flag = false;
		bool flag2 = a.baseCharacter.LowProfile();
		bool flag3 = b.baseCharacter.LowProfile();
		bool flag4 = a.awareness.WouldCrouchToAvoidBeingSpotted(b);
		bool flag5 = b.awareness.WouldCrouchToAvoidBeingSpotted(a);
		bool flag6 = flag2 || flag4;
		bool flag7 = flag3 || flag5;
		if (!flag6 && !flag7)
		{
			if (mustCastToBeSure)
			{
				return Obstructed(a.awareness, b.awareness);
			}
			return false;
		}
		if (flag6)
		{
			start += a.awareness.crouchingEyeLevel;
			a.awareness.CrouchObstructionChecked |= b.ident;
		}
		else
		{
			start += a.awareness.standingEyeLevel;
		}
		if (flag7)
		{
			end += b.awareness.crouchingEyeLevel;
			b.awareness.CrouchObstructionChecked |= a.ident;
		}
		else
		{
			end += b.awareness.standingEyeLevel;
		}
		flag = Obstructed(start, end);
		if (flag)
		{
			if (flag4)
			{
				ForcePendingCrouch(a, b);
			}
			if (flag5)
			{
				ForcePendingCrouch(b, a);
			}
		}
		return flag;
	}

	private void CheckStanceVisibility_Certain(Actor a, Actor b, CoverTable.VisibilityEstimate aVisibilityWhenCrouched, CoverTable.VisibilityEstimate bVisibilityWhenCrouched)
	{
		bool possibleresult = false;
		if (MustCheckAccurately(a, b, out possibleresult) && (!a.behaviour.PlayerControlled || !b.awareness.KnowWhereabouts(a)) && (!b.behaviour.PlayerControlled || !a.awareness.KnowWhereabouts(b)) && (aVisibilityWhenCrouched != CoverTable.VisibilityEstimate.Certain || bVisibilityWhenCrouched != CoverTable.VisibilityEstimate.Certain))
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = a.baseCharacter.LowProfile();
			bool flag4 = b.baseCharacter.LowProfile();
			if (aVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Impossible)
			{
				bool flag5 = a.awareness.WouldCrouchToAvoidBeingSpotted(b);
				if (flag3 || flag5)
				{
					possibleresult = true;
					if (flag5)
					{
						ForcePendingCrouch(a, b);
					}
					goto IL_0206;
				}
				flag = true;
			}
			if (bVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Impossible)
			{
				bool flag6 = b.awareness.WouldCrouchToAvoidBeingSpotted(a);
				if (flag4 || flag6)
				{
					possibleresult = true;
					if (flag6)
					{
						ForcePendingCrouch(b, a);
					}
					goto IL_0206;
				}
				flag2 = true;
			}
			if (!flag || !flag2)
			{
				Vector3 position = a.GetPosition();
				Vector3 position2 = b.GetPosition();
				if (flag)
				{
					if (bVisibilityWhenCrouched != CoverTable.VisibilityEstimate.Certain)
					{
						position += a.awareness.standingEyeLevel;
						possibleresult = CheckForSuccessfulCrouch_OneOnly(position2, position, b, a, false);
					}
				}
				else if (flag2)
				{
					if (aVisibilityWhenCrouched != CoverTable.VisibilityEstimate.Certain)
					{
						position2 += b.awareness.standingEyeLevel;
						possibleresult = CheckForSuccessfulCrouch_OneOnly(position, position2, a, b, false);
					}
				}
				else if (bVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Certain)
				{
					position2 += b.awareness.standingEyeLevel;
					possibleresult = CheckForSuccessfulCrouch_OneOnly(position, position2, a, b, false);
				}
				else if (aVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Certain)
				{
					position += a.awareness.standingEyeLevel;
					possibleresult = CheckForSuccessfulCrouch_OneOnly(position2, position, b, a, false);
				}
				else
				{
					possibleresult = CheckForSuccessfulCrouch_Either(position, position2, a, b, false);
				}
			}
		}
		goto IL_0206;
		IL_0206:
		inCrowds[a.quickIndex].SetObstructed(b.ident, possibleresult);
		inCrowds[b.quickIndex].SetObstructed(a.ident, possibleresult);
	}

	private GameObject SpawnDebugLOSBox(Vector3 p1, Vector3 p2)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Object.DestroyImmediate(gameObject.GetComponent<BoxCollider>());
		gameObject.transform.localScale = new Vector3(0.01f, 0.01f, (p1 - p2).magnitude);
		gameObject.transform.position = (p1 + p2) * 0.5f;
		gameObject.transform.forward = p1 - p2;
		return gameObject;
	}

	private void CheckStanceVisibility_Possible(Actor a, Actor b, CoverTable.VisibilityEstimate aVisibilityWhenCrouched, CoverTable.VisibilityEstimate bVisibilityWhenCrouched)
	{
		bool flag = false;
		if ((a.behaviour.PlayerControlled && b.awareness.KnowWhereabouts(a)) || (b.behaviour.PlayerControlled && a.awareness.KnowWhereabouts(b)))
		{
			flag = Obstructed(a.awareness, b.awareness);
		}
		else
		{
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = a.baseCharacter.LowProfile();
			bool flag5 = b.baseCharacter.LowProfile();
			if (aVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Impossible)
			{
				bool flag6 = a.awareness.WouldCrouchToAvoidBeingSpotted(b);
				if (flag4 || flag6)
				{
					flag = true;
					if (flag6)
					{
						ForcePendingCrouch(a, b);
					}
					goto IL_0183;
				}
				flag2 = true;
			}
			if (bVisibilityWhenCrouched == CoverTable.VisibilityEstimate.Impossible)
			{
				bool flag7 = b.awareness.WouldCrouchToAvoidBeingSpotted(a);
				if (flag5 || flag7)
				{
					flag = true;
					if (flag7)
					{
						ForcePendingCrouch(b, a);
					}
					goto IL_0183;
				}
				flag3 = true;
			}
			if (flag2 && flag3)
			{
				flag = Obstructed(a.awareness, b.awareness);
			}
			else
			{
				Vector3 position = a.GetPosition();
				Vector3 position2 = b.GetPosition();
				if (flag2)
				{
					position += a.awareness.standingEyeLevel;
					flag = CheckForSuccessfulCrouch_OneOnly(position2, position, b, a, true);
				}
				else if (flag3)
				{
					position2 += NewCoverPointManager.standingHeightOffset;
					flag = CheckForSuccessfulCrouch_OneOnly(position, position2, a, b, true);
				}
				else
				{
					flag = CheckForSuccessfulCrouch_Either(position, position2, a, b, true);
				}
			}
		}
		goto IL_0183;
		IL_0183:
		inCrowds[a.quickIndex].SetObstructed(b.ident, flag);
		inCrowds[b.quickIndex].SetObstructed(a.ident, flag);
	}

	private bool UpdateInCrowdObstruction(int index)
	{
		Actor actor = actorArray[index];
		AwarenessComponent awareness = actor.awareness;
		uint num = ~actor.ident;
		int inCrowdObstructionBookmark = InCrowdObstructionBookmark;
		for (int i = inCrowdObstructionBookmark; i < activeListLen; i++)
		{
			int num2 = activeList[i];
			if (num2 == index)
			{
				continue;
			}
			obstructionValidationFrameCost++;
			Actor actor2 = actorArray[num2];
			uint num3 = ~actor2.ident;
			AwarenessComponent awareness2 = actor2.awareness;
			uint num4 = (uint)(1 << (int)awareness2.faction);
			float num5 = (awareness.cachedTrans.position - awareness2.cachedTrans.position).sqrMagnitude;
			crouchBecauseOf[index] &= num3;
			crouchBecauseOf[num2] &= num;
			awareness.CrouchObstructionChecked &= num3;
			awareness2.CrouchObstructionChecked &= num;
			CoverTable.VisibilityEstimate myVisibilityCrouched;
			CoverTable.VisibilityEstimate otherVisibilityCrouched;
			CoverTable.VisibilityEstimate visibilityEstimate = awareness.EstimateVisibility(awareness2, out myVisibilityCrouched, out otherVisibilityCrouched);
			if ((FactionBroadcastsTo[(int)awareness.faction] & num4) != 0)
			{
				if (!awareness.AwarenessZonesInSync(awareness2))
				{
					num5 = float.MaxValue;
				}
				awareness.EnableBroadcast(actor2.ident, actor.behaviour.InActiveAlertState() && num5 < awareness2.broadcastPerceptionRangeSqr);
				awareness2.EnableBroadcast(actor.ident, actor2.behaviour.InActiveAlertState() && num5 < awareness.broadcastPerceptionRangeSqr);
			}
			else
			{
				awareness.EnableBroadcast(actor2.ident, false);
				awareness2.EnableBroadcast(actor.ident, false);
				if (((awareness.EnemiesIKnowAboutRecent & actor2.ident) | (awareness2.EnemiesIKnowAboutRecent & actor.ident)) != 0)
				{
					TargetScorer.CalculateDistanceScores(actor, actor2, num5);
				}
			}
			LastLOSTime[index, num2] = Time.time;
			LastLOSTime[num2, index] = Time.time;
			switch (visibilityEstimate)
			{
			case CoverTable.VisibilityEstimate.Certain:
				CheckStanceVisibility_Certain(actor, actor2, myVisibilityCrouched, otherVisibilityCrouched);
				break;
			case CoverTable.VisibilityEstimate.Impossible:
				inCrowds[index].SetObstructed(actor2.ident, true);
				inCrowds[num2].SetObstructed(actor.ident, true);
				break;
			case CoverTable.VisibilityEstimate.Possible:
			{
				bool possibleresult;
				if (MustCheckAccurately(actor, actor2, out possibleresult))
				{
					if (NewCoverPointManager.Instance().baked.coverTable.uncertaintyMapInvalid && Obstructed(awareness, awareness2))
					{
						inCrowds[index].SetObstructed(actor2.ident, true);
						inCrowds[num2].SetObstructed(actor.ident, true);
					}
					else
					{
						CheckStanceVisibility_Possible(actor, actor2, myVisibilityCrouched, otherVisibilityCrouched);
					}
				}
				else
				{
					inCrowds[index].SetObstructed(actor2.ident, possibleresult);
					inCrowds[num2].SetObstructed(actor.ident, possibleresult);
				}
				break;
			}
			}
			if (i < activeListLen - 1)
			{
				InCrowdObstructionBookmark = i + 1;
				if (obstructionValidationFrameCost >= kObstructionValidationMaxCost)
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool Obstructed(Vector3 traceStart, Vector3 traceEnd)
	{
		traces++;
		obstructionValidationFrameCost += 200;
		Vector3 collision;
		return !WorldHelper.IsClearTrace(traceStart, traceEnd, out collision);
	}

	private bool Obstructed(AwarenessComponent ac1, AwarenessComponent ac2)
	{
		Vector3 position = ac1.cachedTrans.position;
		position.y += ac1.standingEyeLevel.y;
		Vector3 position2 = ac2.cachedTrans.position;
		position2.y += ac2.standingEyeLevel.y;
		traces++;
		obstructionValidationFrameCost += 200;
		return !WorldHelper.IsClearTrace(position, position2);
	}

	public void RefreshFactionEnemyMask(FactionHelper.Category faction)
	{
		uint num = 1u;
		uint num2 = (uint)(1 << (int)faction);
		enemyMask[(int)faction] = 0u;
		for (int i = 0; i < 32; i++)
		{
			if ((identsInUse & num) != 0)
			{
				AwarenessComponent awareness = actorArray[i].awareness;
				if ((FactionHostileTo[(int)awareness.faction] & num2) != 0)
				{
					enemyMask[(int)faction] |= num;
				}
			}
			num += num;
		}
	}

	private void CheckForSpottingAndHearing()
	{
		aliveMask = 0u;
		upAndAboutMask = 0u;
		selectableMask = 0u;
		for (int i = 0; i < activeListLen; i++)
		{
			int num = activeList[i];
			actorArray[num].awareness.InitForSpotCheck();
			inCrowds[num].InitForSpotCheck();
			inCrowds[num].upToDate = (inCrowds[num].LastKnownPosition - actorArray[num].GetPosition()).sqrMagnitude < 9f;
		}
		globalAlertness = 0u;
		for (int num = 0; num < 8; num++)
		{
			globalVision[num] = 0u;
			factionMask[num] = 0u;
			enemyMask[num] = 0u;
		}
		for (int i = 0; i < activeListLen - 1; i++)
		{
			int num = activeList[i];
			uint num2 = (uint)(1 << num);
			AwarenessComponent awareness = actorArray[num].awareness;
			uint obstructed = inCrowds[num].obstructed;
			for (int j = i + 1; j < activeListLen; j++)
			{
				int num3 = activeList[j];
				uint num4 = (uint)(1 << num3);
				AwarenessComponent awareness2 = actorArray[num3].awareness;
				if ((FactionHostileTo[(int)awareness.faction] & (uint)(1 << (int)awareness2.faction)) == 0)
				{
					if ((obstructed & num4) == 0)
					{
						inCrowds[num].friendlyMembers |= num4;
						inCrowds[num3].friendlyMembers |= num2;
						if (awareness2.InFOV(awareness) != 0)
						{
							awareness.EyesOnMe |= num4;
							awareness2.FriendsICanSee |= num2;
						}
						if (awareness.InFOV(awareness2) != 0)
						{
							awareness2.EyesOnMe |= num2;
							awareness.FriendsICanSee |= num4;
						}
					}
					continue;
				}
				enemyMask[(int)awareness.faction] |= num4;
				enemyMask[(int)awareness2.faction] |= num2;
				float sqrMagnitude = (awareness.cachedTrans.position - awareness2.cachedTrans.position).sqrMagnitude;
				if (awareness2.CanHear(awareness, sqrMagnitude))
				{
					awareness2.ICanHear |= num2;
					inCrowds[num].futureHostileMembers |= num4 | awareness2.BroadcastsTo;
				}
				if (awareness.CanHear(awareness2, sqrMagnitude))
				{
					awareness.ICanHear |= num4;
					inCrowds[num3].futureHostileMembers |= num2 | awareness.BroadcastsTo;
				}
				if ((obstructed & num4) == 0)
				{
					InFOVResult inFOVResult = awareness2.InFOV(awareness);
					if (inFOVResult == InFOVResult.YesIfStanding)
					{
						bool flag = awareness.myActor.baseCharacter.LowProfile();
						bool flag2 = awareness.WouldCrouchToAvoidBeingSpotted(awareness2.myActor);
						if (flag || flag2)
						{
							inFOVResult = InFOVResult.No;
							if (flag2)
							{
								crouchBecauseOf[num] |= num4;
							}
						}
						else
						{
							inFOVResult = InFOVResult.Yes;
						}
					}
					if (inFOVResult == InFOVResult.Yes)
					{
						crouchBecauseOf[num] &= ~num4;
						inCrowds[num].futureHostileMembers |= num4 | awareness2.BroadcastsTo;
						awareness.EyesOnMe |= num4;
						awareness2.EnemiesICanSee |= num2;
						if (sqrMagnitude < awareness2.closestVisibleEnemyDistSqr && !awareness.myActor.realCharacter.IsDead() && !awareness.myActor.realCharacter.IsMortallyWounded())
						{
							awareness2.closestVisibleEnemyDistSqr = sqrMagnitude;
							awareness2.closestVisibleEnemy = awareness.myActor;
						}
					}
					InFOVResult inFOVResult2 = awareness.InFOV(awareness2);
					if (inFOVResult2 == InFOVResult.YesIfStanding)
					{
						bool flag3 = awareness2.myActor.baseCharacter.LowProfile();
						bool flag4 = awareness2.WouldCrouchToAvoidBeingSpotted(awareness.myActor);
						if (flag3 || flag4)
						{
							inFOVResult2 = InFOVResult.No;
							if (flag4)
							{
								crouchBecauseOf[num3] |= num2;
							}
						}
						else
						{
							inFOVResult2 = InFOVResult.Yes;
						}
					}
					if (inFOVResult2 == InFOVResult.Yes)
					{
						crouchBecauseOf[num3] &= ~num2;
						inCrowds[num3].futureHostileMembers |= num2 | awareness.BroadcastsTo;
						awareness2.EyesOnMe |= num2;
						awareness.EnemiesICanSee |= num4;
						if (sqrMagnitude < awareness.closestVisibleEnemyDistSqr && !awareness2.myActor.realCharacter.IsDead() && !awareness2.myActor.realCharacter.IsMortallyWounded())
						{
							awareness.closestVisibleEnemyDistSqr = sqrMagnitude;
							awareness.closestVisibleEnemy = awareness2.myActor;
						}
					}
					if (((awareness.EnemiesICanSee | awareness.EnemiesIKnowAboutRecent) & num4) != 0 && sqrMagnitude < awareness.closestUnobstructedEnemyDistSqr && !awareness2.myActor.realCharacter.IsDead() && !awareness2.myActor.realCharacter.IsMortallyWounded())
					{
						awareness.closestUnobstructedEnemyDistSqr = sqrMagnitude;
						awareness.closestUnobstructedEnemy = awareness2.myActor;
					}
					if (((awareness2.EnemiesICanSee | awareness2.EnemiesIKnowAboutRecent) & num2) != 0 && sqrMagnitude < awareness2.closestUnobstructedEnemyDistSqr && !awareness.myActor.realCharacter.IsDead() && !awareness.myActor.realCharacter.IsMortallyWounded())
					{
						awareness2.closestUnobstructedEnemyDistSqr = sqrMagnitude;
						awareness2.closestUnobstructedEnemy = awareness.myActor;
					}
				}
				else
				{
					if ((crouchBecauseOf[num] & num4) != 0 && awareness2.InFOV(awareness) == InFOVResult.No)
					{
						crouchBecauseOf[num] &= ~num4;
					}
					if ((crouchBecauseOf[num3] & num2) != 0 && awareness.InFOV(awareness2) == InFOVResult.No)
					{
						crouchBecauseOf[num3] &= ~num2;
					}
				}
			}
		}
		for (int i = 0; i < activeListLen; i++)
		{
			int num = activeList[i];
			if (crouchBecauseOf[num] != 0)
			{
				ApplyPendingCrouch(actorArray[num]);
			}
		}
		uint num5 = 0u;
		playerControlledMask = 0u;
		for (int i = 0; i < 32; i++)
		{
			knowledgeMatrix[i] = 0u;
		}
		for (int num = 0; num < 9; num++)
		{
			characterTypeMask[num] = 0u;
		}
		for (int i = 0; i < activeListLen; i++)
		{
			int num = activeList[i];
			AwarenessComponent awareness3 = actorArray[num].awareness;
			uint num2 = (uint)(1 << num);
			num5 |= num2;
			if (inCrowds[num].futureHostileMembers != 0)
			{
				inCrowds[num].updateLocationUntil = Time.time + 2f;
			}
			if (Time.time <= inCrowds[num].updateLocationUntil)
			{
				inCrowds[num].LastKnownPosition = awareness3.cachedTrans.position;
				inCrowds[num].lastSeenTime = WorldHelper.ThisFrameTime;
				if (awareness3.closestCoverPoint != null)
				{
					inCrowds[num].lastClosestCoverPoint = awareness3.closestCoverPoint.index;
				}
				else
				{
					inCrowds[num].lastClosestCoverPoint = -1;
				}
				inCrowds[num].wasInCover = awareness3.isInCover;
			}
			inCrowds[num].hostileMembers |= inCrowds[num].futureHostileMembers;
			globalAlertness |= inCrowds[num].hostileMembers;
			knowledgeMatrix[31 - num] = inCrowds[num].futureHostileMembers;
			characterTypeMask[(int)awareness3.ChDefCharacterType] |= num2;
		}
		TransposeKnowledgeMatrix();
		for (int i = 0; i < activeListLen; i++)
		{
			int num = activeList[i];
			uint num2 = (uint)(1 << num);
			AwarenessComponent awareness4 = actorArray[num].awareness;
			awareness4.EnemiesIKnowAboutFresh = knowledgeMatrix[31 - num] & num5;
			awareness4.ICanSee = awareness4.FriendsICanSee | awareness4.EnemiesICanSee;
			int faction = (int)awareness4.faction;
			globalVision[faction] |= awareness4.ICanSee;
			factionMask[faction] |= num2;
			if (awareness4.myActor.behaviour.PlayerControlled)
			{
				playerControlledMask |= num2;
			}
		}
		globalAlertness &= num5;
		for (int num = 0; num < 8; num++)
		{
			globalVision[num] &= num5;
		}
	}

	private void TransposeKnowledgeMatrix()
	{
		uint num = 65535u;
		int num2 = 16;
		while (num2 != 0)
		{
			for (int num3 = 0; num3 < 32; num3 = (num3 + num2 + 1) & ~num2)
			{
				uint num4 = (knowledgeMatrix[num3] ^ (knowledgeMatrix[num3 + num2] >> num2)) & num;
				knowledgeMatrix[num3] ^= num4;
				knowledgeMatrix[num3 + num2] = knowledgeMatrix[num3 + num2] ^ (num4 << num2);
			}
			num2 >>= 1;
			num ^= num << num2;
		}
	}

	public Actor GetActorFromIndex(int i)
	{
		return actorArray[i];
	}

	public Actor GetActorFromIdent(uint ident)
	{
		return actorArray[IndexFromIdent(ident)];
	}

	public void RegisterActor(Actor ac)
	{
		uint num = 1u;
		for (int i = 0; i < 32; i++)
		{
			if ((identsInUse & num) == 0)
			{
				inCrowds[i].lastClosestCoverPoint = -1;
				inCrowds[i].wasInCover = false;
				inCrowds[i].friendlyMembers = 0u;
				inCrowds[i].hostileMembers = 0u;
				inCrowds[i].futureHostileMembers = 0u;
				inCrowds[i].obstructed = uint.MaxValue;
				actorArray[i] = ac;
				identsInUse |= num;
				ac.ident = num;
				ac.quickIndex = i;
				activeList.Add(i);
				activeList.Sort();
				activeListLen++;
				for (int j = 0; j < 32; j++)
				{
					if (i != j)
					{
						inCrowds[j].obstructed |= num;
					}
				}
				return;
			}
			num += num;
		}
		TBFAssert.DoAssert(false);
	}

	public void RemoveActor(Actor ac)
	{
		uint num = ~ac.ident;
		identsInUse &= num;
		for (int i = 0; i < 8; i++)
		{
			factionMask[i] &= num;
			globalVision[i] &= num;
			enemyMask[i] &= num;
		}
		crouchBecauseOf[ac.quickIndex] = 0u;
		persistentMask &= num;
		aliveMask &= num;
		playerControlledMask &= num;
		for (int i = 0; i < 32; i++)
		{
			inCrowds[i].obstructed |= ac.ident;
			inCrowds[i].friendlyMembers &= num;
			inCrowds[i].hostileMembers &= num;
			inCrowds[i].futureHostileMembers &= num;
			crouchBecauseOf[i] &= num;
		}
		for (int i = 0; i < 9; i++)
		{
			characterTypeMask[i] &= num;
		}
		TargetScorer.Expunge(ac.quickIndex);
		actorArray[ac.quickIndex] = null;
		activeList.Remove(ac.quickIndex);
		activeList.Sort();
		activeListLen--;
		ActorMask.Expunge(num);
		if (NewCoverPointManager.Instance() != null)
		{
			NewCoverPointManager.Instance().Expunge(num);
		}
	}

	private int IndexFromIdent(uint ident)
	{
		int num = 0;
		if ((ident & 0xFFFF0000u) != 0)
		{
			num += 16;
		}
		if ((ident & 0xFF00FF00u) != 0)
		{
			num += 8;
		}
		if ((ident & 0xF0F0F0F0u) != 0)
		{
			num += 4;
		}
		if ((ident & 0xCCCCCCCCu) != 0)
		{
			num += 2;
		}
		if ((ident & 0xAAAAAAAAu) != 0)
		{
			num++;
		}
		return num;
	}

	public int UnitCount(uint x)
	{
		x -= (x >> 1) & 0x55555555;
		x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
		x = ((x >> 4) + x) & 0xF0F0F0Fu;
		x += x >> 8;
		x += x >> 16;
		return (int)(x & 0x3F);
	}

	public void EjectMembersFromZone(uint zid)
	{
		uint num = 1u;
		for (int i = 0; i < 32; i++)
		{
			if ((identsInUse & num) != 0)
			{
				actorArray[i].awareness.LeaveAwarenessZone(zid);
			}
			num += num;
		}
	}

	public void GlDebugVisualise(bool forceShow)
	{
		GL.PushMatrix();
		DebugDraw.LineMaterial.SetPass(0);
		GL.Color(Color.blue);
		GL.Begin(1);
		Actor[] array = actorArray;
		foreach (Actor actor in array)
		{
			if (actor != null)
			{
				actor.awareness.GLDebugVisualise(forceShow);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	public void AwarenessLog(string s)
	{
	}

	public void AwarenessLog(string s, Actor a, Actor b)
	{
	}
}
