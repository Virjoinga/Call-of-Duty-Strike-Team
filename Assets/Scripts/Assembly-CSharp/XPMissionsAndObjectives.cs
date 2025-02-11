using System;
using UnityEngine;

[Serializable]
public class XPMissionsAndObjectives
{
	[SerializeField]
	private XPType MissionPassedNormal;

	[SerializeField]
	private XPType MissionPassedVeteran;

	[SerializeField]
	private XPType SecondaryObjectiveNormal;

	[SerializeField]
	private XPType SecondaryObjectiveVeteran;

	[SerializeField]
	private XPType Intel;

	[SerializeField]
	private XPType EarnedMedalNormal;

	[SerializeField]
	private XPType EarnedMedalVeteran;

	public int MissionPassedXP(DifficultyMode mode)
	{
		return (mode != 0) ? MissionPassedVeteran.GetXP() : MissionPassedNormal.GetXP();
	}

	public int SecondaryObjectiveXP(DifficultyMode mode)
	{
		return (mode != 0) ? SecondaryObjectiveVeteran.GetXP() : SecondaryObjectiveNormal.GetXP();
	}

	public int EarnedMedalXP(DifficultyMode mode)
	{
		return (mode != 0) ? EarnedMedalVeteran.GetXP() : EarnedMedalNormal.GetXP();
	}

	public int IntelXP()
	{
		return Intel.GetXP();
	}
}
