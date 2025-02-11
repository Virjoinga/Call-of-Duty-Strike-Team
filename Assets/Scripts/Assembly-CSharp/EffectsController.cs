using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
	public enum TracerType
	{
		None = 0,
		Friendly = 1,
		Enemy = 2,
		Minigun = 3
	}

	public class EffectPool
	{
		private GameObject template;

		private GameObject[] pool;

		public EffectPool(GameObject original)
		{
			template = original;
			pool = new GameObject[4];
		}

		public GameObject Request()
		{
			GameObject gameObject = null;
			if (pool != null)
			{
				for (int i = 0; i < pool.GetLength(0); i++)
				{
					if (pool[i] != null)
					{
						gameObject = pool[i];
						pool[i] = null;
						return gameObject;
					}
				}
			}
			return (GameObject)UnityEngine.Object.Instantiate(template);
		}

		public void Replace(GameObject obj)
		{
			obj.transform.position = new Vector3(0f, 5000f, 0f);
			for (int i = 0; i < pool.GetLength(0); i++)
			{
				if (pool[i] == null)
				{
					pool[i] = obj;
					return;
				}
			}
			GameObject[] array = new GameObject[pool.GetLength(0) + 4];
			Array.Copy(pool, array, pool.GetLength(0));
			array[pool.GetLength(0)] = obj;
			pool = array;
		}
	}

	public EffectsList Effects;

	private EffectPool BloodPool_pool;

	private EffectPool BloodSpray_pool;

	private EffectPool MasonrySpray_pool;

	private EffectPool MetalSpray_pool;

	private EffectPool WoodSpray_pool;

	private EffectPool TracerFire_pool;

	private ImpactEffectManager m_impactEffectManager;

	[SerializeField]
	private GameObject m_impactEffectManagerPrefab;

	[SerializeField]
	private bool m_useImpactEffectManager = true;

	private static EffectsController smInstance;

	public static EffectsController Instance
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
			throw new Exception("Can not have multiple EffectsControllers");
		}
		smInstance = this;
		BloodPool_pool = new EffectPool(Effects.BloodPool);
		GameObject[] array = new GameObject[10];
		for (int i = 0; i < 10; i++)
		{
			array[i] = BloodPool_pool.Request();
		}
		for (int j = 0; j < 10; j++)
		{
			BloodPool_pool.Replace(array[j]);
		}
		BloodSpray_pool = new EffectPool(Effects.BloodSpray);
		MasonrySpray_pool = new EffectPool(Effects.MasonrySpray);
		MetalSpray_pool = new EffectPool(Effects.MetalSpray);
		WoodSpray_pool = new EffectPool(Effects.WoodSpray);
		TracerFire_pool = new EffectPool(Effects.TracerFire);
		if (m_useImpactEffectManager && m_impactEffectManagerPrefab != null)
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
		if (m_impactEffectManager != null)
		{
			UnityEngine.Object.Destroy(m_impactEffectManager);
			m_impactEffectManager = null;
		}
		smInstance = null;
	}

	public GameObject GetBloodPool(Vector3 origin)
	{
		GameObject gameObject = BloodPool_pool.Request();
		gameObject.transform.position = origin;
		gameObject.transform.particleSystem.Play();
		float seconds = gameObject.transform.particleSystem.duration + gameObject.transform.particleSystem.startLifetime;
		StartCoroutine(PoolGameObjectAfter(BloodPool_pool, gameObject, seconds));
		return gameObject;
	}

	public void TriggerSurfaceImpact(SurfaceImpact impact)
	{
		TriggerSFX(impact);
		if (CameraManager.Instance != null && CameraManager.Instance.CurrentCamera != null)
		{
			Vector3 vector = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(impact.position);
			if (!(vector.x > 0f) || !(vector.x < 1f) || !(vector.y > 0f) || !(vector.y < 1f) || !(vector.z > 0f) || Vector3.Dot(CameraManager.Instance.CurrentCamera.transform.forward, impact.direction) > 0f)
			{
				return;
			}
		}
		if (OverwatchController.Instance != null && OverwatchController.Instance.Active && !OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.GByteDevice))
		{
			return;
		}
		switch (impact.material)
		{
		case SurfaceMaterial.None:
			break;
		case SurfaceMaterial.Metal:
			if (!Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Metal, DecalManager.DecalType.BulletMetal, impact.noDecal))
			{
				GetMetalSpray(impact.position, impact.direction, impact.normal);
			}
			break;
		case SurfaceMaterial.Wood:
			if (!Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Wood, DecalManager.DecalType.BulletGeneric, impact.noDecal))
			{
				GetWoodSpray(impact.position, impact.direction, impact.normal);
			}
			break;
		case SurfaceMaterial.Flesh:
			GetBloodSpray(impact.position, impact.direction);
			break;
		case SurfaceMaterial.Snow:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Snow, DecalManager.DecalType.BulletSnow, impact.noDecal);
			break;
		case SurfaceMaterial.Masonry:
			if (!Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Masonry, DecalManager.DecalType.BulletConcrete, impact.noDecal))
			{
				GetMasonrySpray(impact.position, impact.direction, impact.normal);
			}
			break;
		case SurfaceMaterial.Water:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Water);
			break;
		case SurfaceMaterial.Carpet:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Carpet, DecalManager.DecalType.BulletGeneric, impact.noDecal);
			break;
		case SurfaceMaterial.Dirt:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Dirt, DecalManager.DecalType.BulletGeneric, impact.noDecal);
			break;
		case SurfaceMaterial.Mud:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Dirt, DecalManager.DecalType.BulletGeneric, impact.noDecal);
			break;
		case SurfaceMaterial.Cement:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Cement, DecalManager.DecalType.BulletGeneric, impact.noDecal);
			break;
		case SurfaceMaterial.Sand:
			Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Sand, DecalManager.DecalType.BulletGeneric, impact.noDecal);
			break;
		default:
			if (!Emit(impact.position, impact.normal, ImpactEffectManager.ImpactType.Default, DecalManager.DecalType.BulletConcrete, impact.noDecal))
			{
				GetMasonrySpray(impact.position, impact.direction, impact.normal);
			}
			break;
		}
	}

	private void TriggerSFX(SurfaceImpact impact)
	{
		float num = Vector3.Dot(impact.direction, impact.normal);
		SoundFXData soundFXData = null;
		switch (impact.material)
		{
		case SurfaceMaterial.Metal:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitMetal : ImpactSFX.Instance.BulletRicochetMetal);
			break;
		case SurfaceMaterial.Wood:
		case SurfaceMaterial.Mud:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitWood : ImpactSFX.Instance.BulletRicochetWood);
			break;
		case SurfaceMaterial.Flesh:
			soundFXData = ImpactSFX.Instance.BulletHitBody;
			break;
		case SurfaceMaterial.Snow:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitSnow : ImpactSFX.Instance.BulletRicochetSnow);
			break;
		case SurfaceMaterial.Ice:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitIce : ImpactSFX.Instance.BulletRicochetIce);
			break;
		case SurfaceMaterial.Water:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitWater : ImpactSFX.Instance.BulletRicochetWater);
			break;
		case SurfaceMaterial.Duct:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitDuct : ImpactSFX.Instance.BulletRicochetDuct);
			break;
		case SurfaceMaterial.Carpet:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitCarpet : ImpactSFX.Instance.BulletRicochetCarpet);
			break;
		case SurfaceMaterial.Dirt:
		case SurfaceMaterial.Grass:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitDirt : ImpactSFX.Instance.BulletRicochetDirt);
			break;
		case SurfaceMaterial.Glass:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitGlass : ImpactSFX.Instance.BulletRicochetGlass);
			break;
		case SurfaceMaterial.Leaves:
			soundFXData = ImpactSFX.Instance.BulletHitFoliage;
			break;
		case SurfaceMaterial.Sand:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitSand : ImpactSFX.Instance.BulletRicochetSand);
			break;
		default:
			soundFXData = ((!(num < 0.5f)) ? ImpactSFX.Instance.BulletHitMasonry : ImpactSFX.Instance.BulletRicochetMasonry);
			break;
		case SurfaceMaterial.None:
			break;
		}
		if (soundFXData != null)
		{
			SoundManager.Instance.PlaySpotSfxAtPosition(soundFXData, impact.position);
		}
	}

	public GameObject GetBloodSpray(Vector3 origin, Vector3 direction)
	{
		GameObject gameObject = BloodSpray_pool.Request();
		gameObject.transform.position = origin;
		gameObject.transform.forward = direction;
		gameObject.transform.particleSystem.Play();
		StartCoroutine(PoolGameObjectAfter(BloodSpray_pool, gameObject, gameObject.transform.particleSystem.duration));
		return gameObject;
	}

	private GameObject GetMasonrySpray(Vector3 origin, Vector3 direction, Vector3 normal)
	{
		GameObject gameObject = MasonrySpray_pool.Request();
		gameObject.transform.position = origin;
		gameObject.transform.forward = normal;
		gameObject.transform.particleSystem.Play(true);
		StartCoroutine(PoolGameObjectAfter(MasonrySpray_pool, gameObject, gameObject.transform.particleSystem.duration));
		return gameObject;
	}

	private void EmitMasonrySprayImpactEffect(Vector3 origin, Vector3 normal)
	{
		m_impactEffectManager.Emit(origin, normal, ImpactEffectManager.ImpactType.Masonry);
	}

	private bool Emit(Vector3 origin, Vector3 normal, ImpactEffectManager.ImpactType _effectType)
	{
		if (!m_useImpactEffectManager)
		{
			return false;
		}
		m_impactEffectManager.Emit(origin, normal, _effectType);
		return true;
	}

	private bool Emit(Vector3 origin, Vector3 normal, ImpactEffectManager.ImpactType _effectType, DecalManager.DecalType _decalType, bool noDecal)
	{
		if (!noDecal)
		{
			DecalManager.Instance.Add(origin, normal, _decalType);
		}
		if (!m_useImpactEffectManager)
		{
			return false;
		}
		m_impactEffectManager.Emit(origin, normal, _effectType);
		return true;
	}

	private GameObject GetMetalSpray(Vector3 origin, Vector3 direction, Vector3 normal)
	{
		GameObject gameObject = MetalSpray_pool.Request();
		gameObject.transform.position = origin;
		gameObject.transform.forward = normal;
		gameObject.transform.particleSystem.Play(true);
		if (Vector3.Dot(direction, normal) < 0.5f)
		{
			ImpactSFX.Instance.BulletRicochetMetal.Play(gameObject);
		}
		else
		{
			ImpactSFX.Instance.BulletHitMetal.Play(gameObject);
		}
		StartCoroutine(PoolGameObjectAfter(MetalSpray_pool, gameObject, gameObject.transform.particleSystem.duration));
		return gameObject;
	}

	private void EmitMetalSprayImpactEffect(Vector3 origin, Vector3 normal)
	{
		m_impactEffectManager.Emit(origin, normal, ImpactEffectManager.ImpactType.Metal);
	}

	private GameObject GetWoodSpray(Vector3 origin, Vector3 direction, Vector3 normal)
	{
		GameObject gameObject = WoodSpray_pool.Request();
		gameObject.transform.position = origin;
		gameObject.transform.forward = normal;
		gameObject.transform.particleSystem.Play(true);
		StartCoroutine(PoolGameObjectAfter(WoodSpray_pool, gameObject, gameObject.transform.particleSystem.duration));
		return gameObject;
	}

	private void EmitWoodSprayImpactEffect(Vector3 origin, Vector3 normal)
	{
		m_impactEffectManager.Emit(origin, normal, ImpactEffectManager.ImpactType.Wood);
	}

	private void EmitSnowSprayImpactEffect(Vector3 origin, Vector3 normal)
	{
		m_impactEffectManager.Emit(origin, normal, ImpactEffectManager.ImpactType.Snow);
	}

	public GameObject GetBulletEject()
	{
		return UnityEngine.Object.Instantiate(Effects.BulletEject) as GameObject;
	}

	public GameObject GetMuzzleFlash()
	{
		return UnityEngine.Object.Instantiate(Effects.MuzzleFlash) as GameObject;
	}

	public GameObject GetTracer(Vector3 origin, Vector3 target, TracerType type, SurfaceImpact impact, bool allowAudio)
	{
		GameObject gameObject = TracerFire_pool.Request();
		Tracer component = gameObject.GetComponent<Tracer>();
		component.Fire(origin, target, GetTracerMaterialForType(type), impact, allowAudio);
		return gameObject;
	}

	public GameObject GetTracer(Vector3 origin, Vector3 target, TracerType type, SurfaceImpact impact, float speed, bool allowAudio)
	{
		GameObject gameObject = TracerFire_pool.Request();
		Tracer component = gameObject.GetComponent<Tracer>();
		component.Fire(origin, target, GetTracerMaterialForType(type), impact, speed, allowAudio);
		return gameObject;
	}

	public void ReplaceTracer(GameObject tracer)
	{
		TracerFire_pool.Replace(tracer);
		LineRenderer component = tracer.GetComponent<LineRenderer>();
		component.enabled = false;
	}

	public GameObject GetRangeRingProjector()
	{
		return UnityEngine.Object.Instantiate(Effects.RangeRingProjector) as GameObject;
	}

	public BlobShadow AddBlobShadow(GameObject source, GameObject actorBase, bool useFootShadows)
	{
		GameObject gameObject = SceneNanny.Instantiate(Effects.BlobShadow) as GameObject;
		BlobShadow blobShadow = gameObject.AddComponent<BlobShadow>();
		blobShadow.transform.localScale = new Vector3(1f, 1.2f, 1f);
		blobShadow.ShadowCasterRoot = source.transform;
		blobShadow.ShadowCaster = source.transform.FindInHierarchy("Bip002");
		blobShadow.ActorBase = actorBase;
		return blobShadow;
	}

	public HaloEffect AddHalo(GameObject obj, Transform attachPoint)
	{
		HaloEffect haloEffect = obj.AddComponent<HaloEffect>();
		haloEffect.AttachPoint = attachPoint;
		haloEffect.HaloModel = Effects.HaloModel;
		return haloEffect;
	}

	public ICollection<Material> Fade(IEnumerable<Renderer> renderers)
	{
		List<Material> list = new List<Material>();
		foreach (Renderer renderer in renderers)
		{
			Material[] array = new Material[renderer.materials.Length];
			for (int i = 0; i < renderer.materials.Length; i++)
			{
				Material material = renderer.materials[i];
				Shader fadingShader = Instance.GetFadingShader(material.shader);
				if (fadingShader != null)
				{
					Material material2 = new Material(material);
					material2.shader = fadingShader;
					array[i] = material2;
					list.Add(material2);
				}
				else
				{
					Debug.Log("Could not get fading shader for " + material.shader.name);
					array[i] = material;
				}
			}
			renderer.materials = array;
		}
		return list;
	}

	public Shader GetFadingShader(Shader opaqueShader)
	{
		FadingShader[] fadingShaders = Effects.FadingShaders;
		foreach (FadingShader fadingShader in fadingShaders)
		{
			if (fadingShader.OpaqueShader == opaqueShader)
			{
				return fadingShader.TransparentShader;
			}
		}
		return null;
	}

	private IEnumerator DestroyGameObjectAfter(GameObject gameObject, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		UnityEngine.Object.Destroy(gameObject);
	}

	private IEnumerator PoolGameObjectAfter(EffectPool ep, GameObject gameObject, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		ep.Replace(gameObject);
	}

	private Material GetTracerMaterialForType(TracerType type)
	{
		switch (type)
		{
		case TracerType.Friendly:
			return Effects.FriendlyTracer;
		case TracerType.Enemy:
			return Effects.EnemyTracer;
		case TracerType.Minigun:
			return Effects.MinigunTracer;
		default:
			return Effects.EnemyTracer;
		}
	}
}
