using UnityEngine;

public abstract class TaskDescriptor : MonoBehaviour
{
	public bool CancelTasksOfType;

	public TaskConfigDescriptor ConfigFlags = new TaskConfigDescriptor();

	public abstract Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags);

	public virtual void ResolveGuidLinks()
	{
	}
}
