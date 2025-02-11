using UnityEngine;

public class PageIndicator : MonoBehaviour
{
	public float Spacing = 1f;

	public Color InFocus = new Color(0.79607844f, 63f / 85f, 0.20392157f, 0.5f);

	public Color NotInFocus = new Color(1f, 1f, 1f, 0.3f);

	private UIButton[] Buttons;

	private UIButton PageButton;

	private Scale9Grid BackgroundBox;

	private void Awake()
	{
		PageButton = GetComponentInChildren<UIButton>();
		BackgroundBox = GetComponentInChildren<Scale9Grid>();
	}

	public void Setup(int numPages)
	{
		float num = (float)numPages * Spacing;
		float num2 = 0f - num * 0.5f + Spacing * 0.5f;
		EZScreenPlacement component = PageButton.GetComponent<EZScreenPlacement>();
		component.renderCamera = Camera.main;
		Buttons = new UIButton[numPages];
		Buttons[0] = PageButton;
		component.screenPos.x = num2;
		component.PositionOnScreenRecursively();
		for (int i = 1; i < numPages; i++)
		{
			num2 += Spacing;
			UIButton uIButton = (UIButton)Object.Instantiate(PageButton);
			uIButton.transform.parent = PageButton.transform.parent;
			Buttons[i] = uIButton;
			EZScreenPlacement component2 = uIButton.GetComponent<EZScreenPlacement>();
			component2.screenPos.x = num2;
			component2.PositionOnScreenRecursively();
		}
		BackgroundBox.size.x = num;
		BackgroundBox.Resize();
	}

	public void SetPage(int index)
	{
		int num = Buttons.Length;
		for (int i = 0; i < num; i++)
		{
			UpdatePageIndicator(Buttons[i], i == index);
		}
	}

	private void UpdatePageIndicator(UIButton pageIndicator, bool inFocus)
	{
		if (pageIndicator != null)
		{
			pageIndicator.SetColor((!inFocus) ? NotInFocus : InFocus);
		}
	}
}
