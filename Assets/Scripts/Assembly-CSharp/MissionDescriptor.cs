using System;
using UnityEngine;

public class MissionDescriptor : MonoBehaviour
{
	public bool m_PlayIntroSequence = true;

	public GameObject m_IntroSequenceObject;

	public GameObject m_GameplaySequenceObject;

	[HideInInspector]
	public ScriptedSequence m_IntroSequence;

	[HideInInspector]
	public ScriptedSequence m_GameplaySequence;

	private static MissionDescriptor smInstance;

	private bool mIntroStarted;

	public static MissionDescriptor Instance
	{
		get
		{
			return smInstance;
		}
	}

	public bool StartIntroSequence()
	{
		bool result = false;
		if (m_PlayIntroSequence && !mIntroStarted)
		{
			TimeManager.instance.ResumeNormalTime();
			if (m_IntroSequenceObject != null)
			{
				m_IntroSequence = m_IntroSequenceObject.GetComponentInChildren<ScriptedSequence>();
				if (m_IntroSequence != null)
				{
					m_IntroSequence.StartSequence();
					result = true;
				}
			}
			if (m_GameplaySequenceObject != null)
			{
				m_GameplaySequence = m_GameplaySequenceObject.GetComponentInChildren<ScriptedSequence>();
				if (m_GameplaySequence != null)
				{
					m_GameplaySequence.StartSequence();
					result = true;
				}
			}
			GameplayController gameplayController = GameplayController.Instance();
			if ((bool)gameplayController)
			{
				m_IntroSequence.OnSequenceComplete += gameplayController.LevelStarted;
			}
			mIntroStarted = true;
		}
		return result;
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple MissionDescriptors");
		}
		smInstance = this;
	}

	private void OnDestroy()
	{
		smInstance = null;
	}
}
