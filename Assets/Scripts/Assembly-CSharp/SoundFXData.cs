using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundFXData
{
	public enum SoundChoiceBehaviour
	{
		Sequential = 0,
		Random = 1,
		RandomNoRepeat = 2
	}

	public enum VolumeGroup
	{
		Sfx = 0,
		Cutscene = 1,
		FrontEnd = 2,
		Music = 3,
		Indoor_Ambience = 4,
		Outdoor_Ambience = 5,
		BattleChatterWalla = 6,
		VO = 7,
		FrontEnd_Music = 8,
		MaxVolGroups = 9
	}

	public SoundChoiceBehaviour m_nextSoundChoice = SoundChoiceBehaviour.Random;

	public List<AudioClip> m_audioSourceData = new List<AudioClip>();

	public VolumeGroup VolGroup;

	public bool m_bypassEffects;

	public bool m_loop;

	public int m_priority = 128;

	public float m_volume = 1f;

	public float m_volumeVariance;

	public float m_pitch = 1f;

	public float m_pitchVariance;

	public bool m_mute;

	public bool m_playOnAwake = true;

	public int m_maxInstances = 1;

	public float m_timeBetweenDuplicatePlay = 0.1f;

	public bool m_play3D;

	public float m_panLevel = 1f;

	public float m_spread;

	public float m_dopplerLevel;

	public float m_minDistance = 5f;

	public float m_maxDistance = 50f;

	public AudioRolloffMode m_rollOffMode = AudioRolloffMode.Linear;

	public float m_pan2D;

	public SoundManager.SoundInstance Play(GameObject go)
	{
		return SoundManager.Instance.Play(this, go);
	}

	public SoundManager.SoundInstance Play(GameObject go, AudioFilter filter)
	{
		return SoundManager.Instance.Play(this, go, filter);
	}

	public SoundManager.SoundInstance Play2D()
	{
		return SoundManager.Instance.Play2D(this);
	}

	public void Stop(GameObject go)
	{
		SoundManager.Instance.Stop(this, go);
	}

	public void Stop2D()
	{
		SoundManager.Instance.Stop2D(this);
	}

	public void StopAfterLoop(GameObject go)
	{
		SoundManager.Instance.StopAfterLoop(this, go);
	}

	public void StopAll()
	{
		SoundManager.Instance.StopAll(this);
	}

	public void StopAllAndCleanup()
	{
		SoundManager.Instance.StopAllAndCleanup(this);
	}

	public void Fade(GameObject go, float duration, float desiredVolume, bool destroyAfterFade)
	{
		SoundManager.Instance.Fade(go, this, duration, desiredVolume, destroyAfterFade);
	}
}
