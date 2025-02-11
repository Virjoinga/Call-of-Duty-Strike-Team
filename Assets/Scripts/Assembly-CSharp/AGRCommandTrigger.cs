using UnityEngine;

public class AGRCommandTrigger : MonoBehaviour
{
	public AGRController Owner;

	public Transform Bomb;

	private bool mHasRegistered;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!mHasRegistered)
		{
			Actor component = other.gameObject.GetComponent<Actor>();
			if (!(component == null) && component.behaviour.PlayerControlled)
			{
				Owner.RegisterBombDiffusalRequest(Bomb);
				mHasRegistered = true;
			}
		}
	}
}
