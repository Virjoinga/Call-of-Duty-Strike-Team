using UnityEngine;

public class BloodEffectTest : MonoBehaviour
{
	private const float c_maxHealth = 100f;

	private float m_healthRechargeTimer;

	private float m_health = 100f;

	private float m_lastHealth = 100f;

	[SerializeField]
	private float m_healthRechargeDelay = 5f;

	[SerializeField]
	private float m_healthRechargeRate = 20f;

	[SerializeField]
	private float m_hitDamage = 10f;

	[SerializeField]
	private BloodEffect m_bloodEffect;

	private void Update()
	{
		if (m_health < 100f)
		{
			float deltaTime = Time.deltaTime;
			m_healthRechargeTimer += deltaTime;
			if (m_healthRechargeTimer > m_healthRechargeDelay)
			{
				m_health = Mathf.Min(m_health + deltaTime * m_healthRechargeRate, 100f);
				if (m_bloodEffect != null)
				{
					float num = 1f - m_health / 100f;
					float num2 = 1f - m_lastHealth / 100f;
					m_bloodEffect.Alpha = num / num2;
					m_bloodEffect.Severity = num;
				}
			}
		}
		else if (m_bloodEffect != null)
		{
			m_bloodEffect.Alpha = 0f;
			m_bloodEffect.Severity = 0f;
		}
	}

	private void ApplyDamage(float _amount)
	{
		m_health = Mathf.Max(0f, m_health - _amount);
		m_lastHealth = m_health;
		m_healthRechargeTimer = 0f;
		if (m_bloodEffect != null)
		{
			m_bloodEffect.Alpha = 1f;
			m_bloodEffect.Severity = 1f - m_health / 100f;
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect((float)Screen.width / 4f, 10f, (float)Screen.width / 2f, 10f));
		GUILayout.HorizontalSlider(m_health, 0f, 100f);
		GUILayout.EndArea();
		GUILayout.BeginVertical();
		int num = Screen.height / 16;
		int num2 = Screen.width / 10;
		GUILayoutOption[] options = new GUILayoutOption[2]
		{
			GUILayout.MinHeight(num),
			GUILayout.MinWidth(num2)
		};
		if (GUILayout.Button("Apply Damage", options))
		{
			ApplyDamage(m_hitDamage);
		}
		GUILayout.EndVertical();
	}
}
