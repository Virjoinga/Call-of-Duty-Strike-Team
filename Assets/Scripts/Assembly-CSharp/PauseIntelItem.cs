using UnityEngine;

public class PauseIntelItem : MonoBehaviour
{
	public SpriteText DescriptionText;

	public void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public void Setup(MissionData data)
	{
		int currentSection = ActStructure.Instance.CurrentSection;
		string textKey = "S_TOTAL_INTEL";
		int num = StatsHelper.IntelCollectedForCurrentMission();
		int num2 = ((data != null && currentSection < data.Sections.Count) ? data.Sections[currentSection].IntelToCollect : 0);
		string arg = num + "/" + num2;
		if (DescriptionText != null)
		{
			DescriptionText.Text = string.Format(AutoLocalize.Get(textKey), arg);
		}
	}

	public void LayoutComponents(float width, float height)
	{
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}
}
