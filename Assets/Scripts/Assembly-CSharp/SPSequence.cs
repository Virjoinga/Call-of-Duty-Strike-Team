using System;
using System.Collections.Generic;

[Serializable]
public class SPSequence
{
	public List<SPStatement> Statements;

	private int CurrentIndex;

	private string LastAction;

	private SetPieceModule mModuleRef;

	public SPSequence()
	{
		LastAction = string.Empty;
		Statements = new List<SPStatement>();
		CurrentIndex = -1;
	}

	public void ReceiveSignal(string sig)
	{
		foreach (SPStatement statement in Statements)
		{
			statement.ReceiveSignal(sig);
		}
	}

	public void Activate(SetPieceModule moduleRef)
	{
		CurrentIndex = 0;
		Statements[CurrentIndex].StartStatement(moduleRef);
		mModuleRef = moduleRef;
	}

	public void Deactivate()
	{
		if (CurrentIndex != -1)
		{
			Statements[CurrentIndex].Reset();
		}
		CurrentIndex = -1;
	}

	public void Skip(SetPieceModule moduleRef)
	{
		for (int i = CurrentIndex; i < Statements.Count; i++)
		{
			Statements[i].Skip(moduleRef);
		}
	}

	public void PostSkip()
	{
		for (int i = 0; i < Statements.Count; i++)
		{
			Statements[i].PostSkip();
		}
	}

	public bool Update()
	{
		if (CurrentIndex != -1)
		{
			SPAction.ReturnCode returnCode = SPAction.ReturnCode.Wait;
			SPStatement sPStatement = Statements[CurrentIndex];
			SPCondition.ReturnCode returnCode2 = sPStatement.TestConditions();
			if (returnCode2 == SPCondition.ReturnCode.Pass)
			{
				returnCode = sPStatement.DoActions();
			}
			if (returnCode2 == SPCondition.ReturnCode.Skip)
			{
				returnCode = SPAction.ReturnCode.Done;
			}
			if (returnCode == SPAction.ReturnCode.Done)
			{
				GotoNextStatement();
			}
			if (returnCode == SPAction.ReturnCode.Branch)
			{
				GotoBranchStatement();
			}
		}
		if (CurrentIndex == -1)
		{
			return false;
		}
		return true;
	}

	public string GetLastAction()
	{
		if (CurrentIndex != -1)
		{
			SPStatement sPStatement = Statements[CurrentIndex];
			LastAction = sPStatement.GetLastAction();
		}
		return LastAction;
	}

	public int GetCurrentStatement()
	{
		return CurrentIndex;
	}

	public float GetCurrentTimer()
	{
		if (CurrentIndex != -1)
		{
			return Statements[CurrentIndex].GetStatementTime();
		}
		return 0f;
	}

	public void InsertAction(SPAction.SPActionType type)
	{
		if (CurrentIndex != -1)
		{
			SPStatement sPStatement = Statements[CurrentIndex];
			if (sPStatement != null)
			{
				sPStatement.InsertAction(type, mModuleRef);
			}
		}
	}

	private void GotoNextStatement()
	{
		if (CurrentIndex >= 0)
		{
			Statements[CurrentIndex].Reset();
		}
		CurrentIndex++;
		if (CurrentIndex >= Statements.Count)
		{
			CurrentIndex = -1;
		}
		else
		{
			Statements[CurrentIndex].StartStatement(mModuleRef);
		}
	}

	private void GotoBranchStatement()
	{
		if (CurrentIndex >= 0)
		{
			Statements[CurrentIndex].Reset();
		}
		CurrentIndex = Statements[CurrentIndex].branchTo;
		if (CurrentIndex < 0 || CurrentIndex >= Statements.Count)
		{
			CurrentIndex = -1;
		}
		else
		{
			Statements[CurrentIndex].StartStatement(mModuleRef);
		}
	}

	public bool HasFinished()
	{
		return CurrentIndex == -1;
	}
}
