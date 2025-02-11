using UnityEngine;

public class HelicopterTrigger : MonoBehaviour
{
	public enum TriggerType
	{
		HelicopterTakeoff = 0,
		HelicopterDestroy = 1,
		TriggeredByMessage = 2
	}

	public HelicopterRoutine HelicopterRef;

	public TriggerType TypeOfTrigger;

	private bool mActive;

	private void Start()
	{
		mActive = true;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (TypeOfTrigger != TriggerType.TriggeredByMessage && mActive)
		{
			switch (TypeOfTrigger)
			{
			case TriggerType.HelicopterTakeoff:
				Activate();
				break;
			case TriggerType.HelicopterDestroy:
				Deactivate();
				break;
			}
			mActive = false;
		}
	}

	public void Activate()
	{
		HelicopterRef.TriggerStart();
	}

	public void Deactivate()
	{
		HelicopterRef.TriggerDestroy(null);
	}
}
