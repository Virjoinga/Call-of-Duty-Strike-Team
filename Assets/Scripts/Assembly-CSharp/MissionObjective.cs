using UnityEngine;

public class MissionObjective : MonoBehaviour
{
	public enum ObjectiveState
	{
		InProgress = 0,
		Passed = 1,
		Failed = 2,
		Dormant = 3
	}

	public delegate void OnObjectivePassedEventHandler(object sender);

	public delegate void OnObjectiveFailedEventHandler(object sender);

	public delegate void OnObjectiveInProgressEventHandler(object sender);

	private ObjectiveState mState;

	[HideInInspector]
	public MissionObjective[] EnableObjectivesOnPass;

	[HideInInspector]
	public MissionObjective[] DisableObjectivesOnPass;

	public MissionObjectiveData m_Interface;

	protected bool mMissionPassIfNotFail;

	private bool ToBeEnabledOnActivation;

	private ObjectiveBlip mBlip;

	private GameObject mBlipObject;

	private ObjectiveManager mMyObjectiveManager;

	public ObjectiveManager MyObjectiveManager
	{
		get
		{
			return mMyObjectiveManager;
		}
		set
		{
			mMyObjectiveManager = value;
		}
	}

	public ObjectiveState State
	{
		get
		{
			return mState;
		}
	}

	public bool MissionPassIfNotFail
	{
		get
		{
			return mMissionPassIfNotFail;
		}
	}

	public event OnObjectivePassedEventHandler OnObjectivePassed;

	public event OnObjectiveFailedEventHandler OnObjectiveFailed;

	public event OnObjectiveInProgressEventHandler OnObjectiveInProgress;

	protected void DisableObjective()
	{
		if (State != ObjectiveState.Dormant)
		{
			mState = ObjectiveState.Dormant;
			if (mBlip != null)
			{
				mBlip.SwitchOff();
			}
			LogMessage(" Disabled");
		}
	}

	public void SetBlipColour(Color colour)
	{
		if (mBlip != null)
		{
			mBlip.ColourBlip(colour);
		}
	}

	public void OutputObjectiveDescription()
	{
		if (m_Interface.IsVisible)
		{
			CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get(m_Interface.ObjectiveLabel));
		}
	}

	public void ToggleBlipInfiniteLoop()
	{
		if (mBlip != null)
		{
			mBlip.InfiniteLoop = !mBlip.InfiniteLoop;
		}
	}

	protected void EnableObjective()
	{
		if (State == ObjectiveState.Dormant)
		{
			mState = ObjectiveState.InProgress;
			if (this.OnObjectiveInProgress != null)
			{
				this.OnObjectiveInProgress(this);
			}
			OutputObjectiveDescription();
			if (mBlip != null)
			{
				mBlip.InfiniteLoop = m_Interface.InfiniteBlipFlashing;
				mBlip.AllowHighlight = m_Interface.ShowBlipFlashing;
				mBlip.SwitchOn();
			}
			LogMessage(" Enabled");
		}
	}

	public virtual void HideObjective()
	{
		if (m_Interface.IsVisible)
		{
			m_Interface.IsVisible = false;
		}
	}

	public void Awake()
	{
		GameObject[] gameObjectsToActivate = m_Interface.GameObjectsToActivate;
		foreach (GameObject gameObject in gameObjectsToActivate)
		{
			if (gameObject == null)
			{
				Debug.LogWarning("GameObjectsToActivate has size but no object refs! obj = " + base.gameObject.name);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public virtual void Start()
	{
		if (m_Interface.BlipType != ObjectiveBlip.BlipType.None && (bool)MyObjectiveManager.ObjectiveBlipPrefab)
		{
			GameObject gameObject = Object.Instantiate(MyObjectiveManager.ObjectiveBlipPrefab) as GameObject;
			mBlip = gameObject.GetComponent<ObjectiveBlip>();
			mBlipObject = new GameObject();
			mBlipObject.transform.parent = base.gameObject.transform;
			mBlip.Target = mBlipObject.transform;
			mBlip.SwitchOff();
		}
		ResetState();
	}

	public void MoveToBoundsY(GameObject go)
	{
		MeshFilter meshFilter = go.GetComponentInChildren(typeof(MeshFilter)) as MeshFilter;
		if (!(meshFilter == null))
		{
			Mesh sharedMesh = meshFilter.sharedMesh;
			if (!(sharedMesh == null))
			{
				Vector3 size = sharedMesh.bounds.size;
				Vector3 localScale = meshFilter.gameObject.transform.localScale;
				base.gameObject.transform.position += new Vector3(0f, size.y * localScale.y, 0f);
			}
		}
	}

	public void UpdateBlipTarget()
	{
		if (mBlipObject != null)
		{
			switch (m_Interface.BlipType)
			{
			case ObjectiveBlip.BlipType.Prop:
				mBlipObject.transform.position = base.transform.position + Vector3.up * ObjectiveBlip.BlipYOffset[1];
				break;
			case ObjectiveBlip.BlipType.Character:
				mBlipObject.transform.position = base.transform.position + Vector3.up * ObjectiveBlip.BlipYOffset[2];
				break;
			case ObjectiveBlip.BlipType.Important:
			case ObjectiveBlip.BlipType.Location:
			{
				Vector3 vector = base.transform.position + new Vector3(0f, 1f, 0f);
				RaycastHit hitInfo;
				if (Physics.Raycast(vector, -Vector3.up, out hitInfo, float.PositiveInfinity, 1))
				{
					vector = hitInfo.point + Vector3.up * ObjectiveBlip.BlipYOffset[(int)m_Interface.BlipType];
				}
				mBlipObject.transform.position = vector;
				break;
			}
			default:
				mBlipObject.transform.position = base.transform.position;
				break;
			}
		}
		if (mBlip != null)
		{
			mBlip.Target = mBlipObject.transform;
		}
	}

	public virtual void OnDestroy()
	{
		if (mBlip != null)
		{
			Object.Destroy(mBlip.gameObject);
		}
		LogMessage(" Destroyed");
	}

	private void ResetState()
	{
		if (m_Interface.StartDormant || (SectionManager.GetSectionManager() != null && !SectionManager.GetSectionManager().SectionActivated))
		{
			if (!m_Interface.StartDormant)
			{
				ToBeEnabledOnActivation = true;
			}
			mState = ObjectiveState.Dormant;
			if (mBlip != null)
			{
				mBlip.SwitchOff();
			}
			LogMessage(" Dormant");
		}
		else
		{
			mState = ObjectiveState.InProgress;
			if (this.OnObjectiveInProgress != null)
			{
				this.OnObjectiveInProgress(this);
			}
			if (mBlip != null)
			{
				mBlip.SwitchOn();
			}
			LogMessage(" Started");
		}
	}

	public void ActivateSectionObjective()
	{
		if (ToBeEnabledOnActivation)
		{
			EnableObjective();
		}
	}

	public void Fail()
	{
		if (mState == ObjectiveState.InProgress)
		{
			mState = ObjectiveState.Failed;
			if (this.OnObjectiveFailed != null)
			{
				this.OnObjectiveFailed(this);
			}
			if (mBlip != null)
			{
				mBlip.SwitchOff();
			}
			LogMessage(" Failed");
		}
	}

	public void Pass()
	{
		if (mState == ObjectiveState.InProgress)
		{
			mState = ObjectiveState.Passed;
			LogMessage("Passed");
			if (mBlip != null)
			{
				mBlip.SwitchOff();
			}
			EventHub.Instance.Report(new Events.ObjectiveCompleted(ActStructure.Instance.CurrentMissionMode, m_Interface.IsPrimaryObjective));
			MissionObjective[] disableObjectivesOnPass = DisableObjectivesOnPass;
			foreach (MissionObjective missionObjective in disableObjectivesOnPass)
			{
				MyObjectiveManager.DeregisterObjective(missionObjective);
				missionObjective.DisableObjective();
				LogMessage(" Disabled", missionObjective.gameObject);
			}
			if (this.OnObjectivePassed != null)
			{
				this.OnObjectivePassed(this);
			}
			MissionObjective[] enableObjectivesOnPass = EnableObjectivesOnPass;
			foreach (MissionObjective missionObjective2 in enableObjectivesOnPass)
			{
				missionObjective2.EnableObjective();
				LogMessage(" Enabled", missionObjective2.gameObject);
			}
			DoOnCompleteCall();
		}
	}

	public void DoOnCompleteCall()
	{
		GameObject[] gameObjectsToActivate = m_Interface.GameObjectsToActivate;
		foreach (GameObject gameObject in gameObjectsToActivate)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(InterfaceableObject), true);
			Component[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				InterfaceableObject interfaceableObject = (InterfaceableObject)array[j];
				interfaceableObject.StartViewable = true;
			}
		}
		if (m_Interface.ObjectToCallOnComplete == null)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectToCallOnComplete)
		{
			Container.SendMessage(item, m_Interface.FunctionToCallOnComplete[num], base.gameObject);
			num++;
		}
	}

	public virtual void SetupFromSetPieceOverride(GameObject[] Objects)
	{
		Debug.LogWarning("Setting up objective in virtual func, please add functionality to the real objective");
	}

	public virtual void Activate()
	{
		if (State == ObjectiveState.Dormant)
		{
			EnableObjective();
		}
		else
		{
			Pass();
		}
	}

	public virtual void Deactivate()
	{
		if (State == ObjectiveState.InProgress)
		{
			DisableObjective();
		}
	}

	public void ToggleDormantState()
	{
		m_Interface.StartDormant = !m_Interface.StartDormant;
		ResetState();
	}

	public void SetDormantState()
	{
		m_Interface.StartDormant = true;
		ResetState();
	}

	public void PingBlip()
	{
		mBlip.PingBlip();
	}

	private string GetMessageColour(string message)
	{
		switch (message)
		{
		case "Started":
		case "Passed":
		case "Enabled":
			return "00FF00";
		case "Dormant":
		case "Failed":
		case "Disabled":
		case "Destroyed":
			return "FF0000";
		default:
			return "FFFFFF";
		}
	}

	public void LogMessage(string message)
	{
	}

	public void LogMessage(string message, GameObject other)
	{
	}
}
