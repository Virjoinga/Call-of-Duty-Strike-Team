using System;
using UnityEngine;

public class MissionSetup : MonoBehaviour
{
	private static MissionSetup smInstance;

	public bool StartInVeteranMode;

	public DifficultyModifier[] RegularDifficultyModifiers;

	public DifficultyModifier[] VeteranDifficultyModifiers;

	public string Theme;

	public string BGMusicTrack;

	public AudioFilter.AudioFilterType AudioFilter;

	public string MissionTitle;

	public int MissionSectionNumber = 1;

	public GuidRef[] InitiallyDisabledPlayers;

	public bool LockWeaponSelection;

	public bool PlayMissionBriefing = true;

	public MissionBriefingOverride MissionBriefSequence;

	public float MissionBriefingCameraOrientation;

	public bool DoLoadoutOnStart = true;

	public float SkyBoxRotation;

	public float MinimumVisibility;

	public float HeightFogBottom;

	public float HeightFogTop;

	public bool DefaultCameraIsFirstPerson;

	public WeaponDescriptor[] ScriptedWeapons;

	public float BloomStartStrength = 0.5f;

	public float Bloomification = 2f;

	public float BlurWidthScaler = 1f;

	public float BloomPixelColourCutOff = 0.95f;

	public Texture ColourCorrectionRampTexture;

	public Texture ReflectionMap;

	public float SmallScreenStartFov = 25f;

	public float SmallScreenMinFov = 20f;

	public float SmallScreenMaxFov = 55f;

	public float LargeScreenStartFov = 45f;

	public float LargeScreenMinFov = 20f;

	public float LargeScreenMaxFov = 55f;

	public static MissionSetup Instance
	{
		get
		{
			return smInstance;
		}
	}

	public Texture ReflectionMapOverride { get; set; }

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple MissionSetup scripts");
		}
		smInstance = this;
		if (MissionTitle == string.Empty)
		{
			MissionTitle = "no missions title set";
		}
		else
		{
			MissionTitle = AutoLocalize.Get(MissionTitle);
		}
		if (StartInVeteranMode)
		{
		}
		ResolveGuidLinks();
	}

	public void ResolveGuidLinks()
	{
		if (InitiallyDisabledPlayers != null)
		{
			GuidRef[] initiallyDisabledPlayers = InitiallyDisabledPlayers;
			foreach (GuidRef guidRef in initiallyDisabledPlayers)
			{
				guidRef.ResolveLink();
			}
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	public void Update()
	{
		Update(RenderSettings.fog);
	}

	public void Update(bool enableFog)
	{
		UpdateAmbientLight();
		UpdateFog(enableFog);
		UpdateSkyBox();
		UpdateSkyBoxRotation();
	}

	public void UpdateAmbientLight()
	{
		Shader.SetGlobalColor("_AmbientLight", RenderSettings.ambientLight);
	}

	public void UpdateFog(bool enableFog)
	{
		Color fogColor = RenderSettings.fogColor;
		float w = ((!enableFog) ? 0f : RenderSettings.fogDensity);
		float num = ((!enableFog) ? float.MaxValue : RenderSettings.fogStartDistance);
		float num2 = ((!enableFog) ? float.MaxValue : RenderSettings.fogEndDistance);
		Shader.SetGlobalVector("_FogParams", new Vector4(fogColor.r, fogColor.g, fogColor.b, w));
		Shader.SetGlobalFloat("_FogStart", num);
		Shader.SetGlobalFloat("_FogEnd", num2);
		if (RenderSettings.fog)
		{
			float num3 = 1f / (num2 - num);
			float num4 = RenderSettings.fogEndDistance * num3;
			Shader.SetGlobalVector("_FogRange", new Vector4(0f - num3, num4 - 1f, MinimumVisibility, 0f));
			Shader.SetGlobalVector("_FogHeight", new Vector4(0f - HeightFogBottom, 1f / (HeightFogTop - HeightFogBottom), 0f, 0f));
		}
		else
		{
			Shader.SetGlobalVector("_FogRange", Vector4.zero);
			Shader.SetGlobalVector("_FogHeight", Vector4.zero);
		}
	}

	public void UpdateSkyBox()
	{
		Texture texture = ((!(ReflectionMapOverride != null)) ? ReflectionMap : ReflectionMapOverride);
		if (texture == null && RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Tex"))
		{
			texture = RenderSettings.skybox.GetTexture("_Tex");
		}
		if (texture != null)
		{
			Shader.SetGlobalTexture("_ThemedCube", texture);
		}
	}

	public void UpdateSkyBoxRotation()
	{
		Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, SkyBoxRotation, 0f), Vector3.one);
		Shader.SetGlobalMatrix("_SkyBoxRotation", mat);
	}

	public static void LoadAndCopySectionSFX(MissionListings.eMissionID mission, int section)
	{
		string[] array = new string[3] { "Arc", "Afg", "Kow" };
		if (GameDialogueSFX.Instance.DialogueData.Dialogue.Length != 0)
		{
			return;
		}
		LevelLayer masterLayer = LevelManager.GetMasterLayer();
		if (GameDialogueSFX.Instance.transform.parent == null && masterLayer != null)
		{
			GameDialogueSFX.Instance.transform.parent = masterLayer.transform;
		}
		MissionListings.eMissionID eMissionID = MissionListings.eMissionID.MI_MAX;
		eMissionID = ((mission == MissionListings.eMissionID.MI_MAX) ? GetMissionId() : mission);
		if (eMissionID < MissionListings.eMissionID.MI_MISSION_ARCTIC || eMissionID > MissionListings.eMissionID.MI_MISSION_KOWLOON)
		{
			return;
		}
		string text = array[(int)(eMissionID - 1)];
		int num = 0;
		num = ((section == -1) ? GetSection() : section);
		string path = "SectionVO/VOData_" + text + "_S" + (num + 1).ToString("D2");
		GameObject gameObject = Resources.Load(path) as GameObject;
		if ((bool)gameObject)
		{
			MissionVOData missionVOData = gameObject.GetComponent("MissionVOData") as MissionVOData;
			if ((bool)missionVOData)
			{
				int num2 = missionVOData.DialogueData.Dialogue.Length;
				GameDialogueSFX.Instance.DialogueData.Dialogue = new SoundFXData[num2];
				Debug.Log("GameDialogueSFX.Instance: " + GameDialogueSFX.Instance.name);
				int num3 = 0;
				SoundFXData[] dialogue = missionVOData.DialogueData.Dialogue;
				foreach (SoundFXData soundFXData in dialogue)
				{
					GameDialogueSFX.Instance.DialogueData.Dialogue[num3] = soundFXData;
					num3++;
				}
			}
		}
		if (num != 0)
		{
			return;
		}
		path = "SectionVO/VODataBriefing_" + text;
		gameObject = Resources.Load(path) as GameObject;
		if (!gameObject)
		{
			return;
		}
		MissionVOData missionVOData2 = gameObject.GetComponent("MissionVOData") as MissionVOData;
		if ((bool)missionVOData2)
		{
			int num4 = missionVOData2.DialogueData.Dialogue.Length;
			BriefingSFX.Instance.BriefingData.Narration = new SoundFXData[num4];
			if (BriefingSFX.Instance.transform.parent == null && masterLayer != null)
			{
				BriefingSFX.Instance.transform.parent = masterLayer.transform;
			}
			else if (SplashScreenControl.SplashShown)
			{
				GameObject gameObject2 = new GameObject();
				BriefingSFX.Instance.transform.parent = gameObject2.transform;
			}
			int num5 = 0;
			SoundFXData[] dialogue2 = missionVOData2.DialogueData.Dialogue;
			foreach (SoundFXData soundFXData2 in dialogue2)
			{
				BriefingSFX.Instance.BriefingData.Narration[num5] = soundFXData2;
				num5++;
			}
		}
	}

	private static MissionListings.eMissionID GetMissionId()
	{
		MissionListings.eMissionID eMissionID = MissionListings.eMissionID.MI_MISSION_ARCTIC;
		return ActStructure.Instance.CurrentMissionID;
	}

	private static int GetSection()
	{
		int num = 0;
		return ActStructure.Instance.CurrentSection;
	}

	public MissionListings.eMissionID GetMissionIDFromSetup()
	{
		MissionListings.eMissionID result = MissionListings.eMissionID.MI_MAX;
		string theme = Theme;
		if (theme.Contains("Afghanistan"))
		{
			result = MissionListings.eMissionID.MI_MISSION_AFGHANISTAN;
		}
		else if (theme.Contains("Urban"))
		{
			result = MissionListings.eMissionID.MI_MISSION_KOWLOON;
		}
		else if (theme.Contains("Snow"))
		{
			result = MissionListings.eMissionID.MI_MISSION_ARCTIC;
		}
		return result;
	}

	public int GetMissionSectionFromSetup()
	{
		MissionSectionNumber = Mathf.Clamp(MissionSectionNumber, 1, 7);
		return MissionSectionNumber - 1;
	}
}
