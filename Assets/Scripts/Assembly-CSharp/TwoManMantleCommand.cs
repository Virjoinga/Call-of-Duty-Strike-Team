using System.Collections;

public class TwoManMantleCommand : Command
{
	private enum States
	{
		OutOfRange = 0,
		InRange = 1,
		ConditionsMet = 2,
		Done = 3
	}

	public GuidRef Mantle;

	private InterfaceableObject io;

	public ObjectMessage SelectAllPlayersMessage;

	public ObjectMessage ConditionsMetMessage;

	public ObjectMessage ConditionsUnmetMessage;

	private States mState;

	public override bool Blocking()
	{
		return true;
	}

	public override void ResolveGuidLinks()
	{
		Mantle.ResolveLink();
	}

	public override IEnumerator Execute()
	{
		io = Mantle.theObject.GetComponentInChildren<InterfaceableObject>();
		ChangeState();
		while (mState != States.Done)
		{
			yield return null;
		}
	}

	private IEnumerator StateOutOfRange()
	{
		if (ConditionsUnmetMessage.Object != null && ConditionsUnmetMessage.Message != null && ConditionsUnmetMessage.Message.Length > 0)
		{
			Container.SendMessage(ConditionsUnmetMessage.Object, ConditionsUnmetMessage.Message, base.gameObject);
		}
		while (mState == States.OutOfRange)
		{
			if (AreWeInRange())
			{
				mState = States.InRange;
			}
			yield return null;
		}
		ChangeState();
	}

	private IEnumerator StateInRange()
	{
		if (HowManyPlayersSelected() == 2)
		{
			mState = States.ConditionsMet;
		}
		else if (SelectAllPlayersMessage.Object != null && SelectAllPlayersMessage.Message != null && SelectAllPlayersMessage.Message.Length > 0)
		{
			Container.SendMessage(SelectAllPlayersMessage.Object, SelectAllPlayersMessage.Message, base.gameObject);
		}
		while (mState == States.InRange)
		{
			if (!AreWeInRange())
			{
				mState = States.OutOfRange;
			}
			else if (HowManyPlayersSelected() == 2)
			{
				mState = States.ConditionsMet;
			}
			yield return null;
		}
		ChangeState();
	}

	private IEnumerator StateConditionsMet()
	{
		if (ConditionsMetMessage.Object != null && ConditionsMetMessage.Message != null && ConditionsMetMessage.Message.Length > 0)
		{
			Container.SendMessage(ConditionsMetMessage.Object, ConditionsMetMessage.Message, base.gameObject);
		}
		bool conditionsUnmet = false;
		while (mState == States.ConditionsMet)
		{
			if (!AreWeInRange())
			{
				mState = States.OutOfRange;
				conditionsUnmet = true;
			}
			else if (HowManyPlayersSelected() != 2)
			{
				mState = States.InRange;
				conditionsUnmet = true;
			}
			else if (AreWePerformingTheMantle())
			{
				mState = States.Done;
			}
			yield return null;
		}
		if (conditionsUnmet && ConditionsUnmetMessage.Object != null && ConditionsUnmetMessage.Message != null && ConditionsUnmetMessage.Message.Length > 0)
		{
			Container.SendMessage(ConditionsUnmetMessage.Object, ConditionsUnmetMessage.Message, base.gameObject);
		}
		ChangeState();
	}

	private void ChangeState()
	{
		switch (mState)
		{
		case States.OutOfRange:
			StartCoroutine(StateOutOfRange());
			break;
		case States.InRange:
			StartCoroutine(StateInRange());
			break;
		case States.ConditionsMet:
			StartCoroutine(StateConditionsMet());
			break;
		}
	}

	private bool AreWeInRange()
	{
		if (io != null)
		{
			if (io.ContextBlip.IsOnScreen && io.ContextBlip.IsSwitchedOn)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private int HowManyPlayersSelected()
	{
		return GameplayController.instance.Selected.Count;
	}

	private bool AreWePerformingTheMantle()
	{
		bool flag = false;
		bool flag2 = false;
		foreach (Actor item in GameplayController.instance.Selected)
		{
			if (item.tasks.IsRunningTask(typeof(TaskMultiCharacterSetPiece)))
			{
				flag2 = true;
			}
			else if (item.tasks.Immediate.Stack.Count == 0)
			{
				flag = true;
			}
		}
		return flag2 && flag;
	}
}
