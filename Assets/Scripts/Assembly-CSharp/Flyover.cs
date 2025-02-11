using UnityEngine;

public class Flyover : MonoBehaviour
{
	public enum PlaneState
	{
		straightAhead = 0,
		turnLeft = 1,
		turnRight = 2,
		numPlaneStates = 3
	}

	private GameObject mPlane;

	private Vector3 mStartPoint = default(Vector3);

	private Vector3 mStartRotation = default(Vector3);

	private bool mFlying;

	public PlaneState mPlaneState;

	public float moveSpeed = 10f;

	public float turnSpeed = 5f;

	public float maxRotationX = 45f;

	public float maxRoationY = 1f;

	public float decideToTurnAfter = 50f;

	public float maxDistanceToTravel = 5000f;

	private void Start()
	{
		mPlane = base.gameObject;
		mStartPoint = mPlane.transform.position;
		mStartRotation = mPlane.transform.eulerAngles;
	}

	private void Update()
	{
		if (!mFlying)
		{
			return;
		}
		if (base.transform.localPosition.x > mStartPoint.x - maxDistanceToTravel)
		{
			mPlane.transform.Translate(Vector3.left * moveSpeed);
		}
		if (base.transform.localPosition.x < mStartPoint.x - decideToTurnAfter)
		{
			switch (mPlaneState)
			{
			case PlaneState.straightAhead:
				break;
			case PlaneState.turnLeft:
			{
				Quaternion to = Quaternion.Euler(mStartRotation.x - maxRotationX, mStartRotation.y + maxRoationY, mStartRotation.z);
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, to, Time.deltaTime * turnSpeed);
				break;
			}
			case PlaneState.turnRight:
			{
				Quaternion to = Quaternion.Euler(mStartRotation.x + maxRotationX, mStartRotation.y - maxRoationY, mStartRotation.z);
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, to, Time.deltaTime * turnSpeed);
				break;
			}
			}
		}
	}

	private void Restart()
	{
		mPlane.transform.position = mStartPoint;
		mPlane.transform.eulerAngles = mStartRotation;
		mFlying = true;
		mPlaneState = (PlaneState)Random.Range(0, 3);
	}
}
