using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGunfire : MonoBehaviour
{
	public bool OnMessageOnly;

	public int MinimumBurstLength = 3;

	public int MaximumBurstLength = 5;

	public float MinimumTimeBetweenBurstShots = 0.05f;

	public float MaximumTimeBetweenBurstShots = 0.05f;

	public float MinimumTimeBetweenBursts = 1f;

	public float MaximumTimeBetweenBursts = 2f;

	public int NumberOfShotsBetweenTracers;

	public float TracerSpeed = 100f;

	public float MaximumRange = 1000f;

	public int NumberOfImpactPoints = 16;

	public float ImpactPointSpread = 1f;

	public SurfaceImpact[] Impacts = new SurfaceImpact[0];

	private void OnEnable()
	{
		StartCoroutine(SpawnRandomGunfire());
	}

	private IEnumerator SpawnRandomGunfire()
	{
		while (true)
		{
			if (OnMessageOnly || Impacts.Length == 0)
			{
				yield return new WaitForSeconds(1f);
				continue;
			}
			yield return new WaitForSeconds(Random.Range(MinimumTimeBetweenBursts, MaximumTimeBetweenBursts));
			int burstLength = Random.Range(MinimumBurstLength, MaximumBurstLength);
			int tracerCountdown = 0;
			for (int i = 0; i < burstLength; i++)
			{
				bool tracer = tracerCountdown == 0;
				FireSingleShot(base.transform.position, tracer);
				tracerCountdown = ((tracerCountdown != 0) ? (tracerCountdown - 1) : NumberOfShotsBetweenTracers);
				yield return new WaitForSeconds(Random.Range(MinimumTimeBetweenBurstShots, MaximumTimeBetweenBurstShots));
			}
		}
	}

	public void FireSingleShot(Vector3 muzzlePosition, bool tracer)
	{
		SurfaceImpact surfaceImpact = Impacts[Random.Range(0, Impacts.Length)];
		WeaponSFX.Instance.LSATFire.Play(base.gameObject);
		if (tracer)
		{
			EffectsController.Instance.GetTracer(muzzlePosition, surfaceImpact.position, EffectsController.TracerType.Enemy, surfaceImpact, TracerSpeed, true);
		}
		else
		{
			EffectsController.Instance.TriggerSurfaceImpact(surfaceImpact);
		}
	}

	public void GenerateImpactPoints()
	{
		List<SurfaceImpact> list = new List<SurfaceImpact>();
		for (int i = 0; i < NumberOfImpactPoints; i++)
		{
			SurfaceImpact surfaceImpact = GenerateImpactPoint(base.transform.position, base.transform.forward + ImpactPointSpread * 0.05f * Random.insideUnitSphere);
			if (surfaceImpact != null)
			{
				list.Add(surfaceImpact);
			}
		}
		Impacts = list.ToArray();
	}

	public SurfaceImpact GenerateImpactPoint(Vector3 origin, Vector3 direction)
	{
		return ProjectileManager.Trace(origin, origin + MaximumRange * direction, ProjectileManager.ProjectileMask);
	}
}
