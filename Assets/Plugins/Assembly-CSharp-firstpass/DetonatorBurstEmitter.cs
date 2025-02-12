using UnityEngine;

public class DetonatorBurstEmitter : DetonatorComponent
{
	private ParticleEmitter _particleEmitter;

	private ParticleRenderer _particleRenderer;

	private ParticleAnimator _particleAnimator;

	private float _baseDamping = 0.1300004f;

	private float _baseSize = 1f;

	private Color _baseColor = Color.white;

	public float damping = 1f;

	public float startRadius = 1f;

	public float maxScreenSize = 2f;

	public bool explodeOnAwake;

	public bool oneShot = true;

	public float sizeVariation;

	public float particleSize = 1f;

	public float count = 1f;

	public float sizeGrow = 20f;

	public bool exponentialGrowth = true;

	public float durationVariation;

	public bool useWorldSpace = true;

	public float upwardsBias;

	public float angularVelocity = 20f;

	public bool randomRotation = true;

	public ParticleRenderMode renderMode;

	public bool useExplicitColorAnimation;

	public Color[] colorAnimation = new Color[5];

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public Material material;

	private float _emitTime;

	private float speed = 3f;

	private float initFraction = 0.1f;

	private static float epsilon = 0.01f;

	private float _tmpParticleSize;

	private Vector3 _tmpPos;

	private Vector3 _tmpDir;

	private Vector3 _thisPos;

	private float _tmpDuration;

	private float _tmpCount;

	private float _scaledDuration;

	private float _scaledDurationVariation;

	private float _scaledStartRadius;

	private float _scaledColor;

	private float _randomizedRotation;

	private float _tmpAngularVelocity;

	public override void Init()
	{
		MonoBehaviour.print("UNUSED");
	}

	public void Awake()
	{
		_particleEmitter = base.gameObject.AddComponent<EllipsoidParticleEmitter>() as ParticleEmitter;
		_particleRenderer = base.gameObject.AddComponent<ParticleRenderer>() as ParticleRenderer;
		_particleAnimator = base.gameObject.AddComponent<ParticleAnimator>() as ParticleAnimator;
		_particleEmitter.hideFlags = HideFlags.HideAndDontSave;
		_particleRenderer.hideFlags = HideFlags.HideAndDontSave;
		_particleAnimator.hideFlags = HideFlags.HideAndDontSave;
		_particleAnimator.damping = _baseDamping;
		_particleEmitter.emit = false;
		_particleRenderer.maxParticleSize = maxScreenSize;
		_particleRenderer.material = material;
		_particleRenderer.material.color = Color.white;
		_particleAnimator.sizeGrow = sizeGrow;
		if (explodeOnAwake)
		{
			Explode();
		}
	}

	private void Update()
	{
		if (exponentialGrowth)
		{
			float num = Time.time - _emitTime;
			float num2 = SizeFunction(num - epsilon);
			float num3 = SizeFunction(num);
			float num4 = (num3 / num2 - 1f) / epsilon;
			_particleAnimator.sizeGrow = num4;
		}
		else
		{
			_particleAnimator.sizeGrow = sizeGrow;
		}
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}

	private float SizeFunction(float elapsedTime)
	{
		float num = 1f - 1f / (1f + elapsedTime * speed);
		return initFraction + (1f - initFraction) * num;
	}

	public void Reset()
	{
		size = _baseSize;
		color = _baseColor;
		damping = _baseDamping;
	}

	public override void Explode()
	{
		if (!on)
		{
			return;
		}
		_particleEmitter.useWorldSpace = useWorldSpace;
		_scaledDuration = timeScale * duration;
		_scaledDurationVariation = timeScale * durationVariation;
		_scaledStartRadius = size * startRadius;
		_particleRenderer.particleRenderMode = renderMode;
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			Color[] array = _particleAnimator.colorAnimation;
			if (useExplicitColorAnimation)
			{
				array[0] = colorAnimation[0];
				array[1] = colorAnimation[1];
				array[2] = colorAnimation[2];
				array[3] = colorAnimation[3];
				array[4] = colorAnimation[4];
			}
			else
			{
				array[0] = new Color(color.r, color.g, color.b, color.a * 0.7f);
				array[1] = new Color(color.r, color.g, color.b, color.a * 1f);
				array[2] = new Color(color.r, color.g, color.b, color.a * 0.5f);
				array[3] = new Color(color.r, color.g, color.b, color.a * 0.3f);
				array[4] = new Color(color.r, color.g, color.b, color.a * 0f);
			}
			_particleAnimator.colorAnimation = array;
			_particleRenderer.material = material;
			_particleAnimator.force = force;
			_tmpCount = count * detail;
			if (_tmpCount < 1f)
			{
				_tmpCount = 1f;
			}
			if (_particleEmitter.useWorldSpace)
			{
				_thisPos = base.gameObject.transform.position;
			}
			else
			{
				_thisPos = new Vector3(0f, 0f, 0f);
			}
			for (int i = 1; (float)i <= _tmpCount; i++)
			{
				_tmpPos = Vector3.Scale(Random.insideUnitSphere, new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius));
				_tmpPos = _thisPos + _tmpPos;
				_tmpDir = Vector3.Scale(Random.insideUnitSphere, new Vector3(velocity.x, velocity.y, velocity.z));
				_tmpDir.y += 2f * (Mathf.Abs(_tmpDir.y) * upwardsBias);
				if (randomRotation)
				{
					_randomizedRotation = Random.Range(-1f, 1f);
					_tmpAngularVelocity = Random.Range(-1f, 1f) * angularVelocity;
				}
				else
				{
					_randomizedRotation = 0f;
					_tmpAngularVelocity = angularVelocity;
				}
				_tmpDir = Vector3.Scale(_tmpDir, new Vector3(size, size, size));
				_tmpParticleSize = size * (particleSize + Random.value * sizeVariation);
				_tmpDuration = _scaledDuration + Random.value * _scaledDurationVariation;
				_particleEmitter.Emit(_tmpPos, _tmpDir, _tmpParticleSize, _tmpDuration, color, _randomizedRotation, _tmpAngularVelocity);
			}
			_emitTime = Time.time;
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}
}
