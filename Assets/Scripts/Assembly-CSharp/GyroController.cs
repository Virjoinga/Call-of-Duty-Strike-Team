using UnityEngine;

public class GyroController : MonoBehaviour
{
	private enum State
	{
		Off = 0,
		DemoMode = 1,
		KeyboardEmulation = 2
	}

	private Vector3 driftBias;

	private Vector3 gyroDelta;

	private static float DEMO_AXIS_TIME = 3f;

	private State mTestState;

	private Vector3 mRotationRateUnbiased;

	private float mDemoAxisTime;

	private int mDemoDirection;

	private void Start()
	{
		mTestState = State.Off;
		mDemoAxisTime = DEMO_AXIS_TIME;
		Input.gyro.enabled = true;
		driftBias = Vector3.zero;
		gyroDelta = Vector3.zero;
	}

	private void Update()
	{
		switch (mTestState)
		{
		case State.DemoMode:
			UpdateDemo();
			break;
		case State.KeyboardEmulation:
			UpdateKeyboard();
			break;
		}
		Vector3 rotationRateUnbiased = Input.gyro.rotationRateUnbiased;
		if (InputSettings.GyroAntiDrift)
		{
			if (Mathf.Abs(rotationRateUnbiased.x) < InputSettings.GyroDriftThreshold)
			{
				driftBias.x *= InputSettings.GyroDriftSmoothing;
				driftBias.x += rotationRateUnbiased.x * (1f - InputSettings.GyroDriftSmoothing);
				rotationRateUnbiased.x = 0f;
			}
			else
			{
				rotationRateUnbiased.x -= driftBias.x;
			}
			if (Mathf.Abs(rotationRateUnbiased.y) < InputSettings.GyroDriftThreshold)
			{
				driftBias.y *= InputSettings.GyroDriftSmoothing;
				driftBias.y += rotationRateUnbiased.y * (1f - InputSettings.GyroDriftSmoothing);
				rotationRateUnbiased.y = 0f;
			}
			else
			{
				rotationRateUnbiased.y -= driftBias.y;
			}
			gyroDelta = Vector3.Slerp(gyroDelta, rotationRateUnbiased, InputSettings.GyroSmoothing);
		}
		else
		{
			gyroDelta = rotationRateUnbiased;
		}
	}

	public Vector3 GetRotationRateUnbiased()
	{
		if (mTestState == State.Off)
		{
			return gyroDelta;
		}
		return mRotationRateUnbiased;
	}

	private void UpdateDemo()
	{
		mRotationRateUnbiased = Vector3.zero;
		switch (mDemoDirection)
		{
		case 0:
			mRotationRateUnbiased.x = Time.deltaTime * 10f;
			break;
		case 1:
			mRotationRateUnbiased.y = Time.deltaTime * 10f;
			break;
		case 2:
			mRotationRateUnbiased.x = 0f - Time.deltaTime * 10f;
			break;
		case 3:
			mRotationRateUnbiased.y = 0f - Time.deltaTime * 10f;
			break;
		}
		mDemoAxisTime -= Time.deltaTime;
		if (mDemoAxisTime <= 0f)
		{
			mDemoAxisTime = DEMO_AXIS_TIME;
			mDemoDirection++;
			if (mDemoDirection > 3)
			{
				mDemoDirection = 0;
			}
		}
	}

	private void UpdateKeyboard()
	{
		bool flag = false;
		bool flag2 = false;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			mRotationRateUnbiased.y += Time.deltaTime * 10f;
			flag = true;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			mRotationRateUnbiased.y -= Time.deltaTime * 10f;
			flag = true;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			mRotationRateUnbiased.x += Time.deltaTime * 10f;
			flag2 = true;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			mRotationRateUnbiased.x -= Time.deltaTime * 10f;
			flag2 = true;
		}
		if (!flag2)
		{
			mRotationRateUnbiased.x = Mathf.Lerp(mRotationRateUnbiased.x, 0f, Time.deltaTime * 10f);
		}
		if (!flag)
		{
			mRotationRateUnbiased.y = Mathf.Lerp(mRotationRateUnbiased.y, 0f, Time.deltaTime * 10f);
		}
	}
}
