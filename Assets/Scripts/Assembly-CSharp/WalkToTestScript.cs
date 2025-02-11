using UnityEngine;

public class WalkToTestScript : MonoBehaviour
{
	public GameObject child;

	public GameObject target;

	public Vector3 mLastPosition;

	public float walkspeed;

	public float walkspeedoverride;

	private Vector3 mAimDir;

	public Vector3 mStartAimDir;

	public bool crouch;

	public bool run;

	private void Start()
	{
		mLastPosition = child.transform.position;
		mAimDir = Vector3.forward;
		crouch = false;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		MoveAimModule component = child.GetComponent<MoveAimModule>();
		Vector3 position = base.transform.position;
		Vector3 mLogicalPosition = component.mLogicalPosition;
		float num = walkspeed * walkspeedoverride;
		if (Random.Range(0, 30) == 0)
		{
			walkspeedoverride = Random.Range(0.9f, 1.1f);
		}
		position -= mLogicalPosition;
		position.y = 0f;
		if (position.sqrMagnitude > 0f)
		{
			float magnitude = position.magnitude;
			if (magnitude > num)
			{
				position *= num / magnitude;
			}
		}
		mLogicalPosition.x += position.x;
		mLogicalPosition.z += position.z;
		mAimDir = target.transform.position - mLogicalPosition;
		mAimDir.Normalize();
		mLastPosition = mLogicalPosition;
		AnimDirector component2 = child.GetComponent<AnimDirector>();
		component2.EnableOverride(component2.GetOverrideHandle("Crouch"), crouch);
		component2.EnableOverride(component2.GetOverrideHandle("Run"), run);
	}
}
