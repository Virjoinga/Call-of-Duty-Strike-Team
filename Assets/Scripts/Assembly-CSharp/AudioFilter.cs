using System;
using UnityEngine;

[Serializable]
public class AudioFilter : MonoBehaviour
{
	public enum AudioFilterType
	{
		Default = 0,
		EnvironmentEcho = 1,
		LowPass = 2,
		NumFilters = 3
	}

	public AudioFilterType m_AudioFilterType;

	public float m_Delay = 500f;

	public float m_DecayRatio = 0.5f;

	public float m_WetMix = 1f;

	public float m_DryMix = 1f;

	public float m_CutoffFrequency = 5000f;

	public float m_LowpassResonaceQ = 1f;
}
