using System.Collections;
using UnityEngine;

public class BreathManagerTest : MonoBehaviour
{
	private BreathManager m_manager;

	private void Awake()
	{
		m_manager = GetComponentInChildren<BreathManager>();
	}

	private IEnumerator Start()
	{
		WaitForSeconds timeInState = new WaitForSeconds(5f);
		while (true)
		{
			m_manager.GotoState(BreathState.Normal, 1f);
			yield return timeInState;
			m_manager.GotoState(BreathState.Running, 1f);
			yield return timeInState;
			m_manager.GotoState(BreathState.Heavy, 1f);
			yield return timeInState;
		}
	}
}
