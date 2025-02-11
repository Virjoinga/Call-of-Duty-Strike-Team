using UnityEngine;

public class CloseSnowDebugControls : MonoBehaviour
{
	private CloseSnow m_closeSnow;

	private void Start()
	{
		m_closeSnow = GetComponent<CloseSnow>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Home))
		{
			m_closeSnow.Toggle(true, 1f);
		}
		else if (Input.GetKeyDown(KeyCode.End))
		{
			m_closeSnow.Toggle(false, 1f);
		}
		else if (Input.GetKeyDown(KeyCode.PageUp))
		{
			m_closeSnow.m_numParticles += 250;
			m_closeSnow.BuildMesh();
		}
		else if (Input.GetKeyDown(KeyCode.PageDown))
		{
			m_closeSnow.m_numParticles -= 250;
			m_closeSnow.BuildMesh();
		}
	}
}
