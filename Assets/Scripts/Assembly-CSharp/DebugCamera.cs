using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DebugCamera : MonoBehaviour
{
	private Transform m_transform;

	private Camera m_camera;

	private Camera m_prevCamera;

	[SerializeField]
	private float m_normalSpeed = 5f;

	[SerializeField]
	private float m_fastSpeed = 10f;

	[SerializeField]
	private VirtualDPad m_dpad;

	private void Awake()
	{
		m_transform = base.transform;
		m_camera = base.GetComponent<Camera>();
		m_camera.enabled = false;
	}

	public void Toggle()
	{
		if (m_camera.enabled)
		{
			m_camera.enabled = false;
			if (m_prevCamera != null)
			{
				m_prevCamera.enabled = true;
				m_prevCamera = null;
			}
			return;
		}
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			if (allCameras[i].enabled)
			{
				m_prevCamera = allCameras[i];
				m_prevCamera.enabled = false;
			}
		}
		m_camera.enabled = true;
		if (m_prevCamera != null)
		{
			base.transform.position = m_prevCamera.transform.position;
			base.transform.rotation = m_prevCamera.transform.rotation;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
		{
			Toggle();
		}
		if (m_camera.enabled)
		{
			float num = ((!Input.GetKey(KeyCode.LeftShift)) ? m_normalSpeed : m_fastSpeed);
			if (Time.timeScale > float.Epsilon)
			{
				num /= Time.timeScale;
			}
			float num2 = m_dpad.YPos;
			float num3 = m_dpad.XPos;
			if (Input.GetKey(KeyCode.W))
			{
				num2 = num;
			}
			if (Input.GetKey(KeyCode.S))
			{
				num2 = 0f - num;
			}
			if (Input.GetKey(KeyCode.D))
			{
				num3 = num;
			}
			if (Input.GetKey(KeyCode.A))
			{
				num3 = 0f - num;
			}
			float num4 = ((!(Time.deltaTime > float.Epsilon)) ? Time.fixedDeltaTime : Time.deltaTime);
			m_transform.position += m_transform.forward * num2 * num4;
			m_transform.position += m_transform.right * num3 * num4;
			if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftShift))
			{
				string text = Application.persistentDataPath + string.Format("/capture-{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now);
				ScreenCapture.CaptureScreenshot(text);
				Debug.Log(text);
			}
		}
	}
}
