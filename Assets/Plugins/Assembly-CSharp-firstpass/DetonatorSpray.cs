using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Object Spray")]
public class DetonatorSpray : DetonatorComponent
{
	public GameObject sprayObject;

	public int count = 10;

	public float startingRadius;

	public float minScale = 1f;

	public float maxScale = 1f;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	private Vector3 _explosionPosition;

	private float _tmpScale;

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
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			int num = (int)(detail * (float)count);
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = Random.onUnitSphere * (startingRadius * size);
				Vector3 b = new Vector3(velocity.x * size, velocity.y * size, velocity.z * size);
				GameObject gameObject = Object.Instantiate(sprayObject, base.transform.position + vector, base.transform.rotation) as GameObject;
				gameObject.transform.parent = base.transform;
				_tmpScale = minScale + Random.value * (maxScale - minScale);
				_tmpScale *= size;
				gameObject.transform.localScale = new Vector3(_tmpScale, _tmpScale, _tmpScale);
				gameObject.GetComponent<Rigidbody>().velocity = Vector3.Scale(vector.normalized, b);
				Object.Destroy(gameObject, duration * timeScale);
				_delayedExplosionStarted = false;
				_explodeDelay = 0f;
			}
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
		velocity = new Vector3(15f, 15f, 15f);
	}
}
