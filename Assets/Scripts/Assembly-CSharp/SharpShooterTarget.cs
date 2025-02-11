using UnityEngine;

public class SharpShooterTarget : MonoBehaviour
{
	public int EnemyShot = 5;

	public float StartingTime = 60f;

	public float CurrentTime;

	private bool isActive;

	public GameObject ScriptedObjectiveObject;

	private bool mFireOnce = true;

	private void Start()
	{
		isActive = false;
	}

	private void Update()
	{
		if (CommonHudController.Instance != null && !isActive)
		{
			CommonHudController.Instance.MissionTimer.StartTimer();
			CommonHudController.Instance.MissionTimer.Set(StartingTime, 0f);
			isActive = true;
		}
		if (mFireOnce && CommonHudController.Instance.MissionTimer.CurrentTime() <= 0f)
		{
			SquadCasualtiesObjective componentInChildren = ScriptedObjectiveObject.GetComponentInChildren<SquadCasualtiesObjective>();
			if (componentInChildren != null)
			{
				componentInChildren.ForceFail();
				mFireOnce = false;
			}
		}
	}

	public void Deactivate()
	{
	}

	public void Activate()
	{
		CurrentTime = CommonHudController.Instance.MissionTimer.CurrentTime();
		Debug.Log(CurrentTime);
		CurrentTime += EnemyShot;
		Debug.Log(CurrentTime);
		CommonHudController.Instance.MissionTimer.Set(CurrentTime, 0f);
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.ClearHeldMessage();
			HUDMessenger.Instance.PushMessage("+" + EnemyShot, false);
		}
	}
}
