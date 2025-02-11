using System.Collections.Generic;
using UnityEngine;

public class ComboObjective : MissionObjective
{
	public ComboObjectiveData m_comboInterface;

	[HideInInspector]
	public List<MissionObjective> SubObjectives;

	public override void Start()
	{
		base.Start();
		SubObjectives = ObjectiveManager.GetAllValidSubObjectives(base.gameObject, SubObjectives);
		if (SubObjectives != null && SubObjectives.Count > 0)
		{
			foreach (MissionObjective subObjective in SubObjectives)
			{
				subObjective.OnObjectivePassed += OnComboObjectivePassed;
				subObjective.OnObjectiveFailed += OnComboObjectiveFailed;
			}
			return;
		}
		Pass();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (MissionObjective subObjective in SubObjectives)
		{
			if (subObjective != null)
			{
				subObjective.OnObjectivePassed -= OnComboObjectivePassed;
				subObjective.OnObjectiveFailed -= OnComboObjectiveFailed;
			}
		}
	}

	private void CheckPassFail()
	{
		bool flag = true;
		foreach (MissionObjective subObjective in SubObjectives)
		{
			if (subObjective.m_Interface.IsPrimaryObjective)
			{
				if (subObjective.State == ObjectiveState.Failed)
				{
					Fail();
					return;
				}
				if (subObjective.State != ObjectiveState.Passed)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			Pass();
		}
	}

	private void OnComboObjectivePassed(object sender)
	{
		MissionObjective missionObjective = sender as MissionObjective;
		TBFAssert.DoAssert(missionObjective != null, "invalid sender of objective message?");
		CheckPassFail();
	}

	private void OnComboObjectiveFailed(object sender)
	{
		MissionObjective missionObjective = sender as MissionObjective;
		TBFAssert.DoAssert(missionObjective != null, "invalid sender of objective message?");
		CheckPassFail();
	}
}
