using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moga_Input : MonoBehaviour
{
	private enum ButtonState
	{
		RELEASED = 0,
		PRESSING = 1,
		PRESSED = 2,
		RELEASING = 3
	}

	private const float ANALOG_DEADZONE = 0.19f;

	public int controllerID = 1;

	private static GameObject mogaManager = null;

	private static Moga_ControllerManager mogaControllerManager = null;

	private Moga_Controller mControllerManager;

	private static bool unityControllerConnected;

	private bool mFocused;

	public static bool keypressedHere = false;

	private static Dictionary<string, Dictionary<int, float>> mAxes = new Dictionary<string, Dictionary<int, float>>();

	private static Dictionary<int, ButtonState> mogaButtons = new Dictionary<int, ButtonState>();

	private static Dictionary<string, int> buttonStrings = new Dictionary<string, int>();

	private static Dictionary<KeyCode, int> buttonKeyCodes = new Dictionary<KeyCode, int>();

	private static bool mAnyKey;

	private static bool mAnyKeyDown;

	private static Coroutine checkForControllers;

	public static Vector3 acceleration
	{
		get
		{
			return Input.acceleration;
		}
	}

	public static int accelerationEventCount
	{
		get
		{
			return Input.accelerationEventCount;
		}
	}

	public static bool anyKey
	{
		get
		{
			return mAnyKey || Input.anyKey;
		}
	}

	public static bool anyKeyDown
	{
		get
		{
			return mAnyKeyDown || Input.anyKeyDown;
		}
	}

	public static Compass compass
	{
		get
		{
			return Input.compass;
		}
	}

	public static string compositionString
	{
		get
		{
			return Input.compositionString;
		}
	}

	public static Vector2 compositionCursorPos
	{
		get
		{
			return Input.compositionCursorPos;
		}
	}

	public static DeviceOrientation deviceOrientation
	{
		get
		{
			return Input.deviceOrientation;
		}
	}

	public static Gyroscope gyro
	{
		get
		{
			return Input.gyro;
		}
	}

	public static IMECompositionMode imeCompositionMode
	{
		get
		{
			return Input.imeCompositionMode;
		}
		set
		{
			Input.imeCompositionMode = value;
		}
	}

	public static bool imeIsSelected
	{
		get
		{
			return Input.imeIsSelected;
		}
	}

	public static string inputString
	{
		get
		{
			return Input.inputString;
		}
	}

	public static Vector3 mousePosition
	{
		get
		{
			return Input.mousePosition;
		}
	}

	public static bool multiTouchEnabled
	{
		get
		{
			return Input.multiTouchEnabled;
		}
		set
		{
			Input.multiTouchEnabled = value;
		}
	}

	public static int touchCount
	{
		get
		{
			return Input.touchCount;
		}
	}

	public static Touch[] touches
	{
		get
		{
			return Input.touches;
		}
	}

	public static LocationService location
	{
		get
		{
			Debug.LogError("Define GPS_ENABLED to use this property");
			return null;
		}
	}

	private void Awake()
	{
		if (checkForControllers == null)
		{
			checkForControllers = StartCoroutine(CheckForControllers());
		}
		mogaManager = GameObject.Find("MogaControllerManager");
		if (mogaManager == null)
		{
			mogaManager = new GameObject("MogaControllerManager");
			mogaManager.AddComponent<Moga_ControllerManager>();
		}
	}

	private IEnumerator CheckForControllers()
	{
		while (true)
		{
			string[] controllers = Input.GetJoystickNames();
			if (!unityControllerConnected && controllers.Length > 0)
			{
				unityControllerConnected = true;
				Debug.Log("Controller Changed: Connected");
			}
			else if (unityControllerConnected && controllers.Length == 0)
			{
				unityControllerConnected = false;
				Debug.Log("Controller Changed: Disconnected");
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void Start()
	{
		mControllerManager = mogaManager.GetComponent<Moga_ControllerManager>().mogaControllerManager;
		RegisterMogaController();
	}

	private void Update()
	{
		foreach (Dictionary<int, float> value in mAxes.Values)
		{
			foreach (int item in new List<int>(value.Keys))
			{
				//value[item] = mControllerManager.getAxisValue(item);
			}
		}
		foreach (int item2 in new List<int>(mogaButtons.Keys))
		{
			/*int keyCode = mControllerManager.getKeyCode(item2);
			switch (mogaButtons[item2])
			{
			case ButtonState.RELEASED:
				if (keyCode == 0)
				{
					mogaButtons[item2] = ButtonState.PRESSING;
				}
				break;
			case ButtonState.PRESSING:
				if (keyCode == 1)
				{
					mogaButtons[item2] = ButtonState.RELEASING;
				}
				else
				{
					mogaButtons[item2] = ButtonState.PRESSED;
				}
				break;
			case ButtonState.PRESSED:
				mAnyKeyDown = true;
				mAnyKey = true;
				if (keyCode == 1)
				{
					mogaButtons[item2] = ButtonState.RELEASING;
				}
				break;
			case ButtonState.RELEASING:
				mAnyKeyDown = false;
				mAnyKey = false;
				if (keyCode == 0)
				{
					mogaButtons[item2] = ButtonState.PRESSING;
				}
				else
				{
					mogaButtons[item2] = ButtonState.RELEASED;
				}
				break;
			}*/
		}
	}

	public static void RegisterMogaController()
	{
		mogaManager = GameObject.Find("MogaControllerManager");
		if (mogaManager != null)
		{
			mogaControllerManager = mogaManager.GetComponent<Moga_ControllerManager>();
		}
		if (mogaControllerManager == null)
		{
			Debug.Log("MOGA Controller Manager could not be found.  Access the MOGA Menu to create one!");
		}
		else
		{
			MapController();
		}
	}

	private static void MapController()
	{
		RegisterInputKey(mogaControllerManager.p1ButtonA, 96);
		RegisterInputKey(mogaControllerManager.p1ButtonB, 97);
		RegisterInputKey(mogaControllerManager.p1ButtonX, 99);
		RegisterInputKey(mogaControllerManager.p1ButtonY, 100);
		RegisterInputKey(mogaControllerManager.p1ButtonL1, 102);
		RegisterInputKey(mogaControllerManager.p1ButtonR1, 103);
		RegisterInputKey(mogaControllerManager.p1ButtonSelect, 109);
		RegisterInputKey(mogaControllerManager.p1ButtonStart, 108);
		RegisterInputKey(mogaControllerManager.p1ButtonL3, 106);
		RegisterInputKey(mogaControllerManager.p1ButtonR3, 107);
		RegisterInputKey(mogaControllerManager.p1ButtonL2, 104);
		RegisterInputKey(mogaControllerManager.p1ButtonR2, 105);
		RegisterInputKey(mogaControllerManager.p1ButtonDPadUp, 19);
		RegisterInputKey(mogaControllerManager.p1ButtonDPadDown, 20);
		RegisterInputKey(mogaControllerManager.p1ButtonDPadLeft, 21);
		RegisterInputKey(mogaControllerManager.p1ButtonDPadRight, 22);
		RegisterInputAxis(mogaControllerManager.p1AxisHorizontal, 0);
		RegisterInputAxis(mogaControllerManager.p1AxisVertical, 1);
		RegisterInputAxis(mogaControllerManager.p1AxisLookHorizontal, 11);
		RegisterInputAxis(mogaControllerManager.p1AxisLookVertical, 14);
		RegisterInputAxis(mogaControllerManager.p1AxisL2, 17);
		RegisterInputAxis(mogaControllerManager.p1AxisR2, 18);
	}

	public static void RegisterInputAxis(string name, int axis)
	{
		Dictionary<int, float> value;
		if (!mAxes.TryGetValue(name, out value))
		{
			value = new Dictionary<int, float>();
			mAxes.Add(name, value);
		}
		if (!value.ContainsKey(axis))
		{
			value.Add(axis, 0f);
		}
		else
		{
			value[axis] = 0f;
		}
	}

	public static void RegisterInputButton(string name, int button)
	{
		if (!buttonStrings.ContainsKey(name))
		{
			buttonStrings.Add(name, button);
			mogaButtons.Add(button, ButtonState.RELEASED);
		}
	}

	public static void RegisterInputKey(KeyCode name, int buttonID)
	{
		if (!buttonKeyCodes.ContainsKey(name))
		{
			buttonKeyCodes.Add(name, buttonID);
			mogaButtons.Add(buttonID, ButtonState.RELEASED);
		}
	}

	public static bool GetControllerConnected()
	{
		if (mogaControllerManager != null)
		{
			if (mogaControllerManager.isControllerConnected() || unityControllerConnected)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool GetMogaControllerConnected()
	{
		if (mogaControllerManager != null)
		{
			return mogaControllerManager.isControllerConnected();
		}
		return false;
	}

	public static bool GetMogaExtended()
	{
		if (mogaControllerManager != null)
		{
			return mogaControllerManager.isControllerMogaPro();
		}
		return false;
	}

	public static AccelerationEvent GetAccelerationEvent(int index)
	{
		return Input.GetAccelerationEvent(index);
	}

	public static float GetAxis(string axisName)
	{
		Dictionary<int, float> value;
		if (mAxes.TryGetValue(axisName, out value))
		{
			foreach (float value2 in value.Values)
			{
				float num = value2;
				if (Math.Abs(num) > 0.19f)
				{
					return num;
				}
			}
		}
		return Input.GetAxis(axisName);
	}

	public static float GetAxisRaw(string axisName)
	{
		Dictionary<int, float> value;
		if (mAxes.TryGetValue(axisName, out value))
		{
			foreach (float value2 in value.Values)
			{
				float num = value2;
				if (Math.Abs(num) > 0.19f)
				{
					return num;
				}
			}
		}
		return Input.GetAxisRaw(axisName);
	}

	public static bool GetButton(string buttonName)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(buttonName, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.PRESSING || buttonState == ButtonState.PRESSED)
			{
				return true;
			}
		}
		return Input.GetButton(buttonName);
	}

	public static bool GetButtonDown(string buttonName)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(buttonName, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.PRESSING)
			{
				return true;
			}
		}
		return Input.GetButtonDown(buttonName);
	}

	public static bool GetButtonUp(string buttonName)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(buttonName, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.RELEASING)
			{
				return true;
			}
		}
		return Input.GetButtonUp(buttonName);
	}

	public static string[] GetJoystickNames()
	{
		return null;
	}

	public static bool GetKey(KeyCode key)
	{
		int value;
		ButtonState value2;
		if (buttonKeyCodes.TryGetValue(key, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.PRESSING || buttonState == ButtonState.PRESSED)
			{
				return true;
			}
		}
		return Input.GetKey(key);
	}

	public static bool GetKey(string name)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(name, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.PRESSING || buttonState == ButtonState.PRESSED)
			{
				return true;
			}
		}
		return Input.GetKey(name);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		int value;
		if (buttonKeyCodes.TryGetValue(key, out value))
		{
			keypressedHere = true;
			ButtonState value2;
			if (mogaButtons.TryGetValue(value, out value2))
			{
				ButtonState buttonState = value2;
				if (buttonState == ButtonState.PRESSING)
				{
					return true;
				}
			}
		}
		return Input.GetKeyDown(key);
	}

	public static bool GetKeyDown(string name)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(name, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.PRESSING)
			{
				return true;
			}
		}
		return Input.GetKeyDown(name);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		int value;
		ButtonState value2;
		if (buttonKeyCodes.TryGetValue(key, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.RELEASING)
			{
				return true;
			}
		}
		return Input.GetKeyUp(key);
	}

	public static bool GetKeyUp(string name)
	{
		int value;
		ButtonState value2;
		if (buttonStrings.TryGetValue(name, out value) && mogaButtons.TryGetValue(value, out value2))
		{
			ButtonState buttonState = value2;
			if (buttonState == ButtonState.RELEASING)
			{
				return true;
			}
		}
		return Input.GetKeyUp(name);
	}

	public static bool GetMouseButton(int button)
	{
		return Input.GetMouseButton(button);
	}

	public static bool GetMouseButtonDown(int button)
	{
		return Input.GetMouseButtonUp(button);
	}

	public static bool GetMouseButtonUp(int button)
	{
		return Input.GetMouseButtonUp(button);
	}

	public static Touch GetTouch(int index)
	{
		return Input.GetTouch(index);
	}

	public static void ResetInputAxes()
	{
		foreach (Dictionary<int, float> value in mAxes.Values)
		{
			foreach (int item in new List<int>(value.Keys))
			{
				value[item] = 0f;
			}
		}
		foreach (int item2 in new List<int>(mogaButtons.Keys))
		{
			mogaButtons[item2] = ButtonState.RELEASED;
		}
		Input.ResetInputAxes();
	}
}
