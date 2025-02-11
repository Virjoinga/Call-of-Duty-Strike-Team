using UnityEngine;

public class StatisticsPanel : MonoBehaviour
{
	public enum Type
	{
		SoldierAssault = 0,
		SoldierLmg = 1,
		SoldierShot = 2,
		SoldierSniper = 3,
		Rank = 4,
		Accuracy = 5,
		Kills = 6,
		Medals = 7,
		Squad = 8,
		Weapon = 9,
		MissionArctic01 = 10,
		MissionArctic02 = 11,
		MissionArctic03 = 12,
		MissionArctic04 = 13,
		MissionArctic05 = 14,
		MissionArctic06 = 15,
		MissionArcticGMG = 16,
		MissionArcticTA = 17,
		MissionArcticDOM = 18,
		MissionAfghan01 = 19,
		MissionAfghan02 = 20,
		MissionAfghan03 = 21,
		MissionAfghan04 = 22,
		MissionAfghan05 = 23,
		MissionAfghanGMG = 24,
		MissionAfghanTA = 25,
		MissionAfghanDOM = 26,
		MissionKowloon01 = 27,
		MissionKowloon02 = 28,
		MissionKowloon03 = 29,
		MissionKowloon04 = 30,
		MissionKowloon05 = 31,
		MissionKowloonGMG = 32,
		MissionKowloonTA = 33,
		MissionKowloonDOM = 34,
		MissionMoroccoGMG = 35,
		MissionMoroccoTA = 36,
		MissionMoroccoDOM = 37,
		MissionLocked = 38
	}

	public SpriteText TitleText;

	public SpriteText[] StatTexts;

	public SpriteText[] StatValues;

	public PackedSprite Image;

	public void SetData(string name, string[] texts, string[] values, Type type)
	{
		if (TitleText != null && StatTexts != null && StatValues != null && StatTexts.Length > 0)
		{
			int num = StatTexts.Length;
			FillOutSpriteText(TitleText, name);
			for (int i = 0; i < num; i++)
			{
				if (StatTexts.Length > i && StatValues.Length > i)
				{
					if (texts.Length > i)
					{
						FillOutSpriteText(StatTexts[i], texts[i]);
					}
					else
					{
						SetInactive(StatTexts[i]);
					}
					if (values.Length > i)
					{
						FillOutSpriteText(StatValues[i], values[i]);
					}
					else
					{
						SetInactive(StatValues[i]);
					}
				}
			}
		}
		if (Image != null)
		{
			Image.SetFrame(0, (int)type);
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

	private void FillOutSpriteText(SpriteText text, string strValue)
	{
		if (text != null)
		{
			text.Text = strValue;
		}
	}

	private void SetInactive(SpriteText text)
	{
		if (text != null)
		{
			text.Text = string.Empty;
			if (text.transform.parent != null)
			{
				SimpleSprite component = text.transform.parent.GetComponent<SimpleSprite>();
				Color color = component.Color;
				color.a *= 0.3f;
				component.SetColor(color);
			}
		}
	}
}
