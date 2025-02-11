using UnityEngine;

public class EntranceGateLogic : MonoBehaviour
{
	public GameObject GateCutscene;

	public GameObject GateObject1;

	public GameObject GateObject2;

	public ActorWrapper GateGuard;

	public GameObject GateGuardInvestigateDesc;

	public GameObject GateGuardAlertDesc;

	public RoutineDescriptor GateRoutineDesc;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StartCutscene()
	{
		Container.SendMessage(GateCutscene, "StartSequence", base.gameObject);
	}

	public void OpenGates()
	{
		Container.SendMessage(GateObject1, "OnEnter", base.gameObject);
		Container.SendMessage(GateObject2, "OnEnter", base.gameObject);
		Container.SendMessage(GateGuardInvestigateDesc, "OnEnter", base.gameObject);
	}

	public void FinishedInvestigate()
	{
		Actor actor = GateGuard.GetActor();
		GateRoutineDesc.CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType);
		GateGuardInvestigateDesc.SetActive(false);
	}

	public void Alerted()
	{
		GateGuardAlertDesc.SendMessage("OnEnter");
		GateGuardInvestigateDesc.SetActive(false);
	}
}
