using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicTriggerData
{
	public enum MusicToTriggerType
	{
		TitleMusic = 0,
		StrategyMapMusic = 1,
		AmbientRainMusic = 2,
		AmbientArcticMusic = 3,
		AmbientMiddleEastMusic = 4,
		LoadOutMusic = 5,
		MissionFailMusic = 6,
		MissionPassMusic = 7,
		TensionThemeMusic = 8,
		CombatThemeMusic = 9,
		HighDramaThemeMusic = 10,
		MortallyWoundedThemeMusic = 11,
		AfgIntro = 12,
		AfgOutro = 13,
		ArcIntro = 14,
		ArcOutro = 15,
		KowIntro = 16,
		KowOutro = 17,
		Silence = 18,
		CombatTheme1 = 19,
		CombatTheme2 = 20,
		HighDrama1 = 21,
		HighDrama2 = 22
	}

	public enum PriorityLevel
	{
		Level1 = 0,
		Level2 = 1,
		Level3 = 2,
		Level4 = 3,
		Level5 = 4,
		Level6 = 5,
		Level7 = 6,
		Level8 = 7,
		Level9 = 8,
		Level10 = 9
	}

	public string CurrentlyPlaying;

	public MusicToTriggerType TrackName;

	public PriorityLevel Priority;

	public float Volume;

	public float FadeInTime;

	public float FadeOutTime;

	public bool StartEnabled = true;

	public bool OnlyTriggerOnce;

	public BehaviourController.AlertState WhenEnemiesAre = BehaviourController.AlertState.Unused;

	public BehaviourController.AlertState Or = BehaviourController.AlertState.Unused;

	public BehaviourController.AlertState _Or = BehaviourController.AlertState.Unused;

	public List<GameObject> GroupObjectToCallOnAllDead = new List<GameObject>();

	public List<string> GroupFunctionToCallOnAllDead = new List<string>();

	private string mTrackNameAsString;

	public string GetTrackNameAsString()
	{
		if (string.IsNullOrEmpty(mTrackNameAsString))
		{
			mTrackNameAsString = TrackName.ToString();
		}
		return mTrackNameAsString;
	}
}
