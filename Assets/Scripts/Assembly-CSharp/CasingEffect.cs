using UnityEngine;

public class CasingEffect
{
	private ParticleEmitter mEmitter;

	public CasingEffect(ParticleEmitter emitter)
	{
		mEmitter = emitter;
	}

	public void Fire(bool playSound)
	{
		Fire(playSound, Vector3.zero);
	}

	public void Fire(bool playSound, Vector3 velocity)
	{
		if (mEmitter != null)
		{
			mEmitter.worldVelocity = velocity;
			mEmitter.Emit();
			if (playSound)
			{
				SoundManager.Instance.PlayWeaponCasingSfx(mEmitter.gameObject.transform.position, mEmitter.worldVelocity);
			}
		}
	}
}
