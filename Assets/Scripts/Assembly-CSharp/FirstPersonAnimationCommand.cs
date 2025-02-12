using System.Collections;
using UnityEngine;

public class FirstPersonAnimationCommand : Command
{
	public string Id;

	public AnimationClip Clip;

	public GameObject Actor;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		ActorWrapper actorWrapper = ((!(Actor != null)) ? null : Actor.GetComponentInChildren<ActorWrapper>());
		Actor actor = ((!(actorWrapper != null)) ? null : actorWrapper.GetActor());
		float time = 0f - Time.deltaTime;
		while (time < Clip.length)
		{
			time += Time.deltaTime;
			ViewModelRig.Instance().SetOverride(Id, Clip, time, Vector3.zero, Quaternion.identity, true);
			yield return null;
		}
		Transform cameraLocator = ViewModelRig.Instance().GetCameraLocator();
		UnityEngine.AI.NavMeshHit navMeshHit;
		if (actor != null && actor.realCharacter != null && actor.realCharacter.FirstPersonCamera != null && cameraLocator != null && UnityEngine.AI.NavMesh.SamplePosition(cameraLocator.position, out navMeshHit, 2f, actor.navAgent.walkableMask))
		{
			actor.SetPosition(navMeshHit.position);
			Vector3 angles = cameraLocator.eulerAngles;
			if (angles.x < -180f)
			{
				angles += new Vector3(360f, 0f, 0f);
			}
			else if (angles.x > 180f)
			{
				angles -= new Vector3(360f, 0f, 0f);
			}
			actor.realCharacter.FirstPersonCamera.Angles = angles;
		}
		ViewModelRig.Instance().ClearOverride();
	}
}
