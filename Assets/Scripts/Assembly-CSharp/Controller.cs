using UnityEngine;

public static class Controller
{
	public enum ButtonId
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		LeftShoulder = 4,
		RightShoulder = 5,
		LeftTrigger = 6,
		RightTrigger = 7,
		DPadUp = 8,
		DPadDown = 9,
		DPadLeft = 10,
		DPadRight = 11,
		LeftThumbstickUp = 12,
		LeftThumbstickDown = 13,
		LeftThumbstickLeft = 14,
		LeftThumbstickRight = 15,
		RightThumbstickUp = 16,
		RightThumbstickDown = 17,
		RightThumbstickLeft = 18,
		RightThumbstickRight = 19
	}

	public enum DPadId
	{
		DPad = 0,
		LeftThumbstick = 1,
		RightThumbstick = 2
	}

	public struct Button
	{
		public bool pressed;

		public bool up;

		public bool down;

		public float value;
	}

	public struct DPad
	{
		public Button up;

		public Button down;

		public Button left;

		public Button right;

		public float x;

		public float y;
	}

	public struct State
	{
		public bool connected;

		public bool extended;

		public int playerIndex;

		public bool pause;

		public Button a;

		public Button b;

		public Button x;

		public Button y;

		public Button leftShoulder;

		public Button leftTrigger;

		public Button rightShoulder;

		public Button rightTrigger;

		public DPad dpad;

		public DPad leftThumbstick;

		public DPad rightThumbstick;

		public bool select;
	}

	public enum ControllerType
	{
		None = 0,
		Basic = 1,
		Extended = 2
	}

	private static State mState = default(State);

	private static bool m_Connected = false;

	public static void Update()
	{
		UpdateController(ref mState);
		if (mState.connected && !m_Connected)
		{
			m_Connected = true;
			if (!SecureStorage.Instance.ControllerHasBeenConnected)
			{
				SwrveEventsUI.ControllerFirstConnected();
				SecureStorage.Instance.ControllerHasBeenConnected = true;
			}
		}
	}

	public static State GetState()
	{
		return mState;
	}

	public static Button GetButton(State state, ButtonId id)
	{
		switch (id)
		{
		case ButtonId.A:
			return state.a;
		case ButtonId.B:
			return state.b;
		case ButtonId.X:
			return state.x;
		case ButtonId.Y:
			return state.y;
		case ButtonId.LeftShoulder:
			return state.leftShoulder;
		case ButtonId.RightShoulder:
			return state.rightShoulder;
		case ButtonId.LeftTrigger:
			return state.leftTrigger;
		case ButtonId.RightTrigger:
			return state.rightTrigger;
		case ButtonId.DPadUp:
			return state.dpad.up;
		case ButtonId.DPadDown:
			return state.dpad.down;
		case ButtonId.DPadLeft:
			return state.dpad.left;
		case ButtonId.DPadRight:
			return state.dpad.right;
		case ButtonId.LeftThumbstickUp:
			return state.leftThumbstick.up;
		case ButtonId.LeftThumbstickDown:
			return state.leftThumbstick.down;
		case ButtonId.LeftThumbstickLeft:
			return state.leftThumbstick.left;
		case ButtonId.LeftThumbstickRight:
			return state.leftThumbstick.right;
		case ButtonId.RightThumbstickUp:
			return state.rightThumbstick.up;
		case ButtonId.RightThumbstickDown:
			return state.rightThumbstick.down;
		case ButtonId.RightThumbstickLeft:
			return state.rightThumbstick.left;
		case ButtonId.RightThumbstickRight:
			return state.rightThumbstick.right;
		default:
			return default(Button);
		}
	}

	public static DPad GetDPad(State state, DPadId id)
	{
		switch (id)
		{
		case DPadId.DPad:
			return state.dpad;
		case DPadId.LeftThumbstick:
			return state.leftThumbstick;
		case DPadId.RightThumbstick:
			return state.rightThumbstick;
		default:
			return default(DPad);
		}
	}

	private static void UpdateController(ref State state)
	{
		if (GetControllerType() == ControllerType.Extended)
		{
			state.extended = true;
		}
		else
		{
			state.extended = false;
		}
		state.connected = Moga_Input.GetControllerConnected();
		EvaluateButton(ref state.a, KeyCode.JoystickButton0);
		EvaluateButton(ref state.b, KeyCode.JoystickButton1);
		EvaluateButton(ref state.x, KeyCode.JoystickButton2);
		EvaluateButton(ref state.y, KeyCode.JoystickButton3);
		EvaluateButton(ref state.leftShoulder, KeyCode.JoystickButton4);
		EvaluateButton(ref state.rightShoulder, KeyCode.JoystickButton5);
		EvaluateButton(ref state.dpad.left, KeyCode.LeftArrow);
		EvaluateButton(ref state.dpad.right, KeyCode.RightArrow);
		EvaluateButton(ref state.dpad.up, KeyCode.UpArrow);
		EvaluateButton(ref state.dpad.down, KeyCode.DownArrow);
		if (Moga_Input.GetKeyDown(KeyCode.Return))
		{
			state.pause = true;
		}
		else
		{
			state.pause = false;
		}
		if (Moga_Input.GetKeyDown(KeyCode.Escape))
		{
			state.select = true;
		}
		else
		{
			state.select = false;
		}
		state.leftThumbstick.x = Moga_Input.GetAxis("AndAxis1");
		state.leftThumbstick.y = 0f - Moga_Input.GetAxis("AndAxis2");
		state.rightThumbstick.x = Moga_Input.GetAxis("AndAxis3");
		state.rightThumbstick.y = 0f - Moga_Input.GetAxis("AndAxis4");
		EvaluateTrigger(ref state.leftTrigger, "AndAxis5");
		EvaluateTrigger(ref state.rightTrigger, "AndAxis6");
	}

	public static bool GetMogaConnected()
	{
		return Moga_Input.GetMogaControllerConnected();
	}

	private static void EvaluateTrigger(ref Button button, string axis)
	{
		float axis2 = Moga_Input.GetAxis(axis);
		if ((double)button.value <= 0.5 && (double)axis2 > 0.5)
		{
			button.down = true;
		}
		else
		{
			button.down = false;
		}
		if ((double)button.value >= 0.5 && (double)axis2 < 0.5)
		{
			button.up = true;
		}
		else
		{
			button.up = false;
		}
		if ((double)axis2 > 0.5)
		{
			button.pressed = true;
		}
		else
		{
			button.pressed = false;
		}
		button.value = axis2;
	}

	private static void EvaluateButton(ref Button button, KeyCode keycode)
	{
		button.up = Moga_Input.GetKeyUp(keycode);
		button.down = Moga_Input.GetKeyDown(keycode);
		button.pressed = Moga_Input.GetKey(keycode);
	}

	public static bool GetIsMogaBasic()
	{
		if (Moga_Input.GetMogaControllerConnected() && !Moga_Input.GetMogaExtended())
		{
			return true;
		}
		return false;
	}

	public static bool GetIsMogaPro()
	{
		if (Moga_Input.GetMogaControllerConnected() && Moga_Input.GetMogaExtended())
		{
			return true;
		}
		return false;
	}

	public static bool GetIsGenericGamepad()
	{
		if (!Moga_Input.GetMogaControllerConnected())
		{
			return true;
		}
		return false;
	}

	public static ControllerType GetControllerType()
	{
		if (Moga_Input.GetMogaControllerConnected())
		{
			if (Moga_Input.GetMogaExtended())
			{
				return ControllerType.Extended;
			}
			return ControllerType.Basic;
		}
		return ControllerType.Basic;
	}

	public static string GetManufacturer()
	{
		return null;
	}
}
