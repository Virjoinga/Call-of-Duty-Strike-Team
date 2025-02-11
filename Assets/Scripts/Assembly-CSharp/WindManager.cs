using UnityEngine;

public class WindManager : MonoBehaviour
{
	private float m_windSpeed = 1f;

	private float m_windDirTime;

	private float m_windDirTime2;

	public float m_windVariation = 1f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		float num = Mathf.Sin(Time.time * 0.5f);
		float w = (num + 1f) * 0.5f;
		m_windSpeed = 0.8f + num * 0.5f * m_windVariation;
		m_windSpeed += Mathf.Sin(Time.time * 2.43f) * 0.15f * m_windVariation;
		m_windDirTime += Time.deltaTime * m_windSpeed;
		m_windDirTime2 += Time.deltaTime * m_windSpeed * 0.5f;
		Shader.SetGlobalVector("g_globalWindData", new Vector4(m_windDirTime, m_windDirTime2, 0f, w));
		Vector3 direction = Wind.direction;
		direction.y = -0.35f;
		direction.Normalize();
		Shader.SetGlobalVector("g_globalWindDir", new Vector4(direction.x, direction.y, direction.z, 0f));
		Vector3 vector = Quaternion.AngleAxis(110f, Vector3.up) * direction;
		vector.y = -0.07f;
		vector.Normalize();
		Shader.SetGlobalVector("g_globalWindDir2", new Vector4(vector.x, vector.y, vector.z, 0f));
	}
}
