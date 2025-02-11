using UnityEngine;

public class Moga_Controller
{
	public const int ACTION_DOWN = 0;

	public const int ACTION_UP = 1;

	public const int ACTION_FALSE = 0;

	public const int ACTION_TRUE = 1;

	public const int ACTION_DISCONNECTED = 0;

	public const int ACTION_CONNECTED = 1;

	public const int ACTION_CONNECTING = 2;

	public const int ACTION_VERSION_MOGA = 0;

	public const int ACTION_VERSION_MOGAPRO = 1;

	public const int MODE_MULTI_CONTROLLER = 1;

	public const int MODE_HID_TO_MOGA = 2;

	public const int KEYCODE_DPAD_UP = 19;

	public const int KEYCODE_DPAD_DOWN = 20;

	public const int KEYCODE_DPAD_LEFT = 21;

	public const int KEYCODE_DPAD_RIGHT = 22;

	public const int KEYCODE_BUTTON_A = 96;

	public const int KEYCODE_BUTTON_B = 97;

	public const int KEYCODE_BUTTON_X = 99;

	public const int KEYCODE_BUTTON_Y = 100;

	public const int KEYCODE_BUTTON_L1 = 102;

	public const int KEYCODE_BUTTON_R1 = 103;

	public const int KEYCODE_BUTTON_L2 = 104;

	public const int KEYCODE_BUTTON_R2 = 105;

	public const int KEYCODE_BUTTON_THUMBL = 106;

	public const int KEYCODE_BUTTON_THUMBR = 107;

	public const int KEYCODE_BUTTON_START = 108;

	public const int KEYCODE_BUTTON_SELECT = 109;

	public const int INFO_UNKNOWN = 0;

	public const int INFO_KNOWN_DEVICE_COUNT = 1;

	public const int INFO_ACTIVE_DEVICE_COUNT = 2;

	public const int INFO_BLUETOOTH_ENABLED = 3;

	public const int AXIS_X = 0;

	public const int AXIS_Y = 1;

	public const int AXIS_Z = 11;

	public const int AXIS_RZ = 14;

	public const int AXIS_HAT_X = 15;

	public const int AXIS_HAT_Y = 16;

	public const int AXIS_LTRIGGER = 17;

	public const int AXIS_RTRIGGER = 18;

	public const int STATE_UNKNOWN = 0;

	public const int STATE_CONNECTION = 1;

	public const int STATE_POWER_LOW = 2;

	public const int STATE_SUPPORTED_PRODUCT_VERSION = 3;

	public const int STATE_CURRENT_PRODUCT_VERSION = 4;

	private readonly AndroidJavaObject mController;

	public Moga_Controller(AndroidJavaObject activity, AndroidJavaObject controller)
	{
		mController = controller;
	}

	public static AndroidJavaObject getInstance(AndroidJavaObject activity)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.bda.controller.Controller"))
		{
			return androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[1] { activity });
		}
	}

	public bool init()
	{
		return mController.Call<bool>("init", new object[0]);
	}

	public void enableModes(int mode)
	{
		mController.Call("enableMode", mode);
	}

	public bool isModeEnabled()
	{
		return mController.Call<bool>("isModeEnabled", new object[0]);
	}

	public void disableModes(int mode)
	{
		mController.Call("disableModes", mode);
	}

	public void allowNewConnections()
	{
		mController.Call("allowNewConnections");
	}

	public bool isAllowingNewConnections()
	{
		return mController.Call<bool>("isAllowingNewConnections", new object[0]);
	}

	public void disallowNewConnections()
	{
		mController.Call("disallowNewConnections");
	}

	public void showLobby(int minPlayers, int maxPlayers)
	{
		mController.Call("showLobby", minPlayers, maxPlayers);
	}

	public bool isLobbyVisible()
	{
		return mController.Call<bool>("isDialogShowing", new object[0]);
	}

	public void setLobbyMinMax(int minPlayers, int maxPlayers)
	{
		mController.Call("setLobbyMinMax", minPlayers, maxPlayers);
	}

	public float getAxisValue(int controllerID, int axis)
	{
		return mController.Call<float>("getAxisValue", new object[2] { controllerID, axis });
	}

	public float getAxisValue(int axis)
	{
		return mController.Call<float>("getAxisValue", new object[2] { 1, axis });
	}

	public int getKeyCode(int controllerID, int keyCode)
	{
		return mController.Call<int>("getKeyCode", new object[2] { controllerID, keyCode });
	}

	public int getKeyCode(int keyCode)
	{
		return mController.Call<int>("getKeyCode", new object[2] { 1, keyCode });
	}

	public int getInfo(int controllerID, int info)
	{
		return mController.Call<int>("getInfo", new object[2] { controllerID, info });
	}

	public int getInfo(int info)
	{
		return mController.Call<int>("getInfo", new object[2] { 1, info });
	}

	public int getState(int controllerID, int state)
	{
		return mController.Call<int>("getState", new object[2] { controllerID, state });
	}

	public int getState(int state)
	{
		return mController.Call<int>("getState", new object[2] { 1, state });
	}

	public int getControllerModel(int controllerID)
	{
		return mController.Call<int>("getState", new object[2] { controllerID, 4 });
	}

	public int getControllerModel()
	{
		return mController.Call<int>("getState", new object[2] { 1, 4 });
	}

	public void onPause()
	{
		mController.Call("onPause");
	}

	public void onResume()
	{
		mController.Call("onResume");
	}

	public void onExit()
	{
		mController.Call("onExit");
	}
}
