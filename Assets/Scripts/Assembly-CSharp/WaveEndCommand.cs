using System.Collections;
using UnityEngine;

public class WaveEndCommand : Command
{
	public int WaveNum;

	public int TokenReward;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameObject mh2 = GameObject.Find("Music_Danger");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		mh2 = GameObject.Find("Music_Combat");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		foreach (Grenade grenade in Grenade.GlobalPoolCache)
		{
			if (grenade != null)
			{
				grenade.MarkForDelete = true;
			}
		}
		string secondTxt = string.Empty;
		string thirdTxt = string.Empty;
		WaveNum = GMGData.Instance.CurrentWave();
		yield return new WaitForEndOfFrame();
		int xpForWaves = GMGData.Instance.CurrentWave() * WaveStats.Instance.XPForEachWave;
		int xpForAccuracy = WaveStats.Instance.Accuracy;
		StatsManager.Instance.PlayerStats().AddWaveXP(xpForWaves + xpForAccuracy, WaveNum);
		EventHub.Instance.Report(new Events.SpecOpsWaveComplete(WaveNum));
		SwrveEventsGameplay.WaveComplete(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection, ActStructure.Instance.CurrentMissionType(), WaveNum);
		CommonHudController.Instance.ShowGMGResults(true);
		if (HUDMessenger.Instance != null)
		{
			TokenReward = GMGData.Instance.GetRewardForWave(WaveNum);
			if (WaveNum % 3 == 0)
			{
				secondTxt = Language.Get("S_GMG_WAVE_END_REWARD_HEADER");
				thirdTxt = string.Format(Language.Get("S_GMG_WAVE_END_REWARD"), CommonHelper.HardCurrencySymbol(), TokenReward.ToString());
				GameSettings.Instance.PlayerCash().AwardHardCash(TokenReward, "SpecOpsWaveReward");
				GMGSFX.Instance.TokenAwarded.Play2D();
			}
			else
			{
				int nextRewardPoint = WaveNum + (3 - WaveNum % GMGData.Instance.WaveStep);
				int temp2 = nextRewardPoint - WaveNum;
				if (temp2 > 1)
				{
					int tokenReward2 = GMGData.Instance.GetRewardForWave(nextRewardPoint);
					secondTxt = string.Format(Language.Get("S_GMG_WAVE_MID"), temp2.ToString(), CommonHelper.HardCurrencySymbol(), tokenReward2.ToString());
				}
				else
				{
					temp2 = WaveNum + 1;
					int tokenReward2 = GMGData.Instance.GetRewardForWave(temp2);
					secondTxt = string.Format(Language.Get("S_GMG_WAVE_MID_NEXT"), CommonHelper.HardCurrencySymbol(), tokenReward2.ToString());
				}
			}
			GMGData.Instance.IncrementCurrentWave();
		}
		HUDMessenger.Instance.PushMessage(string.Format(Language.Get("S_GMG_WAVE_HEADER_END"), WaveNum.ToString()), secondTxt, thirdTxt, false);
		GMGSFX.Instance.WaveComplete.Play2D();
		yield return new WaitForSeconds(2f);
		string dial = "S_SURVIVAL_DIALOGUE_WAVECOMPLETE_0" + Random.Range(1, 5);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, SpecOpsVOSFX.Instance, dial, false, 1f);
		CommonHudController.Instance.ShowWave(false);
		if (GMGData.Instance.CurrentGameType == GMGData.GameType.Specops)
		{
			CommonHudController.Instance.MissionTimer.Set(GMGData.Instance.WaitBetweenWaves, 0f);
			CommonHudController.Instance.MissionTimer.StartTimer();
			float nextBleep = 5f;
			while (CommonHudController.Instance.MissionTimer.CurrentTime() > 0.5f)
			{
				if (CommonHudController.Instance.MissionTimer.CurrentTime() <= nextBleep)
				{
					if (CommonHudController.Instance.MissionTimer.CurrentTime() <= 1f)
					{
						GMGSFX.Instance.NextWaveCountdownLast.Play2D();
						break;
					}
					GMGSFX.Instance.NextWaveCountdown.Play2D();
					nextBleep -= 1f;
				}
				yield return null;
			}
		}
		else
		{
			CommonHudController.Instance.MissionTimer.Add(5f);
			CommonHudController.Instance.MissionTimer.PauseTimer();
			yield return new WaitForSeconds(GMGData.Instance.WaitBetweenWaves);
		}
	}
}
