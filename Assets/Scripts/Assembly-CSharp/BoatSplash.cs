using UnityEngine;

public class BoatSplash : MonoBehaviour
{
	private Transform m_transform;

	private Material m_material;

	private Vector3 m_lastPosition;

	private float m_lastSpeed;

	private Color m_initialTint;

	private bool m_particlesEnabled;

	[SerializeField]
	private Renderer m_rendererA;

	[SerializeField]
	private Renderer m_rendererB;

	[SerializeField]
	private ParticleSystem[] m_particleSystems;

	[SerializeField]
	private float m_speedParticleCutoff = 3f;

	[SerializeField]
	private float m_speedIntensityScale = 0.1f;

	[SerializeField]
	private float m_speedIntensityCutoff = 3f;

	[SerializeField]
	private float m_speedScrollScale = -0.15f;

	[SerializeField]
	private float m_speedScrollCutoff = 2f;

	private void Awake()
	{
		m_transform = base.transform;
	}

	private void Start()
	{
		if (m_rendererA.sharedMaterial != m_rendererB.sharedMaterial)
		{
			Debug.LogError("Renderer A and Renderer B should use the same material");
		}
		m_material = new Material(m_rendererA.sharedMaterial);
		m_rendererA.sharedMaterial = m_material;
		m_rendererB.sharedMaterial = m_material;
		ParticleSystem[] particleSystems = m_particleSystems;
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			particleSystem.enableEmission = false;
		}
		m_particlesEnabled = false;
		m_initialTint = m_material.GetColor("_TintColor");
	}

	private void OnEnable()
	{
		m_lastPosition = m_transform.position;
	}

	private void OnDestroy()
	{
		if (m_material != null)
		{
			Object.DestroyImmediate(m_material);
			m_material = null;
		}
	}

	private void LateUpdate()
	{
		Vector3 position = m_transform.position;
		float deltaTime = Time.deltaTime;
		float num = ((!(deltaTime > float.Epsilon)) ? m_lastSpeed : ((position - m_lastPosition).magnitude / deltaTime));
		bool flag = num > m_speedParticleCutoff;
		if (m_particlesEnabled != flag)
		{
			ParticleSystem[] particleSystems = m_particleSystems;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				particleSystem.enableEmission = flag;
			}
			m_particlesEnabled = flag;
		}
		float a = Mathf.Max(0f, num - m_speedIntensityCutoff) * m_speedIntensityScale;
		Color color = m_initialTint * new Color(1f, 1f, 1f, a);
		m_material.SetColor("_TintColor", color);
		Vector3 vector = m_material.mainTextureOffset;
		float num2 = Mathf.Max(0f, num - m_speedScrollCutoff) * m_speedScrollScale * deltaTime;
		vector.x = Mathf.Repeat(vector.x + num2, 1f);
		m_material.mainTextureOffset = vector;
		m_lastPosition = position;
		m_lastSpeed = num;
	}
}
