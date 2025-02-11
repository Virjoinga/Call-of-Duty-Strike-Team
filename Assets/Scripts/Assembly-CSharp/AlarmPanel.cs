using System.Collections.Generic;
using UnityEngine;

public class AlarmPanel : MonoBehaviour
{
	private enum State
	{
		TurnedOff = 1,
		TurnedOn = 2,
		Hacked = 4,
		OutOfAction = 8,
		Disabled = 0x10
	}

	public AlarmPanelData m_Interface;

	public List<SpawnerBase> spawners;

	public List<SpawnerCoordinator> spawnCoordinators;

	public List<InterestZone> InterestZones = new List<InterestZone>();

	public PlaySoundFx sfx;

	public AuditoryAwarenessEvent aae;

	public SetPieceModule ActivateSetPiece;

	private List<Actor> mInterestedPlayerUnits = new List<Actor>();

	private State m_State = State.TurnedOff;

	private HaloEffect m_Halo;

	private List<Actor> InterestedPartiesThatAreDead = new List<Actor>();

	private List<Actor> m_InterestedParties = new List<Actor>();

	public Actor m_BookingAgent;

	private bool m_EnemyRegisteredAsInterested;

	public bool EnemyRegisteredAsInterested
	{
		get
		{
			return m_EnemyRegisteredAsInterested;
		}
	}

	private void Start()
	{
		aae = GetComponent<AuditoryAwarenessEvent>();
		aae.Radius = m_Interface.range;
		m_Halo = GetComponent<HaloEffect>();
		m_EnemyRegisteredAsInterested = false;
	}

	public void Update()
	{
		CheckInterestedUnitDistances();
		if (m_BookingAgent == null)
		{
			foreach (Actor interestedParty in m_InterestedParties)
			{
				if (!(interestedParty != null))
				{
					continue;
				}
				if (interestedParty.realCharacter.IsDead())
				{
					InterestedPartiesThatAreDead.Add(interestedParty);
					continue;
				}
				float sqrMagnitude = (interestedParty.transform.position - base.transform.position).sqrMagnitude;
				if (!(sqrMagnitude < 5f))
				{
					continue;
				}
				m_BookingAgent = interestedParty;
				break;
			}
		}
		foreach (Actor item in InterestedPartiesThatAreDead)
		{
			if (item != null)
			{
				m_InterestedParties.Remove(item);
			}
		}
		InterestedPartiesThatAreDead.Clear();
		bool flag = false;
		foreach (Actor interestedParty2 in m_InterestedParties)
		{
			if (interestedParty2 != null && interestedParty2.awareness.faction != 0)
			{
				flag = true;
				break;
			}
		}
		if (m_InterestedParties.Count == 0 || !flag)
		{
			m_EnemyRegisteredAsInterested = false;
		}
		if (!(m_BookingAgent != null))
		{
			return;
		}
		if (m_BookingAgent.realCharacter != null)
		{
			bool flag2 = m_BookingAgent.tasks.IsRunningTask<TaskHack>() || m_BookingAgent.tasks.IsRunningTask<TaskUseAlarmPanel>();
			if (m_BookingAgent.realCharacter.IsDead() || !flag2)
			{
				m_InterestedParties.Remove(m_BookingAgent);
				m_BookingAgent = null;
				return;
			}
		}
		foreach (Actor interestedParty3 in m_InterestedParties)
		{
			if (interestedParty3 != null && interestedParty3 != m_BookingAgent)
			{
				interestedParty3.tasks.CancelTasks(typeof(TaskMoveTo));
				interestedParty3.tasks.CancelTasks(typeof(TaskUseAlarmPanel));
				interestedParty3.tasks.CancelTasks(typeof(TaskHack));
				interestedParty3.tasks.CancelTasks(typeof(TaskSetPiece));
			}
		}
		m_InterestedParties.Clear();
		m_EnemyRegisteredAsInterested = false;
	}

	private void CheckInterestedUnitDistances()
	{
		if (m_BookingAgent == null || mInterestedPlayerUnits == null || mInterestedPlayerUnits.Count == 0)
		{
			return;
		}
		Vector2 vector = base.transform.position.xz();
		float num = Vector2.SqrMagnitude(m_BookingAgent.GetPosition().xz() - vector);
		float num2 = float.MaxValue;
		int index = -1;
		for (int i = 0; i < mInterestedPlayerUnits.Count; i++)
		{
			if (!(mInterestedPlayerUnits[i] == null))
			{
				float num3 = Vector2.SqrMagnitude(mInterestedPlayerUnits[i].GetPosition().xz() - vector);
				if (num3 < num2)
				{
					num2 = num3;
					index = i;
				}
			}
		}
		if (!(num > 1f) || !(num2 > 1f))
		{
			if (num <= num2)
			{
				StopInterestedPlayerUnits();
				return;
			}
			m_BookingAgent.tasks.CancelTasks<TaskUseAlarmPanel>();
			m_BookingAgent.tasks.CancelTasks<TaskSetPiece>();
			m_BookingAgent = null;
			Actor item = mInterestedPlayerUnits[index];
			mInterestedPlayerUnits.RemoveAt(index);
			StopInterestedPlayerUnits();
			mInterestedPlayerUnits.Add(item);
		}
	}

	private void StopInterestedPlayerUnits()
	{
		List<Actor> list = new List<Actor>();
		foreach (Actor mInterestedPlayerUnit in mInterestedPlayerUnits)
		{
			if (!(mInterestedPlayerUnit == null))
			{
				TaskHack taskHack = mInterestedPlayerUnit.tasks.GetRunningTask(typeof(TaskHack)) as TaskHack;
				if (taskHack != null)
				{
					taskHack.FinishQuick = true;
					list.Add(mInterestedPlayerUnit);
				}
			}
		}
		foreach (Actor item in list)
		{
			if (item != null)
			{
				item.tasks.CancelTasks<TaskHack>();
				item.tasks.CancelTasks<TaskCacheStanceState>();
				item.tasks.CancelTasks<TaskSetPiece>();
			}
		}
		mInterestedPlayerUnits.Clear();
	}

	public void AddToInterestedPartiesList(Actor actor)
	{
		if (!m_InterestedParties.Contains(actor))
		{
			if (actor.awareness.faction == FactionHelper.Category.Enemy)
			{
				m_EnemyRegisteredAsInterested = true;
			}
			m_InterestedParties.Add(actor);
		}
	}

	public void AddInterestedPlayerUnit(Actor actor)
	{
		if (actor != null && mInterestedPlayerUnits != null)
		{
			mInterestedPlayerUnits.Add(actor);
		}
	}

	public void RemovePlayerUnitFromInterested(Actor actor)
	{
		if (actor != null && mInterestedPlayerUnits != null)
		{
			mInterestedPlayerUnits.Remove(actor);
		}
	}

	public void Activate()
	{
		StartAlarm();
	}

	public void Deactivate()
	{
		StopAlarm(false);
	}

	public void MarkAsHacked()
	{
		m_State = State.Hacked;
	}

	private void StartAlarm()
	{
		if (!CanBeTurnedOn() || m_State == State.Disabled)
		{
			return;
		}
		if ((bool)AlarmManager.Instance)
		{
			AlarmManager.Instance.Activate(base.gameObject.transform.position);
		}
		if (aae != null)
		{
			aae.Trigger();
		}
		foreach (SpawnerBase spawner in spawners)
		{
			if (spawner as SpawnerDoor != null)
			{
				(spawner as SpawnerDoor).Activate();
			}
			if (spawner as SpawnerRoof != null)
			{
				(spawner as SpawnerRoof).Activate();
			}
		}
		foreach (SpawnerCoordinator spawnCoordinator in spawnCoordinators)
		{
			if (spawnCoordinator != null)
			{
				spawnCoordinator.Activate();
			}
		}
		m_State = State.TurnedOn;
		if (!GameplayController.Instance().PanelAlarmSounded)
		{
			GameplayController.Instance().PanelAlarmSounded = true;
		}
	}

	private void StopAlarm(bool stoppedDueToHack)
	{
		if ((bool)AlarmManager.Instance)
		{
			AlarmManager.Instance.Deactivate(stoppedDueToHack);
		}
		foreach (SpawnerBase spawner in spawners)
		{
			if (spawner as SpawnerDoor != null)
			{
				(spawner as SpawnerDoor).Deactivate();
			}
			if (spawner as SpawnerRoof != null)
			{
				(spawner as SpawnerRoof).Deactivate();
			}
		}
		foreach (SpawnerCoordinator spawnCoordinator in spawnCoordinators)
		{
			if (spawnCoordinator != null)
			{
				spawnCoordinator.Deactivate();
			}
		}
		m_State = State.TurnedOff;
	}

	public void Use()
	{
		if (m_State == State.TurnedOff)
		{
			StartAlarm();
		}
		else if (m_State == State.TurnedOn)
		{
			StopAlarm(false);
		}
		else if (m_State == State.Hacked)
		{
			m_State = State.OutOfAction;
		}
	}

	public void TurnOnPanel()
	{
		m_State = State.TurnedOff;
	}

	public void TurnOffPanel()
	{
		m_State = State.Disabled;
	}

	public bool CanBeTurnedOn()
	{
		return m_State == State.TurnedOff;
	}

	public bool CanBeTurnedOff()
	{
		return m_State == State.TurnedOn;
	}

	public bool CanBeHacked()
	{
		return m_State != State.Hacked;
	}

	public bool CanBeTriggered()
	{
		if (m_State != State.OutOfAction)
		{
			if (m_State == State.Hacked)
			{
				m_State = State.OutOfAction;
			}
			return true;
		}
		return false;
	}

	public bool CantBeUsed()
	{
		return m_State == State.Disabled;
	}

	public void EnableLight(bool onOff)
	{
		if (m_Halo != null)
		{
			if (onOff)
			{
				m_Halo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkMedium);
			}
			else
			{
				m_Halo.SetBlinkPattern(HaloEffect.BlinkPattern.Off);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, m_Interface.range);
	}
}
