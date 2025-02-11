using System;
using System.Collections.Generic;

public class PerksManager : StatsManagerUpdatable, iSwrveUpdatable
{
	private const string PerkStatusKey = "PerkStatus";

	private EventLog m_PerksLog = new EventLog();

	public int[] TierUnlock;

	private PerkStatus[] m_CurrentPerkStatus;

	public EventLog Log()
	{
		return m_PerksLog;
	}

	public override void Init()
	{
		m_CurrentPerkStatus = new PerkStatus[23];
		for (int i = 0; i < m_CurrentPerkStatus.Length; i++)
		{
			m_CurrentPerkStatus[i] = new PerkStatus();
		}
	}

	public Perk GetPerk(PerkType type)
	{
		return StatsManager.Instance.PerksList.GetPerk(type);
	}

	public float GetModifierForPerk(PerkType type)
	{
		Perk perk = GetPerk(type);
		GameSettings instance = GameSettings.Instance;
		if (instance.HasPerk(type))
		{
			if (GetPerkStatus(type).ProXP >= perk.ProXPTarget || instance.WasProPerkUnlockedEarly(type))
			{
				return perk.ProModifierValue;
			}
			return perk.ModifierValue;
		}
		return 1f;
	}

	public PerkStatus GetPerkStatus(PerkType type)
	{
		return m_CurrentPerkStatus[(int)type];
	}

	public override void SessionEnd()
	{
		foreach (int value in Enum.GetValues(typeof(PerkType)))
		{
			if (value == 23 || value == -1)
			{
				continue;
			}
			PerkStatus perkStatus = m_CurrentPerkStatus[value];
			perkStatus.ReachedProThisMission = false;
			for (int i = 0; i < 4; i++)
			{
				if (GameSettings.Instance.HasPerk((PerkType)value))
				{
					int proXP = perkStatus.ProXP;
					int proXPTarget = GetPerk((PerkType)value).ProXPTarget;
					string id = StatsManager.ConvertSoldierIndexToId(i);
					perkStatus.ProXP += StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(id).TotalXP;
					if (proXP < proXPTarget && perkStatus.ProXP >= proXPTarget && !GameSettings.Instance.WasProPerkUnlockedEarly((PerkType)value))
					{
						perkStatus.ReachedProThisMission = true;
					}
				}
			}
		}
	}

	public bool ReachedProThisMission(PerkType p)
	{
		return m_CurrentPerkStatus[(int)p].ReachedProThisMission;
	}

	public override void Reset()
	{
		for (int i = 0; i < m_CurrentPerkStatus.Length; i++)
		{
			m_CurrentPerkStatus[i].Reset();
		}
	}

	protected override void SetEventListeners()
	{
	}

	public void CheckPerksUnlockedByXP(int xpBefore, int xpAfter)
	{
		Perk[] perks = StatsManager.Instance.PerksList.Perks;
		foreach (Perk perk in perks)
		{
			if (xpBefore < GetPerk(perk.Identifier).ProXPTarget && xpAfter >= GetPerk(perk.Identifier).ProXPTarget)
			{
				EventHub.Instance.Report(new Events.PerkUnlocked(perk.Identifier));
			}
		}
	}

	public override void Save()
	{
		SaveLoadHelper.SaveArray("PerkStatus", m_CurrentPerkStatus);
	}

	public override void Load()
	{
		SaveLoadHelper.LoadArray("PerkStatus", m_CurrentPerkStatus);
	}

	public void UpdateFromSwrve()
	{
		Perk[] perks = StatsManager.Instance.PerksList.Perks;
		foreach (Perk perk in perks)
		{
			UpdatePerk(perk);
		}
	}

	private void UpdatePerk(Perk perk)
	{
		string itemId = "Perk_" + perk.Identifier;
		Dictionary<string, string> resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources(itemId, out resourceDictionary) && resourceDictionary != null)
		{
			perk.UnlockCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Cost", perk.UnlockCost);
			perk.ProCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "ProCost", perk.ProCost);
			perk.ModifierValue = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "Modifier", perk.ModifierValue);
			perk.ProModifierValue = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "ProModifier", perk.ProModifierValue);
		}
	}
}
