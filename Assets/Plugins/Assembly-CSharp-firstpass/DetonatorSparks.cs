using UnityEngine;

[AddComponentMenu("Detonator/Sparks")]
[RequireComponent(typeof(Detonator))]
public class DetonatorSparks : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 4f;

	private Vector3 _baseVelocity = new Vector3(155f, 155f, 155f);

	private Color _baseColor = Color.white;

	private Vector3 _baseForce = Physics.gravity;

	private float _scaledDuration;

	private GameObject _sparks;

	private DetonatorBurstEmitter _sparksEmitter;

	public Material sparksMaterial;

	public override void Init()
	{
		FillMaterials(false);
		BuildSparks();
	}

	public void FillMaterials(bool wipe)
	{
		if (!sparksMaterial || wipe)
		{
			sparksMaterial = MyDetonator().sparksMaterial;
		}
	}

	public void BuildSparks()
	{
		_sparks = new GameObject("Sparks");
		_sparksEmitter = (DetonatorBurstEmitter)_sparks.AddComponent<DetonatorBurstEmitter>();
		_sparks.transform.parent = base.transform;
		_sparks.transform.localPosition = localPosition;
		_sparks.transform.localRotation = Quaternion.identity;
		_sparksEmitter.material = sparksMaterial;
		_sparksEmitter.force = Physics.gravity / 3f;
		_sparksEmitter.useExplicitColorAnimation = false;
		_sparksEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_sparksEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateSparks()
	{
		_scaledDuration = duration * timeScale;
		_sparksEmitter.color = color;
		_sparksEmitter.duration = _scaledDuration / 4f;
		_sparksEmitter.durationVariation = _scaledDuration;
		_sparksEmitter.count = (int)(detail * 50f);
		_sparksEmitter.particleSize = 0.5f;
		_sparksEmitter.sizeVariation = 0.25f;
		if (_sparksEmitter.upwardsBias > 0f)
		{
			_sparksEmitter.velocity = new Vector3(velocity.x / Mathf.Log(_sparksEmitter.upwardsBias), velocity.y * Mathf.Log(_sparksEmitter.upwardsBias), velocity.z / Mathf.Log(_sparksEmitter.upwardsBias));
		}
		else
		{
			_sparksEmitter.velocity = velocity;
		}
		_sparksEmitter.startRadius = 0f;
		_sparksEmitter.size = size;
		_sparksEmitter.explodeDelayMin = explodeDelayMin;
		_sparksEmitter.explodeDelayMax = explodeDelayMax;
	}

	public void Reset()
	{
		FillMaterials(true);
		on = true;
		size = _baseSize;
		duration = _baseDuration;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
		velocity = _baseVelocity;
		force = _baseForce;
	}

	public override void Explode()
	{
		if (on)
		{
			UpdateSparks();
			_sparksEmitter.Explode();
		}
	}
}
