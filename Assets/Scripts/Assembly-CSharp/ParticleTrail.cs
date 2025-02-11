using UnityEngine;

public class ParticleTrail : MonoBehaviour
{
	private struct TrailParticle
	{
		public Vector3 m_position;

		public Vector3 m_velocity;

		public Color m_colour;

		public float m_life;
	}

	private Transform m_transform;

	private MeshRenderer m_meshRenderer;

	private MeshFilter m_meshFilter;

	private Mesh m_mesh;

	private TrailParticle[] m_particles;

	private int m_firstParticle;

	private int m_activeParticleCount;

	private Vector3[] m_vertices;

	private Vector3[] m_normals;

	private Vector2[] m_uvs;

	private Color32[] m_colours;

	private float m_emitTime;

	private Color32 m_litColour;

	[SerializeField]
	private float m_emitInterval = 0.1f;

	[SerializeField]
	private float m_emitSpeed = 0.1f;

	[SerializeField]
	private float m_lifeTime = 1f;

	[SerializeField]
	private float m_gravityScale = -0.1f;

	[SerializeField]
	private float m_dragFactor = 0.1f;

	[SerializeField]
	private Color m_colour = Color.gray;

	[SerializeField]
	private AnimationCurve m_alphaCurve;

	[SerializeField]
	private Material m_material;

	[SerializeField]
	private int m_maxParticles = 32;

	[SerializeField]
	private bool m_sampleLightProbes = true;

	[SerializeField]
	private float m_lightProbeUpdateInterval = 0.1f;

	[SerializeField]
	private Renderer m_lightProbeHint;

	private static float[] s_sampleBuffer = new float[27];

	private void Awake()
	{
		m_transform = base.transform;
		m_meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		m_meshRenderer.material = m_material;
		m_meshRenderer.castShadows = false;
		m_meshRenderer.receiveShadows = false;
		m_meshRenderer.enabled = false;
		m_mesh = new Mesh();
		m_mesh.name = "ParticleTrail";
		m_mesh.MarkDynamic();
		m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
		m_meshFilter.mesh = m_mesh;
		m_particles = new TrailParticle[m_maxParticles];
		int num = m_maxParticles * 2;
		m_vertices = new Vector3[num];
		m_normals = new Vector3[num];
		m_uvs = new Vector2[num];
		m_colours = new Color32[num];
		m_mesh.vertices = m_vertices;
		m_mesh.normals = m_normals;
		m_mesh.uv = m_uvs;
		m_mesh.colors32 = m_colours;
		int num2 = m_maxParticles - 1;
		int num3 = num2 * 6;
		int[] array = new int[num3];
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < num2; i++)
		{
			array[num5] = num4;
			array[num5 + 1] = num4 + 1;
			array[num5 + 2] = num4 + 2;
			array[num5 + 3] = num4 + 2;
			array[num5 + 4] = num4 + 1;
			array[num5 + 5] = num4 + 3;
			num4 += 2;
			num5 += 6;
		}
		m_mesh.triangles = array;
		m_litColour = m_colour;
	}

	private void OnDestroy()
	{
		if (m_meshRenderer != null)
		{
			Object.Destroy(m_meshRenderer);
			m_meshRenderer = null;
		}
		if (m_meshFilter != null)
		{
			Object.Destroy(m_meshFilter);
			m_meshFilter = null;
		}
		if (m_mesh != null)
		{
			Object.Destroy(m_mesh);
			m_mesh = null;
		}
	}

	private void OnEnable()
	{
		m_activeParticleCount = 0;
		if (m_sampleLightProbes)
		{
			SampleProbes();
			if (m_lightProbeUpdateInterval > float.Epsilon)
			{
				InvokeRepeating("SampleProbes", m_lightProbeUpdateInterval, m_lightProbeUpdateInterval);
			}
		}
		UpdateMesh();
	}

	private void OnDisable()
	{
		m_activeParticleCount = 0;
		CancelInvoke();
		UpdateMesh();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (deltaTime > float.Epsilon)
		{
			UpdateParticles(deltaTime);
			UpdateMesh();
		}
	}

	private void UpdateParticles(float _deltaTime)
	{
		m_emitTime += _deltaTime;
		if (m_emitTime > m_emitInterval)
		{
			m_emitTime = Mathf.Repeat(m_emitTime, m_emitInterval);
			m_firstParticle = Repeat(m_firstParticle - 1, m_maxParticles);
			m_particles[m_firstParticle].m_position = m_transform.position;
			m_particles[m_firstParticle].m_velocity = Random.insideUnitSphere * m_emitSpeed;
			m_particles[m_firstParticle].m_colour = m_litColour;
			m_particles[m_firstParticle].m_life = 0f;
			if (m_activeParticleCount < m_maxParticles)
			{
				m_activeParticleCount++;
			}
		}
		float num = Mathf.Pow(m_dragFactor, _deltaTime);
		Vector3 vector = Physics.gravity * m_gravityScale * _deltaTime;
		float num2 = _deltaTime / m_lifeTime;
		for (int i = 0; i < m_activeParticleCount; i++)
		{
			int num3 = Repeat(m_firstParticle + i, m_maxParticles);
			Vector3 position = m_particles[num3].m_position;
			Vector3 velocity = m_particles[num3].m_velocity;
			float life = m_particles[num3].m_life;
			velocity = velocity * num + vector;
			position += velocity * _deltaTime;
			life += num2;
			m_particles[num3].m_position = position;
			m_particles[num3].m_velocity = velocity;
			m_particles[num3].m_life = life;
			if (life > 1f)
			{
				m_activeParticleCount = i;
				break;
			}
		}
	}

	private void UpdateMesh()
	{
		if (m_activeParticleCount < 2)
		{
			m_meshRenderer.enabled = false;
			return;
		}
		m_meshRenderer.enabled = true;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		int i = 0;
		float num = 0f;
		for (; i < m_activeParticleCount; i++)
		{
			int num2 = Repeat(m_firstParticle + i, m_maxParticles);
			Vector3 vector3 = m_transform.InverseTransformPoint(m_particles[num2].m_position);
			Color colour = m_particles[num2].m_colour;
			colour.a *= m_alphaCurve.Evaluate(m_particles[num2].m_life);
			Vector3 vector4 = vector3 - vector;
			Color32 color = colour;
			int num3 = i * 2;
			m_vertices[num3] = vector3;
			m_normals[num3] = vector4;
			m_uvs[num3] = new Vector2(0f, num);
			m_colours[num3] = color;
			int num4 = num3 + 1;
			m_vertices[num4] = vector3;
			m_normals[num4] = vector4;
			m_uvs[num4] = new Vector2(1f, num);
			m_colours[num4] = color;
			vector = vector3;
			vector2 = vector4;
			num += 1f;
		}
		Color32 color2 = new Color32(0, 0, 0, 0);
		for (; i < m_maxParticles; i++)
		{
			int num5 = i * 2;
			m_vertices[num5] = vector;
			m_normals[num5] = vector2;
			m_colours[num5] = color2;
			int num6 = num5 + 1;
			m_vertices[num6] = vector;
			m_normals[num6] = vector2;
			m_colours[num6] = color2;
		}
		m_mesh.vertices = m_vertices;
		m_mesh.normals = m_normals;
		m_mesh.uv = m_uvs;
		m_mesh.colors32 = m_colours;
		m_mesh.RecalculateBounds();
	}

	private void SampleProbes()
	{
		LightProbes lightProbes = LightmapSettings.lightProbes;
		if (lightProbes != null)
		{
			lightProbes.GetInterpolatedLightProbe(m_transform.position, m_lightProbeHint, s_sampleBuffer);
			Color colour = m_colour;
			Color ambientLight = RenderSettings.ambientLight;
			colour.r *= s_sampleBuffer[0] + ambientLight.r;
			colour.g *= s_sampleBuffer[1] + ambientLight.g;
			colour.b *= s_sampleBuffer[2] + ambientLight.b;
			colour = ColorUtils.ClampColor(colour);
			m_litColour = colour;
		}
	}

	private static int Repeat(int _i, int _length)
	{
		int num = _i % _length;
		return (num >= 0) ? num : (_length + num);
	}
}
