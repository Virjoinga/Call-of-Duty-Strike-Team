using System;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
	public enum ExplosionType
	{
		Grenade = 0,
		DoorBreach = 1,
		Claymore = 2,
		Overwatch = 3,
		RPG = 4,
		Crockery = 5,
		Glass = 6,
		Vehicle = 7,
		MonitorGlass = 8,
		Barrel = 9,
		Helicopter = 10,
		FakeExplosion = 11
	}

	[Serializable]
	public class SurfaceSpecificPrefab
	{
		public SurfaceMaterial Surface;

		public GameObject Prefab;
	}

	private static ExplosionManager smInstance = null;

	public SurfaceSpecificPrefab[] GrenadeExplosions;

	public SurfaceSpecificPrefab[] FakeExplosions;

	public GameObject DoorBreachExplosionPrefab;

	public GameObject CrockeryExplosionPrefab;

	public GameObject GlassExplosionPrefab;

	public GameObject VehicleExplosionPrefab;

	public GameObject HelicopterExplosionPrefab;

	public GameObject MonitorGlassExplosionPrefab;

	public GameObject BarrelExplosionPrefab;

	public GameObject RPGExplosionPrefab;

	public GameObject Claymore;

	public GameObject Grenade;

	public GameObject C4;

	public GameObject RPG;

	public GameObject RPGOverwatch;

	[SerializeField]
	private GameObject m_impactEffectManagerPrefab;

	private static ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private List<Explosion> mExplosions;

	private GameObject[] mGrenadeExplosions;

	private GameObject[] mFakeExplosions;

	private ImpactEffectManager m_impactEffectManager;

	public static ExplosionManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple ExplosionManager");
		}
		smInstance = this;
		mExplosions = new List<Explosion>();
	}

	private void Start()
	{
		ResetState();
		int num = (int)(EnumUtils.GetMaxValue<SurfaceMaterial>() + 1);
		mGrenadeExplosions = new GameObject[num];
		SurfaceSpecificPrefab[] grenadeExplosions = GrenadeExplosions;
		foreach (SurfaceSpecificPrefab surfaceSpecificPrefab in grenadeExplosions)
		{
			mGrenadeExplosions[(int)surfaceSpecificPrefab.Surface] = surfaceSpecificPrefab.Prefab;
		}
		mFakeExplosions = new GameObject[num];
		if (FakeExplosions != null)
		{
			SurfaceSpecificPrefab[] fakeExplosions = FakeExplosions;
			foreach (SurfaceSpecificPrefab surfaceSpecificPrefab2 in fakeExplosions)
			{
				mFakeExplosions[(int)surfaceSpecificPrefab2.Surface] = surfaceSpecificPrefab2.Prefab;
			}
		}
		if (m_impactEffectManagerPrefab != null)
		{
			if (ImpactEffectManager.Instance == null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_impactEffectManagerPrefab);
				m_impactEffectManager = gameObject.GetComponent<ImpactEffectManager>();
			}
			else
			{
				m_impactEffectManager = ImpactEffectManager.Instance;
			}
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void Update()
	{
		for (int num = mExplosions.Count - 1; num >= 0; num--)
		{
			mExplosions[num].Update();
			if (mExplosions[num].HasExpired())
			{
				mExplosions.RemoveAt(num);
			}
		}
	}

	public void ResetState()
	{
		mExplosions.Clear();
	}

	public void GlDebugVisualise()
	{
		GL.PushMatrix();
		DebugDraw.LineMaterial.SetPass(0);
		GL.Begin(1);
		foreach (Explosion mExplosion in mExplosions)
		{
			mExplosion.GLDebugVisualise();
		}
		GL.End();
		GL.PopMatrix();
	}

	public void StartExplosion(Vector3 origin, float radius)
	{
		StartExplosion(origin, radius, ExplosionType.Grenade);
	}

	public void StartExplosion(Vector3 origin, float radius, ExplosionType expType)
	{
		Explosion item = new Explosion(origin, radius);
		GameObject gameObject = null;
		switch (expType)
		{
		case ExplosionType.Grenade:
		{
			SurfaceImpact surfaceImpact2 = ProjectileManager.Trace(origin, origin + Vector3.down, ProjectileManager.ProjectileMask);
			SurfaceMaterial surfaceMaterial2 = ((!((surfaceImpact2.position - origin).sqrMagnitude > 1f)) ? surfaceImpact2.material : SurfaceMaterial.None);
			bool flag2 = EmitGrenadeEffect(origin, surfaceImpact2.normal, surfaceMaterial2);
			if (!flag2)
			{
				GameObject fakeExplosionForSurface = GetGrenadeExplosionForSurface(surfaceMaterial2);
				if (fakeExplosionForSurface != null)
				{
					gameObject = UnityEngine.Object.Instantiate(fakeExplosionForSurface, origin, Quaternion.identity) as GameObject;
					flag2 = true;
				}
			}
			else
			{
				GameObject gameObject3 = new GameObject("OffScrExp");
				gameObject3.transform.position = origin;
				UnityEngine.Object.Destroy(gameObject3, 2f);
				ExplosivesSFX.Instance.GrenadeExplosion.Play(gameObject3);
			}
			if (flag2)
			{
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		}
		case ExplosionType.DoorBreach:
			if (DoorBreachExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(DoorBreachExplosionPrefab, origin, Quaternion.identity) as GameObject;
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		case ExplosionType.Claymore:
		{
			SurfaceImpact surfaceImpact3 = ProjectileManager.Trace(origin, origin + Vector3.down, ProjectileManager.ProjectileMask);
			SurfaceMaterial surfaceMaterial3 = ((!((surfaceImpact3.position - origin).sqrMagnitude > 1f)) ? surfaceImpact3.material : SurfaceMaterial.None);
			GameObject fakeExplosionForSurface = GetGrenadeExplosionForSurface(surfaceMaterial3);
			if (fakeExplosionForSurface != null)
			{
				gameObject = UnityEngine.Object.Instantiate(fakeExplosionForSurface, origin, Quaternion.identity) as GameObject;
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		}
		case ExplosionType.RPG:
			if (RPGExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(RPGExplosionPrefab, origin, Quaternion.identity) as GameObject;
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		case ExplosionType.Crockery:
			if (CrockeryExplosionPrefab != null)
			{
				gameObject = SceneNanny.Instantiate(CrockeryExplosionPrefab) as GameObject;
				gameObject.transform.position = origin;
			}
			break;
		case ExplosionType.Glass:
			if (GlassExplosionPrefab != null)
			{
				gameObject = SceneNanny.Instantiate(GlassExplosionPrefab) as GameObject;
				gameObject.transform.position = origin;
			}
			break;
		case ExplosionType.Vehicle:
			if (VehicleExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(VehicleExplosionPrefab, origin, Quaternion.identity) as GameObject;
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		case ExplosionType.MonitorGlass:
			if (MonitorGlassExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(MonitorGlassExplosionPrefab, origin, Quaternion.identity) as GameObject;
			}
			break;
		case ExplosionType.Barrel:
			if (BarrelExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(BarrelExplosionPrefab, origin, Quaternion.identity) as GameObject;
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		case ExplosionType.Helicopter:
			if (HelicopterExplosionPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(HelicopterExplosionPrefab, origin, Quaternion.identity) as GameObject;
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		case ExplosionType.FakeExplosion:
		{
			SurfaceImpact surfaceImpact = ProjectileManager.Trace(origin, origin + Vector3.down, ProjectileManager.ProjectileMask);
			SurfaceMaterial surfaceMaterial = ((!((surfaceImpact.position - origin).sqrMagnitude > 1f)) ? surfaceImpact.material : SurfaceMaterial.None);
			bool flag = EmitGrenadeEffect(origin, surfaceImpact.normal, surfaceMaterial);
			if (!flag)
			{
				GameObject fakeExplosionForSurface = GetFakeExplosionForSurface(surfaceMaterial);
				if (fakeExplosionForSurface != null)
				{
					gameObject = UnityEngine.Object.Instantiate(fakeExplosionForSurface, origin, Quaternion.identity) as GameObject;
					flag = true;
				}
			}
			else
			{
				GameObject gameObject2 = new GameObject("OffScrExp");
				gameObject2.transform.position = origin;
				UnityEngine.Object.Destroy(gameObject2, 2f);
				ExplosivesSFX.Instance.ExplodeMortar.Play(gameObject2);
			}
			if (flag)
			{
				DecalManager.Instance.AddToFloor(origin, DecalManager.DecalType.GrenadeExplosion);
				CameraManager.Instance.AddExplosionShake(origin, radius);
			}
			break;
		}
		default:
			TBFAssert.DoAssert(false, "no known explosion type?");
			break;
		}
		if (gameObject != null)
		{
			AutoDestroyExplosion(gameObject);
		}
		mExplosions.Add(item);
	}

	private void AutoDestroyExplosion(GameObject explosion)
	{
		ParticleSystem componentInChildren = explosion.GetComponentInChildren<ParticleSystem>();
		if (componentInChildren != null)
		{
			UnityEngine.Object.Destroy(explosion, componentInChildren.duration);
		}
	}

	public static void BroadcastNoise(Vector3 origin, Actor owner)
	{
		if (owner == null)
		{
			ActorIdentIterator aii = myActorIdentIterator.ResetWithMask(GKM.ActorsInPlay);
			BroadcastNoise(origin, owner, aii);
		}
		else
		{
			ActorIdentIterator aii2 = myActorIdentIterator.ResetWithMask(GKM.EnemiesMask(owner) & GKM.AliveMask);
			BroadcastNoise(origin, owner, aii2);
		}
	}

	private static void BroadcastNoise(Vector3 origin, Actor owner, ActorIdentIterator aii)
	{
		Actor a;
		while (aii.NextActor(out a))
		{
			if (a.ears == null || !a.ears.CanHear || !AwarenessZone.IsUnregisteredGameObjectAwarenessInSync(origin, a.gameObject.transform.position))
			{
				continue;
			}
			float magnitude = (origin - a.GetPosition()).magnitude;
			if (magnitude < AudioResponseRanges.Explosion + a.ears.Range)
			{
				if (owner == null)
				{
					a.behaviour.BlameEnemyForEvent(origin);
				}
				else
				{
					a.awareness.BecomeAware(owner, origin);
				}
			}
		}
	}

	private bool EmitGrenadeEffect(Vector3 origin, Vector3 normal, SurfaceMaterial material)
	{
		if (m_impactEffectManager == null)
		{
			return false;
		}
		ImpactEffectManager.ImpactType impactType = ImpactEffectManager.ImpactType.Default;
		switch (material)
		{
		case SurfaceMaterial.Cement:
			impactType = ImpactEffectManager.ImpactType.ConcreteGrenade;
			break;
		case SurfaceMaterial.Water:
			impactType = ImpactEffectManager.ImpactType.WaterGrenade;
			break;
		case SurfaceMaterial.Snow:
			impactType = ImpactEffectManager.ImpactType.SnowGrenade;
			break;
		case SurfaceMaterial.Sand:
			impactType = ImpactEffectManager.ImpactType.SandGrenade;
			break;
		case SurfaceMaterial.Dirt:
			impactType = ImpactEffectManager.ImpactType.DirtGrenade;
			break;
		default:
			return false;
		}
		if (CameraManager.Instance != null && CameraManager.Instance.CurrentCamera != null)
		{
			Vector3 vector = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(origin);
			if (!(vector.x > 0f) || !(vector.x < 1f) || !(vector.y > 0f) || !(vector.y < 1f) || !(vector.z > 0f))
			{
				return true;
			}
		}
		m_impactEffectManager.Emit(origin, normal, impactType);
		return true;
	}

	private GameObject GetGrenadeExplosionForSurface(SurfaceMaterial surfaceMaterial)
	{
		return mGrenadeExplosions[(int)surfaceMaterial] ?? mGrenadeExplosions[0];
	}

	private GameObject GetFakeExplosionForSurface(SurfaceMaterial surfaceMaterial)
	{
		return mFakeExplosions[(int)surfaceMaterial] ?? mFakeExplosions[0];
	}
}
