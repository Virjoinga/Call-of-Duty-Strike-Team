using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
	protected class StateFunctions
	{
		public delegate void EnterStateFunc(int fromState);

		public delegate void UpdateStateFunc();

		public delegate void ExitStateFunc(int toState);

		public EnterStateFunc mEnter;

		public UpdateStateFunc mUpdate;

		public ExitStateFunc mExit;

		public string mDebugName;

		public StateFunctions(EnterStateFunc enter, UpdateStateFunc update, ExitStateFunc exit, string name)
		{
			mEnter = enter;
			mUpdate = update;
			mExit = exit;
			mDebugName = name;
		}
	}

	private int mCurrentState;

	private int mNextState;

	private int mPreviousState;

	protected Dictionary<int, StateFunctions> mStateTable;

	protected abstract string FSMName();

	public int CurrentState()
	{
		return mCurrentState;
	}

	public int PreviousState()
	{
		return mPreviousState;
	}

	public string CurrentStateName()
	{
		StateFunctions value;
		if (mStateTable.TryGetValue(mCurrentState, out value))
		{
			return value.mDebugName;
		}
		return "Unknown state";
	}

	public virtual void Awake()
	{
		mNextState = (mCurrentState = (mPreviousState = -1));
		mStateTable = new Dictionary<int, StateFunctions>();
	}

	public virtual void Update()
	{
		StateFunctions value;
		if (mCurrentState != mNextState)
		{
			if (mStateTable.TryGetValue(mCurrentState, out value))
			{
				value.mExit(mNextState);
			}
			mPreviousState = mCurrentState;
			mCurrentState = mNextState;
			if (mStateTable.TryGetValue(mCurrentState, out value))
			{
				value.mEnter(mPreviousState);
			}
		}
		if (mStateTable.TryGetValue(mCurrentState, out value))
		{
			value.mUpdate();
			return;
		}
		Debug.LogError(FSMName() + ": Switching to state " + mCurrentState + " which does not exist in this state table");
	}

	protected void SwitchState(int newState)
	{
		mNextState = newState;
	}
}
