using UnityEngine;

public class DebugFlyCamera : MonoBehaviour
{
	private enum InputState
	{
		Invalid = 0,
		Move = 1,
		Look = 2,
		Roll = 3
	}

	private Vector3 mCameraRotations;

	private Vector3 mBufferedMousePos;

	private InputState mMouseInputState;

	private int mTouchLookIndex = -1;

	private int mTouchMoveIndex = -1;

	private int mTouchRollIndex = -1;

	private Vector2 mTouchLookPos;

	private Vector2 mTouchMovePos;

	private Vector2 mTouchRollPos;

	public float MoveSpeed = 0.7f;

	public float RotateSpeed = 0.8f;

	private void Start()
	{
		mBufferedMousePos = Vector3.zero;
		mMouseInputState = InputState.Invalid;
		mCameraRotations = base.transform.rotation.eulerAngles;
		base.transform.rotation = Quaternion.identity;
		mTouchLookPos = Vector2.zero;
		mTouchMovePos = Vector2.zero;
		mTouchRollPos = Vector2.zero;
	}

	private InputState GetInputStateFromPoint(Vector2 pos)
	{
		return GetInputStateFromPoint(new Vector3(pos.x, pos.y, 0f));
	}

	private InputState GetInputStateFromPoint(Vector3 pos)
	{
		if (pos.x > (float)Screen.width / 2f)
		{
			if (pos.y > (float)Screen.height - (float)Screen.height / 4f)
			{
				return InputState.Roll;
			}
			return InputState.Look;
		}
		return InputState.Move;
	}

	private void UpdateTouch()
	{
		if (Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			Touch[] array = touches;
			for (int i = 0; i < array.Length; i++)
			{
				Touch touch = array[i];
				if (touch.phase != 0)
				{
					continue;
				}
				switch (GetInputStateFromPoint(touch.position))
				{
				case InputState.Look:
					if (mTouchLookIndex == -1)
					{
						mTouchLookIndex = touch.fingerId;
						mTouchLookPos = touch.position;
					}
					if (mTouchMoveIndex == touch.fingerId)
					{
						mTouchMoveIndex = -1;
					}
					if (mTouchRollIndex == touch.fingerId)
					{
						mTouchRollIndex = -1;
					}
					break;
				case InputState.Move:
					if (mTouchMoveIndex == -1)
					{
						mTouchMoveIndex = touch.fingerId;
						mTouchMovePos = touch.position;
					}
					if (mTouchLookIndex == touch.fingerId)
					{
						mTouchLookIndex = -1;
					}
					if (mTouchRollIndex == touch.fingerId)
					{
						mTouchRollIndex = -1;
					}
					break;
				case InputState.Roll:
					if (mTouchRollIndex == -1)
					{
						mTouchRollIndex = touch.fingerId;
						mTouchRollPos = touch.position;
					}
					if (mTouchLookIndex == touch.fingerId)
					{
						mTouchLookIndex = -1;
					}
					if (mTouchMoveIndex == touch.fingerId)
					{
						mTouchMoveIndex = -1;
					}
					break;
				}
			}
			Touch[] array2 = touches;
			for (int j = 0; j < array2.Length; j++)
			{
				Touch touch2 = array2[j];
				TouchPhase phase = touch2.phase;
				if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
				{
					if (touch2.fingerId == mTouchLookIndex)
					{
						mTouchLookIndex = -1;
					}
					if (touch2.fingerId == mTouchMoveIndex)
					{
						mTouchMoveIndex = -1;
					}
					if (touch2.fingerId == mTouchRollIndex)
					{
						mTouchRollIndex = -1;
					}
				}
			}
			Touch[] array3 = touches;
			for (int k = 0; k < array3.Length; k++)
			{
				Touch touch3 = array3[k];
				switch (touch3.phase)
				{
				case TouchPhase.Moved:
					if (touch3.fingerId == -1)
					{
						break;
					}
					if (touch3.fingerId == mTouchLookIndex)
					{
						Vector2 vector = touch3.position - mTouchLookPos;
						mTouchLookPos = touch3.position;
						if (vector.magnitude < 20f)
						{
							mCameraRotations.x -= vector.y * RotateSpeed;
							mCameraRotations.y += vector.x * RotateSpeed;
						}
					}
					else if (touch3.fingerId == mTouchMoveIndex)
					{
						Vector2 vector2 = touch3.position - mTouchMovePos;
						mTouchMovePos = touch3.position;
						if (vector2.magnitude < 20f)
						{
							Vector3 position = base.transform.position;
							position += base.transform.forward * vector2.y * MoveSpeed;
							position += base.transform.right * vector2.x * MoveSpeed;
							base.transform.position = position;
						}
					}
					else if (touch3.fingerId == mTouchRollIndex)
					{
						Vector2 vector3 = touch3.position - mTouchRollPos;
						mTouchRollPos = touch3.position;
						if (vector3.magnitude < 20f)
						{
							mCameraRotations.z += vector3.x * RotateSpeed;
						}
					}
					break;
				}
			}
			base.transform.rotation = Quaternion.Euler(mCameraRotations);
		}
		else
		{
			mTouchLookIndex = -1;
			mTouchMoveIndex = -1;
			mTouchRollIndex = -1;
		}
	}

	private void UpdateMouse()
	{
		if (Input.GetMouseButton(0))
		{
			if (mMouseInputState == InputState.Invalid)
			{
				mBufferedMousePos = Input.mousePosition;
				mMouseInputState = GetInputStateFromPoint(Input.mousePosition);
			}
			Vector3 vector = Input.mousePosition - mBufferedMousePos;
			mBufferedMousePos = Input.mousePosition;
			switch (mMouseInputState)
			{
			case InputState.Roll:
				mCameraRotations.z += vector.x * RotateSpeed;
				break;
			case InputState.Look:
				mCameraRotations.x -= vector.y * RotateSpeed;
				mCameraRotations.y += vector.x * RotateSpeed;
				break;
			case InputState.Move:
			{
				Vector3 position = base.transform.position;
				position += base.transform.forward * vector.y * MoveSpeed;
				position += base.transform.right * vector.x * MoveSpeed;
				base.transform.position = position;
				break;
			}
			}
			base.transform.rotation = Quaternion.Euler(mCameraRotations);
		}
		else
		{
			mMouseInputState = InputState.Invalid;
		}
	}

	private void Update()
	{
		UpdateTouch();
	}
}
