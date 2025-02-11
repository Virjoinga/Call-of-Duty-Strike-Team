using System.Collections;
using UnityEngine;

public class WaveStartCommand : Command
{
	public enum EnemyDef
	{
		General = 0,
		RPG = 1,
		Sniper = 2,
		Riot = 3,
		Helicopter = 4,
		LMG = 5
	}

	public int WaveNum;

	public EnemyDef EnemyType;

	private int tokenReward;

	private bool firstWave;

	private string secondLineTxt = string.Empty;

	private string thirdLineTxt = string.Empty;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		EventHub.Instance.Report(new Events.SpecOpsWaveStarted(GMGData.Instance.CurrentWave()));
		if (HUDMessenger.Instance != null)
		{
			WaveNum = GMGData.Instance.CurrentWave();
			tokenReward = GMGData.Instance.GetRewardForWave(WaveNum);
			if (WaveNum % GMGData.Instance.WaveStep == 0)
			{
				secondLineTxt = string.Format(Language.Get("S_GMG_WAVE_THIS_REWARD"), CommonHelper.HardCurrencySymbol(), tokenReward.ToString());
			}
			else if (WaveNum == 1)
			{
				int temp = WaveNum + GMGData.Instance.WaveStep - 1;
				secondLineTxt = string.Format(Language.Get("S_GMG_WAVE_START"), temp.ToString(), CommonHelper.HardCurrencySymbol(), GMGData.Instance.GetRewardForWave(temp).ToString());
				firstWave = true;
			}
		}
		DoWarningDialogue();
		HUDMessenger.Instance.PushMessage(string.Format(Language.Get("S_GMG_WAVE_HEADER_START"), WaveNum.ToString()), secondLineTxt, thirdLineTxt, false);
		if (GMGData.Instance.CurrentGameType == GMGData.GameType.Specops)
		{
			CommonHudController.Instance.MissionTimer.StopTimer();
		}
		else if (CommonHudController.Instance.MissionTimer.CurrentState == MissionTimer.TimerState.Paused)
		{
			CommonHudController.Instance.MissionTimer.StartTimer();
		}
		CommonHudController.Instance.ShowWave(true);
		GMGSFX.Instance.NextWave.Play2D();
		GameObject mh2 = null;
		float result = WaveNum % 6;
		mh2 = ((result != 0f) ? GameObject.Find("Music_Combat") : GameObject.Find("Music_Danger"));
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Activate");
		}
		yield break;
	}

	private void DoWarningDialogue()
	{
		string key = "S_SURVIVAL_MSG_INCOMING_01";
		if (firstWave)
		{
			string text = "S_PICKUP_SURVIVAL_DIALOGUE_START_0" + Random.Range(1, 5);
			CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text);
			PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, SpecOpsVOSFX.Instance, text, false, 1f);
		}
		else
		{
			string text2 = string.Empty;
			switch (EnemyType)
			{
			case EnemyDef.Helicopter:
				text2 = "S_PICKUP_SURVIVAL_DIALOGUE_HELICOPTERINCOMING_0" + Random.Range(1, 3);
				key = "S_SURVIVAL_MSG_HELI_01";
				break;
			case EnemyDef.LMG:
				text2 = "S_SURVIVAL_DIALOGUE_LMGINCOMING_0" + Random.Range(1, 3);
				break;
			case EnemyDef.Riot:
				text2 = "S_PICKUP_SURVIVAL_DIALOGUE_RIOTINCOMING_0" + Random.Range(1, 3);
				key = "S_SURVIVAL_MSG_RIOT_01";
				break;
			case EnemyDef.RPG:
				text2 = "S_PICKUP_SURVIVAL_DIALOGUE_RPGINCOMING_0" + Random.Range(1, 3);
				key = "S_SURVIVAL_MSG_RPG_01";
				break;
			case EnemyDef.Sniper:
				text2 = "S_PICKUP_SURVIVAL_DIALOGUE_SNIPERINCOMING_0" + Random.Range(1, 3);
				key = "S_SURVIVAL_MSG_SNIPER_01";
				break;
			case EnemyDef.General:
				text2 = "S_PICKUP_SURVIVAL_DIALOGUE_WAVEINCOMING_0" + Random.Range(1, 3);
				break;
			}
			if (text2 != string.Empty)
			{
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text2);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, SpecOpsVOSFX.Instance, text2, false, 1f);
			}
		}
		thirdLineTxt = Language.Get(key);
	}
}
