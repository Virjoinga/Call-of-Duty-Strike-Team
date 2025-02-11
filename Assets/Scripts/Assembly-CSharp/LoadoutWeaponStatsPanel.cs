using UnityEngine;

public class LoadoutWeaponStatsPanel : MonoBehaviour
{
	private const float STATS_START = 0.52f;

	private const float STAT_TEXT_WIDTH = 0.4f;

	private const float STAT_BAR_WIDTH = 0.6f;

	private const float STAT_TOTAL_HEIGHT = 0.35f;

	public Stat[] Stats;

	public CommonBackgroundBox Box;

	private void Awake()
	{
		int childCount = base.transform.childCount;
		float num = 0.52f;
		if (childCount != Stats.Length)
		{
			Debug.LogWarning("Load-out weapon stats panel has incorrect number of stats or child objects");
		}
		float num2 = ((Stats.Length <= 0) ? 0.35f : (0.35f / (float)Stats.Length));
		for (int i = 0; i < childCount && i < Stats.Length; i++)
		{
			Transform child = base.transform.GetChild(i);
			CommonBackgroundBoxPlacement placement = null;
			CommonBackgroundBoxPlacement placement2 = null;
			CommonBackgroundBoxPlacement placement3 = null;
			CommonBackgroundBoxPlacement placement4 = null;
			ProgressBar[] componentsInChildren = child.GetComponentsInChildren<ProgressBar>();
			ProgressBar[] array = componentsInChildren;
			foreach (ProgressBar progressBar in array)
			{
				if (progressBar.name == "ProgressBarLarge")
				{
					placement2 = progressBar.transform.parent.GetComponent<CommonBackgroundBoxPlacement>();
					Stats[i].ProgressBarSelected = progressBar;
				}
				else if (progressBar.name == "ProgressBarSmall")
				{
					placement3 = progressBar.transform.parent.GetComponent<CommonBackgroundBoxPlacement>();
					Stats[i].ProgressBarCurrent = progressBar;
				}
			}
			SimpleSprite[] componentsInChildren2 = child.GetComponentsInChildren<SimpleSprite>();
			SimpleSprite[] array2 = componentsInChildren2;
			foreach (SimpleSprite simpleSprite in array2)
			{
				if (simpleSprite.name == "ProgressBackground")
				{
					placement = simpleSprite.GetComponent<CommonBackgroundBoxPlacement>();
				}
				else if (simpleSprite.name == "TextSubtitle")
				{
					placement4 = simpleSprite.GetComponent<CommonBackgroundBoxPlacement>();
				}
			}
			SpriteText componentInChildren = child.GetComponentInChildren<SpriteText>();
			if (componentInChildren != null)
			{
				componentInChildren.Text = AutoLocalize.Get(Stats[i].StringKey);
			}
			Position(placement, num, num2);
			Position(placement2, num, num2 * 0.6f);
			Position(placement3, num + num2 * 0.6f, num2 * 0.3f);
			Position(placement4, num, num2);
			num += num2;
		}
		if (Box != null)
		{
			Box.RefreshPlacements = true;
		}
	}

	public void Populate(WeaponDescriptor weaponData, WeaponDescriptor currentWeaponData)
	{
		for (int i = 0; i < Stats.Length; i++)
		{
			if (Stats[i].ProgressBarSelected != null && Stats[i].ProgressBarCurrent != null)
			{
				float value = 0f;
				float num = 0f;
				switch (Stats[i].Type)
				{
				case Stat.StatType.Accuracy:
					value = weaponData.Loadout.Accuracy;
					num = currentWeaponData.Loadout.Accuracy;
					break;
				case Stat.StatType.Damage:
					value = weaponData.Loadout.Damage;
					num = currentWeaponData.Loadout.Damage;
					break;
				case Stat.StatType.Range:
					value = weaponData.Loadout.Range;
					num = currentWeaponData.Loadout.Range;
					break;
				case Stat.StatType.FireRate:
					value = weaponData.Loadout.RateOfFire;
					num = currentWeaponData.Loadout.RateOfFire;
					break;
				case Stat.StatType.Mobility:
					value = weaponData.Loadout.Mobility;
					num = currentWeaponData.Loadout.Mobility;
					break;
				}
				Stats[i].ProgressBarSelected.SetValue(value);
				Stats[i].ProgressBarCurrent.SetValue((!(weaponData != currentWeaponData)) ? 0f : num);
			}
		}
	}

	private void Position(CommonBackgroundBoxPlacement placement, float y, float h)
	{
		if (placement != null)
		{
			placement.StartPositionAsPercentageOfBoxHeight = y;
			placement.HeightAsPercentageOfBoxHeight = h;
		}
	}
}
