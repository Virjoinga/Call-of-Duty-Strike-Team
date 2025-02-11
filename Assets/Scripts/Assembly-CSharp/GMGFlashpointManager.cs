using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMGFlashpointManager : MonoBehaviour
{
	private int mKills;

	private bool mFinished;

	public List<GameObject> Coordinators = new List<GameObject>();

	public GameObject Smorc;

	public List<GameObject> Collectables = new List<GameObject>();

	public List<GameObject> Targets = new List<GameObject>();

	public Transform PlayerSpawner;

	public GameObject mObjective_Object;

	private List<SpawnerCoordinator> mCoordinators = new List<SpawnerCoordinator>();

	private SendMessageOnRecievedCount mSmorc;

	private List<PickUpObject> mCollectables = new List<PickUpObject>();

	private List<PlacedC4> mTargets = new List<PlacedC4>();

	private List<int> mTargetIdsHidden = new List<int>();

	private List<int> mRandNumbers = new List<int>();

	private float mTimeSurvived;

	private string dial = string.Empty;

	private bool mWarningDelay = true;

	private bool doesTimeOutMeanSuccess;

	private bool missionSuccess;

	private int mInternalObjectiveCount;

	private bool mTriggeredFirstTime;

	private ScriptedObjective mObjective;

	private bool mHasDifficultyIncreased;

	private int[,] WaveNumbersForLevel = new int[3, 7];

	public float TimeSurvived
	{
		get
		{
			return mTimeSurvived;
		}
		set
		{
			mTimeSurvived = value;
		}
	}

	private void Awake()
	{
		OnDisable();
		WaveStats.Instance.Reset();
		foreach (GameObject coordinator in Coordinators)
		{
			mCoordinators.Add(coordinator.GetComponentInChildren<SpawnerCoordinator>());
		}
		foreach (GameObject collectable in Collectables)
		{
			mCollectables.Add(collectable.GetComponentInChildren<PickUpObject>());
		}
		foreach (GameObject target in Targets)
		{
			mTargets.Add(target.GetComponentInChildren<PlacedC4>());
		}
		mSmorc = Smorc.GetComponentInChildren<SendMessageOnRecievedCount>();
	}

	public void SetStartWave(string val)
	{
	}

	public void UpdateWave()
	{
	}

	private void UpdateScore(object sender, Events.XPEarned args)
	{
		EventHub.Instance.Report(new Events.GMGScoreAdded(args.XP));
		mKills++;
		if (mFinished)
		{
			return;
		}
		MissionListings.FlashpointData currentFlashPointData = MissionListings.Instance.CurrentFlashPointData;
		switch (currentFlashPointData.CurrentObjective)
		{
		case MissionListings.FlashpointData.Objective.Survive:
			break;
		case MissionListings.FlashpointData.Objective.Clear:
			if (mKills >= currentFlashPointData.RequiredKills)
			{
				mSmorc.ChangeTarget(GKM.UnitCount(GKM.FactionMask(FactionHelper.Category.Enemy) & GKM.AliveMask));
				StopSpawning();
				mFinished = true;
			}
			break;
		case MissionListings.FlashpointData.Objective.Collect:
			break;
		}
	}

	private void UpdateEnemiesRemaining(GameObject sender)
	{
		SendMessageOnRecievedCount componentInChildren = sender.GetComponentInChildren<SendMessageOnRecievedCount>();
		if ((bool)componentInChildren)
		{
			CommonHudController.Instance.UpdateEnemiesRemaining(componentInChildren.Target - componentInChildren.Count, componentInChildren.Target);
		}
		mInternalObjectiveCount++;
		MissionListings.FlashpointData currentFlashPointData = MissionListings.Instance.CurrentFlashPointData;
		if (currentFlashPointData.CurrentObjective == MissionListings.FlashpointData.Objective.Collect)
		{
			float num = (float)componentInChildren.Count / (float)componentInChildren.Target;
			if ((double)num >= 0.5 && !mHasDifficultyIncreased)
			{
				foreach (PickUpObject mCollectable in mCollectables)
				{
					if ((bool)mCollectable && mCollectable.gameObject.activeInHierarchy)
					{
						mCollectable.BlipUp();
					}
				}
				SetNextDifficulty(1);
				mHasDifficultyIncreased = true;
			}
		}
		if (currentFlashPointData.CurrentObjective == MissionListings.FlashpointData.Objective.Destroy)
		{
			float num2 = (float)componentInChildren.Count / (float)componentInChildren.Target;
			if ((double)num2 >= 0.5 && !mHasDifficultyIncreased)
			{
				SetNextDifficulty(1);
				mHasDifficultyIncreased = true;
			}
		}
		if (componentInChildren.Target < mInternalObjectiveCount)
		{
			CommonHudController.Instance.ShowEnemiesRemaining(false);
			missionSuccess = true;
			CommonHudController.Instance.MissionTimer.StopTimer();
			StartCoroutine("EndMission");
		}
	}

	private void OnC4Placed()
	{
		if (mTargetIdsHidden.Count > 0)
		{
			int index = mTargetIdsHidden[0];
			mTargets[index].BlipUp();
			Targets[index].SetActive(true);
			mTargetIdsHidden.RemoveAt(0);
		}
		if (!mFinished && mTriggeredFirstTime)
		{
			int fPTimeBonusDestroy = GMGData.Instance.FPTimeBonusDestroy;
			string msg = Language.Get("S_FL_DESTROY_EXPLOSIVEPLACED") + " +" + fPTimeBonusDestroy + Language.Get("S_TIME_SECONDS").ToUpper();
			CommonHudController.Instance.AddXpFeedback(0, msg, null);
			CommonHudController.Instance.MissionTimer.Add(fPTimeBonusDestroy);
			CommonHudController.Instance.MissionTimer.StartTimer();
		}
		mTriggeredFirstTime = true;
	}

	public void C4SetPieceStart()
	{
		CommonHudController.Instance.MissionTimer.PauseTimer();
	}

	private void StopSpawning()
	{
		foreach (SpawnerCoordinator mCoordinator in mCoordinators)
		{
			Container.SendMessage(mCoordinator.gameObject, "Deactivate");
		}
	}

	private void SetStartTime(int val)
	{
		CommonHudController.Instance.MissionTimer.Set(val, 0f);
	}

	public void SetupFlashpointMission()
	{
		MissionListings.FlashpointData currentFlashPointData = MissionListings.Instance.CurrentFlashPointData;
		int num = 0;
		foreach (GameObject collectable in Collectables)
		{
			collectable.SetActive(false);
		}
		foreach (GameObject target in Targets)
		{
			target.SetActive(false);
		}
		TweakSpawningForGameMode(currentFlashPointData.CurrentObjective);
		switch (currentFlashPointData.CurrentObjective)
		{
		case MissionListings.FlashpointData.Objective.Survive:
			if (mObjective != null)
			{
				mObjective.m_Interface.ObjectiveLabel = "S_FL_SURVIVE_OBJECTIVE";
				mObjective.Activate();
			}
			WaveNumbersForLevel = GMGData.Instance.FPMaxSimGroupEnemies_Survive;
			SetNextDifficulty(0);
			break;
		case MissionListings.FlashpointData.Objective.Clear:
			if (mObjective != null)
			{
				mObjective.m_Interface.ObjectiveLabel = "S_FL_CLEAR_OBJECTIVE";
				mObjective.Activate();
			}
			break;
		case MissionListings.FlashpointData.Objective.Collect:
			if (mObjective != null)
			{
				mObjective.m_Interface.ObjectiveLabel = "S_FL_COLLECT_OBJECTIVE";
				mObjective.Activate();
			}
			num = 0;
			mSmorc.ChangeTarget(currentFlashPointData.RequiredIntel);
			GenerateRandomNumberRange(currentFlashPointData.RequiredIntel);
			for (num = 0; num < currentFlashPointData.RequiredIntel; num++)
			{
				int num2 = mRandNumbers[num];
				Collectables[num2].SetActive(true);
				mCollectables[num2].m_Interface.ObjectsToMessageOnCollection.Add(Smorc);
				mCollectables[num2].m_Interface.FunctionsToCallOnCollection.Add("Activate");
				mCollectables[num2].m_Interface.ParamToPass.Add(null);
				mCollectables[num2].m_Interface.ObjectsToMessageOnCollection.Add(base.gameObject);
				mCollectables[num2].m_Interface.FunctionsToCallOnCollection.Add("UpdateEnemiesRemaining");
				mCollectables[num2].m_Interface.ParamToPass.Add(Smorc);
			}
			foreach (SpawnerCoordinator mCoordinator in mCoordinators)
			{
				mCoordinator.m_Interface.EventsListOverride = null;
			}
			CommonHudController.Instance.ShowEnemiesRemaining(true);
			WaveNumbersForLevel = GMGData.Instance.FPMaxSimGroupEnemies_Collect;
			SetNextDifficulty(0);
			break;
		case MissionListings.FlashpointData.Objective.Destroy:
			if (mObjective != null)
			{
				mObjective.m_Interface.ObjectiveLabel = "S_FL_DESTROY_OBJECTIVE";
				mObjective.Activate();
			}
			num = 0;
			mSmorc.ChangeTarget(currentFlashPointData.RequiredTargets);
			GenerateRandomNumberRange(currentFlashPointData.RequiredTargets);
			for (num = 0; num < currentFlashPointData.RequiredTargets; num++)
			{
				int num2 = mRandNumbers[num];
				mTargetIdsHidden.Add(num2);
				mTargets[num2].mInterface.ObjectsToMessageOnCollection.Add(Smorc);
				mTargets[num2].mInterface.FunctionsToCallOnCollection.Add("Activate");
				mTargets[num2].mInterface.ParamToPass.Add(null);
				mTargets[num2].mInterface.ObjectsToMessageOnCollection.Add(base.gameObject);
				mTargets[num2].mInterface.FunctionsToCallOnCollection.Add("UpdateEnemiesRemaining");
				mTargets[num2].mInterface.ParamToPass.Add(Smorc);
				mTargets[num2].mInterface.ObjectsToMessageOnCollection.Add(base.gameObject);
				mTargets[num2].mInterface.FunctionsToCallOnCollection.Add("OnC4Placed");
				mTargets[num2].mInterface.ParamToPass.Add(null);
			}
			SortTargets();
			OnC4Placed();
			foreach (SpawnerCoordinator mCoordinator2 in mCoordinators)
			{
				mCoordinator2.m_Interface.EventsListOverride = null;
			}
			CommonHudController.Instance.ShowEnemiesRemaining(true);
			WaveNumbersForLevel = GMGData.Instance.FPMaxSimGroupEnemies_Destroy;
			SetNextDifficulty(0);
			break;
		}
		mKills = 0;
	}

	protected void OnEnable()
	{
		EventHub.Instance.OnXPEarned += UpdateScore;
		CommonHudController.Instance.ShowWave(false);
		CommonHudController.Instance.UpdateTokens(GameSettings.Instance.PlayerCash().HardCash());
		GMGData.Instance.CurrentGameType = GMGData.GameType.Flashpoint;
		if (mObjective_Object == null)
		{
			mObjective_Object = GameObject.Find("Scripted");
		}
		if (mObjective_Object != null)
		{
			mObjective = mObjective_Object.GetComponentInChildren<ScriptedObjective>();
			if (mObjective != null)
			{
				mObjective.SetDormantState();
			}
		}
		SetupFlashpointMission();
		MissionListings.FlashpointData currentFlashPointData = MissionListings.Instance.CurrentFlashPointData;
		if (currentFlashPointData.CurrentObjective == MissionListings.FlashpointData.Objective.Survive)
		{
			doesTimeOutMeanSuccess = true;
		}
		SetStartTime(currentFlashPointData.TimeLimit);
		CommonHudController.Instance.MissionTimer.StartTimer();
		StartCoroutine(TimerMonitor());
	}

	protected void OnDisable()
	{
		EventHub.Instance.OnXPEarned -= UpdateScore;
	}

	private void TweakSpawningForGameMode(MissionListings.FlashpointData.Objective gm)
	{
		switch (gm)
		{
		case MissionListings.FlashpointData.Objective.Collect:
		{
			foreach (SpawnerCoordinator mCoordinator in mCoordinators)
			{
				mCoordinator.m_Interface.SpawnDelay = Random.Range(2, 3);
			}
			break;
		}
		case MissionListings.FlashpointData.Objective.Destroy:
		{
			foreach (SpawnerCoordinator mCoordinator2 in mCoordinators)
			{
				mCoordinator2.m_Interface.SpawnDelay = Random.Range(5, 5);
			}
			break;
		}
		}
	}

	private IEnumerator TimerMonitor()
	{
		float nextBleep = 5f;
		float curTime2 = 0f;
		int currentWarning = 1;
		int difficultyLevel = 0;
		int difficultyInterval = GMGData.Instance.FPDifficultyInterval;
		MissionListings.FlashpointData fd = MissionListings.Instance.CurrentFlashPointData;
		while (CommonHudController.Instance.MissionTimer.CurrentTime() > 0f)
		{
			curTime2 = CommonHudController.Instance.MissionTimer.CurrentTime();
			if (fd.CurrentObjective == MissionListings.FlashpointData.Objective.Survive && curTime2 <= (float)(fd.TimeLimit - difficultyInterval))
			{
				difficultyInterval += difficultyInterval;
				difficultyLevel++;
				SetNextDifficulty(difficultyLevel);
			}
			if (curTime2 <= 7f && mWarningDelay)
			{
				StartCoroutine("WarningDelay");
				switch (currentWarning)
				{
				case 1:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR1";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning++;
					break;
				case 2:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR2";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning++;
					break;
				case 3:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR1";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning = 1;
					break;
				}
			}
			if (curTime2 <= nextBleep)
			{
				if (curTime2 <= 1f)
				{
					GMGSFX.Instance.NextWaveCountdownLast.Play2D();
				}
				else
				{
					GMGSFX.Instance.NextWaveCountdown.Play2D();
				}
				nextBleep -= 1f;
			}
			else if (curTime2 >= 5f && nextBleep < 5f)
			{
				nextBleep = 5f;
			}
			yield return null;
		}
		dial = "S_TT_DIALOGUE_TIMEOUT";
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
		if (doesTimeOutMeanSuccess)
		{
			missionSuccess = true;
		}
		StartCoroutine("EndMission");
	}

	private IEnumerator WarningDelay()
	{
		mWarningDelay = false;
		yield return new WaitForSeconds(20f);
		mWarningDelay = true;
	}

	private IEnumerator EndMission()
	{
		GameObject mh2 = GameObject.Find("Music_Danger");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		mh2 = GameObject.Find("Music_Combat");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		CommonHudController.Instance.ShowGMGResults(true);
		TutorialToggles.LockFPPMovement = true;
		CommonHudController.Instance.TriggerLocked = true;
		mFinished = true;
		if (missionSuccess)
		{
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_WIN"), string.Empty, string.Empty, false);
		}
		else
		{
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_FAIL"), string.Empty, string.Empty, false);
		}
		SendMessageOnRecievedCount sm = Smorc.GetComponentInChildren<SendMessageOnRecievedCount>();
		Container.SendMessage(sm.m_Interface.ObjectToMessage, "Activate");
		StopSpawning();
		GameController.Instance.mFirstPersonActor.awareness.CanBeLookedAt = false;
		GameController.Instance.mFirstPersonActor.awareness.FlushAllAwareness();
		GameController.Instance.mFirstPersonActor.health.Invulnerable = true;
		float wait = 5f;
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if (GlobalObjectiveManager.Instance == null)
		{
			CommonHudController.Instance.TriggerLocked = false;
			GameController.Instance.OverrideTimePlayed(TimeSurvived);
			if (missionSuccess)
			{
				GameController.Instance.OnMissionPassed(this, 1f);
			}
			else
			{
				GameController.Instance.OnMissionFailed(null);
			}
		}
		else
		{
			GlobalObjectiveManager.Instance.SectionFailedGameOver(null);
		}
	}

	private void GenerateRandomNumberRange(int RangeSize)
	{
		mRandNumbers.Clear();
		int i;
		for (i = 0; i < RangeSize; i++)
		{
			mRandNumbers.Add(i);
		}
		i = RangeSize;
		while (i > 1)
		{
			int index = Random.Range(0, RangeSize);
			i--;
			int value = mRandNumbers[index];
			mRandNumbers[index] = mRandNumbers[i];
			mRandNumbers[i] = value;
		}
	}

	private void SetNextDifficulty(int level)
	{
		int num = 0;
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		List<int> list2 = list;
		switch (level)
		{
		case 0:
			list2[0] = WaveNumbersForLevel[level, 0];
			list2[1] = WaveNumbersForLevel[level, 1];
			list2[2] = WaveNumbersForLevel[level, 2];
			list2[3] = WaveNumbersForLevel[level, 3];
			list2[4] = WaveNumbersForLevel[level, 4];
			list2[5] = WaveNumbersForLevel[level, 5];
			list2[6] = WaveNumbersForLevel[level, 6];
			break;
		case 1:
			list2[0] = WaveNumbersForLevel[level, 0];
			list2[1] = WaveNumbersForLevel[level, 1];
			list2[2] = WaveNumbersForLevel[level, 2];
			list2[3] = WaveNumbersForLevel[level, 3];
			list2[4] = WaveNumbersForLevel[level, 4];
			list2[5] = WaveNumbersForLevel[level, 5];
			list2[6] = WaveNumbersForLevel[level, 6];
			break;
		case 2:
			list2[0] = WaveNumbersForLevel[level, 0];
			list2[1] = WaveNumbersForLevel[level, 1];
			list2[2] = WaveNumbersForLevel[level, 2];
			list2[3] = WaveNumbersForLevel[level, 3];
			list2[4] = WaveNumbersForLevel[level, 4];
			list2[5] = WaveNumbersForLevel[level, 5];
			list2[6] = WaveNumbersForLevel[level, 6];
			break;
		default:
			list2[0] = WaveNumbersForLevel[2, 0];
			list2[1] = WaveNumbersForLevel[2, 1];
			list2[2] = WaveNumbersForLevel[2, 2];
			list2[3] = WaveNumbersForLevel[2, 3];
			list2[4] = WaveNumbersForLevel[2, 4];
			list2[5] = WaveNumbersForLevel[2, 5];
			list2[6] = WaveNumbersForLevel[2, 6];
			break;
		}
		if (Coordinators.Count > 6)
		{
			list2[6] = GMGData.Instance.TTMaxSimGroupEnemies[5, 6];
		}
		foreach (SpawnerCoordinator mCoordinator in mCoordinators)
		{
			mCoordinator.m_Interface.MaxSimultaneousEnemies = list2[num];
			num++;
		}
	}

	private void SortTargets()
	{
		int num = 0;
		int[] array = new int[mTargetIdsHidden.Count];
		Vector3 position;
		if (PlayerSpawner != null)
		{
			position = PlayerSpawner.position;
		}
		else
		{
			PlayerOverride playerOverride = Object.FindObjectOfType<PlayerOverride>();
			position = playerOverride.transform.position;
		}
		List<int> list = new List<int>(mTargetIdsHidden);
		while (list.Count > 0)
		{
			int bestTarget = GetBestTarget(position, list);
			array[num++] = bestTarget;
			list.Remove(bestTarget);
			position = Targets[bestTarget].transform.position;
		}
		for (int i = 0; i < array.Length; i++)
		{
			mTargetIdsHidden[i] = array[i];
		}
	}

	private int GetBestTarget(Vector3 pathOrigin, List<int> targetIds)
	{
		List<int> list = new List<int>(targetIds);
		Dictionary<int, int> pathCosts = new Dictionary<int, int>(targetIds.Count);
		for (int i = 0; i < list.Count; i++)
		{
			float num = float.MaxValue;
			NavMeshPath navMeshPath = new NavMeshPath();
			if (NavMesh.CalculatePath(pathOrigin, Targets[list[i]].transform.position, -1, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				num = 0f;
				for (int j = 0; j < navMeshPath.corners.Length - 1; j++)
				{
					num += (navMeshPath.corners[j + 1] - navMeshPath.corners[j]).sqrMagnitude;
				}
			}
			pathCosts[list[i]] = (int)num;
		}
		list.Sort((int targetIdFirst, int targetIdSecond) => Comparer<int>.Default.Compare(pathCosts[targetIdFirst], pathCosts[targetIdSecond]));
		int index = -1;
		if (list.Count >= 3)
		{
			index = list.Count / 2 + Random.Range(-1, 2);
		}
		else if (list.Count == 2)
		{
			index = Random.Range(0, 2);
		}
		else if (list.Count == 1)
		{
			index = 0;
		}
		else
		{
			Debug.LogError("GMGFlashpointManager => SortTargets. Something has gone wrong! Working with an empty list.");
		}
		return list[index];
	}
}
