using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCreator : MonoBehaviour
{
	protected int m_HighlightedTaskIndex = -1;

	public virtual Type[] GetTypes()
	{
		return null;
	}

	public virtual List<Command> GetCommands()
	{
		return null;
	}

	public virtual List<TaskDescriptor> GetDescriptors()
	{
		return null;
	}

	public virtual List<EventDescriptor> GetEvents()
	{
		return null;
	}

	public virtual List<Condition> GetConditions()
	{
		return null;
	}

	public virtual void PopulateItems()
	{
	}

	public virtual void SetItemVisibility()
	{
	}

	public virtual void AddItem(int commandTypeIndex)
	{
	}

	public virtual void RemoveItem(Command commandToRemove)
	{
	}

	public virtual void RemoveItem(TaskDescriptor taskToRemove)
	{
	}

	public virtual void RemoveItem(Condition conditionToRemove)
	{
	}

	public virtual void RemoveItem(EventDescriptor eventToRemove)
	{
	}

	public virtual void SwapItems(int Index1, int Index2)
	{
	}

	public virtual void SetCurrentItem(int currentCommandIndex)
	{
	}

	public virtual Command GetCurrentCommand()
	{
		return null;
	}

	public virtual TaskDescriptor GetCurrentDescriptor()
	{
		return null;
	}

	public virtual EventDescriptor GetCurrentEvent()
	{
		return null;
	}

	public virtual Condition GetCurrentCondition()
	{
		return null;
	}

	public virtual int GetCurrentHighlightedIndex()
	{
		return m_HighlightedTaskIndex;
	}
}
