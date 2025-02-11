using UnityEngine;

public class AssaultTester : MonoBehaviour
{
	public GameObject assaultObj;

	public float OffenceModifier = 1f;

	public float DefenceModifier = 1f;

	public float ExposureModifier = 1f;

	public float GoalModifier = 1f;

	public bool DoNextThink;

	public bool DoTenThinks;

	public TaskAssault task;

	private void Start()
	{
	}

	private void Update()
	{
		Actor component = GetComponent<Actor>();
		if (assaultObj != null)
		{
			InheritableMovementParams moveParams = new InheritableMovementParams();
			AssaultParams assaultParams = new AssaultParams();
			assaultParams.Target.theObject = assaultObj;
			new TaskAssault(component.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType, moveParams, assaultParams);
			assaultObj = null;
		}
		if (task == null)
		{
			task = component.tasks.GetRunningTask<TaskAssault>();
		}
		if (task != null)
		{
			task.mAssaultParams.DefenceModifier = DefenceModifier;
			task.mAssaultParams.OffenceModifier = OffenceModifier;
			task.mAssaultParams.ExposureModifier = ExposureModifier;
			task.mAssaultParams.GoalModifier = GoalModifier;
			if (DoNextThink)
			{
				task.AddThinkSteps(1);
				DoNextThink = false;
			}
			if (DoTenThinks)
			{
				task.AddThinkSteps(10);
				DoTenThinks = false;
			}
		}
	}
}
