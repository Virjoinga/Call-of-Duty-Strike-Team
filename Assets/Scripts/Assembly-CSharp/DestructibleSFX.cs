using System;

[Serializable]
public class DestructibleSFX
{
	public SFXBank SoundBank;

	public string PlayFunction;

	public float m_HealthToTrigger;

	private bool m_Actived;

	private bool m_Expired;

	public bool Actived
	{
		get
		{
			return m_Actived;
		}
		set
		{
			m_Actived = value;
		}
	}

	public bool Expired
	{
		get
		{
			return m_Expired;
		}
		set
		{
			m_Expired = value;
		}
	}
}
