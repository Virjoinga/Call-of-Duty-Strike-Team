using System.Collections.Generic;

public class TaskSynchroniser
{
	public enum SyncState
	{
		Undefined = 0,
		InPosition = 1,
		MeleeAttack = 2,
		WeaponsFree = 3,
		Done = 4
	}

	private class SynchedTaskData
	{
		public SyncState state;
	}

	private Dictionary<Task, SynchedTaskData> synchedTasks;

	public TaskSynchroniser(bool enabled)
	{
		if (enabled)
		{
			synchedTasks = new Dictionary<Task, SynchedTaskData>();
		}
	}

	public void AddTask(Task t)
	{
		if (synchedTasks != null)
		{
			synchedTasks.Add(t, new SynchedTaskData());
		}
	}

	public void TaskFinished(Task t)
	{
		if (synchedTasks != null)
		{
			synchedTasks.Remove(t);
		}
	}

	public bool WaitForSync(Task t, SyncState s)
	{
		if (synchedTasks == null)
		{
			return false;
		}
		SynchedTaskData value;
		if (synchedTasks.TryGetValue(t, out value))
		{
			value.state = s;
			if (s == SyncState.Done)
			{
				return false;
			}
			foreach (KeyValuePair<Task, SynchedTaskData> synchedTask in synchedTasks)
			{
				if (synchedTask.Value.state < s)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanContinue(Task t, SyncState s)
	{
		if (synchedTasks == null)
		{
			return true;
		}
		foreach (KeyValuePair<Task, SynchedTaskData> synchedTask in synchedTasks)
		{
			if (synchedTask.Value.state < s)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanContinue(Task t)
	{
		if (synchedTasks == null)
		{
			return true;
		}
		SynchedTaskData value;
		if (synchedTasks.TryGetValue(t, out value))
		{
			if (value.state == SyncState.Done)
			{
				return true;
			}
			foreach (KeyValuePair<Task, SynchedTaskData> synchedTask in synchedTasks)
			{
				if (synchedTask.Value.state < value.state)
				{
					return false;
				}
			}
		}
		return true;
	}
}
