using System.Collections.Generic;
using UnityEngine;

public class DominationCapturePoint : MonoBehaviour
{
	public enum CapturePointStatusEnum
	{
		PlayerControlled = 0,
		Neutral = 1,
		EnemyControlled = 2
	}

	public enum CharacterFaction
	{
		Player = 0,
		Enemy = 1,
		None = 2
	}

	[HideInInspector]
	public DominationCapturePointData m_Interface;

	public GameObject TriggerVolume;

	public GameObject CPMForSwitching;

	public DominationFloorMarker FloorMarker;

	public float MaxSizeFloorMarker = 5f;

	public List<DestinationRoutineWrapper> AlternateDestinationRoutines = new List<DestinationRoutineWrapper>();

	[HideInInspector]
	public CapturePointStatusEnum mCurrentStatus = CapturePointStatusEnum.Neutral;

	private float mCapturePointCurrentProgress;

	private int mEnemiesInArea;

	private int mPlayersInArea;

	private List<GameObject> mEnemyCache = new List<GameObject>();

	private List<GameObject> EnemiesInTrigger = new List<GameObject>();

	private List<Actor> PlayersInTrigger = new List<Actor>();

	private bool mCapturingInProgress;

	public GameObject ProgressBlipRef;

	private HackingBlip mProgressBlip;

	private float mCount;

	private ObjectiveBlip mBlip;

	private CommonHudController hud;

	private bool mWarningPlaying;

	private SoundFXData SFX_ObjectiveCaptureLoop;

	private string dial = string.Empty;

	private void Start()
	{
		mCapturePointCurrentProgress = 0f;
		if (ProgressBlipRef != null)
		{
			ProgressBlipRef = Object.Instantiate(ProgressBlipRef) as GameObject;
			mProgressBlip = ProgressBlipRef.GetComponent<HackingBlip>();
			if (mProgressBlip != null)
			{
				mProgressBlip.Target = base.transform;
				mProgressBlip.Target.position = base.transform.position - new Vector3(0f, 1f, 0f);
			}
		}
		GameObject gameObject = Object.Instantiate(m_Interface.ObjectiveBlipPrefab) as GameObject;
		mBlip = gameObject.GetComponent<ObjectiveBlip>();
		if ((bool)mBlip)
		{
			mBlip.mObjectiveText = Language.Get("S_DOMINATION_CAPTURE");
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.position = base.transform.position + new Vector3(0f, 3f, 0f);
			mBlip.Target = gameObject2.transform;
			ObjectiveBlip.BlipType blipType = ObjectiveBlip.BlipType.Important;
			switch (m_Interface.NameOfCapturePoint)
			{
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveA:
				blipType = ObjectiveBlip.BlipType.A;
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveB:
				blipType = ObjectiveBlip.BlipType.B;
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveC:
				blipType = ObjectiveBlip.BlipType.C;
				break;
			}
			mBlip.SetBlipType(blipType);
			mBlip.SwitchOn();
		}
		if (FloorMarker != null)
		{
			FloorMarker.SetColourBeacon(ColourChart.ObjectiveSuccess);
		}
		SFX_ObjectiveCaptureLoop = GMGSFX.Instance.ObjectiveCaptureLoop;
	}

	private void Update()
	{
		TrimEnemyList();
		if (mCurrentStatus == CapturePointStatusEnum.PlayerControlled)
		{
			if (EnemiesInTrigger.Count > 0 && !CheckPlayerInVolumeAndAlive())
			{
				CapturingArea(ColourChart.ObjectivePassed, ColourChart.ObjectiveFailed);
				mCapturingInProgress = true;
				if (mCapturePointCurrentProgress >= m_Interface.TimeForEnemyToCapture)
				{
					mBlip.mObjectiveText = Language.Get("S_DOMINATION_CAPTURE");
					AreaTakenByNewOwner(CapturePointStatusEnum.EnemyControlled);
					mCapturingInProgress = false;
				}
			}
			else if (EnemiesInTrigger.Count > 0 && CheckPlayerInVolumeAndAlive())
			{
				AreaContested();
				mCapturingInProgress = false;
			}
			else if (EnemiesInTrigger.Count == 0)
			{
				ResetAreaAfterAttemptedTakeOver();
			}
		}
		else if (mCurrentStatus == CapturePointStatusEnum.EnemyControlled)
		{
			if (CheckPlayerInVolumeAndAlive() && EnemiesInTrigger.Count == 0)
			{
				CapturingArea(ColourChart.ObjectiveFailed, ColourChart.ObjectivePassed);
				if (mCapturePointCurrentProgress >= m_Interface.TimeForPlayerToCapture)
				{
					mBlip.mObjectiveText = Language.Get("S_DOMINATION_DEFEND");
					AreaTakenByNewOwner(CapturePointStatusEnum.PlayerControlled);
					mCapturingInProgress = false;
					StopCaptureSoundLoop();
				}
			}
			else if (CheckPlayerInVolumeAndAlive() && EnemiesInTrigger.Count > 0)
			{
				AreaContested();
				StopCaptureSoundLoop();
			}
			else if (!CheckPlayerInVolumeAndAlive())
			{
				ResetAreaAfterAttemptedTakeOver();
				StopCaptureSoundLoop();
			}
		}
		else
		{
			if (mCurrentStatus != CapturePointStatusEnum.Neutral)
			{
				return;
			}
			if (EnemiesInTrigger.Count > 0 && !CheckPlayerInVolumeAndAlive())
			{
				mCapturingInProgress = true;
				CapturingArea(ColourChart.ObjectiveSuccess, ColourChart.ObjectiveFailed);
				if (mCapturePointCurrentProgress >= m_Interface.TimeForEnemyToCapture)
				{
					mBlip.mObjectiveText = Language.Get("S_DOMINATION_CAPTURE");
					AreaTakenByNewOwner(CapturePointStatusEnum.EnemyControlled);
					mCapturingInProgress = false;
				}
			}
			else if (EnemiesInTrigger.Count > 0 && CheckPlayerInVolumeAndAlive())
			{
				AreaContested();
				mCapturingInProgress = false;
				StopCaptureSoundLoop();
			}
			else if (EnemiesInTrigger.Count == 0 && CheckPlayerInVolumeAndAlive())
			{
				CapturingArea(ColourChart.ObjectiveSuccess, ColourChart.ObjectivePassed);
				mCapturingInProgress = false;
				if (mCapturePointCurrentProgress >= m_Interface.TimeForPlayerToCapture)
				{
					mBlip.mObjectiveText = Language.Get("S_DOMINATION_DEFEND");
					AreaTakenByNewOwner(CapturePointStatusEnum.PlayerControlled);
					mCapturingInProgress = false;
					StopCaptureSoundLoop();
				}
			}
			else if (EnemiesInTrigger.Count == 0 && !CheckPlayerInVolumeAndAlive())
			{
				ResetAreaAfterAttemptedTakeOver();
				mCapturingInProgress = false;
				FloorMarker.SetColourBeacon(ColourChart.ObjectiveSuccess);
				StopCaptureSoundLoop();
			}
		}
	}

	private void CapturingArea(Color CurrentOwner, Color OccupyingForce)
	{
		mProgressBlip.ShowBlip();
		mCapturePointCurrentProgress += Time.deltaTime;
		if (TimeManager.instance != null && TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused)
		{
			float progress;
			if (mCapturingInProgress)
			{
				progress = mCapturePointCurrentProgress / m_Interface.TimeForEnemyToCapture;
				if (mCount == 0f)
				{
					mBlip.PingBlip(OccupyingForce);
					mCount += Time.deltaTime;
				}
				if (mCount > 1.5f)
				{
					mCount = 0f;
				}
				else
				{
					mCount += Time.deltaTime;
				}
			}
			else
			{
				progress = mCapturePointCurrentProgress / m_Interface.TimeForPlayerToCapture;
				if (mCount == 0f)
				{
					mBlip.PingBlip(OccupyingForce);
					mCount += Time.deltaTime;
				}
				if (mCount > 1.5f)
				{
					mCount = 0f;
				}
				else
				{
					mCount += Time.deltaTime;
				}
			}
			mProgressBlip.SetProgress(progress);
			if (!mWarningPlaying)
			{
				if (EnemiesInTrigger.Count > 0 && !CheckPlayerInVolumeAndAlive())
				{
					switch (m_Interface.NameOfCapturePoint)
					{
					case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveA:
						dial = "S_DOMINATION_DIALOGUE_POINTALOST_VAR2";
						CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
						PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
						break;
					case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveB:
						dial = "S_DOMINATION_DIALOGUE_POINTBLOST_VAR3";
						CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
						PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
						break;
					case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveC:
						dial = "S_DOMINATION_DIALOGUE_POINTCLOST_VAR2";
						CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
						PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
						break;
					}
				}
				else if (EnemiesInTrigger.Count == 0 && CheckPlayerInVolumeAndAlive())
				{
					PlayCaptureSoundLoop();
				}
				mWarningPlaying = true;
			}
		}
		mBlip.ColourBlip(CurrentOwner);
		FloorMarker.SetColourBeacon(CurrentOwner);
	}

	private void PlayCaptureSoundLoop()
	{
		SoundFXData objectiveCaptureStart = GMGSFX.Instance.ObjectiveCaptureStart;
		objectiveCaptureStart.Play(base.gameObject);
		if (SFX_ObjectiveCaptureLoop != null)
		{
			SFX_ObjectiveCaptureLoop.Play(base.gameObject);
		}
	}

	private void StopCaptureSoundLoop()
	{
		if (SFX_ObjectiveCaptureLoop != null)
		{
			SFX_ObjectiveCaptureLoop.Stop(base.gameObject);
		}
	}

	private void AreaTakenByNewOwner(CapturePointStatusEnum newOwner)
	{
		mWarningPlaying = false;
		Color color = ColourChart.ObjectiveSuccess;
		mCount = 0f;
		mCapturePointCurrentProgress = 0f;
		mProgressBlip.HideBlip();
		SendEnemiesToTakeOtherPoints();
		switch (newOwner)
		{
		case CapturePointStatusEnum.EnemyControlled:
			color = ColourChart.ObjectiveFailed;
			mCurrentStatus = CapturePointStatusEnum.EnemyControlled;
			switch (m_Interface.NameOfCapturePoint)
			{
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveA:
				dial = "S_DOMINATION_DIALOGUE_POINTALOST_VAR1";
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveB:
				dial = "S_DOMINATION_DIALOGUE_POINTBLOST_VAR1";
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveC:
				dial = "S_DOMINATION_DIALOGUE_POINTCLOST_VAR1";
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
				break;
			}
			break;
		case CapturePointStatusEnum.PlayerControlled:
		{
			SoundFXData objectiveSecure = GMGSFX.Instance.ObjectiveSecure;
			objectiveSecure.Play(base.gameObject);
			color = ColourChart.ObjectivePassed;
			mCurrentStatus = CapturePointStatusEnum.PlayerControlled;
			switch (m_Interface.NameOfCapturePoint)
			{
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveA:
				HUDMessenger.Instance.PushMessage(Language.Get("S_DOMINATION_PlayerTaken"), string.Empty, string.Empty, false);
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveB:
				HUDMessenger.Instance.PushMessage(Language.Get("S_DOMINATION_PlayerTaken"), string.Empty, string.Empty, false);
				break;
			case DominationCapturePointData.CapturePointIdentityEnum.DominationObjectiveC:
				HUDMessenger.Instance.PushMessage(Language.Get("S_DOMINATION_PlayerTaken"), string.Empty, string.Empty, false);
				break;
			default:
				HUDMessenger.Instance.PushMessage(Language.Get("S_DOMINATION_PlayerTaken"), string.Empty, string.Empty, false);
				break;
			}
			break;
		}
		}
		if (mBlip != null)
		{
			mBlip.ColourBlip(color);
			mBlip.PingBlip(color);
		}
		if (FloorMarker != null)
		{
			FloorMarker.SetColourBeacon(color);
		}
	}

	private void AreaContested()
	{
		mWarningPlaying = false;
		mCount = 0f;
		mBlip.ColourBlip(ColourChart.ObjectiveNeutral);
		mProgressBlip.HideBlip();
		if (FloorMarker != null)
		{
			FloorMarker.SetColourBeacon(ColourChart.ObjectiveNeutral);
		}
	}

	private void ResetAreaAfterAttemptedTakeOver()
	{
		mWarningPlaying = false;
		mCount = 0f;
		Color color = ((mCurrentStatus == CapturePointStatusEnum.EnemyControlled) ? ColourChart.ObjectiveFailed : ((mCurrentStatus != 0) ? ColourChart.ObjectiveSuccess : ColourChart.ObjectivePassed));
		mBlip.ColourBlip(color);
		mCapturePointCurrentProgress = 0f;
		mProgressBlip.HideBlip();
		if (FloorMarker != null)
		{
			FloorMarker.SetColourBeacon(color);
		}
	}

	private void SendEnemiesToTakeOtherPoints()
	{
		if (EnemiesInTrigger.Count <= m_Interface.MaxNumberHoldingPoint || !(CPMForSwitching != null) || mCurrentStatus != CapturePointStatusEnum.EnemyControlled)
		{
			return;
		}
		for (int i = m_Interface.MaxNumberHoldingPoint; i < EnemiesInTrigger.Count; i++)
		{
			List<DestinationRoutineWrapper> list = new List<DestinationRoutineWrapper>(AlternateDestinationRoutines);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].Point.mCurrentStatus == CapturePointStatusEnum.EnemyControlled)
				{
					list.RemoveAt(num);
				}
			}
			if (list.Count > 0)
			{
				int index = Random.Range(0, list.Count);
				Container.SendMessageWithParam(CPMForSwitching, "addToActors", EnemiesInTrigger[i]);
				Container.SendMessageWithParam(CPMForSwitching, "DominationSwitchRoutine", list[index].DestinationRoutine);
				Container.SendMessage(CPMForSwitching, "clearActors");
			}
		}
	}

	private bool CheckPlayerInVolumeAndAlive()
	{
		int num = 0;
		foreach (Actor item in PlayersInTrigger)
		{
			if (!item.realCharacter.IsMortallyWounded())
			{
				num++;
			}
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	private void TrimEnemyList()
	{
		mEnemyCache.Clear();
		int num = 0;
		while (num < EnemiesInTrigger.Count)
		{
			if (EnemiesInTrigger[num] == null)
			{
				mEnemiesInArea--;
				EnemiesInTrigger.RemoveAt(num);
				continue;
			}
			Actor componentInChildren = EnemiesInTrigger[num].GetComponentInChildren<Actor>();
			if (componentInChildren.realCharacter.IsDead())
			{
				mEnemyCache.Add(EnemiesInTrigger[num]);
			}
			num++;
		}
		for (num = 0; num < mEnemyCache.Count; num++)
		{
			OnTriggerExit(mEnemyCache[num].collider);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		GameObject gameObject = col.gameObject;
		if (CheckUnitFaction(gameObject) == CharacterFaction.Enemy)
		{
			EnemiesInTrigger.Add(gameObject);
			mEnemiesInArea++;
			SendEnemiesToTakeOtherPoints();
		}
		else if (CheckUnitFaction(gameObject) == CharacterFaction.Player)
		{
			Actor component = gameObject.GetComponent<Actor>();
			PlayersInTrigger.Add(component);
			mPlayersInArea++;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		GameObject gameObject = col.gameObject;
		if (CheckUnitFaction(gameObject) == CharacterFaction.Enemy)
		{
			EnemiesInTrigger.Remove(gameObject);
		}
		else if (CheckUnitFaction(gameObject) == CharacterFaction.Player)
		{
			Actor component = gameObject.GetComponent<Actor>();
			PlayersInTrigger.Remove(component);
		}
	}

	private CharacterFaction CheckUnitFaction(GameObject mActor)
	{
		Actor component = mActor.GetComponent<Actor>();
		if (component != null)
		{
			if (component.awareness.faction == FactionHelper.Category.Enemy)
			{
				return CharacterFaction.Enemy;
			}
			if (component.awareness.faction == FactionHelper.Category.Player)
			{
				return CharacterFaction.Player;
			}
			return CharacterFaction.None;
		}
		return CharacterFaction.None;
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.red.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
