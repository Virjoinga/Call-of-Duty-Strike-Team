using UnityEngine;

public class BackDoorLogic : MonoBehaviour
{
	public NavGate m_NavGate;

	public GameObject m_DoorToAnimate;

	public string m_AnimationName;

	public GenericTrigger m_Trigger;

	public ActorWrapper m_Spawner;

	private void Awake()
	{
		if (m_Trigger != null && m_Spawner != null)
		{
			m_Trigger.Actors.Add(m_Spawner);
		}
	}

	private void TriggerBackDoor()
	{
		if (m_NavGate != null)
		{
			m_NavGate.OpenNavGate();
		}
		if (m_DoorToAnimate != null && m_DoorToAnimate.animation != null)
		{
			m_DoorToAnimate.animation.Play(m_AnimationName);
		}
	}
}
