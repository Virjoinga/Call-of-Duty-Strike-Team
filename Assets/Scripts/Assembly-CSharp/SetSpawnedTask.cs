using System.Collections;
using UnityEngine;

public class SetSpawnedTask : MonoBehaviour
{
	public TaskDescriptor TaskDescriptor;

	private void OnSpawned(GameObject spawned)
	{
		StartCoroutine("SetTask", spawned.GetComponent<Actor>());
	}

	private IEnumerator SetTask(Actor actor)
	{
		yield return new WaitForEndOfFrame();
		if ((bool)actor)
		{
			actor.tasks.CancelTasks<TaskRoutine>();
			TaskDescriptor.CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default);
		}
	}
}
