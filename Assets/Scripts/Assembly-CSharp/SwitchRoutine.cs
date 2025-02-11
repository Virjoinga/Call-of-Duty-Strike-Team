using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRoutine : MonoBehaviour
{
	public SwitchRoutineData m_Interface;

	[HideInInspector]
	public List<ActorWrapper> Actors;

	[HideInInspector]
	public List<ActorWrapper> TriggerActors;

	public List<TaskDescriptor> FirstRoutines;

	public List<TaskDescriptor> SecondRoutines;

	private bool bDoingSecondaryRoutine;

	private int ActorsInTrigger;

	private bool bTriggered;

	private void Start()
	{
		ActorsInTrigger = 0;
		bDoingSecondaryRoutine = false;
	}

	private void Update()
	{
	}

	private IEnumerator RoutineTimer(bool bPrimary)
	{
		if (!bPrimary)
		{
			yield return new WaitForSeconds(m_Interface.SecondRoutineTimeOut);
			Debug.Log("Switching to primary routine on Timer.....");
			SwitchAllRoutines();
		}
		else
		{
			yield return new WaitForSeconds(m_Interface.FirstRoutineTimeOut);
			Debug.Log("Switching to Secondary routine on Timer.....");
			SwitchAllRoutines();
		}
	}

	public void Activate()
	{
		SwitchAllRoutines();
	}

	public void Deactivate()
	{
	}

	public void OnTriggerEnter(Collider other)
	{
		if (m_Interface.SwitchOnTriggerVolume && (m_Interface.IgnoreActorOfInterestCheck || IsUnitOfInterest(other.gameObject)))
		{
			if (ActorsInTrigger < Actors.Count)
			{
				ActorsInTrigger++;
			}
			Debug.Log("Actor Entered " + ActorsInTrigger);
			if ((!m_Interface.AllActorsRequiredInVolume || ActorsInTrigger == Actors.Count) && (!m_Interface.OneShot || !bTriggered))
			{
				SwitchAllRoutines();
				bTriggered = true;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (m_Interface.SwitchOnTriggerVolume && IsUnitOfInterest(other.gameObject))
		{
			if (ActorsInTrigger > 0)
			{
				ActorsInTrigger--;
			}
			Debug.Log("Actor left " + ActorsInTrigger);
		}
	}

	private void SwitchAllRoutines()
	{
		int num = 0;
		foreach (ActorWrapper actor2 in Actors)
		{
			Actor actor = actor2.GetActor();
			actor.tasks.CancelAllTasks();
			if (bDoingSecondaryRoutine)
			{
				FirstRoutines[num].CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType);
				Debug.Log("Setting primary routine for Actor" + num);
			}
			else
			{
				Debug.Log("Setting Secondary routine for Actor" + num);
				SecondRoutines[num].CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType);
			}
			num++;
		}
		bDoingSecondaryRoutine = !bDoingSecondaryRoutine;
		if (bDoingSecondaryRoutine && m_Interface.SecondRoutineTimeOut != 0f)
		{
			StartCoroutine(RoutineTimer(false));
			Debug.Log("SETUP A TIMER ON SECONDARY ROUTINE");
		}
		else if (!bDoingSecondaryRoutine && m_Interface.FirstRoutineTimeOut != 0f)
		{
			StartCoroutine(RoutineTimer(true));
			Debug.Log("SETUP A TIMER ON PRIMARY ROUTINE");
		}
		ActorsInTrigger = 0;
	}

	private bool IsUnitOfInterest(GameObject unitToCheck)
	{
		if (TriggerActors.Count > 0)
		{
			foreach (ActorWrapper triggerActor in TriggerActors)
			{
				if (triggerActor != null && triggerActor.GetGameObject() == unitToCheck)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (ActorWrapper actor in Actors)
			{
				if (actor != null && actor.GetGameObject() == unitToCheck)
				{
					return true;
				}
			}
		}
		return false;
	}
}
