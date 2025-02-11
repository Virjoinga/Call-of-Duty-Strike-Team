using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableAndroidOptLevel
{
	[SerializeField]
	public List<string> keys = new List<string>();

	[SerializeField]
	public List<PhoneDetails> values = new List<PhoneDetails>();

	private Dictionary<string, PhoneDetails> phoneDictionary = new Dictionary<string, PhoneDetails>();

	public Dictionary<string, PhoneDetails> PhoneDictionary
	{
		get
		{
			return phoneDictionary;
		}
		set
		{
			phoneDictionary = value;
			Serialize();
		}
	}

	public void Deserialize()
	{
		phoneDictionary = new Dictionary<string, PhoneDetails>();
		for (int i = 0; i < keys.Count; i++)
		{
			phoneDictionary.Add(keys[i].ToLowerInvariant(), values[i]);
		}
	}

	public void Serialize()
	{
		keys = new List<string>();
		values = new List<PhoneDetails>();
		foreach (string key in phoneDictionary.Keys)
		{
			keys.Add(key.ToLowerInvariant());
			values.Add(phoneDictionary[key]);
		}
	}

	public void Add(string key, PhoneDetails val)
	{
		key = key.ToLowerInvariant();
		if (!phoneDictionary.ContainsKey(key))
		{
			phoneDictionary.Add(key, val);
		}
		else
		{
			Debug.Log("Duplicate android versions");
		}
		if (!keys.Contains(key))
		{
			keys.Add(key);
			values.Add(val);
		}
		else
		{
			Debug.Log("Duplicate android versions");
		}
	}
}
