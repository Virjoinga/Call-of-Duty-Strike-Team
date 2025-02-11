using System.Collections.Generic;
using UnityEngine;

public class RandomAmbientEffect : MonoBehaviour
{
	public List<SoundBankData> m_RandomSoundData = new List<SoundBankData>();

	public float m_MinDelay;

	public float m_MaxDelay = 10f;

	private float m_AccumulatedDeltaTime;

	private float m_CurrentDelay = 10f;

	private AmbientAudioSource m_AmbientAudioSource;

	public void Awake()
	{
	}

	public void Start()
	{
		m_AmbientAudioSource = GetComponent<AmbientAudioSource>();
		m_CurrentDelay = Random.Range(m_MinDelay, m_MaxDelay);
		if (m_AmbientAudioSource != null && m_RandomSoundData != null && m_RandomSoundData.Count > 1)
		{
			m_AmbientAudioSource.SoundData = m_RandomSoundData[0];
			m_AmbientAudioSource.Play();
		}
	}

	public void OnDestroy()
	{
		if (m_AmbientAudioSource != null)
		{
			m_AmbientAudioSource.Stop();
		}
		m_RandomSoundData.Clear();
	}

	public void Update()
	{
		if (TimeManager.instance.GlobalTimeState == TimeManager.State.IngamePaused)
		{
			return;
		}
		m_AccumulatedDeltaTime += TimeManager.DeltaTime;
		if (!(m_AccumulatedDeltaTime > m_CurrentDelay))
		{
			return;
		}
		if (m_AmbientAudioSource != null && m_RandomSoundData != null)
		{
			if (m_RandomSoundData.Count > 1)
			{
				SoundBankData soundData = m_AmbientAudioSource.SoundData;
				m_RandomSoundData.Remove(soundData);
				int index = Random.Range(0, m_RandomSoundData.Count - 1);
				m_AmbientAudioSource.SoundData = m_RandomSoundData[index];
				m_RandomSoundData.Add(soundData);
			}
			else if (m_RandomSoundData.Count != 0)
			{
				m_AmbientAudioSource.SoundData = m_RandomSoundData[0];
			}
			m_AmbientAudioSource.Play();
		}
		m_CurrentDelay = Random.Range(m_MinDelay, m_MaxDelay);
		m_AccumulatedDeltaTime = 0f;
	}
}
