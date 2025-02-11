using UnityEngine;

public class TetherDescriptor : TaskDescriptor
{
	public Transform TetherLocation;

	public float Range;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		Actor component = owner.GetComponent<Actor>();
		Vector3 vector = ((!(TetherLocation != null)) ? component.GetPosition() : TetherLocation.position);
		if (component.tether.Active && (component.tether.Position - vector).magnitude < 0.1f)
		{
			Debug.Log("WARNING - TetherDescriptor trying to tether owner to the same (or nearly same) position");
			return null;
		}
		component.tether.SetPositionAndRangeAndActivate(vector, Range);
		return null;
	}
}
