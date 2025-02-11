using System.Collections.Generic;
using UnityEngine;

public class GlobalObjectiveManager : MonoBehaviour
{
	public float MissionCompleteDelayTime = 5f;

	private ObjectiveManager mCurrentObjMan;

	private ObjectiveManager mPendingObjMan;

	private bool mCurrentlyPassed;

	private static GlobalObjectiveManager instance;

	public List<ObjectivesTagData> ObjectivesList
	{
		get
		{
			return null;
		}
	}

	public ObjectiveManager CurrentObjectiveManager
	{
		get
		{
			return mCurrentObjMan;
		}
		set
		{
			mCurrentObjMan = value;
			mCurrentlyPassed = false;
		}
	}

	public ObjectiveManager PendingObjectiveManager
	{
		get
		{
			return mPendingObjMan;
		}
		set
		{
			mPendingObjMan = value;
			if (mCurrentObjMan == null)
			{
				mCurrentObjMan = value;
			}
		}
	}

	public bool CurrentlyPassed
	{
		get
		{
			return mCurrentlyPassed;
		}
	}

	public static GlobalObjectiveManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		TBFAssert.DoAssert(instance == null, "Trying to register multiple Global Objective Managers");
		instance = this;
	}

	private void Start()
	{
		if (CheckpointManager.Instance != null && CheckpointManager.Instance.LoadFromSavePoint)
		{
			CheckpointManager.Instance.LoadCheckPointObjectiveCompletions();
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public bool AllSecondaryObjectivesComplete()
	{
		return true;
	}

	public int MaxNumberOfCasualties()
	{
		for (int i = 0; i < CurrentObjectiveManager.Objectives.Count; i++)
		{
			SquadCasualtiesObjective squadCasualtiesObjective = CurrentObjectiveManager.Objectives[i] as SquadCasualtiesObjective;
			if (squadCasualtiesObjective != null)
			{
				return squadCasualtiesObjective.MaxPlayerCasualties;
			}
		}
		return 4;
	}

	public void SectionPassed()
	{
		mCurrentlyPassed = true;
		MissionCompletedCheck();
	}

	public void MissionCompletedCheck()
	{
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if (sectionManager != null)
		{
			GameController.Instance.OnMissionPassed(this, MissionCompleteDelayTime);
		}
	}

	public void SectionFailedGameOver(MissionObjective mo)
	{
		GameController.Instance.OnMissionFailed(mo);
	}

	public bool SafeToGotoNextSection()
	{
		if (CurrentObjectiveManager == null)
		{
			return true;
		}
		return mCurrentlyPassed;
	}

	public void AddTagData(MissionObjective mo)
	{
	}

	public void MarkMissionTagAsCompleted(MissionObjective mo)
	{
	}

	public static void AddMissionObjectivesToGlobalManager()
	{
		GlobalObjectiveManager globalObjectiveManager = Object.FindObjectOfType(typeof(GlobalObjectiveManager)) as GlobalObjectiveManager;
		if (globalObjectiveManager == null)
		{
			Debug.LogError("No Global Objective Manager in the scene!");
		}
		MissionObjective[] array = Object.FindObjectsOfType(typeof(MissionObjective)) as MissionObjective[];
		MissionObjective[] array2 = array;
		foreach (MissionObjective mo in array2)
		{
			globalObjectiveManager.AddTagData(mo);
		}
	}

	public void SetPendingObjectiveManagerActive()
	{
		if (PendingObjectiveManager != null)
		{
			CurrentObjectiveManager = PendingObjectiveManager;
			CurrentObjectiveManager.ActivateObjectives();
		}
	}
}
