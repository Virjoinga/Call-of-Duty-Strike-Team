using UnityEngine;

public class Interaction
{
	public GameObject GameObj;

	public Task GameTask;

	public ActionComponent ActionComp;

	public bool bSkippable = true;

	public Interaction(GameObject GO, Task task, ActionComponent actComp)
	{
		GameObj = GO;
		GameTask = task;
		ActionComp = actComp;
	}
}
