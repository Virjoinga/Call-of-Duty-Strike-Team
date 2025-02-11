using System.Collections.Generic;

public static class SaveLoadHelper
{
	public static void SaveArray<T>(string prefix, T[] array) where T : ISaveLoadNamed
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Save(prefix + i);
		}
	}

	public static void LoadArray<T>(string prefix, T[] array) where T : ISaveLoadNamed
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Load(prefix + i);
		}
	}

	public static void ResetArray<T>(T[] array) where T : ISaveLoadNamed
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Reset();
		}
	}

	public static void SaveStringList(string key, List<string> items)
	{
		SecureStorage.Instance.SetInt(key + ".num", items.Count);
		for (int i = 0; i < items.Count; i++)
		{
			SecureStorage.Instance.SetString(key + i, items[i]);
		}
	}

	public static void LoadStringList(string key, List<string> items)
	{
		int @int = SecureStorage.Instance.GetInt(key + ".num");
		items.Clear();
		for (int i = 0; i < @int; i++)
		{
			items.Add(SecureStorage.Instance.GetString(key + i, string.Empty));
		}
	}

	public static void ResetArray(bool[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = false;
		}
	}

	public static void SaveArray(string prefix, bool[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			SecureStorage.Instance.SetBool(prefix + i, array[i]);
		}
	}

	public static void LoadArray(string prefix, bool[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = SecureStorage.Instance.GetBool(prefix + i);
		}
	}

	public static void ResetArray(int[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
	}

	public static void SaveArray(string prefix, int[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			SecureStorage.Instance.SetInt(prefix + i, array[i]);
		}
	}

	public static void LoadArray(string prefix, int[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = SecureStorage.Instance.GetInt(prefix + i);
		}
	}
}
