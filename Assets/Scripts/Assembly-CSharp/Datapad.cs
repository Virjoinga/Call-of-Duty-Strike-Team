using UnityEngine;

public class Datapad : MonoBehaviour
{
	private float m_brightness;

	private bool m_screenOn = true;

	private bool m_switchOnOff = true;

	public float m_screenOnTime = 0.15f;

	public float m_screenOffTime = 0.3f;

	public float m_minBrightness = 0.25f;

	public float m_maxBrightness = 2f;

	public bool ScreenOn
	{
		get
		{
			return m_screenOn;
		}
		set
		{
			if (m_screenOn != value)
			{
				m_screenOn = value;
				m_switchOnOff = true;
			}
		}
	}

	private void Awake()
	{
		m_brightness = m_maxBrightness;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.F10))
		{
			ScreenOn = !ScreenOn;
		}
		if (m_switchOnOff)
		{
			float num = ((!m_screenOn) ? m_minBrightness : m_maxBrightness);
			float num2 = ((!m_screenOn) ? m_screenOffTime : m_screenOnTime);
			float t = ((!(num2 > 0f)) ? 1f : (1f - Mathf.Pow(0.05f, deltaTime / num2)));
			m_brightness = Mathf.Lerp(m_brightness, num, t);
			if (Mathf.Abs(m_brightness - num) < 0.01f)
			{
				m_brightness = num;
				m_switchOnOff = false;
			}
			Shader.SetGlobalFloat("g_datapadBrightness", m_brightness);
		}
	}
}
