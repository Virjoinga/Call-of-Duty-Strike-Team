using System.Reflection;
using UnityEngine;

public class ParticleSFX : MonoBehaviour
{
	public enum ConditionType
	{
		NumParticlesBiggerThanLastNumParticles = 0,
		LastNumParticlesIsZeroAndCurrentIsNonZero = 1
	}

	private const float mDistanceToAllowSound = 20f;

	public SFXBank SoundBank;

	public string PlayFunction;

	public bool PlayOnceWhenFirstEmit;

	public ConditionType PlayCondition;

	private ParticleSystem mParticleSystem;

	private int mLastNumParticles;

	private bool mHasPlayedOnce;

	public void Awake()
	{
		mParticleSystem = base.gameObject.GetComponent<ParticleSystem>();
	}

	public void OnDestroy()
	{
		SoundFXData soundData = GetSoundData(SoundBank, PlayFunction, true);
		if (soundData != null && mHasPlayedOnce)
		{
			SoundManager.Instance.Stop(soundData, base.gameObject);
		}
	}

	public void Start()
	{
	}

	public void Update()
	{
		if (!mParticleSystem || (PlayOnceWhenFirstEmit && (!PlayOnceWhenFirstEmit || mHasPlayedOnce)) || (!IsCloseToCamera() && !PlayOnceWhenFirstEmit))
		{
			return;
		}
		int particleCount = mParticleSystem.particleCount;
		if (ShouldPlaySound(particleCount))
		{
			SoundFXData soundData = GetSoundData(SoundBank, PlayFunction, false);
			if (soundData != null)
			{
				SoundManager.Instance.Play(soundData, base.gameObject);
				mHasPlayedOnce = true;
			}
		}
		mLastNumParticles = particleCount;
	}

	private bool ShouldPlaySound(int numParticles)
	{
		bool result = false;
		switch (PlayCondition)
		{
		case ConditionType.NumParticlesBiggerThanLastNumParticles:
			result = numParticles > mLastNumParticles;
			break;
		case ConditionType.LastNumParticlesIsZeroAndCurrentIsNonZero:
			result = mLastNumParticles == 0 && numParticles != 0;
			break;
		}
		return result;
	}

	private bool IsCloseToCamera()
	{
		bool result = false;
		if (CameraManager.Instance != null && CameraManager.Instance.CurrentCamera != null && (base.transform.position - CameraManager.Instance.CurrentCamera.transform.position).sqrMagnitude < 20f)
		{
			result = true;
		}
		return result;
	}

	private SoundFXData GetSoundData(SFXBank soundBank, string playFunction, bool fromDestroy)
	{
		SoundFXData result = null;
		SFXBank sFXBank = null;
		if (soundBank != null && playFunction != null)
		{
			if (fromDestroy)
			{
				PropertyInfo property = soundBank.GetType().GetProperty("HasInstance");
				if (property != null && property.GetValue(null, null) as bool? == false)
				{
					return null;
				}
			}
			PropertyInfo property2 = soundBank.GetType().GetProperty("Instance");
			if (property2 != null)
			{
				sFXBank = property2.GetValue(null, null) as SFXBank;
				if (sFXBank != null)
				{
					result = sFXBank.GetSFXDataFromName(playFunction);
				}
			}
		}
		return result;
	}
}
