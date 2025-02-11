using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class GameplayController : NanniedBehaviour
{
	public delegate void OnDeathEventHandler(object sender, HealthComponent.HeathChangeEventArgs hce);

	public delegate void OnGrenadeEventHandler(object sender);

	public delegate void OnShootOrderEventHandler(object sender);

	public delegate void OnPlayerCharacterDeadEventHandler(object sender);

	public delegate void OnEnemyCharacterDeadEventHandler(object sender);

	public delegate void OnPlayerCharacterAboutToBeMortallyWoundedEventHandler(Actor actor);

	public delegate void OnPlayerCharacterHealedEventHandler(Actor actor);

	public delegate void OnEnemyAlarmedEventHandler(object sender);

	public delegate void OnEnemyReactEventHandler(object sender);

	public delegate void OnCameraAlarmSoundedEventHandler(object sender);

	public delegate void OnPanelAlarmSoundedEventHandler(object sender);

	public delegate void OnFingerDragEnd(object sender);

	public const float NormalMovementCoverSearchRadius = 2f;

	public const float FirstToThirdPersonCoverSearchRadius = 2f;

	private const float dragToContextMenuDelay = 0.4f;

	public GameObject debugHitThing;

	public static GameplayController instance;

	public float MultiSelectTimeout = 1f;

	public ContextObjectBlip ContextObjBlip;

	public DropZoneBlip DropZoneObjBlip;

	public DebugBlip DebugBlipObj;

	public bool ShowDebugBlips;

	public float FormationSpacingColumn = 1f;

	public float FormationSpacingRow = 1f;

	public float FormationCoverSearchRadius = 5f;

	public float UnitCoverSearchRadius = 3f;

	public ScriptedSequence GameplayStartupSequence;

	public List<AnimationClip> SpecialCaseAnimations = new List<AnimationClip>();

	private int smMoveToHitLayerMask = 1;

	private float mPreviewShowTimer;

	private Vector3 mWorldMouseClick;

	private bool mWorldMouseClickValid;

	private SelectableObject dragSelection;

	private SelectableObject underDragSelection;

	private float dragToContextMenuTimer = -1f;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private bool mBufferDrag;

	private SelectableObject mBufferDragSelection;

	private Vector2 mBufferDragStartPos;

	private Vector2 mGhostDragOffset;

	private Vector2 mDoubleTapFirstTapPos = Vector2.zero;

	private Vector2 mDoubleTapSecondTapPos = Vector2.zero;

	private SelectableObject mDoubleTapFirstTapObject;

	private SelectableObject mDoubleTapSecondTapObject;

	private bool mPlacingClaymore;

	private bool mPlacingBody;

	private bool mInputEnabled;

	private bool mCameraAlarmSounded;

	private bool mPanelAlarmSounded;

	private ClaymoreLayingComponent mClaymoreLayingComponent;

	private BodyDroppingComponent mBodyDroppingComponent;

	private GameObject mFirstScriptTriggerActor;

	private bool mTapOrderBuffered;

	private Vector2 mBufferedFingerPos;

	private BaseCharacter.MovementStyle mBufferedMovementSpeed;

	private bool mContextMenuInteractionBuffered;

	private Vector2 mContextMenuInteractionPositionBuffered;

	private float mContextMenuInteractionTimeBuffered;

	private Vector3 mGrenadeTargetPosition;

	private bool mGrenadeTargetPositionBuffered;

	private Vector3 mClaymoreTargetPosition;

	private bool mClaymoreTargetPositionBuffered;

	private Vector3 mBodyTargetPosition;

	private bool mBodyTargetPositionBuffered;

	private List<Actor> mSelected;

	private float mSelectedChangedTime;

	private bool mWaypointMarkerDragMode;

	private GameObject mWaypointMarkerToDragOwner;

	private bool mSelectionChanged;

	public bool PlacingClaymore
	{
		get
		{
			return mPlacingClaymore;
		}
	}

	public bool PlacingBody
	{
		get
		{
			return mPlacingBody;
		}
	}

	public bool CameraAlarmSounded
	{
		get
		{
			return mCameraAlarmSounded;
		}
		set
		{
			mCameraAlarmSounded = value;
			if (mCameraAlarmSounded && this.OnCameraAlarmSounded != null)
			{
				this.OnCameraAlarmSounded(this);
			}
		}
	}

	public bool PanelAlarmSounded
	{
		get
		{
			return mPanelAlarmSounded;
		}
		set
		{
			mPanelAlarmSounded = value;
			if (mPanelAlarmSounded && this.OnPanelAlarmSounded != null)
			{
				this.OnPanelAlarmSounded(this);
			}
		}
	}

	public bool SettingClaymore
	{
		get
		{
			return mClaymoreLayingComponent != null;
		}
	}

	public GameObject FirstScriptTriggerActor
	{
		get
		{
			return mFirstScriptTriggerActor;
		}
		set
		{
			mFirstScriptTriggerActor = value;
		}
	}

	public float MultiSelectModeTime
	{
		get
		{
			return Time.realtimeSinceStartup - mSelectedChangedTime;
		}
	}

	public List<Actor> Selected
	{
		get
		{
			return mSelected;
		}
	}

	public Actor SelectedLeader
	{
		get
		{
			if (Selected.Count > 0)
			{
				return mSelected[0];
			}
			return null;
		}
		set
		{
			if (Selected.Contains(value))
			{
				int index = mSelected.IndexOf(value);
				Actor value2 = mSelected[0];
				mSelected[0] = mSelected[index];
				mSelected[index] = value2;
				MarkNooneAsLeader();
				value.behaviour.SelectedMarkerObj.MarkAsLeader(true);
			}
		}
	}

	public List<Actor> ActorsOtherThanFPP
	{
		get
		{
			if (!GameController.Instance.IsFirstPerson)
			{
				return mSelected;
			}
			List<Actor> list = new List<Actor>();
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if (a.realCharacter.IsSelectable() && !a.baseCharacter.IsInASetPiece && a != GameController.Instance.mFirstPersonActor)
				{
					list.Add(a);
				}
			}
			return list;
		}
	}

	public event OnDeathEventHandler OnDeath;

	public event OnGrenadeEventHandler OnGrenade;

	public event OnShootOrderEventHandler OnShootOrder;

	public event OnPlayerCharacterDeadEventHandler OnPlayerCharacterDead;

	public event OnEnemyCharacterDeadEventHandler OnEnemyCharacterDead;

	public event OnPlayerCharacterAboutToBeMortallyWoundedEventHandler OnPlayerCharacterAboutToBeMortallyWounded;

	public event OnPlayerCharacterHealedEventHandler OnPlayerCharacterHealed;

	public event OnEnemyAlarmedEventHandler OnEnemyAlarmed;

	public event OnEnemyReactEventHandler OnEnemyReact;

	public event OnCameraAlarmSoundedEventHandler OnCameraAlarmSounded;

	public event OnPanelAlarmSoundedEventHandler OnPanelAlarmSounded;

	public event OnFingerDragEnd OnFingerDragEndClick;

	public static GameplayController Instance()
	{
		return instance;
	}

	public void FireAlarmEvents(Actor actor)
	{
		if (this.OnEnemyAlarmed != null)
		{
			this.OnEnemyAlarmed(actor);
		}
	}

	public void FireOnReactEvents(Actor actor)
	{
		if (this.OnEnemyReact != null)
		{
			this.OnEnemyReact(actor);
		}
	}

	public void HideGhostPreview(Actor a)
	{
		WaypointMarkerManager.Instance.RemoveMarker(a.gameObject);
		QueuedOrder.DestroyOrders(a);
	}

	private bool IsPreviewGhost(WaypointMarker wpm)
	{
		return wpm.owner.GetComponent<Actor>() == null;
	}

	private void Awake()
	{
		instance = this;
		mSelected = new List<Actor>();
		mInputEnabled = false;
		mSelectionChanged = false;
		NannyMe();
		if ((bool)UIManager.instance)
		{
			UIManager uIManager = UIManager.instance;
			uIManager.ShowDebugPress = (UIManager.ShowDebugPressDelegate)Delegate.Combine(uIManager.ShowDebugPress, new UIManager.ShowDebugPressDelegate(ShowDebugPress));
		}
		smMoveToHitLayerMask = 1 << LayerMask.NameToLayer("Default");
	}

	public void EnableInput()
	{
		if (!mInputEnabled)
		{
			InputManager.Instance.AddOnFingerTapEventHandler(Screen_OnTap, 10);
			InputManager.Instance.AddOnFingerDoubleTapEventHandler(Screen_OnDoubleTap, 10);
			InputManager.Instance.AddOnFingerUpEventHandler(Screen_OnFingerUp, 10);
			InputManager.Instance.AddOnFingerDragBeginEventHandler(Screen_OnFingerDragBegin, 10);
			InputManager.Instance.AddOnFingerDragMoveEventHandler(Screen_OnFingerDragMove, 10);
			InputManager.Instance.AddOnFingerDragEndEventHandler(Screen_OnFingerDragEnd, 10);
			mInputEnabled = true;
		}
	}

	public void DisableInput()
	{
		if (mInputEnabled)
		{
			InputManager.Instance.RemoveOnFingerTapEventHandler(Screen_OnTap);
			InputManager.Instance.RemoveOnFingerDoubleTapEventHandler(Screen_OnDoubleTap);
			InputManager.Instance.RemoveOnFingerUpEventHandler(Screen_OnFingerUp);
			InputManager.Instance.RemoveOnFingerDragBeginEventHandler(Screen_OnFingerDragBegin);
			InputManager.Instance.RemoveOnFingerDragMoveEventHandler(Screen_OnFingerDragMove);
			InputManager.Instance.RemoveOnFingerDragEndEventHandler(Screen_OnFingerDragEnd);
			mInputEnabled = false;
		}
	}

	private void OnDestroy()
	{
		instance = null;
		if (UIManager.Exists())
		{
			UIManager uIManager = UIManager.instance;
			uIManager.ShowDebugPress = (UIManager.ShowDebugPressDelegate)Delegate.Remove(uIManager.ShowDebugPress, new UIManager.ShowDebugPressDelegate(ShowDebugPress));
		}
	}

	private void Start()
	{
		ResetState();
		debugHitThing = null;
		ImpactSFX impactSFX = ImpactSFX.Instance;
		VocalFriendlySFX vocalFriendlySFX = VocalFriendlySFX.Instance;
		WeaponSFX weaponSFX = WeaponSFX.Instance;
		mBufferDrag = false;
	}

	private void Update()
	{
		ValidateSquadSelection();
		if (mGrenadeTargetPositionBuffered)
		{
			InitiateBufferedReleaseOrderThrowGrenade(mGrenadeTargetPosition);
			mGrenadeTargetPositionBuffered = false;
			mTapOrderBuffered = false;
		}
		else if (mClaymoreTargetPositionBuffered)
		{
			PlaceClaymore(mClaymoreTargetPosition);
			mClaymoreTargetPositionBuffered = false;
		}
		else if (mBodyTargetPositionBuffered)
		{
			PlaceBody(mBodyTargetPosition);
			mBodyTargetPositionBuffered = false;
		}
		else if (mTapOrderBuffered)
		{
			InitiateBufferedTapOrder();
			mTapOrderBuffered = false;
		}
		if (mContextMenuInteractionBuffered && mContextMenuInteractionTimeBuffered <= Time.time)
		{
			GameController.ContextMenuLogic.ForceFingerDown(0, mContextMenuInteractionPositionBuffered);
			mContextMenuInteractionBuffered = false;
		}
		if (mSelectionChanged)
		{
			if (GameController.Instance != null)
			{
				bool flag = false;
				foreach (Actor item in mSelected)
				{
					if (item == null)
					{
						continue;
					}
					if (item.tasks.IsRunningTask<TaskEnter>())
					{
						if (IsSelectedLeader(item))
						{
							flag = false;
							break;
						}
					}
					else
					{
						flag = true;
					}
				}
				if (GameController.Instance.IsFirstPerson && !GameController.Instance.IsLockedToFirstPerson)
				{
					GameController.Instance.ZoomOutAvailable = flag;
				}
				else if (GameController.Instance.AllowFirstPersonAtAnyPoint)
				{
					GameController.Instance.ZoomOutAvailable = false;
					GameController.Instance.ZoomInAvailable = flag;
				}
			}
			SelectionChanged();
			mSelectionChanged = false;
		}
		if (underDragSelection != null)
		{
			if (dragToContextMenuTimer != -1f)
			{
				if (!(Time.time > dragToContextMenuTimer))
				{
				}
			}
			else
			{
				underDragSelection = null;
			}
		}
		Spawner.SpawnedEnemyThisFrame = false;
	}

	public void LevelStarted(object sender)
	{
		if (GameplayStartupSequence != null)
		{
			GameplayStartupSequence.StartSequence();
		}
	}

	public void ResetState()
	{
		mTapOrderBuffered = false;
		mGrenadeTargetPositionBuffered = false;
		mClaymoreTargetPositionBuffered = false;
		mBodyTargetPositionBuffered = false;
		mWaypointMarkerDragMode = false;
	}

	public bool IsSelected(Actor actor)
	{
		return mSelected.Contains(actor);
	}

	public bool IsSelectedLeader(Actor actor)
	{
		if (mSelected.Count > 0)
		{
			return SelectedLeader == actor;
		}
		return false;
	}

	public bool IsAnySelectedCarryingBody()
	{
		foreach (Actor item in mSelected)
		{
			if (item.realCharacter.Carried != null)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAnySelectedCarryingAEnemy()
	{
		foreach (Actor item in mSelected)
		{
			if (item.realCharacter.Carried != null && item.realCharacter.Carried.awareness.faction != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAnySelectedMovingByPlayerRequest()
	{
		foreach (Actor item in mSelected)
		{
			TaskRouteTo runningTask = item.tasks.GetRunningTask<TaskRouteTo>();
			if (runningTask != null && (runningTask.ConfigFlags & Task.Config.IssuedByPlayerRequest) != 0)
			{
				return true;
			}
			TaskMoveToCover runningTask2 = item.tasks.GetRunningTask<TaskMoveToCover>();
			if (runningTask2 != null && (runningTask2.ConfigFlags & Task.Config.IssuedByPlayerRequest) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool AnySelectedAllowedCMOption(ContextMenuOptionType cmOptionType)
	{
		foreach (Actor item in mSelected)
		{
			if (cmOptionType == ContextMenuOptionType.HideBody)
			{
				return true;
			}
			TaskDropClaymore taskDropClaymore = item.tasks.GetRunningTask(typeof(TaskDropClaymore)) as TaskDropClaymore;
			if (taskDropClaymore != null)
			{
				return false;
			}
			if (item.realCharacter.Carried == null && (item.realCharacter.CMRules & cmOptionType) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool AnySelectedAllowedCMOptionAndCanReach(ContextMenuOptionType cmOptionType, Actor target)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		foreach (Actor item in mSelected)
		{
			if (cmOptionType == ContextMenuOptionType.HideBody)
			{
				return true;
			}
			TaskDropClaymore taskDropClaymore = item.tasks.GetRunningTask(typeof(TaskDropClaymore)) as TaskDropClaymore;
			if (taskDropClaymore != null)
			{
				return false;
			}
			if (item.realCharacter.Carried == null && (item.realCharacter.CMRules & cmOptionType) != 0 && NavMesh.CalculatePath(item.GetPosition(), target.GetPosition(), item.navAgent.walkableMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				return true;
			}
		}
		return false;
	}

	public void SelectOnlyThis(Actor actor)
	{
		if (actor.realCharacter.IsSelectable())
		{
			MarkNooneAsLeader();
			mSelected.Clear();
			AddToSelected(actor);
		}
	}

	private void MarkNooneAsLeader()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			a.behaviour.SelectedMarkerObj.MarkAsLeader(false);
		}
	}

	public bool IsSelectionEmpty()
	{
		return mSelected.Count == 0;
	}

	private bool BasicAimedShotTests(Actor a, Actor target)
	{
		if (target == null)
		{
			return false;
		}
		if (target.awareness.closestCoverPoint == null)
		{
			return false;
		}
		if (a.tasks.RunningTaskDeniesPlayerInput)
		{
			return false;
		}
		if (a.realCharacter.IsMortallyWounded())
		{
			return false;
		}
		if ((target.awareness.EnemiesIKnowAbout & a.ident) == 0)
		{
			return true;
		}
		if ((target.awareness.EnemiesIKnowAboutRecent & a.ident) == 0 && !target.awareness.KnowWhereabouts(a))
		{
			return true;
		}
		if ((target.awareness.closestCoverPoint.noCoverAgainst & a.ident) == 0)
		{
			return false;
		}
		return true;
	}

	public bool CanAimedShot(Actor a, Actor target)
	{
		if (!BasicAimedShotTests(a, target))
		{
			return false;
		}
		float sqrMagnitude = (a.GetPosition() - target.GetPosition()).sqrMagnitude;
		return sqrMagnitude < 900f;
	}

	public uint GetSelectedWhoCanAimedShot(Actor target)
	{
		uint num = 0u;
		foreach (Actor item in mSelected)
		{
			if (CanAimedShot(item, target))
			{
				num |= item.ident;
			}
		}
		return num;
	}

	public void SelectAll()
	{
		if (!TutorialToggles.PlayerSelectAllLocked)
		{
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				AddToSelected(a);
			}
			mSelectedChangedTime = 0f;
			mSelectionChanged = true;
		}
	}

	public void AddToSelected(Actor actor)
	{
		if (!actor.behaviour.PlayerControlled)
		{
			TBFAssert.DoAssert(false, "Trying to select NPC for control");
		}
		if (actor.realCharacter.IsSelectable())
		{
			EventOnSelected componentInChildren = actor.GetComponentInChildren<EventOnSelected>();
			if (componentInChildren != null)
			{
				componentInChildren.Selected();
			}
			if (!mSelected.Contains(actor))
			{
				mSelected.Add(actor);
				actor.behaviour.SelectedMarkerObj.MarkAsLeader(mSelected.Count == 1);
			}
			mSelectedChangedTime = Time.realtimeSinceStartup;
			int num = GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask);
			if (num == mSelected.Count && num != 1)
			{
				mSelectedChangedTime = 0f;
			}
			else if (GameController.Instance.mFirstPersonActor != null)
			{
				mSelectedChangedTime = 0f;
			}
			mSelectionChanged = true;
			if (SelectedLeader == actor)
			{
				SelectedLeader.behaviour.SelectedMarkerObj.MarkAsLeader(true);
			}
		}
	}

	public void RemoveFromSelected(Actor act)
	{
		MarkNooneAsLeader();
		if (mSelected.Contains(act))
		{
			mSelected.Remove(act);
		}
		if (SelectedLeader != null)
		{
			SelectedLeader.behaviour.SelectedMarkerObj.MarkAsLeader(true);
		}
		else
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(RealCharacter));
			UnityEngine.Object[] array2 = array;
			foreach (UnityEngine.Object @object in array2)
			{
				RealCharacter realCharacter = @object as RealCharacter;
				if (realCharacter != null)
				{
					Actor myActor = realCharacter.myActor;
					if (myActor != null && myActor.behaviour.PlayerControlled && !realCharacter.IsMortallyWounded() && !realCharacter.IsDead() && realCharacter.IsSelectable())
					{
						mSelected.Add(myActor);
						SelectedLeader = myActor;
						break;
					}
				}
			}
		}
		mSelectedChangedTime = Time.realtimeSinceStartup;
		mSelectionChanged = true;
	}

	public void RemoveFromSelected_NoAutoSelect(Actor act)
	{
		MarkNooneAsLeader();
		if (mSelected.Contains(act))
		{
			mSelected.Remove(act);
		}
		if (SelectedLeader != null)
		{
			SelectedLeader.behaviour.SelectedMarkerObj.MarkAsLeader(true);
		}
		mSelectedChangedTime = Time.realtimeSinceStartup;
		mSelectionChanged = true;
	}

	public bool AreAllAliveSelected(Actor apartFrom)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(RealCharacter));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			RealCharacter realCharacter = @object as RealCharacter;
			if (realCharacter != null)
			{
				Actor myActor = realCharacter.myActor;
				if (myActor != null && myActor != apartFrom && myActor.behaviour.PlayerControlled && !realCharacter.IsDead() && !realCharacter.IsMortallyWounded() && !mSelected.Contains(myActor))
				{
					return false;
				}
			}
		}
		return true;
	}

	public void OnSquadMemberHealed(Actor healedActor)
	{
		if (AreAllAliveSelected(healedActor))
		{
			AddToSelected(healedActor);
		}
	}

	public void AdditionalySelectTarget(Actor actor, bool toggle)
	{
		if (!WorldHelper.IsPlayerControlledActor(actor))
		{
			return;
		}
		if (toggle && IsSelected(actor))
		{
			if (Selected.Count == 1)
			{
				SelectAll();
			}
			else
			{
				RemoveFromSelected(actor);
			}
		}
		else
		{
			AddToSelected(actor);
		}
	}

	public bool IsInMultiSelectMode()
	{
		if (MultiSelectModeTime < MultiSelectTimeout)
		{
			return true;
		}
		return false;
	}

	public ReadOnlyCollection<Actor> GetValidFirstPersonActors()
	{
		List<Actor> list = new List<Actor>();
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.realCharacter.IsSelectable() && a.realCharacter.CanGoFirstPerson())
			{
				list.Add(a);
			}
		}
		return new ReadOnlyCollection<Actor>(list);
	}

	public void SuicideSquad()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.realCharacter.IsMortallyWounded())
			{
				a.health.HealthMinClamped = a.health.HealthMin;
				a.health.Kill("Naturaly Expire", a.gameObject);
			}
			else
			{
				a.health.HealthMinClamped = a.health.HealthMin;
				a.health.Kill("Suicide Pill", a.gameObject);
			}
		}
	}

	public void RegisterActor(Actor actor)
	{
		if (actor.navAgent != null)
		{
			NavGateManager.Instance.SetGatesOnNavAgent(actor.navAgent, !actor.behaviour.PlayerControlled, actor.awareness.faction);
		}
	}

	public void DeregisterActor(Actor actor)
	{
	}

	public Actor GetBestFirstPersonActor()
	{
		if (SelectedLeader != null && SelectedLeader.realCharacter.CanGoFirstPerson())
		{
			return SelectedLeader;
		}
		return null;
	}

	public void BroadcastEventDeath(Actor actor, HealthComponent.HeathChangeEventArgs hce)
	{
		if (this.OnDeath == null)
		{
			return;
		}
		this.OnDeath(actor, hce);
		if (WorldHelper.IsPlayerControlledActor(actor))
		{
			if (this.OnPlayerCharacterDead != null)
			{
				this.OnPlayerCharacterDead(this);
			}
		}
		else if (this.OnEnemyCharacterDead != null)
		{
			this.OnEnemyCharacterDead(this);
		}
	}

	public void BroadcastEventAboutToBeMortallyWounded(Actor actor)
	{
		if (WorldHelper.IsPlayerControlledActor(actor) && this.OnPlayerCharacterAboutToBeMortallyWounded != null)
		{
			this.OnPlayerCharacterAboutToBeMortallyWounded(actor);
		}
	}

	public void BroadcastEventHealed(Actor actor)
	{
		TBFAssert.DoAssert(actor != null, "Should not be called with a null actor");
		if (WorldHelper.IsPlayerControlledActor(actor) && this.OnPlayerCharacterHealed != null)
		{
			this.OnPlayerCharacterHealed(actor);
		}
	}

	public void BroadcastEventGrenade(Actor actor)
	{
		if (this.OnGrenade != null)
		{
			this.OnGrenade(actor);
		}
	}

	public void BroadcastEventShootOrder(Actor actor)
	{
		if (this.OnShootOrder != null)
		{
			this.OnShootOrder(actor);
		}
	}

	private GameObject CreateRingProjector(SoldierMarker sm)
	{
		GameObject rangeRingProjector = EffectsController.Instance.GetRangeRingProjector();
		rangeRingProjector.transform.position = sm.Target.position;
		rangeRingProjector.transform.parent = sm.Target;
		rangeRingProjector.transform.position = rangeRingProjector.transform.position + new Vector3(0f, 2f, 0f);
		return rangeRingProjector;
	}

	private bool IsAllowedToProcessScreenTap()
	{
		if (GUISystem.Instance.m_uiManager.DidAnyPointerHitUI())
		{
			return false;
		}
		if (Time.realtimeSinceStartup - mSelectedChangedTime < 0.1f)
		{
			return false;
		}
		if (GameController.Instance.IsFirstPerson)
		{
			return false;
		}
		VisualiserManager visualiserManager = VisualiserManager.Instance();
		if (visualiserManager != null && visualiserManager.ShouldTakeInput())
		{
			return false;
		}
		if (mPlacingClaymore || mPlacingBody)
		{
			return false;
		}
		return true;
	}

	public bool ProcessUnitSelectLogic(GameObject target)
	{
		InterfaceSFX.Instance.SelectUnit.Play2D();
		if (TutorialToggles.PlayerSelectionLocked)
		{
			return false;
		}
		Actor component = target.GetComponent<Actor>();
		if (!WorldHelper.IsSelectableActor(component))
		{
			return false;
		}
		if (CameraManager.Instance.ActiveCamera != 0)
		{
			return false;
		}
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
		if (playCameraInterface != null)
		{
			if (IsInMultiSelectMode())
			{
				if (IsSelected(component))
				{
					return true;
				}
				AdditionalySelectTarget(component, true);
				WaypointMarkerManager.Instance.MutePreviews(null);
				return true;
			}
			playCameraInterface.FocusAndSelectTarget(target.transform);
			WaypointMarkerManager.Instance.MutePreviews(null);
			return true;
		}
		return false;
	}

	public void CancelAnyPrimedGrenade()
	{
		foreach (Actor item in mSelected)
		{
			if (item.grenadeThrower == null || !item.grenadeThrower.IsPrimed())
			{
				continue;
			}
			item.grenadeThrower.Cancel();
			break;
		}
	}

	private void BufferContextMenuOrder(Vector2 fingerPos)
	{
		mContextMenuInteractionPositionBuffered = fingerPos;
		mContextMenuInteractionBuffered = true;
		mContextMenuInteractionTimeBuffered = Time.time + FingerGestures.Instance.defaultComponents.DoubleTap.MaxDelayBetweenTaps;
	}

	private void ShowDebugPress(Vector3 hudPos)
	{
		Vector2 fingerPos = GUISystem.Instance.m_guiCamera.WorldToScreenPoint(hudPos).xy();
		ShowDebugBlip(fingerPos);
	}

	private void ShowDebugBlip(Vector2 fingerPos)
	{
		if (ShowDebugBlips && DebugBlipObj != null)
		{
			DebugBlip debugBlip = UnityEngine.Object.Instantiate(DebugBlipObj) as DebugBlip;
			debugBlip.ScreenPos = fingerPos;
			debugBlip.Decay(2f);
		}
	}

	private void Screen_OnTap(int fingerIndex, Vector2 fingerPos)
	{
		mDoubleTapFirstTapPos = mDoubleTapSecondTapPos;
		mDoubleTapFirstTapObject = mDoubleTapSecondTapObject;
		mDoubleTapSecondTapPos = fingerPos;
		mDoubleTapSecondTapObject = null;
		if (fingerIndex != 0)
		{
			return;
		}
		ShowDebugBlip(fingerPos);
		SelectableObject selectableObject = (mDoubleTapSecondTapObject = SelectableObject.PickSelectableObject(fingerPos));
		if (GUISystem.Instance.m_uiManager.DidAnyPointerHitUI() || GameController.ContextMenuLogic.HasSelection())
		{
			return;
		}
		if (selectableObject != null)
		{
			if (selectableObject.quickType != SelectableObject.QuickType.EnemySoldier && selectableObject.quickType == SelectableObject.QuickType.PreviewGhost)
			{
				WaypointMarker component = selectableObject.GetComponent<WaypointMarker>();
				WaypointMarkerManager.Instance.HighlightPreviewMarker(component);
			}
		}
		else if (IsAllowedToProcessScreenTap() && TutorialToggles.enableTapToMove && !TutorialToggles.LockToRunOnly)
		{
			float num = 0.2f;
			float num2 = Time.time - FingerGestures.GetFinger(fingerIndex).StarTime;
			if (!(num2 > num))
			{
				InterfaceSFX.Instance.PlaceUnit.Play2D();
				NavMeshCamera.EnablePushing();
				NavMeshCamera.ZeroPush();
				BufferManualTapOrder(fingerPos, BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			}
		}
	}

	private void Screen_OnDoubleTap(int fingerIndex, Vector2 fingerPos)
	{
		mContextMenuInteractionBuffered = false;
		if (fingerIndex != 0 || (mDoubleTapFirstTapObject != mDoubleTapSecondTapObject && (mDoubleTapFirstTapPos - mDoubleTapSecondTapPos).sqrMagnitude > 2500f))
		{
			return;
		}
		GameController.ContextMenuLogic.ClearBufferedContextSelection();
		if (mTapOrderBuffered && !TutorialToggles.LockToWalkOnly)
		{
			mBufferedMovementSpeed = BaseCharacter.MovementStyle.Run;
			return;
		}
		SelectableObject selectableObject = mDoubleTapSecondTapObject ?? mDoubleTapFirstTapObject;
		if (selectableObject != null)
		{
			switch (selectableObject.quickType)
			{
			case SelectableObject.QuickType.PlayerSoldier:
				if (CommonHudController.Instance.ZoomInButton.gameObject.activeInHierarchy)
				{
					Actor component2 = selectableObject.AssociatedObject.GetComponent<Actor>();
					if (component2 != null && !component2.realCharacter.IsMortallyWounded() && !component2.realCharacter.IsDead() && component2.health.IsReviving)
					{
					}
				}
				break;
			case SelectableObject.QuickType.WayPointMarker:
			case SelectableObject.QuickType.PreviewGhost:
				if (!TutorialToggles.LockToWalkOnly)
				{
					MakeSelectedRun();
					mBufferedMovementSpeed = BaseCharacter.MovementStyle.Run;
					mTapOrderBuffered = false;
				}
				break;
			case SelectableObject.QuickType.EnemySoldier:
			{
				Actor component = selectableObject.AssociatedObject.GetComponent<Actor>();
				if (!(component == null) && TutorialToggles.enableDoubleTapAimedShot)
				{
					OrdersHelper.OrderShootAtTarget(this, component, false);
				}
				break;
			}
			}
		}
		else if (TutorialToggles.enableTapToMove && TutorialToggles.LockToRunOnly && IsAllowedToProcessScreenTap())
		{
			InterfaceSFX.Instance.PlaceUnit.Play2D();
			NavMeshCamera.EnablePushing();
			NavMeshCamera.ZeroPush();
			BufferManualTapOrder(fingerPos, BaseCharacter.MovementStyle.Run);
			InitiateBufferedTapOrder();
			MakeSelectedRun();
			mTapOrderBuffered = false;
		}
	}

	private void MakeSelectedRun()
	{
		foreach (Actor item in Selected)
		{
			if (item.realCharacter.StartRunning())
			{
				WaypointMarkerManager.Instance.NowRunning(item.gameObject);
			}
		}
	}

	private bool Screen_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		if (fingerIndex != 0)
		{
			return true;
		}
		if (CameraManager.Instance.ActiveCamera != 0)
		{
			return true;
		}
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
		{
			Vector3 point = hitInfo.point;
			bool flag = false;
			foreach (Actor item in mSelected)
			{
				if (item.grenadeThrower == null || !item.grenadeThrower.IsPrimed())
				{
					continue;
				}
				flag = true;
				break;
			}
			if (flag)
			{
				mGrenadeTargetPosition = point;
				mGrenadeTargetPositionBuffered = true;
			}
			if (mPlacingClaymore)
			{
				mClaymoreTargetPosition = point;
				mClaymoreTargetPositionBuffered = true;
			}
			if (mPlacingBody)
			{
				mBodyTargetPosition = point;
				mBodyTargetPositionBuffered = true;
			}
			return true;
		}
		return true;
	}

	private void Screen_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
		mGhostDragOffset = Vector2.zero;
		if (CameraManager.Instance.ActiveCamera != 0 || fingerIndex != 0 || InputManager.Instance.NumFingersActive != 1)
		{
			return;
		}
		GameController gameController = GameController.Instance;
		if (gameController.GrenadeThrowingModeActive || gameController.ClaymoreDroppingModeActive || gameController.PlacementModeActive)
		{
			return;
		}
		SelectableObject selectableObject = SelectableObject.PickDraggableObject(fingerPos);
		if (selectableObject != null && (selectableObject.quickType == SelectableObject.QuickType.PlayerSoldier || selectableObject.quickType == SelectableObject.QuickType.PreviewGhost || selectableObject.quickType == SelectableObject.QuickType.WayPointMarker))
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.AllowInput(false);
			}
			NavMeshCamera.DisablePushing();
			underDragSelection = null;
			mWaypointMarkerDragMode = true;
			if (selectableObject.quickType == SelectableObject.QuickType.PlayerSoldier)
			{
				mBufferDragSelection = selectableObject;
				mBufferDrag = true;
				mBufferDragStartPos = startPos;
			}
			else
			{
				mBufferDragSelection = selectableObject;
				mBufferDrag = false;
				StartDrag(fingerIndex, fingerPos);
			}
		}
	}

	private void StartDrag(int fingerIndex, Vector2 fingerPos)
	{
		if (!TutorialToggles.enableGhostDrag)
		{
			return;
		}
		SelectableObject selectableObject = mBufferDragSelection;
		switch (selectableObject.quickType)
		{
		case SelectableObject.QuickType.PlayerSoldier:
		{
			Actor component2 = selectableObject.AssociatedObject.GetComponent<Actor>();
			if (!component2.realCharacter.IsDead() && component2.realCharacter.IsSelectable() && !component2.realCharacter.IsMortallyWounded())
			{
				WaypointMarker component = WaypointMarkerManager.Instance.AddMarker(selectableObject.AssociatedObject, selectableObject.transform.position, WaypointMarker.Type.OpenGround, WaypointMarker.State.Walk);
				dragSelection = component.GetComponent<SelectableObject>();
				dragSelection.quickType = SelectableObject.QuickType.PreviewGhost;
				WaypointMarkerManager.Instance.SetGameObjectMarkerFacing(component2.model, component2.model.transform.forward);
				mWaypointMarkerToDragOwner = component.owner;
				if (!component2.baseCharacter.IsMoving())
				{
					component2.baseCharacter.MovementStyleActive = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				}
				dragSelection.enabled = false;
			}
			break;
		}
		case SelectableObject.QuickType.PreviewGhost:
		{
			WaypointMarker component = selectableObject.GetComponent<WaypointMarker>();
			dragSelection = component.GetComponent<SelectableObject>();
			mWaypointMarkerToDragOwner = component.owner;
			dragSelection.enabled = false;
			break;
		}
		case SelectableObject.QuickType.WayPointMarker:
		{
			WaypointMarker component = selectableObject.GetComponent<WaypointMarker>();
			dragSelection = component.GetComponent<SelectableObject>();
			mWaypointMarkerToDragOwner = component.owner;
			dragSelection.enabled = false;
			break;
		}
		case SelectableObject.QuickType.EnemySoldier:
			break;
		}
	}

	public void CancelDragging()
	{
		mBufferDrag = false;
		if (mWaypointMarkerDragMode && mWaypointMarkerToDragOwner != null)
		{
			WaypointMarkerManager.Instance.RemoveMarker(mWaypointMarkerToDragOwner);
			mWaypointMarkerToDragOwner = null;
		}
		mWaypointMarkerDragMode = false;
	}

	private void Screen_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (InputManager.Instance.NumFingersActive > 1 || !TutorialToggles.enableGhostDrag)
		{
			return;
		}
		if (mBufferDrag)
		{
			if ((fingerPos - mBufferDragStartPos).sqrMagnitude < 10000f)
			{
				return;
			}
			mGhostDragOffset = Vector2.up * 100f;
			StartDrag(fingerIndex, mBufferDragStartPos);
			mBufferDrag = false;
		}
		if (!mWaypointMarkerDragMode || !(mWaypointMarkerToDragOwner != null))
		{
			return;
		}
		SelectableObject selectableObject = SelectableObject.PickSelectableObject(fingerPos + mGhostDragOffset);
		if (selectableObject != null && selectableObject.quickType == SelectableObject.QuickType.EnemySoldier)
		{
			if (selectableObject != underDragSelection)
			{
				dragToContextMenuTimer = Time.time + 0.4f;
			}
		}
		else
		{
			selectableObject = null;
			dragToContextMenuTimer = -1f;
		}
		underDragSelection = selectableObject;
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos + mGhostDragOffset);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, smMoveToHitLayerMask))
		{
			mWorldMouseClick = hitInfo.point;
			List<Actor> list = new List<Actor>();
			list.Add(mWaypointMarkerToDragOwner.GetComponent<Actor>());
			SquadFormation.PreviewSquadMove(list, mWorldMouseClick, list[0].baseCharacter.MovementStyleRequested, FormationSpacingRow, FormationSpacingColumn, UnitCoverSearchRadius);
		}
	}

	private void Screen_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		if (mWaypointMarkerDragMode)
		{
			if (dragSelection != null)
			{
				dragSelection.enabled = true;
			}
			dragSelection = null;
			Actor actor = null;
			if (mWaypointMarkerToDragOwner != null)
			{
				actor = mWaypointMarkerToDragOwner.GetComponent<Actor>();
			}
			if (actor != null && TutorialToggles.enableGhostDrag)
			{
				BaseCharacter.MovementStyle speed = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				TaskMoveTo runningTask = actor.tasks.GetRunningTask<TaskMoveTo>();
				if (runningTask != null && runningTask.MoveSpeed == BaseCharacter.MovementStyle.Run)
				{
					speed = runningTask.MoveSpeed;
				}
				ExecuteGhostMovement(speed);
			}
			else if (this.OnFingerDragEndClick != null)
			{
				this.OnFingerDragEndClick(this);
			}
			if (!GameController.Instance.ClaymoreDroppingModeActive)
			{
				StartCoroutine(EnableCameraInput());
				NavMeshCamera.DisablePushing();
			}
			mWaypointMarkerDragMode = false;
		}
		mBufferDrag = false;
	}

	private IEnumerator EnableCameraInput()
	{
		yield return new WaitForEndOfFrame();
		CameraController cc = CameraManager.Instance.PlayCameraController;
		PlayCameraInterface cfd = cc.CurrentCameraBase as PlayCameraInterface;
		if (cfd != null)
		{
			cfd.AllowInput(true);
		}
	}

	private void BufferManualTapOrder(Vector2 fingerPos, BaseCharacter.MovementStyle speed)
	{
		mBufferedMovementSpeed = speed;
		mTapOrderBuffered = true;
		mBufferedFingerPos = fingerPos;
		mWorldMouseClickValid = false;
		mSelectedChangedTime = 0f;
	}

	public void BufferManualTapOrder(Vector3 WorldPos, BaseCharacter.MovementStyle speed)
	{
		mBufferedMovementSpeed = speed;
		mTapOrderBuffered = true;
		mWorldMouseClickValid = true;
		mWorldMouseClick = WorldPos;
		mSelectedChangedTime = 0f;
	}

	private void InitiateBufferedTapOrder()
	{
		if (!IsAllowedToProcessScreenTap())
		{
			mTapOrderBuffered = false;
		}
		else
		{
			if (CameraManager.Instance.ActiveCamera != 0)
			{
				return;
			}
			if (!mWorldMouseClickValid)
			{
				Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(mBufferedFingerPos);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, smMoveToHitLayerMask))
				{
					return;
				}
				mWorldMouseClick = hitInfo.point;
				debugHitThing = hitInfo.collider.gameObject;
			}
			InitiateBufferedTapOrderMove(mWorldMouseClick);
		}
	}

	private bool InitiateBufferedReleaseOrderThrowGrenade(Vector3 worldPosition)
	{
		bool result = false;
		foreach (Actor item in mSelected)
		{
			if (item.grenadeThrower.IsPrimed())
			{
				float sqrMagnitude = (worldPosition - item.GetPosition()).sqrMagnitude;
				float sqrMagnitude2 = (worldPosition.xz() - item.GetPosition().xz()).sqrMagnitude;
				if (sqrMagnitude2 <= item.grenadeThrower.LastKnownMinimumThrowDistanceSq * 0.3f || sqrMagnitude < item.grenadeThrower.LastKnownMinimumThrowDistanceSq || sqrMagnitude > item.grenadeThrower.LastKnownDistanceRangeSq)
				{
					item.grenadeThrower.Cancel();
				}
				else
				{
					OrdersHelper.RegisterTaskAsPlayerIssued(new TaskThrowGrenade(item.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType | Task.Config.DenyPlayerInput, worldPosition));
				}
				result = true;
			}
		}
		return result;
	}

	public void ExecuteGhostMovement(BaseCharacter.MovementStyle speed)
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		mBufferedMovementSpeed = (GameController.Instance.PlayerEngagedInCombat ? BaseCharacter.MovementStyle.Run : BaseCharacter.MovementStyle.Walk);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(a.gameObject);
			if (!(marker != null) || !marker.IsPreview())
			{
				continue;
			}
			a.baseCharacter.safeMovementStyle = BaseCharacter.MovementStyle.Run;
			Vector3 ajustedPosition = Vector3.zero;
			bool flag = TutorialToggles.WithinTapRestrictionRadius(marker.transform.position, ref ajustedPosition);
			if (IsValidPathfindDestination(a, ajustedPosition))
			{
				if (!flag)
				{
					break;
				}
				WaypointMarkerManager.Instance.LockOutDeletion();
				OrdersHelper.OrderUnitMove(this, a, ajustedPosition, speed, marker.GetCoverPoint(), QueuedOrder.OrderedTarget(a));
				WaypointMarkerManager.Instance.UnlockDeletion();
			}
			else
			{
				WaypointMarkerManager.Instance.RemoveMarker(a.gameObject);
			}
		}
	}

	public bool InitiateBufferedTapOrderMove(Vector3 worldPosition)
	{
		if (mSelected.Count == 0)
		{
			return false;
		}
		if (!TutorialToggles.WithinTapRestrictionRadius(worldPosition, ref worldPosition))
		{
			return false;
		}
		Actor selected = mSelected[0];
		foreach (Actor item in mSelected)
		{
			if (item.navAgent.enabled)
			{
				selected = item;
				break;
			}
		}
		if (!IsValidPathfindDestination(selected, worldPosition))
		{
			return false;
		}
		BaseCharacter.MovementStyle speed = mBufferedMovementSpeed;
		if (mSelected.Count > 0)
		{
			if (mSelected.Count == 1)
			{
				OrdersHelper.OrderSquadMove(this, worldPosition, speed, FormationCoverSearchRadius, 2f);
			}
			else
			{
				OrdersHelper.OrderSquadMove(this, worldPosition, speed, FormationCoverSearchRadius, FormationCoverSearchRadius);
			}
			return true;
		}
		return false;
	}

	public void MoveIntoNearbyCover(Actor a)
	{
		OrdersHelper.OrderUnitMoveToCover(this, a, a.GetPosition() + a.transform.forward * 0.1f, BaseCharacter.MovementStyle.AsFastAsSafelyPossible, 2f, false);
	}

	public void BeginPlacingClaymore()
	{
		mPlacingClaymore = true;
		GameController.Instance.StartPlacementMode();
		GameObject gameObject = new GameObject();
		mClaymoreLayingComponent = gameObject.AddComponent<ClaymoreLayingComponent>();
		mClaymoreLayingComponent.BeginPlacingClaymore();
	}

	private void PlaceClaymore(Vector3 dropPosition)
	{
		mPlacingClaymore = false;
		mClaymoreLayingComponent.PrepareClaymore(dropPosition);
		GameController.Instance.EndPlacementMode();
	}

	public void BeginPlacingBody()
	{
		mPlacingBody = true;
		GameController.Instance.StartPlacementMode();
		GameObject gameObject = new GameObject();
		mBodyDroppingComponent = gameObject.AddComponent<BodyDroppingComponent>();
		mBodyDroppingComponent.BeginDroppingBody();
	}

	private void PlaceBody(Vector3 dropPosition)
	{
		mPlacingBody = false;
		mBodyDroppingComponent.DropBody(dropPosition);
		GameController.Instance.EndPlacementMode();
	}

	public void CancelAnyPlacement()
	{
		if (CommonHudController.Instance != null)
		{
			CommonHudController.Instance.RemovePlaceFingerHereMarker();
			CommonHudController.Instance.RemoveGrenadeThrowMarker();
		}
		mPlacingBody = false;
		mPlacingClaymore = false;
		if ((bool)mClaymoreLayingComponent)
		{
			mClaymoreLayingComponent.TidyUp();
			mClaymoreLayingComponent = null;
		}
		if ((bool)mBodyDroppingComponent)
		{
			mBodyDroppingComponent.TidyUp();
			mBodyDroppingComponent = null;
		}
		GameController gameController = GameController.Instance;
		if (gameController != null && gameController.PlacementModeActive)
		{
			gameController.EndPlacementMode();
		}
	}

	private void SelectionChanged()
	{
		GameController gameController = GameController.Instance;
		CancelAnyPlacement();
		if (gameController.PlacementModeActive)
		{
			gameController.EndPlacementMode();
		}
	}

	private bool IsValidPathfindDestination(Actor selected, Vector3 destination)
	{
		if (!selected.navAgent.enabled)
		{
			return false;
		}
		NavMeshPath navMeshPath = new NavMeshPath();
		if (WorldHelper.CalculatePath_AvoidingMantlesWhenCarrying(selected, destination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
		{
			return true;
		}
		return false;
	}

	private void ValidateSquadSelection()
	{
		for (int num = mSelected.Count - 1; num >= 0; num--)
		{
			if (!mSelected[num].realCharacter.IsSelectable())
			{
				mSelected.RemoveAt(num);
				mSelectionChanged = true;
			}
		}
	}

	private List<Actor> GetControllableActors()
	{
		List<Actor> list = new List<Actor>();
		foreach (Actor item in Selected)
		{
			if (item != null && !item.tasks.RunningTaskDeniesPlayerInput)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public AnimationClip GetSpecialCaseAnim(string animName)
	{
		if (string.IsNullOrEmpty(animName))
		{
			return null;
		}
		int count = SpecialCaseAnimations.Count;
		for (int i = 0; i < count; i++)
		{
			if (SpecialCaseAnimations[i] != null && SpecialCaseAnimations[i].name == animName)
			{
				return SpecialCaseAnimations[i];
			}
		}
		return null;
	}
}
