using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreachSequence : MonoBehaviour
{
	public AnimationClip BreachAnimation;

	public float TimeToStartEffects;

	public Transform BreachStartLocator;

	public float MaximumSlowDownDuration;

	public GameObject Door;

	public GameObject BreachComponents;

	public int RequiredActors;

	public BreachMessages BreachMessages = new BreachMessages();

	public TimedSoundFx[] RealtimeSounds;

	public FadeSoundGroup[] BeginFadeSoundGroups;

	public FadeSoundGroup[] EndFadeSoundGroups;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public bool Available { get; private set; }

	public void Awake()
	{
		Available = true;
		BuildingDoor componentInChildren = Door.GetComponentInChildren<BuildingDoor>();
		if (componentInChildren != null)
		{
			componentInChildren.BreachSequence = this;
		}
	}

	public void StartSequence(Actor actor)
	{
		Available = false;
		GameController.Instance.IsPlayerBreaching = true;
		StartCoroutine(RunSequence(actor));
	}

	private void UpdateBreachAnimation(float breachTime)
	{
		ViewModelRig.Instance().SetOverride("C4", BreachAnimation, breachTime, BreachStartLocator.position, BreachStartLocator.rotation, true);
	}

	private IEnumerator RunSequence(Actor actor)
	{
		ActorIdentIterator aii = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		bool bThrowMessage = false;
		int iAliveActors = 0;
		Actor pc;
		while (aii.NextActor(out pc))
		{
			if (pc.realCharacter.IsMortallyWounded())
			{
				if (RequiredActors == -1)
				{
					bThrowMessage = true;
				}
			}
			else
			{
				iAliveActors++;
			}
		}
		if (bThrowMessage | (RequiredActors != -1 && iAliveActors < RequiredActors))
		{
			FrontEndController.Instance.TransitionTo(ScreenID.ContinueScreen);
			yield return new WaitForSeconds(1f);
			while (FrontEndController.Instance.ActiveScreen == ScreenID.ContinueScreen)
			{
				yield return null;
			}
			bool bRevived = true;
			aii = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
			while (aii.NextActor(out pc))
			{
				if (pc.realCharacter.IsDead())
				{
					bRevived = false;
				}
			}
			if (!bRevived)
			{
				yield break;
			}
		}
		StartCoroutine(RunRealTimeSounds());
		StartCoroutine(RunBeginFadeSoundGroups());
		GameController.Instance.SwitchToFirstPerson(actor, true);
		GameController.Instance.IsLockedToFirstPerson = true;
		GameController.Instance.IsLockedToCurrentCharacter = true;
		GameController.Instance.FirstPersonAccuracyCheat = true;
		actor.weapon.ReadyForBreach();
		actor.realCharacter.FirstPersonCamera.Angles = actor.transform.eulerAngles;
		BreachActor[] breachActors = BreachComponents.GetComponentsInChildren<BreachActor>();
		ChunkCannon[] chunkCannons = BreachComponents.GetComponentsInChildren<ChunkCannon>();
		HostageSequence[] hostageSequences = BreachComponents.GetComponentsInChildren<HostageSequence>();
		BuildingDoor[] doors = new BuildingDoor[1] { Door.GetComponentInChildren<BuildingDoor>() };
		float breachTime = 0f;
		bool effectsStarted = false;
		if (actor != null)
		{
			if (actor.realCharacter.IsAimingDownSights)
			{
				actor.realCharacter.IsAimingDownSights = false;
			}
			actor.baseCharacter.Stand();
			CommonHudController.Instance.SetCrouchButtonFrame(false);
		}
		SendMessages(BreachMessages.SequenceStarted);
		while (breachTime < BreachAnimation.length)
		{
			UpdateBreachAnimation(breachTime);
			if (breachTime > TimeToStartEffects && !effectsStarted)
			{
				TimeManager.instance.SlowDownTime(0f, TimeManager.SlowTimeType.Breach);
				SoundManager.Instance.ActivateBreachSFX();
				StartCoroutine(SequenceFinishedChecks(breachActors, hostageSequences));
				BreachActor[] array = breachActors;
				foreach (BreachActor breachActor in array)
				{
					breachActor.React();
				}
				ChunkCannon[] array2 = chunkCannons;
				foreach (ChunkCannon cannon in array2)
				{
					cannon.Fire();
				}
				HostageSequence[] array3 = hostageSequences;
				foreach (HostageSequence hostageSequence in array3)
				{
					hostageSequence.TriggerFinalSequence();
				}
				BuildingDoor[] array4 = doors;
				foreach (BuildingDoor door in array4)
				{
					door.Breach(actor);
				}
				SendMessages(BreachMessages.BreachStarted);
				effectsStarted = true;
			}
			breachTime += ((!effectsStarted) ? 1f : 2f) * Time.deltaTime;
			yield return null;
		}
		ViewModelRig.Instance().ClearOverride();
		actor.weapon.TakeOut(4f);
		for (float moveForwardTime = 1f; moveForwardTime > 0f; moveForwardTime -= Time.deltaTime)
		{
			GameController.Instance.LastVelocity += Time.deltaTime * moveForwardTime * 10f * Vector2.up;
		}
	}

	private IEnumerator RunRealTimeSounds()
	{
		float time = 0f;
		List<TimedSoundFx> sortedSounds = new List<TimedSoundFx>(RealtimeSounds);
		sortedSounds.Sort((TimedSoundFx x, TimedSoundFx y) => Comparer<float>.Default.Compare(x.Delay, y.Delay));
		while (sortedSounds.Count > 0)
		{
			if (time >= sortedSounds[0].Delay)
			{
				sortedSounds[0].Sound.Play();
				sortedSounds.RemoveAt(0);
			}
			else
			{
				time += TimeManager.DeltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator RunBeginFadeSoundGroups()
	{
		return RunFadeSoundGroups(BeginFadeSoundGroups);
	}

	private IEnumerator RunEndFadeSoundGroups()
	{
		return RunFadeSoundGroups(EndFadeSoundGroups);
	}

	private IEnumerator RunFadeSoundGroups(IEnumerable<FadeSoundGroup> fadeSoundGroups)
	{
		float time = 0f;
		List<FadeSoundGroup> sortedGroups = new List<FadeSoundGroup>(fadeSoundGroups);
		sortedGroups.Sort((FadeSoundGroup x, FadeSoundGroup y) => Comparer<float>.Default.Compare(x.Delay, y.Delay));
		while (sortedGroups.Count > 0)
		{
			if (time >= sortedGroups[0].Delay)
			{
				ExecuteFadeSoundGroup(sortedGroups[0]);
				sortedGroups.RemoveAt(0);
			}
			else
			{
				time += TimeManager.DeltaTime;
				yield return null;
			}
		}
	}

	private void ExecuteFadeSoundGroup(FadeSoundGroup fadeSoundGroup)
	{
		VolumeGroupFader volumeGroupFader = SoundManager.Instance.gameObject.AddComponent<VolumeGroupFader>();
		volumeGroupFader.TimeToFade = fadeSoundGroup.Duration;
		volumeGroupFader.VolumeGroupToFade = fadeSoundGroup.Group;
		volumeGroupFader.DesiredVolume = fadeSoundGroup.DesiredVolume;
	}

	private IEnumerator SequenceFinishedChecks(IEnumerable<BreachActor> breachActors, IEnumerable<HostageSequence> hostageSequences)
	{
		float time = 0f;
		bool canFinish = false;
		float maximumSlowDownDurationScaled = MaximumSlowDownDuration * TimeManager.TIMESCALE_BREACH;
		while (!canFinish)
		{
			canFinish = true;
			if (time < maximumSlowDownDurationScaled)
			{
				foreach (BreachActor breachActor in breachActors)
				{
					if (breachActor.Alive)
					{
						canFinish = false;
					}
				}
				foreach (HostageSequence hostageSequence in hostageSequences)
				{
					if (!hostageSequence.Complete)
					{
						canFinish = false;
					}
				}
				if (canFinish && AllEnemiesDead(breachActors, hostageSequences))
				{
					SendMessages(BreachMessages.AllEnemiesDeadDuringSlowMotion);
				}
			}
			yield return null;
			time += Time.deltaTime;
		}
		TimeManager.instance.ResumeNormalTime();
		StartCoroutine(RunEndFadeSoundGroups());
		SoundManager.Instance.DeactivateBreachSFX(false);
		GameController.Instance.IsLockedToFirstPerson = false;
		GameController.Instance.IsLockedToCurrentCharacter = false;
		GameController.Instance.FirstPersonAccuracyCheat = false;
		GameController.Instance.IsPlayerBreaching = false;
		SendMessages(BreachMessages.SlowMotionFinished);
		while (!AllEnemiesDead(breachActors, hostageSequences))
		{
			yield return null;
		}
		SendMessages(BreachMessages.AllEnemiesDead);
	}

	private bool AllEnemiesDead(IEnumerable<BreachActor> breachActors, IEnumerable<HostageSequence> hostageSequences)
	{
		foreach (BreachActor breachActor in breachActors)
		{
			if (breachActor.Alive)
			{
				return false;
			}
		}
		foreach (HostageSequence hostageSequence in hostageSequences)
		{
			if (!hostageSequence.HostageTakerDead)
			{
				return false;
			}
		}
		return true;
	}

	private void SendMessages(ObjectMessage[] messages)
	{
		foreach (ObjectMessage objectMessage in messages)
		{
			if (objectMessage.Object != null)
			{
				Container.SendMessage(objectMessage.Object, objectMessage.Message, base.gameObject);
			}
		}
	}
}
