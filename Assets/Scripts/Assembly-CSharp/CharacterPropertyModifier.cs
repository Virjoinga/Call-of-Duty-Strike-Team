using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPropertyModifier : MonoBehaviour
{
	public CharacterPropertyData m_Interface;

	public List<Actor> actors = new List<Actor>();

	public List<ActorWrapper> actorWrappers = new List<ActorWrapper>();

	public Collider ModifyActorsInVolume;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void addToActors(GameObject mEnemy)
	{
		Actor component = mEnemy.GetComponent<Actor>();
		if (component != null)
		{
			actors.Add(component);
		}
	}

	public void clearActors()
	{
		actors.Clear();
	}

	private void FillInActors()
	{
		actors.Clear();
		if (ModifyActorsInVolume != null || (ModifyActorsInVolume == null && actorWrappers.Count == 0))
		{
			List<Actor> actorsOfType = ActorAccessor.GetActorsOfType(m_Interface.ActorTypesToModify, ModifyActorsInVolume, m_Interface.OnlyActorsOutsideArea, m_Interface.OptionalEntityTag);
			foreach (Actor item in actorsOfType)
			{
				if (!item.health.HealthEmpty)
				{
					actors.Add(item);
				}
			}
		}
		if (actorWrappers.Count == 0)
		{
			return;
		}
		foreach (ActorWrapper actorWrapper in actorWrappers)
		{
			if (actorWrapper.GetActor() != null && !actorWrapper.GetActor().health.HealthEmpty)
			{
				actors.Add(actorWrapper.GetActor());
			}
		}
	}

	public void ToggleInvincible()
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				actor.health.Invulnerable = !actor.health.Invulnerable;
				LogMessage(actor.gameObject, "Setting Invincible " + actor.health.Invulnerable, base.gameObject);
			}
		}
	}

	public void Invincible(bool bOn)
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				actor.health.Invulnerable = bOn;
				LogMessage(actor.gameObject, "Setting Invincible " + actor.health.Invulnerable, base.gameObject);
			}
		}
	}

	public void ToggleFireAtWill()
	{
		FillInActors();
		ToggleFireAtWill(actors, base.gameObject);
	}

	public static void ToggleFireAtWill(List<Actor> actorsToModify, GameObject thisObject)
	{
		foreach (Actor item in actorsToModify)
		{
			if (item != null)
			{
				item.tasks.CancelAllTasks();
				RoutineDescriptor componentInChildren = item.GetComponentInChildren<RoutineDescriptor>();
				if (componentInChildren != null && componentInChildren.m_Interface.NoneCombatAI)
				{
					item.fireAtWill.Enabled = !item.fireAtWill.Enabled;
					LogMessage(item.gameObject, "Setting Fire At Will " + item.fireAtWill.Enabled, thisObject);
				}
			}
		}
	}

	public void SetFactionNeutral()
	{
		SetFaction(FactionHelper.Category.Neutral);
	}

	public void SetFactionPlayer()
	{
		SetFaction(FactionHelper.Category.Player);
	}

	public void SetFactionEnemy()
	{
		SetFaction(FactionHelper.Category.Enemy);
	}

	public void SetFactionSoloEnemy()
	{
		SetFaction(FactionHelper.Category.SoloEnemy);
	}

	public void SetFaction(FactionHelper.Category faction)
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			actor.awareness.SetFaction(faction);
		}
	}

	public void DominationSwitchRoutine(GameObject routines)
	{
		foreach (Actor actor in actors)
		{
			actor.tasks.CancelAllTasks();
		}
		SwitchRoutine(routines, actors, base.gameObject);
	}

	public void SwitchRoutine(GameObject routines)
	{
		FillInActors();
		SwitchRoutine(routines, actors, base.gameObject);
	}

	public static void SwitchRoutine(GameObject routine, List<Actor> actorsToModify, GameObject thisObject)
	{
		foreach (Actor item in actorsToModify)
		{
			if (item != null && item.awareness.ChDefCharacterType != CharacterType.RiotShieldNPC && item.awareness.ChDefCharacterType != CharacterType.RPG)
			{
				GameplayController gameplayController = GameplayController.Instance();
				if ((bool)gameplayController)
				{
					gameplayController.StartCoroutine(DeferedSwitchRoutine(item, routine, thisObject));
				}
			}
		}
	}

	public static IEnumerator DeferedSwitchRoutine(Actor a, GameObject routine, GameObject thisObject)
	{
		while (a.tasks.GetRunningTask(typeof(TaskSetPiece)) != null)
		{
			yield return new WaitForEndOfFrame();
		}
		a.tasks.CancelAllTasks();
		RoutineDescriptor rd = routine.GetComponentInChildren<RoutineDescriptor>();
		Container[] componentsInChildren = routine.GetComponentsInChildren<Container>();
		foreach (Container c in componentsInChildren)
		{
			RoutineDescriptor[] componentsInChildren2 = c.gameObject.GetComponentsInChildren<RoutineDescriptor>();
			foreach (RoutineDescriptor t in componentsInChildren2)
			{
				Task ret = t.CreateTask(a.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType);
				if (ret != null && ret is TaskRoutine)
				{
					TaskRoutine taskRoutine = ret as TaskRoutine;
					taskRoutine.OneShotRoutineTasks = rd.m_Interface.OneShotRoutineTasks;
					taskRoutine.PingPongRoutineOrdering = rd.m_Interface.PingPongRoutineOrdering;
					if (rd != null && rd.m_Interface.NoneCombatAI)
					{
						a.fireAtWill.Enabled = !rd.m_Interface.NoneCombatAI;
					}
				}
			}
		}
	}

	public void ClearExistingTasks()
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				actor.tasks.CancelAllTasks();
			}
		}
	}

	public void SwitchEvents(GameObject events)
	{
		FillInActors();
		ClearExistingEvents();
		CopyNewEvents(events);
	}

	public static void SwitchEvents(Actor a, GameObject events)
	{
		ClearExistingEvents(a);
		CopyNewEvents(a, events);
	}

	public void CopyNewEvents(GameObject events)
	{
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				CopyNewEvents(actor, events);
				LogMessage(actor.gameObject, " Switching Events (" + events.name + ")", base.gameObject);
			}
		}
	}

	public static void CopyNewEvents(Actor a, GameObject events)
	{
		EventsCreator componentInChildren = events.GetComponentInChildren<EventsCreator>();
		if (componentInChildren != null)
		{
			foreach (EventDescriptor @event in componentInChildren.GetEvents())
			{
				@event.Initialise(a.gameObject);
			}
		}
		events.name = a.name + " Events List";
		events.transform.parent = a.transform;
	}

	public void ClearExistingEvents()
	{
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				ClearExistingEvents(actor);
			}
		}
	}

	public static void ClearExistingEvents(Actor a)
	{
		EventsCreator componentInChildren = a.GetComponentInChildren<EventsCreator>();
		if (!(componentInChildren != null))
		{
			return;
		}
		foreach (EventDescriptor @event in componentInChildren.GetEvents())
		{
			UnityEngine.Object.Destroy(@event);
		}
	}

	public void SetActorsSelected()
	{
		FillInActors();
		if (actors == null)
		{
			return;
		}
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController == null)
		{
			return;
		}
		int num = 0;
		foreach (Actor actor in actors)
		{
			if (num == 0)
			{
				gameplayController.SelectOnlyThis(actor);
			}
			else
			{
				gameplayController.AddToSelected(actor);
			}
			num++;
		}
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
		if (playCameraInterface != null && actors[0] != null)
		{
			playCameraInterface.FocusOnTarget(actors[0].transform, true);
		}
	}

	public static void SetUnitAggression(Actor a, string aggresive)
	{
		if (a != null && aggresive == "true")
		{
			a.behaviour.aggressive = true;
		}
		if (a != null && aggresive == "false")
		{
			a.behaviour.aggressive = false;
		}
	}

	public void SetUnitAggression(string aggresive)
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			SetUnitAggression(actor, aggresive);
		}
	}

	public static void ToggleUnitAggression(Actor a)
	{
		if (a != null)
		{
			a.behaviour.aggressive = !a.behaviour.aggressive;
		}
	}

	public void ToggleUnitAggression()
	{
		FillInActors();
		foreach (Actor actor in actors)
		{
			ToggleUnitAggression(actor);
		}
	}

	public void TogglePlayerControl()
	{
		FillInActors();
		TogglePlayerControl(actors, base.gameObject);
	}

	public void DisablePlayerControl()
	{
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				SetControl(actor, false, base.gameObject, false);
			}
		}
	}

	public void ToggleCameoImage()
	{
		FillInActors();
		ToggleCameoImage(actors, base.gameObject);
	}

	public static void SetControl(Actor a, bool enable, GameObject thisObject, bool doLockHud)
	{
		if (!WorldHelper.IsPlayerControlledActor(a))
		{
			return;
		}
		if (enable)
		{
			if (a.realCharacter.ScriptHUDControl)
			{
				a.realCharacter.SetSelectable(true, true, GameController.Instance.IsFirstPerson);
				a.realCharacter.ScriptHUDControl = false;
				if (a.baseCharacter.IsFirstPerson && doLockHud)
				{
					LockHudItemsCommand.RestoreHUDItems();
				}
				if (SectionTypeHelper.IsAGMG())
				{
					GameplayController.instance.AddToSelected(GameController.Instance.mFirstPersonActor);
				}
			}
		}
		else if (!a.realCharacter.ScriptHUDControl)
		{
			a.realCharacter.ScriptHUDControl = true;
			a.realCharacter.SetSelectable(false, false);
			if (a.baseCharacter.IsFirstPerson && doLockHud)
			{
				LockHudItemsCommand.DoLockHUDItems(null, false, false, true);
			}
		}
		LogMessage(a.gameObject, " Toggling Control " + a.realCharacter.Selectable + " ", thisObject);
	}

	public static void TogglePlayerControl(List<Actor> actorsToModify, GameObject thisObject)
	{
		foreach (Actor item in actorsToModify)
		{
			if (item != null)
			{
				SetControl(item, !item.realCharacter.Selectable, thisObject, false);
			}
		}
	}

	public static void SetPlayerControl(Actor actorToModify, GameObject thisObject, bool onOff)
	{
		if (actorToModify != null)
		{
			SetControl(actorToModify, onOff, thisObject, false);
		}
	}

	public static void ToggleCameoImage(List<Actor> actorsToModify, GameObject thisObject)
	{
		foreach (Actor item in actorsToModify)
		{
			if (item != null)
			{
				CommonHudController.Instance.TPPUnitSelecter.DisableUnitCameo(item.baseCharacter.UnitName);
			}
		}
	}

	public static void SetTargetPriorityModifier(List<Actor> actors, string val)
	{
		foreach (Actor actor in actors)
		{
			if (actor != null)
			{
				TargetScorer.SetActorModifierScore(actor, float.Parse(val));
			}
		}
	}

	public void SetTargetPriorityModifier(string val)
	{
		FillInActors();
		SetTargetPriorityModifier(actors, val);
	}

	public void ResetLastTargetPriorityModifiedActors()
	{
		FillInActors();
		SetTargetPriorityModifier(actors, "0");
	}

	public static void LogMessage(GameObject obj, string function, GameObject thisObject)
	{
	}

	public void DeactivateCM()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			if (!(actor != null))
			{
				continue;
			}
			InterfaceableObject[] componentsInChildren = actor.GetComponentsInChildren<InterfaceableObject>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				InterfaceableObject[] array = componentsInChildren;
				foreach (InterfaceableObject interfaceableObject in array)
				{
					interfaceableObject.Deactivate();
				}
			}
		}
	}

	public void ActivateCM()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			if (!(actor != null))
			{
				continue;
			}
			InterfaceableObject[] componentsInChildren = actor.GetComponentsInChildren<InterfaceableObject>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				InterfaceableObject[] array = componentsInChildren;
				foreach (InterfaceableObject interfaceableObject in array)
				{
					interfaceableObject.Activate();
				}
			}
		}
	}

	public void IncreaseAlertLevel()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			BehaviourController behaviour = actor.behaviour;
			if (behaviour.nextAlertState < BehaviourController.AlertState.Combat)
			{
				behaviour.nextAlertState++;
			}
		}
	}

	public void DecreaseAlertLevel()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			BehaviourController behaviour = actor.behaviour;
			if (behaviour.nextAlertState > BehaviourController.AlertState.Casual)
			{
				behaviour.nextAlertState--;
			}
		}
	}

	public void SetAlertLevel(string level)
	{
		BehaviourController.AlertState nextAlertState = BehaviourController.AlertState.Casual;
		bool flag = false;
		BehaviourController.AlertState[] values = EnumUtils.GetValues<BehaviourController.AlertState>();
		foreach (BehaviourController.AlertState alertState in values)
		{
			if (level == alertState.ToString())
			{
				nextAlertState = alertState;
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.LogError(level + " is not a valid alert level");
			return;
		}
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			actor.behaviour.nextAlertState = nextAlertState;
		}
	}

	public void SilentRemove()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			SilentRemove(actor);
		}
	}

	public static void Kill(Actor a)
	{
		EventsCreator componentInChildren = a.gameObject.GetComponentInChildren<EventsCreator>();
		if (componentInChildren != null)
		{
			foreach (EventDescriptor eventDescriptor in componentInChildren.m_EventDescriptors)
			{
				if (eventDescriptor != null)
				{
					eventDescriptor.DeInitialise();
					UnityEngine.Object.Destroy(eventDescriptor);
				}
			}
		}
		a.health.Kill("Script", a.gameObject);
	}

	public static void WaveEnemyKill(Actor a)
	{
		EventsCreator componentInChildren = a.gameObject.GetComponentInChildren<EventsCreator>();
		if (componentInChildren != null)
		{
			foreach (EventDescriptor eventDescriptor in componentInChildren.m_EventDescriptors)
			{
				if (eventDescriptor != null)
				{
					eventDescriptor.InitialiseForActor(a);
					eventDescriptor.DeInitialise();
					UnityEngine.Object.Destroy(eventDescriptor);
				}
			}
		}
		a.health.Kill("Script", a.gameObject);
	}

	public static void KillAndHide(Actor a)
	{
		Kill(a);
		SilentRemove(a);
	}

	public static void SilentRemove(Actor a)
	{
		SilentRemove(a, true);
	}

	public static void SilentRemove(Actor a, bool triggerEvents)
	{
		if (!(a != null))
		{
			return;
		}
		if (!triggerEvents)
		{
			EventsCreator componentInChildren = a.gameObject.GetComponentInChildren<EventsCreator>();
			if (componentInChildren != null)
			{
				foreach (EventDescriptor eventDescriptor in componentInChildren.m_EventDescriptors)
				{
					if (eventDescriptor != null)
					{
						eventDescriptor.DeInitialise();
						UnityEngine.Object.Destroy(eventDescriptor);
					}
				}
			}
		}
		UnityEngine.Object.Destroy(a.model.gameObject);
		if (a.realCharacter.Ragdoll != null)
		{
			UnityEngine.Object.Destroy(a.realCharacter.Ragdoll.gameObject);
		}
		if (a.realCharacter.SimpleHitBounds != null)
		{
			UnityEngine.Object.Destroy(a.realCharacter.SimpleHitBounds.gameObject);
		}
		UnityEngine.Object.Destroy(a.gameObject);
	}

	public void Cleanup()
	{
		FillInActors();
		if (actors == null || actors.Count <= 0)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			KillAndHide(actor);
		}
	}

	public void AddAmmo(string amount)
	{
		FillInActors();
		AddAmmo(actors, amount);
	}

	public static void AddAmmo(List<Actor> actors, string amount)
	{
		int amount2 = Convert.ToInt32(amount);
		if (actors == null)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			actor.weapon.PrimaryWeapon.GetWeaponAmmo().AddAmmo(amount2);
		}
	}

	public static void AddAmmoPercent(List<Actor> actors, string amt)
	{
		float amt2 = float.Parse(amt);
		if (actors == null)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			actor.weapon.PrimaryWeapon.GetWeaponAmmo().AddAmmoByPercent(amt2);
		}
	}

	public void FillAmmoByPercent(string amt)
	{
		FillInActors();
		FillAmmoByPercent(actors, amt);
	}

	public static void FillAmmoByPercent(List<Actor> actors, string amt)
	{
		float amt2 = float.Parse(amt);
		if (actors == null)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			actor.weapon.PrimaryWeapon.GetWeaponAmmo().FillAmmoByPercent(amt2);
		}
	}

	public void AddHealth(string amount)
	{
		FillInActors();
		AddHealth(actors, amount);
	}

	public static void AddHealth(List<Actor> actors, string amount)
	{
		float amount2 = Convert.ToSingle(amount);
		if (actors == null)
		{
			return;
		}
		foreach (Actor actor in actors)
		{
			actor.health.ModifyHealth(actor.gameObject, amount2, "Recharge", Vector3.zero, false);
		}
	}
}
