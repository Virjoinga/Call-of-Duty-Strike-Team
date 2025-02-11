using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
	private const string CheckPointString_HasCheckPoint = "CheckPointHasCheckPoint";

	private const string CheckPointString_CurrentMissionID = "CheckPointCurrentMissionID";

	private const string CheckPointString_CurrentSectionID = "CheckPointCurrentSectionID";

	private const string CheckPointString_TrooperDead = "CheckPointTrooperDead";

	private const string CheckPointString_NumberOfObjectives = "CheckPointNumberOfObjectives";

	private const string CheckPointString_PassedObjective = "CheckPointPassedObjective";

	private const int STARTING_ID = 0;

	private const int LAST_ID = 3;

	private static CheckpointManager mInstance;

	public List<CheckPointTrooper> TrooperList;

	public bool LoadFromSavePoint;

	public bool DoSaveTest;

	public static CheckpointManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	private void Awake()
	{
		TBFAssert.DoAssert(mInstance == null, "Only one check point manager allowed");
		mInstance = this;
	}

	private void Start()
	{
		if (EventHub.Instance != null)
		{
			EventHub.Instance.OnStartMission += MissionStarted;
		}
	}

	private void MissionStarted(object sender, Events.StartMission args)
	{
		TrooperList.Clear();
	}

	public void AddTrooper(ActorDescriptor actorDesc, Actor actor)
	{
		CheckPointTrooper item = new CheckPointTrooper(actorDesc.soldierIndex, actorDesc.Name, actor);
		TrooperList.Add(item);
	}

	public void RemoveTrooper(int id)
	{
		TrooperList.Remove(GetCheckPointTrooper(id));
	}

	public bool CheckForTrooper(int id, out Actor actor)
	{
		foreach (CheckPointTrooper trooper in TrooperList)
		{
			if (trooper.TrooperID == id)
			{
				actor = trooper.ActorRef;
				return true;
			}
		}
		actor = null;
		return false;
	}

	public bool CheckForDead(int id)
	{
		return SecureStorage.Instance.GetBool("CheckPointTrooperDead" + id);
	}

	public CheckPointTrooper GetCheckPointTrooper(int id)
	{
		foreach (CheckPointTrooper trooper in TrooperList)
		{
			if (trooper.TrooperID == id)
			{
				return trooper;
			}
		}
		return null;
	}

	public void SaveCheckPointData()
	{
		SecureStorage.Instance.SetBool("CheckPointHasCheckPoint", true);
		SecureStorage.Instance.SetInt("CheckPointCurrentMissionID", (int)ActStructure.Instance.CurrentMissionID);
		if (SectionManager.GetSectionManager() != null)
		{
			SecureStorage.Instance.SetInt("CheckPointCurrentSectionID", SectionManager.GetSectionIndex());
		}
		Actor actor = null;
		for (int i = 0; i <= 3; i++)
		{
			if (CheckForTrooper(i, out actor))
			{
				SecureStorage.Instance.SetBool("CheckPointTrooperDead" + i, actor.health.Health01 <= 0f);
			}
		}
		if (GlobalObjectiveManager.Instance != null && GlobalObjectiveManager.Instance.ObjectivesList != null)
		{
			SecureStorage.Instance.SetInt("CheckPointNumberOfObjectives", GlobalObjectiveManager.Instance.ObjectivesList.Count);
			for (int j = 0; j < GlobalObjectiveManager.Instance.ObjectivesList.Count; j++)
			{
				SecureStorage.Instance.SetBool("CheckPointPassedObjective" + j, GlobalObjectiveManager.Instance.ObjectivesList[j].Completed);
			}
		}
		else
		{
			SecureStorage.Instance.SetInt("CheckPointNumberOfObjectives", 0);
		}
	}

	public void ClearCheckpointData()
	{
		SecureStorage.Instance.SetBool("CheckPointHasCheckPoint", false);
	}

	public bool HasCheckPoint(out int MissionID, out int SectionID)
	{
		if (SecureStorage.Instance.GetBool("CheckPointHasCheckPoint"))
		{
			MissionID = SecureStorage.Instance.GetInt("CheckPointCurrentMissionID");
			SectionID = SecureStorage.Instance.GetInt("CheckPointCurrentSectionID");
			return true;
		}
		MissionID = -1;
		SectionID = -1;
		return false;
	}

	public void LoadCheckPointObjectiveCompletions()
	{
		if (GlobalObjectiveManager.Instance != null && GlobalObjectiveManager.Instance.ObjectivesList != null)
		{
			int @int = SecureStorage.Instance.GetInt("CheckPointNumberOfObjectives");
			TBFAssert.DoAssert(@int == GlobalObjectiveManager.Instance.ObjectivesList.Count, "check point objectives dont match up with mission's");
			for (int i = 0; i < GlobalObjectiveManager.Instance.ObjectivesList.Count; i++)
			{
				GlobalObjectiveManager.Instance.ObjectivesList[i].Completed = SecureStorage.Instance.GetBool("CheckPointPassedObjective" + i);
			}
		}
	}

	public void Update()
	{
		if (DoSaveTest)
		{
			DoSaveTest = false;
			SaveCheckPointData();
		}
	}
}
