public abstract class SingleStat<T>
{
	public string Id { get; set; }

	public SingleStat()
	{
		Reset();
	}

	public abstract void CombineStat(T source);

	public abstract void Reset();

	public abstract void Save(string prefix);

	public abstract void Load(string prefix);

	public void Save(string prefix, ref int val, string key)
	{
		SecureStorage.Instance.SetInt(prefix + Id + key, val);
	}

	public void Load(string prefix, ref int val, string key)
	{
		string key2 = prefix + Id + key;
		if (SecureStorage.Instance.HasKey(key2))
		{
			val = SecureStorage.Instance.GetInt(key2);
		}
	}

	public void Save(string prefix, ref float val, string key)
	{
		SecureStorage.Instance.SetFloat(prefix + Id + key, val);
	}

	public void Load(string prefix, ref float val, string key)
	{
		string key2 = prefix + Id + key;
		if (SecureStorage.Instance.HasKey(key2))
		{
			val = SecureStorage.Instance.GetFloat(key2);
		}
	}

	public void Save(string prefix, ref bool val, string key)
	{
		SecureStorage.Instance.SetBool(prefix + Id + key, val);
	}

	public void Load(string prefix, ref bool val, string key)
	{
		string key2 = prefix + Id + key;
		if (SecureStorage.Instance.HasKey(key2))
		{
			val = SecureStorage.Instance.GetBool(key2);
		}
	}
}
