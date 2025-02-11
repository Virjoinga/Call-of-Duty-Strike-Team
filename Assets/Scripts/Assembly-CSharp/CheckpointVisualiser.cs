using UnityEngine;

public class CheckpointVisualiser : MonoBehaviour
{
	public static CheckpointVisualiser instance;

	private bool mHidden;

	public static CheckpointVisualiser Instance()
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
			string text = "Checkpoint Visualiser";
			Rect position = new Rect(Screen.width - 230, Screen.height - 270, 200f, 250f);
			GUI.Box(position, text);
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
