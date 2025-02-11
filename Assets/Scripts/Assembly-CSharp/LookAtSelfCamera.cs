using UnityEngine;

public class LookAtSelfCamera : CameraBase
{
	public Vector3 Offset = new Vector3(0f, 4.5f, -8f);

	private Quaternion mRotations;

	public override Vector3 Position
	{
		get
		{
			return base.transform.TransformPoint(Offset);
		}
		set
		{
			TBFAssert.DoAssert(false, "not allowed");
			Offset = value - base.transform.position;
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			return mRotations;
		}
		set
		{
			TBFAssert.DoAssert(false, "not allowed");
			base.transform.rotation = value;
		}
	}

	private void Start()
	{
		CalculateLookAt();
	}

	private void CalculateLookAt()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.transform.position = Position;
		base.transform.LookAt(position);
		mRotations = base.transform.rotation;
		base.transform.rotation = rotation;
		base.transform.position = position;
	}

	public override Vector3 TargetPoint()
	{
		return base.transform.position;
	}
}
