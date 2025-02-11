using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InputVisualiser : MonoBehaviour
{
	public static InputVisualiser instance;

	private bool mHidden;

	private bool mCaptureGyro;

	private Queue<string> mGestureLog = new Queue<string>();

	private Queue<string> mGyroLog = new Queue<string>();

	public static InputVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		UnityEngine.Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
		FingerGestures.OnFingerTap += delegate(int fingerIndex, Vector2 fingerPos)
		{
			mGestureLog.Enqueue("OnFingerTap f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnPinchMove += delegate
		{
			mGestureLog.Enqueue("OnPinchMove " + Time.time.ToString("N2"));
		};
		FingerGestures.OnTwoFingerDragBegin += delegate
		{
			mGestureLog.Enqueue("OnTwoFingerDragBegin " + Time.time.ToString("N2"));
		};
		FingerGestures.OnTwoFingerDragMove += delegate
		{
			mGestureLog.Enqueue("OnTwoFingerDragMove " + Time.time.ToString("N2"));
		};
		FingerGestures.OnTwoFingerDragEnd += delegate
		{
			mGestureLog.Enqueue("OnTwoFingerDragEnd " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerDoubleTap += delegate(int fingerIndex, Vector2 fingerPos)
		{
			mGestureLog.Enqueue("OnFingerDoubleTap f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerSwipe += delegate(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
		{
			mGestureLog.Enqueue("OnFingerSwipe " + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerDown += delegate(int fingerIndex, Vector2 fingerPos)
		{
			mGestureLog.Enqueue("OnFingerDown f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerUp += delegate(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
		{
			mGestureLog.Enqueue("OnFingerUp f" + fingerIndex + " dt" + timeHeldDown.ToString("N2") + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerDragBegin += delegate(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
		{
			mGestureLog.Enqueue("OnFingerDragBegin f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerDragEnd += delegate(int fingerIndex, Vector2 fingerPos)
		{
			mGestureLog.Enqueue("OnFingerDragEnd f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnFingerDragMove += delegate(int fingerIndex, Vector2 fingerPos, Vector2 delta)
		{
			mGestureLog.Enqueue("OnFingerDragMove f" + fingerIndex + " " + Time.time.ToString("N2"));
		};
		FingerGestures.OnRotationBegin += delegate
		{
			mGestureLog.Enqueue("OnRotationBegin " + Time.time.ToString("N2"));
		};
		FingerGestures.OnRotationMove += delegate
		{
			mGestureLog.Enqueue("OnRotationMove " + Time.time.ToString("N2"));
		};
		FingerGestures.OnRotationEnd += delegate
		{
			mGestureLog.Enqueue("OnRotationEnd " + Time.time.ToString("N2"));
		};
	}

	private void Update()
	{
		if (!mHidden)
		{
		}
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		Rect position = new Rect(32f, 32f, (float)Screen.width - 64f, 32f);
		GameController.Instance.FirstPersonGyroThreshold = GUI.HorizontalSlider(position, GameController.Instance.FirstPersonGyroThreshold, 0.001f, 1f);
		string text = "Touch Input Visualiser";
		int num = 600;
		int num2 = 340;
		Rect screenRect = new Rect(Screen.width - num2 - 20, Screen.height - (num + 20), num2, num);
		GUILayout.BeginArea(screenRect, text);
		InputManager inputManager = InputManager.Instance;
		GUILayout.BeginVertical("Motion Control", GUI.skin.window);
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.padding = new RectOffset(0, 0, 0, 0);
		gUIStyle.margin = new RectOffset(0, 0, 0, 0);
		GUILayout.Label(string.Format("Gyro Threshold: {0}", GameController.Instance.FirstPersonGyroThreshold), gUIStyle);
		GUILayout.Space(16f);
		GUILayout.Label(string.Format("Screen Orientation: {0}", Screen.orientation.ToString()), gUIStyle);
		GUILayout.Label(string.Format("Accelerometer: {0}", Input.acceleration.ToString("F4")), gUIStyle);
		GUILayout.Label(string.Format("Attitude: {0}", Input.gyro.attitude.eulerAngles.ToString("F4")), gUIStyle);
		GUILayout.Label(string.Format("Gravity: {0}", Input.gyro.gravity.ToString("F4")), gUIStyle);
		GUILayout.Label(string.Format("Rotation Rate: {0}", Input.gyro.rotationRate.ToString("F4")), gUIStyle);
		GUILayout.Label(string.Format("Rotation Rate Unbiased: {0}", Input.gyro.rotationRateUnbiased.ToString("F4")), gUIStyle);
		GUILayout.Label(string.Format("User Acceleration: {0}", Input.gyro.userAcceleration.ToString("F4")), gUIStyle);
		GUILayout.EndVertical();
		if (mCaptureGyro)
		{
			mGyroLog.Enqueue(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", Screen.orientation.ToString(), Input.acceleration.x, Input.acceleration.y, Input.acceleration.z, Input.gyro.attitude.eulerAngles.x, Input.gyro.attitude.eulerAngles.y, Input.gyro.attitude.eulerAngles.z, Input.gyro.gravity.x, Input.gyro.gravity.y, Input.gyro.gravity.z, Input.gyro.rotationRate.x, Input.gyro.rotationRate.y, Input.gyro.rotationRate.z, Input.gyro.rotationRateUnbiased.x, Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.z, Input.gyro.userAcceleration.x, Input.gyro.userAcceleration.y, Input.gyro.userAcceleration.z));
		}
		GUILayout.BeginHorizontal("Capture Gyro", GUI.skin.window);
		if (GUILayout.Button("Start"))
		{
			mCaptureGyro = true;
		}
		if (GUILayout.Button("Clear"))
		{
			mCaptureGyro = false;
			mGyroLog.Clear();
		}
		if (GUILayout.Button("Send"))
		{
			mCaptureGyro = false;
			string data = "Screen Orientation," + "Accelerometer.x,Accelerometer.y,Accelerometer.z," + "Attitude.x,Attitude.y,Attitude.z," + "Gravity.x,Gravity.y,Gravity.z," + "Rotation Rate.x,Rotation Rate.y,Rotation Rate.z," + "Rotation Rate Unbiased.x,Rotation Rate Unbiased.y,Rotation Rate Unbiased.z," + "User Acceleration.x,User Acceleration.y,User Acceleration.z," + Environment.NewLine + string.Join(Environment.NewLine, mGyroLog.ToArray());
			StartCoroutine(SendOverHttp(data));
			mGyroLog.Clear();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical("Touch Control", GUI.skin.window);
		GUILayout.Label("Num Touches " + inputManager.NumFingersActive, gUIStyle);
		GUILayout.Space(16f);
		GestureRecognizer doubleTap = FingerGestures.Instance.defaultComponents.DoubleTap;
		GUILayout.TextArea("DoubleTap: " + doubleTap.IsActive + " " + doubleTap.StartTime.ToString("N2") + " " + doubleTap.ElapsedTime.ToString("N2") + " " + doubleTap.State.ToString() + " " + doubleTap.PreviousState.ToString() + " :" + doubleTap.FailReason);
		while (mGestureLog.Count > 20)
		{
			mGestureLog.Dequeue();
		}
		string text2 = string.Empty;
		foreach (string item in mGestureLog)
		{
			text2 = text2 + item + "\n";
		}
		GUILayout.TextArea(text2);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
	}

	public void Toggle()
	{
		mHidden = !mHidden;
	}

	public void GLDebugVisualise()
	{
		if (!mHidden)
		{
		}
	}

	private IEnumerator SendOverHttp(string data)
	{
		string server = "http://uklbadahen.activision.com:1337";
		WWW www = new WWW(server, Encoding.ASCII.GetBytes(WWW.EscapeURL(data)));
		yield return www;
		www.Dispose();
	}
}
