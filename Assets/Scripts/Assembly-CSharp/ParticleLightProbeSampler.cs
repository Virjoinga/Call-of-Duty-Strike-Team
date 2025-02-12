using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleLightProbeSampler : MonoBehaviour
{
	private Transform m_transform;

	private ParticleSystem m_particleSystem;

	private Color m_initialColour;

	[SerializeField]
	private bool m_enableUpdating = true;

	[SerializeField]
	private float m_updateInterval = 0.1f;

	[SerializeField]
	private Renderer m_lightProbeHint;

	private static float[] s_sampleBuffer = new float[27];

	private void Awake()
	{
		m_transform = base.transform;
		m_particleSystem = GetComponent<ParticleSystem>();
	}

	private void Start()
	{
		m_initialColour = m_particleSystem.startColor;
		SampleProbes();
		if (m_enableUpdating)
		{
			InvokeRepeating("SampleProbes", m_updateInterval, m_updateInterval);
		}
	}

	private void SampleProbes()
	{
		LightProbes lightProbes = LightmapSettings.lightProbes;
		if (lightProbes != null)
		{
			//lightProbes.GetInterpolatedProbe(m_transform.position, m_lightProbeHint, s_sampleBuffer);
			Color initialColour = m_initialColour;
			Color ambientLight = RenderSettings.ambientLight;
			initialColour.r *= s_sampleBuffer[0] + ambientLight.r;
			initialColour.g *= s_sampleBuffer[1] + ambientLight.g;
			initialColour.b *= s_sampleBuffer[2] + ambientLight.b;
			initialColour = ColorUtils.ClampColor(initialColour);
			m_particleSystem.startColor = initialColour;
		}
	}
}
