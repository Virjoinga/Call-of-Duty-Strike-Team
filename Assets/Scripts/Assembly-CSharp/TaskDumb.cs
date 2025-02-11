public class TaskDumb : Task
{
	public TaskDumb(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
	}

	public override void Update()
	{
		mActor.awareness.canLook = false;
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			component.enabled = false;
		}
		AuditoryAwarenessComponent component2 = base.Owner.GetComponent<AuditoryAwarenessComponent>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
	}

	public override bool HasFinished()
	{
		return false;
	}
}
