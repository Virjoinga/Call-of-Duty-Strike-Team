using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableObject : MonoBehaviour
{
	public enum State
	{
		Clean = 0,
		Dirty = 1,
		HackInProgress = 2,
		HackInProgressStalled = 3,
		HackSucessful = 4,
		HackFinished = 5,
		TimeSwitchActive = 6,
		Cancelling = 7
	}

	private enum StanceState
	{
		HackLoop = 0,
		DuckInto = 1,
		DuckLoop = 2,
		DuckOutOf = 3
	}

	private const int k_HigherPriorityThanAllOtherAnimationsLayers = 16;

	public HackableObjectData m_Interface = new HackableObjectData();

	protected bool ShouldUseConsultant;

	public SetPieceModule SetPieceEnter;

	public SetPieceModule SetPieceExit;

	public SingleAnimation HackLoopAnimation;

	public SingleAnimation DuckIntoAnimation;

	public SingleAnimation DuckIdleAnimation;

	public SingleAnimation DuckOutOfAnimation;

	public GameObject ProgressBlipRef;

	private HackingBlip ProgressBlip;

	private Actor mHackingActor;

	[HideInInspector]
	public Actor LastHacker;

	protected ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private Animation mHackingActorsAnimPlayer;

	public bool AllowDestroyOnHack = true;

	private bool mIsTimerPaused;

	private float mTimerProgress;

	private bool mHasOnStartEventTriggeredBefore;

	private bool mIsTransitioning;

	private StanceState mStance;

	public bool IsInUse;

	[HideInInspector]
	public Actor HackingActor
	{
		get
		{
			return mHackingActor;
		}
		set
		{
			if (mHackingActor != null)
			{
				CleanUpCoroutines();
				CleanUpAnimations();
			}
			mHackingActor = value;
			SetUpAnims();
		}
	}

	public bool HasTimerStarted { get; private set; }

	[HideInInspector]
	public bool HasReset { get; set; }

	public State HackState { get; private set; }

	public float HackProgress { get; private set; }

	public bool IsInProgress
	{
		get
		{
			return HackState == State.HackInProgress;
		}
	}

	public float Hacked0To1
	{
		get
		{
			return (HackProgress != 0f) ? (HackProgress / m_Interface.TimeToHack) : 0f;
		}
	}

	public float HackedPercentage
	{
		get
		{
			return Hacked0To1 * 100f;
		}
	}

	public bool FullyHacked
	{
		get
		{
			return Hacked0To1 >= 1f;
		}
	}

	public HackObjective AssociatedHackObjective { get; set; }

	public virtual void Init(Actor actor)
	{
	}

	public virtual void CleanUp(Actor actor)
	{
	}

	private void Start()
	{
		HackState = State.Clean;
		HackProgress = 0f;
		if (ProgressBlipRef != null)
		{
			ProgressBlipRef = Object.Instantiate(ProgressBlipRef) as GameObject;
			if (ProgressBlipRef != null)
			{
				ProgressBlip = ProgressBlipRef.GetComponent<HackingBlip>();
				if (ProgressBlip != null)
				{
					ProgressBlip.Target = base.transform;
				}
			}
		}
		if (m_Interface.TimerData.StatusLight != null)
		{
			GameObject theObject = m_Interface.TimerData.StatusLight.theObject;
			if (theObject != null)
			{
				m_Interface.TimerData.StatusLightComp = theObject.GetComponentInChildren<SwitchLight>();
			}
		}
	}

	public virtual void Update()
	{
		if (ObjectHasBecomeInvalid())
		{
			if (ShouldUseConsultant)
			{
				Consult();
			}
			DisableInteraction();
			base.enabled = false;
			return;
		}
		switch (HackState)
		{
		case State.HackInProgress:
			if (!mHasOnStartEventTriggeredBefore)
			{
				OnHackStart();
			}
			ControlAnimations();
			if (mIsTimerPaused)
			{
				break;
			}
			if (HasTimerStarted && Time.deltaTime > 0f)
			{
				HackProgress += Time.deltaTime;
			}
			if (ProgressBlip != null)
			{
				ProgressBlip.SetProgress(Hacked0To1);
			}
			if (ShouldUseConsultant)
			{
				Consult();
			}
			if (Hacked0To1 >= 1f)
			{
				HackProgress = m_Interface.TimeToHack;
				if (m_Interface.TimerData.StatusLightComp != null)
				{
					Container.SendMessage(m_Interface.TimerData.StatusLightComp.gameObject, "Deactivate", base.gameObject);
				}
				HackState = State.HackSucessful;
				OnHackSuccessful();
				if (HackingActor != null)
				{
					HackingActor.tasks.CancelTasks<TaskSetPiece>();
				}
				if (m_Interface.TimerData.IsTimerSwtich)
				{
					ToggleMarker(false, false);
				}
				else
				{
					DisableInteraction();
				}
			}
			break;
		case State.HackSucessful:
			if (m_Interface.TimerData.IsTimerSwtich)
			{
				mTimerProgress = 0f;
				SpreadMessageToGroup(m_Interface.TimerData.ObjectToControl, m_Interface.TimerData.FuncToCallOnHack);
				if (m_Interface.TimerData.StatusLightComp != null)
				{
					m_Interface.TimerData.StatusLightComp.SendMessage("StartTimed", m_Interface.TimerData.ActiveSwitchTime);
				}
				HackState = State.TimeSwitchActive;
			}
			else
			{
				HackState = State.HackFinished;
			}
			break;
		case State.TimeSwitchActive:
			if (mTimerProgress < m_Interface.TimerData.ActiveSwitchTime)
			{
				mTimerProgress += Time.deltaTime;
				break;
			}
			Deactivate();
			if (m_Interface.TimerData.StatusLightComp != null)
			{
				Container.SendMessage(m_Interface.TimerData.StatusLightComp.gameObject, "Activate", base.gameObject);
			}
			FailHackAttempt(false, false);
			break;
		case State.HackInProgressStalled:
		case State.HackFinished:
			break;
		}
	}

	public void Activate()
	{
		if (m_Interface.TimerData.IsTimerSwtich)
		{
			ToggleMarker(false, false);
			HackState = State.TimeSwitchActive;
		}
	}

	public void Deactivate()
	{
		if (!m_Interface.TimerData.IsTimerSwtich)
		{
			HackState = State.Dirty;
			ToggleMarker(false, true);
		}
		else
		{
			SpreadMessageToGroup(m_Interface.TimerData.ObjectToControl, m_Interface.TimerData.FuncToCallOnTimeOut);
			ToggleMarker(true, false);
		}
	}

	public void EnableInteraction()
	{
		ToggleMarker(true, false);
	}

	public void DisableInteraction()
	{
		ToggleMarker(false, false);
	}

	private void ToggleMarker(bool onoff, bool destroy)
	{
		CMHackableObject component = GetComponent<CMHackableObject>();
		if (component != null)
		{
			if (onoff)
			{
				component.Activate();
				component.TurnOn();
			}
			else
			{
				component.Deactivate();
				if (destroy)
				{
					Object.Destroy(component);
				}
			}
		}
		if (!(base.transform.parent != null))
		{
			return;
		}
		component = base.transform.parent.GetComponent<CMHackableObject>();
		if (!(component != null))
		{
			return;
		}
		if (onoff)
		{
			component.Activate();
			component.TurnOn();
			return;
		}
		component.Deactivate();
		if (destroy)
		{
			Object.Destroy(component);
		}
	}

	private void OnHackStart()
	{
		if (!mHasOnStartEventTriggeredBefore)
		{
			mHasOnStartEventTriggeredBefore = true;
			if (m_Interface.ObjectToCallOnStart != null)
			{
				Container.SendMessage(m_Interface.ObjectToCallOnStart, m_Interface.FunctionToCallOnStart, base.gameObject);
			}
		}
	}

	private void OnHackSuccessful()
	{
		if (m_Interface.ObjectToCallOnSuccess != null)
		{
			Container.SendMessage(m_Interface.ObjectToCallOnSuccess, m_Interface.FunctionToCallOnSuccess, base.gameObject);
		}
		SpreadMessageToGroup(m_Interface.GroupObjectToCallOnSuccess, m_Interface.GroupFunctionToCallOnSuccess);
		if (AssociatedHackObjective != null)
		{
			AssociatedHackObjective.OnSuccessfullHack();
		}
		HideBlip();
		StopHackProgressLoop();
	}

	private void OnHackFail()
	{
		if (m_Interface.ObjectToCallOnFail != null)
		{
			Container.SendMessage(m_Interface.ObjectToCallOnFail, m_Interface.FunctionToCallOnFail, base.gameObject);
		}
		SpreadMessageToGroup(m_Interface.GroupObjectToCallOnFail, m_Interface.GroupFunctionToCallOnFail);
	}

	public void StartHacking()
	{
		if (HackState == State.Clean || HackState == State.Dirty || HackState == State.HackInProgressStalled)
		{
			HackState = State.HackInProgress;
			HasTimerStarted = true;
			mIsTimerPaused = false;
		}
	}

	public void StopHacking()
	{
		if (IsInProgress)
		{
			HackState = State.HackInProgressStalled;
		}
		CleanUpAnimations();
		IsInUse = false;
		HasTimerStarted = false;
		HideBlip();
		StopHackProgressLoop();
		if (!(mHackingActor != null) || !mHackingActor.behaviour.PlayerControlled)
		{
			return;
		}
		mHackingActor.realCharacter.SetSelectable(true, true, true, true);
		if (GKM.UnitCount(GKM.PlayerControlledMask) == 1)
		{
			GameplayController instance = GameplayController.instance;
			if (instance != null)
			{
				instance.AddToSelected(mHackingActor);
			}
		}
	}

	public void ResumeHacking()
	{
		HackState = State.Dirty;
	}

	public void CancelHacking()
	{
		if (HackState == State.HackInProgress && HackingActor != null)
		{
			if (m_Interface != null && m_Interface.IsMultiManHack)
			{
				Reset();
			}
			HackState = State.Cancelling;
			if (mStance == StanceState.DuckLoop)
			{
				PlayDuckOutOfAnimation();
			}
			StartCoroutine("ExitWhenNoLongerDucking");
		}
	}

	private IEnumerator ExitWhenNoLongerDucking()
	{
		while (mIsTimerPaused)
		{
			yield return null;
		}
		HackingActor.tasks.CancelTasks<TaskSetPiece>();
		HackingActor.tasks.CancelTasks<TaskHack>();
		StopHackProgressLoop();
		HackState = State.Dirty;
	}

	public void FailHackAttempt(bool wasTargetDestroyedOrDamaged, bool exitQuickly)
	{
		if (HackState != State.HackInProgress)
		{
			FailHackAttemptCleanUp(wasTargetDestroyedOrDamaged);
			return;
		}
		OnHackFail();
		if (exitQuickly)
		{
			CleanUpAnimations();
			StopHackProgressLoop();
		}
		else
		{
			CancelHacking();
			StopHacking();
		}
		FailHackAttemptCleanUp(wasTargetDestroyedOrDamaged);
	}

	private void FailHackAttemptCleanUp(bool wasTargetDestroyedOrDamaged)
	{
		Reset();
		if (wasTargetDestroyedOrDamaged)
		{
			DisableInteraction();
		}
	}

	private void OnEnableBlip()
	{
		EnableInteraction();
	}

	private void OnDisableBlip()
	{
		DisableInteraction();
	}

	public void ShowBlip()
	{
		if (ProgressBlip != null)
		{
			ProgressBlip.ShowBlip();
		}
	}

	public void HideBlip()
	{
		if (ProgressBlip != null)
		{
			ProgressBlip.HideBlip();
		}
	}

	public void Reset()
	{
		HasReset = true;
		HackState = State.Clean;
		HasTimerStarted = false;
		mIsTimerPaused = false;
		mTimerProgress = 0f;
		mHasOnStartEventTriggeredBefore = false;
		HackProgress = 0f;
		HideBlip();
		if (ProgressBlip != null)
		{
			ProgressBlip.SetProgress(0f);
		}
		EnableInteraction();
		if (m_Interface.TimerData.StatusLightComp != null)
		{
			Container.SendMessage(m_Interface.TimerData.StatusLightComp.gameObject, "Activate", base.gameObject);
		}
	}

	private void ControlAnimations()
	{
		if (HackingActor == null || HackState == State.Cancelling || mIsTransitioning)
		{
			return;
		}
		bool flag = false;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(HackingActor.awareness.EnemiesWhoCanSeeMe());
		Actor a;
		while (actorIdentIterator.NextActor(out a) && !flag)
		{
			if (a.weapon != null && a.weapon.GetTarget() == HackingActor && a.weapon.ShootingIsDesiredAndAllowed() && a.weapon.IsFiring())
			{
				if (mStance == StanceState.HackLoop)
				{
					PlayDuckIntoAnimation();
				}
				mIsTimerPaused = true;
				flag = true;
			}
		}
		if (!flag && mStance == StanceState.DuckLoop)
		{
			PlayDuckOutOfAnimation();
			if (FullyHacked)
			{
				EnableAnimation(DuckIntoAnimation, false);
				EnableAnimation(DuckIdleAnimation, false);
				EnableAnimation(DuckOutOfAnimation, false);
				EnableAnimation(HackLoopAnimation, false);
				HackingActor.animDirector.enabled = true;
			}
		}
	}

	private void SetUpAnims()
	{
		if (HackingActor != null)
		{
			mHackingActorsAnimPlayer = HackingActor.animDirector.AnimationPlayer;
		}
		SetUpAnim(HackLoopAnimation, "HackLoop", WrapMode.Loop);
		SetUpAnim(DuckIntoAnimation, "DuckInto", WrapMode.ClampForever);
		SetUpAnim(DuckIdleAnimation, "DuckIdle", WrapMode.Loop);
		SetUpAnim(DuckOutOfAnimation, "DuckOutOf", WrapMode.ClampForever);
	}

	private void SetUpAnim(SingleAnimation anim, string animName, WrapMode wrapMode)
	{
		if (anim != null && !string.IsNullOrEmpty(animName) && !(mHackingActorsAnimPlayer == null))
		{
			anim.Initialise(animName, mHackingActorsAnimPlayer);
			anim.Enable(false);
			if (anim.State != null)
			{
				anim.State.wrapMode = wrapMode;
				anim.State.time = 0f;
				anim.State.speed = 1f;
				anim.State.weight = 1f;
				anim.State.layer = 16;
			}
		}
	}

	public void CleanUpCoroutines()
	{
		StopCoroutine("PlayDuckIdleAnim");
		StopCoroutine("ResumeHackingAnim");
	}

	public void CleanUpAnimations()
	{
		ResetAnimation(DuckIntoAnimation);
		ResetAnimation(DuckIdleAnimation);
		ResetAnimation(DuckOutOfAnimation);
		ResetAnimation(HackLoopAnimation);
		EnableAnimation(DuckIntoAnimation, false);
		EnableAnimation(DuckIdleAnimation, false);
		EnableAnimation(DuckOutOfAnimation, false);
		EnableAnimation(HackLoopAnimation, false);
		if (HackingActor != null)
		{
			HackingActor.animDirector.enabled = true;
		}
	}

	private void PlayDuckIntoAnimation()
	{
		if (DuckIntoAnimation != null && DuckIdleAnimation != null && !DuckIntoAnimation.State.enabled && !DuckIdleAnimation.State.enabled)
		{
			if (HackingActor != null)
			{
				HackingActor.animDirector.enabled = false;
			}
			ResetAnimationTime(DuckIntoAnimation);
			ResetAnimationTime(DuckIdleAnimation);
			ResetAnimationTime(DuckOutOfAnimation);
			EnableAnimation(HackLoopAnimation, false);
			EnableAnimation(DuckIntoAnimation, true);
			StopHackProgressLoop();
			mStance = StanceState.DuckLoop;
			StartCoroutine("PlayDuckIdleAnim");
		}
	}

	private IEnumerator PlayDuckIdleAnim()
	{
		mIsTransitioning = true;
		while (DuckIntoAnimation != null && DuckIntoAnimation.RemainingTime > 0f)
		{
			yield return null;
		}
		mIsTransitioning = false;
		EnableAnimation(DuckIntoAnimation, false);
		EnableAnimation(DuckIdleAnimation, true);
	}

	private void PlayDuckOutOfAnimation()
	{
		EnableAnimation(DuckIdleAnimation, false);
		EnableAnimation(DuckOutOfAnimation, true);
		mStance = StanceState.HackLoop;
		StartCoroutine("ResumeHackingAnim");
	}

	private IEnumerator ResumeHackingAnim()
	{
		mIsTransitioning = true;
		while (DuckOutOfAnimation != null && DuckOutOfAnimation.RemainingTime > 0f)
		{
			yield return null;
		}
		mIsTransitioning = false;
		mIsTimerPaused = false;
		if (HackLoopAnimation != null)
		{
			EnableAnimation(DuckIntoAnimation, false);
			EnableAnimation(DuckIdleAnimation, false);
			EnableAnimation(DuckOutOfAnimation, false);
			EnableAnimation(HackLoopAnimation, true);
			PlayHackProgressLoop();
		}
	}

	private void SpreadMessageToGroup(List<GameObject> groupObjects, List<string> groupFuncs)
	{
		if (groupObjects == null || groupObjects.Count <= 0 || groupFuncs == null)
		{
			return;
		}
		string message = string.Empty;
		int count = groupObjects.Count;
		for (int i = 0; i < count; i++)
		{
			if (i < groupFuncs.Count)
			{
				message = groupFuncs[i];
			}
			Container.SendMessage(groupObjects[i], message, base.gameObject);
		}
	}

	protected virtual void Consult()
	{
	}

	protected virtual bool ObjectHasBecomeInvalid()
	{
		return false;
	}

	private void PlayHackProgressLoop()
	{
		SetPieceSFX.Instance.HackProgressLoop.Play(base.gameObject);
	}

	private void StopHackProgressLoop()
	{
		SetPieceSFX.Instance.HackProgressLoop.StopAll();
	}

	private void EnableAnimation(SingleAnimation anim, bool isEnabled)
	{
		if (anim != null)
		{
			anim.Enable(isEnabled);
			anim.ResetTime();
		}
	}

	private void ResetAnimation(SingleAnimation anim)
	{
		if (anim != null)
		{
			anim.Reset();
		}
	}

	private void ResetAnimationTime(SingleAnimation anim)
	{
		if (anim != null)
		{
			anim.ResetTime();
		}
	}
}
