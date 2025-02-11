using UnityEngine;

public class AuditoryAwarenessEvent : MonoBehaviour
{
	public float Radius = 10f;

	public bool NoEnemyAwareness;

	private bool mExpired;

	public Vector3 Origin
	{
		get
		{
			return base.transform.position;
		}
	}

	private void Start()
	{
		mExpired = false;
	}

	private void Update()
	{
	}

	public void Trigger()
	{
		if (!mExpired)
		{
			AuditoryAwarenessManager.Instance.RegisterEvent(this);
			mExpired = true;
		}
	}
}
