using UnityEngine;

public class AuditoryAwarenessEventTrigger : MonoBehaviour
{
	public AuditoryAwarenessEvent AuditoryEvent;

	public bool PlayerUnitOnly;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(AuditoryEvent == null))
		{
			Actor component = other.gameObject.GetComponent<Actor>();
			if (!(component == null) && (!PlayerUnitOnly || (PlayerUnitOnly && component.behaviour.PlayerControlled)))
			{
				AuditoryEvent.Trigger();
			}
		}
	}
}
