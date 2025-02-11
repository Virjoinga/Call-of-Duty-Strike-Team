using UnityEngine;

public class Moga_ControllerManager : MonoBehaviour
{
	[HideInInspector]
	public KeyCode p1ButtonA = KeyCode.JoystickButton0;

	[HideInInspector]
	public KeyCode p1ButtonB = KeyCode.JoystickButton1;

	[HideInInspector]
	public KeyCode p1ButtonX = KeyCode.JoystickButton2;

	[HideInInspector]
	public KeyCode p1ButtonY = KeyCode.JoystickButton3;

	[HideInInspector]
	public KeyCode p1ButtonL1 = KeyCode.JoystickButton4;

	[HideInInspector]
	public KeyCode p1ButtonR1 = KeyCode.JoystickButton5;

	[HideInInspector]
	public KeyCode p1ButtonSelect = KeyCode.Escape;

	[HideInInspector]
	public KeyCode p1ButtonStart = KeyCode.Return;

	[HideInInspector]
	public KeyCode p1ButtonL3 = KeyCode.JoystickButton8;

	[HideInInspector]
	public KeyCode p1ButtonR3 = KeyCode.JoystickButton9;

	[HideInInspector]
	public KeyCode p1ButtonL2 = KeyCode.JoystickButton6;

	[HideInInspector]
	public KeyCode p1ButtonR2 = KeyCode.JoystickButton7;

	[HideInInspector]
	public KeyCode p1ButtonDPadUp;

	[HideInInspector]
	public KeyCode p1ButtonDPadDown;

	[HideInInspector]
	public KeyCode p1ButtonDPadLeft;

	[HideInInspector]
	public KeyCode p1ButtonDPadRight;

	[HideInInspector]
	public string p1AxisHorizontal = "AndAxis1";

	[HideInInspector]
	public string p1AxisVertical = "AndAxis2";

	[HideInInspector]
	public string p1AxisLookHorizontal = "AndAxis3";

	[HideInInspector]
	public string p1AxisLookVertical = "AndAxis4";

	[HideInInspector]
	public string p1AxisL2 = "AndAxis5";

	[HideInInspector]
	public string p1AxisR2 = "AndAxis6";

	[HideInInspector]
	public Moga_Controller mogaControllerManager;

	private bool mFocused;

	private void Awake()
	{
		if (mogaControllerManager == null)
		{
			Object.DontDestroyOnLoad(base.transform.gameObject);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject instance = Moga_Controller.getInstance(@static);
			mogaControllerManager = new Moga_Controller(@static, instance);
			mogaControllerManager.init();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		mFocused = focus;
		if (mFocused)
		{
			if (mogaControllerManager != null)
			{
				mogaControllerManager.onResume();
			}
		}
		else if (!mogaControllerManager.isLobbyVisible() && mogaControllerManager != null)
		{
			mogaControllerManager.onPause();
		}
	}

	private void OnDestroy()
	{
		if (mogaControllerManager != null)
		{
			mogaControllerManager.onExit();
		}
	}

	public bool isControllerConnected()
	{
		if (mogaControllerManager != null)
		{
			if (mogaControllerManager.getState(1) == 1)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public int numberOfConnectedControllers()
	{
		return mogaControllerManager.getInfo(1, 1);
	}

	public bool isControllerMogaPro()
	{
		if (mogaControllerManager.getState(4) == 1)
		{
			return true;
		}
		return false;
	}
}
