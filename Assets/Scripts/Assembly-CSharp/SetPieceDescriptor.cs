public class SetPieceDescriptor : TaskDescriptor
{
	public SetPieceLogic SetPiece;

	public bool InPlace;

	public bool PathTo;

	public float Delay;

	public InheritableMovementParams Parameters;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskSetPiece>();
			return null;
		}
		return new TaskSetPiece(owner, priority, flags | ConfigFlags.GetAsFlags(), SetPiece, InPlace, PathTo, false, Parameters.Clone(), Delay);
	}
}
