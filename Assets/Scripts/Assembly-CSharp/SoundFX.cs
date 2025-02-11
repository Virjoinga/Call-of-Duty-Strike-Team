using UnityEngine;

public class SoundFX : IPoolable
{
	public SoundFXData m_sfxData;

	private int m_lastSoundIndex;

	private float m_lastPlayCall;

	public float LastPlayedTime
	{
		get
		{
			return m_lastPlayCall;
		}
		set
		{
			m_lastPlayCall = value;
		}
	}

	public int nPlayingInstances { get; set; }

	public void Create()
	{
		m_sfxData = null;
		m_lastSoundIndex = -1;
		m_lastPlayCall = float.NegativeInfinity;
	}

	public void New()
	{
		m_sfxData = null;
		m_lastSoundIndex = -1;
		m_lastPlayCall = float.NegativeInfinity;
	}

	public void Delete()
	{
		m_sfxData = null;
		m_lastSoundIndex = -1;
		m_lastPlayCall = float.NegativeInfinity;
	}

	public AudioClip ChooseSfxDataToPlay()
	{
		int num = 0;
		int count = m_sfxData.m_audioSourceData.Count;
		switch (m_sfxData.m_nextSoundChoice)
		{
		case SoundFXData.SoundChoiceBehaviour.Random:
			num = Random.Range(0, count);
			break;
		case SoundFXData.SoundChoiceBehaviour.RandomNoRepeat:
			if (count <= 1)
			{
				break;
			}
			if (m_lastSoundIndex == -1)
			{
				num = Random.Range(0, count);
				break;
			}
			do
			{
				num = Random.Range(0, count);
			}
			while (num == m_lastSoundIndex);
			break;
		case SoundFXData.SoundChoiceBehaviour.Sequential:
			num = (m_lastSoundIndex + 1) % count;
			break;
		}
		m_lastSoundIndex = num;
		AudioClip result = null;
		if (num < count)
		{
			result = m_sfxData.m_audioSourceData[num];
		}
		return result;
	}
}
