using UnityEngine;

public class ContextMenuVisualiser : MonoBehaviour
{
	public static ContextMenuVisualiser instance;

	public static bool ContextMenuDebugOptions;

	private static int BUTTON_INDENT_WIDTH = 10;

	private static int BUTTON_HEIGHT = 25;

	private bool mHidden;

	public static ContextMenuVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (!mHidden)
		{
			string text = "Context Menu Visualiser";
			Rect position = new Rect(Screen.width - 500, Screen.height - 80, 480f, 60f);
			GUI.Box(position, text);
			if (DoButton(position.x + (float)BUTTON_INDENT_WIDTH, position.y + (float)BUTTON_HEIGHT * 1f, (position.width - (float)(BUTTON_INDENT_WIDTH * 2)) * 0.45f, BUTTON_HEIGHT, "Toggle Debug Options"))
			{
				ContextMenuDebugOptions = !ContextMenuDebugOptions;
			}
		}
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

	private bool DoButton(float x, float y, float width, float height, string label)
	{
		Rect position = new Rect(x, y, width, height);
		return GUI.Button(position, label);
	}
}
