using System;
using UnityEngine;

[Serializable]
public class DestructibleFX
{
	public GameObject m_ParticleEffect;

	public float m_HealthToTrigger;

	public Transform m_PositionTransform;

	public bool m_TriggerSelfHarm;

	private bool m_Active;

	private GameObject m_Instance;

	public bool Active
	{
		get
		{
			return m_Active;
		}
		set
		{
			m_Active = value;
		}
	}

	public GameObject Instance
	{
		get
		{
			return m_Instance;
		}
		set
		{
			m_Instance = value;
		}
	}
}
