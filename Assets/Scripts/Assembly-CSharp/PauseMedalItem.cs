using UnityEngine;

public class PauseMedalItem : MonoBehaviour
{
	public SpriteText DescriptionText;

	public SpriteText ValueText;

	public SpriteText ProgressText;

	public MedalIconController MedalIcon;

	public void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public void Setup(int count, MissionData data, int section)
	{
		SectionData sectionData = data.Sections[section];
		DifficultyMode currentMissionMode = ActStructure.Instance.CurrentMissionMode;
		bool flag = StatsHelper.HasMedalBeenEarnedGameTotal(data.MissionId, section, count, currentMissionMode);
		if (sectionData.IsSpecOps && !flag)
		{
			flag = StatsHelper.HasMedalBeenEarnedCurrentMission(count, currentMissionMode);
		}
		if (MedalIcon != null)
		{
			MedalIcon.SetMedal(sectionData.Medals[count].Icon, currentMissionMode == DifficultyMode.Veteran, flag);
		}
		if (DescriptionText != null)
		{
			DescriptionText.Text = MedalManager.GetMedalString(sectionData, count, currentMissionMode);
		}
		if (ValueText != null)
		{
			ValueText.Text = string.Format("{0}{1}", XPManager.Instance.m_XPMissionsAndObjectives.EarnedMedalXP(currentMissionMode), Language.Get("S_RESULT_XP"));
		}
		if (ProgressText != null)
		{
			if (sectionData.IsSpecOps)
			{
				ProgressText.Text = MedalManager.GetMedalProgressString(data.MissionId, section, sectionData, count, currentMissionMode);
			}
			else
			{
				ProgressText.Text = string.Empty;
			}
		}
	}

	public void LayoutComponents(float width, float height)
	{
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}
}
