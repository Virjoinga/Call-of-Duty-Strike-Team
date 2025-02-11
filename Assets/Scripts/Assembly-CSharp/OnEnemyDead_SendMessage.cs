using UnityEngine;

public class OnEnemyDead_SendMessage : MonoBehaviour
{
	public OnDeadData m_Interface;

	private void OnSpawned(GameObject spawned)
	{
		if (!(m_Interface.Target == null))
		{
			HealthComponent component = spawned.GetComponent<HealthComponent>();
			component.OnHealthEmpty += delegate
			{
				Container.SendMessage(m_Interface.Target, m_Interface.MessageToSend, base.gameObject);
			};
		}
	}
}
