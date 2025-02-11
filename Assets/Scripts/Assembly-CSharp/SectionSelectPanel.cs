using System.Globalization;
using UnityEngine;

public class SectionSelectPanel : MonoBehaviour
{
	public delegate void SectionSelected(SectionData data, bool forceRefresh);

	private const float ICON_FRONT = -1.1f;

	private const float FRONT = -1f;

	private const float BACK = 0f;

	public MedalIconController[] Icons;

	public SpriteText Name;

	public SpriteText IntelText;

	public SpriteText XPLevelRequiredText;

	public SpriteText TokenUnlockText;

	public PackedSprite IntelIcon;

	public PackedSprite LockedImage;

	public PackedSprite GMGModeIcon;

	public PackedSprite[] EnvironmentImages;

	private CommonBackgroundBoxPlacement[] mPlacements;

	private SectionSelected mCallback;

	private SectionData mSectionData;

	private PackedSprite mCurrentImage;

	private CommonBackgroundBoxPlacement mListPlacement;

	private DifficultyMode mCachedDifficulty;

	private NumberFormatInfo mNfi;

	private float mWidth;

	private float mHeight;

	private int mSectionIndex;

	private MissionData mMissionData;

	public SectionData Data
	{
		get
		{
			return mSectionData;
		}
	}

	public int Index
	{
		get
		{
			return mSectionIndex;
		}
	}

	private void Awake()
	{
		mPlacements = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		mNfi = GlobalizationUtils.GetNumberFormat(0);
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
		Vector2 boxSize = new Vector2(mWidth, mHeight);
		CommonBackgroundBoxPlacement[] array = mPlacements;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
		if (mMissionData != null && mSectionIndex == mMissionData.Sections.Count - 1)
		{
			SwrveEventsUI.ViewedAll(mMissionData.MissionId);
		}
		SwrveEventsProgression.FirstSectionSelect();
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	public void LayoutComponents(float width, float height, CommonBackgroundBoxPlacement totalListBounds)
	{
		mWidth = width;
		mHeight = height;
		mListPlacement = totalListBounds;
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] array = mPlacements;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}

	public void Setup(int section, int specOpsImageStartIndex, MissionData missionData, DifficultyMode selectedDifficulty, SectionSelected callback)
	{
		SectionData sectionData = missionData.Sections[section];
		if (sectionData == null)
		{
			return;
		}
		mSectionData = sectionData;
		mSectionIndex = section;
		mMissionData = missionData;
		mCallback = callback;
		if (Name != null)
		{
			if (sectionData.Locked)
			{
				Name.Text = Language.Get("S_LOCKED");
			}
			else if (sectionData.IsSpecOps)
			{
				Name.Text = Language.Get(sectionData.Name);
			}
			else if (sectionData.Name != string.Empty)
			{
				Name.Text = string.Format("{0}. {1}", section + 1, Language.Get(sectionData.Name));
			}
			else
			{
				Name.Text = string.Format("{0}. {1}", section + 1, sectionData.SceneName);
			}
		}
		int num = StatsHelper.IntelCollectedForMission(missionData.MissionId, section);
		if (IntelIcon != null)
		{
			IntelIcon.SetColor((num == 0 || num != sectionData.IntelToCollect) ? ColourChart.GreyedOutIcon : Color.white);
			IntelIcon.gameObject.SetActive(!sectionData.IsSpecOps);
		}
		if (IntelText != null)
		{
			IntelText.Text = ((!sectionData.IsSpecOps) ? string.Format("{0}/{1}", num, sectionData.IntelToCollect) : string.Empty);
			IntelText.SetColor((!sectionData.Locked) ? Color.white : Color.clear);
		}
		if (LockedImage != null)
		{
			Vector3 position = LockedImage.transform.position;
			position.z = ((!sectionData.Locked) ? 0f : (-1f));
			LockedImage.transform.position = position;
		}
		if (XPLevelRequiredText != null && TokenUnlockText != null)
		{
			bool flag = sectionData.Locked && sectionData.UnlockedAtXpLevel != -1;
			XPLevelRequiredText.SetColor((!flag) ? Color.clear : Color.white);
			TokenUnlockText.SetColor((!flag) ? Color.clear : Color.white);
			if (flag)
			{
				char c = CommonHelper.HardCurrencySymbol();
				string arg = string.Format("{0}{1}", c, sectionData.UnlockEarlyCost.ToString("N", mNfi));
				XPLevelRequiredText.Text = string.Format(Language.Get("S_UNLOCKED_AT_XP_LEVEL"), sectionData.UnlockedAtXpLevel);
				TokenUnlockText.Text = string.Format(Language.Get("S_UNLOCK_EARLY_COST"), arg);
			}
		}
		if (GMGModeIcon != null)
		{
			int num2 = -1;
			if (sectionData.IsSpecOps)
			{
				switch (sectionData.GMGGameType)
				{
				case GMGData.GameType.Specops:
				case GMGData.GameType.Total:
					num2 = 0;
					break;
				case GMGData.GameType.TimeAttack:
					num2 = 1;
					break;
				case GMGData.GameType.Domination:
					num2 = 2;
					break;
				case GMGData.GameType.Flashpoint:
					num2 = 3;
					break;
				}
				if (num2 != -1)
				{
					GMGModeIcon.SetFrame(0, num2);
					GMGModeIcon.SetColor(ColourChart.UnselectedSection);
				}
			}
			Vector3 position2 = GMGModeIcon.transform.position;
			position2.z = ((!sectionData.IsSpecOps || num2 == -1) ? 0f : (-1.1f));
			GMGModeIcon.transform.position = position2;
		}
		if (EnvironmentImages != null)
		{
			for (int i = 0; i < EnvironmentImages.Length; i++)
			{
				if (!(EnvironmentImages[i] != null))
				{
					continue;
				}
				bool flag2 = !sectionData.Locked && missionData.SectionSelectImage == (MissionData.eSectionImages)i;
				Vector3 position3 = EnvironmentImages[i].transform.position;
				position3.z = ((!flag2) ? 0f : (-1f));
				EnvironmentImages[i].transform.position = position3;
				if (flag2)
				{
					if (missionData.Sections[mSectionIndex].IsSpecOps)
					{
						EnvironmentImages[i].SetFrame(0, specOpsImageStartIndex + mSectionIndex);
					}
					else
					{
						EnvironmentImages[i].SetFrame(0, mSectionIndex);
					}
					mCurrentImage = EnvironmentImages[i];
					mCurrentImage.SetColor(ColourChart.UnselectedSection);
				}
			}
		}
		UpdateDifficulty(selectedDifficulty);
	}

	public void UpdateDifficulty(DifficultyMode selectedDifficulty)
	{
		if (mMissionData != null && mSectionData != null)
		{
			DifficultyMode difficultyMode = (mCachedDifficulty = ((!mSectionData.IsSpecOps && !mSectionData.IsTutorial) ? selectedDifficulty : DifficultyMode.Regular));
			for (int i = 0; i < mSectionData.Medals.Length; i++)
			{
				bool earned = StatsHelper.HasMedalBeenEarnedGameTotal(mMissionData.MissionId, mSectionIndex, i, difficultyMode);
				Icons[i].SetMedal(mSectionData.Medals[i].Icon, difficultyMode == DifficultyMode.Veteran, earned);
			}
		}
	}

	public void SetSelected(bool selected)
	{
		if (mCurrentImage != null && !mSectionData.Locked)
		{
			mCurrentImage.SetColor((!selected) ? ColourChart.UnselectedSection : Color.white);
			GMGModeIcon.SetColor((!selected) ? ColourChart.UnselectedSection : Color.white);
		}
	}

	private void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
		if (MessageBoxController.Instance.IsAnyMessageActive)
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) || !hitInfo.collider.gameObject || mSectionData.Locked)
		{
			return;
		}
		for (int i = 0; i < Icons.Length; i++)
		{
			if (hitInfo.collider.gameObject == Icons[i].gameObject)
			{
				DoMedalInformation(i, Icons[i].gameObject);
				return;
			}
		}
		if (IntelIcon != null && hitInfo.collider.gameObject == IntelIcon.gameObject)
		{
			DoIntelInformation(IntelIcon.gameObject);
		}
	}

	public void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (!MessageBoxController.Instance.IsAnyMessageActive && mCallback != null)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			if (new Rect(vector.x - mWidth * 0.5f, vector.y - mHeight * 0.5f, mWidth, mHeight * 0.9f).Contains(fingerPos))
			{
				bool forceRefresh = false;
				mCallback(mSectionData, forceRefresh);
			}
		}
	}

	private void DoMedalInformation(int index, GameObject over)
	{
		string medalString = MedalManager.GetMedalString(mSectionData, index, mCachedDifficulty);
		ToolTipController.Instance.DoTooltip(medalString, over, mListPlacement.BoundingRect);
	}

	private void DoIntelInformation(GameObject over)
	{
		string text = Language.Get("S_INTEL_INFOMATION");
		ToolTipController.Instance.DoTooltip(text, over, mListPlacement.BoundingRect);
	}
}
