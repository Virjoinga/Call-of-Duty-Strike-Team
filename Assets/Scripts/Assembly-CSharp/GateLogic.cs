using System.Collections.Generic;
using UnityEngine;

public class GateLogic : MonoBehaviour
{
	private enum State
	{
		Open = 0,
		Closed = 1
	}

	public List<GameObject> GatesComponents;

	private ObjectMover[] mGateMovers;

	public bool StartOpen;

	public GameObject NavGateObject;

	private NavGate NavGate;

	private State mState = State.Closed;

	private void Start()
	{
		mGateMovers = new ObjectMover[GatesComponents.Count];
		for (int i = 0; i < GatesComponents.Count; i++)
		{
			if (GatesComponents[i] != null)
			{
				mGateMovers[i] = GatesComponents[i].GetComponent<ObjectMover>();
			}
		}
		if (NavGateObject != null)
		{
			NavGate = NavGateObject.GetComponentInChildren<NavGate>();
		}
		if (StartOpen)
		{
			OnEnter();
		}
	}

	private void Update()
	{
	}

	public void Activate()
	{
		OnEnter();
	}

	public void Deactivate()
	{
		OnLeave();
	}

	private void OnEnter()
	{
		if (mState != State.Closed)
		{
			return;
		}
		bool flag = false;
		ObjectMover[] array = mGateMovers;
		foreach (ObjectMover objectMover in array)
		{
			if (objectMover != null && objectMover.IsMoving())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		bool flag2 = false;
		ObjectMover[] array2 = mGateMovers;
		foreach (ObjectMover objectMover2 in array2)
		{
			if (objectMover2 != null)
			{
				objectMover2.OnEnter();
				flag2 = true;
			}
		}
		if (flag2)
		{
			if (NavGate != null)
			{
				NavGate.OpenNavGate();
			}
			mState = State.Open;
		}
	}

	private void OnLeave()
	{
		if (mState != 0)
		{
			return;
		}
		bool flag = false;
		ObjectMover[] array = mGateMovers;
		foreach (ObjectMover objectMover in array)
		{
			if (objectMover != null && objectMover.IsMoving())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		bool flag2 = false;
		ObjectMover[] array2 = mGateMovers;
		foreach (ObjectMover objectMover2 in array2)
		{
			if (objectMover2 != null)
			{
				objectMover2.OnLeave();
				flag2 = true;
			}
		}
		if (flag2)
		{
			if (NavGate != null)
			{
				NavGate.CloseNavGate();
			}
			mState = State.Closed;
		}
	}
}
