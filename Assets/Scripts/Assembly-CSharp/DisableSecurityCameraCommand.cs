using System.Collections;
using UnityEngine;

public class DisableSecurityCameraCommand : Command
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
					securityCameraTask.DisableCamera();
				}
				else
				{
					Debug.Log("ERROR - DisableSecurityCameraCommand : Null securityCameraTask");
				}
			}
			else
			{
				Debug.Log("ERROR - DisableSecurityCameraCommand : Null realCharacter");
			}
		}
		else
		{
			Debug.Log("ERROR - DisableSecurityCameraCommand : Null SecurityCamera");
		}
		yield break;
	}
}
