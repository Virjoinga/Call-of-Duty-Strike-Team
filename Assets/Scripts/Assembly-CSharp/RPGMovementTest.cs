using System;
using UnityEngine;

public class RPGMovementTest : MonoBehaviour
{
	private Vector3 m_centerPosition;

	private float m_angle;

	[SerializeField]
	private float m_rotationRate;

	[SerializeField]
	private float m_rotationRadius;

	private void Awake()
	{
		m_centerPosition = base.transform.position;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(false);
		}
		SetPosition();
	}

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		m_angle += m_rotationRate * Time.deltaTime;
		m_angle = Mathf.Repeat(m_angle, 360f);
		SetPosition();
	}

	private void SetPosition()
	{
		float f = m_angle * ((float)Math.PI / 180f);
		Vector3 centerPosition = m_centerPosition;
		centerPosition.x += Mathf.Cos(f) * m_rotationRadius;
		centerPosition.z += Mathf.Sin(f) * m_rotationRadius;
		base.transform.position = centerPosition;
	}
}
