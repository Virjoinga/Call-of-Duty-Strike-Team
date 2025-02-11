using UnityEngine;

public class ZoomToggleButton : MonoBehaviour
{
	private PackedSprite mIcon;

	private void Awake()
	{
		mIcon = GetComponent<PackedSprite>();
		SetZoomButtonFrame();
	}

	private void Start()
	{
		SetZoomButtonFrame();
	}

	public void OnEnable()
	{
		SetZoomButtonFrame();
	}

	private void SetZoomButtonFrame()
	{
		if (mIcon != null && GameController.Instance != null)
		{
			mIcon.SetFrame(0, GameController.Instance.IsFirstPerson ? 1 : 0);
		}
	}
}
