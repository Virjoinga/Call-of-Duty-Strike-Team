using System.Collections.Generic;
using UnityEngine;

public class LinkedAIController : MonoBehaviour
{
	private const string ROUTINE_PARENT = "Custom Routines";

	private const string CONDITIONAL_ROUTINE_PARENT = "Conditional Routines";

	private const string LOGIC_PARENT = "Override Logics";

	private const string TRIGGER_PARENT = "Triggers";

	public List<GameObject> customRoutines;

	public CharacterPropertyOverride characterPropertyModifier;

	public GameObject eventListener;

	public List<OverrideLogicOverride> overrideLogic;

	public List<ConditionalDescriptor> conditionalRoutines;

	public List<TriggerVolume> triggerVolumes;

	public List<GameObject> genericTriggers;

	public GameObject tetherPoint;

	public GameObject coverCluster;

	public GameObject magnet;

	private EnemyOverride AI;

	private GameObject AIObj;

	private GameObject AIParnetObj;

	private ContainerOverride contOver;

	public void FindActor()
	{
		if (AI == null)
		{
			AI = GetComponent<EnemyOverride>();
			if (AI == null)
			{
				AI = base.transform.parent.gameObject.GetComponentInChildren<EnemyOverride>();
			}
			if (AI != null)
			{
				AIObj = AI.gameObject;
				AIParnetObj = AI.transform.parent.gameObject;
			}
		}
	}

	public ContainerOverride GetContainerOverride()
	{
		if (contOver == null)
		{
			contOver = GetComponent<ContainerOverride>();
		}
		if (contOver != null)
		{
			return contOver;
		}
		return null;
	}

	public EnemyOverride GetAI()
	{
		return AI;
	}

	public void CreateEmptyRoutine(GameObject newRD)
	{
		GameObject gameObject = null;
		bool flag = false;
		if (customRoutines == null || customRoutines.Count == 0)
		{
			customRoutines = new List<GameObject>();
			gameObject = new GameObject("Custom Routines");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			flag = true;
		}
		else
		{
			gameObject = customRoutines[0].transform.parent.gameObject;
			flag = false;
		}
		newRD.transform.parent = gameObject.transform;
		newRD.transform.localPosition = Vector3.zero;
		customRoutines.Add(newRD);
		if (flag)
		{
			AI.m_RoutineOverrideData.TaskListObject = newRD;
		}
		flag = false;
	}

	public void CreateCPM(GameObject newCPM)
	{
		newCPM.transform.parent = base.transform;
		newCPM.transform.localPosition = Vector3.zero;
		characterPropertyModifier = newCPM.GetComponent<CharacterPropertyOverride>();
		characterPropertyModifier.m_OverrideData.ActorsToModify = new List<GameObject>();
		characterPropertyModifier.m_OverrideData.ActorsToModify.Add(AIObj);
	}

	public void CreateEventsListener(GameObject newEL)
	{
		newEL.transform.parent = base.transform;
		newEL.transform.localPosition = Vector3.zero;
		eventListener = newEL;
		AI.m_SpawnerOverrideData.EventsList = eventListener;
	}

	public void CreateOverrideLogic(GameObject newOL)
	{
		GameObject gameObject = null;
		if (overrideLogic == null || overrideLogic.Count == 0)
		{
			overrideLogic = new List<OverrideLogicOverride>();
			gameObject = new GameObject("Override Logics");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject = overrideLogic[0].gameObject.transform.parent.gameObject;
		}
		newOL.transform.parent = gameObject.transform;
		newOL.transform.localPosition = Vector3.zero;
		overrideLogic.Add(newOL.GetComponent<OverrideLogicOverride>());
		overrideLogic[overrideLogic.Count - 1].m_OverrideData.Actors = new List<ActorOverride>();
		overrideLogic[overrideLogic.Count - 1].m_OverrideData.Actors.Add(new ActorOverride());
		overrideLogic[overrideLogic.Count - 1].m_OverrideData.Actors[0].Actor = AIObj;
	}

	public void CreateConditionalRoutine(GameObject newCCR, GameObject rd)
	{
		GameObject gameObject = null;
		if (conditionalRoutines == null || conditionalRoutines.Count == 0)
		{
			conditionalRoutines = new List<ConditionalDescriptor>();
			gameObject = new GameObject("Conditional Routines");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject = conditionalRoutines[0].gameObject.transform.parent.gameObject;
		}
		newCCR.transform.parent = gameObject.transform;
		newCCR.transform.localPosition = Vector3.zero;
		conditionalRoutines.Add(newCCR.GetComponent<ConditionalDescriptor>());
		conditionalRoutines[conditionalRoutines.Count - 1].Task = rd;
	}

	public void CreateAlarmPanelOverride(GameObject newAP, GameObject alarmPanel)
	{
		GameObject gameObject = null;
		if (customRoutines == null || customRoutines.Count == 0)
		{
			customRoutines = new List<GameObject>();
			gameObject = new GameObject("Custom Routines");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject = customRoutines[0].transform.parent.gameObject;
		}
		newAP.transform.parent = gameObject.transform;
		newAP.transform.localPosition = Vector3.zero;
		AlarmOverrideLogicOverride component = newAP.GetComponent<AlarmOverrideLogicOverride>();
		if (component != null)
		{
			component.SetCPM(characterPropertyModifier.GetComponentInChildren<CharacterPropertyModifier>());
			component.actor = new ActorOverride();
			component.actor.Actor = AIObj;
			component.alarm = alarmPanel;
		}
		ContainerOverride containerOverride = GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		Container component2 = newAP.GetComponent<Container>();
		containerOverride = newAP.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		containerOverride.ApplyOverride(component2);
		customRoutines.Add(newAP);
	}

	public void CreateTrigger(GameObject newGT, string functionToCall, GameObject optionalObjParam, bool oneShot)
	{
		GameObject gameObject = null;
		if (genericTriggers == null || genericTriggers.Count == 0)
		{
			genericTriggers = new List<GameObject>();
			gameObject = new GameObject("Triggers");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject = genericTriggers[0].transform.parent.gameObject;
		}
		newGT.transform.parent = gameObject.transform;
		newGT.transform.localPosition = Vector3.zero;
		GenericTriggerOverride component = newGT.GetComponent<GenericTriggerOverride>();
		if (component.m_OverrideData.Actors == null)
		{
			component.m_OverrideData.Actors = new List<GameObject>();
		}
		component.m_OverrideData.Actors.Add(AIObj);
		if (characterPropertyModifier != null)
		{
			component.m_OverrideData.ObjectToCall = characterPropertyModifier.gameObject;
		}
		component.m_OverrideData.FunctionToCall = functionToCall;
		component.m_OverrideData.OptionalObjectParam = optionalObjParam;
		component.m_OverrideData.OneShot = oneShot;
		genericTriggers.Add(newGT);
	}

	public void CreateCoverCluster(GameObject newCC)
	{
		newCC.transform.parent = base.transform;
		newCC.transform.localPosition = Vector3.zero;
		coverCluster = newCC;
		if (AI.m_SpawnerOverrideData.quickDestination.coverCluster == null)
		{
			AI.m_SpawnerOverrideData.quickDestination.coverCluster = new GuidRef();
			AI.m_SpawnerOverrideData.quickDestination.coverCluster.theObject = newCC;
		}
		else if (AI.m_SpawnerOverrideData.quickDestination.coverCluster.theObject == null)
		{
			AI.m_SpawnerOverrideData.quickDestination.coverCluster.theObject = newCC;
		}
	}

	public void CreateTetherPoint(GameObject newTP)
	{
		newTP.transform.parent = base.transform;
		newTP.transform.localPosition = Vector3.zero;
		tetherPoint = newTP;
		if (AI.m_SpawnerOverrideData.TetherPoint == null)
		{
			AI.m_SpawnerOverrideData.TetherPoint = newTP;
		}
	}

	public void CreateRoutine(int type, int numberOfDestinations, List<int> secondsToWait, List<AnimationClip> animationClips, bool createStareAt)
	{
		RoutineCreator component = customRoutines[customRoutines.Count - 1].GetComponent<RoutineCreator>();
		if (component == null)
		{
			return;
		}
		component.PopulateItems();
		switch (type)
		{
		case 1:
		{
			for (int j = 0; j < numberOfDestinations; j++)
			{
				string text2 = (j + 1).ToString("00");
				CreateMoveToDescriptor(component, text2);
				CreateWaitDescriptor(component, secondsToWait[j], createStareAt, "LAP" + text2);
			}
			break;
		}
		case 2:
		{
			for (int k = 0; k < numberOfDestinations; k++)
			{
				string text3 = (k + 1).ToString("00");
				CreateMoveToDescriptor(component, text3);
				CreateAnimationDescriptor(component, animationClips[k]);
			}
			break;
		}
		case 3:
		{
			for (int i = 0; i < numberOfDestinations; i++)
			{
				string text = (i + 1).ToString("00");
				CreateMoveToDescriptor(component, text);
				CreateAnimationDescriptor(component, animationClips[i]);
				CreateWaitDescriptor(component, secondsToWait[i], createStareAt, "LAP" + text);
			}
			break;
		}
		}
	}

	private void CreateMoveToDescriptor(RoutineCreator rc, string name)
	{
		rc.AddItem(8);
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = rc.transform;
		gameObject.transform.localPosition = Vector3.zero;
		MoveToDescriptor moveToDescriptor = (MoveToDescriptor)rc.GetCurrentDescriptor();
		moveToDescriptor.Target = gameObject.transform;
	}

	private void CreateWaitDescriptor(RoutineCreator rc, int waitTime, bool createLAP, string name)
	{
		rc.AddItem(20);
		WaitDescriptor waitDescriptor = (WaitDescriptor)rc.GetCurrentDescriptor();
		waitDescriptor.Seconds = waitTime;
		if (createLAP)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = rc.transform;
			gameObject.transform.localPosition = Vector3.zero;
			waitDescriptor.StareAt = gameObject.transform;
		}
	}

	private void CreateAnimationDescriptor(RoutineCreator rc, AnimationClip clip)
	{
		rc.AddItem(9);
		PlayAnimationDescriptor playAnimationDescriptor = (PlayAnimationDescriptor)rc.GetCurrentDescriptor();
		playAnimationDescriptor.Clip = clip;
	}

	public void RetrieveExistingData()
	{
		Transform[] componentsInChildren = AIParnetObj.GetComponentsInChildren<Transform>();
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		GameObject gameObject3 = null;
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.gameObject.name.Equals("Custom Routines"))
			{
				gameObject = transform.gameObject;
			}
			else if (transform.gameObject.name.Equals("Override Logics"))
			{
				gameObject2 = transform.gameObject;
			}
			else if (transform.gameObject.name.Equals("Conditional Routines"))
			{
				gameObject3 = transform.gameObject;
			}
		}
		RoutineDescriptor[] componentsInChildren2 = AIParnetObj.GetComponentsInChildren<RoutineDescriptor>();
		AlarmOverrideLogicOverride componentInChildren = AIParnetObj.GetComponentInChildren<AlarmOverrideLogicOverride>();
		GameObject gameObject4 = null;
		if (componentInChildren != null)
		{
			gameObject4 = componentInChildren.gameObject;
		}
		if (componentsInChildren2.Length > 0 || gameObject4 != null)
		{
			if (customRoutines == null)
			{
				customRoutines = new List<GameObject>();
			}
			else
			{
				customRoutines.Clear();
			}
		}
		else if (customRoutines != null)
		{
			customRoutines = null;
		}
		RoutineDescriptor[] array2 = componentsInChildren2;
		foreach (RoutineDescriptor routineDescriptor in array2)
		{
			if (routineDescriptor.transform.parent.name.Equals("Spawner") || routineDescriptor.transform.parent.name.Equals("AlarmOverrideLogic"))
			{
				continue;
			}
			if (routineDescriptor.transform.parent.name != "Custom Routines")
			{
				if (gameObject == null)
				{
					gameObject = new GameObject("Custom Routines");
					gameObject.transform.parent = base.transform;
					gameObject.transform.localPosition = Vector3.zero;
				}
				routineDescriptor.transform.parent = gameObject.transform;
			}
			if (routineDescriptor.Tasks != null)
			{
				foreach (TaskDescriptor task in routineDescriptor.Tasks)
				{
					MoveToDescriptor moveToDescriptor = task as MoveToDescriptor;
					if (moveToDescriptor != null && moveToDescriptor.Target != null && moveToDescriptor.Target.IsChildOf(AIParnetObj.transform))
					{
						moveToDescriptor.Target.parent = routineDescriptor.transform;
					}
					WaitDescriptor waitDescriptor = task as WaitDescriptor;
					if (waitDescriptor != null && waitDescriptor.StareAt != null && waitDescriptor.StareAt.IsChildOf(AIParnetObj.transform))
					{
						waitDescriptor.StareAt.parent = routineDescriptor.transform;
					}
				}
			}
			customRoutines.Add(routineDescriptor.gameObject);
		}
		if (gameObject4 != null)
		{
			gameObject4.transform.parent = gameObject.transform;
			customRoutines.Add(gameObject4);
		}
		characterPropertyModifier = AIParnetObj.GetComponentInChildren<CharacterPropertyOverride>();
		EventsCreator componentInChildren2 = AIParnetObj.GetComponentInChildren<EventsCreator>();
		if (componentInChildren2 != null)
		{
			eventListener = componentInChildren2.gameObject;
		}
		OverrideLogicOverride[] componentsInChildren3 = AIParnetObj.GetComponentsInChildren<OverrideLogicOverride>();
		if (componentsInChildren3.Length > 0)
		{
			if (overrideLogic == null)
			{
				overrideLogic = new List<OverrideLogicOverride>();
			}
			else
			{
				overrideLogic.Clear();
			}
		}
		else if (overrideLogic != null)
		{
			overrideLogic = null;
		}
		OverrideLogicOverride[] array3 = componentsInChildren3;
		foreach (OverrideLogicOverride overrideLogicOverride in array3)
		{
			if (overrideLogicOverride.transform.parent.name != "Override Logics")
			{
				if (gameObject2 == null)
				{
					gameObject2 = new GameObject("Override Logics");
					gameObject2.transform.parent = base.transform;
					gameObject2.transform.localPosition = Vector3.zero;
				}
				overrideLogicOverride.transform.parent = gameObject2.transform;
			}
			overrideLogic.Add(overrideLogicOverride);
		}
		ConditionalDescriptor[] componentsInChildren4 = AIParnetObj.GetComponentsInChildren<ConditionalDescriptor>();
		if (componentsInChildren4.Length > 0)
		{
			if (conditionalRoutines == null)
			{
				conditionalRoutines = new List<ConditionalDescriptor>();
			}
			else
			{
				conditionalRoutines.Clear();
			}
		}
		else if (conditionalRoutines != null)
		{
			conditionalRoutines = null;
		}
		ConditionalDescriptor[] array4 = componentsInChildren4;
		foreach (ConditionalDescriptor conditionalDescriptor in array4)
		{
			if (conditionalDescriptor.transform.parent.name != "Conditional Routines")
			{
				if (gameObject3 == null)
				{
					gameObject3 = new GameObject("Conditional Routines");
					gameObject3.transform.parent = base.transform;
					gameObject3.transform.localPosition = Vector3.zero;
				}
				conditionalDescriptor.transform.parent = gameObject3.transform;
			}
			conditionalRoutines.Add(conditionalDescriptor);
		}
		coverCluster = AI.m_SpawnerOverrideData.quickDestination.coverCluster.theObject;
		tetherPoint = AI.m_SpawnerOverrideData.TetherPoint;
		magnet = AI.m_SpawnerOverrideData.quickDestination.magnet.theObject;
	}

	public void CleanUp()
	{
		List<OverrideLogicOverride> list = new List<OverrideLogicOverride>();
		if (overrideLogic != null && overrideLogic.Count > 0)
		{
			foreach (OverrideLogicOverride item in overrideLogic)
			{
				if (item == null)
				{
					list.Add(item);
				}
			}
			foreach (OverrideLogicOverride item2 in list)
			{
				overrideLogic.Remove(item2);
			}
			if (overrideLogic.Count == 0)
			{
				overrideLogic = null;
				SearchAndDestroy(base.gameObject, "Override Logics");
			}
		}
		else
		{
			overrideLogic = null;
		}
		list = null;
		if (customRoutines != null && customRoutines.Count > 0)
		{
			List<GameObject> list2 = new List<GameObject>();
			foreach (GameObject customRoutine in customRoutines)
			{
				if (customRoutine == null)
				{
					list2.Add(customRoutine);
				}
			}
			foreach (GameObject item3 in list2)
			{
				customRoutines.Remove(item3);
			}
			if (customRoutines.Count == 0)
			{
				customRoutines = null;
				SearchAndDestroy(base.gameObject, "Custom Routines");
			}
			list2 = null;
		}
		else
		{
			customRoutines = null;
		}
		if (conditionalRoutines != null && conditionalRoutines.Count > 0)
		{
			List<ConditionalDescriptor> list3 = new List<ConditionalDescriptor>();
			foreach (ConditionalDescriptor conditionalRoutine in conditionalRoutines)
			{
				if (conditionalRoutine == null)
				{
					list3.Add(conditionalRoutine);
				}
			}
			foreach (ConditionalDescriptor item4 in list3)
			{
				conditionalRoutines.Remove(item4);
			}
			if (conditionalRoutines.Count == 0)
			{
				conditionalRoutines = null;
				SearchAndDestroy(base.gameObject, "Conditional Routines");
			}
			list3 = null;
		}
		else
		{
			conditionalRoutines = null;
		}
		if (genericTriggers != null && genericTriggers.Count > 0)
		{
			List<GameObject> list4 = new List<GameObject>();
			foreach (GameObject genericTrigger in genericTriggers)
			{
				if (genericTrigger == null)
				{
					list4.Add(genericTrigger);
				}
			}
			foreach (GameObject item5 in list4)
			{
				genericTriggers.Remove(item5);
			}
			if (genericTriggers.Count == 0)
			{
				genericTriggers = null;
				SearchAndDestroy(base.gameObject, "Triggers");
			}
			list4 = null;
		}
		else
		{
			genericTriggers = null;
		}
	}

	private void SearchAndDestroy(GameObject gameObject, string name)
	{
		Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.childCount == 0 && transform.name.Equals(name))
			{
				Object.DestroyImmediate(transform.gameObject);
				break;
			}
		}
	}
}
