using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Force")]
public class DetonatorForce : DetonatorComponent
{
	private float _baseRadius = 50f;

	private float _basePower = 4000f;

	private float _scaledRange;

	private float _scaledIntensity;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float radius;

	public float power;

	public GameObject fireObject;

	public float fireObjectLife;

	private Collider[] _colliders;

	private GameObject _tempFireObject;

	private Vector3 _explosionPosition;

	public override void Init()
	{
	}

	private void Update()
	{
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
		if (!on || detailThreshold > detail)
		{
			return;
		}
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			_explosionPosition = base.transform.position;
			_colliders = Physics.OverlapSphere(_explosionPosition, radius);
			Collider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				if (!collider || !collider.GetComponent<Rigidbody>())
				{
					continue;
				}
				collider.GetComponent<Rigidbody>().AddExplosionForce(power * size, _explosionPosition, radius * size, 4f * MyDetonator().upwardsBias * size);
				SendMessage("OnDetonatorForceHit", null, SendMessageOptions.DontRequireReceiver);
				if ((bool)fireObject)
				{
					if ((bool)collider.transform.Find(fireObject.name + "(Clone)"))
					{
						return;
					}
					_tempFireObject = Object.Instantiate(fireObject, base.transform.position, base.transform.rotation) as GameObject;
					_tempFireObject.transform.parent = collider.transform;
					_tempFireObject.transform.localPosition = new Vector3(0f, 0f, 0f);
					if ((bool)_tempFireObject.GetComponent<ParticleEmitter>())
					{
						_tempFireObject.GetComponent<ParticleEmitter>().emit = true;
						Object.Destroy(_tempFireObject, fireObjectLife);
					}
				}
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
		radius = _baseRadius;
		power = _basePower;
	}
}
