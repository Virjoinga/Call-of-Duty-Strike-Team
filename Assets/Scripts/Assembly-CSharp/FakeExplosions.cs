using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeExplosions : MonoBehaviour
{
	public float MinimumTimeBetweenExplosions = 5f;

	public float MaximumTimeBetweenExplosions = 10f;

	public int NumberOfImpactPoints = 16;

	public float ImpactPointSpread = 1f;

	public SurfaceImpact[] Impacts = new SurfaceImpact[0];

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private static float kSqrSafeRadius = 49f;

	private static float kSqrToFarRadius = 8000f;

	private void OnEnable()
	{
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.FakeExplosions))
		{
			StartCoroutine(SpawnRandomExplosions());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator SpawnRandomExplosions()
	{
		while (SceneLoader.IsTeleporting)
		{
			yield return null;
		}
		while (SectionManager.GetSectionManager() != null && !SectionManager.GetSectionManager().SectionActivated)
		{
			yield return null;
		}
		if (GameController.Instance == null || !GameController.Instance.GameplayStarted)
		{
			yield return null;
		}
		while (true)
		{
			if (Impacts.Length == 0 || ExplosionManager.Instance == null)
			{
				yield return new WaitForSeconds(1f);
				continue;
			}
			bool isSafePlaceForeExplosion = true;
			SurfaceImpact explosionPoint = Impacts[Random.Range(0, Impacts.Length)];
			if (CameraManager.Instance != null && CameraManager.Instance.CurrentCamera != null && (explosionPoint.position - CameraManager.Instance.CurrentCamera.transform.position).sqrMagnitude > kSqrToFarRadius)
			{
				isSafePlaceForeExplosion = false;
			}
			if (isSafePlaceForeExplosion)
			{
				ActorIdentIterator aii = myActorIdentIterator.ResetWithMask(GKM.ActorsInPlay);
				Actor a;
				while (aii.NextActor(out a))
				{
					if (a != null && (explosionPoint.position - a.GetPosition()).sqrMagnitude <= kSqrSafeRadius)
					{
						isSafePlaceForeExplosion = false;
						break;
					}
				}
			}
			if (isSafePlaceForeExplosion)
			{
				float delay = Random.Range(MinimumTimeBetweenExplosions, MaximumTimeBetweenExplosions);
				float mortarLength = 0.5f;
				float mortarDelay = delay - mortarLength;
				StartCoroutine(DoWhistleSFX(mortarDelay));
				StartCoroutine(DoExplosion(delay, explosionPoint));
				yield return new WaitForSeconds(delay);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
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
		float num = 1000f;
		SurfaceImpact surfaceImpact = ProjectileManager.Trace(origin, origin + num * direction, ProjectileManager.ProjectileMask);
		return (surfaceImpact.material != SurfaceMaterial.None) ? surfaceImpact : null;
	}

	public IEnumerator DoWhistleSFX(float delay)
	{
		float delayed = 0f;
		while (delayed < delay)
		{
			delayed += Time.deltaTime;
			yield return null;
		}
		WeaponSFX.Instance.MortarWhistle.Play(base.gameObject);
	}

	public IEnumerator DoExplosion(float delay, SurfaceImpact explosionPoint)
	{
		float delayed = 0f;
		while (delayed < delay)
		{
			delayed += Time.deltaTime;
			yield return null;
		}
		float normalOffset = 0.01f;
		ExplosionManager.Instance.StartExplosion(explosionPoint.position + normalOffset * explosionPoint.normal, 50f, ExplosionManager.ExplosionType.FakeExplosion);
	}
}
