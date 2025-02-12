using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Light")]
public class DetonatorLight : DetonatorComponent
{
	private float _baseIntensity = 1f;

	private Color _baseColor = Color.white;

	private float _scaledDuration;

	private float _explodeTime = -1000f;

	private GameObject _light;

	private Light _lightComponent;

	public float intensity;

	private float _reduceAmount;

	public override void Init()
	{
		_light = new GameObject("Light");
		_light.transform.parent = base.transform;
		_light.transform.localPosition = localPosition;
		_lightComponent = (Light)_light.AddComponent<Light>();
		_lightComponent.type = LightType.Point;
		_lightComponent.enabled = false;
	}

	private void Update()
	{
		if (_explodeTime + _scaledDuration > Time.time && _lightComponent.intensity > 0f)
		{
			_reduceAmount = intensity * (Time.deltaTime / _scaledDuration);
			_lightComponent.intensity -= _reduceAmount;
		}
		else if ((bool)_lightComponent)
		{
			_lightComponent.enabled = false;
		}
	}

	public override void Explode()
	{
		if (!(detailThreshold > detail))
		{
			_lightComponent.color = color;
			_lightComponent.range = size * 50f;
			_scaledDuration = duration * timeScale;
			_lightComponent.enabled = true;
			_lightComponent.intensity = intensity;
			_explodeTime = Time.time;
		}
	}

	public void Reset()
	{
		color = _baseColor;
		intensity = _baseIntensity;
	}
}
