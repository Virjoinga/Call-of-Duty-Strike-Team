using UnityEngine;

public class PauseMenuObjectivesController : MenuScreenBlade
{
	public ObjectiveSpriteText[] PanelText;

	public SpriteText SectionName;

	public void OnEnable()
	{
		ObjectiveManager objectiveManager = Object.FindObjectOfType(typeof(ObjectiveManager)) as ObjectiveManager;
		if (objectiveManager != null)
		{
			int i = 0;
			int num = objectiveManager.ObjectivesInOrder.Count - 1;
			while (num >= 0 && i < PanelText.Length)
			{
				MissionObjective missionObjective = objectiveManager.ObjectivesInOrder[num];
				if (missionObjective != null && missionObjective.m_Interface.IsVisible && missionObjective.State != MissionObjective.ObjectiveState.Dormant)
				{
					PanelText[i].Store(Language.Get(missionObjective.m_Interface.ObjectiveLabel), string.Empty, missionObjective.State, true);
					PanelText[i].ShowNow();
					i++;
				}
				num--;
			}
			for (; i < PanelText.Length; i++)
			{
				PanelText[i].Clear();
			}
		}
		if (!(SectionName != null))
		{
			return;
		}
		MissionListings instance = MissionListings.Instance;
		ActStructure instance2 = ActStructure.Instance;
		MissionListings.eMissionID id = ((instance2.CurrentMissionID == MissionListings.eMissionID.MI_MAX) ? instance2.LastMissionID : instance2.CurrentMissionID);
		MissionData missionData = instance.Mission(id);
		int currentMissionSection = instance2.CurrentMissionSection;
		if (missionData != null && currentMissionSection >= 0 && currentMissionSection < missionData.Sections.Count)
		{
			SectionData sectionData = missionData.Sections[currentMissionSection];
			if (sectionData != null)
			{
				SectionName.Text = string.Format("{0}. {1}", currentMissionSection + 1, AutoLocalize.Get(sectionData.Name));
			}
		}
	}
}
