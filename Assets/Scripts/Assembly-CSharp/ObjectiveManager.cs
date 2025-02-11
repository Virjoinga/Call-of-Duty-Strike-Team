using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	public GameObject ObjectiveBlipPrefab;

	public List<MissionObjective> Objectives;

	public string Tag;

	private List<MissionObjective> mVisibleObjectivesInOrder;

	public List<MissionObjective> ObjectivesInOrder
	{
		get
		{
			return mVisibleObjectivesInOrder;
		}
	}

	private void Awake()
	{
		Objectives = GetAllValidSubObjectives(base.gameObject, Objectives);
		mVisibleObjectivesInOrder = new List<MissionObjective>();
		foreach (MissionObjective objective in Objectives)
		{
			if (objective != null)
			{
				objective.OnObjectivePassed += OnObjectivePassed;
				objective.OnObjectiveFailed += OnObjectiveFailed;
				objective.OnObjectiveInProgress += OnObjectiveInProgress;
				objective.MyObjectiveManager = this;
			}
		}
	}

	private void Start()
	{
		if (GlobalObjectiveManager.Instance != null)
		{
			GlobalObjectiveManager.Instance.PendingObjectiveManager = this;
		}
	}

	private void GetVisibleActiveComboObjectiveStrings(ComboObjective co, ref List<string> objetiveText)
	{
		foreach (MissionObjective subObjective in co.SubObjectives)
		{
			ComboObjective comboObjective = subObjective as ComboObjective;
			if (comboObjective != null)
			{
				GetVisibleActiveComboObjectiveStrings(comboObjective, ref objetiveText);
			}
			else
			{
				GetVisibleActiveObjectiveStrings(subObjective, ref objetiveText);
			}
		}
	}

	private void GetVisibleActiveObjectiveStrings(MissionObjective mo, ref List<string> objetiveText)
	{
		if (mo != null && mo.m_Interface.IsVisible && mo.State == MissionObjective.ObjectiveState.InProgress)
		{
			objetiveText.Add(mo.m_Interface.ObjectiveLabel);
		}
	}

	public string[] GetVisibleActiveObjectiveStrings()
	{
		List<string> objetiveText = new List<string>();
		if (Objectives != null)
		{
			foreach (MissionObjective objective in Objectives)
			{
				ComboObjective comboObjective = objective as ComboObjective;
				if (comboObjective != null)
				{
					GetVisibleActiveComboObjectiveStrings(comboObjective, ref objetiveText);
				}
				else
				{
					GetVisibleActiveObjectiveStrings(objective, ref objetiveText);
				}
			}
		}
		return objetiveText.ToArray();
	}

	public static List<MissionObjective> GetAllValidSubObjectives(GameObject root, List<MissionObjective> knownObjectives)
	{
		MissionObjective[] componentsInChildren = root.GetComponentsInChildren<MissionObjective>();
		HashSet<MissionObjective> hashSet = new HashSet<MissionObjective>(componentsInChildren);
		List<MissionObjective> list = new List<MissionObjective>(hashSet);
		foreach (MissionObjective item in list)
		{
			if (item.gameObject.activeInHierarchy)
			{
				bool flag = false;
				Transform parent = item.transform;
				while (parent != root && parent != null)
				{
					if ((bool)parent.gameObject.GetComponent<ComboObjective>())
					{
						flag = true;
						break;
					}
					parent = parent.transform.parent;
				}
				if (!flag)
				{
					continue;
				}
				if (item.gameObject == root)
				{
					ComboObjective comboObjective = item as ComboObjective;
					if (comboObjective == null)
					{
						continue;
					}
				}
			}
			hashSet.Remove(item);
		}
		if (knownObjectives != null)
		{
			hashSet.UnionWith(knownObjectives);
		}
		return new List<MissionObjective>(hashSet);
	}

	public void RegisterObjective(MissionObjective objective)
	{
		Objectives.Add(objective);
	}

	public void DeregisterObjective(MissionObjective objective)
	{
		Objectives.Remove(objective);
	}

	private void OnObjectivePassed(object sender)
	{
		if (GameController.Instance.MissionEnding)
		{
			return;
		}
		MissionObjective missionObjective = sender as MissionObjective;
		TBFAssert.DoAssert(missionObjective != null, "invalid sender of objective message?");
		if (GlobalObjectiveManager.Instance != null)
		{
			GlobalObjectiveManager.Instance.MarkMissionTagAsCompleted(missionObjective);
		}
		if (missionObjective.m_Interface.IsPrimaryObjective)
		{
			bool flag = true;
			foreach (MissionObjective objective in Objectives)
			{
				if (objective != null && objective.m_Interface.IsPrimaryObjective && objective.State != MissionObjective.ObjectiveState.Passed && !objective.MissionPassIfNotFail)
				{
					flag = false;
				}
			}
			if (flag)
			{
				if (GlobalObjectiveManager.Instance == null)
				{
					GameController.Instance.OnMissionPassed(this, 5f);
				}
				else
				{
					GlobalObjectiveManager.Instance.SectionPassed();
				}
			}
		}
		if (missionObjective.m_Interface.IsVisible)
		{
			CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get("S_OBJECTIVECOMPLETE"));
		}
	}

	private void OnObjectiveFailed(object sender)
	{
		if (GameController.Instance.MissionEnding)
		{
			return;
		}
		MissionObjective missionObjective = sender as MissionObjective;
		TBFAssert.DoAssert(missionObjective != null, "invalid sender of objective message?");
		if (missionObjective.m_Interface.IsPrimaryObjective)
		{
			if (GlobalObjectiveManager.Instance == null)
			{
				GameController.Instance.OnMissionFailed(missionObjective);
			}
			else
			{
				GlobalObjectiveManager.Instance.SectionFailedGameOver(missionObjective);
			}
		}
		if (missionObjective.m_Interface.IsVisible)
		{
			CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get(missionObjective.m_Interface.ObjectiveLabel) + "-" + AutoLocalize.Get("S_FAIL"));
		}
	}

	private void OnObjectiveInProgress(object sender)
	{
		if (GameController.Instance.MissionEnding)
		{
			return;
		}
		MissionObjective missionObjective = sender as MissionObjective;
		TBFAssert.DoAssert(missionObjective != null, "invalid sender of objective message?");
		if (missionObjective.m_Interface.IsVisible)
		{
			if (mVisibleObjectivesInOrder.Contains(missionObjective))
			{
				mVisibleObjectivesInOrder.Remove(missionObjective);
			}
			mVisibleObjectivesInOrder.Add(missionObjective);
		}
	}

	public bool AllSecondaryObjectivesComplete()
	{
		bool result = true;
		foreach (MissionObjective objective in Objectives)
		{
			if (objective != null && !objective.m_Interface.IsPrimaryObjective && objective.State != MissionObjective.ObjectiveState.Passed)
			{
				result = false;
			}
		}
		return result;
	}

	public void ActivateObjectives()
	{
		foreach (MissionObjective objective in Objectives)
		{
			if (objective != null)
			{
				objective.ActivateSectionObjective();
			}
		}
	}

	public int MaxNumberOfCasualties()
	{
		for (int i = 0; i < Objectives.Count; i++)
		{
			SquadCasualtiesObjective squadCasualtiesObjective = Objectives[i] as SquadCasualtiesObjective;
			if (squadCasualtiesObjective != null)
			{
				return squadCasualtiesObjective.MaxPlayerCasualties;
			}
		}
		return 4;
	}
}
