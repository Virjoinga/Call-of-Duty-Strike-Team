using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedSequence : MonoBehaviour
{
	public delegate void SequenceCompleteEventHandler(object sender);

	public ScriptedSequenceData m_Interface;

	public List<Command> Commands = new List<Command>();

	private bool hasFired;

	private int mCommandIndex;

	private bool mIsActive;

	public event SequenceCompleteEventHandler OnSequenceComplete;

	private void Start()
	{
		ResetSequence();
		if (m_Interface.autoRun && !m_Interface.runAfterLoadout)
		{
			StartSequence();
		}
		if (m_Interface.runAfterLoadout)
		{
			StartCoroutine(RunAfterLoadout());
		}
	}

	private IEnumerator RunAfterLoadout()
	{
		while (!hasFired)
		{
			GameController gameController = GameController.Instance;
			if (gameController != null && gameController.GameplayStarted)
			{
				StartSequence();
				hasFired = true;
			}
			yield return null;
		}
	}

	public void Activate()
	{
		StartSequence();
	}

	public void Deactivate()
	{
		EndSequence();
		StopAllCoroutines();
		mIsActive = false;
	}

	public void StartSequence()
	{
		mIsActive = true;
		StartCoroutine(RunSequence());
		InteractionsManager.Instance.RegisterAction(base.gameObject, null);
	}

	public void EndSequence()
	{
		InteractionsManager.Instance.FinishedAction(base.gameObject, null);
	}

	public void ResetSequence()
	{
		mCommandIndex = 0;
	}

	public void RestartSequence()
	{
		StopAllCoroutines();
		mIsActive = false;
		ResetSequence();
		StartSequence();
	}

	private IEnumerator RunSequence()
	{
		while (GameController.Instance == null || (!GameController.Instance.IsReady && mIsActive))
		{
			yield return null;
		}
		while (mCommandIndex < Commands.Count && mIsActive)
		{
			Command command = Commands[mCommandIndex];
			mCommandIndex++;
			LogSequenceDebug(command);
			Coroutine commandCoroutine = StartCoroutine(command.Execute());
			if (command.Blocking())
			{
				yield return commandCoroutine;
			}
		}
		if (this.OnSequenceComplete != null && mIsActive)
		{
			Time.timeScale = 1f;
			this.OnSequenceComplete(this);
		}
		if (m_Interface.autoLoopOnComplete)
		{
			RestartSequence();
		}
		EndSequence();
	}

	private void LogSequenceDebug(Command com)
	{
	}

	public void SkipSequence()
	{
		Time.timeScale = 99.9f;
	}
}
