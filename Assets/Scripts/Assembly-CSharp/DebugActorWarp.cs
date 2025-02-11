using UnityEngine;

public class DebugActorWarp : MonoBehaviour
{
	public DebugActorWarpData m_Interface;

	public ActorWrapper actorWrapper;

	private void Start()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	private void Update()
	{
		ActorWrapper componentInChildren = m_Interface.ActorToWarp.GetComponentInChildren<ActorWrapper>();
		if (componentInChildren != null)
		{
			if (componentInChildren.GetActor() != null)
			{
				CameraTarget cameraTarget = Object.FindObjectOfType(typeof(CameraTarget)) as CameraTarget;
				if (cameraTarget != null)
				{
					cameraTarget.GameCamera.FocusOnPoint(base.transform.position);
				}
				Object.DestroyImmediate(base.gameObject);
			}
		}
		else
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
