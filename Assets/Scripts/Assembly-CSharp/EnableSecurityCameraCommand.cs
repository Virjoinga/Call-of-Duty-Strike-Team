using System.Collections;
using UnityEngine;

public class EnableSecurityCameraCommand : Command
{
	public ActorWrapper SecurityCamera;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (SecurityCamera != null)
		{
			Actor actor = SecurityCamera.GetActor();
			if (actor != null)
			{
				TaskSecurityCamera securityCameraTask = actor.tasks.GetRunningTask<TaskSecurityCamera>();
				if (securityCameraTask != null)
				{
					securityCameraTask.EnableCamera();
				}
				else
				{
					Debug.Log("ERROR - EnableSecurityCameraCommand : Null securityCameraTask");
				}
			}
			else
			{
				Debug.Log("ERROR - EnableSecurityCameraCommand : Null realCharacter");
			}
		}
		else
		{
			Debug.Log("ERROR - EnableSecurityCameraCommand : Null SecurityCamera");
		}
		yield break;
	}
}
