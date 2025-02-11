using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SPStatement
{
	public List<SPCondition> Conditions;

	public List<SPAction> Actions;

	public bool IsTestStatement;

	public int branchTo;

	private bool WaitOnActions;

	private float StatementStartTime;

	private string LastAction;

	private bool followBranch;

	private SetPieceModule mModuleRef;

	public SPStatement()
	{
		Conditions = new List<SPCondition>();
		Actions = new List<SPAction>();
		WaitOnActions = false;
		IsTestStatement = false;
		branchTo = -1;
		StatementStartTime = -1f;
		LastAction = string.Empty;
		followBranch = false;
	}

	public void Skip(SetPieceModule moduleRef)
	{
		foreach (SPAction action in Actions)
		{
			action.SetUp(moduleRef);
			action.Skip();
		}
		foreach (SPCondition condition in Conditions)
		{
			if (condition.Type == SPCondition.SPConditionType.OutroEnd && GameController.Instance != null)
			{
				GameController.Instance.OnMissionPassed(this, 0f);
			}
		}
	}

	public void PostSkip()
	{
		foreach (SPAction action in Actions)
		{
			action.PostSkip();
		}
	}

	public void ReceiveSignal(string sig)
	{
		foreach (SPCondition condition in Conditions)
		{
			condition.ReceiveSignal(sig);
		}
	}

	public SPCondition.ReturnCode TestConditions()
	{
		if (!WaitOnActions)
		{
			foreach (SPCondition condition in Conditions)
			{
				SPCondition.ReturnCode returnCode = condition.TestCondition(Time.time - mModuleRef.PausedTime);
				if (returnCode != SPCondition.ReturnCode.Pass)
				{
					return returnCode;
				}
			}
		}
		return SPCondition.ReturnCode.Pass;
	}

	public SPAction.ReturnCode DoActions()
	{
		bool waitOnActions = false;
		foreach (SPAction action in Actions)
		{
			if (!action.ActionDone)
			{
				SPAction.ReturnCode returnCode = action.DoAction(GetStatementTime());
				if (returnCode == SPAction.ReturnCode.Wait)
				{
					waitOnActions = true;
				}
				else
				{
					LastAction = action.Type.ToString();
				}
				if (returnCode == SPAction.ReturnCode.Branch)
				{
					followBranch = true;
				}
			}
		}
		WaitOnActions = waitOnActions;
		if (!WaitOnActions)
		{
			foreach (SPAction action2 in Actions)
			{
				action2.Reset();
			}
			if (followBranch)
			{
				return SPAction.ReturnCode.Branch;
			}
			return SPAction.ReturnCode.Done;
		}
		return SPAction.ReturnCode.Wait;
	}

	public string GetLastAction()
	{
		return LastAction;
	}

	public void InsertAction(SPAction.SPActionType type, SetPieceModule moduleRef)
	{
		SPAction sPAction = new SPAction();
		sPAction.SetUp(type, moduleRef);
		sPAction.DelayTime = GetStatementTime();
		Actions.Add(sPAction);
	}

	public void StartStatement(SetPieceModule moduleRef)
	{
		mModuleRef = moduleRef;
		StatementStartTime = Time.time - mModuleRef.PausedTime;
		foreach (SPCondition condition in Conditions)
		{
			condition.StartCondition(StatementStartTime);
		}
		foreach (SPAction action in Actions)
		{
			action.SetUp(moduleRef);
		}
	}

	public float GetStatementTime()
	{
		if (StatementStartTime == -1f)
		{
			return 0f;
		}
		return Time.time - mModuleRef.PausedTime - StatementStartTime;
	}

	public void Reset()
	{
		foreach (SPAction action in Actions)
		{
			action.Reset();
		}
		foreach (SPCondition condition in Conditions)
		{
			condition.Reset();
		}
	}
}
