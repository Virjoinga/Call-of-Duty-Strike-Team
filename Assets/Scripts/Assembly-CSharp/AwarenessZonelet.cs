using UnityEngine;

public class AwarenessZonelet : MonoBehaviour
{
	public AwarenessZone parent;

	private Vector3 position;

	private Quaternion invrotation;

	private Vector3 extent;

	private void Start()
	{
		position = base.transform.position;
		invrotation = Quaternion.Inverse(base.transform.rotation);
		extent = base.transform.lossyScale * 0.5f;
	}

	private void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null))
		{
			parent.EnterZonelet(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null))
		{
			parent.ExitZonelet(component);
		}
	}

	public bool Contains(Vector3 pos)
	{
		Vector3 vector = invrotation * (pos - position);
		if (Mathf.Abs(vector.x) > extent.x)
		{
			return false;
		}
		if (Mathf.Abs(vector.z) > extent.z)
		{
			return false;
		}
		if (Mathf.Abs(vector.y) > extent.y)
		{
			return false;
		}
		return true;
	}
}
