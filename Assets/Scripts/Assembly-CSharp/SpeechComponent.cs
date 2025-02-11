using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeechComponent : BaseActorComponent
{
	public enum SpeechMode
	{
		Normal = 0,
		Occasional = 1
	}

	public enum SpeechEventType
	{
		ManDown = 0,
		KillConfirm = 1,
		StealthKill = 2,
		GrenadeThrown = 3,
		FriendlyAttackOrder = 4,
		Pain = 5,
		SniperSpotted = 6,
		Reload = 7,
		SniperKilled = 8,
		Healed = 9,
		LostAimedShot = 10,
		BattleChatter = 11,
		GMGDeath = 12,
		GMGRevive = 13,
		IntelCollected = 14,
		FollowMe = 15,
		HitBySniper = 16
	}

	private class SpeechEvent
	{
		private float mTimer;

		private SpeechEventType mTypeId;

		public SpeechEvent(SpeechEventType typeId, float waitTime)
		{
			mTypeId = typeId;
			mTimer = waitTime;
		}

		public void Update(SpeechComponent owner)
		{
			if (!(mTimer > 0f))
			{
				return;
			}
			mTimer -= Time.deltaTime;
			if (mTimer <= 0f && IsValidToSpeak(owner, mTypeId))
			{
				switch (mTypeId)
				{
				case SpeechEventType.ManDown:
					VocalSFXHelper.ManDown(owner.myActor);
					break;
				case SpeechEventType.KillConfirm:
					VocalSFXHelper.KillConfirm(owner.myActor);
					break;
				case SpeechEventType.StealthKill:
					VocalSFXHelper.StealthKillConfirm(owner.myActor);
					break;
				case SpeechEventType.GrenadeThrown:
					VocalSFXHelper.GrenadeReaction(owner.myActor);
					break;
				case SpeechEventType.FriendlyAttackOrder:
					VocalSFXHelper.OrderReceived(owner.myActor);
					break;
				case SpeechEventType.Pain:
					VocalSFXHelper.PainCry(owner.myActor);
					break;
				case SpeechEventType.SniperSpotted:
					VocalSFXHelper.SniperSpotted(owner.myActor);
					break;
				case SpeechEventType.Reload:
					VocalSFXHelper.Reload(owner.myActor);
					break;
				case SpeechEventType.SniperKilled:
					VocalSFXHelper.SniperKilled(owner.myActor);
					break;
				case SpeechEventType.Healed:
					VocalSFXHelper.Healed(owner.myActor);
					break;
				case SpeechEventType.LostAimedShot:
					VocalSFXHelper.LostAimedShot(owner.myActor);
					break;
				case SpeechEventType.BattleChatter:
					VocalSFXHelper.BattleChatter(owner.myActor);
					break;
				case SpeechEventType.GMGDeath:
					VocalSFXHelper.GMGDeath(owner.myActor);
					break;
				case SpeechEventType.GMGRevive:
					VocalSFXHelper.GMGRevive(owner.myActor);
					break;
				case SpeechEventType.IntelCollected:
					VocalSFXHelper.IntelCollected(owner.myActor);
					break;
				case SpeechEventType.FollowMe:
					VocalSFXHelper.FollowMe(owner.myActor);
					break;
				case SpeechEventType.HitBySniper:
					VocalSFXHelper.HitBySniper(owner.myActor);
					break;
				}
				if (mTypeId != SpeechEventType.Pain)
				{
					owner.SpeechCooldown = true;
				}
			}
		}

		public bool HasExpired()
		{
			return mTimer <= 0f;
		}

		public static bool IsValidToSpeak(SpeechComponent owner, SpeechEventType typeId)
		{
			if (CommonHudController.Instance != null && CommonHudController.Instance.MissionDialogueQueue.IsDialogueBeingPlayed())
			{
				return false;
			}
			if ((bool)InteractionsManager.Instance && InteractionsManager.Instance.IsPlayingCutscene())
			{
				return false;
			}
			if (owner.SpeechCooldown)
			{
				return false;
			}
			if (typeId == SpeechEventType.Healed)
			{
				return true;
			}
			if (owner.myActor.realCharacter.IsDead())
			{
				return false;
			}
			if (owner.myActor.realCharacter.IsMortallyWounded() && typeId != SpeechEventType.Pain && typeId != SpeechEventType.GMGDeath && typeId != SpeechEventType.GMGRevive)
			{
				return false;
			}
			return true;
		}
	}

	private const float mMaxRandomTimeForBattleChatter = 5f;

	private List<SpeechEvent> mEvents;

	private float mSpeechCooldown;

	private float mTimeOfLastShootOrder;

	private static float mMinimumTimeBetweenOrderAudio = 1f;

	private float mTimeOfLastPain;

	private static float mMinimumTimeBetweenPainAudio = 1f;

	private static float mOccassionalMinimumTimeBetweenPainAudio = 4f;

	private static float mTimeOfLastOnDeath;

	private static float mMinimumTimeBetweenOnDeathAudio = 0.5f;

	private static float mTimeOfLastFollowMe;

	private static float mMinimumTimeBetweenFollowMeAudio = 1f;

	private float mTimeOfLastBattleChatter;

	private static float mMinimumTimeBetweenBattleChatterAudio = 1f;

	private static float mRandomExtraTimeForBattleChatter;

	public bool SpeechCooldown
	{
		get
		{
			return mSpeechCooldown > 0f;
		}
		set
		{
			mSpeechCooldown = ((!value) ? 0f : 1f);
		}
	}

	public static bool BattleChatterActive { get; set; }

	private void Awake()
	{
		mEvents = new List<SpeechEvent>();
	}

	private void Start()
	{
		TBFAssert.DoAssert(myActor != null, "Owner ref should've been setup by ActorGenerator?");
		GameplayController gameplayController = GameplayController.Instance();
		gameplayController.OnDeath += OnDeath;
		gameplayController.OnGrenade += OnGrenade;
		gameplayController.OnShootOrder += OnShootOrder;
		gameplayController.OnPlayerCharacterAboutToBeMortallyWounded += OnPlayerCharacterAboutToBeMortallyWounded;
		myActor.health.OnHealthChange += OnHealthChange;
		mTimeOfLastShootOrder = Time.time;
	}

	private void OnDestroy()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			gameplayController.OnDeath -= OnDeath;
			gameplayController.OnGrenade -= OnGrenade;
			gameplayController.OnShootOrder -= OnShootOrder;
			gameplayController.OnPlayerCharacterAboutToBeMortallyWounded -= OnPlayerCharacterAboutToBeMortallyWounded;
		}
		myActor.health.OnHealthChange -= OnHealthChange;
	}

	private void Update()
	{
		if (mSpeechCooldown > 0f)
		{
			mSpeechCooldown -= Time.deltaTime;
		}
		for (int num = mEvents.Count - 1; num >= 0; num--)
		{
			SpeechEvent speechEvent = mEvents[num];
			speechEvent.Update(this);
			if (speechEvent.HasExpired())
			{
				mEvents.RemoveAt(num);
			}
		}
	}

	private bool OverwatchOmit(HealthComponent.HeathChangeEventArgs hce)
	{
		bool result = false;
		if ((bool)OverwatchController.Instance && OverwatchController.Instance.Active && hce != null && hce.From != null)
		{
			result = true;
		}
		return result;
	}

	private void OnDeath(object sender, HealthComponent.HeathChangeEventArgs hce)
	{
		Actor actor = sender as Actor;
		if (!(actor == null) && !(actor == myActor) && !(hce.DamageType == "Script") && !OverwatchOmit(hce) && Time.time > mTimeOfLastOnDeath + mMinimumTimeBetweenOnDeathAudio && (myActor.behaviour.PlayerControlled || myActor.awareness.CanSee(actor)))
		{
			SpeechEvent item = (KillTypeHelper.IsAStealthKill(hce) ? new SpeechEvent(SpeechEventType.StealthKill, 4f) : ((!actor.realCharacter || !actor.realCharacter.IsSniper) ? new SpeechEvent(SpeechEventType.KillConfirm, 1.5f) : new SpeechEvent(SpeechEventType.SniperKilled, 1.5f)));
			mEvents.Add(item);
			mTimeOfLastOnDeath = Time.time;
		}
	}

	private void OnGrenade(object sender)
	{
		Actor actor = sender as Actor;
		if (!(actor == null) && !(actor == myActor) && myActor.baseCharacter.VocalAccent != actor.baseCharacter.VocalAccent)
		{
			float magnitude = (actor.GetPosition() - myActor.GetPosition()).magnitude;
			if (magnitude < 20f && (myActor.behaviour.PlayerControlled || myActor.awareness.CanSee(actor)))
			{
				SpeechEvent item = new SpeechEvent(SpeechEventType.GrenadeThrown, 2f);
				mEvents.Add(item);
			}
		}
	}

	public void OnShootOrder(object sender)
	{
		if (Time.time > mTimeOfLastShootOrder + mMinimumTimeBetweenOrderAudio)
		{
			Actor actor = sender as Actor;
			if (!(actor == null) && !(actor != myActor))
			{
				SpeechEvent item = new SpeechEvent(SpeechEventType.FriendlyAttackOrder, 0.25f);
				mEvents.Add(item);
				mTimeOfLastShootOrder = Time.time;
			}
		}
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
		HealthComponent.HeathChangeEventArgs heathChangeEventArgs = (HealthComponent.HeathChangeEventArgs)args;
		if (heathChangeEventArgs.Amount >= 0f || heathChangeEventArgs.DamageType == "Script")
		{
			return;
		}
		DoHitMarker(heathChangeEventArgs);
		float num = ((heathChangeEventArgs.SpeechMode != 0) ? mOccassionalMinimumTimeBetweenPainAudio : mMinimumTimeBetweenPainAudio);
		if (heathChangeEventArgs.HeadShot || !(Time.time > mTimeOfLastPain + num))
		{
			return;
		}
		bool flag = false;
		if (myActor.realCharacter.IsFirstPerson && (bool)heathChangeEventArgs.From)
		{
			Actor component = heathChangeEventArgs.From.GetComponent<Actor>();
			if ((bool)component && (bool)component.realCharacter && component.realCharacter.IsSniper)
			{
				flag = true;
			}
		}
		if (flag)
		{
			SpeechEvent item = new SpeechEvent(SpeechEventType.HitBySniper, 0.001f);
			mEvents.Add(item);
		}
		else
		{
			SpeechEvent item2 = new SpeechEvent(SpeechEventType.Pain, 0.25f);
			mEvents.Add(item2);
		}
		mTimeOfLastPain = Time.time;
	}

	private void OnPlayerCharacterAboutToBeMortallyWounded(Actor actor)
	{
		if (actor == null || actor != myActor || ((bool)OverwatchController.Instance && OverwatchController.Instance.Active))
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.health.IsMortallyWounded())
			{
				a.speech.PlayManDown();
				CharacterSFX.Instance.PlayerDeath.Play2D();
				return;
			}
		}
		CharacterSFX.Instance.PlayerDeathLastMan.Play2D();
	}

	public void PlayManDown()
	{
		SpeechEvent item = new SpeechEvent(SpeechEventType.ManDown, 2f);
		mEvents.Add(item);
	}

	public void EnemySpotted(Actor observer, Actor target)
	{
		if ((bool)target.realCharacter && target.realCharacter.IsSniper && (bool)observer.baseCharacter && observer.baseCharacter.VocalAccent == BaseCharacter.Nationality.Friendly)
		{
			float magnitude = (observer.transform.position - target.transform.position).magnitude;
			if (magnitude < 25f)
			{
				SpeechEvent item = new SpeechEvent(SpeechEventType.SniperSpotted, 0.25f);
				mEvents.Add(item);
			}
		}
	}

	public void Reload(BaseCharacter owner)
	{
		if (!SpeechCooldown && (bool)owner && owner.IsFirstPerson)
		{
			SpeechEvent item = new SpeechEvent(SpeechEventType.Reload, 0.01f);
			mEvents.Add(item);
		}
	}

	public void Healed()
	{
		if (!ActStructure.Instance || !ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			SpeechEvent item = new SpeechEvent(SpeechEventType.Healed, 0.25f);
			mEvents.Add(item);
		}
	}

	public void LostAimedShot()
	{
		if (!myActor.tasks.IsRunningTask<TaskHack>())
		{
			SpeechEvent item = new SpeechEvent(SpeechEventType.LostAimedShot, 0.25f);
			mEvents.Add(item);
		}
	}

	public void GMGDeath()
	{
		SpeechEvent item = new SpeechEvent(SpeechEventType.GMGDeath, 0.25f);
		mEvents.Add(item);
	}

	public void GMGRevive()
	{
		SpeechEvent item = new SpeechEvent(SpeechEventType.GMGRevive, 0.0001f);
		mEvents.Add(item);
	}

	public void IntelCollected()
	{
		SpeechEvent item = new SpeechEvent(SpeechEventType.IntelCollected, 0.0001f);
		mEvents.Add(item);
	}

	public void FollowMe()
	{
		if (Time.time > mTimeOfLastFollowMe + mMinimumTimeBetweenFollowMeAudio && SpeechEvent.IsValidToSpeak(this, SpeechEventType.FollowMe))
		{
			SpeechEvent item = new SpeechEvent(SpeechEventType.FollowMe, 0.0001f);
			mEvents.Add(item);
			mTimeOfLastFollowMe = Time.time;
		}
	}

	public void PlayBattleChatter(object sender)
	{
		if (Time.time > mTimeOfLastBattleChatter + (mMinimumTimeBetweenBattleChatterAudio + mRandomExtraTimeForBattleChatter))
		{
			Actor actor = sender as Actor;
			if (!(actor == null) && !(actor != myActor))
			{
				SpeechEvent item = new SpeechEvent(SpeechEventType.BattleChatter, 0.25f);
				mEvents.Add(item);
				mTimeOfLastBattleChatter = Time.time;
				mRandomExtraTimeForBattleChatter = UnityEngine.Random.Range(0f, 5f);
			}
		}
	}

	private void DoHitMarker(HealthComponent.HeathChangeEventArgs args)
	{
		if (!myActor.behaviour.PlayerControlled && (!myActor.realCharacter || !myActor.realCharacter.IsDead()) && args.DamageType == "Shot" && (bool)args.From && !args.OneShotKill && !args.HeadShot)
		{
			Actor component = args.From.GetComponent<Actor>();
			if ((bool)component && (bool)component.realCharacter && component.realCharacter.IsFirstPerson)
			{
				WeaponSFX.Instance.HitMarker.Play2D();
			}
		}
	}
}
