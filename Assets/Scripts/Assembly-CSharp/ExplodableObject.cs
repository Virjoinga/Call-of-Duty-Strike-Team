using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CMSetUpExplosives))]
[RequireComponent(typeof(SetPieceLogic))]
public class ExplodableObject : MonoBehaviour
{
	public delegate void OnExplodeEventHandler(object sender);

	public delegate void OnArmEventHandler(object sender);

	public ExplodableObjectData m_Interface;

	public float SetUpRadius = 1.5f;

	private float mDetonationTimer;

	private bool mArmed;

	[HideInInspector]
	public bool CanExplode = true;

	public GameObject PlantSetPiece;

	public GameObject PlantSetPiece_FPP;

	private GameObject mGameObjectToTest;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	[HideInInspector]
	public Actor ArmedBy;

	private bool mTriggerExplosives;

	private SetPieceLogic spl;

	public SetPieceModule RemoteTriggerSetPiece;

	private SetPieceLogic mRemoteTriggerLogic;

	private bool mWasInFPPBeforeRemoteTrigger;

	public bool IsArmed
	{
		get
		{
			return mArmed;
		}
	}

	public GameObject TestGameObject
	{
		get
		{
			return mGameObjectToTest;
		}
		set
		{
			mGameObjectToTest = value;
		}
	}

	public event OnExplodeEventHandler OnExplode;

	public event OnArmEventHandler OnArm;

	public void Awake()
	{
		GameObject[] explodedObjectsToShow = m_Interface.ExplodedObjectsToShow;
		foreach (GameObject gameObject in explodedObjectsToShow)
		{
			gameObject.SetActive(false);
		}
		if (m_Interface.GlobalTriggerObject == null)
		{
			m_Interface.GlobalTriggerObject = base.gameObject;
		}
	}

	public void SwitchToCorrectSPL()
	{
		spl = GetComponent<SetPieceLogic>();
		if (spl != null)
		{
			SetPieceModule setPieceModule = PlantSetPiece.GetComponent<SetPieceModule>();
			if (GameController.Instance.IsFirstPerson)
			{
				setPieceModule = GameController.Instance.mFirstPersonActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kPlantC4);
			}
			spl.SetModule(setPieceModule);
			int count = setPieceModule.m_PlaygroundActors.Count;
			spl.ObjectActors = new GameObject[count + 1];
			spl.ObjectActors[count] = base.gameObject;
			spl.Mortal = false;
		}
	}

	public void AddExplodeEventHandler(OnExplodeEventHandler eventHandler)
	{
		this.OnExplode = (OnExplodeEventHandler)Delegate.Combine(this.OnExplode, eventHandler);
	}

	public void AddArmEventHandler(OnArmEventHandler eventHandler)
	{
		this.OnArm = (OnArmEventHandler)Delegate.Combine(this.OnArm, eventHandler);
	}

	public void RemoveExplodeEventHandler(OnExplodeEventHandler eventHandler)
	{
		this.OnExplode = (OnExplodeEventHandler)Delegate.Remove(this.OnExplode, eventHandler);
	}

	public void RemoveArmEventHandler(OnArmEventHandler eventHandler)
	{
		this.OnArm = (OnArmEventHandler)Delegate.Remove(this.OnArm, eventHandler);
	}

	public void Arm()
	{
		CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get("S_EXPLOSIVEARMED"));
		mArmed = true;
		if (m_Interface.DetonationType == ExplodableObjectData.Type.Timer && m_Interface.UseMissionTimer)
		{
			StartCoroutine(WaitForTimeToExpire());
		}
		if (this.OnArm != null)
		{
			this.OnArm(this);
		}
		if (m_Interface.MessagesOnArm != null && m_Interface.MessagesOnArm.Count > 0)
		{
			for (int i = 0; i < m_Interface.MessagesOnArm.Count; i++)
			{
				Container.SendMessage(m_Interface.MessagesOnArm[i].Object, m_Interface.MessagesOnArm[i].Message, base.gameObject);
			}
		}
	}

	public void Update()
	{
		if (!mArmed || !CanExplode)
		{
			return;
		}
		if (m_Interface.DetonationType == ExplodableObjectData.Type.Distance)
		{
			mTriggerExplosives = UpdateForRadius();
		}
		else if (!m_Interface.UseMissionTimer)
		{
			mTriggerExplosives = UpdateForTime();
		}
		if (!mTriggerExplosives)
		{
			return;
		}
		if (!m_Interface.UseRemoteTrigger || ArmedBy == null || RemoteTriggerSetPiece == null)
		{
			TriggerExplosion();
			return;
		}
		if (!ArmedBy.realCharacter.IsMortallyWounded() && !ArmedBy.realCharacter.IsDead())
		{
			mWasInFPPBeforeRemoteTrigger = ArmedBy == GameController.Instance.mFirstPersonActor;
			ArmedBy.baseCharacter.HoldPosition();
			mRemoteTriggerLogic = ArmedBy.realCharacter.CreateSetPieceLogic();
			mRemoteTriggerLogic.SetModule(RemoteTriggerSetPiece);
			mRemoteTriggerLogic.PlaceSetPiece(ArmedBy.transform);
			mRemoteTriggerLogic.SetActor_IndexOnlyCharacters(0, ArmedBy);
			mRemoteTriggerLogic.PlaySetPiece();
		}
		else
		{
			mRemoteTriggerLogic = null;
		}
		StartCoroutine(DelayExplosionTrigger());
		mArmed = false;
		CanExplode = false;
	}

	private IEnumerator DelayExplosionTrigger()
	{
		do
		{
			yield return new WaitForEndOfFrame();
		}
		while (mRemoteTriggerLogic != null && !mRemoteTriggerLogic.HasFinished());
		TriggerExplosion();
	}

	private void TriggerExplosion()
	{
		if (this.OnExplode != null)
		{
			this.OnExplode(this);
		}
		if (spl.SPModule.m_PlaygroundActors.Count > 1)
		{
			UnityEngine.Object.DestroyImmediate(spl.SPModule.m_PlaygroundActors[1]);
		}
		RemoveBlip();
		if (ArmedBy != null)
		{
			RulesSystem.DoAreaOfEffectDamage(base.transform.position, 5f, 1f, ArmedBy.gameObject, m_Interface.ExplosionType, "Explosion");
		}
		else
		{
			ExplosionManager.Instance.StartExplosion(base.gameObject.transform.position, m_Interface.TriggerRadius * 0.5f);
			ExplosionManager.BroadcastNoise(base.gameObject.transform.position, null);
		}
		SwapObjects();
		if (m_Interface.MessagesOnDetonation != null && m_Interface.MessagesOnDetonation.Count > 0)
		{
			for (int i = 0; i < m_Interface.MessagesOnDetonation.Count; i++)
			{
				Container.SendMessage(m_Interface.MessagesOnDetonation[i].Object, m_Interface.MessagesOnDetonation[i].Message, base.gameObject);
			}
		}
		if (mRemoteTriggerLogic != null && mWasInFPPBeforeRemoteTrigger)
		{
			GameController.Instance.StartCoroutine(GameController.Instance.SwitchToFirstPerson(ArmedBy, 2f));
		}
		base.gameObject.SetActive(false);
	}

	public void Explode()
	{
		CanExplode = true;
	}

	public void RemoveBlip()
	{
		CMSetUpExplosives componentInChildren = IncludeDisabled.GetComponentInChildren<CMSetUpExplosives>(base.gameObject);
		if (componentInChildren != null)
		{
			componentInChildren.RemoveBlip();
		}
	}

	private bool UpdateForRadius()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if ((a.transform.position - m_Interface.GlobalTriggerObject.transform.position).sqrMagnitude < m_Interface.TriggerRadius * m_Interface.TriggerRadius)
			{
				return false;
			}
		}
		return true;
	}

	private bool UpdateForTime()
	{
		if (m_Interface.TimeToDetonate == 0f)
		{
			return true;
		}
		if (mDetonationTimer >= 1f)
		{
			m_Interface.TimeToDetonate -= 1f;
			mDetonationTimer -= 1f;
		}
		mDetonationTimer += TimeManager.DeltaTime;
		return false;
	}

	private IEnumerator WaitForTimeToExpire()
	{
		while (CommonHudController.Instance == null)
		{
			yield return null;
		}
		CommonHudController.Instance.MissionTimer.Set(m_Interface.TimeToDetonate, 0f);
		CommonHudController.Instance.MissionTimer.StartTimer();
		while (CommonHudController.Instance.MissionTimer.CurrentState != MissionTimer.TimerState.Finished)
		{
			yield return null;
		}
		CommonHudController.Instance.MissionTimer.StopTimer();
		mTriggerExplosives = true;
	}

	private void SwapObjects()
	{
		GameObject[] explodedObjectsToRemove = m_Interface.ExplodedObjectsToRemove;
		foreach (GameObject gameObject in explodedObjectsToRemove)
		{
			gameObject.SetActive(false);
		}
		GameObject[] explodedObjectsToShow = m_Interface.ExplodedObjectsToShow;
		foreach (GameObject gameObject2 in explodedObjectsToShow)
		{
			gameObject2.SetActive(true);
		}
	}

	public void EnableInteraction()
	{
		CMSetUpExplosives component = base.gameObject.GetComponent<CMSetUpExplosives>();
		if (component != null)
		{
			component.StartViewable = true;
			component.Activate();
		}
	}

	public void DisableInteraction()
	{
		CMSetUpExplosives component = base.gameObject.GetComponent<CMSetUpExplosives>();
		if (component != null)
		{
			component.Deactivate();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, SetUpRadius);
		Gizmos.color = Color.red;
		if (m_Interface.GlobalTriggerObject != null)
		{
			Gizmos.DrawWireSphere(m_Interface.GlobalTriggerObject.transform.position, m_Interface.TriggerRadius);
		}
		else
		{
			Gizmos.DrawWireSphere(base.transform.position, m_Interface.TriggerRadius);
		}
	}
}
