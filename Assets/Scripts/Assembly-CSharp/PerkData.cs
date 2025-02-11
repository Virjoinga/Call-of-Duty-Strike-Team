using System;
using System.Collections.Generic;

[Serializable]
public class PerkData
{
	public Perk[] Perks;

	private List<Perk> mTier1Perks;

	private List<Perk> mTier2Perks;

	private List<Perk> mTier3Perks;

	private List<Perk> mTier4Perks;

	public void SortIntoTiers()
	{
		mTier1Perks = new List<Perk>();
		mTier2Perks = new List<Perk>();
		mTier3Perks = new List<Perk>();
		mTier4Perks = new List<Perk>();
		Perk[] perks = Perks;
		foreach (Perk perk in perks)
		{
			if (perk.Tier == 1)
			{
				mTier1Perks.Add(perk);
			}
			else if (perk.Tier == 2)
			{
				mTier2Perks.Add(perk);
			}
			else if (perk.Tier == 3)
			{
				mTier3Perks.Add(perk);
			}
			else if (perk.Tier == 4)
			{
				mTier4Perks.Add(perk);
			}
		}
	}

	public Perk GetPerk(PerkType type)
	{
		if (type == PerkType.None)
		{
			return null;
		}
		TBFAssert.DoAssert(Perks[(int)type].Identifier == type);
		return Perks[(int)type];
	}

	public Perk[] GetPerksForTier(int tier)
	{
		Perk[] result = null;
		switch (tier)
		{
		case 1:
			if (mTier1Perks == null)
			{
				return null;
			}
			result = mTier1Perks.ToArray();
			break;
		case 2:
			if (mTier2Perks == null)
			{
				return null;
			}
			result = mTier2Perks.ToArray();
			break;
		case 3:
			if (mTier3Perks == null)
			{
				return null;
			}
			result = mTier3Perks.ToArray();
			break;
		case 4:
			if (mTier4Perks == null)
			{
				return null;
			}
			result = mTier4Perks.ToArray();
			break;
		}
		return result;
	}
}
