using System;
using System.Collections.Generic;

public class ObjectPool
{
	private static Dictionary<Type, PoolableObject> pools = new Dictionary<Type, PoolableObject>();

	public static T New<T>() where T : IPoolable, new()
	{
		T val = default(T);
		PoolableObject poolableObject = null;
		if (!pools.ContainsKey(typeof(T)))
		{
			pools[typeof(T)] = new PoolableObject(16);
		}
		poolableObject = pools[typeof(T)];
		if (poolableObject.Count > 0)
		{
			val = (T)poolableObject.Pop();
		}
		else
		{
			val = new T();
			val.Create();
		}
		val.New();
		return val;
	}

	public static void Delete<T>(T obj) where T : IPoolable
	{
		if (pools.ContainsKey(typeof(T)))
		{
			obj.Delete();
			pools[typeof(T)].Push(obj);
			return;
		}
		throw new Exception("ObjectPool.Delete can not be called for object which is not created using ObjectPool.New");
	}

	public static void Clear()
	{
		foreach (PoolableObject value in pools.Values)
		{
			value.Clear();
		}
		pools.Clear();
	}
}
