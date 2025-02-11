using UnityEngine;

public class MedalsAndIntelController : MenuScreenBlade
{
	private enum Sequence
	{
		Idle = 0,
		Medals = 1,
		Intel = 2
	}

	public PackedSprite[] Medals;

	public PackedSprite[] Intel;

	public SpriteText[] TextLines;

	public SpriteText IntelTextLine;

	private MissionData mMissionData;

	private SectionData mSectionData;

	private MedalIconController[] mMedalsIcons;

	private string[] mMedalsStrings;

	private float mTimeForEachItem;

	private float mTimeInSequence;

	private Sequence mSequence;

	private int mItems;

	private bool[] mMedalsAchieved;

	public override void Awake()
	{
		ClearTextLines();
		base.Awake();
		MissionListings instance = MissionListings.Instance;
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		mMissionData = instance.Mission(currentMissionID);
		mSectionData = mMissionData.Sections[ActStructure.Instance.CurrentSection];
		mSequence = Sequence.Idle;
	}

	public void BeginMedalSequence(float timeForSequence)
	{
		ClearTextLines();
		int numMedalSlots = SectionData.numMedalSlots;
		mMedalsIcons = new MedalIconController[numMedalSlots];
		mMedalsStrings = new string[numMedalSlots];
		mMedalsAchieved = new bool[numMedalSlots];
		Transform transform = base.transform.FindChild("Medals");
		Transform transform2 = transform.FindChild("Content");
		for (int i = 0; i < numMedalSlots; i++)
		{
			Transform transform3 = transform2.FindChild("Medal" + (i + 1));
			Transform transform4 = transform3.FindChild("Icon");
			mMedalsIcons[i] = transform4.GetComponent<MedalIconController>();
			if (mMedalsIcons[i] != null)
			{
				mMedalsIcons[i].SetMedal(mSectionData.Medals[i].Icon, false, false);
			}
		}
		for (int j = 0; j < numMedalSlots; j++)
		{
			int medalTargetType = (int)mSectionData.Medals[j].MedalTargetType;
			mMedalsStrings[j] = AutoLocalize.Get("S_MISSION_MEDAL_0" + (medalTargetType + 1));
		}
		int num = 0;
		for (int k = 0; k < numMedalSlots; k++)
		{
			DifficultyMode currentMissionMode = ActStructure.Instance.CurrentMissionMode;
			mMedalsAchieved[k] = StatsHelper.HasMedalBeenEarnedCurrentMission(k, currentMissionMode);
			mMedalsStrings[k] = MedalManager.GetMedalString(mSectionData, k, currentMissionMode);
			if (mMedalsAchieved[k])
			{
				num++;
			}
		}
		if (num == 0 && TextLines[0] != null)
		{
			TextLines[0].Text = AutoLocalize.Get("S_NO_MISSION_MEDALS");
		}
		mTimeForEachItem = timeForSequence / (float)(num + 1);
		mItems = num;
		mTimeInSequence = 0f;
		mSequence = Sequence.Medals;
	}

	public void BeginIntelSequence(float timeForSequence)
	{
		ClearTextLines();
		int num = mMissionData.IntelCount();
		int num2 = StatsHelper.IntelCollectedForCurrentMission();
		if (IntelTextLine != null)
		{
			string format = AutoLocalize.Get("S_COLLECTED");
			IntelTextLine.Text = string.Format(format, num2, num);
		}
		mTimeForEachItem = timeForSequence / (float)(num2 + 1);
		mItems = num2;
		mTimeInSequence = 0f;
		mSequence = Sequence.Intel;
	}

	public void Finish()
	{
		ClearTextLines();
	}

	public override void Update()
	{
		base.Update();
		mTimeInSequence += TimeManager.DeltaTime;
		if (mSequence == Sequence.Medals)
		{
			UpdateMedalSequence();
		}
		else if (mSequence == Sequence.Intel)
		{
			UpdateIntelSequence();
		}
	}

	private void UpdateMedalSequence()
	{
		int num = 0;
		int num2 = (int)(mTimeInSequence / mTimeForEachItem);
		int num3 = 0;
		while (num < TextLines.Length && num3 < mMedalsStrings.Length)
		{
			if (num3 < num2)
			{
				if (mMedalsIcons[num3] != null)
				{
					mMedalsIcons[num3].SetMedal(mSectionData.Medals[num3].Icon, ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran, mMedalsAchieved[num3]);
				}
				if (TextLines[num] != null)
				{
					TextLines[num].Text = mMedalsStrings[num3];
					num++;
				}
			}
			num3++;
		}
		if (num2 >= mItems)
		{
			mSequence = Sequence.Idle;
		}
	}

	private void UpdateIntelSequence()
	{
		mSequence = Sequence.Idle;
	}

	private void ClearTextLines()
	{
		SpriteText[] textLines = TextLines;
		foreach (SpriteText spriteText in textLines)
		{
			spriteText.Text = string.Empty;
		}
	}
}
