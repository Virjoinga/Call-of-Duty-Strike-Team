using System;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour
{
	private static AlarmManager smInstance;

	public float m_CutOffAlarmTime = 120f;

	public bool m_CanCutOffAlarm;

	public float m_DurationBeforeFade = 5f;

	public float m_TimeToFade = 2.5f;

	public float m_VolumeToFadeTo = 0.05f;

	public List<GameObject> m_ObjectsToCallOnHack = new List<GameObject>();

	public List<string> m_FunctionsToCallOnHack = new List<string>();

	public List<GameObject> m_ObjectsToCallOnTrigger = new List<GameObject>();

	public List<string> m_FunctionsToCallOnTrigger = new List<string>();

	public List<GameObject> m_AlarmPanels = new List<GameObject>();

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public bool UseAwarenessZones = true;

	private float m_CurrentAlarmTime;

	private bool m_AlarmSounding;

	private bool m_AlarmFaded;

	private Vector3 m_PositionAlarmSetOff = Vector3.zero;

	public static AlarmManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public bool AlarmSounding
	{
		get
		{
			return m_AlarmSounding;
		}
	}

	public Vector3 AlarmPosition
	{
		get
		{
			return m_PositionAlarmSetOff;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple AlarmManagers");
		}
		smInstance = this;
		m_CurrentAlarmTime = 0f;
		m_AlarmSounding = false;
		m_AlarmFaded = false;
		m_PositionAlarmSetOff = Vector3.zero;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void Update()
	{
		if (m_AlarmSounding)
		{
			m_CurrentAlarmTime += Time.deltaTime;
			if (m_CanCutOffAlarm && m_CurrentAlarmTime > m_CutOffAlarmTime)
			{
				m_AlarmFaded = false;
				m_AlarmSounding = false;
			}
			if (!m_AlarmFaded && m_CurrentAlarmTime > m_DurationBeforeFade)
			{
				m_AlarmFaded = true;
				FadeSoundFx.FadeSfxHelper(base.gameObject, EnvironmentSFX.Instance.AlarmSiren1, true, false, m_TimeToFade, m_VolumeToFadeTo, false);
			}
		}
	}

	public void Activate()
	{
		Activate(base.gameObject.transform.position);
	}

	public void Activate(Vector3 position)
	{
		if (m_AlarmSounding)
		{
			return;
		}
		m_AlarmSounding = true;
		m_CurrentAlarmTime = 0f;
		m_PositionAlarmSetOff = position;
		EnvironmentSFX.Instance.AlarmSiren1.Play(base.gameObject);
		BroadcastAlarm(m_PositionAlarmSetOff);
		if (m_ObjectsToCallOnTrigger != null && m_ObjectsToCallOnTrigger.Count > 0)
		{
			int num = 0;
			string message = string.Empty;
			foreach (GameObject item in m_ObjectsToCallOnTrigger)
			{
				if (m_FunctionsToCallOnTrigger != null && num < m_FunctionsToCallOnTrigger.Count)
				{
					message = m_FunctionsToCallOnTrigger[num];
				}
				Container.SendMessage(item, message, base.gameObject);
				num++;
			}
		}
		EnableAlarmPanelLights(true);
	}

	public void Deactivate()
	{
		Deactivate(false);
	}

	public void DeactivateDueToHack()
	{
		Deactivate(true);
	}

	public void Deactivate(bool hacked)
	{
		m_PositionAlarmSetOff = Vector3.zero;
		EnvironmentSFX.Instance.AlarmSiren1.StopAfterLoop(base.gameObject);
		if (hacked && m_ObjectsToCallOnHack != null && m_ObjectsToCallOnHack.Count > 0)
		{
			int num = 0;
			string message = string.Empty;
			foreach (GameObject item in m_ObjectsToCallOnHack)
			{
				if (m_FunctionsToCallOnHack != null && num < m_FunctionsToCallOnHack.Count)
				{
					message = m_FunctionsToCallOnHack[num];
				}
				Container.SendMessage(item, message, base.gameObject);
				num++;
			}
		}
		EnableAlarmPanelLights(false);
	}

	private void EnableAlarmPanelLights(bool yesNo)
	{
		if (m_AlarmPanels == null || m_AlarmPanels.Count == 0 || !(m_AlarmPanels[0] != null))
		{
			return;
		}
		foreach (GameObject alarmPanel in m_AlarmPanels)
		{
			AlarmPanel componentInChildren = alarmPanel.GetComponentInChildren<AlarmPanel>();
			if (componentInChildren != null)
			{
				componentInChildren.EnableLight(yesNo);
			}
		}
	}

	private void BroadcastAlarm(Vector3 origin)
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.ears == null || !a.ears.CanHear)
			{
				continue;
			}
			bool flag = true;
			if (UseAwarenessZones)
			{
				flag = AwarenessZone.IsUnregisteredGameObjectAwarenessInSync(origin, a.gameObject.transform.position);
			}
			if (flag)
			{
				float magnitude = (origin - a.GetPosition()).magnitude;
				if (magnitude < AudioResponseRanges.Alarm + a.ears.Range)
				{
					a.behaviour.BlameEnemyForEvent(origin);
				}
			}
		}
	}

	public AlarmPanel GetNearestAlarmPanel(Actor actor)
	{
		if (actor == null)
		{
			return null;
		}
		Vector3 position = actor.GetPosition();
		if (m_AlarmPanels != null && m_AlarmPanels.Count != 0 && m_AlarmPanels[0] != null)
		{
			AlarmPanel result = null;
			float num = float.PositiveInfinity;
			{
				foreach (GameObject alarmPanel in m_AlarmPanels)
				{
					AlarmPanel componentInChildren = alarmPanel.GetComponentInChildren<AlarmPanel>();
					if (!(componentInChildren != null) || componentInChildren.CantBeUsed() || (actor.awareness.faction == FactionHelper.Category.Enemy && componentInChildren.EnemyRegisteredAsInterested) || componentInChildren.InterestZones == null || componentInChildren.InterestZones.Count == 0 || !(componentInChildren.InterestZones[0] != null))
					{
						continue;
					}
					foreach (InterestZone interestZone in componentInChildren.InterestZones)
					{
						if (interestZone != null && interestZone.Contains(actor))
						{
							float sqrMagnitude = (alarmPanel.transform.position - position).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								result = componentInChildren;
								num = sqrMagnitude;
							}
						}
					}
				}
				return result;
			}
		}
		return null;
	}
}
