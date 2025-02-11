using UnityEngine;

public class Cash : ISaveLoad
{
	private const string SoftCashKey = "Softcash";

	private const string HardCashKey = "HardCash";

	private const string TotalHardCashEverPurchasedKey = "TotalHardCashEver";

	private const string TotalHardCashEverSpentKey = "TotalHardCashSpent";

	private const string TotalHardCashEverAwardedKey = "TotalHardCashAwarded";

	private int m_SoftCash;

	private int m_HardCash;

	private int m_TotalHardCashEverPurchased;

	private int m_TotalHardCashEverSpent;

	private int m_TotalHardCashEverAwarded;

	public int AwardHardCashFreebie(int amount, string name)
	{
		TBFAssert.DoAssert(amount >= 0);
		if (amount > 0)
		{
			SwrveEventsPurchase.HardCurrencyAwarded((ulong)amount, name);
		}
		return AdjustHardCash(amount, false, true);
	}

	public int AdjustHardCash(int amount)
	{
		return AdjustHardCash(amount, false, false);
	}

	public int AwardHardCash(int amount, string name)
	{
		TBFAssert.DoAssert(amount >= 0);
		if (amount > 0)
		{
			SwrveEventsPurchase.HardCurrencyAwarded((ulong)amount, name);
		}
		return AdjustHardCash(amount, false, false);
	}

	public int AdjustHardCashFromStore(int amount)
	{
		return AdjustHardCash(amount, true, false);
	}

	private int AdjustHardCash(int amount, bool fromPurchase, bool freebie)
	{
		m_HardCash = Mathf.Max(0, m_HardCash + amount);
		EventHub.Instance.Report(new Events.HardCurrencyChanged(amount, fromPurchase, freebie));
		if (amount > 0)
		{
			if (fromPurchase)
			{
				m_TotalHardCashEverPurchased += amount;
			}
			else
			{
				m_TotalHardCashEverAwarded += amount;
			}
		}
		else
		{
			m_TotalHardCashEverSpent -= amount;
		}
		SecureStorage.Instance.SaveGameSettings();
		return m_HardCash;
	}

	public int SoftCash()
	{
		return m_SoftCash;
	}

	public int HardCash()
	{
		return m_HardCash;
	}

	public int TotalHardCashEverPurchased()
	{
		return m_TotalHardCashEverPurchased;
	}

	public int TotalHardCashEverSpent()
	{
		return m_TotalHardCashEverSpent;
	}

	public int TotalHardCashEverAwarded()
	{
		return m_TotalHardCashEverAwarded;
	}

	public void Save()
	{
		SecureStorage.Instance.SetInt("Softcash", m_SoftCash);
		SecureStorage.Instance.SetInt("HardCash", m_HardCash);
		SecureStorage.Instance.SetInt("TotalHardCashEver", m_TotalHardCashEverPurchased);
		SecureStorage.Instance.SetInt("TotalHardCashSpent", m_TotalHardCashEverSpent);
		SecureStorage.Instance.SetInt("TotalHardCashAwarded", m_TotalHardCashEverAwarded);
	}

	public void Load()
	{
		m_SoftCash = SecureStorage.Instance.GetInt("Softcash", m_SoftCash);
		m_HardCash = SecureStorage.Instance.GetInt("HardCash", m_HardCash);
		m_TotalHardCashEverPurchased = SecureStorage.Instance.GetInt("TotalHardCashEver");
		m_TotalHardCashEverSpent = SecureStorage.Instance.GetInt("TotalHardCashSpent");
		m_TotalHardCashEverAwarded = SecureStorage.Instance.GetInt("TotalHardCashAwarded");
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableKInvite) && SwrveServerVariables.Instance.KInviteEnabled)
		{
			TBFUtils.CheckKInviteForRewards();
		}
	}

	public void Reset()
	{
		m_SoftCash = 0;
		m_HardCash = 0;
		m_TotalHardCashEverPurchased = 0;
		m_TotalHardCashEverSpent = 0;
		m_TotalHardCashEverAwarded = 0;
	}
}
