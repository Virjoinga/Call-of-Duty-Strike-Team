using UnityEngine;

public class OverviewAndObjectivesController : MenuScreenBlade
{
	private enum Sequence
	{
		None = 0,
		Medals = 1,
		Intel = 2,
		Objectives = 3,
		NumSequenceTypes = 4
	}

	public ObjectiveSpriteText[] PanelText;

	public ObjectiveSpriteText[] FlashpointPanelText;

	public MedalIconController[] MedalsIcons;

	public PackedSprite Intel;

	public SpriteText MedalsTitleText;

	public SpriteText IntelTitleText;

	public SpriteText IntelText;

	public SpriteText LevelText;

	public SpriteText SpecOpsText;

	public SpriteText SectionName;

	public CountUpText TotalText;

	public ProgressBar LevelProgress;

	public RankIconController Rank;

	public GameObject ObjectivesRoot;

	public GameObject FlashpointRoot;

	private ObjectiveSpriteText[] mPanelTextInUse;

	private bool[] mMedalsWon;

	private MissionData mMissionData;

	private SectionData mSectionData;

	private float mTimeForEachSequenceType;

	private float mTimeForEachItem;

	private float mTimeInSequence;

	private Sequence mSequence;

	private int mSectionIndex;

	private int mCurrentIndex;

	private int mTotalIntel;

	private int mNumIntelFound;

	private bool mIsSpecOps;

	private bool mVeteran;

	private bool mIsFlashpoint;

	public override void Awake()
	{
		base.Awake();
		mSequence = Sequence.None;
		mIsSpecOps = ActStructure.Instance.CurrentMissionIsSpecOps();
		mVeteran = ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran;
		mIsFlashpoint = mIsSpecOps && ActStructure.Instance.CurrentMissionType() == GMGData.GameType.Flashpoint;
		MissionListings instance = MissionListings.Instance;
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		mMissionData = instance.Mission(currentMissionID);
		mSectionIndex = ActStructure.Instance.CurrentSection;
		if (mSectionIndex != -1)
		{
			mSectionData = mMissionData.Sections[mSectionIndex];
		}
		SetupMedals();
		SetupIntel();
		SetupPanelText();
		SetupRankLevelAndProgress();
	}

	private void SetupMedals()
	{
		if (mSectionData == null)
		{
			return;
		}
		mMedalsWon = new bool[mSectionData.Medals.Length];
		for (int i = 0; i < mSectionData.Medals.Length; i++)
		{
			if (MedalsIcons[i] != null && mSectionData.Medals[i] != null)
			{
				if (!mIsFlashpoint)
				{
					MedalsIcons[i].SetMedal(mSectionData.Medals[i].Icon, mVeteran, false);
				}
				else
				{
					MedalsIcons[i].gameObject.SetActive(false);
				}
			}
		}
	}

	private void SetupIntel()
	{
		mTotalIntel = ((mMissionData != null && mSectionIndex < mMissionData.Sections.Count) ? mMissionData.Sections[mSectionIndex].IntelToCollect : 0);
		if (Intel != null && IntelTitleText != null && IntelText != null)
		{
			Intel.gameObject.SetActive(!mIsFlashpoint && !mIsSpecOps && mTotalIntel > 0);
			IntelTitleText.gameObject.SetActive(!mIsFlashpoint && !mIsSpecOps && mTotalIntel > 0);
			IntelText.Text = string.Empty;
		}
	}

	private void SetupPanelText()
	{
		if (SectionName != null && mSectionData != null)
		{
			if (mIsFlashpoint)
			{
				SectionName.Text = string.Format("{0} {1}", Language.Get("S_FL_RESULTS_TITLE_02"), Language.Get(mMissionData.NameKey).ToUpper());
			}
			else
			{
				SectionName.Text = string.Format("{0}. {1}", mSectionIndex + 1, Language.Get(mSectionData.Name));
			}
		}
		if (mIsFlashpoint)
		{
			mPanelTextInUse = FlashpointPanelText;
			if (ObjectivesRoot != null)
			{
				ObjectivesRoot.SetActive(false);
			}
			if (SpecOpsText != null)
			{
				SpecOpsText.Text = Language.Get("S_FL_RESULTS_TOTAL_SCORE");
			}
			if (MedalsTitleText != null)
			{
				MedalsTitleText.gameObject.SetActive(false);
			}
		}
		else
		{
			mPanelTextInUse = PanelText;
			if (FlashpointRoot != null)
			{
				FlashpointRoot.SetActive(false);
			}
		}
		for (int i = 0; i < mPanelTextInUse.Length; i++)
		{
			mPanelTextInUse[i].Clear();
		}
		if (TotalText != null)
		{
			if (mIsSpecOps)
			{
				TotalText.Postfix = string.Empty;
				return;
			}
			string postfix = Language.Get("S_RESULT_XP");
			TotalText.Postfix = postfix;
		}
	}

	private void SetupRankLevelAndProgress()
	{
		int xp = StatsHelper.PlayerXP();
		XPManager instance = XPManager.Instance;
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		instance.ConvertXPToLevel(xp, out level, out prestigeLevel, out xpToNextLevel, out percent);
		int rank = level + (instance.m_XPLevels.Count - 1) * prestigeLevel;
		if (Rank != null)
		{
			Rank.gameObject.SetActive(!mIsSpecOps && !mIsFlashpoint);
			Rank.SetRank(rank);
		}
		if (LevelText != null)
		{
			LevelText.Text = ((!mIsSpecOps) ? string.Format("{0} {1}", Language.Get("S_LEVEL"), level) : string.Empty);
		}
		if (LevelProgress != null)
		{
			LevelProgress.gameObject.SetActive(!mIsSpecOps && !mIsFlashpoint);
			LevelProgress.SoundActive = false;
			LevelProgress.SetValueNow(percent);
		}
	}

	protected override void OnActivate()
	{
		if (TotalText != null)
		{
			TotalText.Reset();
		}
		base.OnActivate();
	}

	public override void Update()
	{
		base.Update();
		mTimeInSequence += TimeManager.DeltaTime;
		if (mTimeInSequence > mTimeForEachItem)
		{
			mTimeInSequence = 0f;
			switch (mSequence)
			{
			case Sequence.Medals:
				UpdateMedalSequence();
				break;
			case Sequence.Intel:
				UpdateIntelSequence();
				break;
			case Sequence.Objectives:
				UpdateObjectiveSequence();
				break;
			}
		}
	}

	public void BeginSequence(float totalTimeToTake)
	{
		mTimeForEachSequenceType = totalTimeToTake / 4f;
		NextSequence();
	}

	private void NextSequence()
	{
		if (mSequence == Sequence.None && mIsFlashpoint)
		{
			BeginObjectivesSequence(mTimeForEachSequenceType);
			return;
		}
		mSequence++;
		if (mSequence < Sequence.NumSequenceTypes)
		{
			switch (mSequence)
			{
			case Sequence.Medals:
				BeginMedalSequence(mTimeForEachSequenceType);
				break;
			case Sequence.Intel:
				BeginIntelSequence(mTimeForEachSequenceType);
				break;
			case Sequence.Objectives:
				BeginObjectivesSequence(mTimeForEachSequenceType);
				break;
			}
		}
	}

	private void UpdateMedalSequence()
	{
		if (mCurrentIndex < MedalsIcons.Length)
		{
			if (mMedalsWon != null && mMedalsWon[mCurrentIndex] && MedalsIcons[mCurrentIndex] != null && mSectionData.Medals[mCurrentIndex] != null)
			{
				MedalsIcons[mCurrentIndex].SetMedal(mSectionData.Medals[mCurrentIndex].Icon, mVeteran, true);
				MedalsIcons[mCurrentIndex].gameObject.ScaleFrom(new Vector3(1.5f, 1.5f, 1.5f), mTimeForEachItem, 0f);
			}
			mCurrentIndex++;
		}
		else
		{
			NextSequence();
		}
	}

	private void UpdateIntelSequence()
	{
		if (!(Intel != null))
		{
			return;
		}
		if (mCurrentIndex < mNumIntelFound && !mIsSpecOps)
		{
			mCurrentIndex++;
			Intel.gameObject.ScaleFrom(new Vector3(1.5f, 1.5f, 1.5f), mTimeForEachItem * 0.8f, 0f);
			if (IntelText != null)
			{
				IntelText.Text = string.Format("{0}/{1}", mCurrentIndex, mTotalIntel);
			}
		}
		else
		{
			NextSequence();
		}
	}

	private void UpdateObjectiveSequence()
	{
		if (mIsSpecOps)
		{
			UpdateObjectivesSequenceForSpecOps();
		}
		else
		{
			UpdateObjectivesSequenceForSinglePlayer();
		}
	}

	private void UpdateObjectivesSequenceForSpecOps()
	{
		if (mCurrentIndex < mPanelTextInUse.Length)
		{
			mPanelTextInUse[mCurrentIndex].ShowNow();
			mCurrentIndex++;
		}
	}

	private void UpdateObjectivesSequenceForSinglePlayer()
	{
		if (mCurrentIndex < mPanelTextInUse.Length)
		{
			mPanelTextInUse[mCurrentIndex].ShowNow();
			mCurrentIndex++;
		}
	}

	private void BeginMedalSequence(float timeToTake)
	{
		mTimeForEachItem = timeToTake / (float)(MedalsIcons.Length + 1);
		if (mMedalsWon != null)
		{
			for (int i = 0; i < mMedalsWon.Length; i++)
			{
				mMedalsWon[i] = StatsHelper.HasMedalBeenEarnedGameTotal(mMissionData.MissionId, mSectionIndex, i, ActStructure.Instance.CurrentMissionMode);
			}
		}
		mCurrentIndex = 0;
		mSequence = Sequence.Medals;
		UpdateGameProgress();
	}

	private void BeginIntelSequence(float timeToTake)
	{
		if (!mIsSpecOps && mTotalIntel > 0)
		{
			mNumIntelFound = StatsHelper.IntelCollectedForCurrentMission();
			mTimeForEachItem = timeToTake / (float)(mNumIntelFound + 1);
			if (IntelText != null)
			{
				IntelText.Text = string.Format("0/{0}", mTotalIntel);
			}
		}
		mCurrentIndex = 0;
		mSequence = Sequence.Intel;
	}

	private void BeginObjectivesSequence(float timeToTake)
	{
		if (mIsFlashpoint)
		{
			SetupForFlashpoint(timeToTake);
		}
		else if (mIsSpecOps)
		{
			SetupForSpecOps(timeToTake);
		}
		else
		{
			SetupTextForSinglePlayer(timeToTake);
		}
		mCurrentIndex = 0;
		mSequence = Sequence.Objectives;
	}

	public void UpdateTheTotalXP()
	{
		if (TotalText != null)
		{
			TotalText.CountTo(StatsHelper.CurrentMissionScore(), 0.5f);
		}
		StatsManager.Instance.SquadStats().BuildFromCharacters();
		if (!mIsSpecOps)
		{
			SetupTextForSinglePlayer(mTimeForEachSequenceType);
			for (int i = 0; i < mPanelTextInUse.Length; i++)
			{
				mPanelTextInUse[i].ShowNow();
			}
		}
		UpdateGameProgress();
	}

	public void OnTap(Vector2 fingerPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) || !hitInfo.collider.gameObject)
		{
			return;
		}
		for (int i = 0; i < MedalsIcons.Length; i++)
		{
			if (hitInfo.collider.gameObject == MedalsIcons[i].gameObject)
			{
				DoMedalInformation(i, MedalsIcons[i].gameObject);
				return;
			}
		}
		if (Intel != null && hitInfo.collider.gameObject == Intel.gameObject)
		{
			DoIntelInformation(Intel.gameObject);
		}
	}

	private void DoMedalInformation(int index, GameObject over)
	{
		string medalString = MedalManager.GetMedalString(mSectionData, index, mVeteran ? DifficultyMode.Veteran : DifficultyMode.Regular);
		ToolTipController.Instance.DoTooltip(medalString, over);
	}

	private void DoIntelInformation(GameObject over)
	{
		string text = Language.Get("S_INTEL_INFOMATION");
		ToolTipController.Instance.DoTooltip(text, over);
	}

	private void SetupTextForSinglePlayer(float timeToTake)
	{
		if (mPanelTextInUse != null && mPanelTextInUse.Length >= 6)
		{
			StatsManager instance = StatsManager.Instance;
			CharacterXP currentMissionCombinedStat = instance.CharacterXPStats().GetCurrentMissionCombinedStat();
			PlayerStat currentMissionStat = instance.PlayerStats().GetCurrentMissionStat();
			MissionStat currentMissionStat2 = instance.MissionStats().GetCurrentMissionStat(mMissionData.MissionId, mSectionIndex);
			instance.SquadStats().BuildFromCharacters();
			CharacterStat currentMission = instance.SquadStats().GetCurrentMission();
			int num = 0;
			mPanelTextInUse[0].Store(Language.Get("S_HEAD_SHOTS_FPS"), currentMission.NumHeadShotsInFP.ToString(), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[1].Store(Language.Get("S_ACCURACY_FPS"), StatsHelper.AccuracyToString(currentMission.AccuracyInFP), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[2].Store(Language.Get("S_MISSION_TIME"), StatsHelper.MinutesToString(currentMissionStat2.TimePlayed), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[3].Store(Language.Get("S_XP_MISSION"), currentMissionStat.XPFromMissionComplete.ToString(), MissionObjective.ObjectiveState.Passed, false);
			num += currentMissionStat.XPFromMissionComplete;
			mPanelTextInUse[4].Store(Language.Get("S_SQUAD_XP"), (currentMissionCombinedStat.XPFromBonuses + currentMissionCombinedStat.XPFromKills).ToString(), MissionObjective.ObjectiveState.Passed, false);
			num += currentMissionCombinedStat.XPFromBonuses + currentMissionCombinedStat.XPFromKills;
			mPanelTextInUse[5].Store(Language.Get("S_XP_OTHER"), (currentMissionStat.XPBeforeMultipler - num).ToString(), MissionObjective.ObjectiveState.Passed, false);
			mTimeForEachItem = timeToTake / 6f;
		}
		if (TotalText != null)
		{
			TotalText.CountTo(StatsHelper.CurrentMissionXP(), timeToTake);
		}
	}

	private void SetupForSpecOps(float timeToTake)
	{
		if (mPanelTextInUse != null && mPanelTextInUse.Length >= 6)
		{
			StatsManager instance = StatsManager.Instance;
			MissionStat currentMissionStat = instance.MissionStats().GetCurrentMissionStat(mMissionData.MissionId, mSectionIndex);
			CharacterStat currentMissionCombinedStat = instance.CharacterStats().GetCurrentMissionCombinedStat();
			char c = CommonHelper.HardCurrencySymbol();
			string val = string.Format("{0}{1}", c, StatsHelper.CurrentMissionHardCurrencyEarned());
			mPanelTextInUse[0].Store(Language.Get("S_KILLS"), currentMissionCombinedStat.NumKills.ToString(), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[1].Store(Language.Get("S_HEAD_SHOTS_FPS"), currentMissionCombinedStat.NumHeadShots.ToString(), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[2].Store(Language.Get("S_ACCURACY_FPS"), StatsHelper.AccuracyToString(currentMissionCombinedStat.AccuracyInFP), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[3].Store(Language.Get("S_GMG_TOKENS_EARNED"), val, MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[4].Store(Language.Get("S_GMG_TIME"), StatsHelper.MinutesToString(currentMissionStat.TimePlayed), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[5].Clear();
			mTimeForEachItem = timeToTake / 6f;
		}
		if (TotalText != null)
		{
			TotalText.CountTo(StatsHelper.CurrentMissionScore(), timeToTake);
		}
	}

	private void SetupForFlashpoint(float timeToTake)
	{
		if (mPanelTextInUse != null && mPanelTextInUse.Length >= 6)
		{
			StatsManager instance = StatsManager.Instance;
			MissionStat currentMissionStat = instance.MissionStats().GetCurrentMissionStat(mMissionData.MissionId, mSectionIndex);
			CharacterStat currentMissionCombinedStat = instance.CharacterStats().GetCurrentMissionCombinedStat();
			char c = CommonHelper.HardCurrencySymbol();
			string val = string.Format("{0}{1}", c, StatsHelper.CurrentMissionHardCurrencyEarned());
			mPanelTextInUse[0].Store(Language.Get("S_KILLS"), currentMissionCombinedStat.NumKills.ToString(), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[1].Store(Language.Get("S_HEAD_SHOTS_FPS"), currentMissionCombinedStat.NumHeadShots.ToString(), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[2].Store(Language.Get("S_ACCURACY_FPS"), StatsHelper.AccuracyToString(currentMissionCombinedStat.AccuracyInFP), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[3].Store(Language.Get("S_FL_RESULTS_TIME"), StatsHelper.MinutesToString(currentMissionStat.TimePlayed), MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[4].Store(Language.Get("S_FL_RESULTS_REWARD"), val, MissionObjective.ObjectiveState.Passed, false);
			mPanelTextInUse[5].Store(Language.Get("S_FL_RESULTS_SCORE"), StatsHelper.MinutesToString(currentMissionStat.TimePlayed), MissionObjective.ObjectiveState.Passed, false);
			mTimeForEachItem = timeToTake / 6f;
		}
		if (TotalText != null)
		{
			TotalText.CountTo(StatsHelper.CurrentMissionScore(), timeToTake);
		}
	}

	private void UpdateGameProgress()
	{
		int xp = StatsHelper.PlayerXP();
		XPManager instance = XPManager.Instance;
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		instance.ConvertXPToLevel(xp, out level, out prestigeLevel, out xpToNextLevel, out percent);
		int rank = level + (instance.m_XPLevels.Count - 1) * prestigeLevel;
		if (Rank != null)
		{
			Rank.gameObject.SetActive(!mIsSpecOps && !mIsFlashpoint);
			Rank.SetRank(rank);
		}
		if (LevelText != null)
		{
			LevelText.Text = ((!mIsSpecOps) ? string.Format("{0} {1}", Language.Get("S_LEVEL"), level) : string.Empty);
		}
		if (LevelProgress != null)
		{
			LevelProgress.SoundActive = true;
			LevelProgress.gameObject.SetActive(!mIsSpecOps && !mIsFlashpoint);
			LevelProgress.SetValue(percent);
		}
		if (SpecOpsText != null)
		{
			SpecOpsText.gameObject.SetActive(mIsSpecOps || mIsFlashpoint);
		}
	}

	private void UpdateObjectivePanelText(MissionObjective objective, int panelIndex)
	{
		if (panelIndex >= 0 && panelIndex < mPanelTextInUse.Length && objective != null && mPanelTextInUse[panelIndex] != null)
		{
			MissionObjective.ObjectiveState objectiveState = ((objective.State != 0) ? objective.State : MissionObjective.ObjectiveState.Failed);
			bool colour = objectiveState == MissionObjective.ObjectiveState.Failed;
			mPanelTextInUse[panelIndex].Store(Language.Get(objective.m_Interface.ObjectiveLabel), string.Empty, objectiveState, colour);
		}
	}
}
