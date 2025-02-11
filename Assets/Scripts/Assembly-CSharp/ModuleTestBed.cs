using UnityEngine;

public class ModuleTestBed : MonoBehaviour
{
	public GameObject child;

	public GameObject target;

	public Vector3 mLastPosition;

	private float animangx;

	private float animangz;

	public float animspeedx;

	public float animspeedz;

	public float animscalex;

	public float animscalez;

	public float animspeedoverride;

	public float mStartAnimTime;

	private Vector3 mAimDir;

	public Vector3 mStartAimDir;

	public bool crouch;

	private void Start()
	{
		mLastPosition = base.transform.position;
		animangx = 0f;
		animangz = 0f;
		mAimDir = Vector3.forward;
		crouch = false;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		Vector3 position = base.transform.position;
		position.x = Mathf.PingPong(animangx, 2f);
		base.transform.position = position;
		mAimDir = target.transform.position - position;
		mAimDir.Normalize();
		mLastPosition = position;
		animangx += animspeedx;
		if (animangx > 4f)
		{
			animangx = 0f;
			animangz = 0f;
			position.x = Mathf.Cos(animangx) * animscalex;
			position.z = Mathf.Sin(animangz) * animscalez;
			mLastPosition = position;
			base.transform.position = position;
			mAimDir = target.transform.position - position;
			mAimDir.Normalize();
		}
		AnimDirector component = child.GetComponent<AnimDirector>();
		component.EnableOverride(component.GetOverrideHandle("Crouch"), crouch);
	}
}
