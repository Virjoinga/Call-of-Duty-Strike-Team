using System.Collections.Generic;
using UnityEngine;

public class TaskDescriptorOverrideHub : MonoBehaviour
{
	public TaskDesriptOverrideHubData m_Interface;

	public List<ActorWrapper> EntitiesToOverride;

	public List<ActorWrapper> ActorsToSelect;

	public List<TaskDescriptor> TasksToAssign;

	private bool mProcessing;

	private void Start()
	{
		mProcessing = true;
		if (!m_Interface.StartOnTrigger)
		{
			DoOverride();
		}
	}

	private void Update()
	{
	}

	public void Activate()
	{
		DoOverride();
	}

	public void Deactivate()
	{
		for (int i = 0; i < EntitiesToOverride.Count; i++)
		{
			Actor actor = EntitiesToOverride[i].GetActor();
			if (!(actor == null) && !actor.realCharacter.IsDead() && !m_Interface.DontEnableControlOnFinished)
			{
				TogglePlayerInteraction(actor, true);
			}
		}
		for (int i = 0; i < ActorsToSelect.Count; i++)
		{
			Actor actor2 = ActorsToSelect[i].GetActor();
			if (!(actor2 == null) && actor2.behaviour.PlayerControlled && actor2.realCharacter.IsSelectable())
			{
				GameplayController.Instance().AddToSelected(actor2);
			}
		}
	}

	public void OnEnter()
	{
		DoOverride();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (m_Interface.StartOnPlayerBoxCollider)
		{
			Actor component = other.gameObject.GetComponent<Actor>();
			if (!(component == null) && component.behaviour.PlayerControlled)
			{
				DoOverride();
			}
		}
	}

	private void TogglePlayerInteraction(Actor actor, bool enabled)
	{
		CharacterPropertyModifier.SetControl(actor, enabled, base.gameObject, false);
	}

	private void DoOverride()
	{
		if (!mProcessing)
		{
			return;
		}
		int num = ((m_Interface.EntityAssignmentLimit != 0) ? m_Interface.EntityAssignmentLimit : EntitiesToOverride.Count);
		int num2 = 0;
		foreach (ActorWrapper item in EntitiesToOverride)
		{
			Actor actor = item.GetActor();
			if (actor == null || num <= 0)
			{
				continue;
			}
			if (m_Interface.ClearCurrentTasks && !actor.realCharacter.IsDead())
			{
				actor.tasks.CancelTasksExcluding<TaskRoutine>();
			}
			TogglePlayerInteraction(actor, false);
			bool flag = false;
			if (num2 < TasksToAssign.Count && !actor.realCharacter.IsDead())
			{
				TaskDescriptor taskDescriptor = TasksToAssign[num2];
				Task task = taskDescriptor.CreateTask(actor.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.DenyPlayerInput);
				TBFAssert.DoAssert(task != null, string.Format("Override Task failure for {0}", actor.realCharacter.name));
				if (m_Interface.BroadcastOnCompletion != null && num2 < m_Interface.BroadcastOnCompletion.Count)
				{
					task.BroadcastOnCompletion = m_Interface.BroadcastOnCompletion[num2];
				}
				else
				{
					task.BroadcastOnCompletion = new GameObjectBroadcaster();
				}
				task.BroadcastOnCompletion.Targets.Add(base.gameObject);
				task.BroadcastOnCompletion.Functions.Add("Deactivate");
				RoutineDescriptor routineDescriptor = taskDescriptor as RoutineDescriptor;
				foreach (TaskDescriptor task2 in routineDescriptor.Tasks)
				{
					if (!flag)
					{
						MoveToDescriptor moveToDescriptor = task2 as MoveToDescriptor;
						if (moveToDescriptor != null)
						{
							List<Actor> list = new List<Actor>();
							list.Add(actor);
							OrdersHelper.UpdatePlayerSquadTetherPoint(moveToDescriptor.Target.position, list);
							flag = true;
						}
					}
				}
			}
			num--;
			num2++;
		}
		if (m_Interface.DestroyOnTrigger)
		{
			mProcessing = false;
		}
	}
}
