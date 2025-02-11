using UnityEngine;

public class WarpToFixedGun : MonoBehaviour
{
	public GameObject Actor;

	public GameObject FixedGun;

	public bool LockIn;

	public bool Airborne;

	public bool DontDismount;

	public bool UseMainCharacter;

	public bool SuppressTransition;

	private void Update()
	{
		Actor actor = null;
		if (UseMainCharacter)
		{
			GameObject firstScriptTriggerActor = GameplayController.Instance().FirstScriptTriggerActor;
			if (firstScriptTriggerActor != null)
			{
				actor = IncludeDisabled.GetComponentInChildren<Actor>(firstScriptTriggerActor);
			}
		}
		else
		{
			ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
			if (componentInChildren == null)
			{
				return;
			}
			actor = componentInChildren.GetActor();
		}
		if (actor == null)
		{
			return;
		}
		FixedGun componentInChildren2 = FixedGun.GetComponentInChildren<FixedGun>();
		if (componentInChildren2 == null)
		{
			return;
		}
		Task.Config flags = Task.Config.Default;
		new TaskCacheStanceState(actor.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, actor);
		new TaskUseFixedGun(actor.tasks, TaskManager.Priority.IMMEDIATE, flags, componentInChildren2, true, Airborne, DontDismount, SuppressTransition);
		base.enabled = false;
		if (LockIn)
		{
			GameController.Instance.IsLockedToFirstPerson = true;
			GameController.Instance.IsLockedToCurrentCharacter = true;
			if (Airborne)
			{
				actor.realCharacter.ChangeHUDStatus(false);
				SoldierMarker.SetStatesForVTOLGunner();
			}
		}
	}
}
