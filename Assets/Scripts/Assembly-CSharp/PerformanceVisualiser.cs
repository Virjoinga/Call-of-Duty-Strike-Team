using UnityEngine;

public class PerformanceVisualiser : MonoBehaviour
{
	public static PerformanceVisualiser instance;

	private bool mHidden;

	public static PerformanceVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		mHidden = true;
	}

	private void Update()
	{
		if (!mHidden)
		{
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
}
