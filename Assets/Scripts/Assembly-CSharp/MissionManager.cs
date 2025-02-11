using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	public SelectableMissionLabel MissionLabel;

	public SelectableMissionBlip MissionBlip;

	private SelectableMissionMarker[] m_FlashpointMarkers = new SelectableMissionMarker[8];

	private static MissionManager smInstance;

	public static MissionManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public void Awake()
	{
		if (smInstance != null)
		{
			Debug.LogWarning("Can not have multiple MissionManager, destroying the new one");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			smInstance = this;
		}
	}

	private void Start()
	{
		HudBlipIcon.ClearCutsceneFlag();
		StartCoroutine(SpawnMissionMarkers());
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private IEnumerator SpawnMissionMarkers()
	{
		yield return new WaitForSeconds(2f);
		CreditsController creditsController = UnityEngine.Object.FindObjectOfType(typeof(CreditsController)) as CreditsController;
		while (creditsController != null && creditsController.SequenceIsRunning)
		{
			yield return new WaitForEndOfFrame();
		}
		MissionData[] activeMissions = ActStructure.Instance.AvailableMissions;
		SelectableMissionMarker[] markers = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (markers == null)
		{
			yield break;
		}
		MissionData[] array = activeMissions;
		foreach (MissionData mission in array)
		{
			if (mission == null)
			{
				continue;
			}
			bool markerFoundForMission = false;
			bool ignoreMission = false;
			if (new List<MissionListings.eMissionID>
			{
				MissionListings.eMissionID.MI_MISSION_MOROCCO_GMG,
				MissionListings.eMissionID.MI_MISSION_CARRIER_GMG,
				MissionListings.eMissionID.MI_MISSION_AFGHANISTAN_FL,
				MissionListings.eMissionID.MI_MISSION_ARCTIC_FL,
				MissionListings.eMissionID.MI_MISSION_CARRIER_FL,
				MissionListings.eMissionID.MI_MISSION_KOWLOON_FL,
				MissionListings.eMissionID.MI_MISSION_MOROCCO_FL,
				MissionListings.eMissionID.MI_MISSION_KINVITE,
				MissionListings.eMissionID.MI_MISSION_EVERYPLAY
			}.Contains(mission.MissionId))
			{
				ignoreMission = true;
			}
			if (!ignoreMission)
			{
				List<GMGData.GameType> missionTypesToBeRemoved = new List<GMGData.GameType>
				{
					GMGData.GameType.TimeAttack,
					GMGData.GameType.Domination
				};
				for (int i = 0; i < mission.Sections.Count; i++)
				{
					if (missionTypesToBeRemoved.Contains(mission.Sections[i].GMGGameType))
					{
						mission.Sections.RemoveAt(i);
						i--;
					}
				}
				SelectableMissionMarker[] array2 = markers;
				foreach (SelectableMissionMarker ma in array2)
				{
					if (ma != null && string.Compare(ma.gameObject.name, mission.Location, StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						ma.GiveMission(mission);
						markerFoundForMission = true;
						int flashpointIndex = mission.FlashPointIndex;
						if (flashpointIndex >= 0)
						{
							m_FlashpointMarkers[flashpointIndex] = ma;
						}
						break;
					}
				}
			}
			if (!markerFoundForMission)
			{
				Debug.LogWarning("no marker found for mission " + mission.Location);
			}
		}
		SelectableMissionMarker[] array3 = markers;
		foreach (SelectableMissionMarker ma2 in array3)
		{
			if (ma2 != null && ma2.Data == null)
			{
				UnityEngine.Object.Destroy(ma2.gameObject);
			}
		}
	}

	public void SetFlashpointMissionActive(int index, bool active)
	{
		if (m_FlashpointMarkers[index] != null)
		{
			TBFAssert.DoAssert(m_FlashpointMarkers[index].FlashpointMissionIndex() == index);
			m_FlashpointMarkers[index].SetBlipActive(active);
		}
	}

	public void SetFlashpointMissionActive(MissionListings.eMissionID id, bool active)
	{
		for (int i = 0; i < m_FlashpointMarkers.GetLength(0); i++)
		{
			if (m_FlashpointMarkers[i].Data.MissionId == id)
			{
				m_FlashpointMarkers[i].SetBlipActive(active);
				break;
			}
		}
	}

	private void Update()
	{
	}
}
