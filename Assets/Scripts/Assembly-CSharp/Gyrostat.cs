using UnityEngine;

public class Gyrostat : MonoBehaviour
{
	private const float PROJECTION_LENGTH = 1000f;

	public Transform Parent;

	private Transform mParent;

	private Vector3 mProjectedPoint;

	private Vector3 ProjectionTarget
	{
		get
		{
			return base.transform.position + Parent.forward * 1000f;
		}
	}

	private void Start()
	{
		base.transform.parent = null;
		mProjectedPoint = ProjectionTarget;
	}

	private void Update()
	{
		base.transform.position = Parent.position;
		mProjectedPoint = Vector3.Lerp(mProjectedPoint, ProjectionTarget, Time.deltaTime * 1f);
		base.transform.forward = (mProjectedPoint - base.transform.position).normalized;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, ProjectionTarget);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, mProjectedPoint);
	}
}
