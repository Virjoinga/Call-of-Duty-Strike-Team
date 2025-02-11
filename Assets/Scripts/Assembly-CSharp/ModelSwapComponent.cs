using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class ModelSwapComponent : MonoBehaviour
{
	public enum eDamagedState
	{
		kNormal = 0,
		kDamaged = 1
	}

	public GameObject m_NonDamagedModel;

	public GameObject m_DamagedModel;

	public float m_DamagedAtHealthValue;

	private eDamagedState m_State;

	private void Awake()
	{
		Show(m_NonDamagedModel, true);
		Show(m_DamagedModel, false);
	}

	public void UpdateHealth(float health)
	{
		if (m_State == eDamagedState.kNormal && health <= m_DamagedAtHealthValue)
		{
			Show(m_NonDamagedModel, false);
			Show(m_DamagedModel, true);
			m_State = eDamagedState.kDamaged;
		}
	}

	private void Show(GameObject Obj, bool bOnOff)
	{
		if (Obj != null)
		{
			MeshRenderer[] componentsInChildren = Obj.GetComponentsInChildren<MeshRenderer>(true);
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = bOnOff;
			}
		}
	}
}
