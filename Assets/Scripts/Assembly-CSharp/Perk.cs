using System;

[Serializable]
public class Perk
{
	public PerkType Identifier = PerkType.None;

	public int Tier;

	public int UnlockCost;

	public int UnlockLevel;

	public int ProCost = 100;

	public float ModifierValue = 1f;

	public float ProModifierValue = 1f;

	public int ProXPTarget = 250000;

	public int Index
	{
		get
		{
			int num = 0;
			Perk[] perks = StatsManager.Instance.PerksList.Perks;
			for (int i = 0; i < perks.Length; i++)
			{
				if (perks[i].Tier == Tier)
				{
					num++;
					if (perks[i].Identifier == Identifier)
					{
						return num;
					}
				}
			}
			return 0;
		}
	}
}
