using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectManager : MonoBehaviour
{
	public enum ImpactType
	{
		Metal = 0,
		Sand = 1,
		Masonry = 2,
		Wood = 3,
		Snow = 4,
		Water = 5,
		WaterGrenade = 6,
		Glass = 7,
		GlassReverse = 8,
		Carpet = 9,
		Cement = 10,
		Dirt = 11,
		GlassBulletProof = 12,
		Default = 13,
		SnowGrenade = 14,
		ConcreteGrenade = 15,
		SandGrenade = 16,
		DirtGrenade = 17
	}

	private class ParticleSystemData
	{
		public ParticleSystem m_rootSystem;

		public ParticleSystem[] m_systems;

		public Color[] m_startColours;

		public bool[] m_applyLighting;
	}

	public static ImpactEffectManager Instance = null;

	private static float[] s_lightProbeData = new float[27];

	private GameObject[] m_effectGO;

	private ParticleSystemData[] m_effectParticleSystem;

	private Mesh[] m_impactDecalMeshes;

	private List<Matrix4x4>[] m_decals;

	public GameObject[] m_effectPrefab;

	public Material m_impactDecalMaterial;

	public Rect[] m_impactDecalUVs;

	public float m_decalSize = 0.1f;

	public bool m_disableDecal;

	public bool m_disableLighting;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Danger Will Robinson, Trying to create another impact effect manager!");
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Start()
	{
		m_effectGO = new GameObject[m_effectPrefab.Length];
		m_effectParticleSystem = new ParticleSystemData[m_effectPrefab.Length];
		for (int i = 0; i < m_effectPrefab.Length; i++)
		{
			m_effectGO[i] = Object.Instantiate(m_effectPrefab[i]) as GameObject;
			m_effectParticleSystem[i] = new ParticleSystemData();
			m_effectParticleSystem[i].m_rootSystem = m_effectGO[i].GetComponent<ParticleSystem>();
			ParticleSystem[] componentsInChildren = m_effectGO[i].GetComponentsInChildren<ParticleSystem>();
			m_effectParticleSystem[i].m_systems = componentsInChildren;
			m_effectParticleSystem[i].m_startColours = new Color[componentsInChildren.Length];
			m_effectParticleSystem[i].m_applyLighting = new bool[componentsInChildren.Length];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				m_effectParticleSystem[i].m_startColours[j] = componentsInChildren[j].startColor;
				bool flag = true;
				ParticleLightingMarkup component = componentsInChildren[j].GetComponent<ParticleLightingMarkup>();
				if (component != null)
				{
					flag = component.ApplyLighting;
				}
				m_effectParticleSystem[i].m_applyLighting[j] = flag;
				componentsInChildren[j].loop = false;
				componentsInChildren[j].playOnAwake = false;
			}
		}
	}

	public void Emit(Vector3 _position, Vector3 _normal)
	{
		Emit(_position, _normal, ImpactType.Default);
	}

	public void Emit(Vector3 _position, Vector3 _normal, ImpactType _impactType)
	{
		Vector3 position = _position + _normal * 0.1f;
		ParticleSystemData particleSystemData = m_effectParticleSystem[(int)_impactType];
		if (UseLighting(_impactType) && LightmapSettings.lightProbes != null)
		{
			LightProbes lightProbes = LightmapSettings.lightProbes;
			Color color = Color.white;
			if (lightProbes != null)
			{
				LightmapSettings.lightProbes.GetInterpolatedLightProbe(position, null, s_lightProbeData);
				color.r = s_lightProbeData[0];
				color.g = s_lightProbeData[1];
				color.b = s_lightProbeData[2];
				color += RenderSettings.ambientLight;
				color = ColorUtils.ClampColor(color);
			}
			for (int i = 0; i < particleSystemData.m_systems.Length; i++)
			{
				if (particleSystemData.m_applyLighting[i])
				{
					Color color2 = particleSystemData.m_startColours[i];
					color2 *= 0.5f;
					Color startColor = color2 + color2 * color;
					particleSystemData.m_systems[i].startColor = startColor;
				}
			}
		}
		Quaternion rotation = Quaternion.LookRotation(GetNormal(_normal, _impactType));
		m_effectGO[(int)_impactType].transform.position = position;
		m_effectGO[(int)_impactType].transform.rotation = rotation;
		particleSystemData.m_rootSystem.Play(true);
	}

	private bool UseDecal(ImpactType _impactType)
	{
		switch (_impactType)
		{
		case ImpactType.Water:
		case ImpactType.WaterGrenade:
		case ImpactType.Glass:
		case ImpactType.GlassReverse:
			return false;
		default:
			return !m_disableDecal;
		}
	}

	private bool UseLighting(ImpactType _impactType)
	{
		if (_impactType == ImpactType.Metal)
		{
			return false;
		}
		return !m_disableLighting;
	}

	private Vector3 GetNormal(Vector3 _normal, ImpactType _impactType)
	{
		if (_impactType == ImpactType.GlassReverse)
		{
			return -_normal;
		}
		return _normal;
	}

	private static Vector3 ShadeSH9(Vector3 n, float[] c)
	{
		float num = n.x * 0.325735f;
		float num2 = n.y * 0.325735f;
		float num3 = n.z * 0.325735f;
		float num4 = n.x * n.y * 0.27313712f;
		float num5 = n.y * n.z * 0.27313712f;
		float num6 = n.z * n.x * 0.27313712f;
		float num7 = n.z * n.z * 0.23654369f;
		float num8 = (n.x * n.x - n.y * n.y) * 0.1628675f;
		Vector3 result = default(Vector3);
		result.x = 0.2820948f * c[0] - 0.07884789f * c[18] - c[9] * num - c[3] * num2 + c[6] * num3 + (c[12] * num4 - c[15] * num5 - c[21] * num6) + (c[18] * num7 + c[24] * num8);
		result.y = 0.2820948f * c[1] - 0.07884789f * c[19] - c[10] * num - c[4] * num2 + c[7] * num3 + (c[13] * num4 - c[16] * num5 - c[22] * num6) + (c[19] * num7 + c[25] * num8);
		result.z = 0.2820948f * c[2] - 0.07884789f * c[20] - c[11] * num - c[5] * num2 + c[8] * num3 + (c[14] * num4 - c[17] * num5 - c[23] * num6) + (c[20] * num7 + c[26] * num8);
		return result;
	}
}
