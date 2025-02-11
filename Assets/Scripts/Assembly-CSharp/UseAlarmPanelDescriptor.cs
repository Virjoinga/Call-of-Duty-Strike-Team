public class UseAlarmPanelDescriptor : TaskDescriptor
{
	public GuidRef alarmObject;

	public override void ResolveGuidLinks()
	{
		alarmObject.ResolveLink();
	}

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskUseAlarmPanel>();
			return null;
		}
		if (alarmObject.theObject == null)
		{
			return null;
		}
		AlarmPanel componentInChildren = alarmObject.theObject.GetComponentInChildren<AlarmPanel>();
		if (componentInChildren == null)
		{
			return null;
		}
		return new TaskUseAlarmPanel(owner, priority, flags | ConfigFlags.GetAsFlags(), componentInChildren);
	}
}
