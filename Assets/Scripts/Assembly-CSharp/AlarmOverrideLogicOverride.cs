using System.Collections.Generic;
using UnityEngine;

public class AlarmOverrideLogicOverride : ContainerOverride
{
	public ActorOverride actor;

	public GameObject alarm;

	public GameObject regularRoutine;

	private RoutineDescriptor alarmRoutine;

	private AlarmPanel ap;

	private CharacterPropertyModifier cpm;

	private ActorWrapper aw;

	private bool goingForAlarm;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		if (actor == null)
		{
			actor = new ActorOverride();
		}
		if (actor.Actor == null)
		{
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				aw = parent.gameObject.GetComponentInChildren<ActorWrapper>();
				if (aw != null)
				{
					actor.Actor = parent.gameObject;
				}
			}
		}
		else if (aw == null)
		{
			aw = actor.Actor.GetComponentInChildren<ActorWrapper>();
		}
		if (alarm != null && ap == null)
		{
			ap = alarm.GetComponentInChildren<AlarmPanel>();
		}
		alarmRoutine = GetComponentInChildren<RoutineDescriptor>();
		alarmRoutine.m_Interface.NoneCombatAI = true;
		CharacterPropertyOverride componentInChildren = GetComponentInChildren<CharacterPropertyOverride>();
		if (componentInChildren != null)
		{
			componentInChildren.m_OverrideData.ActorsToModify.Clear();
			componentInChildren.m_OverrideData.ActorsToModify.Add(actor.Actor);
			Container componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<Container>();
			if (componentInChildren2 != null)
			{
				componentInChildren.ApplyOverride(componentInChildren2);
			}
		}
		if (cpm == null)
		{
			cpm = GetComponentInChildren<CharacterPropertyModifier>();
		}
		if (alarmRoutine != null)
		{
			MoveToDescriptor componentInChildren3 = GetComponentInChildren<MoveToDescriptor>();
			if (componentInChildren3 != null && ap != null)
			{
				if (ap != null)
				{
					componentInChildren3.Parameters.mFinalLookAt = ap.transform.position;
				}
				Transform[] componentsInChildren = ap.gameObject.GetComponentsInChildren<Transform>();
				if (componentsInChildren != null)
				{
					Transform[] array = componentsInChildren;
					foreach (Transform transform in array)
					{
						if (transform.gameObject.name == "Destination")
						{
							componentInChildren3.Target = transform;
							break;
						}
					}
				}
			}
		}
		SendMessageDescriptor[] componentsInChildren2 = GetComponentsInChildren<SendMessageDescriptor>();
		if (componentsInChildren2 != null && ap != null && componentsInChildren2.Length > 0)
		{
			SendMessageDescriptor[] array2 = componentsInChildren2;
			foreach (SendMessageDescriptor sendMessageDescriptor in array2)
			{
				if (sendMessageDescriptor.FunctionToCall == "Activate")
				{
					sendMessageDescriptor.ObjectToMessage = ap.gameObject;
				}
				else if (sendMessageDescriptor.FunctionToCall == "Deactivate")
				{
					sendMessageDescriptor.ObjectToMessage = base.gameObject;
				}
			}
		}
		if (actor == null || !(actor.Actor != null))
		{
			return;
		}
		EventOnAlerted componentInChildren4 = actor.Actor.GetComponentInChildren<EventOnAlerted>();
		if (componentInChildren4 == null)
		{
			EventsCreator componentInChildren5 = actor.Actor.GetComponentInChildren<EventsCreator>();
			if (componentInChildren5 != null)
			{
				componentInChildren5.AddItem(13);
				componentInChildren4 = actor.Actor.GetComponentInChildren<EventOnAlerted>();
				if (componentInChildren4 != null)
				{
					if (componentInChildren4.ObjectsToCall == null)
					{
						componentInChildren4.ObjectsToCall = new List<GameObject>();
					}
					if (!componentInChildren4.ObjectsToCall.Contains(base.gameObject))
					{
						componentInChildren4.ObjectsToCall.Add(base.gameObject);
					}
					if (componentInChildren4.FunctionsToCall == null)
					{
						componentInChildren4.FunctionsToCall = new List<string>();
					}
					if (!componentInChildren4.FunctionsToCall.Contains("Activate"))
					{
						componentInChildren4.FunctionsToCall.Add("Activate");
					}
				}
			}
		}
		else
		{
			if (componentInChildren4.ObjectsToCall == null)
			{
				componentInChildren4.ObjectsToCall = new List<GameObject>();
			}
			if (!componentInChildren4.ObjectsToCall.Contains(base.gameObject))
			{
				componentInChildren4.ObjectsToCall.Add(base.gameObject);
			}
			if (componentInChildren4.FunctionsToCall == null)
			{
				componentInChildren4.FunctionsToCall = new List<string>();
			}
			if (!componentInChildren4.FunctionsToCall.Contains("Activate"))
			{
				componentInChildren4.FunctionsToCall.Add("Activate");
			}
		}
		if (regularRoutine == null)
		{
			EnemyOverride component = actor.Actor.GetComponent<EnemyOverride>();
			if (component != null && component.m_RoutineOverrideData.TaskListObject != null)
			{
				regularRoutine = component.m_RoutineOverrideData.TaskListObject;
			}
		}
	}

	public void Activate()
	{
		if (aw != null && ap != null)
		{
			Actor actor = aw.GetActor();
			if (actor != null && !actor.health.IsMortallyWounded() && actor.health.Health > 0f && !ap.CanBeTurnedOff() && cpm != null)
			{
				cpm.SwitchRoutine(alarmRoutine.gameObject);
				goingForAlarm = true;
			}
		}
	}

	public void Deactivate()
	{
		if (cpm != null)
		{
			if (regularRoutine != null)
			{
				cpm.SwitchRoutine(regularRoutine);
			}
			else
			{
				cpm.ClearExistingTasks();
			}
		}
	}

	public bool IsGoingForAlarm()
	{
		return goingForAlarm;
	}

	public void SetCPM(CharacterPropertyModifier cpm)
	{
		this.cpm = cpm;
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SendMessage(methodName);
	}
}
