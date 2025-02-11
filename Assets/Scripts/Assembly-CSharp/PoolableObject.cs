using System;
using System.Collections.Generic;

public class PoolableObject
{
	private Stack<IPoolable> pool;

	public int Count
	{
		get
		{
			return pool.Count;
		}
	}

	public PoolableObject(int capacity)
	{
		pool = new Stack<IPoolable>(capacity);
	}

	public IPoolable Pop()
	{
		if (pool.Count > 0)
		{
			return pool.Pop();
		}
		return null;
	}

	public void Push(IPoolable obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("Items added to a Pool cannot be null");
		}
		pool.Push(obj);
	}

	public void Clear()
	{
		pool.Clear();
	}
}
