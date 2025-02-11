using UnityEngine;

public class MissionCompleteSoldierPanelsSeqencer : MonoBehaviour
{
	private enum Sequence
	{
		On = 0,
		MiaKia = 1,
		Idle = 2
	}

	public MissionCompleteSoldierPanelController[] SoldierPanels;

	private float mTimeOnPerPanel;

	private float mCurrentTime;

	private int mNumSoldierPanels;

	private Sequence mSequence;

	private bool mSpecOps;

	public void BeginOnSequence(float timeToTake)
	{
		if (mSpecOps)
		{
			return;
		}
		MissionListings.eMissionID lastMissionID = ActStructure.Instance.LastMissionID;
		MissionData mission = MissionListings.Instance.Mission(lastMissionID);
		GameSettings instance = GameSettings.Instance;
		if (instance.VtolSoldierPresent)
		{
			mNumSoldierPanels = 1;
			string soldierName = Language.Get("S_DEFAULT_SOLDIER_NAME_5");
			SoldierPanels[0].Setup(4, soldierName, true, mission, 0);
		}
		else
		{
			mNumSoldierPanels = 0;
			for (int i = 0; i < SoldierPanels.Length; i++)
			{
				MissionCompleteSoldierPanelController missionCompleteSoldierPanelController = SoldierPanels[i];
				if (missionCompleteSoldierPanelController != null && instance.Soldiers[i].Present)
				{
					missionCompleteSoldierPanelController.Setup(i, instance.Soldiers[i], mission);
					mNumSoldierPanels++;
				}
			}
		}
		mTimeOnPerPanel = timeToTake / (float)mNumSoldierPanels * 0.5f;
		mCurrentTime = 0f;
		mSequence = Sequence.On;
	}

	public void AnimateBladesOff(float timeToTake)
	{
		float num = timeToTake / (float)mNumSoldierPanels;
		for (int i = 0; i < SoldierPanels.Length; i++)
		{
			MissionCompleteSoldierPanelController missionCompleteSoldierPanelController = SoldierPanels[i];
			if (missionCompleteSoldierPanelController != null)
			{
				missionCompleteSoldierPanelController.DelayedDeactivate(num * (float)(i + 1));
			}
		}
	}

	public void BeginMiaKiaSequence(float timeToTake)
	{
		if (!mSpecOps)
		{
			mTimeOnPerPanel = timeToTake / (float)mNumSoldierPanels;
			mCurrentTime = 0f;
			mSequence = Sequence.MiaKia;
		}
	}

	private void Awake()
	{
		mSpecOps = ActStructure.Instance.CurrentMissionIsSpecOps();
		mSequence = Sequence.Idle;
	}

	private void Update()
	{
		if (mSpecOps || mSequence == Sequence.Idle)
		{
			return;
		}
		GameSettings instance = GameSettings.Instance;
		mCurrentTime += TimeManager.DeltaTime;
		int num = (int)(mCurrentTime / mTimeOnPerPanel);
		for (int i = 0; i < SoldierPanels.Length; i++)
		{
			MissionCompleteSoldierPanelController missionCompleteSoldierPanelController = SoldierPanels[i];
			if (i > num)
			{
				continue;
			}
			bool flag = false;
			flag = ((!instance.VtolSoldierPresent) ? instance.Soldiers[i].Present : (i == 0));
			if (missionCompleteSoldierPanelController != null && flag)
			{
				if (mSequence != Sequence.Idle && !missionCompleteSoldierPanelController.IsActive && !missionCompleteSoldierPanelController.IsTransitioning)
				{
					missionCompleteSoldierPanelController.Activate();
				}
				if (mSequence == Sequence.MiaKia)
				{
					missionCompleteSoldierPanelController.UpdateStatus();
				}
			}
			if (i == SoldierPanels.Length - 1)
			{
				mSequence = Sequence.Idle;
			}
		}
	}

	public void PlayerPurchasedSoldierXP(int index)
	{
		MissionCompleteSoldierPanelController missionCompleteSoldierPanelController = SoldierPanels[index];
		missionCompleteSoldierPanelController.PlayerPurchasedXP();
	}
}
