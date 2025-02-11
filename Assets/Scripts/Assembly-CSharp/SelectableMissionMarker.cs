using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SelectableMissionMarker : MonoBehaviour
{
	public float Pitch;

	public float Yaw;

	public static float Radius = 69f;

	public float FrameCamPitchOffs;

	public float FrameCamYawOffs;

	public float FrameCamRadiusOffs = 150f;

	private Vector3 FrameCamPosition;

	public GameObject Marker3d;

	private SelectableMissionLabel Label;

	private SelectableMissionBlip Blip;

	private MissionData mMissionData;

	public MissionData Data
	{
		get
		{
			return mMissionData;
		}
	}

	public bool CanBeSeen
	{
		get
		{
			if (GlobeSelect.Instance == null)
			{
				return false;
			}
			Vector3 normalized = GlobeSelect.Instance.GlobeCamera.transform.position.normalized;
			Vector3 normalized2 = base.transform.position.normalized;
			float num = Vector3.Dot(normalized, normalized2);
			return num > 0f;
		}
	}

	private void Awake()
	{
		base.gameObject.GetComponent<BoxCollider>().enabled = false;
	}

	private void Start()
	{
	}

	public int FlashpointMissionIndex()
	{
		if (Data == null)
		{
			return -1;
		}
		return Data.FlashPointIndex;
	}

	private void OnEnable()
	{
		GlobeSelectNavigator.BackToMissionSelect += OnReturnToMissionSelect;
		if (mMissionData != null && mMissionData.Type == MissionData.eType.MT_KINVITE)
		{
			SwrveServerVariables.SwrveBlipsEnabledChanged += RefreshKInviteButton;
		}
	}

	private void OnDisable()
	{
		GlobeSelectNavigator.BackToMissionSelect -= OnReturnToMissionSelect;
		if (mMissionData != null && mMissionData.Type == MissionData.eType.MT_KINVITE)
		{
			SwrveServerVariables.SwrveBlipsEnabledChanged -= RefreshKInviteButton;
		}
	}

	public void SetBlipHighlighted(bool highlighted)
	{
		if (Blip != null)
		{
			Blip.Highlight = highlighted;
		}
	}

	public bool IsBlipHighlighted()
	{
		return Blip != null && Blip.Highlight;
	}

	public void SetBlipActive(bool active)
	{
		if (Blip != null)
		{
			if (Blip.Active != active)
			{
				base.gameObject.GetComponent<BoxCollider>().enabled = active;
			}
			Blip.SetActive(active);
		}
	}

	public bool IsBlipActive()
	{
		if (Blip != null)
		{
			return Blip.Active;
		}
		return false;
	}

	public void GiveMission(MissionData mission)
	{
		mMissionData = mission;
		if (mMissionData.Type != 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(MissionManager.Instance.MissionBlip.gameObject) as GameObject;
			Blip = gameObject.GetComponent<SelectableMissionBlip>();
			Blip.Target = base.transform;
			Blip.transform.parent = base.transform;
			if (Blip.Label != null)
			{
				Blip.Label.Text = string.Empty;
				GameSettings instance = GameSettings.Instance;
				if (instance != null && StatsHelper.MissionsPlayed() == 0 && instance.HighlightMissionID == mMissionData.MissionId && instance.HighlightSectionIndex < mMissionData.Sections.Count && mMissionData.Sections[instance.HighlightSectionIndex] != null && mMissionData.Sections[instance.HighlightSectionIndex].IsTutorial)
				{
					Blip.Label.Text = Language.Get("S_GLOBE_TUTORIAL_HIGHLIGHT");
				}
			}
			RefreshAfterUnlock();
		}
		base.gameObject.GetComponent<BoxCollider>().enabled = true;
	}

	private void RefreshKInviteButton(object sender, EventArgs e)
	{
		if (Blip != null && Blip.Icon != null)
		{
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableKInvite) && SwrveServerVariables.Instance.KInviteEnabled)
			{
				Blip.Icon.SetFrame(0, 2);
				SetBlipActive(true);
			}
			else
			{
				SetBlipActive(false);
			}
		}
	}

	private void RefreshAfterUnlock()
	{
		if (mMissionData == null || Blip == null || Blip.Icon == null)
		{
			return;
		}
		switch (mMissionData.Type)
		{
		case MissionData.eType.MT_STORY:
			Blip.Icon.SetColor((!mMissionData.IsLocked()) ? ColourChart.MissionStory : ColourChart.MissionLocked);
			SetBlipActive(true);
			break;
		case MissionData.eType.MT_SURVIVAL:
			if (mMissionData.NumValidSections() > 0)
			{
				Blip.Icon.SetColor((!mMissionData.IsLocked()) ? ColourChart.MissionSurvival : ColourChart.MissionLocked);
				Blip.Icon.SetFrame(0, 1);
				SetBlipActive(true);
			}
			break;
		case MissionData.eType.MT_DEMO:
			Blip.Icon.SetFrame(0, 0);
			SetBlipActive(true);
			break;
		case MissionData.eType.MT_KINVITE:
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableKInvite) && SwrveServerVariables.Instance.KInviteEnabled)
			{
				Blip.Icon.SetFrame(0, 2);
				SetBlipActive(true);
			}
			break;
		case MissionData.eType.MT_FLASHPOINT:
			break;
		case MissionData.eType.MT_EVERYPLAY_Removed:
			break;
		}
	}

	public static void RefreshAllAfterUnlock()
	{
		SelectableMissionMarker[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (array != null)
		{
			SelectableMissionMarker[] array2 = array;
			foreach (SelectableMissionMarker selectableMissionMarker in array2)
			{
				selectableMissionMarker.RefreshAfterUnlock();
			}
		}
	}

	public void LoadMission(DifficultyMode mode, int section)
	{
		if (!mMissionData.IsValid || mMissionData.Type == MissionData.eType.MT_EMPTY)
		{
			return;
		}
		ActStructure.Instance.MissionStart(mMissionData.MissionId, mode, section);
		if (mMissionData.Sections.Count <= section)
		{
			return;
		}
		if (section == 0)
		{
			if (mMissionData.MissionId == MissionListings.eMissionID.MI_MISSION_ARCTIC)
			{
				Application.LoadLevel("IntroMovie");
			}
			else
			{
				SceneLoader.AsyncLoadSceneWithLoadingScreen(mMissionData.Sections[section].SceneName);
			}
		}
		else
		{
			SectionLoader.AsyncLoadSceneWithLoadingScreen(mMissionData.Sections[section].SceneName);
		}
	}

	private void OnReturnToMissionSelect(object owner, EventArgs e)
	{
		StopAllCoroutines();
	}

	private void Update()
	{
		float num = 1f;
		if (GlobeSelect.Instance != null && GlobeSelect.Instance.GlobeCamera != null)
		{
			GlobeCamera component = GlobeSelect.Instance.GlobeCamera.GetComponent<GlobeCamera>();
			if (component != null && component.IsZooming())
			{
				num = component.Distance / component.maxDistance;
			}
		}
		num = ((!CanBeSeen) ? 0f : num);
		Marker3d.transform.localScale = new Vector3(num, num, num);
		BoxCollider component2 = base.transform.GetComponent<BoxCollider>();
		if (component2 != null)
		{
			num *= 12f;
			component2.size = new Vector3(num, num, num);
		}
		Color color = Marker3d.renderer.material.GetColor("_Color");
		color.a = GlobeSelect.GetPositionAlphaPos(base.transform.position);
		Marker3d.renderer.material.SetColor("_Color", color);
		if (Data == null || Data.FlashPointIndex < 0 || !(Blip != null))
		{
			return;
		}
		int timeRemainingInSeconds = GlobalUnrestController.Instance.GetTimeRemainingInSeconds(Data.FlashPointIndex);
		if (timeRemainingInSeconds > 0)
		{
			Blip.SetTime(timeRemainingInSeconds);
			if (timeRemainingInSeconds > 3600)
			{
				Blip.SetLabelColour(ColourChart.HudGreen);
			}
			else
			{
				Blip.SetLabelColour(ColourChart.HudRed);
			}
		}
	}

	public void SetPositionFromAngles(float pitch, float yaw)
	{
		Pitch = pitch;
		Yaw = yaw;
		Pitch = Mathf.Clamp(Pitch, -85f, 85f);
		if (Yaw > 180f)
		{
			Yaw -= 360f;
		}
		if (Yaw < -180f)
		{
			Yaw += 360f;
		}
		Quaternion quaternion = Quaternion.AngleAxis(0f - Pitch, Vector3.right);
		Quaternion quaternion2 = Quaternion.AngleAxis(Yaw, Vector3.up);
		Vector3 vector = Vector3.forward * Radius;
		vector = quaternion * vector;
		vector = quaternion2 * vector;
		base.transform.position = vector;
	}

	private void SetCamPositionFromAngles(float pitchOffs, float yawOffs, float distOffs)
	{
		FrameCamPitchOffs = pitchOffs;
		FrameCamYawOffs = yawOffs;
		FrameCamRadiusOffs = distOffs;
		if (FrameCamYawOffs > 180f)
		{
			FrameCamYawOffs -= 360f;
		}
		if (FrameCamYawOffs < -180f)
		{
			FrameCamYawOffs += 360f;
		}
		Quaternion quaternion = Quaternion.AngleAxis(0f - (FrameCamPitchOffs + Pitch), Vector3.right);
		Quaternion quaternion2 = Quaternion.AngleAxis(FrameCamYawOffs + Yaw, Vector3.up);
		Vector3 vector = Vector3.forward * (Radius + FrameCamRadiusOffs);
		vector = quaternion * vector;
		vector = quaternion2 * vector;
		FrameCamPosition = vector;
	}

	private void OnDrawGizmos()
	{
	}

	private void OnDrawGizmosSelected()
	{
		OnDrawGizmos();
		Gizmos.DrawIcon(FrameCamPosition, "camera");
		Gizmos.color = Color.white;
		Gizmos.DrawLine(FrameCamPosition, base.transform.position);
	}

	public static void HideAllMissionMarkers(bool hide)
	{
		SelectableMissionMarker[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != null) || array[i].Data == null)
			{
				continue;
			}
			SelectableMissionBlip blip = array[i].Blip;
			if (blip != null)
			{
				if (hide)
				{
					blip.SwitchOff();
				}
				else
				{
					blip.SwitchOn();
				}
			}
		}
	}

	public static void StopAllMissionBlipSounds()
	{
		SelectableMissionMarker[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].Data != null)
			{
				SelectableMissionBlip blip = array[i].Blip;
				if (blip != null)
				{
					blip.StopBlipSFXLoop();
				}
			}
		}
	}
}
