using System;
using UnityEngine;

[Serializable]
public class Detonator_0020Spray_0020Helper : MonoBehaviour
{
	public float startTimeMin;

	public float startTimeMax;

	public float stopTimeMin;

	public float stopTimeMax;

	public Material firstMaterial;

	public Material secondMaterial;

	private float startTime;

	private float stopTime;

	private float spawnTime;

	private bool isReallyOn;

	public Detonator_0020Spray_0020Helper()
	{
		stopTimeMin = 10f;
		stopTimeMax = 10f;
	}

	public virtual void Start()
	{
		isReallyOn = GetComponent<ParticleEmitter>().emit;
		GetComponent<ParticleEmitter>().emit = false;
		spawnTime = Time.time;
		startTime = UnityEngine.Random.value * (startTimeMax - startTimeMin) + startTimeMin + Time.time;
		stopTime = UnityEngine.Random.value * (stopTimeMax - stopTimeMin) + stopTimeMin + Time.time;
		if (!(UnityEngine.Random.value <= 0.5f))
		{
			GetComponent<Renderer>().material = firstMaterial;
		}
		else
		{
			GetComponent<Renderer>().material = secondMaterial;
		}
	}

	public virtual void FixedUpdate()
	{
		if (!(Time.time <= startTime))
		{
			GetComponent<ParticleEmitter>().emit = isReallyOn;
		}
		if (!(Time.time <= stopTime))
		{
			GetComponent<ParticleEmitter>().emit = false;
		}
	}

	public virtual void Main()
	{
	}
}
