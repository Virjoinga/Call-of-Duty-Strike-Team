using System.Collections;
using UnityEngine;

public class WhileNotCloseEnoughToStealthCommand : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		bool closeEnough = false;
		bool triggerVisible = false;
		float triggerActivatedTime = 0f;
		while (!closeEnough)
		{
			if (!triggerVisible && CommonHudController.Instance.GetTriggerFrame() == 1)
			{
				triggerActivatedTime = Time.realtimeSinceStartup;
				triggerVisible = true;
			}
			else if (triggerVisible && CommonHudController.Instance.GetTriggerFrame() != 1)
			{
				triggerVisible = false;
			}
			else if (triggerVisible)
			{
				if (Time.realtimeSinceStartup - triggerActivatedTime > 0.25f)
				{
					closeEnough = true;
				}
			}
			else if (GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.tasks.IsRunningTask(typeof(TaskStealthKill)))
			{
				closeEnough = true;
			}
			yield return null;
		}
	}
}
