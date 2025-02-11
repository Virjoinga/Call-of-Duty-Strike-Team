using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Sound")]
public class DetonatorSound : DetonatorComponent
{
	public AudioClip[] nearSounds;

	public AudioClip[] farSounds;

	public float distanceThreshold = 50f;

	public float minVolume = 0.4f;

	public float maxVolume = 1f;

	public float rolloffFactor = 0.5f;

	private AudioSource _soundComponent;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	private int _idx;

	public override void Init()
	{
		_soundComponent = (AudioSource)base.gameObject.AddComponent("AudioSource");
	}

	private void Update()
	{
		_soundComponent.pitch = Time.timeScale;
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}

	public override void Explode()
	{
		if (detailThreshold > detail)
		{
			return;
		}
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			if (Vector3.Distance(Camera.main.transform.position, base.transform.position) < distanceThreshold)
			{
				_idx = (int)(Random.value * (float)nearSounds.Length);
				_soundComponent.PlayOneShot(nearSounds[_idx]);
			}
			else
			{
				_idx = (int)(Random.value * (float)farSounds.Length);
				_soundComponent.PlayOneShot(farSounds[_idx]);
			}
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
	}
}
