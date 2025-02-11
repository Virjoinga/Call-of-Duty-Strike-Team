using UnityEngine;

public class CharacterStateListener : MonoBehaviour
{
	public CharacterStateListenerData m_Interface;

	private TaskDescriptorOverrideHub mTaskDescriptorOverrideHub;

	private bool mCompleted;

	private void Start()
	{
		mTaskDescriptorOverrideHub = GetComponent<TaskDescriptorOverrideHub>();
		if (!m_Interface.BroadcastImmediately && mTaskDescriptorOverrideHub != null)
		{
			mTaskDescriptorOverrideHub.m_Interface.BroadcastOnCompletion.Clear();
			mTaskDescriptorOverrideHub.m_Interface.BroadcastOnCompletion.Add(m_Interface.BroadcastOnCompletion);
		}
		mCompleted = false;
	}

	private void Update()
	{
		if (mCompleted)
		{
			return;
		}
		ActorWrapper actorWrapper = null;
		for (int i = 0; i < m_Interface.Monitored.Count; i++)
		{
			actorWrapper = m_Interface.Monitored[i];
			if (actorWrapper == null)
			{
				continue;
			}
			Actor actor = actorWrapper.GetActor();
			if (actor == null || actor.realCharacter.IsDead())
			{
				continue;
			}
			if (actor.awareness.ChDefCharacterType == CharacterType.SecurityCamera && actor.behaviour.alertState >= BehaviourController.AlertState.Alerted)
			{
				mCompleted = true;
			}
			else if (actor.behaviour.InActiveAlertState())
			{
				if (mTaskDescriptorOverrideHub != null)
				{
					mTaskDescriptorOverrideHub.OnEnter();
				}
				mCompleted = true;
				if (!m_Interface.BroadcastImmediately)
				{
					return;
				}
			}
		}
		if (mCompleted)
		{
			m_Interface.BroadcastOnCompletion.SendMessages();
			ActivateTurnonables();
			DeactivateTurnoffables();
		}
	}

	public void Activate()
	{
		ActivateTurnonables();
	}

	public void Deactivate()
	{
		DeactivateTurnoffables();
	}

	public void ActivateTurnonables()
	{
		if (m_Interface.Turnonables == null || m_Interface.Turnonables.Count <= 0)
		{
			return;
		}
		foreach (GameObject turnonable in m_Interface.Turnonables)
		{
			turnonable.SetActive(true);
		}
	}

	public void DeactivateTurnoffables()
	{
		if (m_Interface.Turnoffables == null || m_Interface.Turnoffables.Count <= 0)
		{
			return;
		}
		foreach (GameObject turnoffable in m_Interface.Turnoffables)
		{
			turnoffable.SetActive(true);
		}
	}
}
