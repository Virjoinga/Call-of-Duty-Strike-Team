public class MoveToCoverDescriptor : TaskDescriptor
{
	public GuidRef Target = new GuidRef();

	public InheritableMovementParams Parameters = new InheritableMovementParams();

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskMoveToCover>();
			return null;
		}
		if (ConfigFlags == null)
		{
			return null;
		}
		Task task = null;
		Actor component = owner.GetComponent<Actor>();
		if (Parameters.coverCluster != null)
		{
			component.awareness.coverCluster = Parameters.coverCluster;
		}
		CoverPointCore coverPointCore = null;
		if (Target.theObject != null)
		{
			Parameters.mDestination = GeneralArea.GetLocation(Target.theObject);
			NewCoverPoint component2 = Target.theObject.GetComponent<NewCoverPoint>();
			if (component2 != null && component2.index < NewCoverPointManager.Instance().coverPoints.Length)
			{
				flags |= Task.Config.ClearAllCurrentType;
				task = new TaskMoveToCover(owner, priority, flags | ConfigFlags.GetAsFlags(), NewCoverPointManager.Instance().coverPoints[component2.index], Parameters.Clone());
			}
			CoverCluster component3 = Target.theObject.GetComponent<CoverCluster>();
			if (component3 != null && component != null)
			{
				coverPointCore = component3.RandomValidCover(component);
				if (coverPointCore != null)
				{
					flags |= Task.Config.ClearAllCurrentType;
					task = new TaskMoveToCover(owner, priority, flags | ConfigFlags.GetAsFlags(), coverPointCore, Parameters.Clone());
				}
			}
		}
		if (task == null)
		{
			coverPointCore = component.awareness.GetValidCoverNearestSpecifiedPosition(Parameters.mDestination, 25f, 0f, true, 0f);
			if (coverPointCore != null)
			{
				flags |= Task.Config.ClearAllCurrentType;
				task = new TaskMoveToCover(owner, priority, flags | ConfigFlags.GetAsFlags(), coverPointCore, Parameters.Clone());
			}
			else
			{
				task = new TaskRouteTo(owner, priority, flags | ConfigFlags.GetAsFlags(), Parameters.Clone());
			}
		}
		if (component != null && component.behaviour != null && component.behaviour.PlayerControlled)
		{
			OrdersHelper.UpdatePlayerUnitTetherPoint(Parameters.mDestination, component);
		}
		GameController.Instance.StartCoroutine(GameController.Instance.WaitForEnterCover(component));
		return task;
	}

	public override void ResolveGuidLinks()
	{
		Target.ResolveLink();
		Parameters.ResolveGuidLinks();
	}
}
