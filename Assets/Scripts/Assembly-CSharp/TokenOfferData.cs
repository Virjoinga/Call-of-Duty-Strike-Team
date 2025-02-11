using System.Collections.Generic;
using UnityEngine;

public class TokenOfferData
{
	private int[] m_HardCurrencyAmount;

	private uint m_Start;

	private uint m_Duration;

	private bool m_Enabled;

	private bool m_EnablediOS;

	private bool m_EnabledAndroid;

	private bool m_EnabledWP8;

	public bool InProgress
	{
		get
		{
			Debug.Log("TimeRemainingSeconds = " + TimeRemainingSeconds());
			return m_Enabled && TimeRemainingSeconds() != 0 && m_EnabledAndroid;
		}
	}

	public TokenOfferData(int numProducts)
	{
		m_HardCurrencyAmount = new int[numProducts];
		for (int i = 0; i < numProducts; i++)
		{
			m_HardCurrencyAmount[i] = 0;
		}
		m_Start = 0u;
		m_Duration = 0u;
		m_Enabled = true;
		m_EnablediOS = true;
		m_EnabledAndroid = true;
		m_EnabledWP8 = true;
	}

	public uint TimeRemainingSeconds()
	{
		uint? num = SynchronizedClock.Instance.SynchronizedTimeOrBestGuess;
		if (num.HasValue)
		{
			uint value = num.Value;
			uint num2 = m_Start + m_Duration;
			if (value >= m_Start && value <= num2)
			{
				return num2 - value;
			}
		}
		return 0u;
	}

	public int TokenOfferHardCurrencyAmount(int index)
	{
		TBFAssert.DoAssert(index >= 0 && index < m_HardCurrencyAmount.Length, "Invalid product index");
		return m_HardCurrencyAmount[index];
	}

	public bool ItemHasTokenOffer(int index)
	{
		TBFAssert.DoAssert(index >= 0 && index < m_HardCurrencyAmount.Length, "Invalid product index");
		return m_HardCurrencyAmount[index] != 0;
	}

	public void UpdateFromSwrve(int numProducts)
	{
		string itemId = "offer_tokens";
		Dictionary<string, string> resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources(itemId, out resourceDictionary) && resourceDictionary != null)
		{
			for (int i = 0; i < numProducts; i++)
			{
				m_HardCurrencyAmount[i] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "NewPackAmount0" + (i + 1), m_HardCurrencyAmount[i]);
			}
			m_Start = BedrockExtensions.GetFromResourceDictionaryAsUInt(resourceDictionary, "Start", m_Start);
			m_Duration = BedrockExtensions.GetFromResourceDictionaryAsUInt(resourceDictionary, "Duration", m_Duration);
			m_Enabled = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "Enabled", m_Enabled);
			m_EnablediOS = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "iOS", m_EnablediOS);
			m_EnabledAndroid = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "Android", m_EnabledAndroid);
			m_EnabledWP8 = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "WP8", m_EnabledWP8);
		}
	}
}
