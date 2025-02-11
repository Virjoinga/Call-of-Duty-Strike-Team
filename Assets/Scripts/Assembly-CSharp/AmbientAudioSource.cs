using UnityEngine;

public class AmbientAudioSource : MonoBehaviour
{
	public SoundBankData m_SoundBankData;

	private SoundManager.SoundInstance m_sndInst;

	public SoundBankData SoundData
	{
		get
		{
			return m_SoundBankData;
		}
		set
		{
			m_SoundBankData = value;
		}
	}

	public SoundFXData InternalSFXData
	{
		get
		{
			return m_SoundBankData.m_SoundFXData;
		}
		set
		{
			m_SoundBankData.m_SoundFXData = value;
		}
	}

	public void Awake()
	{
		m_sndInst = SoundManager.SoundInstance.Null;
	}

	public void OnDestroy()
	{
		Stop();
		m_SoundBankData.m_SoundFXData = null;
	}

	public void Start()
	{
		if (m_SoundBankData != null && m_SoundBankData.m_SoundFXData != null && m_SoundBankData.m_SoundFXData.m_playOnAwake)
		{
			Play();
		}
	}

	public void Stop()
	{
		SoundManager.Instance.Stop(m_SoundBankData.m_SoundFXData, base.gameObject);
	}

	public void Play()
	{
		if (m_SoundBankData != null && m_SoundBankData.m_SoundFXData != null)
		{
			m_sndInst = SoundManager.Instance.Play(m_SoundBankData.m_SoundFXData, base.gameObject);
		}
	}

	public void SetVolume(string volume)
	{
		SetVolume(float.Parse(volume));
	}

	public void SetVolume(float volume)
	{
		m_sndInst.Volume = volume;
	}
}
