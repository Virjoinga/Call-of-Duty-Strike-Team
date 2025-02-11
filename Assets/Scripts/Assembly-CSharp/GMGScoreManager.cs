using System;
using UnityEngine;

public class GMGScoreManager : MonoBehaviour
{
	private ScriptedObjective mObjective;

	public GameObject mObjective_Object;

	private void Awake()
	{
		OnDisable();
		WaveStats.Instance.Reset();
	}

	public void SetStartWave(string val)
	{
		GMGData.Instance.SetStartWave(Convert.ToInt32(val));
	}

	public void UpdateWave()
	{
		CommonHudController.Instance.UpdateWave(GMGData.Instance.CurrentWave());
	}

	private void UpdateScore(object sender, Events.XPEarned args)
	{
		EventHub.Instance.Report(new Events.GMGScoreAdded(args.XP));
	}

	private void UpdateEnemiesRemaining(GameObject sender)
	{
		SendMessageOnRecievedCount componentInChildren = sender.GetComponentInChildren<SendMessageOnRecievedCount>();
		if ((bool)componentInChildren)
		{
			CommonHudController.Instance.UpdateEnemiesRemaining(componentInChildren.Target - componentInChildren.Count);
		}
	}

	protected void OnEnable()
	{
		EventHub.Instance.OnXPEarned += UpdateScore;
		CommonHudController.Instance.ShowWave(true);
		CommonHudController.Instance.UpdateTokens(GameSettings.Instance.PlayerCash().HardCash());
		if (mObjective_Object == null)
		{
			mObjective_Object = GameObject.Find("Scripted");
		}
		if (mObjective_Object != null)
		{
			mObjective = mObjective_Object.GetComponentInChildren<ScriptedObjective>();
			if (mObjective != null)
			{
				mObjective.SetDormantState();
				mObjective.m_Interface.ObjectiveLabel = "S_SURVIVAL_OBJ_SURVIVE";
				mObjective.Activate();
			}
		}
	}

	protected void OnDisable()
	{
		EventHub.Instance.OnXPEarned -= UpdateScore;
		CommonHudController.Instance.ShowWave(false);
	}
}
