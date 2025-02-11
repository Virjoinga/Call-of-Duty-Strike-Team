using UnityEngine;

public class TransitionCamera : CameraBase, PlayCameraInterface
{
	private float mBaseFieldOfView;

	public override float Fov
	{
		get
		{
			return mBaseFieldOfView;
		}
		set
		{
			mBaseFieldOfView = value;
		}
	}

	public override Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			TBFAssert.DoAssert(false, "not allowed");
		}
	}

	public Vector3 PanOffset
	{
		get
		{
			return Vector3.zero;
		}
	}

	public Vector3 PanVelocity
	{
		get
		{
			return Vector3.zero;
		}
	}

	public bool MovingToFocusPoint
	{
		get
		{
			return false;
		}
	}

	public float WorldRotation
	{
		get
		{
			return base.transform.eulerAngles.y;
		}
	}

	public void Start()
	{
	}

	public void LateUpdate()
	{
	}

	public void FocusAndSelectTarget(Transform trans)
	{
	}

	public void SnapToTarget(Transform trans)
	{
	}

	public bool FocusOnTarget(Transform trans, bool forceEvenIfOnScreen)
	{
		return false;
	}
}
