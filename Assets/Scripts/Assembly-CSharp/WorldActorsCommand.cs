using UnityEngine;

public class WorldActorsCommand : MonoBehaviour
{
	public WorldActorData m_Interface;

	public Collider OptionalAreaToUse;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void PerformCommand()
	{
		foreach (Actor item in ActorAccessor.GetActorsOfType(m_Interface.TypesOfActor, OptionalAreaToUse, m_Interface.OnlyActorsOutsideArea, m_Interface.OptionalEntityTag))
		{
			ToggleAction(item);
		}
		if (!m_Interface.IncludeFakeActors)
		{
			return;
		}
		foreach (FakeGunmanOverride item2 in ActorAccessor.GetFakeActorsOfType(base.gameObject, m_Interface.TypesOfActor, OptionalAreaToUse))
		{
			ToggleAction(item2);
		}
	}

	private void ToggleAction(FakeGunmanOverride g)
	{
		switch (m_Interface.TypeOfAction)
		{
		case WorldActorData.ActionTypes.AI:
			g.SendOverrideMessage(g.gameObject, "Deactivate");
			break;
		case WorldActorData.ActionTypes.Kill:
			g.SendOverrideMessage(g.gameObject, "Kill");
			break;
		case WorldActorData.ActionTypes.Enabled:
			g.enabled = !g.enabled;
			break;
		case WorldActorData.ActionTypes.KillAndHide:
			g.SendOverrideMessage(g.gameObject, "Kill");
			Object.Destroy(g.gameObject);
			break;
		}
	}

	private void ToggleAction(Actor a)
	{
		switch (m_Interface.TypeOfAction)
		{
		case WorldActorData.ActionTypes.AI:
			a.tasks.CancelAllTasks();
			break;
		case WorldActorData.ActionTypes.Kill:
		{
			if (a.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
			{
				CharacterPropertyModifier.Kill(a);
				break;
			}
			TaskSecurityCamera taskSecurityCamera2 = (TaskSecurityCamera)a.tasks.GetRunningTask(typeof(TaskSecurityCamera));
			if (taskSecurityCamera2 != null)
			{
				taskSecurityCamera2.DisableCamera();
			}
			break;
		}
		case WorldActorData.ActionTypes.Enabled:
			if (WorldHelper.IsPlayerControlledActor(a))
			{
				CharacterPropertyModifier.SetControl(a, !a.enabled, base.gameObject, false);
			}
			else if (a.realCharacter != null)
			{
				a.realCharacter.ChangeHUDStatus(!a.enabled);
				if (a.realCharacter.Ragdoll != null)
				{
					a.realCharacter.Ragdoll.enabled = !a.enabled;
				}
				if (a.realCharacter.SimpleHitBounds != null)
				{
					a.realCharacter.SimpleHitBounds.enabled = !a.enabled;
				}
				if (a.realCharacter.SnapTarget != null)
				{
					a.realCharacter.SnapTarget.enabled = !a.enabled;
				}
			}
			if (a.realCharacter.Shadow != null)
			{
				a.realCharacter.Shadow.gameObject.SetActive(!a.enabled);
			}
			a.enabled = !a.enabled;
			a.model.gameObject.SetActive(!a.model.gameObject.activeInHierarchy);
			break;
		case WorldActorData.ActionTypes.KillAndHide:
		{
			if (a.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
			{
				CharacterPropertyModifier.KillAndHide(a);
				break;
			}
			TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)a.tasks.GetRunningTask(typeof(TaskSecurityCamera));
			if (taskSecurityCamera != null)
			{
				taskSecurityCamera.DisableCamera();
			}
			break;
		}
		}
	}

	public void Activate()
	{
		PerformCommand();
	}

	public void Deactivate()
	{
		PerformCommand();
	}

	public void OnDrawGizmos()
	{
		if (OptionalAreaToUse != null)
		{
			BoxCollider boxCollider = OptionalAreaToUse as BoxCollider;
			if (boxCollider != null)
			{
				Gizmos.color = Color.black.Alpha(0.25f);
				Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size);
				Gizmos.color = Color.black;
				Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
			}
		}
	}
}
