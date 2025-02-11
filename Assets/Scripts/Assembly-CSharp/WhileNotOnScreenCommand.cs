using System.Collections;
using UnityEngine;

public class WhileNotOnScreenCommand : Command
{
	public GameObject ObjectToTest;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		ActorWrapper aw = ObjectToTest.GetComponentInChildren<ActorWrapper>();
		Actor a = null;
		if (aw != null)
		{
			a = aw.GetActor();
		}
		float radiusFactor = 1.4f;
		Vector3 characterCentre2 = ObjectToTest.transform.position + base.transform.up;
		Vector3 viewPortPos2 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre2);
		float radius2 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre2 + radiusFactor * CameraManager.Instance.CurrentCamera.transform.right).x - viewPortPos2.x;
		CameraManager cameras = CameraManager.Instance;
		NavMeshCamera nmc = null;
		if (cameras != null)
		{
			nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		}
		bool quitLoop = false;
		bool camMoving2 = true;
		while (!quitLoop)
		{
			camMoving2 = ((!(nmc != null) || !(nmc.PanVelocity.magnitude < 6f)) ? true : false);
			if (a == null)
			{
				characterCentre2 = ObjectToTest.transform.position + base.transform.up;
				viewPortPos2 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre2);
				radius2 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre2 + radiusFactor * CameraManager.Instance.CurrentCamera.transform.right).x - viewPortPos2.x;
				if (viewPortPos2.z >= 0f && viewPortPos2.x >= 0f - radius2 && viewPortPos2.x <= 1f + radius2 && viewPortPos2.y >= 0f - radius2 && viewPortPos2.y <= 1f + radius2 && !camMoving2)
				{
					quitLoop = true;
				}
			}
			else if (a.OnScreen && !camMoving2)
			{
				quitLoop = true;
			}
			yield return null;
		}
	}
}
