using UnityEngine;

public class WindBlownSnow : MonoBehaviour
{
	private Vector4 m_windSnowTime;

	public Color m_windSnowColour = Color.white;

	public Material m_flatMaterial;

	public Material m_sideMaterial;

	public Transform m_windDirection;

	private void Start()
	{
		SetConstantGlobals();
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_windSnowTime.x += deltaTime * 0.1f;
		if (m_windSnowTime.x >= 1f)
		{
			m_windSnowTime.x -= 1f;
		}
		m_windSnowTime.y += deltaTime * 0.2f;
		if (m_windSnowTime.y >= 1f)
		{
			m_windSnowTime.y -= 1f;
		}
		m_windSnowTime.z += deltaTime * 0.4f;
		if (m_windSnowTime.z >= 1f)
		{
			m_windSnowTime.z -= 1f;
		}
		m_windSnowTime.w = m_windSnowTime.x;
		Shader.SetGlobalVector("g_windSnowData", m_windSnowTime);
	}

	private void SetConstantGlobals()
	{
		Shader.SetGlobalColor("g_windSnowColour", m_windSnowColour);
	}
}
