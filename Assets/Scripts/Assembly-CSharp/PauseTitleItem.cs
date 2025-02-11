using UnityEngine;

public class PauseTitleItem : MonoBehaviour
{
	public SpriteText TitleText;

	public void LayoutComponents(string title, float width, float height)
	{
		if (TitleText != null)
		{
			TitleText.Text = AutoLocalize.Get(title);
		}
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}
}
