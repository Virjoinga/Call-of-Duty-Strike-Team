using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BreathManager : MonoBehaviour
{
	[Serializable]
	public class BreathStateData
	{
		public float m_playbackSpeed;

		public float m_startSpeed;
	}

	private ParticleSystem m_particleSystem;

	[SerializeField]
	private BreathStateData m_normal;

	[SerializeField]
	private BreathStateData m_heavy;

	[SerializeField]
	private BreathStateData m_running;

	private void Awake()
	{
		m_particleSystem = GetComponent<ParticleSystem>();
	}

	public void GotoState(BreathState _targetState, float _duration)
	{
		switch (_targetState)
		{
		case BreathState.Normal:
			StopAllCoroutines();
			StartCoroutine(StateTransition(m_normal, _duration));
			break;
		case BreathState.Heavy:
			StopAllCoroutines();
			StartCoroutine(StateTransition(m_heavy, _duration));
			break;
		case BreathState.Running:
			StopAllCoroutines();
			StartCoroutine(StateTransition(m_running, _duration));
			break;
		}
	}

	private IEnumerator StateTransition(BreathStateData _targetStateData, float _duration)
	{
		float lerpScale = 1f / _duration;
		float lerp = 0f;
		float initialPlaybackSpeed = m_particleSystem.playbackSpeed;
		float initialStartSpeed = m_particleSystem.startSpeed;
		while (true)
		{
			yield return null;
			lerp += Time.deltaTime * lerpScale;
			if (lerp > 1f)
			{
				break;
			}
			m_particleSystem.playbackSpeed = Mathf.Lerp(initialPlaybackSpeed, _targetStateData.m_playbackSpeed, lerp);
			m_particleSystem.startSpeed = Mathf.Lerp(initialStartSpeed, _targetStateData.m_startSpeed, lerp);
		}
		m_particleSystem.playbackSpeed = _targetStateData.m_playbackSpeed;
		m_particleSystem.startSpeed = _targetStateData.m_startSpeed;
	}
}
