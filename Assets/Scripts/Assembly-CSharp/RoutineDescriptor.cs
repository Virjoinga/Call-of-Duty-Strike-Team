using System.Collections.Generic;
using UnityEngine;

public class RoutineDescriptor : TaskDescriptor
{
	public List<TaskDescriptor> Tasks;

	public RoutineDescriptorData m_Interface = new RoutineDescriptorData();

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		return CreateTask(owner, priority, flags, true);
	}

	public Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags, bool ReplaceExistingRoutine)
	{
		if (ReplaceExistingRoutine)
		{
			owner.CancelTasksOfEqualOrHigherPriority(typeof(TaskRoutine), priority);
		}
		if (ConfigFlags == null)
		{
			Debug.LogWarning("ConfigFlags are null! in RoutineDescriptor.CreateTask :" + owner.name);
			return null;
		}
		if (m_Interface == null)
		{
			Debug.LogWarning("m_Interface are null! in RoutineDescriptor.CreateTask :" + owner.name);
			return null;
		}
		bool flag = (priority == TaskManager.Priority.IMMEDIATE && flags == Task.Config.DenyPlayerInput) || m_Interface.OneShotRoutineTasks;
		Actor component = owner.GetComponent<Actor>();
		if (component != null && component.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC && !flag)
		{
			Debug.LogWarning("Trying to put a Routine on an unsupported character type! RoutineDescriptor.CreateTask :" + owner.name);
			return null;
		}
		if (priority == TaskManager.Priority.IMMEDIATE)
		{
			TaskOverrideRoutine taskOverrideRoutine = new TaskOverrideRoutine(owner, priority, flags | ConfigFlags.GetAsFlags(), Tasks, m_Interface.NoneCombatAI);
			LogRoutineDebug(owner, taskOverrideRoutine);
			return taskOverrideRoutine;
		}
		if (m_Interface.AlertedRoutineObject != null)
		{
			flags |= Task.Config.AbortOnAlert;
			RoutineDescriptor component2 = m_Interface.AlertedRoutineObject.GetComponent<RoutineDescriptor>();
			component2.CreateTask(owner, priority, Task.Config.Default);
		}
		TaskRoutine taskRoutine = new TaskRoutine(owner, priority, flags | ConfigFlags.GetAsFlags(), Tasks, m_Interface.NoneCombatAI);
		taskRoutine.Magnet = m_Interface.Magnet;
		if (m_Interface.OneShotRoutineTasks)
		{
			taskRoutine.OneShotRoutineTasks = true;
		}
		if (m_Interface.PingPongRoutineOrdering)
		{
			taskRoutine.PingPongRoutineOrdering = true;
		}
		LogRoutineDebug(owner, taskRoutine);
		return taskRoutine;
	}

	private void LogRoutineDebug(TaskManager owner, Task tr)
	{
	}

	public void CreateOverrideTaskChain(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		for (int num = Tasks.Count - 1; num >= 0; num--)
		{
			TaskDescriptor taskDescriptor = Tasks[num];
			TBFAssert.DoAssert(taskDescriptor as RoutineDescriptor == null, "RoutineDescriptos within RoutineDesciptors are not supported for the purpose of overriding");
			taskDescriptor.CreateTask(owner, priority, flags | ConfigFlags.GetAsFlags());
		}
	}
}
